using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using com.fourth.pipeline.pos;
using com.fourth.pipeline.pos.Model;
using CsvHelper;
using domain.pipeline.fourth.com.Enums;
using domain.pipeline.fourth.com.Services.Square;
using domain.pipeline.fourth.com.Services.Square.SquareToFourthTimesheetsApi;
using NUnit.Framework;
using Shouldly;
using square.pipeline.fourth.com.Services;
using Square;

namespace tests.domain.pipelines.fourth.com.FourthSales
{
    [TestFixture]
    [Category("Sandbox")]
    public class SquareSandboxReplayTests
    {
        private const string RunId = "20260704204200";
        private const string ExpandedRunId = "20260704232039";
        private const string ReadinessRunId = "20260706122223";
        private const string PaymentRefundRunId = "20260706235456";
        private const string LocationId = "L8WQDAS2AGWZC";
        private const string IgnoredOpenOrderId = "yP5SQp8IlXeNArt5zXQYau6pUhGZY";
        private const string ExpandedIgnoredOpenOrderId = "Qa3gRodfsSCblVvUmh9eH8qKqx5YY";

        private static readonly DateTime SalesStartUtc = DateTime.Parse(
            "2026-07-04T20:40:00.7555900Z",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal);

        private static readonly DateTime SalesEndUtc = DateTime.Parse(
            "2026-07-04T20:57:12.3297190Z",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal);

        private static readonly DateTime LaborStartUtc = DateTime.Parse(
            "2026-07-04T15:42:15.4922078Z",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal);

        private static readonly DateTime LaborEndUtc = DateTime.Parse(
            "2026-07-04T20:57:12.3297190Z",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal);

        private static readonly DateTime ExpandedSalesStartUtc = DateTime.Parse(
            "2026-07-04T23:18:39.2726976Z",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal);

        private static readonly DateTime ExpandedSalesEndUtc = DateTime.Parse(
            "2026-07-04T23:36:29.9151453Z",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal);

        private static readonly DateTime ExpandedLaborStartUtc = DateTime.Parse(
            "2026-06-27T07:00:00.0000000Z",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal);

        private static readonly DateTime ExpandedLaborEndUtc = DateTime.Parse(
            "2026-07-04T23:35:55.4340721Z",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal);

        private static readonly DateTime ReadinessSalesStartUtc = DateTime.Parse(
            "2026-07-06T12:20:23.9957074Z",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal);

        private static readonly DateTime ReadinessSalesEndUtc = DateTime.Parse(
            "2026-07-06T12:39:45.2131183Z",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal);

        private static readonly DateTime ReadinessLaborStartUtc = DateTime.Parse(
            "2026-06-22T07:00:00.0000000Z",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal);

        private static readonly DateTime ReadinessLaborEndUtc = DateTime.Parse(
            "2026-07-06T12:37:54.5698547Z",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal);

        private static readonly DateTime PaymentRefundSalesStartUtc = DateTime.Parse(
            "2026-07-06T23:52:56.1038633Z",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal);

        private static readonly DateTime PaymentRefundSalesEndUtc = DateTime.Parse(
            "2026-07-07T00:11:49.2112519Z",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal);

        private static readonly DateTime PaymentRefundLaborStartUtc = DateTime.Parse(
            "2026-06-22T07:00:00.0000000Z",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal);

        private static readonly DateTime PaymentRefundLaborEndUtc = DateTime.Parse(
            "2026-07-07T00:10:27.3577176Z",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal);

        private static readonly string[] PaidOrderIds =
        {
            "sGCkIIE4Y0104kib5sF1HBCEzD8YY",
            "KjXFD36HSOnhH65cflL6W5c790TZY",
            "e19KDgZupWhsbDzCG5v6KcVAIWXZY"
        };

        private static readonly string[] TimecardIds =
        {
            "CDQ46SQPAC8D5",
            "DTWANW2S3S8KJ",
            "QBDF4KVRA1CFF"
        };

        private static readonly IReadOnlyDictionary<string, string> ExpandedPaidOrderPaymentIds =
            new Dictionary<string, string>
            {
                ["IYQQVjAr4VrfoM2iSPQFnMNosBMZY"] = "Tj9SVeYhF18RncWlNPE4XNGkEvDZY",
                ["QWPRZ1ybM0cO5U9w77xYykBLt6AZY"] = "D1sdMvEprnQ8qcTMsaD9VgBpa68YY",
                ["4gSfQCksBesWS5reVNdugbcmPW8YY"] = "F0Pnmu7CqBJ9fArGxw7mF2SPS9TZY",
                ["uzQImDCsa4vyr63EUWh6Atg6RLaZY"] = "VuKOde6lz8cfgVqJZ8olUO4BeS8YY"
            };

        private static readonly IReadOnlyDictionary<string, string> ExpandedTeamMemberEmployeeNumbers =
            new Dictionary<string, string>
            {
                ["TMUgrlW3NkxhCC7A"] = "SANDBOX-EMP-01",
                ["TMR0GsrflLiBDAdG"] = "SANDBOX-EMP-02",
                ["TMrqvySaweUlI6nC"] = "SANDBOX-EMP-03",
                ["TMqPkaJzDQJ7XUwu"] = "SANDBOX-EMP-04",
                ["TMEpcOdQjMX1WgaO"] = "SANDBOX-EMP-05",
                ["TMDi6n5dYcWZwMqB"] = "SANDBOX-EMP-06",
                ["TMbcjlRxw3gqnDKJ"] = "SANDBOX-EMP-07",
                ["TMok6MwuanneQR2e"] = "SANDBOX-EMP-08",
                ["TMbnMAKm0athTYoO"] = "SANDBOX-EMP-09",
                ["TMFA51hfPp69f2N6"] = "SANDBOX-EMP-10"
            };

