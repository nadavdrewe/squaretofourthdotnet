using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RevelFourthPipeline.Domain.Configuration;
using RevelFourthPipeline.Domain.Fourth;
using RevelFourthPipeline.Domain.Pipeline;
using RevelFourthPipeline.Domain.Revel;
using RevelFourthPipeline.Infrastructure.Abstractions;

namespace RevelFourthPipeline.Infrastructure.Pipeline;

public sealed class RevelFourthPipelineRunner(
    IOptions<RevelFourthPipelineOptions> options,
    IRevelFourthIntegrationSource integrationSource,
    IRevelOperationsReportClient revelClient,
    IRevelProductMixReportClient productMixClient,
    IRevelOperationsToFourthMapper mapper,
    IRevelProductMixToFourthMapper productMixMapper,
    IFourthSalesXmlBuilder xmlBuilder,
    IFourthSoapClient fourthSoapClient,
    IFourthSubmissionLedger submissionLedger,
    ILogger<RevelFourthPipelineRunner> logger)
    : IRevelFourthPipelineRunner
{
    private readonly RevelFourthPipelineOptions _options = options.Value;

    public async Task<IReadOnlyList<StorePipelineRunResult>> RunForRangeAsync(
        DateTime rangeStart,
        DateTime rangeEnd,
        CancellationToken cancellationToken)
    {
        var integrations = await integrationSource.GetActiveIntegrationsAsync(cancellationToken).ConfigureAwait(false);
        if (integrations.Count == 0)
        {
            logger.LogWarning("No active Revel/Fourth integrations are configured for the pipeline.");
            return [];
        }

        var results = new List<StorePipelineRunResult>();

        foreach (var integration in integrations)
        {
            cancellationToken.ThrowIfCancellationRequested();
            results.Add(await RunStoreAsync(integration, rangeStart, rangeEnd, cancellationToken).ConfigureAwait(false));
        }

        return results;
    }

    private async Task<StorePipelineRunResult> RunStoreAsync(
        RevelFourthIntegration integration,
        DateTime rangeStart,
        DateTime rangeEnd,
        CancellationToken cancellationToken)
    {
        var context = new StoreRunContext
        {
            BrandId = integration.BrandId,
            BrandName = integration.BrandName,
            DatabaseEstablishmentId = integration.DatabaseEstablishmentId,
            StoreName = integration.StoreName,
            RevelEstablishmentId = integration.RevelEstablishmentId,
            FourthLocation = integration.FourthLocation,
            FourthRevenueCentre = integration.FourthRevenueCentre,
            RangeStart = rangeStart,
            RangeEnd = rangeEnd
        };

        try
        {
            var (sourceReport, transactions) = await PullAndMapAsync(
                integration,
                context,
                cancellationToken).ConfigureAwait(false);

            if (transactions.Count == 0)
            {
                logger.LogWarning(
                    "No Fourth transactions generated for {StoreName} from {Start} to {End}.",
                    context.StoreName,
                    context.RangeStart,
                    context.RangeEnd);

                return new StorePipelineRunResult
                {
                    Context = context,
                    SourceReport = sourceReport,
                    DryRun = _options.DryRun,
                    Succeeded = true,
                    Message = "No transactions generated; Fourth submit skipped."
                };
            }

            var submission = new FourthSalesSubmission
            {
                OrganisationId = integration.FourthOrganisationId,
                UserName = integration.FourthUsername,
                Password = integration.FourthPassword,
                SalesDate = context.RangeStart.Date,
                Location = context.FourthLocation,
                RevenueCentre = context.FourthRevenueCentre,
                Transactions = transactions
            };

            var xml = xmlBuilder.BuildXml(submission).Xml;

            if (_options.DryRun)
            {
                var fourthLoginValidated = false;
                if (_options.ValidateFourthLoginInDryRun)
                {
                    await fourthSoapClient
                        .LoginAsync(integration.FourthUsername, integration.FourthPassword, cancellationToken)
                        .ConfigureAwait(false);

                    fourthLoginValidated = true;
                    logger.LogInformation(
                        "Dry run validated Fourth SOAP login for {StoreName}; SubmitSales skipped.",
                        context.StoreName);
                }

                logger.LogInformation(
                    "Dry run generated Fourth XML for {StoreName}: {TransactionCount} transactions, Net={TotalNetSales}, VAT={TotalVat}, Gross={TotalGrossSales}.",
                    context.StoreName,
                    transactions.Count,
                    transactions.Sum(x => x.TotalNetSales),
                    transactions.Sum(x => x.Vat),
                    transactions.Sum(x => x.TotalGrossSales));

                return new StorePipelineRunResult
                {
                    Context = context,
                    SourceReport = sourceReport,
                    Transactions = transactions,
                    FourthXml = xml,
                    DryRun = true,
                    FourthLoginValidated = fourthLoginValidated,
                    Succeeded = true,
                    Message = fourthLoginValidated
                        ? "Dry run completed; Fourth login validated and SubmitSales skipped."
                        : "Dry run completed; Fourth submit skipped."
                };
            }

            var ledgerEntry = FourthSubmissionLedgerEntry.Create(
                ResolveSourceName(),
                context,
                ComputeSha256(xml));

            if (_options.RunLedger.Enabled
                && !_options.RunLedger.AllowDuplicateLiveSubmissions
                && await submissionLedger.HasSuccessfulSubmissionAsync(ledgerEntry, cancellationToken).ConfigureAwait(false))
            {
                logger.LogWarning(
                    "Skipping duplicate live Fourth submission for {StoreName} {RangeStart} to {RangeEnd}; payload hash {PayloadSha256} is already in the run ledger.",
                    context.StoreName,
                    context.RangeStart,
                    context.RangeEnd,
                    ledgerEntry.PayloadSha256);

                return new StorePipelineRunResult
                {
                    Context = context,
                    SourceReport = sourceReport,
                    Transactions = transactions,
                    FourthXml = xml,
                    DryRun = false,
                    Succeeded = true,
                    Message = "Duplicate live submission skipped by run ledger."
                };
            }

            var token = await fourthSoapClient
                .LoginAsync(integration.FourthUsername, integration.FourthPassword, cancellationToken)
                .ConfigureAwait(false);

            var submitResult = await fourthSoapClient
                .SubmitSalesAsync(token, xml, cancellationToken)
                .ConfigureAwait(false);

            if (submitResult.Succeeded)
            {
                await submissionLedger
                    .RecordSuccessfulSubmissionAsync(ledgerEntry, cancellationToken)
                    .ConfigureAwait(false);
            }

            return new StorePipelineRunResult
            {
                Context = context,
                SourceReport = sourceReport,
                Transactions = transactions,
                FourthXml = xml,
                SubmitResult = submitResult,
                DryRun = false,
                FourthLoginValidated = true,
                Succeeded = submitResult.Succeeded,
                Message = submitResult.Succeeded
                    ? $"Fourth SubmitSales returned {submitResult.ResultCode}."
                    : "Fourth SubmitSales returned 0."
            };
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Store pipeline run failed for {StoreName} establishment {EstablishmentId}.",
                context.StoreName,
                context.RevelEstablishmentId);

            return new StorePipelineRunResult
            {
                Context = context,
                DryRun = _options.DryRun,
                Succeeded = false,
                Message = ex.Message
            };
        }
    }

    private async Task<(object SourceReport, List<FourthSalesTransactionDraft> Transactions)> PullAndMapAsync(
        RevelFourthIntegration integration,
        StoreRunContext context,
        CancellationToken cancellationToken)
    {
        if (UseProductMixSource())
        {
            var report = await productMixClient.GetProductMixReportAsync(
                new RevelProductMixRequest
                {
                    StoreName = context.StoreName,
                    RevelEstablishmentId = context.RevelEstablishmentId,
                    RevelBaseUrl = integration.RevelBaseUrl,
                    RevelApiKeySecret = integration.RevelApiKeySecret,
                    RangeStart = context.RangeStart,
                    RangeEnd = context.RangeEnd
                },
                cancellationToken).ConfigureAwait(false);

            var transactions = productMixMapper.Map(report, context).ToList();
            LogProductMixReconciliation(context, report, transactions);
            return (report, transactions);
        }

        var operationsReport = await revelClient.GetOperationsReportAsync(
            new RevelOperationsRequest
            {
                StoreName = context.StoreName,
                RevelEstablishmentId = context.RevelEstablishmentId,
                RevelBaseUrl = integration.RevelBaseUrl,
                RevelApiKeySecret = integration.RevelApiKeySecret,
                RangeStart = context.RangeStart,
                RangeEnd = context.RangeEnd
            },
            cancellationToken).ConfigureAwait(false);

        var operationsTransactions = mapper.Map(operationsReport, context).ToList();
        LogOperationsReconciliation(context, operationsReport, operationsTransactions);
        return (operationsReport, operationsTransactions);
    }

    private bool UseProductMixSource()
    {
        return string.IsNullOrWhiteSpace(_options.Revel.SalesReportSource)
               || string.Equals(_options.Revel.SalesReportSource, "ProductMix", StringComparison.OrdinalIgnoreCase);
    }

    private string ResolveSourceName()
    {
        return UseProductMixSource() ? "RevelProductMix" : "RevelOperations";
    }

    private void LogOperationsReconciliation(
        StoreRunContext context,
        OperationsReport report,
        IReadOnlyList<FourthSalesTransactionDraft> transactions)
    {
        var productMixRows = report.ProductMixData
            .Where(IsIncludedProductMixRow)
            .ToList();

        var sourceNet = RoundCurrency(productMixRows.Sum(x => x.TaxableSales + x.UntaxableSales));
        var sourceTax = RoundCurrency(productMixRows.Sum(x => x.Tax));
        var sourceGross = RoundCurrency(sourceNet + sourceTax);

        var generatedNet = transactions.Sum(x => x.TotalNetSales);
        var generatedTax = transactions.Sum(x => x.Vat);
        var generatedGross = transactions.Sum(x => x.TotalGrossSales);

        logger.LogInformation(
            "Product-mix/Fourth reconciliation for {StoreName}: SourceRows={SourceRows}, SourceNet={SourceNet}, XmlNet={XmlNet}, NetDelta={NetDelta}; SourceVAT={SourceVat}, XmlVAT={XmlVat}, VatDelta={VatDelta}; SourceGross={SourceGross}, XmlGross={XmlGross}, GrossDelta={GrossDelta}.",
            context.StoreName,
            productMixRows.Count,
            sourceNet.ToString("0.00", CultureInfo.InvariantCulture),
            generatedNet.ToString("0.00", CultureInfo.InvariantCulture),
            (sourceNet - generatedNet).ToString("0.00", CultureInfo.InvariantCulture),
            sourceTax.ToString("0.00", CultureInfo.InvariantCulture),
            generatedTax.ToString("0.00", CultureInfo.InvariantCulture),
            (sourceTax - generatedTax).ToString("0.00", CultureInfo.InvariantCulture),
            sourceGross.ToString("0.00", CultureInfo.InvariantCulture),
            generatedGross.ToString("0.00", CultureInfo.InvariantCulture),
            (sourceGross - generatedGross).ToString("0.00", CultureInfo.InvariantCulture));

        logger.LogInformation(
            "Revel sales_data for {StoreName}: NetSales={NetSales}, TotalSales={TotalSales}, GrossSales={GrossSales}, SalesTax={SalesTax}.",
            context.StoreName,
            FormatMoney(ParseDecimal(report.SalesData.NetSales)),
            FormatMoney(ParseDecimal(report.SalesData.TotalSales)),
            FormatMoney(ParseDecimal(report.SalesData.GrossSales)),
            FormatMoney(ParseDecimal(report.SalesData.SalesTax)));
    }

    private void LogProductMixReconciliation(
        StoreRunContext context,
        ProductMixReport report,
        IReadOnlyList<FourthSalesTransactionDraft> transactions)
    {
        var productRows = report.ProductMix
            .Where(IsIncludedProductMixReportRow)
            .ToList();

        var sourceNet = RoundCurrency(productRows.Sum(x => x.TaxableSales + x.UntaxableSales));
        var sourceTax = RoundCurrency(productRows.Sum(x => x.Tax));
        var sourceGross = RoundCurrency(sourceNet + sourceTax);

        var generatedNet = transactions.Sum(x => x.TotalNetSales);
        var generatedTax = transactions.Sum(x => x.Vat);
        var generatedGross = transactions.Sum(x => x.TotalGrossSales);

        logger.LogInformation(
            "ProductMix/Fourth reconciliation for {StoreName}: SourceRows={SourceRows}, XmlTransactions={TransactionCount}, SourceNet={SourceNet}, XmlNet={XmlNet}, NetDelta={NetDelta}; SourceVAT={SourceVat}, XmlVAT={XmlVat}, VatDelta={VatDelta}; SourceGross={SourceGross}, XmlGross={XmlGross}, GrossDelta={GrossDelta}.",
            context.StoreName,
            productRows.Count,
            transactions.Count,
            sourceNet.ToString("0.00", CultureInfo.InvariantCulture),
            generatedNet.ToString("0.00", CultureInfo.InvariantCulture),
            (sourceNet - generatedNet).ToString("0.00", CultureInfo.InvariantCulture),
            sourceTax.ToString("0.00", CultureInfo.InvariantCulture),
            generatedTax.ToString("0.00", CultureInfo.InvariantCulture),
            (sourceTax - generatedTax).ToString("0.00", CultureInfo.InvariantCulture),
            sourceGross.ToString("0.00", CultureInfo.InvariantCulture),
            generatedGross.ToString("0.00", CultureInfo.InvariantCulture),
            (sourceGross - generatedGross).ToString("0.00", CultureInfo.InvariantCulture));
    }

    private static decimal? ParseDecimal(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var invariant))
        {
            return Math.Round(invariant, 2, MidpointRounding.AwayFromZero);
        }

        if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out var current))
        {
            return Math.Round(current, 2, MidpointRounding.AwayFromZero);
        }

        return null;
    }

    private static string FormatMoney(decimal? value)
    {
        return value.HasValue ? value.Value.ToString("0.00", CultureInfo.InvariantCulture) : "n/a";
    }

    private static bool IsIncludedProductMixRow(ProductMixData row)
    {
        if (string.Equals(row.RowType, "totals_row", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return !string.IsNullOrWhiteSpace(row.ProductSku)
               || !string.IsNullOrWhiteSpace(row.ProductName)
               || !string.IsNullOrWhiteSpace(row.ProductDescription);
    }

    private static bool IsIncludedProductMixReportRow(ProductMixRow row)
    {
        return string.Equals(row.RowType, "Product", StringComparison.OrdinalIgnoreCase)
               || string.Equals(row.RowType, "Parent_Product", StringComparison.OrdinalIgnoreCase);
    }

    private static decimal RoundCurrency(decimal value)
    {
        return Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }

    private static string ComputeSha256(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
