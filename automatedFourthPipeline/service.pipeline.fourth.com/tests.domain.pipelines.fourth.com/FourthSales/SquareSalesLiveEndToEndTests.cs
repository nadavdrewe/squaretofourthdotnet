using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using com.fourth.pipeline.pos;
using com.fourth.pipeline.pos.Model;
using com.fourth.pipeline.pos.Services.SalesApi;
using data.pipeline.fourth.com.Models;
using data.pipeline.fourth.com.Models.Credentials;
using domain.pipeline.fourth.com.Enums;
using domain.pipeline.fourth.com.Models;
using domain.pipeline.fourth.com.Services.Square;
using domain.pipeline.fourth.com.Services.Square.Oauth;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using shared.pipeline.fourth.com;
using Shouldly;
using Square;

namespace tests.domain.pipelines.fourth.com.FourthSales
{
    [TestFixture]
    [Category("Live")]
    public class SquareSalesLiveEndToEndTests
    {
        private const string BrandNameEnvVar = "FOURTH_PIPELINE_LIVE_BRAND";
        private const string StartUtcEnvVar = "FOURTH_PIPELINE_LIVE_START_UTC";
        private const string EndUtcEnvVar = "FOURTH_PIPELINE_LIVE_END_UTC";
        private const string LocationIdEnvVar = "FOURTH_PIPELINE_LIVE_LOCATION_ID";
        private const string PushToFourthEnvVar = "FOURTH_PIPELINE_LIVE_PUSH_TO_FOURTH";
        private const string SquareAccessTokenEnvVar = "FOURTH_PIPELINE_LIVE_SQUARE_ACCESS_TOKEN";

