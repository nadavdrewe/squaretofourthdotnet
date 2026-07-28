using com.fourth.pipeline.pos.Model;
using core.lightspeed.com.Models.Core;
using core.lightspeed.com.Models.Financial.Receipts;
using core.lightspeed.com.Models.Inventory;
using core.lightspeed.com.Models.Labour;
using domain.pipeline.fourth.com.CSVRowCreatorFactories.LSResto.Helper;
using domain.pipeline.fourth.com.CSVRowCreatorFactories.LSResto.SalesRowFactories;
using domain.pipeline.fourth.com.CSVRowCreatorFactories.LSResto.SalesRowFactories.DiscountRow;
using domain.pipeline.fourth.com.CSVRowCreatorFactories.LSResto.SalesRowFactories.ModifierRow;
using domain.pipeline.fourth.com.CSVRowCreatorFactories.LSResto.SalesRowFactories.OpenCloseRows;
using domain.pipeline.fourth.com.CSVRowCreatorFactories.LSResto.SalesRowFactories.SalesItemRow;
using domain.pipeline.fourth.com.CSVRowCreatorFactories.LSResto.SalesRowFactories.TenderRow;
using domain.pipeline.fourth.com.SalesFactories.Helper;
using domain.pipeline.fourth.com.SalesFactories.SalesRowFactories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace domain.pipeline.fourth.com.Services.LightspeedResto
{
    public class LsRestoToFouthSalesCSVGenerator
    {
        /// <summary>
        /// This does the work!
        /// </summary>
        /// <param name="unitId"></param>
        /// <returns></returns>
        public IEnumerable<TransactionDatasetRow> CreateSalesRows(DateTime transactionDate,
            string unitId,
            string siteLocationCode,
            string revenueCenter,
            string currency,
            IEnumerable<Receipt> receipts,
            IEnumerable<Employee> employees,
            IEnumerable<Product> products,
            IEnumerable<ProductGroup> productGroups,
            IEnumerable<Reservation> reservations)
        {
            RecordActivityCodeService recordActivityCodeService = new RecordActivityCodeService();
            List<TransactionDatasetRow> data = new List<TransactionDatasetRow>();

            //todo: impleent voids
            foreach (var receipt in receipts)
            {
                try
                {
                    //test
                    if (receipt.id == 70623384)
                    {
                        var stopThisReceipt = "";
                    }

                    //end


                    //reset
                    List<TransactionDatasetRow> dataForThisReceiptOnly = new List<TransactionDatasetRow>();
                    recordActivityCodeService.ResetToZero();

                    //this might be null
                    var reservation = receipt.reservationId != 0 ? reservations.First(x => x.id == receipt.reservationId) : null;
                    //test code
                    if (reservation != null)
                    {
                        var stop = "";
                    }

                    var employeeForReceipt = employees.FirstOrDefault(x => x.userId == receipt.userId);

                    //GATHER ALL DATA FROM RECEIPT
                    //GET ALL DATA WE NEED TO POPULATE ROW
                    var deviceName = "";
                    var deviceId = "";
                    //END DATA GATHER

                    //throw new NotImplementedException();

                    ////CREATE TABOPEN ROW
                    recordActivityCodeService.Increment(); //for open row
                    var tabOpenRow = TabOpenLSRestoFactory.Create(employeeForReceipt,
                        receipt,
                        reservation,
                        transactionDate,
                        unitId,
                        siteLocationCode,
                        revenueCenter,
                        recordActivityCodeService.GetCurrentActivityCode().ToString(),
                        deviceId,
                        deviceName);
                    dataForThisReceiptOnly.Add(tabOpenRow);
                    //////Each item is either - salesitem, discountItem, Modifier Item (are these just seperate)?

                    //NEED TO TAG VOIDS AS DELTETED??
                    var orderLevelDiscountItems = LSRestoSalesHelper.GetDiscountItemsFromListOfItems(receipt.items); //These are ORDER level 'discounts'
                    var normalItems = receipt.items.Where(x => x.priceTypeId == 0).ToList();
                    var serviceChargeItems = LSRestoSalesHelper.GetServiceChargeItemsFromListOfItems(receipt.items); //service charge

                    //do items
                    foreach (var item in normalItems ?? new List<Item>())
                    {
                        try
                        {
                            //IF ITS A VOID THERE MIGHT NOT BE AN ITEM - HAVE TO TRY AND GET... PARRENT ITEM? 
                            var productForItem = products.FirstOrDefault(x => x.id.ToString() == item.prodId) ?? null;
                            var groupsWeAreIn = (productForItem != null && productForItem.id != 0) ? productForItem.groupIds.Select(x => x.ToString()) : new List<string> { "0" };
                            var categoriesForProduct = groupsWeAreIn.First() != "0" ? productGroups.Where(x => groupsWeAreIn.Contains(x.id.ToString())).OrderBy(X => X.sequence).ToList() : LSRestoSalesHelper.CreateUnknownProductGroup();

                            //do item row
                            recordActivityCodeService.Increment(); //for sales row
                            var anyDiscounts = orderLevelDiscountItems.Where(x => x.info == item.info).ToList();
                            var salesitemrow = LSRestoSalesItemFactory.Create(employeeForReceipt,
                                receipt,
                                reservation,
                                item,
                                productForItem,
                                anyDiscounts,
                                transactionDate,
                                unitId,
                                siteLocationCode,
                                recordActivityCodeService.GetCurrentActivityCode(),
                                "",
                                "",
                               categoriesForProduct.ElementAt(0).name,
                               categoriesForProduct.ElementAt(0).name,
                               categoriesForProduct.ElementAt(0).name,
                               currency
                                );

                            //PROCESS VOIDED
                            //VOIDED = original receipt
                            //VOID = new receipt with minus amount
                            //set voided special attributes
                            //now set 'deleted' if status is 'void', else do nothing
                            if (receipt.type.Trim().ToLower() == "void") //want to augent receipt that was refunded
                            {
                                //set that receipt ID of each secondary receipt to the original parennt Receit
                                //and a new transactionId / paymentID of the second receipt                    
                                var originalReciept = receipts.Where(X => X.id == receipt.parentId).FirstOrDefault();
                                if (originalReciept != null)
                                {
                                    salesitemrow.ReceiptCode = originalReciept.id.ToString();
                                }
                                LSRestoSalesHelper.AssignCorrectVoidedTypeToVoidRow(salesitemrow, item);
                            }
                            dataForThisReceiptOnly.Add(salesitemrow);

                            //do modifiers rows
                            foreach (var mod in item.modifierValues)
                            {
                                //ignore blank ones
                                if (mod.price > 0 && mod.name.Trim().ToLower() != "none")
                                {
                                    var modifierProd = products.FirstOrDefault(x => x.sku.ToLower().Trim() == mod.plu.ToLower().Trim());
                                    //var secondTryAndModifierProduct = products.FirstOrDefault(x => x.id == mod.modifierId);
                                    //var thirdTryModifierPoduct = products.FirstOrDefault(x => x.id == mod.id);

                                    recordActivityCodeService.Increment(); //for sales row
                                    var modRow = LSModifierRowFactory.Create(employeeForReceipt,
                                        receipt,
                                        reservation,
                                        item,
                                        mod,
                                        modifierProd,
                                        transactionDate,
                                        unitId,
                                        siteLocationCode,
                                        recordActivityCodeService.GetCurrentActivityCode(), deviceId, deviceName, categoriesForProduct.ElementAt(0).name, currency
                                        );
                                    //create a modifier row
                                    dataForThisReceiptOnly.Add(modRow);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            var itemFailed = item;
                            throw ex;
                        }
                    }

                    //do discounts
                    //check with receipt level discount
                    foreach (var discountItem in orderLevelDiscountItems)
                    {
                        var prodForDiscoutn = products.First(x => x.id.ToString() == discountItem.prodId);
                        var groupIdForDiscountProd = prodForDiscoutn.groupIds.Select(x => x).ToList();
                        recordActivityCodeService.Increment(); //for sales row
                        var linkedItem = normalItems.FirstOrDefault(x => x.info == discountItem.info);
                        var discountCategory = productGroups.Where(x => groupIdForDiscountProd.Contains(x.id.ToString())).OrderBy(X => X.sequence).ToList().First();

                        if (discountItem.totalPrice != 0.00M) //if there's no discount amount, don't bother
                        {
                            var discountRow = LSDiscountRowFactory.Create(employeeForReceipt,
                                    receipt,
                                    reservation,
                                    discountItem,
                                    linkedItem,
                                    transactionDate,
                                    unitId,
                                    siteLocationCode,
                                    recordActivityCodeService.GetCurrentActivityCode(),
                                    deviceId,
                                    deviceName,
                                    discountCategory.name,
                                    currency
                                    );
                            dataForThisReceiptOnly.Add(discountRow);
                        }
                    }


                    //do tenders
                    var payments = receipt.payments.ToList();
                    foreach (var payment in payments)
                    {
                        recordActivityCodeService.Increment(); //for sales row
                        var paymentRow = LSTenderRowFactory.Create(employeeForReceipt,
                            receipt,
                            reservation,
                            payment,
                            transactionDate,
                            unitId,
                            siteLocationCode,
                            recordActivityCodeService.GetCurrentActivityCode(), "", "", currency);

                        dataForThisReceiptOnly.Add(paymentRow);

                        //is there a tip? if so create tip row
                        if (payment.tips > 0)
                        {
                            var thisIsATipRow = payment.tips;
                        }
                    }


                    //close order
                    recordActivityCodeService.Increment(); //for sales row
                    var closeRow = TabClosedLSRestoFactory.Create(employeeForReceipt,
                        receipt,
                        reservation,
                        transactionDate,
                        unitId,
                        siteLocationCode,
                        revenueCenter,
                        recordActivityCodeService.GetCurrentActivityCode().ToString(),
                        deviceId,
                        deviceName);
                    dataForThisReceiptOnly.Add(closeRow);


                    data.AddRange(dataForThisReceiptOnly);
                }
                catch (Exception ex)
                {
                    var whichWasit = receipt;
                    throw ex;
                }
            }

            return data;
        }
    }
}