        private static readonly IReadOnlyDictionary<string, string> ExpandedProductPlusModifier =
            new Dictionary<string, string>
            {
                ["FOURTH-SANDBOX-BURGER"] = "Cheese",
                ["FOURTH-SANDBOX-FRIES"] = "Truffle Salt",
                ["FOURTH-SANDBOX-COFFEE"] = "Oat Milk",
                ["FOURTH-SANDBOX-TEA"] = "Honey",
                ["FOURTH-SANDBOX-PASTA"] = "Parmesan",
                ["FOURTH-SANDBOX-SALAD"] = "Chicken",
                ["FOURTH-SANDBOX-STEAK"] = "Peppercorn Sauce",
                ["FOURTH-SANDBOX-DESSERT"] = "Birthday Plate",
                ["FOURTH-SANDBOX-JUICE"] = "Ginger Shot",
                ["FOURTH-SANDBOX-WINE"] = "Large Glass"
            };

        private static readonly string[] ReadinessPaidOrderIds =
        {
            "QM8WZgzBetTZEg6UzPDEHw5VJpJZY",
            "qxQYQyJnonfT06EnGTU0pKRFWpZZY",
            "QImKgsN26trPW3mj2Dwclf90nYLZY",
            "WPMOrrBqTrEBz6TLnxaJTevLt1TZY",
            "iD5XUonAA2fW0D6ZJTtZnAiyWORZY",
            "qdk9zgg68C8IUTmtlWekMrb0ZZMZY",
            "82u6Z26fbroF5Jxwu0gQTRI3GGJZY",
            "sugvLkOMEx0rdDFVsxV8wNkCYNAZY",
            "ej8eJXDoKQdctngoYu2UAaPARTXZY",
            "Cp0FL2It6ZUoElnvcL3zHkskCnFZY",
            "61OwmDF6Nrz6Jtt7Bd91px8R51BZY",
            "YGDY9THenlqIGSe7ULTtjVMft8IZY"
        };

        private static readonly string[] ReadinessIgnoredOpenOrderIds =
        {
            "6d8n7nBKB2EwZqpFrEKdpRUYUUIZY",
            "mT0Xr6JrJeIZ6KqQfrhwobVtXBbZY"
        };

        private static readonly string[] PaymentRefundPaidOrderIds =
        {
            "G113HdDDldfznmp3J0X8MkwW6hcZY",
            "48aAmWT7DsfjEZANXuhYj6r2nMKZY",
            "kkbuzwmd4e6FUxQXz2bUnbokucPZY",
            "ip0z44BYGl4RNjnsi5SktJHMMuCZY",
            "Sr7SxbohMegyAO8rEoxcVuFuQkTZY",
            "MKQGepZRW7ONqpQ5GlfeiDKX788YY",
            "CBycgHdJrPbvc3OsihZXe2lqvwWZY",
            "Iyn15D7kIs7OrxY5NljrJr1KM3eZY",
            "2r60qaf2LSphJfdEjI6wsWW2yTGZY",
            "6ThaezMo5n8WzTELHidQVSW4IfPZY"
        };

        private static readonly string[] PaymentRefundIgnoredOpenOrderIds =
        {
            "8i8KtvGvViFnEI49Ytg6ZkxpyQZZY",
            "qByteD5Dm9Y9sAXRnff08RjBACJZY"
        };

        private static readonly IReadOnlyDictionary<string, string> ReadinessTeamMemberEmployeeNumbers =
            new Dictionary<string, string>
            {
                ["TMYjZmf3yuCfXwNc"] = "SANDBOX-EMP-01",
                ["TMUx6Ek0JtzIaJki"] = "SANDBOX-EMP-02",
                ["TMFdnMUFblFitZlg"] = "SANDBOX-EMP-03",
                ["TMMLYzDPbOmMFTU5"] = "SANDBOX-EMP-04",
                ["TMZOQW6HkugugklC"] = "SANDBOX-EMP-05",
                ["TM5Spifr7kizoxY-"] = "SANDBOX-EMP-06",
                ["TMzIUTMg-nJIweI6"] = "SANDBOX-EMP-07",
                ["TMKk2lrw5vjXbwpY"] = "SANDBOX-EMP-08",
                ["TMFLHCweX2vkmtBP"] = "SANDBOX-EMP-09",
                ["TM_m4BTGpXkzsEH5"] = "SANDBOX-EMP-10",
                ["TMVYHsyUg2kWKQqb"] = "SANDBOX-EMP-11",
                ["TMZFM0U7kT3lofUC"] = "SANDBOX-EMP-12",
                ["TM3sL8Zbb5Cz-llR"] = "SANDBOX-EMP-13",
                ["TMsQhufBIE0zU2nO"] = "SANDBOX-EMP-14",
                ["TMC7dISTNqgNCXYU"] = "SANDBOX-EMP-15"
            };