        [Test]
        [Explicit("Hits live Square and optionally Fourth. Requires active OAuth-backed brand credentials in the database.")]
        public async Task LiveSalesPipeline_ProducesCsvThatMatchesSquareSourceData()
        {
            var brandName = Environment.GetEnvironmentVariable(BrandNameEnvVar);
            if (string.IsNullOrWhiteSpace(brandName))
            {
                Assert.Ignore($"Set {BrandNameEnvVar} to the brand name you want to test.");
            }

            var (startUtc, endUtc) = ResolveUtcWindow();
            var requestedLocationId = Environment.GetEnvironmentVariable(LocationIdEnvVar);
            var shouldPushToFourth = ParseBooleanEnvironmentVariable(PushToFourthEnvVar);

            await using var context = new FourthPipelineContext();

            var brand = await context.Brands
                .Include(x => x.BrandIntegrations)
                .Include(x => x.BrandCredentials)
                .Include("Stores.StoreIntegrations.SquareStoreConfigs")
                .Include("Stores.StoreIntegrations.FourthSalesApiStoreConfigs")
                .FirstOrDefaultAsync(x => x.Active && x.Name.ToLower() == brandName.ToLower());

            if (brand == null)
            {
                Assert.Ignore($"No active brand named '{brandName}' was found in the database.");
            }

            brand.BrandIntegrations.Any(x => x.Active && x.IntegrationType == IntegrationTypes.SquareToFourthPosSales)
                .ShouldBeTrue($"Brand '{brand.Name}' does not have an active Square-to-Fourth sales integration.");

            var squareCredential = brand.BrandCredentials
                .Where(x => x.Active && x.CredentialType == shared.pipeline.fouth.com.Enums.CredentialTypes.SquareApi)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();

            if (squareCredential == null)
            {
                Assert.Ignore($"Brand '{brand.Name}' does not have an active Square OAuth credential.");
            }

            var accessToken = await GetAccessTokenAsync(context, squareCredential);
            var squareClient = new SquareClient(accessToken);
            var locationsResponse = await squareClient.Locations.ListAsync();
            var allLocations = locationsResponse.Locations?.ToList() ?? new List<Location>();

            var activeStoreIntegrations = brand.Stores
                .Where(x => x.Active)
                .SelectMany(
                    store => store.StoreIntegrations
                        .Where(integration => integration.Active && integration.IntegrationType == IntegrationTypes.SquareToFourthPosSales)
                        .Select(integration => new LiveStoreIntegrationContext
                        {
                            Store = store,
                            Integration = integration
                        }))
                .ToList();

            if (!string.IsNullOrWhiteSpace(requestedLocationId))
            {
                activeStoreIntegrations = activeStoreIntegrations
                    .Where(x => x.Integration.SquareStoreConfigs.Any(y => y.Active && y.LocationId == requestedLocationId))
                    .ToList();
            }

            if (activeStoreIntegrations.Count == 0)
            {
                Assert.Ignore("No active Square-to-Fourth store integrations matched the requested live test scope.");
            }

            var tempOutputDirectory = Path.Combine(Path.GetTempPath(), "fourth-live-square-tests");
            Directory.CreateDirectory(tempOutputDirectory);

            var storeResults = new List<string>();

            foreach (var storeContext in activeStoreIntegrations)
            {
                var storeIntegration = storeContext.Integration;
                var squareConfig = storeIntegration.SquareStoreConfigs.FirstOrDefault(x => x.Active);
                squareConfig.ShouldNotBeNull($"Store integration {storeIntegration.Id} is missing an active Square store config.");

                var fourthConfig = storeIntegration.FourthSalesApiStoreConfigs.FirstOrDefault(x => x.Active);
                if (shouldPushToFourth)
                {
                    fourthConfig.ShouldNotBeNull($"Store integration {storeIntegration.Id} is missing an active Fourth sales config.");
                }

                var location = allLocations.FirstOrDefault(x => x.Id == squareConfig.LocationId);
                location.ShouldNotBeNull($"Square location '{squareConfig.LocationId}' could not be found for store integration {storeIntegration.Id}.");

                var generator = new SquareToFourthCSVGenerator(accessToken);
                await generator.GatherDataForBrand();

                var gatherResult = await generator.GatherDataForLocation(startUtc, endUtc, location);
                gatherResult.DataGatherResult.ShouldBe(
                    DataGatherResult.Complete,
                    $"Failed to gather Square data for location '{location.Name}' between {startUtc:O} and {endUtc:O}. {gatherResult.Exception?.Message}");

                var dataset = generator._squareLocationDatasets.Single(x => x.Location.Id == location.Id);
                var completedOrders = dataset.orders.Where(x => x.State == OrderState.Completed).ToList();
                var rows = generator.CreateSalesRows(fourthConfig?.UnitId ?? "LIVE_TEST_UNIT").ToList();

                rows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.TAB_OPEN).ShouldBe(completedOrders.Count);
                rows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.TAB_CLOSE).ShouldBe(completedOrders.Count);
                rows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.TENDER).ShouldBe(CountExpectedTenderRows(completedOrders));
                rows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.SALES_ITEM).ShouldBe(CountExpectedSalesItemRows(completedOrders));
                rows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.MODIFIER_ITEM).ShouldBe(CountExpectedModifierRows(completedOrders));
                rows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.DISC_ITEM).ShouldBe(CountExpectedDiscountRows(completedOrders));

                rows.All(x => x.SiteLocationCode == location.Id).ShouldBeTrue();

                if (completedOrders.Count > 0)
                {
                    rows.Count.ShouldBeGreaterThan(0);
                    rows.All(x => !string.IsNullOrWhiteSpace(x.TradingDate)).ShouldBeTrue();
                }

                var fileName = BuildSafeFileName($"{startUtc:yyyy_MM_dd}_{brand.Name}_{storeContext.Store.Name}_SquareLiveE2E.csv");
                var fullPath = Path.Combine(tempOutputDirectory, fileName);
                WriteCsv(rows, fullPath);

                if (shouldPushToFourth)
                {
                    var fourthCredential = await context.CredentialsPool
                        .Where(x => x.Active && x.BrandId == brand.Id)
                        .OrderByDescending(x => x.Id)
                        .FirstOrDefaultAsync(x => x.CredentialType == shared.pipeline.fouth.com.Enums.CredentialTypes.FourthBaseCredential);

                    fourthCredential.ShouldNotBeNull($"Brand '{brand.Name}' does not have an active Fourth credential, so the live push cannot run.");

                    var fourthApiService = new FourthApiService(fourthCredential.Username, fourthCredential.Password, fourthCredential.BaseEndpoint);
                    var loginResponse = await fourthApiService.Login();
                    loginResponse.IsSuccessStatusCode.ShouldBeTrue($"Fourth login failed for brand '{brand.Name}'.");

                    var pushResponse = await fourthApiService.SendSalesDataToFourth(fileName, fullPath);
                    pushResponse.IsSuccessStatusCode.ShouldBeTrue($"Fourth upload failed for location '{location.Name}'.");
                }

                storeResults.Add($"{location.Name}: orders={completedOrders.Count}, rows={rows.Count}, csv={fullPath}");
            }

            TestContext.WriteLine($"Brand: {brand.Name}");
            TestContext.WriteLine($"UTC window: {startUtc:O} -> {endUtc:O}");
            foreach (var result in storeResults)
            {
                TestContext.WriteLine(result);
            }
        }

        [Test]
        [Explicit("Hits live Square only. Requires FOURTH_PIPELINE_LIVE_SQUARE_ACCESS_TOKEN, FOURTH_PIPELINE_LIVE_LOCATION_ID, and UTC date window env vars.")]
        public async Task LiveSquareToken_ProducesCsvForLocation()
        {
            var accessToken = Environment.GetEnvironmentVariable(SquareAccessTokenEnvVar);
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                Assert.Ignore($"Set {SquareAccessTokenEnvVar} to a Square production access token.");
            }

            var requestedLocationId = Environment.GetEnvironmentVariable(LocationIdEnvVar);
            if (string.IsNullOrWhiteSpace(requestedLocationId))
            {
                Assert.Ignore($"Set {LocationIdEnvVar} to the Square production location id to test.");
            }

            var (startUtc, endUtc) = ResolveUtcWindow();
            var squareClient = new SquareClient(accessToken);
            var locationsResponse = await squareClient.Locations.ListAsync();
            var location = locationsResponse.Locations?.FirstOrDefault(x => x.Id == requestedLocationId);
            location.ShouldNotBeNull($"Square location '{requestedLocationId}' could not be found.");

            var generator = new SquareToFourthCSVGenerator(accessToken);
            await generator.GatherDataForBrand();

            var gatherResult = await generator.GatherDataForLocation(startUtc, endUtc, location);
            gatherResult.DataGatherResult.ShouldNotBe(
                DataGatherResult.Error,
                $"Failed to gather Square data for location '{location.Name}' between {startUtc:O} and {endUtc:O}. {gatherResult.Exception?.Message}");

            if (gatherResult.DataGatherResult == DataGatherResult.OrderEmpty)
            {
                Assert.Ignore($"Square returned no orders for location '{location.Name}' between {startUtc:O} and {endUtc:O}.");
            }

            var dataset = generator._squareLocationDatasets.Single(x => x.Location.Id == location.Id);
            var completedOrders = dataset.orders.Where(x => x.State == OrderState.Completed).ToList();
            var rows = generator.CreateSalesRows("LIVE_TOKEN_TEST_UNIT").ToList();

            var tempOutputDirectory = Path.Combine(Path.GetTempPath(), "fourth-live-square-tests");
            Directory.CreateDirectory(tempOutputDirectory);
            var fileName = BuildSafeFileName($"{startUtc:yyyy_MM_dd}_{location.Name}_SquareTokenSmoke.csv");
            var fullPath = Path.Combine(tempOutputDirectory, fileName);
            WriteCsv(rows, fullPath);

            TestContext.WriteLine($"Location: {location.Name} ({location.Id})");
            TestContext.WriteLine($"UTC window: {startUtc:O} -> {endUtc:O}");
            TestContext.WriteLine($"Completed orders: {completedOrders.Count}");
            TestContext.WriteLine($"Rows: {rows.Count}");
            TestContext.WriteLine($"CSV: {fullPath}");
        }

        private static async Task<string> GetAccessTokenAsync(FourthPipelineContext context, BaseCredential credential)
        {
            if (credential == null)
            {
                throw new InvalidOperationException("Square credential was not found.");
            }

            if (!string.IsNullOrWhiteSpace(credential.RefreshToken))
            {
                if (string.IsNullOrWhiteSpace(credential.ClientId) || string.IsNullOrWhiteSpace(credential.ClientSecret))
                {
                    throw new InvalidOperationException("Square credential is missing the client id or client secret needed to refresh the access token.");
                }

                var tokenService = new SquareOAuthTokenService();
                var refreshResponse = await tokenService.RefreshSquareToken(
                    credential.ClientId,
                    credential.ClientSecret,
                    credential.RefreshToken);

                credential.LatestAccessToken = refreshResponse.AccessToken;
                if (!string.IsNullOrWhiteSpace(refreshResponse.RefreshToken))
                {
                    credential.RefreshToken = refreshResponse.RefreshToken;
                }

                credential.WhenUpdatedUTC = DateTime.UtcNow;
                context.Update(credential);
                await context.SaveChangesAsync();
            }

            if (string.IsNullOrWhiteSpace(credential.LatestAccessToken))
            {
                throw new InvalidOperationException("Square credential does not have a usable access token.");
            }

            return credential.LatestAccessToken;
        }

        private static (DateTime startUtc, DateTime endUtc) ResolveUtcWindow()
        {
            var startUtcRaw = Environment.GetEnvironmentVariable(StartUtcEnvVar);
            var endUtcRaw = Environment.GetEnvironmentVariable(EndUtcEnvVar);

            if (string.IsNullOrWhiteSpace(startUtcRaw) || string.IsNullOrWhiteSpace(endUtcRaw))
            {
                Assert.Ignore($"Set both {StartUtcEnvVar} and {EndUtcEnvVar} as ISO-8601 UTC timestamps, for example 2026-04-18T00:00:00Z.");
            }

            var startUtc = DateTime.Parse(
                startUtcRaw,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);

            var endUtc = DateTime.Parse(
                endUtcRaw,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);

            if (endUtc <= startUtc)
            {
                throw new InvalidOperationException($"{EndUtcEnvVar} must be later than {StartUtcEnvVar}.");
            }

            return (startUtc, endUtc);
        }

        private static bool ParseBooleanEnvironmentVariable(string variableName)
        {
            return bool.TryParse(Environment.GetEnvironmentVariable(variableName), out var value) && value;
        }

        private static int CountExpectedTenderRows(IEnumerable<Order> completedOrders)
        {
            return completedOrders.Sum(x => x.Tenders?.Count() ?? 0);
        }

        private static int CountExpectedSalesItemRows(IEnumerable<Order> completedOrders)
        {
            return completedOrders.Sum(x => x.LineItems?.Count() ?? 0);
        }

        private static int CountExpectedModifierRows(IEnumerable<Order> completedOrders)
        {
            return completedOrders.Sum(order => (order.LineItems ?? new List<OrderLineItem>())
                .Sum(item => item.Modifiers?.Count() ?? 0));
        }

        private static int CountExpectedDiscountRows(IEnumerable<Order> completedOrders)
        {
            return completedOrders.Sum(order =>
            {
                var orderDiscountUids = new HashSet<string>((order.Discounts ?? new List<OrderLineItemDiscount>())
                    .Where(x => !string.IsNullOrWhiteSpace(x.Uid))
                    .Select(x => x.Uid));

                return (order.LineItems ?? new List<OrderLineItem>())
                    .Sum(item => (item.AppliedDiscounts ?? new List<OrderLineItemAppliedDiscount>())
                        .Count(discount => !string.IsNullOrWhiteSpace(discount.DiscountUid) && orderDiscountUids.Contains(discount.DiscountUid)));
            });
        }

        private static void WriteCsv(IEnumerable<TransactionDatasetRow> rows, string fullPath)
        {
            using var writer = new StreamWriter(fullPath);
            using var csv = new CsvHelper.CsvWriter(writer, CultureInfo.CurrentCulture);
            csv.WriteRecords(rows);
            csv.Flush();
        }

        private static string BuildSafeFileName(string fileName)
        {
            var invalidCharacters = new string(Path.GetInvalidFileNameChars());
            var pattern = $"[{Regex.Escape(invalidCharacters)}]+";
            return Regex.Replace(fileName, pattern, "_");
        }

        private sealed class LiveStoreIntegrationContext
        {
            public Store Store { get; set; }
            public StoreIntegration Integration { get; set; }
        }
    }
}
