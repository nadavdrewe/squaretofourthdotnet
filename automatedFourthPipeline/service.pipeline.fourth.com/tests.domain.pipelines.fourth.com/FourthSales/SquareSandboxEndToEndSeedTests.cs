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
using Square;
using Square.Catalog.Object;
using Square.Labor;
using Square.Payments;
using Square.Refunds;

namespace tests.domain.pipelines.fourth.com.FourthSales
{
    [TestFixture]
    [Category("Sandbox")]
    public class SquareSandboxEndToEndSeedTests
    {
        [Test]
        [Explicit("Creates rich sandbox catalog, order/payment, team member, and timecard data, then checks the Square-to-Fourth CSV pipeline.")]
        public async Task SeedSandboxSquareData_ThenGenerateFourthCsv()
        {
            var config = SquareSandboxTestConfig.Load();
            var client = new SquareClient(config.AccessToken, new ClientOptions { BaseUrl = config.BaseUrl });

            var locationsResponse = await client.Locations.ListAsync();
            var location = locationsResponse.Locations?.FirstOrDefault();
            location.ShouldNotBeNull("The sandbox token must have at least one Square location.");

            var locationId = location.Id;
            var runId = DateTime.UtcNow.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            var seedStartUtc = DateTime.UtcNow.AddMinutes(-2);
            TestContext.WriteLine($"Run ID: {runId}");
            TestContext.WriteLine($"Seed start UTC: {seedStartUtc:O}");
            var foodCategoryId = await UpsertCategoryAsync(client, "Food", runId);
            var drinkCategoryId = await UpsertCategoryAsync(client, "Drinks", runId);
            var breakfastCategoryId = await UpsertCategoryAsync(client, "Breakfast", runId);
            var retailCategoryId = await UpsertCategoryAsync(client, "Retail", runId);
            var seededMenuItems = new List<SeededMenuItem>
            {
                new("Burger", "Regular", "FOURTH-SANDBOX-BURGER", 1200, foodCategoryId, "Cheese", 100),
                new("Fries", "Large", "FOURTH-SANDBOX-FRIES", 500, foodCategoryId, "Truffle Salt", 75),
                new("Coffee", "Flat White", "FOURTH-SANDBOX-COFFEE", 350, drinkCategoryId, "Oat Milk", 50),
                new("Tea", "Breakfast", "FOURTH-SANDBOX-TEA", 250, drinkCategoryId, "Honey", 25),
                new("Pasta", "Rigatoni", "FOURTH-SANDBOX-PASTA", 1450, foodCategoryId, "Parmesan", 125),
                new("Salad", "Caesar", "FOURTH-SANDBOX-SALAD", 900, foodCategoryId, "Chicken", 350),
                new("Steak", "Sirloin", "FOURTH-SANDBOX-STEAK", 2800, foodCategoryId, "Peppercorn Sauce", 250),
                new("Dessert", "Tiramisu", "FOURTH-SANDBOX-DESSERT", 700, foodCategoryId, "Birthday Plate", 150),
                new("Juice", "Orange", "FOURTH-SANDBOX-JUICE", 450, drinkCategoryId, "Ginger Shot", 100),
                new("Wine", "House Red", "FOURTH-SANDBOX-WINE", 850, drinkCategoryId, "Large Glass", 200),
                new("Eggs", "Poached", "FOURTH-SANDBOX-EGGS", 1100, breakfastCategoryId, "Smoked Salmon", 450),
                new("Pancakes", "Stack", "FOURTH-SANDBOX-PANCAKES", 950, breakfastCategoryId, "Maple Syrup", 125),
                new("Granola", "Bowl", "FOURTH-SANDBOX-GRANOLA", 650, breakfastCategoryId, "Greek Yoghurt", 150),
                new("Soup", "Tomato", "FOURTH-SANDBOX-SOUP", 625, foodCategoryId, "Sourdough", 175),
                new("Fish", "Sea Bass", "FOURTH-SANDBOX-FISH", 2400, foodCategoryId, "Lemon Butter", 225),
                new("Cocktail", "Negroni", "FOURTH-SANDBOX-COCKTAIL", 1250, drinkCategoryId, "Premium Gin", 300),
                new("Beer", "Pint", "FOURTH-SANDBOX-BEER", 650, drinkCategoryId, "Lime", 50),
                new("Merch", "Tote Bag", "FOURTH-SANDBOX-MERCH", 1500, retailCategoryId, "Gift Wrap", 100),
                new("Voucher", "Gift Card", "FOURTH-SANDBOX-VOUCHER", 2500, retailCategoryId, "Envelope", 50),
                new("Water", "Sparkling", "FOURTH-SANDBOX-WATER", 300, drinkCategoryId, "Ice", 0)
            };

            for (var i = 0; i < seededMenuItems.Count; i++)
            {
                var item = seededMenuItems[i];
                seededMenuItems[i] = item with
                {
                    VariationId = await UpsertItemAsync(client, item.CategoryId, item.Name, item.VariationName, item.Sku, item.PriceMinor, runId)
                };
            }

            var burgerVariationId = seededMenuItems.Single(x => x.Name == "Burger").VariationId;
            var friesVariationId = seededMenuItems.Single(x => x.Name == "Fries").VariationId;
            var coffeeVariationId = seededMenuItems.Single(x => x.Name == "Coffee").VariationId;
            var teaVariationId = seededMenuItems.Single(x => x.Name == "Tea").VariationId;
            var eggsVariationId = seededMenuItems.Single(x => x.Name == "Eggs").VariationId;
            var pancakesVariationId = seededMenuItems.Single(x => x.Name == "Pancakes").VariationId;
            var cocktailVariationId = seededMenuItems.Single(x => x.Name == "Cocktail").VariationId;
            var beerVariationId = seededMenuItems.Single(x => x.Name == "Beer").VariationId;
            var merchVariationId = seededMenuItems.Single(x => x.Name == "Merch").VariationId;
            var soupVariationId = seededMenuItems.Single(x => x.Name == "Soup").VariationId;
            var voucherVariationId = seededMenuItems.Single(x => x.Name == "Voucher").VariationId;
            var seededEmployees = await CreateTeamMembersAsync(client, locationId, runId);
            seededEmployees.Count.ShouldBe(15);
            var frontOfHouseTeamMemberId = seededEmployees[0].TeamMemberId;
            var kitchenTeamMemberId = seededEmployees[1].TeamMemberId;
            var managerTeamMemberId = seededEmployees[2].TeamMemberId;
            var discountedHospitalityOrder = await CreateDiscountedHospitalityOrderAsync(
                client,
                locationId,
                burgerVariationId,
                friesVariationId,
                frontOfHouseTeamMemberId,
                runId);
            var coffeeOrder = await CreateCoffeeOrderAsync(client, locationId, coffeeVariationId, kitchenTeamMemberId, runId);
            var edgeCaseOrder = await CreateEdgeCaseOrderAsync(client, locationId, teaVariationId, managerTeamMemberId, runId);
            var fullMenuModifierOrder = await CreateFullMenuModifierOrderAsync(
                client,
                locationId,
                seededMenuItems,
                seededEmployees[3].TeamMemberId,
                runId);
            var breakfastOrder = await CreateBreakfastTaxedOrderAsync(
                client,
                locationId,
                eggsVariationId,
                pancakesVariationId,
                seededEmployees[4].TeamMemberId,
                runId);
            var barRoundOrder = await CreateBarRoundOrderAsync(
                client,
                locationId,
                cocktailVariationId,
                beerVariationId,
                seededEmployees[5].TeamMemberId,
                runId);
            var retailOrder = await CreateRetailOrderAsync(
                client,
                locationId,
                merchVariationId,
                seededEmployees[6].TeamMemberId,
                runId);
            var cateringOrder = await CreateCateringOrderAsync(
                client,
                locationId,
                seededMenuItems,
                seededEmployees[7].TeamMemberId,
                runId);
            var cashOrder = await CreateCashOrderAsync(
                client,
                locationId,
                soupVariationId,
                seededEmployees[8].TeamMemberId,
                runId);
            var externalOrder = await CreateExternalPaymentOrderAsync(
                client,
                locationId,
                voucherVariationId,
                seededEmployees[9].TeamMemberId,
                runId);
            discountedHospitalityOrder = discountedHospitalityOrder with
            {
                Refunds = new[]
                {
                    await RefundPaymentAsync(
                        client,
                        discountedHospitalityOrder.Payment,
                        Money(250),
                        frontOfHouseTeamMemberId,
                        "Sandbox partial card refund")
                }
            };
            cashOrder = cashOrder with
            {
                Refunds = new[]
                {
                    await RefundPaymentAsync(
                        client,
                        cashOrder.Payment,
                        Money(500),
                        seededEmployees[8].TeamMemberId,
                        "Sandbox cash refund")
                }
            };
            externalOrder = externalOrder with
            {
                Refunds = new[]
                {
                    await RefundPaymentAsync(
                        client,
                        externalOrder.Payment,
                        Money(700),
                        seededEmployees[9].TeamMemberId,
                        "Sandbox external refund")
                }
            };
            var unpaidOpenOrder = await CreateUnpaidOpenOrderAsync(client, locationId, coffeeVariationId, runId);
            var secondUnpaidOpenOrder = await CreateUnpaidOpenOrderAsync(client, locationId, teaVariationId, runId);
            var laborStartUtc = new DateTimeOffset(DateTime.UtcNow.Date.AddDays(-14).AddHours(8), TimeSpan.Zero);
            var laborEndUtc = DateTimeOffset.UtcNow.AddMinutes(15);
            var seededTimecards = await CreateClosedTimecardsAsync(
                client,
                locationId,
                seededEmployees,
                laborStartUtc,
                numberOfDays: 14,
                runId);
            var openTimecards = await CreateOpenTimecardsAsync(
                client,
                locationId,
                seededEmployees.Take(5).ToList(),
                runId);

            var startUtc = seedStartUtc;
            var endUtc = DateTime.UtcNow.AddMinutes(15);

            var generator = new SquareToFourthCSVGenerator(config.AccessToken, config.BaseUrl);
            await generator.GatherDataForBrand();

            var gatherResult = await generator.GatherDataForLocation(startUtc, endUtc, location);
            gatherResult.DataGatherResult.ShouldBe(
                DataGatherResult.Complete,
                $"Expected sandbox orders between {startUtc:O} and {endUtc:O}. {gatherResult.Exception?.Message}");

            var rows = generator.CreateSalesRows("SANDBOX_UNIT").ToList();
            var seededOrderIds = new HashSet<string>
            {
                discountedHospitalityOrder.Order.Id,
                coffeeOrder.Order.Id,
                edgeCaseOrder.Order.Id,
                fullMenuModifierOrder.Order.Id,
                breakfastOrder.Order.Id,
                barRoundOrder.Order.Id,
                retailOrder.Order.Id,
                cateringOrder.Order.Id,
                cashOrder.Order.Id,
                externalOrder.Order.Id
            };
            var seededRows = rows.Where(x => seededOrderIds.Contains(x.ReceiptCode)).ToList();
            rows.Any(x => x.ReceiptCode == unpaidOpenOrder.Id).ShouldBeFalse("Open/unpaid Square orders should not be emitted by the completed-sales pipeline.");
            rows.Any(x => x.ReceiptCode == secondUnpaidOpenOrder.Id).ShouldBeFalse("Every open/unpaid Square order should be excluded from completed-sales output.");
            var paidOrders = new[]
            {
                discountedHospitalityOrder,
                coffeeOrder,
                edgeCaseOrder,
                fullMenuModifierOrder,
                breakfastOrder,
                barRoundOrder,
                retailOrder,
                cateringOrder,
                cashOrder,
                externalOrder
            };
            paidOrders.All(x => x.Payment != null && x.Payment.Status == "COMPLETED").ShouldBeTrue(
                "Every paid seeded order must have a completed Square payment.");
            paidOrders.All(x => !string.IsNullOrWhiteSpace(x.Payment.Id)).ShouldBeTrue(
                "Every paid seeded order must expose a Square payment id for traceability.");
            paidOrders.All(x => x.Payment.OrderId == x.Order.Id).ShouldBeTrue(
                "Every completed Square payment must be attached to its seeded order.");
            var refundedOrders = paidOrders.Where(x => x.Refunds.Count > 0).ToArray();
            refundedOrders.Length.ShouldBe(3);
            refundedOrders.Sum(x => x.Refunds.Count).ShouldBe(3);
            refundedOrders.SelectMany(x => x.Refunds).All(x => x.Status is "COMPLETED" or "APPROVED" or "PENDING").ShouldBeTrue(
                "Every seeded refund should be accepted by Square for downstream replay.");
            var expectedTenderTotal = paidOrders.Sum(ExpectedTenderTotal);
            var expectedServiceChargeTotal = paidOrders.Sum(ExpectedServiceChargeTotal);
            var expectedDiscountTotal = paidOrders.Sum(ExpectedDiscountTotal);

            var seededProductDescriptions = seededMenuItems
                .Select(x => $"Fourth Sandbox {x.Name} - {x.VariationName}")
                .ToList();
            seededRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.TAB_OPEN).ShouldBe(paidOrders.Length);
            seededRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.TAB_CLOSE).ShouldBe(paidOrders.Length);
            seededRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.TENDER && x.TenderAmount > 0).ShouldBe(paidOrders.Length);
            seededRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.TENDER && x.TenderAmount < 0).ShouldBe(refundedOrders.Sum(x => x.Refunds.Count));
            seededRows
                .Where(x => x.TransactionTypeCode == TransactionTypeCodes.TENDER && x.TenderAmount > 0)
                .Select(x => x.TenderTypeCode)
                .Distinct()
                .OrderBy(x => x)
                .ShouldBe(new[] { "CARD", "CASH", "EXTERNAL" });
            seededRows
                .Where(x => x.TransactionTypeCode == TransactionTypeCodes.TENDER && x.TenderAmount < 0)
                .Select(x => x.TenderTypeCode)
                .Distinct()
                .OrderBy(x => x)
                .ShouldBe(new[] { "CARD_REFUND", "CASH_REFUND", "EXTERNAL_REFUND" });
            foreach (var paidOrder in paidOrders)
            {
                seededRows.Count(x => x.ReceiptCode == paidOrder.Order.Id && x.TransactionTypeCode == TransactionTypeCodes.TENDER && x.TenderAmount > 0)
                    .ShouldBe(1, $"Order {paidOrder.Order.Id} should emit exactly one Fourth tender row for Square payment {paidOrder.Payment.Id}.");
            }

            seededRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.SALES_ITEM).ShouldBeGreaterThanOrEqualTo(seededMenuItems.Count);
            seededRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.MODIFIER_ITEM).ShouldBeGreaterThanOrEqualTo(seededMenuItems.Count);
            seededRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.DISC_ITEM).ShouldBeGreaterThanOrEqualTo(8);
            seededRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.SERVICE_CHARGE).ShouldBeGreaterThanOrEqualTo(10);
            seededRows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.SALES_ITEM && x.Tax > 0).ShouldBeGreaterThanOrEqualTo(2);
            seededProductDescriptions
                .All(product => seededRows.Any(x => x.TransactionTypeCode == TransactionTypeCodes.SALES_ITEM && x.SalesItemDesc == product))
                .ShouldBeTrue("Every seeded product should be emitted as a Fourth SALES_ITEM row.");
            seededRows
                .Where(x => x.TransactionTypeCode == TransactionTypeCodes.SALES_ITEM)
                .Select(x => x.SalesItemDesc)
                .Distinct()
                .Count()
                .ShouldBeGreaterThanOrEqualTo(seededMenuItems.Count);
            seededRows.Where(x => x.TransactionTypeCode == TransactionTypeCodes.TENDER && x.TenderAmount > 0).Sum(x => x.TenderAmount).ShouldBe(expectedTenderTotal);
            seededRows.Where(x => x.TransactionTypeCode == TransactionTypeCodes.TENDER && x.TenderAmount < 0).Sum(x => x.TenderAmount).ShouldBe(paidOrders.SelectMany(x => x.Refunds).Sum(x => MoneyToMajor(x.AmountMoney)) * -1);
            seededRows
                .Where(x => x.TransactionTypeCode == TransactionTypeCodes.SERVICE_CHARGE)
                .Sum(x => x.PricePaid)
                .ShouldBe(expectedServiceChargeTotal);
            seededRows
                .Where(x => x.TransactionTypeCode == TransactionTypeCodes.SALES_ITEM)
                .Sum(x => x.Deduction)
                .ShouldBe(expectedDiscountTotal);
            seededRows
                .Where(x => x.TransactionTypeCode == TransactionTypeCodes.DISC_ITEM)
                .Sum(x => x.Deduction)
                .ShouldBeGreaterThanOrEqualTo(expectedDiscountTotal);

            var outputDirectory = Path.Combine(Path.GetTempPath(), "fourth-square-sandbox-seed");
            Directory.CreateDirectory(outputDirectory);
            var csvPath = Path.Combine(outputDirectory, $"{runId}_SquareSandboxFourthCsv.csv");
            WriteCsv(rows, csvPath);

            var timesheetGenerator = new SquareToFourthTimesheetXmlGenerator(config.AccessToken, config.BaseUrl);
            await timesheetGenerator.GatherDataForLocation(laborStartUtc.UtcDateTime.AddHours(-1), laborEndUtc.UtcDateTime, location);
            var seededTimecardIds = seededTimecards.Concat(openTimecards).Select(x => x.Id).ToArray();
            var employeeNumberMap = seededEmployees.ToDictionary(x => x.TeamMemberId, x => x.FourthEmployeeNumber);
            var timesheetEntries = timesheetGenerator.CreateTimesheetEntries(
                "SANDBOX_UNIT",
                employeeNumberMap,
                seededTimecardIds);
            timesheetEntries.Count.ShouldBe(215);
            timesheetEntries.Select(x => x.EmpNo).Distinct().Count().ShouldBe(15);
            timesheetEntries.Select(x => x.CheckIn?.Date).Distinct().Count().ShouldBeGreaterThanOrEqualTo(14);
            timesheetEntries.Count(x => x.CheckIn.HasValue && x.CheckOut.HasValue).ShouldBe(210);
            timesheetEntries.Count(x => x.CheckIn.HasValue && !x.CheckOut.HasValue).ShouldBe(5);

            var xmlPath = Path.Combine(outputDirectory, $"{runId}_SquareSandboxFourthTimesheets.xml");
            var timesheetXml = timesheetGenerator.CreateTimesheetXml(
                "SANDBOX_UNIT",
                DateTime.UtcNow,
                $"square-sandbox-{runId}",
                employeeNumberMap,
                seededTimecardIds);
            timesheetXml.Save(xmlPath);

            TestContext.WriteLine($"Sandbox location: {location.Name} ({locationId})");
            TestContext.WriteLine($"Run ID: {runId}");
            TestContext.WriteLine($"Sales order window UTC: {startUtc:O} to {endUtc:O}");
            TestContext.WriteLine($"Labor timecard window UTC: {laborStartUtc.UtcDateTime.AddHours(-1):O} to {laborEndUtc.UtcDateTime:O}");
            TestContext.WriteLine($"Catalog variations ({seededMenuItems.Count}): {string.Join(", ", seededMenuItems.Select(x => $"{x.Name}:{x.VariationId}"))}");
            TestContext.WriteLine($"Seeded products: {string.Join(", ", seededMenuItems.Select(x => $"{x.Name}/{x.VariationName}+{x.ModifierName}"))}");
            TestContext.WriteLine($"Paid orders/payments: {string.Join(", ", paidOrders.Select(x => $"{x.Order.Id}/{x.Payment.Id}"))}");
            TestContext.WriteLine($"Refunds: {string.Join(", ", paidOrders.SelectMany(x => x.Refunds).Select(x => $"{x.OrderId}/{x.PaymentId}/{x.Id}/{x.AmountMoney?.Amount}/{x.Status}"))}");
            TestContext.WriteLine($"Ignored open orders: {unpaidOpenOrder.Id}, {secondUnpaidOpenOrder.Id}");
            TestContext.WriteLine($"Team members ({seededEmployees.Count}): {string.Join(", ", seededEmployees.Select(x => x.TeamMemberId))}");
            TestContext.WriteLine($"Timecards ({seededTimecardIds.Length}): {string.Join(", ", seededTimecardIds)}");
            TestContext.WriteLine($"Open timecards ({openTimecards.Count}): {string.Join(", ", openTimecards.Select(x => x.Id))}");
            TestContext.WriteLine($"Timesheet entries: {timesheetEntries.Count}");
            TestContext.WriteLine($"Seeded rows: {seededRows.Count}");
            foreach (var row in seededRows.OrderBy(x => x.ReceiptCode).ThenBy(x => x.TransactionTypeCode).ThenBy(x => x.SalesItemDesc))
            {
                TestContext.WriteLine($"{row.ReceiptCode} | {row.TransactionTypeCode} | {row.SalesItemDesc} | tender={row.TenderAmount} | paid={row.PricePaid} | deduction={row.Deduction}");
            }

            TestContext.WriteLine($"CSV: {csvPath}");
            TestContext.WriteLine($"Timesheet XML: {xmlPath}");
        }

        private static async Task<string> UpsertCategoryAsync(SquareClient client, string categoryName, string runId)
        {
            var categoryId = $"#fourth-sandbox-category-{categoryName}-{runId}";
            var response = await client.Catalog.Object.UpsertAsync(new UpsertCatalogObjectRequest
            {
                IdempotencyKey = Guid.NewGuid().ToString("N"),
                Object = new CatalogObject(new CatalogObject.Category(new CatalogObjectCategory
                {
                    Id = categoryId,
                    PresentAtAllLocations = true,
                    CategoryData = new CatalogCategory
                    {
                        Name = $"Fourth Sandbox {categoryName}"
                    }
                }))
            });

            response.Errors.ShouldBeNull();
            return response.CatalogObject.AsCategory().Id;
        }

        private static async Task<string> UpsertItemAsync(
            SquareClient client,
            string categoryId,
            string itemName,
            string variationName,
            string sku,
            long price,
            string runId)
        {
            var itemId = $"#fourth-sandbox-item-{sku}-{runId}";
            var variationId = $"#fourth-sandbox-variation-{sku}-{runId}";
            var response = await client.Catalog.Object.UpsertAsync(new UpsertCatalogObjectRequest
            {
                IdempotencyKey = Guid.NewGuid().ToString("N"),
                Object = new CatalogObject(new CatalogObject.Item(new CatalogObjectItem
                {
                    Id = itemId,
                    PresentAtAllLocations = true,
                    ItemData = new CatalogItem
                    {
                        Name = $"Fourth Sandbox {itemName}",
                        CategoryId = categoryId,
                        Variations = new[]
                        {
                            new CatalogObject(new CatalogObject.ItemVariation(new CatalogObjectItemVariation
                            {
                                Id = variationId,
                                PresentAtAllLocations = true,
                                ItemVariationData = new CatalogItemVariation
                                {
                                    ItemId = itemId,
                                    Name = variationName,
                                    Sku = sku,
                                    PricingType = CatalogPricingType.FixedPricing,
                                    PriceMoney = Money(price)
                                }
                            }))
                        }
                    }
                }))
            });

            response.Errors.ShouldBeNull();
            return response.CatalogObject.AsItem().ItemData.Variations.Single().AsItemVariation().Id;
        }

        private static async Task<List<SeededEmployee>> CreateTeamMembersAsync(
            SquareClient client,
            string locationId,
            string runId)
        {
            var roles = new[]
            {
                "Front Of House",
                "Kitchen",
                "Manager",
                "Bar",
                "Host",
                "Runner",
                "Supervisor",
                "Breakfast",
                "Events",
                "Closing",
                "Pastry",
                "Cellar",
                "Retail",
                "Delivery",
                "Training"
            };

            var employees = new List<SeededEmployee>();
            for (var i = 0; i < roles.Length; i++)
            {
                var teamMemberId = await CreateTeamMemberAsync(client, locationId, roles[i], runId);
                employees.Add(new SeededEmployee(
                    teamMemberId,
                    roles[i],
                    $"SANDBOX-EMP-{(i + 1).ToString("00", CultureInfo.InvariantCulture)}"));
            }

            return employees;
        }

        private static async Task<string> CreateTeamMemberAsync(SquareClient client, string locationId, string role, string runId)
        {
            var response = await client.TeamMembers.CreateAsync(new CreateTeamMemberRequest
            {
                IdempotencyKey = Guid.NewGuid().ToString("N"),
                TeamMember = new TeamMember
                {
                    GivenName = "Sandbox",
                    FamilyName = $"{role} Staff {runId}",
                    AssignedLocations = new TeamMemberAssignedLocations
                    {
                        AssignmentType = TeamMemberAssignedLocationsAssignmentType.ExplicitLocations,
                        LocationIds = new[] { locationId }
                    }
                }
            });

            response.Errors.ShouldBeNull();
            return response.TeamMember.Id;
        }

        private static async Task<List<Timecard>> CreateClosedTimecardsAsync(
            SquareClient client,
            string locationId,
            IReadOnlyList<SeededEmployee> employees,
            DateTimeOffset firstShiftStartUtc,
            int numberOfDays,
            string runId)
        {
            var timecards = new List<Timecard>();
            for (var day = 0; day < numberOfDays; day++)
            {
                for (var employeeIndex = 0; employeeIndex < employees.Count; employeeIndex++)
                {
                    var employee = employees[employeeIndex];
                    var shiftStart = firstShiftStartUtc
                        .AddDays(day)
                        .AddMinutes(employeeIndex * 7);
                    var shiftEnd = shiftStart.AddHours(7).AddMinutes(30 + employeeIndex);
                    var declaredTips = employeeIndex % 3 == 0 ? 150 : 0;

                    var timecard = await CreateClosedTimecardAsync(
                        client,
                        locationId,
                        employee.TeamMemberId,
                        employee.Role,
                        declaredTips,
                        shiftStart,
                        shiftEnd,
                        runId);

                    timecards.Add(timecard);
                }
            }

            return timecards;
        }

        private static async Task<List<Timecard>> CreateOpenTimecardsAsync(
            SquareClient client,
            string locationId,
            IReadOnlyList<SeededEmployee> employees,
            string runId)
        {
            var timecards = new List<Timecard>();
            for (var i = 0; i < employees.Count; i++)
            {
                var employee = employees[i];
                var timecard = await CreateOpenTimecardAsync(
                    client,
                    locationId,
                    employee.TeamMemberId,
                    $"{employee.Role} Open",
                    runId);
                timecards.Add(timecard);
            }

            return timecards;
        }

        private static async Task<Timecard> CreateClosedTimecardAsync(
            SquareClient client,
            string locationId,
            string teamMemberId,
            string title,
            long declaredCashTips,
            DateTimeOffset startAtUtc,
            DateTimeOffset endAtUtc,
            string runId)
        {
            var response = await client.Labor.CreateTimecardAsync(new CreateTimecardRequest
            {
                IdempotencyKey = Guid.NewGuid().ToString("N"),
                Timecard = new Timecard
                {
                    LocationId = locationId,
                    TeamMemberId = teamMemberId,
                    Timezone = "UTC",
                    StartAt = startAtUtc.ToString("O"),
                    EndAt = endAtUtc.ToString("O"),
                    Wage = new TimecardWage
                    {
                        Title = $"Sandbox {title} {runId}",
                        HourlyRate = Money(1500 + declaredCashTips),
                        TipEligible = true
                    },
                    DeclaredCashTipMoney = Money(declaredCashTips)
                }
            });

            response.Errors.ShouldBeNull();
            return response.Timecard;
        }

        private static async Task<SeededOrder> CreateDiscountedHospitalityOrderAsync(
            SquareClient client,
            string locationId,
            string burgerVariationId,
            string friesVariationId,
            string teamMemberId,
            string runId)
        {
            const string discountUid = "sandbox-discount";
            var tipMoney = Money(200);
            var orderResponse = await client.Orders.CreateAsync(new CreateOrderRequest
            {
                IdempotencyKey = Guid.NewGuid().ToString("N"),
                Order = new Order
                {
                    LocationId = locationId,
                    ReferenceId = $"fs-discounted-{runId}",
                    LineItems = new[]
                    {
                        new OrderLineItem
                        {
                            Uid = "burger-line",
                            CatalogObjectId = burgerVariationId,
                            Quantity = "1",
                            Modifiers = new[]
                            {
                                new OrderLineItemModifier
                                {
                                    Name = "Cheese",
                                    Quantity = "1",
                                    BasePriceMoney = Money(100)
                                }
                            },
                            AppliedDiscounts = new[]
                            {
                                new OrderLineItemAppliedDiscount
                                {
                                    DiscountUid = discountUid
                                }
                            }
                        },
                        new OrderLineItem
                        {
                            Uid = "fries-line",
                            CatalogObjectId = friesVariationId,
                            Quantity = "2"
                        }
                    },
                    Discounts = new[]
                    {
                        new OrderLineItemDiscount
                        {
                            Uid = discountUid,
                            Name = "Sandbox Promo",
                            Type = OrderLineItemDiscountType.FixedAmount,
                            AmountMoney = Money(400),
                            Scope = OrderLineItemDiscountScope.LineItem
                        }
                    },
                    ServiceCharges = new[]
                    {
                        new OrderServiceCharge
                        {
                            Name = "Sandbox Hospitality Charge",
                            AmountMoney = Money(150),
                            CalculationPhase = OrderServiceChargeCalculationPhase.TotalPhase,
                            Taxable = false
                        }
                    }
                }
            });

            orderResponse.Errors.ShouldBeNull();
            var order = orderResponse.Order;
            order.TotalMoney.ShouldNotBeNull("Square should return an order total for payment creation.");

            var paymentResponse = await client.Payments.CreateAsync(new CreatePaymentRequest
            {
                IdempotencyKey = Guid.NewGuid().ToString("N"),
                SourceId = "cnon:card-nonce-ok",
                LocationId = locationId,
                OrderId = order.Id,
                TeamMemberId = teamMemberId,
                AmountMoney = order.TotalMoney,
                TipMoney = tipMoney,
                Autocomplete = true
            });

            paymentResponse.Errors.ShouldBeNull();
            paymentResponse.Payment.Status.ShouldBe("COMPLETED");

            return new SeededOrder(order, paymentResponse.Payment, tipMoney);
        }

        private static async Task<SeededOrder> CreateCoffeeOrderAsync(
            SquareClient client,
            string locationId,
            string coffeeVariationId,
            string teamMemberId,
            string runId)
        {
            var orderResponse = await client.Orders.CreateAsync(new CreateOrderRequest
            {
                IdempotencyKey = Guid.NewGuid().ToString("N"),
                Order = new Order
                {
                    LocationId = locationId,
                    ReferenceId = $"fs-coffee-{runId}",
                    LineItems = new[]
                    {
                        new OrderLineItem
                        {
                            CatalogObjectId = coffeeVariationId,
                            Quantity = "1"
                        }
                    },
                    ServiceCharges = new[]
                    {
                        new OrderServiceCharge
                        {
                            Name = "Coffee Service Charge",
                            AmountMoney = Money(50),
                            CalculationPhase = OrderServiceChargeCalculationPhase.TotalPhase,
                            Taxable = false
                        }
                    }
                }
            });

            orderResponse.Errors.ShouldBeNull();
            var order = orderResponse.Order;
            order.TotalMoney.ShouldNotBeNull("Square should return an order total for payment creation.");

            var paymentResponse = await client.Payments.CreateAsync(new CreatePaymentRequest
            {
                IdempotencyKey = Guid.NewGuid().ToString("N"),
                SourceId = "cnon:card-nonce-ok",
                LocationId = locationId,
                OrderId = order.Id,
                TeamMemberId = teamMemberId,
                AmountMoney = order.TotalMoney,
                Autocomplete = true
            });

            paymentResponse.Errors.ShouldBeNull();
            paymentResponse.Payment.Status.ShouldBe("COMPLETED");

            return new SeededOrder(order, paymentResponse.Payment, Money(0));
        }

        private static async Task<SeededOrder> CreateEdgeCaseOrderAsync(
            SquareClient client,
            string locationId,
            string teaVariationId,
            string teamMemberId,
            string runId)
        {
            const string discountUid = "sandbox-percent-discount";
            var orderResponse = await client.Orders.CreateAsync(new CreateOrderRequest
            {
                IdempotencyKey = Guid.NewGuid().ToString("N"),
                Order = new Order
                {
                    LocationId = locationId,
                    ReferenceId = $"fs-edge-{runId}",
                    LineItems = new[]
                    {
                        new OrderLineItem
                        {
                            Uid = "tea-line",
                            CatalogObjectId = teaVariationId,
                            Quantity = "3",
                            Modifiers = new[]
                            {
                                new OrderLineItemModifier
                                {
                                    Name = "Oat Milk",
                                    Quantity = "1",
                                    BasePriceMoney = Money(50)
                                },
                                new OrderLineItemModifier
                                {
                                    Name = "Honey",
                                    Quantity = "1",
                                    BasePriceMoney = Money(25)
                                }
                            },
                            AppliedDiscounts = new[]
                            {
                                new OrderLineItemAppliedDiscount
                                {
                                    DiscountUid = discountUid
                                }
                            }
                        }
                    },
                    Discounts = new[]
                    {
                        new OrderLineItemDiscount
                        {
                            Uid = discountUid,
                            Name = "Sandbox Percentage Promo",
                            Type = OrderLineItemDiscountType.FixedPercentage,
                            Percentage = "10",
                            Scope = OrderLineItemDiscountScope.LineItem
                        }
                    },
                    ServiceCharges = new[]
                    {
                        new OrderServiceCharge
                        {
                            Name = "Edge Service Charge",
                            AmountMoney = Money(75),
                            CalculationPhase = OrderServiceChargeCalculationPhase.TotalPhase,
                            Taxable = false
                        }
                    }
                }
            });

            orderResponse.Errors.ShouldBeNull();
            var order = orderResponse.Order;
            order.TotalMoney.ShouldNotBeNull("Square should return an order total for payment creation.");

            var paymentResponse = await client.Payments.CreateAsync(new CreatePaymentRequest
            {
                IdempotencyKey = Guid.NewGuid().ToString("N"),
                SourceId = "cnon:card-nonce-ok",
                LocationId = locationId,
                OrderId = order.Id,
                TeamMemberId = teamMemberId,
                AmountMoney = order.TotalMoney,
                Autocomplete = true
            });

            paymentResponse.Errors.ShouldBeNull();
            paymentResponse.Payment.Status.ShouldBe("COMPLETED");

            return new SeededOrder(order, paymentResponse.Payment, Money(0));
        }

        private static async Task<SeededOrder> CreateFullMenuModifierOrderAsync(
            SquareClient client,
            string locationId,
            IReadOnlyList<SeededMenuItem> seededMenuItems,
            string teamMemberId,
            string runId)
        {
            const string discountUid = "sandbox-full-menu-discount";
            var lineItems = seededMenuItems.Select((item, index) => new OrderLineItem
            {
                Uid = $"full-menu-line-{index}",
                CatalogObjectId = item.VariationId,
                Quantity = index % 3 == 0 ? "2" : "1",
                Modifiers = new[]
                {
                    new OrderLineItemModifier
                    {
                        Name = item.ModifierName,
                        Quantity = "1",
                        BasePriceMoney = Money(item.ModifierPriceMinor)
                    }
                },
                AppliedDiscounts = index % 2 == 0
                    ? new[]
                    {
                        new OrderLineItemAppliedDiscount
                        {
                            DiscountUid = discountUid
                        }
                    }
                    : null
            }).ToArray();

            var tipMoney = Money(350);
            var orderResponse = await client.Orders.CreateAsync(new CreateOrderRequest
            {
                IdempotencyKey = Guid.NewGuid().ToString("N"),
                Order = new Order
                {
                    LocationId = locationId,
                    ReferenceId = $"fs-full-menu-{runId}",
                    LineItems = lineItems,
                    Discounts = new[]
                    {
                        new OrderLineItemDiscount
                        {
                            Uid = discountUid,
                            Name = "Full Menu Fixed Discount",
                            Type = OrderLineItemDiscountType.FixedAmount,
                            AmountMoney = Money(500),
                            Scope = OrderLineItemDiscountScope.LineItem
                        }
                    },
                    ServiceCharges = new[]
                    {
                        new OrderServiceCharge
                        {
                            Name = "Full Menu Service Charge",
                            AmountMoney = Money(300),
                            CalculationPhase = OrderServiceChargeCalculationPhase.TotalPhase,
                            Taxable = false
                        }
                    }
                }
            });

            orderResponse.Errors.ShouldBeNull();
            var order = orderResponse.Order;
            order.TotalMoney.ShouldNotBeNull("Square should return an order total for payment creation.");

            var paymentResponse = await client.Payments.CreateAsync(new CreatePaymentRequest
            {
                IdempotencyKey = Guid.NewGuid().ToString("N"),
                SourceId = "cnon:card-nonce-ok",
                LocationId = locationId,
                OrderId = order.Id,
                TeamMemberId = teamMemberId,
                AmountMoney = order.TotalMoney,
                TipMoney = tipMoney,
                Autocomplete = true
            });

            paymentResponse.Errors.ShouldBeNull();
            paymentResponse.Payment.Status.ShouldBe("COMPLETED");

            return new SeededOrder(order, paymentResponse.Payment, tipMoney);
        }

        private static async Task<SeededOrder> CreateBreakfastTaxedOrderAsync(
            SquareClient client,
            string locationId,
            string eggsVariationId,
            string pancakesVariationId,
            string teamMemberId,
            string runId)
        {
            const string taxUid = "sandbox-breakfast-tax";
            const string discountUid = "sandbox-breakfast-discount";
            var orderResponse = await client.Orders.CreateAsync(new CreateOrderRequest
            {
                IdempotencyKey = Guid.NewGuid().ToString("N"),
                Order = new Order
                {
                    LocationId = locationId,
                    ReferenceId = $"fs-breakfast-tax-{runId}",
                    LineItems = new[]
                    {
                        new OrderLineItem
                        {
                            Uid = "eggs-line",
                            CatalogObjectId = eggsVariationId,
                            Quantity = "2",
                            Modifiers = new[]
                            {
                                new OrderLineItemModifier
                                {
                                    Name = "Smoked Salmon",
                                    Quantity = "1",
                                    BasePriceMoney = Money(450)
                                }
                            },
                            AppliedTaxes = new[]
                            {
                                new OrderLineItemAppliedTax
                                {
                                    TaxUid = taxUid
                                }
                            }
                        },
                        new OrderLineItem
                        {
                            Uid = "pancakes-line",
                            CatalogObjectId = pancakesVariationId,
                            Quantity = "1",
                            Modifiers = new[]
                            {
                                new OrderLineItemModifier
                                {
                                    Name = "Maple Syrup",
                                    Quantity = "1",
                                    BasePriceMoney = Money(125)
                                }
                            },
                            AppliedDiscounts = new[]
                            {
                                new OrderLineItemAppliedDiscount
                                {
                                    DiscountUid = discountUid
                                }
                            },
                            AppliedTaxes = new[]
                            {
                                new OrderLineItemAppliedTax
                                {
                                    TaxUid = taxUid
                                }
                            }
                        }
                    },
                    Taxes = new[]
                    {
                        new OrderLineItemTax
                        {
                            Uid = taxUid,
                            Name = "Sandbox VAT",
                            Type = OrderLineItemTaxType.Additive,
                            Percentage = "20",
                            Scope = OrderLineItemTaxScope.LineItem
                        }
                    },
                    Discounts = new[]
                    {
                        new OrderLineItemDiscount
                        {
                            Uid = discountUid,
                            Name = "Breakfast Loyalty",
                            Type = OrderLineItemDiscountType.FixedPercentage,
                            Percentage = "15",
                            Scope = OrderLineItemDiscountScope.LineItem
                        }
                    },
                    ServiceCharges = new[]
                    {
                        new OrderServiceCharge
                        {
                            Name = "Breakfast Service",
                            AmountMoney = Money(125),
                            CalculationPhase = OrderServiceChargeCalculationPhase.TotalPhase,
                            Taxable = false
                        }
                    }
                }
            });

            orderResponse.Errors.ShouldBeNull();
            var order = orderResponse.Order;
            order.TotalMoney.ShouldNotBeNull("Square should return an order total for payment creation.");

            var tipMoney = Money(275);
            var paymentResponse = await client.Payments.CreateAsync(new CreatePaymentRequest
            {
                IdempotencyKey = Guid.NewGuid().ToString("N"),
                SourceId = "cnon:card-nonce-ok",
                LocationId = locationId,
                OrderId = order.Id,
                TeamMemberId = teamMemberId,
                AmountMoney = order.TotalMoney,
                TipMoney = tipMoney,
                Autocomplete = true
            });

            paymentResponse.Errors.ShouldBeNull();
            paymentResponse.Payment.Status.ShouldBe("COMPLETED");

            return new SeededOrder(order, paymentResponse.Payment, tipMoney);
        }

        private static async Task<SeededOrder> CreateBarRoundOrderAsync(
            SquareClient client,
            string locationId,
            string cocktailVariationId,
            string beerVariationId,
            string teamMemberId,
            string runId)
        {
            const string discountUid = "sandbox-bar-comp";
            var orderResponse = await client.Orders.CreateAsync(new CreateOrderRequest
            {
                IdempotencyKey = Guid.NewGuid().ToString("N"),
                Order = new Order
                {
                    LocationId = locationId,
                    ReferenceId = $"fs-bar-round-{runId}",
                    LineItems = new[]
                    {
                        new OrderLineItem
                        {
                            Uid = "cocktail-line",
                            CatalogObjectId = cocktailVariationId,
                            Quantity = "3",
                            Modifiers = new[]
                            {
                                new OrderLineItemModifier
                                {
                                    Name = "Premium Gin",
                                    Quantity = "1",
                                    BasePriceMoney = Money(300)
                                }
                            }
                        },
                        new OrderLineItem
                        {
                            Uid = "beer-line",
                            CatalogObjectId = beerVariationId,
                            Quantity = "4",
                            Modifiers = new[]
                            {
                                new OrderLineItemModifier
                                {
                                    Name = "Lime",
                                    Quantity = "1",
                                    BasePriceMoney = Money(50)
                                }
                            },
                            AppliedDiscounts = new[]
                            {
                                new OrderLineItemAppliedDiscount
                                {
                                    DiscountUid = discountUid
                                }
                            }
                        }
                    },
                    Discounts = new[]
                    {
                        new OrderLineItemDiscount
                        {
                            Uid = discountUid,
                            Name = "Bar Comp",
                            Type = OrderLineItemDiscountType.FixedAmount,
                            AmountMoney = Money(300),
                            Scope = OrderLineItemDiscountScope.LineItem
                        }
                    },
                    ServiceCharges = new[]
                    {
                        new OrderServiceCharge
                        {
                            Name = "Late Bar Charge",
                            AmountMoney = Money(225),
                            CalculationPhase = OrderServiceChargeCalculationPhase.TotalPhase,
                            Taxable = false
                        }
                    }
                }
            });

            orderResponse.Errors.ShouldBeNull();
            var order = orderResponse.Order;
            order.TotalMoney.ShouldNotBeNull("Square should return an order total for payment creation.");

            var tipMoney = Money(500);
            var paymentResponse = await client.Payments.CreateAsync(new CreatePaymentRequest
            {
                IdempotencyKey = Guid.NewGuid().ToString("N"),
                SourceId = "cnon:card-nonce-ok",
                LocationId = locationId,
                OrderId = order.Id,
                TeamMemberId = teamMemberId,
                AmountMoney = order.TotalMoney,
                TipMoney = tipMoney,
                Autocomplete = true
            });

            paymentResponse.Errors.ShouldBeNull();
            paymentResponse.Payment.Status.ShouldBe("COMPLETED");

            return new SeededOrder(order, paymentResponse.Payment, tipMoney);
        }

        private static async Task<SeededOrder> CreateRetailOrderAsync(
            SquareClient client,
            string locationId,
            string merchVariationId,
            string teamMemberId,
            string runId)
        {
            var orderResponse = await client.Orders.CreateAsync(new CreateOrderRequest
            {
                IdempotencyKey = Guid.NewGuid().ToString("N"),
                Order = new Order
                {
                    LocationId = locationId,
                    ReferenceId = $"fs-retail-{runId}",
                    LineItems = new[]
                    {
                        new OrderLineItem
                        {
                            Uid = "merch-line",
                            CatalogObjectId = merchVariationId,
                            Quantity = "2",
                            Modifiers = new[]
                            {
                                new OrderLineItemModifier
                                {
                                    Name = "Gift Wrap",
                                    Quantity = "1",
                                    BasePriceMoney = Money(100)
                                }
                            }
                        }
                    }
                }
            });

            orderResponse.Errors.ShouldBeNull();
            var order = orderResponse.Order;
            order.TotalMoney.ShouldNotBeNull("Square should return an order total for payment creation.");

            var paymentResponse = await client.Payments.CreateAsync(new CreatePaymentRequest
            {
                IdempotencyKey = Guid.NewGuid().ToString("N"),
                SourceId = "cnon:card-nonce-ok",
                LocationId = locationId,
                OrderId = order.Id,
                TeamMemberId = teamMemberId,
                AmountMoney = order.TotalMoney,
                Autocomplete = true
            });

            paymentResponse.Errors.ShouldBeNull();
            paymentResponse.Payment.Status.ShouldBe("COMPLETED");

            return new SeededOrder(order, paymentResponse.Payment, Money(0));
        }

        private static async Task<SeededOrder> CreateCateringOrderAsync(
            SquareClient client,
            string locationId,
            IReadOnlyList<SeededMenuItem> seededMenuItems,
            string teamMemberId,
            string runId)
        {
            const string discountUid = "sandbox-catering-discount";
            var lineItems = seededMenuItems
                .Skip(4)
                .Take(12)
                .Select((item, index) => new OrderLineItem
                {
                    Uid = $"catering-line-{index}",
                    CatalogObjectId = item.VariationId,
                    Quantity = index % 4 == 0 ? "3" : "1",
                    Modifiers = index % 2 == 0
                        ? new[]
                        {
                            new OrderLineItemModifier
                            {
                                Name = item.ModifierName,
                                Quantity = "1",
                                BasePriceMoney = Money(item.ModifierPriceMinor)
                            }
                        }
                        : null,
                    AppliedDiscounts = index % 3 == 0
                        ? new[]
                        {
                            new OrderLineItemAppliedDiscount
                            {
                                DiscountUid = discountUid
                            }
                        }
                        : null
                })
                .ToArray();

            var orderResponse = await client.Orders.CreateAsync(new CreateOrderRequest
            {
                IdempotencyKey = Guid.NewGuid().ToString("N"),
                Order = new Order
                {
                    LocationId = locationId,
                    ReferenceId = $"fs-catering-{runId}",
                    LineItems = lineItems,
                    Discounts = new[]
                    {
                        new OrderLineItemDiscount
                        {
                            Uid = discountUid,
                            Name = "Catering Goodwill",
                            Type = OrderLineItemDiscountType.FixedAmount,
                            AmountMoney = Money(750),
                            Scope = OrderLineItemDiscountScope.LineItem
                        }
                    },
                    ServiceCharges = new[]
                    {
                        new OrderServiceCharge
                        {
                            Name = "Delivery Charge",
                            AmountMoney = Money(600),
                            CalculationPhase = OrderServiceChargeCalculationPhase.TotalPhase,
                            Taxable = false
                        },
                        new OrderServiceCharge
                        {
                            Name = "Event Staffing Charge",
                            AmountMoney = Money(950),
                            CalculationPhase = OrderServiceChargeCalculationPhase.TotalPhase,
                            Taxable = false
                        }
                    }
                }
            });

            orderResponse.Errors.ShouldBeNull();
            var order = orderResponse.Order;
            order.TotalMoney.ShouldNotBeNull("Square should return an order total for payment creation.");

            var tipMoney = Money(1000);
            var paymentResponse = await client.Payments.CreateAsync(new CreatePaymentRequest
            {
                IdempotencyKey = Guid.NewGuid().ToString("N"),
                SourceId = "cnon:card-nonce-ok",
                LocationId = locationId,
                OrderId = order.Id,
                TeamMemberId = teamMemberId,
                AmountMoney = order.TotalMoney,
                TipMoney = tipMoney,
                Autocomplete = true
            });

            paymentResponse.Errors.ShouldBeNull();
            paymentResponse.Payment.Status.ShouldBe("COMPLETED");

            return new SeededOrder(order, paymentResponse.Payment, tipMoney);
        }

        private static async Task<SeededOrder> CreateCashOrderAsync(
            SquareClient client,
            string locationId,
            string soupVariationId,
            string teamMemberId,
            string runId)
        {
            var orderResponse = await client.Orders.CreateAsync(new CreateOrderRequest
            {
                IdempotencyKey = Guid.NewGuid().ToString("N"),
                Order = new Order
                {
                    LocationId = locationId,
                    ReferenceId = $"fs-cash-{runId}",
                    LineItems = new[]
                    {
                        new OrderLineItem
                        {
                            CatalogObjectId = soupVariationId,
                            Quantity = "2",
                            Modifiers = new[]
                            {
                                new OrderLineItemModifier
                                {
                                    Name = "Sourdough",
                                    Quantity = "1",
                                    BasePriceMoney = Money(175)
                                }
                            }
                        }
                    },
                    ServiceCharges = new[]
                    {
                        new OrderServiceCharge
                        {
                            Name = "Cash Table Charge",
                            AmountMoney = Money(100),
                            CalculationPhase = OrderServiceChargeCalculationPhase.TotalPhase,
                            Taxable = false
                        }
                    }
                }
            });

            orderResponse.Errors.ShouldBeNull();
            var order = orderResponse.Order;
            order.TotalMoney.ShouldNotBeNull("Square should return an order total for payment creation.");

            var paymentResponse = await client.Payments.CreateAsync(new CreatePaymentRequest
            {
                IdempotencyKey = Guid.NewGuid().ToString("N"),
                SourceId = "CASH",
                LocationId = locationId,
                OrderId = order.Id,
                TeamMemberId = teamMemberId,
                AmountMoney = order.TotalMoney,
                CashDetails = new CashPaymentDetails
                {
                    BuyerSuppliedMoney = Money((order.TotalMoney.Amount ?? 0) + 500)
                },
                Autocomplete = true
            });

            paymentResponse.Errors.ShouldBeNull();
            paymentResponse.Payment.Status.ShouldBe("COMPLETED");
            paymentResponse.Payment.SourceType.ShouldBe("CASH");

            return new SeededOrder(order, paymentResponse.Payment, Money(0));
        }

        private static async Task<SeededOrder> CreateExternalPaymentOrderAsync(
            SquareClient client,
            string locationId,
            string voucherVariationId,
            string teamMemberId,
            string runId)
        {
            var orderResponse = await client.Orders.CreateAsync(new CreateOrderRequest
            {
                IdempotencyKey = Guid.NewGuid().ToString("N"),
                Order = new Order
                {
                    LocationId = locationId,
                    ReferenceId = $"fs-external-{runId}",
                    LineItems = new[]
                    {
                        new OrderLineItem
                        {
                            CatalogObjectId = voucherVariationId,
                            Quantity = "1",
                            Modifiers = new[]
                            {
                                new OrderLineItemModifier
                                {
                                    Name = "Envelope",
                                    Quantity = "1",
                                    BasePriceMoney = Money(50)
                                }
                            }
                        }
                    }
                }
            });

            orderResponse.Errors.ShouldBeNull();
            var order = orderResponse.Order;
            order.TotalMoney.ShouldNotBeNull("Square should return an order total for payment creation.");

            var paymentResponse = await client.Payments.CreateAsync(new CreatePaymentRequest
            {
                IdempotencyKey = Guid.NewGuid().ToString("N"),
                SourceId = "EXTERNAL",
                LocationId = locationId,
                OrderId = order.Id,
                TeamMemberId = teamMemberId,
                AmountMoney = order.TotalMoney,
                ExternalDetails = new ExternalPaymentDetails
                {
                    Type = "BANK_TRANSFER",
                    Source = "Sandbox bank transfer",
                    SourceId = $"sandbox-bank-transfer-{runId}"
                },
                Autocomplete = true
            });

            paymentResponse.Errors.ShouldBeNull();
            paymentResponse.Payment.Status.ShouldBe("COMPLETED");
            paymentResponse.Payment.SourceType.ShouldBe("EXTERNAL");

            return new SeededOrder(order, paymentResponse.Payment, Money(0));
        }

        private static async Task<PaymentRefund> RefundPaymentAsync(
            SquareClient client,
            Payment payment,
            Money amount,
            string teamMemberId,
            string reason)
        {
            var refundResponse = await client.Refunds.RefundPaymentAsync(new RefundPaymentRequest
            {
                IdempotencyKey = Guid.NewGuid().ToString("N"),
                PaymentId = payment.Id,
                PaymentVersionToken = payment.VersionToken,
                AmountMoney = amount,
                Reason = reason,
                TeamMemberId = teamMemberId
            });

            refundResponse.Errors.ShouldBeNull();
            refundResponse.Refund.ShouldNotBeNull();
            refundResponse.Refund.PaymentId.ShouldBe(payment.Id);
            refundResponse.Refund.AmountMoney.Amount.ShouldBe(amount.Amount);

            return refundResponse.Refund;
        }

        private static async Task<Order> CreateUnpaidOpenOrderAsync(
            SquareClient client,
            string locationId,
            string coffeeVariationId,
            string runId)
        {
            var orderResponse = await client.Orders.CreateAsync(new CreateOrderRequest
            {
                IdempotencyKey = Guid.NewGuid().ToString("N"),
                Order = new Order
                {
                    LocationId = locationId,
                    ReferenceId = $"fs-open-{runId}",
                    LineItems = new[]
                    {
                        new OrderLineItem
                        {
                            CatalogObjectId = coffeeVariationId,
                            Quantity = "1"
                        }
                    }
                }
            });

            orderResponse.Errors.ShouldBeNull();
            return orderResponse.Order;
        }

        private static async Task<Timecard> CreateTimecardAsync(
            SquareClient client,
            string locationId,
            string teamMemberId,
            string title,
            long declaredCashTips,
            string runId)
        {
            var now = DateTimeOffset.UtcNow;
            var response = await client.Labor.CreateTimecardAsync(new CreateTimecardRequest
            {
                IdempotencyKey = Guid.NewGuid().ToString("N"),
                Timecard = new Timecard
                {
                    LocationId = locationId,
                    TeamMemberId = teamMemberId,
                    Timezone = "UTC",
                    StartAt = now.AddHours(-4).ToString("O"),
                    EndAt = now.AddHours(-1).ToString("O"),
                    Wage = new TimecardWage
                    {
                        Title = $"Sandbox {title}",
                        HourlyRate = Money(1500),
                        TipEligible = true
                    },
                    DeclaredCashTipMoney = Money(declaredCashTips)
                }
            });

            response.Errors.ShouldBeNull();
            return response.Timecard;
        }

        private static async Task<Timecard> CreateOpenTimecardAsync(
            SquareClient client,
            string locationId,
            string teamMemberId,
            string title,
            string runId)
        {
            var now = DateTimeOffset.UtcNow;
            var response = await client.Labor.CreateTimecardAsync(new CreateTimecardRequest
            {
                IdempotencyKey = Guid.NewGuid().ToString("N"),
                Timecard = new Timecard
                {
                    LocationId = locationId,
                    TeamMemberId = teamMemberId,
                    Timezone = "UTC",
                    StartAt = now.AddMinutes(-45).ToString("O"),
                    Wage = new TimecardWage
                    {
                        Title = $"Sandbox {title}",
                        HourlyRate = Money(2000),
                        TipEligible = false
                    }
                }
            });

            response.Errors.ShouldBeNull();
            return response.Timecard;
        }

        private static Money Money(long amount)
        {
            return new Money
            {
                Amount = amount,
                Currency = Currency.Gbp
            };
        }

        private static decimal MinorToMajor(long amount)
        {
            return Convert.ToDecimal(amount) / 100.00M;
        }

        private static decimal MoneyToMajor(Money money)
        {
            return MinorToMajor(money?.Amount ?? 0);
        }

        private static decimal ExpectedTenderTotal(SeededOrder order)
        {
            return MinorToMajor(order.Order.TotalMoney?.Amount ?? 0) + MinorToMajor(order.TipMoney?.Amount ?? 0);
        }

        private static decimal ExpectedServiceChargeTotal(SeededOrder order)
        {
            var orderServiceCharges = order.Order.ServiceCharges?.Sum(x =>
                x.TotalMoney?.Amount ?? x.AppliedMoney?.Amount ?? x.AmountMoney?.Amount ?? 0) ?? 0;

            return MinorToMajor(orderServiceCharges) + MinorToMajor(order.TipMoney?.Amount ?? 0);
        }

        private static decimal ExpectedDiscountTotal(SeededOrder order)
        {
            var discounts = order.Order.Discounts?.Sum(x => x.AppliedMoney?.Amount ?? 0) ?? 0;
            return MinorToMajor(discounts);
        }

        private static void WriteCsv(IEnumerable<TransactionDatasetRow> rows, string fullPath)
        {
            using var writer = new StreamWriter(fullPath);
            using var csv = new CsvWriter(writer, CultureInfo.CurrentCulture);
            csv.WriteRecords(rows);
            csv.Flush();
        }

        private sealed record SeededEmployee(string TeamMemberId, string Role, string FourthEmployeeNumber);

        private sealed record SeededMenuItem(
            string Name,
            string VariationName,
            string Sku,
            long PriceMinor,
            string CategoryId,
            string ModifierName,
            long ModifierPriceMinor)
        {
            public string VariationId { get; init; }
        }

        private sealed record SeededOrder(Order Order, Payment Payment, Money TipMoney)
        {
            public IReadOnlyList<PaymentRefund> Refunds { get; init; } = Array.Empty<PaymentRefund>();
        }
    }
}
