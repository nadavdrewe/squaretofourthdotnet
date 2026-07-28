using CsvHelper;
using domain.pipeline.fourth.com.Helper;
using domain.pipeline.fourth.com.SalesFactories;
using domain.pipeline.fourth.com.SalesFactories.Helper;
using domain.pipeline.fourth.com.SalesFactories.SalesRowFactories;
using domain.pipeline.fourth.com.Square.SalesFactories.Helper;
using domain.pipeline.fourth.com.Square.SalesFactories.SalesRowFactories.DiscountAndModifierRows;
using domain.pipeline.fourth.com.Square.SalesFactories.SalesRowFactories.ServiceChargeRow;
using domain.pipeline.fourth.com.Square.SalesFactories.SalesRowFactories.TenderRow;
using square.pipeline.fourth.com.Extensions;
using square.pipeline.fourth.com.Model;
using square.pipeline.fourth.com.Services;
using Square;
// Square v43 - models in root namespace
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using com.fourth.pipeline.pos.Model;
using domain.pipeline.fourth.com.Enums;

namespace domain.pipeline.fourth.com.Services.Square
{
    public class SquareToFourthCSVGenerator
    {
        //services
        OrdersService v2ordersService;
        PaymentsService paymentsService;
        RefundsService refundsService;
        EmployeesService employeesService;
        CatalogService catalogService;

        //for api access
        string _squareToken;

        /// Location Datasets - all orders, payments for that location
        /// </summary>
        public IList<SquareLocationSalesDataset> _squareLocationDatasets { get; private set; } = new List<SquareLocationSalesDataset>();
        public SquareBrandSalesDataset _squareBrandSalesDataset { get; set; }

        public SquareToFourthCSVGenerator(string squareToken, string squareBaseUrl = null)
        {
            _squareToken = squareToken;

            //move to DI
            v2ordersService = new OrdersService(_squareToken, squareBaseUrl);
            paymentsService = new PaymentsService(_squareToken, squareBaseUrl);
            refundsService = new RefundsService(_squareToken, squareBaseUrl);
            employeesService = new EmployeesService(_squareToken, squareBaseUrl);
            catalogService = new CatalogService(_squareToken, squareBaseUrl);
        }


        /// <summary>
        /// Takes UTC times to pull data from Square for  brand
        /// </summary>
        /// <param name="startUTC"></param>
        /// <param name="endUTC"></param>
        public async Task GatherDataForBrand()
        {
            var dataSet = new SquareBrandSalesDataset();
            //emps

            dataSet.allEmployees = await employeesService.GetEmployees();

            //get all prods
            //list products
            var catalog = await catalogService.GetCatalog();
            dataSet.entireCatalog = catalog;

            var allItems = catalog.Where(x => x.IsItem).Select(x => x.AsItem()).ToList();
            dataSet.allItems = allItems;
            var allItemData = allItems.Select(x => x.ItemData).ToList();
            dataSet.allProductVariations = allItemData.SelectMany(x => x.Variations).Select(x => x.AsItemVariation()).ToList();
            dataSet.allCategories = catalog.Where(x => x.IsCategory).Select(x => x.AsCategory()).ToList();
            dataSet.allModifiers = catalog.Where(x => x.IsModifierList).Select(x => x.AsModifierList().ModifierListData).ToList();

            _squareBrandSalesDataset = dataSet;
        }


        /// <summary>
        /// Takes UTC times to pull data from Square for single location brand + stores
        /// </summary>
        /// <param name="startUTC"></param>
        /// <param name="endUTC"></param>
        public async Task<DataGatherResultAndException> GatherDataForLocation(DateTime startUTC, DateTime endUTC, Location location)
        {

            //Now create dataset from Square
            var dataSet = new SquareLocationSalesDataset();
            try
            {
                dataSet.Location = location;
                //get all the data and fill the dataset
                dataSet.orders = await v2ordersService.GetOrdersForLocationByDateTimeUTC(location.Id, startUTC, endUTC);
                if (dataSet.orders == null)
                {
                    return new DataGatherResultAndException
                    {
                        DataGatherResult = DataGatherResult.OrderEmpty
                    };
                }

                //get V2 payments for the location
                var paymentsResults = await paymentsService.GetPaymentsForLocationByDateTimeUTC(location.Id, startUTC, endUTC);
                dataSet.paymentsForOrders = paymentsResults;

                var refundsResults = await refundsService.GetRefundsForLocationByDateTimeUTC(location.Id, startUTC, endUTC);
                dataSet.refundsForOrders = refundsResults;

            }
            catch (Exception ex)
            {
                //LOG
                return new DataGatherResultAndException
                {
                    DataGatherResult = DataGatherResult.Error,
                    Exception = new Exception("There was a problem filling dataset for: " + location.Name + " in brand " + location.BusinessName, ex)
                };
            }

            _squareLocationDatasets.Add(dataSet);
            return new DataGatherResultAndException
            {
                DataGatherResult = DataGatherResult.Complete
            };
        }

