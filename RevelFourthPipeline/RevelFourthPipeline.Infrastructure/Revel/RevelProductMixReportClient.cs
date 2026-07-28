using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RevelFourthPipeline.Domain.Common;
using RevelFourthPipeline.Domain.Configuration;
using RevelFourthPipeline.Domain.Pipeline;
using RevelFourthPipeline.Domain.Revel;
using RevelFourthPipeline.Infrastructure.Abstractions;
using RevelFourthPipeline.Infrastructure.Serialization;

namespace RevelFourthPipeline.Infrastructure.Revel;

public sealed class RevelProductMixReportClient(
    IHttpClientFactory httpClientFactory,
    IOptions<RevelFourthPipelineOptions> options,
    ILogger<RevelProductMixReportClient> logger)
    : IRevelProductMixReportClient
{
    public const string HttpClientName = "revel-product-mix";

    private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

    private readonly RevelFourthPipelineOptions _options = options.Value;

    public Uri BuildProductMixReportUri(RevelProductMixRequest request)
    {
        var baseUrl = ResolveRevelBaseUrl(request);
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException("Revel base URL is not configured.");
        }

        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri))
        {
            throw new InvalidOperationException($"Revel base URL is invalid: '{baseUrl}'.");
        }

        var baseText = baseUri.ToString().TrimEnd('/');
        var reportUri = new Uri($"{baseText}/reports/product_mix/data/");

        var query = new Dictionary<string, string>
        {
            ["sort_by"] = "",
            ["sort_reverse"] = "",
            ["combo_expand"] = "",
            ["employee"] = "",
            ["online_app"] = "",
            ["online_app_type"] = "",
            ["online_app_platform"] = "",
            ["dining_option"] = "",
            ["show_opened"] = "1",
            ["show_unpaid"] = "1",
            ["show_irregular"] = "1",
            ["sort_view"] = "0",
            ["show_product"] = "1",
            ["show_modifiers"] = "1",
            ["show_sku"] = "1",
            ["show_class"] = "1",
            ["show_category"] = "1",
            ["quantity_settings"] = "1",
            ["show_price_percent"] = "1",
            ["taxable_not_taxable"] = "1",
            ["item_discount"] = "1",
            ["order_discount"] = "1",
            ["no-filter"] = "0",
            ["range_from"] = request.RangeStart.ToRevelDate(),
            ["range_to"] = request.RangeEnd.ToRevelDate(),
            ["establishment"] = request.RevelEstablishmentId.ToString(),
            ["format"] = "json"
        };

        var builder = new UriBuilder(reportUri)
        {
            Query = string.Join("&", query.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"))
        };

        return builder.Uri;
    }

    public async Task<ProductMixReport> GetProductMixReportAsync(
        RevelProductMixRequest request,
        CancellationToken cancellationToken)
    {
        var uri = BuildProductMixReportUri(request);
        var client = httpClientFactory.CreateClient(HttpClientName);
        var baseUrl = ResolveRevelBaseUrl(request);
        var apiKeySecret = ResolveApiKeySecret(request);

        using var message = new HttpRequestMessage(HttpMethod.Get, uri);
        message.Headers.Accept.Clear();
        message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        message.Headers.Add("API-AUTHENTICATION", apiKeySecret);
        message.Headers.Add("Referer", baseUrl);

        logger.LogInformation(
            "Pulling Revel product mix report for {StoreName} establishment {EstablishmentId} from {Start} to {End}",
            request.StoreName,
            request.RevelEstablishmentId,
            request.RangeStart,
            request.RangeEnd);

        using var response = await client.SendAsync(message, cancellationToken).ConfigureAwait(false);
        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError(
                "Revel product mix report failed with status {StatusCode}. Body length: {Length}",
                response.StatusCode,
                content.Length);

            throw new HttpRequestException(
                $"Revel product mix report failed with status {(int)response.StatusCode} {response.ReasonPhrase}.");
        }

        try
        {
            return JsonSerializer.Deserialize<ProductMixReport>(content, JsonOptions)
                   ?? throw new JsonException("Revel returned an empty product mix report payload.");
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Failed to deserialize Revel product mix report. Body length: {Length}", content.Length);
            throw;
        }
    }

    private string ResolveRevelBaseUrl(RevelProductMixRequest request)
    {
        return string.IsNullOrWhiteSpace(request.RevelBaseUrl)
            ? _options.Revel.BaseUrl
            : request.RevelBaseUrl;
    }

    private string ResolveApiKeySecret(RevelProductMixRequest request)
    {
        return string.IsNullOrWhiteSpace(request.RevelApiKeySecret)
            ? _options.Revel.ApiKeySecret
            : request.RevelApiKeySecret;
    }

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };
        options.Converters.Add(new FlexibleStringJsonConverter());
        options.Converters.Add(new FlexibleDecimalJsonConverter());
        return options;
    }
}