        private static readonly IReadOnlyDictionary<string, string> ReadinessProductPlusModifier =
            new Dictionary<string, string>
            {
                ["FOURTH-SANDBOX-BURGER"] = "Cheese",
                ["FOURTH-SANDBOX-FRIES"] = "Truffle Salt",
                ["FOURTH-SANDBOX-COFFEE"] = "Oat Milk",
                ["FOURTH-SANDBOX-TEA"] = "Honey",
                ["FOURTH-SANDBOX-PASTA"] = "Parmesan",
                ["FOURTH-SANDBOX-SALAD"] = "Chicken",
                ["FOURTH-SANDBOX-STEAK"] = "Peppercorn Sauce",
                ["FOURTH-SANDBOX-DESSERT"] = "Birthday Plate",
                ["FOURTH-SANDBOX-JUICE"] = "Ginger Shot",
                ["FOURTH-SANDBOX-WINE"] = "Large Glass",
                ["FOURTH-SANDBOX-EGGS"] = "Smoked Salmon",
                ["FOURTH-SANDBOX-PANCAKES"] = "Maple Syrup",
                ["FOURTH-SANDBOX-GRANOLA"] = "Greek Yoghurt",
                ["FOURTH-SANDBOX-SOUP"] = "Sourdough",
                ["FOURTH-SANDBOX-FISH"] = "Lemon Butter",
                ["FOURTH-SANDBOX-COCKTAIL"] = "Premium Gin",
                ["FOURTH-SANDBOX-BEER"] = "Lime",
                ["FOURTH-SANDBOX-MERCH"] = "Gift Wrap",
                ["FOURTH-SANDBOX-VOUCHER"] = "Envelope",
                ["FOURTH-SANDBOX-WATER"] = "Ice"
            };

        private static readonly IReadOnlyDictionary<string, string> PaymentRefundTeamMemberEmployeeNumbers =
            new Dictionary<string, string>
            {
                ["TMyZhi71qSX9iBzF"] = "SANDBOX-EMP-01",
                ["TMA3pOJ02_tWK7f3"] = "SANDBOX-EMP-02",
                ["TMAhsqZ8m44pPnL9"] = "SANDBOX-EMP-03",
                ["TMxLvjlDVdfebR0n"] = "SANDBOX-EMP-04",
                ["TM0-YdS5k-7eG68t"] = "SANDBOX-EMP-05",
                ["TM4836VzBN7pyeYe"] = "SANDBOX-EMP-06",
                ["TM_8g6ZL2O_GrNKa"] = "SANDBOX-EMP-07",
                ["TMFZSGeacfeJ2yQc"] = "SANDBOX-EMP-08",
                ["TMAD99yhvJrDifMc"] = "SANDBOX-EMP-09",
                ["TMLNWiW_aPyAR7mw"] = "SANDBOX-EMP-10",
                ["TM6dTrt7WDXGNXcx"] = "SANDBOX-EMP-11",
                ["TMVvPK_XradnxFRg"] = "SANDBOX-EMP-12",
                ["TMTVMbWTq4Wiz2pQ"] = "SANDBOX-EMP-13",
                ["TMHXnABOL3qHtPEZ"] = "SANDBOX-EMP-14",
                ["TMiCZpb9_Lrm_uLe"] = "SANDBOX-EMP-15"
            };

