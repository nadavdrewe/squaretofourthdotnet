using CsvHelper;
using domain.pipeline.fourth.com.Helper;
using domain.pipeline.fourth.com.SalesFactories;
using domain.pipeline.fourth.com.SalesFactories.Helper;
using domain.pipeline.fourth.com.SalesFactories.SalesRowFactories;
using domain.pipeline.fourth.com.Square.SalesFactories.Helper;
using domain.pipeline.fourth.com.Square.SalesFactories.SalesRowFactories.DiscountAndModifierRows;
using domain.pipeline.fourth.com.Square.SalesFactories.SalesRowFactories.TenderRow;
using NUnit.Framework;
using com.fourth.pipeline.pos.Extensions;
using com.fourth.pipeline.pos.Model;
using Shouldly;
using square.pipeline.fourth.com.Extensions;
using square.pipeline.fourth.com.Model;
using square.pipeline.fourth.com.Services;
using Square;
using Square.TeamMembers;
using Square.Payments;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using domain.pipeline.fourth.com.Extensions;

namespace tests.domain.pipelines.fourth.com
{
    [TestFixture]
    [Explicit("Requires live Square credentials and current Square data.")]
    public class GenerateSquareToFourthSalesCSVTest
    {


        string emailNadzliveApiKey = "";
        string emailNadzToken = "";

        //TEST STATIC VARIOABLES
        string testUnitId = "TEST_UNIT_ID";
        string testLocationCode = "TEST_LOCATION_CODE";

        OrdersService SUT;
        SquareClient _client;
        List<Location> locations = new List<Location>();
        DateTime startUTC;
        DateTime endUTC;
        IEnumerable<Order> orders;
        IEnumerable<Payment> paymentsForOrders;

        IEnumerable<CatalogObject> entireCatalog;
        IEnumerable<CatalogObjectItemVariation> allProductVariations;
        IEnumerable<CatalogObjectItem> allItemsTyped;
        IEnumerable<CatalogObjectCategory> allCategories;
        IEnumerable<CatalogModifierList> allModifiers;
        IEnumerable<TeamMember> allEmployees;

        [SetUp]
        public async Task Arrange()
        {
            var now = DateTime.Now;
            SUT = new OrdersService(emailNadzToken);
            _client = new SquareClient(emailNadzToken);

            startUTC = new DateTime(now.Year, now.Month, now.Day, 03, 00, 00);
            endUTC = startUTC.AddDays(1);

            // Get locations
            var locationsResponse = await _client.Locations.ListAsync();
            locations = locationsResponse.Locations?.ToList() ?? new List<Location>();

            orders = await SUT.GetOrdersForLocationByDateTimeUTC(locations.First().Id, startUTC, endUTC);

            // Get V2 payments
            var startTimeUTCstring = startUTC.ToSquareDateTime();
            var endTimeUTCstring = endUTC.ToSquareDateTime();
            var paymentsList = new List<Payment>();
            await foreach (var payment in await _client.Payments.ListAsync(
                new ListPaymentsRequest
                {
                    BeginTime = startTimeUTCstring,
                    EndTime = endTimeUTCstring,
                    LocationId = locations.First().Id
                }))
            {
                paymentsList.Add(payment);
            }
            paymentsForOrders = paymentsList;

            //team members
            var teamMemberResponse = await _client.TeamMembers.SearchAsync(
                new SearchTeamMembersRequest());
            allEmployees = teamMemberResponse.TeamMembers;

            //get all prods
            var catalog = new List<CatalogObject>();
            await foreach (var item in await _client.Catalog.ListAsync(new Square.Catalog.ListCatalogRequest()))
            {
                catalog.Add(item);
            }

            entireCatalog = catalog;

            var allItems = catalog.Where(x => x.IsItem).Select(x => x.AsItem()).ToList();
            allItemsTyped = allItems;
            allCategories = catalog.Where(x => x.IsCategory).Select(x => x.AsCategory()).ToList();
            var allItemData = allItems.Select(x => x.ItemData).ToList();
            allProductVariations = allItemData.SelectMany(x => x.Variations).Select(x => x.AsItemVariation()).ToList();
            allModifiers = catalog.Where(x => x.IsModifierList).Select(x => x.AsModifierList().ModifierListData).ToList();

        }


