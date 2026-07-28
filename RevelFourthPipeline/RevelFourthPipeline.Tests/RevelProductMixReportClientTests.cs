using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using RevelFourthPipeline.Domain.Configuration;
using RevelFourthPipeline.Domain.Pipeline;
using RevelFourthPipeline.Infrastructure.Revel;

namespace RevelFourthPipeline.Tests;

public class RevelProductMixReportClientTests
{
    [Fact]
    public void BuildProductMixReportUri_UsesLegacyProductMixQuery()
    {
        var client = CreateClient("https://dynamictenant.revelup.com/");

        var uri = client.BuildProductMixReportUri(new RevelProductMixRequest
        {
            StoreName = "Test Store",
            RevelEstablishmentId = 42,
            RangeStart = new DateTime(2026, 6, 6, 4, 0, 0),
            RangeEnd = new DateTime(2026, 6, 7, 4, 0, 0)
        });

        Assert.StartsWith("https://dynamictenant.revelup.com/reports/product_mix/data/", uri.ToString());
        Assert.Contains("show_product=1", uri.Query);
        Assert.Contains("show_modifiers=1", uri.Query);
        Assert.Contains("show_sku=1", uri.Query);
        Assert.Contains("show_class=1", uri.Query);
        Assert.Contains("show_category=1", uri.Query);
        Assert.Contains("range_from=2026-06-06T04%3A00%3A00", uri.Query);
        Assert.Contains("range_to=2026-06-07T04%3A00%3A00", uri.Query);
        Assert.Contains("establishment=42", uri.Query);
        Assert.Contains("format=json", uri.Query);
    }

    [Fact]
    public async Task GetProductMixReportAsync_SendsRequiredHeadersAndParsesRows()
    {
        var handler = new CapturingHandler("""
            {
              "product_fields": [["product_sku", "SKU"]],
              "product_classes": [],
              "categories": [],
              "productmix": [
                {
                  "product_sku": 1001,
                  "product_name": "Flat White",
                  "row_type": "Product",
                  "n_items": 2,
                  "taxable_sales": "10.00",
                  "untaxable_sales": 0,
                  "tax": "2.00",
                  "price": 10.00,
                  "gm_percent": "n/a"
                }
              ]
            }
            """);

        var client = CreateClient("https://tenant.revelup.com/", handler);

        var report = await client.GetProductMixReportAsync(new RevelProductMixRequest
        {
            StoreName = "Test Store",
            RevelEstablishmentId = 1,
            RangeStart = new DateTime(2026, 6, 6, 4, 0, 0),
            RangeEnd = new DateTime(2026, 6, 7, 4, 0, 0)
        }, CancellationToken.None);

        Assert.Equal("application/json", handler.LastRequest!.Headers.Accept.Single().MediaType);
        Assert.True(handler.LastRequest.Headers.TryGetValues("API-AUTHENTICATION", out var authValues));
        Assert.Equal("test-key-secret", authValues.Single());
        Assert.True(handler.LastRequest.Headers.TryGetValues("Referer", out var refererValues));
        Assert.Equal("https://tenant.revelup.com/", refererValues.Single());
        Assert.Equal("1001", report.ProductMix.Single().ProductSku);
        Assert.Equal("2", report.ProductMix.Single().NumberOfItems);
        Assert.Equal(10m, report.ProductMix.Single().TaxableSales);
    }

    private static RevelProductMixReportClient CreateClient(
        string baseUrl,
        HttpMessageHandler? handler = null)
    {
        var options = Options.Create(new RevelFourthPipelineOptions
        {
            Revel = new RevelOptions
            {
                BaseUrl = baseUrl,
                ApiKeySecret = "test-key-secret"
            }
        });

        var httpClient = new HttpClient(handler ?? new CapturingHandler("{}"));
        return new RevelProductMixReportClient(
            new StubHttpClientFactory(httpClient),
            options,
            NullLogger<RevelProductMixReportClient>.Instance);
    }

    private sealed class StubHttpClientFactory(HttpClient client) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => client;
    }

    private sealed class CapturingHandler(string responseBody) : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseBody)
            });
        }
    }
}