        public IEnumerable<TransactionDatasetRow> CreateSalesRows(string unitId)
        {
            RecordActivityCodeService recordActivityCodeService = new RecordActivityCodeService();
            List<TransactionDatasetRow> data = new List<TransactionDatasetRow>();

            //loop sites
            foreach (var locationDataset in _squareLocationDatasets)
            {
                try
                {
                    //get all products / variations for the location
                    ///loop orders
                    var completed = locationDataset.orders.Where(x => x.State == OrderState.Completed).ToList();
                    var notCompleted = locationDataset.orders.Except(completed).ToList();

                    foreach (var order in completed)
                    {
                        try
                        {
                            //GET ALL DATA WE NEED TO POPULATE ROW
                            var deviceName = "";
                            var deviceId = "";
                            var receiptCode = "";
                            var checkCode = "";

                            var returns = order.Returns ?? new List<OrderReturn>();
                            var returnedLienItems = returns.SelectMany(x => x.ReturnLineItems);
                            var orderPaymentIds = (order.Tenders ?? new List<Tender>())
                                .Select(x => x.PaymentId)
                                .Where(x => !string.IsNullOrWhiteSpace(x))
                                .ToHashSet();
                            var refunds = locationDataset.refundsForOrders?
                                .Where(x => x.OrderId == order.Id || orderPaymentIds.Contains(x.PaymentId))
                                .ToList() ?? new List<PaymentRefund>();

                            List<TenderSet> tenderSet = new List<TenderSet>();
                            TeamMember employee = NullObjectHelper.CreateNullEmployee();
                            recordActivityCodeService.ResetToZero();

                            //can only get the device if there is a v2 payment
                            foreach (var tender in order.Tenders ?? new List<Tender>())
                            {
                                var payment = locationDataset.paymentsForOrders.FirstOrDefault(x => x.Id == tender.PaymentId);
                                tenderSet.Add(new TenderSet { Tender = tender, Payment = payment });
                                if (payment != null)
                                {
                                    //try get a team member
                                    try
                                    {
                                        if (!string.IsNullOrWhiteSpace(payment.TeamMemberId))
                                        {
                                            employee = _squareBrandSalesDataset.allEmployees.FirstOrDefault(x => x.Id == payment.TeamMemberId) ?? employee;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        Console.WriteLine("There was a problem getting employee for this tran");
                                    }

                                    //get 'default device' details from the first payment
                                    var latestPayment = tenderSet.First().Payment;
                                    deviceId = latestPayment?.DeviceDetails?.DeviceId ?? "";
                                    deviceName = latestPayment?.DeviceDetails?.DeviceName ?? deviceId;
                                }
                            }

                            //DATA GATHER COMPLETED
                            //now we've got everything, create the sales row, discount row or whatever
                            recordActivityCodeService.Increment(); //for open row
                            var tabOpenRow = TabOpenRowFactory.Create(employee, order, unitId, locationDataset.Location.Id, recordActivityCodeService.GetCurrentActivityCode().ToString(), deviceId, deviceName);
                            data.Add(tabOpenRow);

                            ////Each item is either - salesitem, discountItem, Modifier Item (are these just seperate)?
                            foreach (var item in order.LineItems ?? new List<OrderLineItem>())
                            {
                                recordActivityCodeService.Increment(); //for open row

                                var prodVariationForItem = _squareBrandSalesDataset.allProductVariations.FirstOrDefault(x => x.Id == item.CatalogObjectId);
                                var productInCatalog = _squareBrandSalesDataset.allItems.FirstOrDefault(x => x.Id == prodVariationForItem?.ItemVariationData?.ItemId);
                                var categoryForItem = _squareBrandSalesDataset.allCategories.FirstOrDefault(x => x.Id == productInCatalog?.ItemData?.CategoryId);

                                var itemCateogoryName = "";
                                var unknownCat = "Unknown Category";
                                if (categoryForItem?.CategoryData != null)
                                {
                                    itemCateogoryName = categoryForItem?.CategoryData?.Name ?? unknownCat;
                                }
                                else
                                {
                                    itemCateogoryName = unknownCat;
                                }

                                var lineItemRow = SalesItemFactory.Create(employee, order, item, prodVariationForItem, unitId, locationDataset.Location.Id, recordActivityCodeService.GetCurrentActivityCode().ToString(), receiptCode, checkCode, deviceId, deviceName, itemCateogoryName);
                                data.Add(lineItemRow);

                                //generate modifer row
                                if (item.Modifiers != null)
                                    foreach (var modifier in item.Modifiers)
                                    {
                                        try
                                        {
                                            recordActivityCodeService.Increment();
                                            var modifierCatalog = _squareBrandSalesDataset.allModifiers.FirstOrDefault();
                                            var modifierRow = ModifierRowFactory.Create(employee, order, item, modifier, unitId, locationDataset.Location.Id, recordActivityCodeService.GetCurrentActivityCode().ToString(), receiptCode, checkCode, deviceId, deviceName, itemCateogoryName);
                                            data.Add(modifierRow);
                                        }
                                        catch (Exception ex)
                                        {
                                            throw new Exception("There was a problem creatng modifier for order:", ex);
                                        }
                                    }

                                //generate discount rows - in v43, discounts are at order level, line items reference via AppliedDiscounts
                                var orderDiscounts = order.Discounts ?? new List<OrderLineItemDiscount>();
                                var appliedDiscountsForItem = item.AppliedDiscounts ?? new List<OrderLineItemAppliedDiscount>();
                                var discountsForItem = appliedDiscountsForItem
                                    .Select(ad => orderDiscounts.FirstOrDefault(d => d.Uid == ad.DiscountUid))
                                    .Where(d => d != null)
                                    .ToList();
                                if (discountsForItem.Count > 0)
                                    foreach (var discount in discountsForItem)
                                    {
                                        try
                                        {
                                            recordActivityCodeService.Increment();
                                            var discountRow = DiscountRowFactory.Create(employee, order, item, prodVariationForItem, discount, unitId, locationDataset.Location.Id, recordActivityCodeService.GetCurrentActivityCode().ToString(), receiptCode, checkCode, deviceId, deviceName, itemCateogoryName);
                                            data.Add(discountRow);
                                        }
                                        catch (Exception ex)
                                        {
                                            throw new Exception("There was a problem creatng discount for order:", ex);
                                        }

                                    }

                            }

                            foreach (var serviceCharge in order.ServiceCharges ?? new List<OrderServiceCharge>())
                            {
                                var serviceChargeAmount = serviceCharge.TotalMoney?.Amount ??
                                    serviceCharge.AppliedMoney?.Amount ??
                                    serviceCharge.AmountMoney?.Amount ??
                                    0;
                                if (serviceChargeAmount <= 0)
                                {
                                    continue;
                                }

                                recordActivityCodeService.Increment();
                                var serviceChargeRow = ServiceChargeRowFactory.CreateForOrderServiceCharge(
                                    employee,
                                    order,
                                    serviceCharge,
                                    unitId,
                                    locationDataset.Location.Id,
                                    recordActivityCodeService.GetCurrentActivityCode().ToString(),
                                    deviceId,
                                    deviceName);
                                data.Add(serviceChargeRow);
                            }

                            //tender rows
                            foreach (var tenderAndPayment in tenderSet)
                            {
                                var tipAmount = tenderAndPayment.Tender.TipMoney?.Amount ??
                                    tenderAndPayment.Payment?.TipMoney?.Amount ??
                                    0;
                                if (tipAmount > 0)
                                {
                                    recordActivityCodeService.Increment();
                                    var tipRow = ServiceChargeRowFactory.CreateForTenderTip(
                                        employee,
                                        order,
                                        tenderAndPayment.Tender,
                                        tenderAndPayment.Payment,
                                        unitId,
                                        locationDataset.Location.Id,
                                        recordActivityCodeService.GetCurrentActivityCode().ToString(),
                                        deviceId,
                                        deviceName);
                                    data.Add(tipRow);
                                }

                                recordActivityCodeService.Increment();
                                var tenderRow = TenderSquareRowFactory.Create(employee, order, tenderAndPayment.Payment, tenderAndPayment.Tender, unitId, locationDataset.Location.Id, recordActivityCodeService.GetCurrentActivityCode().ToString(), deviceId, deviceName);
                                data.Add(tenderRow);
                            }

                            foreach (var refund in refunds.Where(x => (x.AmountMoney?.Amount ?? 0) > 0))
                            {
                                recordActivityCodeService.Increment();
                                var refundedPayment = locationDataset.paymentsForOrders
                                    .FirstOrDefault(x => x.Id == refund.PaymentId);
                                var refundedTender = (order.Tenders ?? new List<Tender>())
                                    .FirstOrDefault(x => x.PaymentId == refund.PaymentId);
                                var refundRow = TenderSquareRowFactory.CreateForPaymentRefund(
                                    employee,
                                    order,
                                    refund,
                                    refundedPayment,
                                    refundedTender,
                                    unitId,
                                    locationDataset.Location.Id,
                                    recordActivityCodeService.GetCurrentActivityCode().ToString(),
                                    deviceId,
                                    deviceName);
                                data.Add(refundRow);
                            }

                            //closed row
                            recordActivityCodeService.Increment();
                            var tabClosedRow = TabClosedRowFactory.Create(employee, order, unitId, locationDataset.Location.Id, recordActivityCodeService.GetCurrentActivityCode().ToString(), receiptCode, checkCode, deviceId, deviceName);
                            data.Add(tabClosedRow);
                        }
                        catch (Exception ex)
                        {

                            throw new Exception("There was an error in square processing order item for sales to fourth  integration - item ID:" + order.Id, ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("There was a problem generating the dataset for: " + locationDataset.Location.Name, ex);
                }
            }

            //returnas all locations as single CSV
            return data;
        }

    }
}