        [Test]
        public async Task CreateSalesRows()
        {

            RecordActivityCodeService recordActivityCodeService = new RecordActivityCodeService();
            List<TransactionDatasetRow> data = new List<TransactionDatasetRow>();

            //loop sites
            foreach (var location in locations)
            {
                var completed = orders.Where(x => x.State == OrderState.Completed).ToList();
                var notCompleted = orders.Except(completed).ToList();

                foreach (var order in completed)
                {
                    var deviceName = "";
                    var deviceId = "";
                    var receiptCode = "";
                    var checkCode = "";

                    List<TenderSet> tenderSet = new List<TenderSet>();
                    TeamMember employee = NullObjectHelper.CreateNullEmployee();
                    recordActivityCodeService.ResetToZero();

                    foreach (var tender in order.Tenders)
                    {
                        var payment = paymentsForOrders?.FirstOrDefault(x => x.Id == tender.PaymentId);
                        if (payment == null)
                        {
                        }
                        else
                        {
                            tenderSet.Add(new TenderSet { Tender = tender, Payment = payment });

                            try
                            {
                                if (!string.IsNullOrWhiteSpace(payment.TeamMemberId))
                                {
                                    employee = allEmployees.FirstOrDefault(x => x.Id == payment.TeamMemberId) ?? employee;
                                }
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("There was a problem getting employee for this tran");
                            }

                            var latestPayment = tenderSet.First().Payment;
                            deviceName = latestPayment?.DeviceDetails?.DeviceId ?? "";
                            deviceId = latestPayment?.DeviceDetails?.DeviceId ?? "";
                        }

                    }


                    recordActivityCodeService.Increment();
                    var tabOpenRow = TabOpenRowFactory.Create(employee, order, testUnitId, location.Id, recordActivityCodeService.GetCurrentActivityCode().ToString(), deviceId, deviceName);
                    data.Add(tabOpenRow);

                    foreach (var item in order.LineItems)
                    {
                        recordActivityCodeService.Increment();

                        var prodVariationForItem = allProductVariations.FirstOrDefault(x => x.Id == item.CatalogObjectId);
                        var productInCatalog = allItemsTyped.FirstOrDefault(x => x.Id == prodVariationForItem?.ItemVariationData?.ItemId);
                        var itemCategory = allCategories.FirstOrDefault(x => x.Id == productInCatalog?.ItemData?.CategoryId);
                        var itemCateogoryName = itemCategory?.CategoryData?.Name ?? "Unknown Category";

                        var lineItemRow = SalesItemFactory.Create(employee, order, item, prodVariationForItem, testUnitId, location.Id, recordActivityCodeService.GetCurrentActivityCode().ToString(), receiptCode, checkCode, deviceId, deviceName, itemCateogoryName);
                        data.Add(lineItemRow);

                        if (item.Modifiers != null)
                            foreach (var modifier in item.Modifiers)
                            {
                                try
                                {
                                    recordActivityCodeService.Increment();
                                    var modifierCatalog = allModifiers.FirstOrDefault();
                                    var modifierRow = ModifierRowFactory.Create(employee, order, item, modifier, testUnitId, location.Id, recordActivityCodeService.GetCurrentActivityCode().ToString(), receiptCode, checkCode, deviceId, deviceName, itemCateogoryName);
                                    data.Add(modifierRow);
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("There was a problem creatng modifier for order:", ex);
                                }
                            }

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
                                    var discountRow = DiscountRowFactory.Create(employee, order, item, prodVariationForItem, discount, testUnitId, location.Id, recordActivityCodeService.GetCurrentActivityCode().ToString(), receiptCode, checkCode, deviceId, deviceName, itemCateogoryName);
                                    data.Add(discountRow);
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("There was a problem creatng discount for order:", ex);
                                }

                            }

                        foreach (var tenderAndPayment in tenderSet)
                        {
                            recordActivityCodeService.Increment();
                            var tenderRow = TenderSquareRowFactory.Create(employee, order, tenderAndPayment.Payment, tenderAndPayment.Tender, testUnitId, location.Id, recordActivityCodeService.GetCurrentActivityCode().ToString(), deviceId, deviceName);
                            data.Add(tenderRow);
                        }

                        recordActivityCodeService.Increment();
                        var tabClosedRow = TabClosedRowFactory.Create(employee, order, testUnitId, location.Id, recordActivityCodeService.GetCurrentActivityCode().ToString(), receiptCode, checkCode, deviceId, deviceName);
                        data.Add(tabClosedRow);
                    }
                }