        [Test]
        [Explicit("Replays the recorded 2026-07-04 Square sandbox seed without creating new sandbox data.")]
        public async Task ReplayRecordedSandboxSeed_ThenGenerateFourthArtifacts()
        {
            var config = SquareSandboxTestConfig.Load();
            var client = new SquareClient(config.AccessToken, new ClientOptions { BaseUrl = config.BaseUrl });

            var locationsResponse = await client.Locations.ListAsync();
            var location = locationsResponse.Locations?.FirstOrDefault(x => x.Id == LocationId);
            location.ShouldNotBeNull($"Expected recorded sandbox location {LocationId}.");

            var salesGenerator = new SquareToFourthCSVGenerator(config.AccessToken, config.BaseUrl);
            await salesGenerator.GatherDataForBrand();
            var salesGatherResult = await salesGenerator.GatherDataForLocation(SalesStartUtc, SalesEndUtc, location);
            salesGatherResult.DataGatherResult.ShouldBe(
                DataGatherResult.Complete,
                $"Expected recorded sandbox orders between {SalesStartUtc:O} and {SalesEndUtc:O}. {salesGatherResult.Exception?.Message}");

            var rows = salesGenerator.CreateSalesRows("SANDBOX_UNIT").ToList();
            var paidOrderIds = new HashSet<string>(PaidOrderIds);
            var replayRows = rows.Where(x => paidOrderIds.Contains(x.ReceiptCode)).ToList();

            rows.Any(x => x.ReceiptCode == IgnoredOpenOrderId).ShouldBeFalse(
                "The recorded open/unpaid sandbox order should still be excluded from completed-sales output.");
            replayRows.Count.ShouldBe(22);
            replayRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.TAB_OPEN).ShouldBe(3);
            replayRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.TAB_CLOSE).ShouldBe(3);
            replayRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.SALES_ITEM).ShouldBe(4);
            replayRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.MODIFIER_ITEM).ShouldBe(3);
            replayRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.DISC_ITEM).ShouldBe(2);
            replayRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.SERVICE_CHARGE).ShouldBe(4);
            replayRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.TENDER).ShouldBe(3);
            replayRows.Sum(x => x.TenderAmount).ShouldBe(36.02M);
            replayRows
                .Where(x => x.TransactionTypeCode == TransactionTypeCodes.SERVICE_CHARGE)
                .Sum(x => x.PricePaid)
                .ShouldBe(4.75M);
            replayRows
                .Where(x => x.TransactionTypeCode == TransactionTypeCodes.SALES_ITEM)
                .Sum(x => x.Deduction)
                .ShouldBe(4.98M);
            replayRows
                .Where(x => x.TransactionTypeCode == TransactionTypeCodes.DISC_ITEM)
                .Sum(x => x.Deduction)
                .ShouldBe(4.98M);

            var timesheetGenerator = new SquareToFourthTimesheetXmlGenerator(config.AccessToken, config.BaseUrl);
            await timesheetGenerator.GatherDataForLocation(LaborStartUtc, LaborEndUtc, location);
            var employeeNumberMap = new Dictionary<string, string>
            {
                ["TMbg6uJB2DvQ7rM0"] = "SANDBOX-FOH",
                ["TMQdYcjHghGEVElc"] = "SANDBOX-KITCHEN",
                ["TMCFkTZd86HFxkf3"] = "SANDBOX-MANAGER"
            };
            var timesheetEntries = timesheetGenerator.CreateTimesheetEntries(
                "SANDBOX_UNIT",
                employeeNumberMap,
                TimecardIds);
            timesheetEntries.Count.ShouldBe(3);
            timesheetEntries.Select(x => x.EmpNo).OrderBy(x => x).ShouldBe(new[] { "SANDBOX-FOH", "SANDBOX-KITCHEN", "SANDBOX-MANAGER" });
            timesheetEntries.Count(x => x.CheckIn.HasValue && x.CheckOut.HasValue).ShouldBe(2);
            timesheetEntries.Count(x => x.CheckIn.HasValue && !x.CheckOut.HasValue).ShouldBe(1);

            var outputDirectory = Path.Combine(Path.GetTempPath(), "fourth-square-sandbox-replay");
            Directory.CreateDirectory(outputDirectory);
            var csvPath = Path.Combine(outputDirectory, $"{RunId}_ReplayFourthCsv.csv");
            var xmlPath = Path.Combine(outputDirectory, $"{RunId}_ReplayFourthTimesheets.xml");
            WriteCsv(replayRows, csvPath);
            var timesheetXml = timesheetGenerator.CreateTimesheetXml(
                "SANDBOX_UNIT",
                DateTime.UtcNow,
                $"square-sandbox-replay-{RunId}",
                employeeNumberMap,
                TimecardIds);
            timesheetXml.Save(xmlPath);

            TestContext.WriteLine($"Replay run ID: {RunId}");
            TestContext.WriteLine($"Sales order window UTC: {SalesStartUtc:O} to {SalesEndUtc:O}");
            TestContext.WriteLine($"Labor timecard window UTC: {LaborStartUtc:O} to {LaborEndUtc:O}");
            TestContext.WriteLine($"Paid orders: {string.Join(", ", PaidOrderIds)}");
            TestContext.WriteLine($"Ignored open order: {IgnoredOpenOrderId}");
            TestContext.WriteLine($"Timecards: {string.Join(", ", TimecardIds)}");
            TestContext.WriteLine($"Replay rows: {replayRows.Count}");
            TestContext.WriteLine($"Replay CSV: {csvPath}");
            TestContext.WriteLine($"Replay timesheet XML: {xmlPath}");
        }

        [Test]
        [Explicit("Replays the expanded 2026-07-04 Square sandbox seed and verifies the read-to-Fourth spreadsheet payload coverage.")]
        public async Task ReplayExpandedSandboxSeed_ThenVerifyFourthSpreadsheetPayloads()
        {
            var config = SquareSandboxTestConfig.Load();
            var client = new SquareClient(config.AccessToken, new ClientOptions { BaseUrl = config.BaseUrl });

            var locationsResponse = await client.Locations.ListAsync();
            var location = locationsResponse.Locations?.FirstOrDefault(x => x.Id == LocationId);
            location.ShouldNotBeNull($"Expected recorded sandbox location {LocationId}.");

            var paymentService = new PaymentsService(config.AccessToken, config.BaseUrl);
            var payments = (await paymentService.GetPaymentsForLocationByDateTimeUTC(
                LocationId,
                ExpandedSalesStartUtc,
                ExpandedSalesEndUtc)).ToList();
            var expectedPaymentIds = ExpandedPaidOrderPaymentIds.Values.ToHashSet();
            var replayPayments = payments.Where(x => expectedPaymentIds.Contains(x.Id)).ToList();
            replayPayments.Count.ShouldBe(ExpandedPaidOrderPaymentIds.Count);
            replayPayments.All(x => x.Status == "COMPLETED").ShouldBeTrue();
            foreach (var orderPayment in ExpandedPaidOrderPaymentIds)
            {
                replayPayments.Count(x => x.Id == orderPayment.Value && x.OrderId == orderPayment.Key).ShouldBe(1);
            }

            var salesGenerator = new SquareToFourthCSVGenerator(config.AccessToken, config.BaseUrl);
            await salesGenerator.GatherDataForBrand();
            var salesGatherResult = await salesGenerator.GatherDataForLocation(
                ExpandedSalesStartUtc,
                ExpandedSalesEndUtc,
                location);
            salesGatherResult.DataGatherResult.ShouldBe(
                DataGatherResult.Complete,
                $"Expected expanded sandbox orders between {ExpandedSalesStartUtc:O} and {ExpandedSalesEndUtc:O}. {salesGatherResult.Exception?.Message}");

            var allRows = salesGenerator.CreateSalesRows("SANDBOX_UNIT").ToList();
            var expandedOrderIds = ExpandedPaidOrderPaymentIds.Keys.ToHashSet();
            var expandedRows = allRows.Where(x => expandedOrderIds.Contains(x.ReceiptCode)).ToList();

            allRows.Any(x => x.ReceiptCode == ExpandedIgnoredOpenOrderId).ShouldBeFalse(
                "The expanded open/unpaid sandbox order should still be excluded from completed-sales output.");
            expandedRows.Count.ShouldBe(52);
            expandedRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.TAB_OPEN).ShouldBe(4);
            expandedRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.TAB_CLOSE).ShouldBe(4);
            expandedRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.SALES_ITEM).ShouldBe(14);
            expandedRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.MODIFIER_ITEM).ShouldBe(13);
            expandedRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.DISC_ITEM).ShouldBe(7);
            expandedRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.SERVICE_CHARGE).ShouldBe(6);
            expandedRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.TENDER).ShouldBe(4);
            expandedRows.Sum(x => x.TenderAmount).ShouldBe(203.02M);
            expandedRows
                .Where(x => x.TransactionTypeCode == TransactionTypeCodes.SERVICE_CHARGE)
                .Sum(x => x.PricePaid)
                .ShouldBe(11.25M);
            expandedRows
                .Where(x => x.TransactionTypeCode == TransactionTypeCodes.SALES_ITEM)
                .Sum(x => x.Deduction)
                .ShouldBe(9.98M);
            expandedRows
                .Where(x => x.TransactionTypeCode == TransactionTypeCodes.DISC_ITEM)
                .Sum(x => x.Deduction)
                .ShouldBeGreaterThanOrEqualTo(9.98M);

            var salesItemPlus = expandedRows
                .Where(x => x.TransactionTypeCode == TransactionTypeCodes.SALES_ITEM)
                .Select(x => x.SalesItemPLU)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToHashSet();
            salesItemPlus.SetEquals(ExpandedProductPlusModifier.Keys).ShouldBeTrue(
                "Every seeded catalog SKU should transfer into a Fourth SALES_ITEM SalesItemPLU.");

            var modifierNames = expandedRows
                .Where(x => x.TransactionTypeCode == TransactionTypeCodes.MODIFIER_ITEM)
                .Select(x => x.SalesItemDesc)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToHashSet();
            ExpandedProductPlusModifier.Values.All(modifierNames.Contains).ShouldBeTrue(
                "Every seeded modifier should transfer into a Fourth MODIFIER_ITEM row.");

            foreach (var orderPayment in ExpandedPaidOrderPaymentIds)
            {
                expandedRows.Count(x =>
                        x.ReceiptCode == orderPayment.Key &&
                        x.TransactionTypeCode == TransactionTypeCodes.TENDER)
                    .ShouldBe(1, $"Order {orderPayment.Key} should emit exactly one Fourth tender row for payment {orderPayment.Value}.");
            }

            var employeeService = new EmployeesService(config.AccessToken, config.BaseUrl);
            var teamMembers = (await employeeService.GetEmployees()).ToList();
            ExpandedTeamMemberEmployeeNumbers.Keys
                .All(teamMemberId => teamMembers.Any(x => x.Id == teamMemberId))
                .ShouldBeTrue("Every seeded Square team member should still be readable from Square.");

            var timesheetGenerator = new SquareToFourthTimesheetXmlGenerator(config.AccessToken, config.BaseUrl);
            await timesheetGenerator.GatherDataForLocation(ExpandedLaborStartUtc, ExpandedLaborEndUtc, location);
            var expandedTimecardIds = timesheetGenerator.Timecards
                .Where(x => ExpandedTeamMemberEmployeeNumbers.ContainsKey(x.TeamMemberId))
                .Select(x => x.Id)
                .ToArray();
            expandedTimecardIds.Length.ShouldBe(70);

            var timesheetEntries = timesheetGenerator.CreateTimesheetEntries(
                "SANDBOX_UNIT",
                ExpandedTeamMemberEmployeeNumbers,
                expandedTimecardIds);
            timesheetEntries.Count.ShouldBe(70);
            timesheetEntries.Select(x => x.EmpNo).Distinct().Count().ShouldBe(10);
            timesheetEntries.Select(x => x.CheckIn?.Date).Distinct().Count().ShouldBe(7);
            timesheetEntries.Count(x => x.CheckIn.HasValue && x.CheckOut.HasValue).ShouldBe(70);
            timesheetEntries.Count(x => x.CheckIn.HasValue && !x.CheckOut.HasValue).ShouldBe(0);

            var outputDirectory = Path.Combine(Path.GetTempPath(), "fourth-square-sandbox-replay");
            Directory.CreateDirectory(outputDirectory);
            var csvPath = Path.Combine(outputDirectory, $"{ExpandedRunId}_ExpandedReplayFourthCsv.csv");
            var xmlPath = Path.Combine(outputDirectory, $"{ExpandedRunId}_ExpandedReplayFourthTimesheets.xml");
            WriteCsv(expandedRows, csvPath);
            var timesheetXml = timesheetGenerator.CreateTimesheetXml(
                "SANDBOX_UNIT",
                DateTime.UtcNow,
                $"square-sandbox-expanded-replay-{ExpandedRunId}",
                ExpandedTeamMemberEmployeeNumbers,
                expandedTimecardIds);
            timesheetXml.Save(xmlPath);

            TestContext.WriteLine($"Expanded replay run ID: {ExpandedRunId}");
            TestContext.WriteLine($"Sales order window UTC: {ExpandedSalesStartUtc:O} to {ExpandedSalesEndUtc:O}");
            TestContext.WriteLine($"Labor timecard window UTC: {ExpandedLaborStartUtc:O} to {ExpandedLaborEndUtc:O}");
            TestContext.WriteLine($"Payments: {string.Join(", ", ExpandedPaidOrderPaymentIds.Select(x => $"{x.Key}/{x.Value}"))}");
            TestContext.WriteLine($"Products: {string.Join(", ", ExpandedProductPlusModifier.Select(x => $"{x.Key}+{x.Value}"))}");
            TestContext.WriteLine($"Team members: {string.Join(", ", ExpandedTeamMemberEmployeeNumbers.Keys)}");
            TestContext.WriteLine($"Timecards: {expandedTimecardIds.Length}");
            TestContext.WriteLine($"Expanded replay rows: {expandedRows.Count}");
            TestContext.WriteLine($"Expanded replay CSV: {csvPath}");
            TestContext.WriteLine($"Expanded replay timesheet XML: {xmlPath}");
        }

        [Test]
        [Explicit("Replays the 2026-07-06 readiness Square sandbox seed and verifies broad sales/hospitality payload coverage.")]
        public async Task ReplayReadinessSandboxSeed_ThenVerifyFourthPayloadCoverage()
        {
            var config = SquareSandboxTestConfig.Load();
            var client = new SquareClient(config.AccessToken, new ClientOptions { BaseUrl = config.BaseUrl });

            var locationsResponse = await client.Locations.ListAsync();
            var location = locationsResponse.Locations?.FirstOrDefault(x => x.Id == LocationId);
            location.ShouldNotBeNull($"Expected recorded sandbox location {LocationId}.");

            var expectedOrderIds = ReadinessPaidOrderIds.ToHashSet();
            var paymentService = new PaymentsService(config.AccessToken, config.BaseUrl);
            var payments = (await paymentService.GetPaymentsForLocationByDateTimeUTC(
                LocationId,
                ReadinessSalesStartUtc,
                ReadinessSalesEndUtc)).ToList();
            var replayPayments = payments.Where(x => expectedOrderIds.Contains(x.OrderId)).ToList();
            replayPayments.Count.ShouldBe(ReadinessPaidOrderIds.Length);
            replayPayments.All(x => x.Status == "COMPLETED").ShouldBeTrue();

            var salesGenerator = new SquareToFourthCSVGenerator(config.AccessToken, config.BaseUrl);
            await salesGenerator.GatherDataForBrand();
            var salesGatherResult = await salesGenerator.GatherDataForLocation(
                ReadinessSalesStartUtc,
                ReadinessSalesEndUtc,
                location);
            salesGatherResult.DataGatherResult.ShouldBe(
                DataGatherResult.Complete,
                $"Expected readiness sandbox orders between {ReadinessSalesStartUtc:O} and {ReadinessSalesEndUtc:O}. {salesGatherResult.Exception?.Message}");

            var allRows = salesGenerator.CreateSalesRows("SANDBOX_UNIT").ToList();
            var readinessRows = allRows.Where(x => expectedOrderIds.Contains(x.ReceiptCode)).ToList();

            ReadinessIgnoredOpenOrderIds
                .Any(openOrderId => allRows.Any(x => x.ReceiptCode == openOrderId))
                .ShouldBeFalse("The readiness open/unpaid sandbox orders should be excluded from completed-sales output.");
            readinessRows.Count.ShouldBe(207);
            readinessRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.TAB_OPEN).ShouldBe(12);
            readinessRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.TAB_CLOSE).ShouldBe(12);
            readinessRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.SALES_ITEM).ShouldBe(65);
            readinessRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.MODIFIER_ITEM).ShouldBe(57);
            readinessRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.DISC_ITEM).ShouldBe(30);
            readinessRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.SERVICE_CHARGE).ShouldBe(19);
            readinessRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.TENDER).ShouldBe(12);
            readinessRows.Sum(x => x.TenderAmount).ShouldBe(1168.71M);
            readinessRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.SALES_ITEM && x.Tax > 0).ShouldBe(2);

            var salesItemPlus = readinessRows
                .Where(x => x.TransactionTypeCode == TransactionTypeCodes.SALES_ITEM)
                .Select(x => x.SalesItemPLU)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToHashSet();
            salesItemPlus.SetEquals(ReadinessProductPlusModifier.Keys).ShouldBeTrue(
                "Every readiness catalog SKU should transfer into a Fourth SALES_ITEM SalesItemPLU.");

            var modifierNames = readinessRows
                .Where(x => x.TransactionTypeCode == TransactionTypeCodes.MODIFIER_ITEM)
                .Select(x => x.SalesItemDesc)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToHashSet();
            ReadinessProductPlusModifier.Values.All(modifierNames.Contains).ShouldBeTrue(
                "Every readiness modifier should transfer into a Fourth MODIFIER_ITEM row.");

            foreach (var orderId in ReadinessPaidOrderIds)
            {
                readinessRows.Count(x =>
                        x.ReceiptCode == orderId &&
                        x.TransactionTypeCode == TransactionTypeCodes.TENDER)
                    .ShouldBe(1, $"Order {orderId} should emit exactly one Fourth tender row.");
            }

            var employeeService = new EmployeesService(config.AccessToken, config.BaseUrl);
            var teamMembers = (await employeeService.GetEmployees()).ToList();
            ReadinessTeamMemberEmployeeNumbers.Keys
                .All(teamMemberId => teamMembers.Any(x => x.Id == teamMemberId))
                .ShouldBeTrue("Every readiness Square team member should still be readable from Square.");

            var timesheetGenerator = new SquareToFourthTimesheetXmlGenerator(config.AccessToken, config.BaseUrl);
            await timesheetGenerator.GatherDataForLocation(ReadinessLaborStartUtc, ReadinessLaborEndUtc, location);
            var readinessTimecardIds = timesheetGenerator.Timecards
                .Where(x => ReadinessTeamMemberEmployeeNumbers.ContainsKey(x.TeamMemberId))
                .Select(x => x.Id)
                .ToArray();
            readinessTimecardIds.Length.ShouldBe(215);

            var timesheetEntries = timesheetGenerator.CreateTimesheetEntries(
                "SANDBOX_UNIT",
                ReadinessTeamMemberEmployeeNumbers,
                readinessTimecardIds);
            timesheetEntries.Count.ShouldBe(215);
            timesheetEntries.Select(x => x.EmpNo).Distinct().Count().ShouldBe(15);
            timesheetEntries.Select(x => x.CheckIn?.Date).Distinct().Count().ShouldBe(15);
            timesheetEntries.Count(x => x.CheckIn.HasValue && x.CheckOut.HasValue).ShouldBe(210);
            timesheetEntries.Count(x => x.CheckIn.HasValue && !x.CheckOut.HasValue).ShouldBe(5);

            var outputDirectory = Path.Combine(Path.GetTempPath(), "fourth-square-sandbox-replay");
            Directory.CreateDirectory(outputDirectory);
            var csvPath = Path.Combine(outputDirectory, $"{ReadinessRunId}_ReadinessReplayFourthCsv.csv");
            var xmlPath = Path.Combine(outputDirectory, $"{ReadinessRunId}_ReadinessReplayFourthTimesheets.xml");
            WriteCsv(readinessRows, csvPath);
            var timesheetXml = timesheetGenerator.CreateTimesheetXml(
                "SANDBOX_UNIT",
                DateTime.UtcNow,
                $"square-sandbox-readiness-replay-{ReadinessRunId}",
                ReadinessTeamMemberEmployeeNumbers,
                readinessTimecardIds);
            timesheetXml.Save(xmlPath);

            TestContext.WriteLine($"Readiness replay run ID: {ReadinessRunId}");
            TestContext.WriteLine($"Sales order window UTC: {ReadinessSalesStartUtc:O} to {ReadinessSalesEndUtc:O}");
            TestContext.WriteLine($"Labor timecard window UTC: {ReadinessLaborStartUtc:O} to {ReadinessLaborEndUtc:O}");
            TestContext.WriteLine($"Paid orders: {string.Join(", ", ReadinessPaidOrderIds)}");
            TestContext.WriteLine($"Ignored open orders: {string.Join(", ", ReadinessIgnoredOpenOrderIds)}");
            TestContext.WriteLine($"Products: {string.Join(", ", ReadinessProductPlusModifier.Select(x => $"{x.Key}+{x.Value}"))}");
            TestContext.WriteLine($"Team members: {string.Join(", ", ReadinessTeamMemberEmployeeNumbers.Keys)}");
            TestContext.WriteLine($"Timecards: {readinessTimecardIds.Length}");
            TestContext.WriteLine($"Readiness replay rows: {readinessRows.Count}");
            TestContext.WriteLine($"Readiness replay CSV: {csvPath}");
            TestContext.WriteLine($"Readiness replay timesheet XML: {xmlPath}");
        }

        [Test]
        [Explicit("Replays the 2026-07-06 Square sandbox payment/refund seed and verifies tender/refund coverage in Fourth CSV output.")]
        public async Task ReplayPaymentRefundSandboxSeed_ThenVerifyTenderAndRefundCoverage()
        {
            var config = SquareSandboxTestConfig.Load();
            var client = new SquareClient(config.AccessToken, new ClientOptions { BaseUrl = config.BaseUrl });

            var locationsResponse = await client.Locations.ListAsync();
            var location = locationsResponse.Locations?.FirstOrDefault(x => x.Id == LocationId);
            location.ShouldNotBeNull($"Expected recorded sandbox location {LocationId}.");

            var expectedOrderIds = PaymentRefundPaidOrderIds.ToHashSet();
            var paymentService = new PaymentsService(config.AccessToken, config.BaseUrl);
            var payments = (await paymentService.GetPaymentsForLocationByDateTimeUTC(
                LocationId,
                PaymentRefundSalesStartUtc,
                PaymentRefundSalesEndUtc)).ToList();
            var replayPayments = payments.Where(x => expectedOrderIds.Contains(x.OrderId)).ToList();
            replayPayments.Count.ShouldBe(PaymentRefundPaidOrderIds.Length);
            replayPayments.All(x => x.Status == "COMPLETED").ShouldBeTrue();
            replayPayments.Select(x => x.SourceType).Distinct().OrderBy(x => x).ShouldBe(new[] { "CARD", "CASH", "EXTERNAL" });

            var refundService = new RefundsService(config.AccessToken, config.BaseUrl);
            var refunds = (await refundService.GetRefundsForLocationByDateTimeUTC(
                LocationId,
                PaymentRefundSalesStartUtc,
                PaymentRefundSalesEndUtc)).ToList();
            var replayPaymentIds = replayPayments.Select(x => x.Id).ToHashSet();
            var replayRefunds = refunds.Where(x => replayPaymentIds.Contains(x.PaymentId)).ToList();
            replayRefunds.Count.ShouldBe(3);
            replayRefunds.Sum(x => x.AmountMoney?.Amount ?? 0).ShouldBe(1450);

            var salesGenerator = new SquareToFourthCSVGenerator(config.AccessToken, config.BaseUrl);
            await salesGenerator.GatherDataForBrand();
            var salesGatherResult = await salesGenerator.GatherDataForLocation(
                PaymentRefundSalesStartUtc,
                PaymentRefundSalesEndUtc,
                location);
            salesGatherResult.DataGatherResult.ShouldBe(
                DataGatherResult.Complete,
                $"Expected payment/refund sandbox orders between {PaymentRefundSalesStartUtc:O} and {PaymentRefundSalesEndUtc:O}. {salesGatherResult.Exception?.Message}");

            var allRows = salesGenerator.CreateSalesRows("SANDBOX_UNIT").ToList();
            var replayRows = allRows.Where(x => expectedOrderIds.Contains(x.ReceiptCode)).ToList();

            PaymentRefundIgnoredOpenOrderIds
                .Any(openOrderId => allRows.Any(x => x.ReceiptCode == openOrderId))
                .ShouldBeFalse("The payment/refund open/unpaid sandbox orders should be excluded from completed-sales output.");
            replayRows.Count.ShouldBe(144);
            replayRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.TAB_OPEN).ShouldBe(10);
            replayRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.TAB_CLOSE).ShouldBe(10);
            replayRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.SALES_ITEM).ShouldBe(43);
            replayRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.MODIFIER_ITEM).ShouldBe(36);
            replayRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.DISC_ITEM).ShouldBe(18);
            replayRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.SERVICE_CHARGE).ShouldBe(14);
            replayRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.TENDER).ShouldBe(13);
            replayRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.TENDER && x.TenderAmount > 0).ShouldBe(10);
            replayRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.TENDER && x.TenderAmount < 0).ShouldBe(3);
            replayRows
                .Where(x => x.TransactionTypeCode == TransactionTypeCodes.TENDER && x.TenderAmount > 0)
                .Select(x => x.TenderTypeCode)
                .Distinct()
                .OrderBy(x => x)
                .ShouldBe(new[] { "CARD", "CASH", "EXTERNAL" });
            replayRows
                .Where(x => x.TransactionTypeCode == TransactionTypeCodes.TENDER && x.TenderAmount < 0)
                .Select(x => x.TenderTypeCode)
                .Distinct()
                .OrderBy(x => x)
                .ShouldBe(new[] { "CARD_REFUND", "CASH_REFUND", "EXTERNAL_REFUND" });
            replayRows
                .Where(x => x.TransactionTypeCode == TransactionTypeCodes.TENDER && x.TenderAmount > 0)
                .Sum(x => x.TenderAmount)
                .ShouldBe(823.69M);
            replayRows
                .Where(x => x.TransactionTypeCode == TransactionTypeCodes.TENDER && x.TenderAmount < 0)
                .Sum(x => x.TenderAmount)
                .ShouldBe(-14.50M);

            var salesItemPlus = replayRows
                .Where(x => x.TransactionTypeCode == TransactionTypeCodes.SALES_ITEM)
                .Select(x => x.SalesItemPLU)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToHashSet();
            salesItemPlus.SetEquals(ReadinessProductPlusModifier.Keys).ShouldBeTrue(
                "Every payment/refund catalog SKU should transfer into a Fourth SALES_ITEM SalesItemPLU.");

            var modifierNames = replayRows
                .Where(x => x.TransactionTypeCode == TransactionTypeCodes.MODIFIER_ITEM)
                .Select(x => x.SalesItemDesc)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToHashSet();
            ReadinessProductPlusModifier.Values.All(modifierNames.Contains).ShouldBeTrue(
                "Every payment/refund modifier should transfer into a Fourth MODIFIER_ITEM row.");

            var timesheetGenerator = new SquareToFourthTimesheetXmlGenerator(config.AccessToken, config.BaseUrl);
            await timesheetGenerator.GatherDataForLocation(PaymentRefundLaborStartUtc, PaymentRefundLaborEndUtc, location);
            var timecardIds = timesheetGenerator.Timecards
                .Where(x => PaymentRefundTeamMemberEmployeeNumbers.ContainsKey(x.TeamMemberId))
                .Select(x => x.Id)
                .ToArray();
            timecardIds.Length.ShouldBe(215);

            var timesheetEntries = timesheetGenerator.CreateTimesheetEntries(
                "SANDBOX_UNIT",
                PaymentRefundTeamMemberEmployeeNumbers,
                timecardIds);
            timesheetEntries.Count.ShouldBe(215);
            timesheetEntries.Select(x => x.EmpNo).Distinct().Count().ShouldBe(15);
            timesheetEntries.Select(x => x.CheckIn?.Date).Distinct().Count().ShouldBe(15);
            timesheetEntries.Count(x => x.CheckIn.HasValue && x.CheckOut.HasValue).ShouldBe(210);
            timesheetEntries.Count(x => x.CheckIn.HasValue && !x.CheckOut.HasValue).ShouldBe(5);

            var outputDirectory = Path.Combine(Path.GetTempPath(), "fourth-square-sandbox-replay");
            Directory.CreateDirectory(outputDirectory);
            var csvPath = Path.Combine(outputDirectory, $"{PaymentRefundRunId}_PaymentRefundReplayFourthCsv.csv");
            var xmlPath = Path.Combine(outputDirectory, $"{PaymentRefundRunId}_PaymentRefundReplayFourthTimesheets.xml");
            WriteCsv(replayRows, csvPath);
            var timesheetXml = timesheetGenerator.CreateTimesheetXml(
                "SANDBOX_UNIT",
                DateTime.UtcNow,
                $"square-sandbox-payment-refund-replay-{PaymentRefundRunId}",
                PaymentRefundTeamMemberEmployeeNumbers,
                timecardIds);
            timesheetXml.Save(xmlPath);

            TestContext.WriteLine($"Payment/refund replay run ID: {PaymentRefundRunId}");
            TestContext.WriteLine($"Sales order window UTC: {PaymentRefundSalesStartUtc:O} to {PaymentRefundSalesEndUtc:O}");
            TestContext.WriteLine($"Labor timecard window UTC: {PaymentRefundLaborStartUtc:O} to {PaymentRefundLaborEndUtc:O}");
            TestContext.WriteLine($"Payments: {string.Join(", ", replayPayments.Select(x => $"{x.OrderId}/{x.Id}/{x.SourceType}"))}");
            TestContext.WriteLine($"Refunds: {string.Join(", ", replayRefunds.Select(x => $"{x.OrderId}/{x.PaymentId}/{x.Id}/{x.AmountMoney?.Amount}/{x.Status}"))}");
            TestContext.WriteLine($"Payment/refund replay rows: {replayRows.Count}");
            TestContext.WriteLine($"Payment/refund replay CSV: {csvPath}");
            TestContext.WriteLine($"Payment/refund replay timesheet XML: {xmlPath}");
        }

        private static void WriteCsv(IEnumerable<TransactionDatasetRow> rows, string fullPath)
        {
            using var writer = new StreamWriter(fullPath);
            using var csv = new CsvWriter(writer, CultureInfo.CurrentCulture);
            csv.WriteRecords(rows);
            csv.Flush();
        }
    }
}
