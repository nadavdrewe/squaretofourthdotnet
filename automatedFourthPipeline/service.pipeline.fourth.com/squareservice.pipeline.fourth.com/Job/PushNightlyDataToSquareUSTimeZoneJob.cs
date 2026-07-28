using com.fourth.pipeline.pos.Model;
using com.fourth.pipeline.pos.Services.SalesApi;
using CsvHelper;
using data.pipeline.fourth.com.Models;
using data.pipeline.fourth.com.Models.Configs.Store;
using data.pipeline.fourth.com.Models.Credentials;
using domain.pipeline.fourth.com.Enums;
using domain.pipeline.fourth.com.Exceptions;
using domain.pipeline.fourth.com.Helper;
using domain.pipeline.fourth.com.Models;
using domain.pipeline.fourth.com.Services.Square;
using domain.pipeline.fourth.com.Services.Square.Oauth;
using domain.pipeline.fourth.com.Services.Square.SquareToFourthTimesheetsApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using Serilog;
using shared.pipeline.fourth.com;
using squareservice.pipeline.fourth.com.Services;
using Square;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace squareservice.pipeline.fourth.com.Job
{
    [DisallowConcurrentExecution]
    public class PushNightlyDataToSquareUSTimeZoneJob : IJob
    {
        private readonly ILogger<PushNightlyDataToSquareUSTimeZoneJob> _logger;
        private readonly FourthPipelineContext _fourthPipelineContext;
        private readonly IConfiguration _configuration;
        private readonly IPipelineAlertService _alertService;
        private readonly string _configuredSquareBaseUrl;
        private readonly string _csvOutputDirectory;
        private readonly bool _uploadSalesCsv;
        private readonly string _timesheetXmlOutputDirectory;
        private readonly string _timesheetXmlEndpoint;
        private readonly bool _uploadTimesheetXml;

        public PushNightlyDataToSquareUSTimeZoneJob(
            ILogger<PushNightlyDataToSquareUSTimeZoneJob> logger,
            FourthPipelineContext fourthPipelineContext,
            IConfiguration configuration,
            IPipelineAlertService alertService)
        {
            _logger = logger;
            _fourthPipelineContext = fourthPipelineContext;
            _configuration = configuration;
            _alertService = alertService;
            _configuredSquareBaseUrl = configuration.GetValue<string>("SquareApi:BaseUrl")
                ?? configuration.GetValue<string>("SquareSandbox:BaseUrl");
            _csvOutputDirectory = configuration.GetValue<string>(
                "SquareToFourthSales:CsvOutputDirectory",
                Path.Combine(AppContext.BaseDirectory, "SquareToFourthSales"));
            _uploadSalesCsv = configuration.GetValue<bool>("SquareToFourthSales:UploadToFourth", false);
            _timesheetXmlOutputDirectory = configuration.GetValue<string>(
                "SquareToFourthTimesheets:XmlOutputDirectory",
                Path.Combine(AppContext.BaseDirectory, "SquareToFourthTimesheets"));
            _timesheetXmlEndpoint = configuration.GetValue<string>("SquareToFourthTimesheets:XmlEndpoint", "");
            _uploadTimesheetXml = configuration.GetValue<bool>("SquareToFourthTimesheets:UploadToFourth", false);
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobCorrelationId = Guid.NewGuid().ToString("N");
            Log.Information("Task fired: Square to Fourth hospitality sales");
            await SavePipelineEventAsync(
                correlationId: jobCorrelationId,
                dataType: "SquareToFourth",
                stage: "Job",
                eventType: "Started",
                status: "Started",
                message: "Square to Fourth hospitality sales job started.");

            try
            {
                Directory.CreateDirectory(_csvOutputDirectory);
                Directory.CreateDirectory(_timesheetXmlOutputDirectory);

                var brands = await _fourthPipelineContext.Brands
                    .Where(x => x.Active)
                    .Include(x => x.BrandIntegrations)
                    .Include(x => x.Stores)
                    .Where(x => x.BrandIntegrations.Any(y =>
                        y.Active && y.IntegrationType == IntegrationTypes.SquareToFourthPosSales))
                    .ToListAsync(context.CancellationToken);

                await SavePipelineEventAsync(
                    correlationId: jobCorrelationId,
                    dataType: "SquareToFourth",
                    stage: "Job",
                    eventType: "BrandsLoaded",
                    status: "Succeeded",
                    itemCount: brands.Count,
                    message: "Loaded active brands with Square-to-Fourth integrations.");

                foreach (var brand in brands)
                {
                    await ProcessBrandAsync(brand, jobCorrelationId);
                }

                await SavePipelineEventAsync(
                    correlationId: jobCorrelationId,
                    dataType: "SquareToFourth",
                    stage: "Job",
                    eventType: "Completed",
                    status: "Succeeded",
                    message: "Square to Fourth hospitality sales job completed.");
            }
            catch (Exception ex)
            {
                await SavePipelineEventAsync(
                    correlationId: jobCorrelationId,
                    dataType: "SquareToFourth",
                    stage: "Job",
                    eventType: "Failed",
                    status: "Failed",
                    message: "Square to Fourth hospitality sales job failed.",
                    exception: ex);
                _logger.LogError(ex, "Square to Fourth hospitality sales job failed.");
                Log.Error(ex, "Square to Fourth hospitality sales job failed.");
                await _alertService.NotifyFailureAsync(new PipelineFailureAlert
                {
                    Scope = "Job",
                    DataType = "SquareToFourth",
                    Status = "Failed",
                    Exception = ex
                });
                throw;
            }
        }

        private async Task ProcessBrandAsync(Brand brand, string jobCorrelationId)
        {
            try
            {
                await SavePipelineEventAsync(
                    brand: brand,
                    correlationId: jobCorrelationId,
                    dataType: "SquareToFourth",
                    stage: "Brand",
                    eventType: "Started",
                    status: "Started",
                    message: "Started processing brand.");

                var squareCredential = await GetSquareCredentialAsync(brand);
                var squareCredentialMode = string.IsNullOrWhiteSpace(squareCredential.RefreshToken)
                    ? "StoredAccessToken"
                    : "OAuthRefreshToken";
                var squareBaseUrl = GetSquareBaseUrl(squareCredential);
                var accessToken = await RefreshSquareCredentialAsync(squareCredential, squareBaseUrl);
                await SavePipelineEventAsync(
                    brand: brand,
                    correlationId: jobCorrelationId,
                    dataType: "SquareToFourth",
                    stage: "SquareAuth",
                    eventType: "CredentialResolved",
                    status: "Succeeded",
                    sourceSystem: "Square",
                    message: "Square credential resolved.",
                    detailsJson: ToJson(new
                    {
                        credentialId = squareCredential.Id,
                        mode = squareCredentialMode,
                        baseUrlConfigured = !string.IsNullOrWhiteSpace(squareBaseUrl)
                    }));

                var storeIntegrations = await GetActiveStoreIntegrationsAsync(brand);
                await SavePipelineEventAsync(
                    brand: brand,
                    correlationId: jobCorrelationId,
                    dataType: "SquareToFourth",
                    stage: "Brand",
                    eventType: "StoreIntegrationsLoaded",
                    status: "Succeeded",
                    itemCount: storeIntegrations.Count,
                    message: "Loaded active store integrations.");

                if (storeIntegrations.Count == 0)
                {
                    await SavePipelineEventAsync(
                        brand: brand,
                        correlationId: jobCorrelationId,
                        dataType: "SquareToFourth",
                        stage: "Brand",
                        eventType: "Skipped",
                        status: "SkippedNoStores",
                        message: "Brand has no active Square-to-Fourth store integrations.");
                    Log.Information("Brand {BrandName} has no active Square-to-Fourth store integrations.", brand.Name);
                    return;
                }

                FourthApiService fourthApiService = null;
                if (_uploadSalesCsv || _uploadTimesheetXml)
                {
                    var fourthCredential = await GetFourthCredentialAsync(brand);

                    fourthApiService = CreateFourthApiService(fourthCredential);

                    await SavePipelineEventAsync(
                        brand: brand,
                        correlationId: jobCorrelationId,
                        dataType: "SquareToFourth",
                        stage: "FourthAuth",
                        eventType: "LoginStarted",
                        status: "Started",
                        targetSystem: "Fourth",
                        message: "Fourth OAuth login started.");

                    var loginResponse = await fourthApiService.Login();
                    if (!loginResponse.IsSuccessStatusCode)
                    {
                        await SavePipelineEventAsync(
                            brand: brand,
                            correlationId: jobCorrelationId,
                            dataType: "SquareToFourth",
                            stage: "FourthAuth",
                            eventType: "LoginFailed",
                            status: "Failed",
                            targetSystem: "Fourth",
                            httpStatusCode: (int)loginResponse.StatusCode,
                            message: "Fourth OAuth login failed.");
                        throw new InvalidOperationException(
                            $"Fourth login failed for brand '{brand.Name}' with status {loginResponse.StatusCode}.");
                    }

                    await SavePipelineEventAsync(
                        brand: brand,
                        correlationId: jobCorrelationId,
                        dataType: "SquareToFourth",
                        stage: "FourthAuth",
                        eventType: "LoginSucceeded",
                        status: "Succeeded",
                        targetSystem: "Fourth",
                        httpStatusCode: (int)loginResponse.StatusCode,
                        message: "Fourth OAuth login succeeded.");

                    fourthCredential.LatestAccessToken = fourthApiService.AccessToken;
                    if (!string.IsNullOrWhiteSpace(fourthApiService.RefreshToken))
                    {
                        fourthCredential.RefreshToken = fourthApiService.RefreshToken;
                    }

                    fourthCredential.WhenUpdatedUTC = DateTime.UtcNow;
                    _fourthPipelineContext.Update(fourthCredential);
                    await _fourthPipelineContext.SaveChangesAsync();
                }

                var squareClient = string.IsNullOrWhiteSpace(squareBaseUrl)
                    ? new SquareClient(accessToken)
                    : new SquareClient(accessToken, new ClientOptions { BaseUrl = squareBaseUrl });
                var locationsResponse = await squareClient.Locations.ListAsync();
                var squareLocations = locationsResponse.Locations?.ToList() ?? new List<Location>();
                await SavePipelineEventAsync(
                    brand: brand,
                    correlationId: jobCorrelationId,
                    dataType: "SquareToFourth",
                    stage: "SquareRead",
                    eventType: "LocationsRead",
                    status: "Succeeded",
                    sourceSystem: "Square",
                    itemCount: squareLocations.Count,
                    message: "Read Square locations.");

                var brandDataGenerator = new SquareToFourthCSVGenerator(accessToken, squareBaseUrl);
                await brandDataGenerator.GatherDataForBrand();
                var brandDataset = brandDataGenerator._squareBrandSalesDataset;
                await SavePipelineEventAsync(
                    brand: brand,
                    correlationId: jobCorrelationId,
                    dataType: "SquareToFourth",
                    stage: "SquareRead",
                    eventType: "CatalogAndTeamRead",
                    status: "Succeeded",
                    sourceSystem: "Square",
                    itemCount: brandDataset?.entireCatalog?.Count(),
                    message: "Read Square catalog and team data.",
                    detailsJson: ToJson(new
                    {
                        catalogObjects = brandDataset?.entireCatalog?.Count() ?? 0,
                        items = brandDataset?.allItems?.Count() ?? 0,
                        variations = brandDataset?.allProductVariations?.Count() ?? 0,
                        categories = brandDataset?.allCategories?.Count() ?? 0,
                        modifierLists = brandDataset?.allModifiers?.Count() ?? 0,
                        teamMembers = brandDataset?.allEmployees?.Count() ?? 0
                    }));

                foreach (var storeIntegration in storeIntegrations)
                {
                    await ProcessStoreIntegrationAsync(
                        brand,
                        storeIntegration,
                        squareLocations,
                        accessToken,
                        squareBaseUrl,
                        brandDataGenerator,
                        fourthApiService,
                        jobCorrelationId);
                }

                await SavePipelineEventAsync(
                    brand: brand,
                    correlationId: jobCorrelationId,
                    dataType: "SquareToFourth",
                    stage: "Brand",
                    eventType: "Completed",
                    status: "Succeeded",
                    message: "Completed processing brand.");
            }
            catch (Exception ex)
            {
                await SavePipelineEventAsync(
                    brand: brand,
                    correlationId: jobCorrelationId,
                    dataType: "SquareToFourth",
                    stage: "Brand",
                    eventType: "Failed",
                    status: "Failed",
                    message: "Brand failed during Square-to-Fourth processing.",
                    exception: ex);
                _logger.LogError(ex, "Brand {BrandName} failed in Square to Fourth hospitality sales.", brand.Name);
                Log.Error(ex, "Brand {BrandName} failed in Square to Fourth hospitality sales.", brand.Name);
                await _alertService.NotifyFailureAsync(new PipelineFailureAlert
                {
                    Scope = "Brand",
                    BrandName = brand?.Name,
                    DataType = "SquareToFourth",
                    Status = "Failed",
                    Exception = ex
                });
            }
        }

        private async Task ProcessStoreIntegrationAsync(
            Brand brand,
            StoreIntegration storeIntegration,
            IList<Location> squareLocations,
            string accessToken,
            string squareBaseUrl,
            SquareToFourthCSVGenerator brandDataGenerator,
            FourthApiService fourthApiService,
            string jobCorrelationId)
        {
            var storeName = storeIntegration.Store?.Name ?? $"StoreIntegration-{storeIntegration.Id}";
            data.pipeline.fourth.com.Models.Configs.Store.FourthSalesApiStoreConfig fourthConfig = null;
            string squareLocationId = null;
            DateTime startDateUtc = default;
            DateTime endDateUtc = default;
            DateTime transactionDate = default;
            var salesRunRecordAlreadySaved = false;
            var salesAlertAlreadySent = false;

            try
            {
                await SavePipelineEventAsync(
                    brand: brand,
                    storeIntegration: storeIntegration,
                    correlationId: jobCorrelationId,
                    dataType: "SquareToFourth",
                    stage: "Store",
                    eventType: "Started",
                    status: "Started",
                    message: "Started processing store integration.");

                var squareConfig = storeIntegration.SquareStoreConfigs.FirstOrDefault(x => x.Active);
                if (squareConfig == null)
                {
                    throw new NoCreditsException($"Store '{storeName}' has no active Square location config.");
                }

                fourthConfig = storeIntegration.FourthSalesApiStoreConfigs.FirstOrDefault(x => x.Active);
                if (fourthConfig == null || string.IsNullOrWhiteSpace(fourthConfig.UnitId))
                {
                    throw new NoCreditsException($"Store '{storeName}' has no active Fourth hospitality sales config.");
                }

                squareLocationId = squareConfig.LocationId;
                var squareLocation = squareLocations.FirstOrDefault(x => x.Id == squareConfig.LocationId);
                if (squareLocation == null)
                {
                    throw new InvalidOperationException(
                        $"Square location '{squareConfig.LocationId}' was not found for store '{storeName}'.");
                }

                var startEndRange = DateTimeHelper.GenerateStandardDayYesterdayToTodayStartTimeGivenUTCNow(
                    storeIntegration.Store.UTCOffsetInHours);
                startDateUtc = startEndRange.StartDate;
                endDateUtc = startEndRange.EndDate;
                transactionDate = startEndRange.StartDate;
                var salesStartDateUtc = GetConfiguredUtcDateTime("SquareToFourthSales:OverrideStartUtc", startDateUtc);
                var salesEndDateUtc = GetConfiguredUtcDateTime("SquareToFourthSales:OverrideEndUtc", endDateUtc);
                var timesheetStartDateUtc = GetConfiguredUtcDateTime("SquareToFourthTimesheets:OverrideStartUtc", startDateUtc);
                var timesheetEndDateUtc = GetConfiguredUtcDateTime("SquareToFourthTimesheets:OverrideEndUtc", endDateUtc);
                transactionDate = salesStartDateUtc;
                await SavePipelineEventAsync(
                    brand: brand,
                    storeIntegration: storeIntegration,
                    squareLocationId: squareLocationId,
                    fourthConfig: fourthConfig,
                    correlationId: jobCorrelationId,
                    dataType: "SquareToFourth",
                    stage: "Store",
                    eventType: "WindowResolved",
                    status: "Succeeded",
                    periodStartUtc: salesStartDateUtc,
                    periodEndUtc: salesEndDateUtc,
                    transactionDate: transactionDate,
                    message: "Resolved sales and timesheet processing windows.",
                    detailsJson: ToJson(new
                    {
                        salesStartUtc = salesStartDateUtc,
                        salesEndUtc = salesEndDateUtc,
                        timesheetStartUtc = timesheetStartDateUtc,
                        timesheetEndUtc = timesheetEndDateUtc
                    }));

                await ProcessTimesheetsAsync(
                    brand,
                    storeIntegration,
                    storeName,
                    squareLocation,
                    accessToken,
                    squareBaseUrl,
                    fourthConfig,
                    timesheetStartDateUtc,
                    timesheetEndDateUtc,
                    transactionDate,
                    fourthApiService,
                    jobCorrelationId);

                var storeDataGenerator = new SquareToFourthCSVGenerator(accessToken, squareBaseUrl)
                {
                    _squareBrandSalesDataset = brandDataGenerator._squareBrandSalesDataset
                };

                var dataGatherResult = await storeDataGenerator.GatherDataForLocation(
                    salesStartDateUtc,
                    salesEndDateUtc,
                    squareLocation);
                var locationDataset = storeDataGenerator._squareLocationDatasets
                    .LastOrDefault(x => x.Location?.Id == squareLocation.Id);
                var orderCount = locationDataset?.orders?.Count() ?? 0;
                var completedOrderCount = locationDataset?.orders?.Count(x => x.State == OrderState.Completed) ?? 0;
                var paymentCount = locationDataset?.paymentsForOrders?.Count() ?? 0;
                var refundCount = locationDataset?.refundsForOrders?.Count() ?? 0;
                await SavePipelineEventAsync(
                    brand: brand,
                    storeIntegration: storeIntegration,
                    squareLocationId: squareLocationId,
                    fourthConfig: fourthConfig,
                    correlationId: jobCorrelationId,
                    dataType: "SalesCsv",
                    stage: "SquareRead",
                    eventType: "OrdersPaymentsRefundsRead",
                    status: dataGatherResult.DataGatherResult.ToString(),
                    sourceSystem: "Square",
                    periodStartUtc: salesStartDateUtc,
                    periodEndUtc: salesEndDateUtc,
                    transactionDate: transactionDate,
                    itemCount: orderCount,
                    message: "Read Square orders, payments, and refunds for sales.",
                    detailsJson: ToJson(new
                    {
                        orders = orderCount,
                        completedOrders = completedOrderCount,
                        payments = paymentCount,
                        refunds = refundCount
                    }),
                    exception: dataGatherResult.Exception);

                if (dataGatherResult.DataGatherResult == DataGatherResult.OrderEmpty)
                {
                    await SavePipelineRunRecordAsync(
                        brand,
                        storeIntegration,
                        squareLocationId,
                        fourthConfig,
                        "SalesCsv",
                        "SkippedNoData",
                        salesStartDateUtc,
                        salesEndDateUtc,
                        transactionDate,
                        0,
                        payloadFormat: "csv");

                    Log.Information(
                        "Store {StoreName} for brand {BrandName} returned no Square orders for {TransactionDate:yyyy-MM-dd}.",
                        storeName,
                        brand.Name,
                        transactionDate);
                    await SavePipelineEventAsync(
                        brand: brand,
                        storeIntegration: storeIntegration,
                        squareLocationId: squareLocationId,
                        fourthConfig: fourthConfig,
                        correlationId: jobCorrelationId,
                        dataType: "SalesCsv",
                        stage: "Sales",
                        eventType: "Skipped",
                        status: "SkippedNoData",
                        periodStartUtc: salesStartDateUtc,
                        periodEndUtc: salesEndDateUtc,
                        transactionDate: transactionDate,
                        message: "No Square orders returned for sales window.");
                    return;
                }

                if (dataGatherResult.DataGatherResult == DataGatherResult.Error)
                {
                    throw new InvalidOperationException(
                        $"Square data gather failed for store '{storeName}'.",
                        dataGatherResult.Exception);
                }

                var dataToSend = storeDataGenerator.CreateSalesRows(fourthConfig.UnitId).ToList();
                await SavePipelineEventAsync(
                    brand: brand,
                    storeIntegration: storeIntegration,
                    squareLocationId: squareLocationId,
                    fourthConfig: fourthConfig,
                    correlationId: jobCorrelationId,
                    dataType: "SalesCsv",
                    stage: "Transform",
                    eventType: "SalesRowsCreated",
                    status: "Succeeded",
                    periodStartUtc: salesStartDateUtc,
                    periodEndUtc: salesEndDateUtc,
                    transactionDate: transactionDate,
                    itemCount: completedOrderCount,
                    rowCount: dataToSend.Count,
                    message: "Created Fourth sales CSV rows from Square data.",
                    detailsJson: ToJson(new
                    {
                        rowTypes = dataToSend
                            .GroupBy(x => x.TransactionTypeCode ?? "")
                            .OrderBy(x => x.Key)
                            .ToDictionary(x => string.IsNullOrWhiteSpace(x.Key) ? "UNKNOWN" : x.Key, x => x.Count()),
                        positiveTenderRows = dataToSend.Count(x => x.TransactionTypeCode == "TENDER" && x.TenderAmount > 0),
                        negativeTenderRows = dataToSend.Count(x => x.TransactionTypeCode == "TENDER" && x.TenderAmount < 0),
                        uniqueProductPlus = dataToSend
                            .Where(x => !string.IsNullOrWhiteSpace(x.SalesItemPLU))
                            .Select(x => x.SalesItemPLU)
                            .Distinct()
                            .Count()
                    }));
                if (dataToSend.Count == 0)
                {
                    await SavePipelineRunRecordAsync(
                        brand,
                        storeIntegration,
                        squareLocationId,
                        fourthConfig,
                        "SalesCsv",
                        "SkippedNoRows",
                        salesStartDateUtc,
                        salesEndDateUtc,
                        transactionDate,
                        0,
                        payloadFormat: "csv");

                    Log.Information(
                        "Store {StoreName} for brand {BrandName} produced no Fourth rows for {TransactionDate:yyyy-MM-dd}.",
                        storeName,
                        brand.Name,
                        transactionDate);
                    await SavePipelineEventAsync(
                        brand: brand,
                        storeIntegration: storeIntegration,
                        squareLocationId: squareLocationId,
                        fourthConfig: fourthConfig,
                        correlationId: jobCorrelationId,
                        dataType: "SalesCsv",
                        stage: "Sales",
                        eventType: "Skipped",
                        status: "SkippedNoRows",
                        periodStartUtc: salesStartDateUtc,
                        periodEndUtc: salesEndDateUtc,
                        transactionDate: transactionDate,
                        message: "Square data produced no Fourth sales rows.");
                    return;
                }

                var csvName = BuildSafeFileName(
                    $"{transactionDate:yyyy_MM_dd}_{brand.Name}_{storeName}_SquareFourthHospitalitySales_{DateTime.UtcNow:yyyy_MM_dd_HHmmss}.csv");
                var csvFullPath = Path.Combine(_csvOutputDirectory, csvName);
                WriteCsv(dataToSend, csvFullPath);
                var csvPayload = await File.ReadAllTextAsync(csvFullPath);
                await SavePipelineEventAsync(
                    brand: brand,
                    storeIntegration: storeIntegration,
                    squareLocationId: squareLocationId,
                    fourthConfig: fourthConfig,
                    correlationId: jobCorrelationId,
                    dataType: "SalesCsv",
                    stage: "File",
                    eventType: "CsvWritten",
                    status: "Succeeded",
                    periodStartUtc: salesStartDateUtc,
                    periodEndUtc: salesEndDateUtc,
                    transactionDate: transactionDate,
                    rowCount: dataToSend.Count,
                    outputFileName: csvName,
                    outputFullPath: csvFullPath,
                    message: "Wrote Fourth sales CSV file.",
                    detailsJson: ToJson(new { payloadLength = csvPayload.Length }));

                if (!_uploadSalesCsv)
                {
                    await SavePipelineRunRecordAsync(
                        brand,
                        storeIntegration,
                        squareLocationId,
                        fourthConfig,
                        "SalesCsv",
                        "Generated",
                        salesStartDateUtc,
                        salesEndDateUtc,
                        transactionDate,
                        dataToSend.Count,
                        csvName,
                        csvFullPath,
                        "csv",
                        csvPayload);

                    Log.Information(
                        "Store {StoreName} in brand {BrandName} generated {RowCount} Fourth hospitality sales rows for {TransactionDate:yyyy-MM-dd}. CSV: {CsvFullPath}",
                        storeName,
                        brand.Name,
                        dataToSend.Count,
                        transactionDate,
                        csvFullPath);
                    await SavePipelineEventAsync(
                        brand: brand,
                        storeIntegration: storeIntegration,
                        squareLocationId: squareLocationId,
                        fourthConfig: fourthConfig,
                        correlationId: jobCorrelationId,
                        dataType: "SalesCsv",
                        stage: "Sales",
                        eventType: "Completed",
                        status: "Generated",
                        periodStartUtc: salesStartDateUtc,
                        periodEndUtc: salesEndDateUtc,
                        transactionDate: transactionDate,
                        rowCount: dataToSend.Count,
                        outputFileName: csvName,
                        outputFullPath: csvFullPath,
                        message: "Generated Fourth sales CSV without upload.");
                    return;
                }

                if (fourthApiService == null)
                {
                    throw new InvalidOperationException("SquareToFourthSales:UploadToFourth is true, but the Fourth API service is not available.");
                }

                var salesResult = await fourthApiService.SendSalesDataToFourth(csvName, csvFullPath);
                var responseBody = await salesResult.Content.ReadAsStringAsync();
                await SavePipelineEventAsync(
                    brand: brand,
                    storeIntegration: storeIntegration,
                    squareLocationId: squareLocationId,
                    fourthConfig: fourthConfig,
                    correlationId: jobCorrelationId,
                    dataType: "SalesCsv",
                    stage: "FourthUpload",
                    eventType: "SalesCsvUploadResponse",
                    status: salesResult.IsSuccessStatusCode ? "Succeeded" : "Failed",
                    targetSystem: "Fourth",
                    periodStartUtc: salesStartDateUtc,
                    periodEndUtc: salesEndDateUtc,
                    transactionDate: transactionDate,
                    rowCount: dataToSend.Count,
                    httpStatusCode: (int)salesResult.StatusCode,
                    outputFileName: csvName,
                    outputFullPath: csvFullPath,
                    message: "Fourth sales CSV upload returned a response.",
                    detailsJson: ToJson(new { responseLength = responseBody?.Length ?? 0 }));
                if (salesResult.IsSuccessStatusCode)
                {
                    await SavePipelineRunRecordAsync(
                        brand,
                        storeIntegration,
                        squareLocationId,
                        fourthConfig,
                        "SalesCsv",
                        "Uploaded",
                        salesStartDateUtc,
                        salesEndDateUtc,
                        transactionDate,
                        dataToSend.Count,
                        csvName,
                        csvFullPath,
                        "csv",
                        csvPayload,
                        (int)salesResult.StatusCode,
                        responseBody);

                    Log.Information(
                        "Store {StoreName} in brand {BrandName} pushed {RowCount} Fourth hospitality sales rows for {TransactionDate:yyyy-MM-dd}. CSV: {CsvFullPath}",
                        storeName,
                        brand.Name,
                        dataToSend.Count,
                        transactionDate,
                        csvFullPath);
                    return;
                }

                await SavePipelineRunRecordAsync(
                    brand,
                    storeIntegration,
                    squareLocationId,
                    fourthConfig,
                    "SalesCsv",
                    "Failed",
                    salesStartDateUtc,
                    salesEndDateUtc,
                    transactionDate,
                    dataToSend.Count,
                    csvName,
                    csvFullPath,
                    "csv",
                    csvPayload,
                    (int)salesResult.StatusCode,
                    responseBody,
                    $"Fourth upload failed with status {salesResult.StatusCode}.");
                salesRunRecordAlreadySaved = true;
                await _alertService.NotifyFailureAsync(new PipelineFailureAlert
                {
                    Scope = "Store",
                    BrandName = brand?.Name,
                    StoreName = storeName,
                    DataType = "SalesCsv",
                    Status = "Failed",
                    PeriodStartUtc = startDateUtc,
                    PeriodEndUtc = endDateUtc,
                    TransactionDate = transactionDate,
                    OutputFullPath = csvFullPath,
                    FourthStatusCode = (int)salesResult.StatusCode,
                    FourthResponseBody = responseBody,
                    Exception = new InvalidOperationException($"Fourth upload failed with status {salesResult.StatusCode}.")
                });
                salesAlertAlreadySent = true;

                throw new InvalidOperationException(
                    $"Fourth upload failed for store '{storeName}' with status {salesResult.StatusCode}: {responseBody}");
            }
            catch (Exception ex)
            {
                await SavePipelineEventAsync(
                    brand: brand,
                    storeIntegration: storeIntegration,
                    squareLocationId: squareLocationId,
                    fourthConfig: fourthConfig,
                    correlationId: jobCorrelationId,
                    dataType: "SalesCsv",
                    stage: "Sales",
                    eventType: "Failed",
                    status: "Failed",
                    periodStartUtc: startDateUtc == default ? null : startDateUtc,
                    periodEndUtc: endDateUtc == default ? null : endDateUtc,
                    transactionDate: transactionDate == default ? null : transactionDate,
                    message: "Store sales CSV processing failed.",
                    exception: ex);
                if (!salesRunRecordAlreadySaved)
                {
                    await SavePipelineRunRecordAsync(
                        brand,
                        storeIntegration,
                        squareLocationId,
                        fourthConfig,
                        "SalesCsv",
                        "Failed",
                        startDateUtc,
                        endDateUtc,
                        transactionDate == default ? DateTime.UtcNow.Date : transactionDate,
                        0,
                        payloadFormat: "csv",
                        errorMessage: ex.ToString());
                }

                _logger.LogError(ex, "Store {StoreName} failed in Square to Fourth hospitality sales.", storeName);
                Log.Error(ex, "Store {StoreName} failed in Square to Fourth hospitality sales.", storeName);
                if (!salesAlertAlreadySent)
                {
                    await _alertService.NotifyFailureAsync(new PipelineFailureAlert
                    {
                        Scope = "Store",
                        BrandName = brand?.Name,
                        StoreName = storeName,
                        DataType = "SalesCsv",
                        Status = "Failed",
                        PeriodStartUtc = startDateUtc == default ? null : startDateUtc,
                        PeriodEndUtc = endDateUtc == default ? null : endDateUtc,
                        TransactionDate = transactionDate == default ? DateTime.UtcNow.Date : transactionDate,
                        Exception = ex
                    });
                }
            }
        }

        private async Task ProcessTimesheetsAsync(
            Brand brand,
            StoreIntegration storeIntegration,
            string storeName,
            Location squareLocation,
            string accessToken,
            string squareBaseUrl,
            data.pipeline.fourth.com.Models.Configs.Store.FourthSalesApiStoreConfig fourthConfig,
            DateTime startDateUtc,
            DateTime endDateUtc,
            DateTime transactionDate,
            FourthApiService fourthApiService,
            string jobCorrelationId)
        {
            var timesheetRunRecordAlreadySaved = false;
            var timesheetAlertAlreadySent = false;

            try
            {
                await SavePipelineEventAsync(
                    brand: brand,
                    storeIntegration: storeIntegration,
                    squareLocationId: squareLocation?.Id,
                    fourthConfig: fourthConfig,
                    correlationId: jobCorrelationId,
                    dataType: "TimesheetXml",
                    stage: "Timesheets",
                    eventType: "Started",
                    status: "Started",
                    periodStartUtc: startDateUtc,
                    periodEndUtc: endDateUtc,
                    transactionDate: transactionDate,
                    message: "Started timesheet XML processing.");

                var timesheetGenerator = new SquareToFourthTimesheetXmlGenerator(accessToken, squareBaseUrl);
                await timesheetGenerator.GatherDataForLocation(startDateUtc, endDateUtc, squareLocation);
                await SavePipelineEventAsync(
                    brand: brand,
                    storeIntegration: storeIntegration,
                    squareLocationId: squareLocation?.Id,
                    fourthConfig: fourthConfig,
                    correlationId: jobCorrelationId,
                    dataType: "TimesheetXml",
                    stage: "SquareRead",
                    eventType: "TimecardsRead",
                    status: "Succeeded",
                    sourceSystem: "Square",
                    periodStartUtc: startDateUtc,
                    periodEndUtc: endDateUtc,
                    transactionDate: transactionDate,
                    itemCount: timesheetGenerator.Timecards.Count,
                    message: "Read Square labor timecards.",
                    detailsJson: ToJson(new
                    {
                        timecards = timesheetGenerator.Timecards.Count,
                        closed = timesheetGenerator.Timecards.Count(x => !string.IsNullOrWhiteSpace(x.EndAt)),
                        open = timesheetGenerator.Timecards.Count(x => string.IsNullOrWhiteSpace(x.EndAt)),
                        teamMembers = timesheetGenerator.Timecards
                            .Where(x => !string.IsNullOrWhiteSpace(x.TeamMemberId))
                            .Select(x => x.TeamMemberId)
                            .Distinct()
                            .Count()
                    }));

                if (timesheetGenerator.Timecards.Count == 0)
                {
                    await SavePipelineRunRecordAsync(
                        brand,
                        storeIntegration,
                        squareLocation.Id,
                        fourthConfig,
                        "TimesheetXml",
                        "SkippedNoData",
                        startDateUtc,
                        endDateUtc,
                        transactionDate,
                        0,
                        payloadFormat: "xml");

                    Log.Information(
                        "Store {StoreName} for brand {BrandName} returned no Square timecards for {TransactionDate:yyyy-MM-dd}.",
                        storeName,
                        brand.Name,
                        transactionDate);
                    await SavePipelineEventAsync(
                        brand: brand,
                        storeIntegration: storeIntegration,
                        squareLocationId: squareLocation?.Id,
                        fourthConfig: fourthConfig,
                        correlationId: jobCorrelationId,
                        dataType: "TimesheetXml",
                        stage: "Timesheets",
                        eventType: "Skipped",
                        status: "SkippedNoData",
                        periodStartUtc: startDateUtc,
                        periodEndUtc: endDateUtc,
                        transactionDate: transactionDate,
                        message: "No Square timecards returned for timesheet window.");
                    return;
                }

                var locationCode = string.IsNullOrWhiteSpace(fourthConfig.SiteLocationCode)
                    ? fourthConfig.UnitId
                    : fourthConfig.SiteLocationCode;
                var employeeNumberMap = BuildEmployeeNumberMap(storeIntegration, squareLocation.Id);
                var timesheetEntries = timesheetGenerator.CreateTimesheetEntries(locationCode, employeeNumberMap).ToList();
                await SavePipelineEventAsync(
                    brand: brand,
                    storeIntegration: storeIntegration,
                    squareLocationId: squareLocation?.Id,
                    fourthConfig: fourthConfig,
                    correlationId: jobCorrelationId,
                    dataType: "TimesheetXml",
                    stage: "Transform",
                    eventType: "TimesheetRowsCreated",
                    status: "Succeeded",
                    periodStartUtc: startDateUtc,
                    periodEndUtc: endDateUtc,
                    transactionDate: transactionDate,
                    itemCount: timesheetGenerator.Timecards.Count,
                    rowCount: timesheetEntries.Count,
                    message: "Created Fourth timesheet XML rows from Square timecards.",
                    detailsJson: ToJson(new
                    {
                        mappedEmployees = timesheetEntries
                            .Where(x => !string.IsNullOrWhiteSpace(x.EmpNo))
                            .Select(x => x.EmpNo)
                            .Distinct()
                            .Count(),
                        configuredEmployeeMappings = employeeNumberMap.Count,
                        closedRows = timesheetEntries.Count(x => x.CheckOut.HasValue),
                        openRows = timesheetEntries.Count(x => !x.CheckOut.HasValue)
                    }));
                if (timesheetEntries.Count == 0)
                {
                    await SavePipelineRunRecordAsync(
                        brand,
                        storeIntegration,
                        squareLocation.Id,
                        fourthConfig,
                        "TimesheetXml",
                        "SkippedNoRows",
                        startDateUtc,
                        endDateUtc,
                        transactionDate,
                        0,
                        payloadFormat: "xml");

                    Log.Information(
                        "Store {StoreName} for brand {BrandName} produced no Fourth timesheet rows for {TransactionDate:yyyy-MM-dd}.",
                        storeName,
                        brand.Name,
                        transactionDate);
                    await SavePipelineEventAsync(
                        brand: brand,
                        storeIntegration: storeIntegration,
                        squareLocationId: squareLocation?.Id,
                        fourthConfig: fourthConfig,
                        correlationId: jobCorrelationId,
                        dataType: "TimesheetXml",
                        stage: "Timesheets",
                        eventType: "Skipped",
                        status: "SkippedNoRows",
                        periodStartUtc: startDateUtc,
                        periodEndUtc: endDateUtc,
                        transactionDate: transactionDate,
                        message: "Square timecards produced no Fourth timesheet XML rows.");
                    return;
                }

                var xmlName = BuildSafeFileName(
                    $"{transactionDate:yyyy_MM_dd}_{brand.Name}_{storeName}_SquareFourthTimesheets_{DateTime.UtcNow:yyyy_MM_dd_HHmmss}.xml");
                var xmlFullPath = Path.Combine(_timesheetXmlOutputDirectory, xmlName);
                var timesheetXml = timesheetGenerator.CreateTimesheetXml(
                    locationCode,
                    DateTime.UtcNow,
                    $"square-timesheets-{brand.Id}-{squareLocation.Id}-{transactionDate:yyyyMMdd}",
                    employeeNumberMap);
                timesheetXml.Save(xmlFullPath);
                await SavePipelineEventAsync(
                    brand: brand,
                    storeIntegration: storeIntegration,
                    squareLocationId: squareLocation?.Id,
                    fourthConfig: fourthConfig,
                    correlationId: jobCorrelationId,
                    dataType: "TimesheetXml",
                    stage: "File",
                    eventType: "XmlWritten",
                    status: "Succeeded",
                    periodStartUtc: startDateUtc,
                    periodEndUtc: endDateUtc,
                    transactionDate: transactionDate,
                    rowCount: timesheetEntries.Count,
                    outputFileName: xmlName,
                    outputFullPath: xmlFullPath,
                    message: "Wrote Fourth timesheet XML file.",
                    detailsJson: ToJson(new { payloadLength = timesheetXml.OuterXml.Length }));

                Log.Information(
                    "Store {StoreName} in brand {BrandName} generated {RowCount} Fourth timesheet rows for {TransactionDate:yyyy-MM-dd}. XML: {XmlFullPath}",
                    storeName,
                    brand.Name,
                    timesheetEntries.Count,
                    transactionDate,
                    xmlFullPath);

                if (!_uploadTimesheetXml)
                {
                    await SavePipelineRunRecordAsync(
                        brand,
                        storeIntegration,
                        squareLocation.Id,
                        fourthConfig,
                        "TimesheetXml",
                        "Generated",
                        startDateUtc,
                        endDateUtc,
                        transactionDate,
                        timesheetEntries.Count,
                        xmlName,
                        xmlFullPath,
                        "xml",
                        timesheetXml.OuterXml);

                    await SavePipelineEventAsync(
                        brand: brand,
                        storeIntegration: storeIntegration,
                        squareLocationId: squareLocation?.Id,
                        fourthConfig: fourthConfig,
                        correlationId: jobCorrelationId,
                        dataType: "TimesheetXml",
                        stage: "Timesheets",
                        eventType: "Completed",
                        status: "Generated",
                        periodStartUtc: startDateUtc,
                        periodEndUtc: endDateUtc,
                        transactionDate: transactionDate,
                        rowCount: timesheetEntries.Count,
                        outputFileName: xmlName,
                        outputFullPath: xmlFullPath,
                        message: "Generated Fourth timesheet XML without upload.");

                    return;
                }

                if (string.IsNullOrWhiteSpace(_timesheetXmlEndpoint))
                {
                    throw new InvalidOperationException("SquareToFourthTimesheets:UploadToFourth is true, but SquareToFourthTimesheets:XmlEndpoint is empty.");
                }

                var uploadResponse = await fourthApiService.SendXmlDataToFourth(timesheetXml.OuterXml, _timesheetXmlEndpoint);
                var responseBody = await uploadResponse.Content.ReadAsStringAsync();
                await SavePipelineEventAsync(
                    brand: brand,
                    storeIntegration: storeIntegration,
                    squareLocationId: squareLocation?.Id,
                    fourthConfig: fourthConfig,
                    correlationId: jobCorrelationId,
                    dataType: "TimesheetXml",
                    stage: "FourthUpload",
                    eventType: "TimesheetXmlUploadResponse",
                    status: uploadResponse.IsSuccessStatusCode ? "Succeeded" : "Failed",
                    targetSystem: "Fourth",
                    periodStartUtc: startDateUtc,
                    periodEndUtc: endDateUtc,
                    transactionDate: transactionDate,
                    rowCount: timesheetEntries.Count,
                    httpStatusCode: (int)uploadResponse.StatusCode,
                    outputFileName: xmlName,
                    outputFullPath: xmlFullPath,
                    message: "Fourth timesheet XML upload returned a response.",
                    detailsJson: ToJson(new { responseLength = responseBody?.Length ?? 0 }));
                if (uploadResponse.IsSuccessStatusCode)
                {
                    await SavePipelineRunRecordAsync(
                        brand,
                        storeIntegration,
                        squareLocation.Id,
                        fourthConfig,
                        "TimesheetXml",
                        "Uploaded",
                        startDateUtc,
                        endDateUtc,
                        transactionDate,
                        timesheetEntries.Count,
                        xmlName,
                        xmlFullPath,
                        "xml",
                        timesheetXml.OuterXml,
                        (int)uploadResponse.StatusCode,
                        responseBody);

                    Log.Information(
                        "Store {StoreName} in brand {BrandName} pushed {RowCount} Fourth timesheet rows for {TransactionDate:yyyy-MM-dd}.",
                        storeName,
                        brand.Name,
                        timesheetEntries.Count,
                        transactionDate);
                    return;
                }

                await SavePipelineRunRecordAsync(
                    brand,
                    storeIntegration,
                    squareLocation.Id,
                    fourthConfig,
                    "TimesheetXml",
                    "Failed",
                    startDateUtc,
                    endDateUtc,
                    transactionDate,
                    timesheetEntries.Count,
                    xmlName,
                    xmlFullPath,
                    "xml",
                    timesheetXml.OuterXml,
                    (int)uploadResponse.StatusCode,
                    responseBody,
                    $"Fourth timesheet XML upload failed with status {uploadResponse.StatusCode}.");
                timesheetRunRecordAlreadySaved = true;
                await _alertService.NotifyFailureAsync(new PipelineFailureAlert
                {
                    Scope = "Store",
                    BrandName = brand?.Name,
                    StoreName = storeName,
                    DataType = "TimesheetXml",
                    Status = "Failed",
                    PeriodStartUtc = startDateUtc,
                    PeriodEndUtc = endDateUtc,
                    TransactionDate = transactionDate,
                    OutputFullPath = xmlFullPath,
                    FourthStatusCode = (int)uploadResponse.StatusCode,
                    FourthResponseBody = responseBody,
                    Exception = new InvalidOperationException($"Fourth timesheet XML upload failed with status {uploadResponse.StatusCode}.")
                });
                timesheetAlertAlreadySent = true;

                throw new InvalidOperationException(
                    $"Fourth timesheet XML upload failed for store '{storeName}' with status {uploadResponse.StatusCode}: {responseBody}");
            }
            catch (Exception ex)
            {
                await SavePipelineEventAsync(
                    brand: brand,
                    storeIntegration: storeIntegration,
                    squareLocationId: squareLocation?.Id,
                    fourthConfig: fourthConfig,
                    correlationId: jobCorrelationId,
                    dataType: "TimesheetXml",
                    stage: "Timesheets",
                    eventType: "Failed",
                    status: "Failed",
                    periodStartUtc: startDateUtc,
                    periodEndUtc: endDateUtc,
                    transactionDate: transactionDate,
                    message: "Store timesheet XML processing failed.",
                    exception: ex);
                if (!timesheetRunRecordAlreadySaved)
                {
                    await SavePipelineRunRecordAsync(
                        brand,
                        storeIntegration,
                        squareLocation?.Id,
                        fourthConfig,
                        "TimesheetXml",
                        "Failed",
                        startDateUtc,
                        endDateUtc,
                        transactionDate,
                        0,
                        payloadFormat: "xml",
                        errorMessage: ex.ToString());
                }

                _logger.LogError(ex, "Store {StoreName} failed in Square to Fourth timesheet XML generation.", storeName);
                Log.Error(ex, "Store {StoreName} failed in Square to Fourth timesheet XML generation.", storeName);
                if (!timesheetAlertAlreadySent)
                {
                    await _alertService.NotifyFailureAsync(new PipelineFailureAlert
                    {
                        Scope = "Store",
                        BrandName = brand?.Name,
                        StoreName = storeName,
                        DataType = "TimesheetXml",
                        Status = "Failed",
                        PeriodStartUtc = startDateUtc,
                        PeriodEndUtc = endDateUtc,
                        TransactionDate = transactionDate,
                        Exception = ex
                    });
                }
            }
        }

        private async Task<BaseCredential> GetSquareCredentialAsync(Brand brand)
        {
            var squareCredential = await _fourthPipelineContext.CredentialsPool
                .Where(x => x.Active && x.BrandId == brand.Id)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync(x => x.CredentialType == shared.pipeline.fouth.com.Enums.CredentialTypes.SquareApi);

            if (squareCredential == null ||
                (string.IsNullOrWhiteSpace(squareCredential.RefreshToken) &&
                 string.IsNullOrWhiteSpace(squareCredential.LatestAccessToken)))
            {
                throw new NoCreditsException(
                    $"No active Square credential exists for brand '{brand.Name}', or both the refresh token and access token are empty.");
            }

            return squareCredential;
        }

        private string GetSquareBaseUrl(BaseCredential squareCredential)
        {
            if (!string.IsNullOrWhiteSpace(squareCredential?.BaseEndpoint))
            {
                return squareCredential.BaseEndpoint;
            }

            return _configuredSquareBaseUrl;
        }

        private async Task<string> RefreshSquareCredentialAsync(BaseCredential squareCredential, string squareBaseUrl)
        {
            if (!SquareOAuthTokenService.IsRefreshDue(squareCredential, DateTime.UtcNow))
            {
                return squareCredential.LatestAccessToken;
            }

            var squareOAuthTokenService = new SquareOAuthTokenService();
            var metadata = SquareOAuthTokenMetadata.FromStoredValue(squareCredential.SupplimentalData2);
            SquareOAuthApplication application = null;
            if (metadata?.SquareOAuthApplicationId is int applicationId)
            {
                application = await _fourthPipelineContext.SquareOAuthApplications
                    .FirstOrDefaultAsync(x => x.Id == applicationId);
            }
            else if (!string.IsNullOrWhiteSpace(squareCredential.ClientId))
            {
                var environment = metadata?.GetEnvironment(squareCredential.BaseEndpoint)
                    ?? SquareOAuthEnvironment.GetEnvironmentFromBaseUrl(squareCredential.BaseEndpoint);
                application = await _fourthPipelineContext.SquareOAuthApplications.FirstOrDefaultAsync(x =>
                    x.ApplicationId == squareCredential.ClientId && x.Environment == environment);
            }

            var clientId = application?.ApplicationId ?? squareCredential.ClientId;
            var clientSecret = application?.ClientSecret ?? squareCredential.ClientSecret;
            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
            {
                throw new NoCreditsException(
                    "The Square OAuth application used for this credential is missing or incomplete.");
            }

            var refreshResponse = await squareOAuthTokenService.RefreshSquareToken(
                clientId,
                clientSecret,
                squareCredential.RefreshToken,
                baseUrl: squareBaseUrl);

            SquareOAuthTokenService.ApplyTokenResponse(squareCredential, refreshResponse);
            _fourthPipelineContext.Update(squareCredential);
            await _fourthPipelineContext.SaveChangesAsync();

            return squareCredential.LatestAccessToken;
        }

        private async Task<BaseCredential> GetFourthCredentialAsync(Brand brand)
        {
            var fourthCredential = await _fourthPipelineContext.CredentialsPool
                .Where(x => x.Active && x.BrandId == brand.Id)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync(x => x.CredentialType == shared.pipeline.fouth.com.Enums.CredentialTypes.FourthBaseCredential);

            if (fourthCredential == null)
            {
                throw new NoCreditsException($"No active Fourth hospitality credential exists for brand '{brand.Name}'.");
            }

            return fourthCredential;
        }

        private static FourthApiService CreateFourthApiService(BaseCredential fourthCredential)
        {
            if (!string.IsNullOrWhiteSpace(fourthCredential.ClientId) ||
                !string.IsNullOrWhiteSpace(fourthCredential.ClientSecret))
            {
                return new FourthApiService(
                    fourthCredential.Username,
                    fourthCredential.Password,
                    fourthCredential.BaseEndpoint,
                    fourthCredential.ClientId,
                    fourthCredential.ClientSecret,
                    fourthCredential.SupplimentalData2,
                    fourthCredential.SupplimentalData1);
            }

            return new FourthApiService(
                fourthCredential.Username,
                fourthCredential.Password,
                fourthCredential.BaseEndpoint);
        }

        private async Task<List<StoreIntegration>> GetActiveStoreIntegrationsAsync(Brand brand)
        {
            var storeIdsForBrand = brand.Stores
                .Where(x => x.Active)
                .Select(x => x.Id)
                .ToList();

            return await _fourthPipelineContext.StoreIntegrations
                .Include(x => x.FourthSalesApiStoreConfigs)
                .Include(x => x.SquareEmployeeMappings)
                .Include(x => x.Store)
                .Include(x => x.SquareStoreConfigs)
                .Where(x => x.IntegrationType == IntegrationTypes.SquareToFourthPosSales)
                .Where(x => x.Active)
                .Where(x => storeIdsForBrand.Contains(x.StoreId))
                .ToListAsync();
        }

        private async Task SavePipelineRunRecordAsync(
            Brand brand,
            StoreIntegration storeIntegration,
            string squareLocationId,
            data.pipeline.fourth.com.Models.Configs.Store.FourthSalesApiStoreConfig fourthConfig,
            string dataType,
            string status,
            DateTime periodStartUtc,
            DateTime periodEndUtc,
            DateTime transactionDate,
            int rowCount,
            string outputFileName = null,
            string outputFullPath = null,
            string payloadFormat = null,
            string payload = null,
            int? fourthStatusCode = null,
            string fourthResponseBody = null,
            string errorMessage = null)
        {
            try
            {
                var now = DateTime.UtcNow;
                var record = new PipelineRunRecord
                {
                    BrandId = brand?.Id,
                    BrandName = Truncate(brand?.Name, 256),
                    StoreId = storeIntegration?.StoreId,
                    StoreName = Truncate(storeIntegration?.Store?.Name, 256),
                    StoreIntegrationId = storeIntegration?.Id,
                    SquareLocationId = Truncate(squareLocationId, 128),
                    FourthUnitId = Truncate(fourthConfig?.UnitId, 128),
                    FourthLocationCode = Truncate(
                        string.IsNullOrWhiteSpace(fourthConfig?.SiteLocationCode) ? fourthConfig?.UnitId : fourthConfig?.SiteLocationCode,
                        128),
                    SourceSystem = "Square",
                    TargetSystem = "Fourth",
                    DataType = Truncate(dataType, 64),
                    Status = Truncate(status, 64),
                    PeriodStartUtc = periodStartUtc,
                    PeriodEndUtc = periodEndUtc,
                    TransactionDate = transactionDate,
                    OutputFileName = Truncate(outputFileName, 512),
                    OutputFullPath = Truncate(outputFullPath, 1024),
                    RowCount = rowCount,
                    PayloadFormat = Truncate(payloadFormat, 32),
                    Payload = payload,
                    FourthStatusCode = fourthStatusCode,
                    FourthResponseBody = fourthResponseBody,
                    ErrorMessage = errorMessage,
                    WhenCreatedUTC = now,
                    WhenUpdatedUTC = now
                };

                _fourthPipelineContext.PipelineRunRecords.Add(record);
                await _fourthPipelineContext.SaveChangesAsync();

                await SavePipelineEventAsync(
                    brand: brand,
                    storeIntegration: storeIntegration,
                    squareLocationId: squareLocationId,
                    fourthConfig: fourthConfig,
                    pipelineRunRecordId: record.Id,
                    dataType: dataType,
                    stage: "RunRecord",
                    eventType: "Persisted",
                    status: status,
                    periodStartUtc: periodStartUtc,
                    periodEndUtc: periodEndUtc,
                    transactionDate: transactionDate,
                    rowCount: rowCount,
                    httpStatusCode: fourthStatusCode,
                    outputFileName: outputFileName,
                    outputFullPath: outputFullPath,
                    message: "Persisted pipeline run record.",
                    detailsJson: ToJson(new
                    {
                        payloadFormat,
                        payloadLength = payload?.Length ?? 0,
                        fourthResponseLength = fourthResponseBody?.Length ?? 0,
                        hasError = !string.IsNullOrWhiteSpace(errorMessage)
                    }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write Square to Fourth pipeline run record.");
                Log.Error(ex, "Failed to write Square to Fourth pipeline run record.");
            }
        }

        private async Task SavePipelineEventAsync(
            Brand brand = null,
            StoreIntegration storeIntegration = null,
            string squareLocationId = null,
            FourthSalesApiStoreConfig fourthConfig = null,
            string correlationId = null,
            int? pipelineRunRecordId = null,
            string dataType = null,
            string stage = null,
            string eventType = null,
            string status = null,
            string sourceSystem = "Square",
            string targetSystem = "Fourth",
            DateTime? periodStartUtc = null,
            DateTime? periodEndUtc = null,
            DateTime? transactionDate = null,
            int? itemCount = null,
            int? rowCount = null,
            long? durationMs = null,
            int? httpStatusCode = null,
            string outputFileName = null,
            string outputFullPath = null,
            string externalReference = null,
            string message = null,
            string detailsJson = null,
            Exception exception = null)
        {
            try
            {
                var now = DateTime.UtcNow;
                var record = new PipelineEventLog
                {
                    PipelineRunRecordId = pipelineRunRecordId,
                    CorrelationId = Truncate(correlationId, 64),
                    BrandId = brand?.Id,
                    BrandName = Truncate(brand?.Name, 256),
                    StoreId = storeIntegration?.StoreId,
                    StoreName = Truncate(storeIntegration?.Store?.Name, 256),
                    StoreIntegrationId = storeIntegration?.Id,
                    SquareLocationId = Truncate(squareLocationId, 128),
                    FourthUnitId = Truncate(fourthConfig?.UnitId, 128),
                    FourthLocationCode = Truncate(
                        string.IsNullOrWhiteSpace(fourthConfig?.SiteLocationCode) ? fourthConfig?.UnitId : fourthConfig?.SiteLocationCode,
                        128),
                    SourceSystem = Truncate(sourceSystem, 64),
                    TargetSystem = Truncate(targetSystem, 64),
                    DataType = Truncate(dataType, 64),
                    Stage = Truncate(stage, 128),
                    EventType = Truncate(eventType, 128),
                    Status = Truncate(status, 64),
                    PeriodStartUtc = periodStartUtc,
                    PeriodEndUtc = periodEndUtc,
                    TransactionDate = transactionDate,
                    ItemCount = itemCount,
                    RowCount = rowCount,
                    DurationMs = durationMs,
                    HttpStatusCode = httpStatusCode,
                    OutputFileName = Truncate(outputFileName, 512),
                    OutputFullPath = Truncate(outputFullPath, 1024),
                    ExternalReference = Truncate(externalReference, 256),
                    Message = Truncate(message, 1024),
                    DetailsJson = detailsJson,
                    ErrorMessage = exception?.ToString(),
                    WhenCreatedUTC = now,
                    WhenUpdatedUTC = now
                };

                _fourthPipelineContext.PipelineEventLogs.Add(record);
                await _fourthPipelineContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write Square to Fourth pipeline event log.");
                Log.Error(ex, "Failed to write Square to Fourth pipeline event log.");
            }
        }

        private static void WriteCsv(IEnumerable<TransactionDatasetRow> rows, string fullPath)
        {
            using var writer = new StreamWriter(fullPath);
            using var csv = new CsvWriter(writer, CultureInfo.CurrentCulture);
            csv.WriteRecords(rows);
            csv.Flush();
        }

        private static string ToJson(object value)
        {
            return value == null
                ? null
                : JsonSerializer.Serialize(value);
        }

        private static string BuildSafeFileName(string fileName)
        {
            var invalidCharacters = new string(Path.GetInvalidFileNameChars());
            var pattern = $"[{Regex.Escape(invalidCharacters)}]+";
            return Regex.Replace(fileName, pattern, "_");
        }

        private static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
            {
                return value;
            }

            return value.Substring(0, maxLength);
        }

        private DateTime GetConfiguredUtcDateTime(string key, DateTime fallback)
        {
            var value = _configuration.GetValue<string>(key);
            if (string.IsNullOrWhiteSpace(value))
            {
                return fallback;
            }

            if (DateTimeOffset.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out var parsed))
            {
                return parsed.UtcDateTime;
            }

            throw new InvalidOperationException($"Configuration value '{key}' must be a valid UTC date/time. Current value: '{value}'.");
        }

        private IReadOnlyDictionary<string, string> BuildEmployeeNumberMap(StoreIntegration storeIntegration, string squareLocationId)
        {
            var employeeNumberMap = ReadEmployeeNumberMap(
                _configuration.GetSection("SquareToFourthTimesheets:EmployeeNumberMappings"));
            var locationEmployeeNumberMap = ReadEmployeeNumberMap(
                _configuration.GetSection($"SquareToFourthTimesheets:LocationEmployeeNumberMappings:{squareLocationId}"));

            foreach (var mapping in locationEmployeeNumberMap)
            {
                employeeNumberMap[mapping.Key] = mapping.Value;
            }

            var databaseEmployeeNumberMap = storeIntegration.SquareEmployeeMappings?
                .Where(x => x.Active
                    && !string.IsNullOrWhiteSpace(x.SquareTeamMemberId)
                    && !string.IsNullOrWhiteSpace(x.FourthEmployeeNumber))
                .ToList() ?? new List<data.pipeline.fourth.com.Models.Mappings.SquareEmployeeMapping>();

            foreach (var mapping in databaseEmployeeNumberMap)
            {
                employeeNumberMap[mapping.SquareTeamMemberId] = mapping.FourthEmployeeNumber;
            }

            return employeeNumberMap;
        }

        private static Dictionary<string, string> ReadEmployeeNumberMap(IConfigurationSection section)
        {
            var mappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (section == null || !section.Exists())
            {
                return mappings;
            }

            foreach (var child in section.GetChildren())
            {
                if (!string.IsNullOrWhiteSpace(child.Key) && !string.IsNullOrWhiteSpace(child.Value))
                {
                    mappings[child.Key] = child.Value;
                }
            }

            return mappings;
        }
    }
}