                //output to csv
                using (TextWriter writer = new StreamWriter(@"c:\test\firstNewSquareTestsPipeline.csv"))
                {
                    var csv = new CsvWriter(writer, System.Globalization.CultureInfo.CurrentCulture);
                    csv.WriteRecords(data);
                    csv.Flush();
                }

            }

        }

        [Test]
        public async Task Create_TAB_OPEN_Row()
        {
            RecordActivityCodeService recordActivityCodeService = new RecordActivityCodeService();
            List<TransactionDatasetRow> data = new List<TransactionDatasetRow>();

            foreach (var order in orders)
            {
                recordActivityCodeService.Increment();

                var openRow = TabOpenRowFactory.Create(new TeamMember(), order, testUnitId, testLocationCode, recordActivityCodeService.GetCurrentActivityCode().ToString(), "tests", "tests");

                var dateTimeUTC = DateTimeOffset.Parse(order.CreatedAt);
                var dateUTC = dateTimeUTC.ToFourthSalesCSVDateUTC();
                var timeUTC = dateTimeUTC.ToFourthSalesCSVTimeUTC();

                openRow.UnitId.ShouldBe(testUnitId);
                openRow.SiteLocationCode.ShouldBe(testLocationCode);
                openRow.TimeFact.ShouldBe("0");
                openRow.RecordActivityCode.ShouldBe(recordActivityCodeService.GetCurrentActivityCode().ToString());
                openRow.TransactionId.ShouldBe(order.Id.ToCodedTransactionId(recordActivityCodeService.GetCurrentActivityCode().ToString()));

                openRow.TradingDate.ShouldBe(dateUTC);
                openRow.Time.ShouldBe(timeUTC);
            }

        }

        [Test]
        public async Task Create_TAB_CLOSED_Row()
        {
            List<TransactionDatasetRow> data = new List<TransactionDatasetRow>();
            RecordActivityCodeService recordActivityCodeService = new RecordActivityCodeService();
            foreach (var order in orders)
            {
                recordActivityCodeService.Increment();
                var baseRow = TabClosedRowFactory.Create(new TeamMember(), order,
                    testUnitId,
                    testLocationCode, recordActivityCodeService.GetCurrentActivityCode().ToString(), order.Id, order.Id, "tests", "tests");

                var dateTimeUTC = DateTimeOffset.Parse(order.CreatedAt);
                var dateUTC = dateTimeUTC.ToFourthSalesCSVDateUTC();
                var timeUTC = dateTimeUTC.ToFourthSalesCSVTimeUTC();

                baseRow.UnitId.ShouldBe(testUnitId);
                baseRow.SiteLocationCode.ShouldBe(testLocationCode);
                baseRow.TimeFact.ShouldBe("0");
                baseRow.RecordActivityCode.ShouldBe(recordActivityCodeService.GetCurrentActivityCode().ToString());
                baseRow.TransactionId.ShouldBe(order.Id.ToCodedTransactionId(recordActivityCodeService.GetCurrentActivityCode().ToString()));

                baseRow.TradingDate.ShouldBe(dateUTC);
                baseRow.Time.ShouldBe(timeUTC);
            }

        }
    }
}
