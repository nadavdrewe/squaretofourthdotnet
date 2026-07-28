using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using RevelFourthPipeline.Domain.Configuration;
using RevelFourthPipeline.Domain.Pipeline;
using RevelFourthPipeline.Infrastructure.Revel;

namespace RevelFourthPipeline.Tests;

public class RevelOperationsReportClientTests
{
    [Fact]
    public void BuildOperationsReportUri_UsesDynamicBaseUrlAndLegacyQuery()
    {
        var client = CreateClient("https://dynamictenant.revelup.com/");
        var uri = client.BuildOperationsReportUri(new RevelOperationsRequest
        {
            StoreName = "Test Store",
            RevelEstablishmentId = 42,
            RangeStart = new DateTime(2026, 6, 6, 4, 0, 0),
            RangeEnd = new DateTime(2026, 6, 7, 4, 0, 0)
        });

        Assert.StartsWith("https://dynamictenant.revelup.com/reports/operations/json/", uri.ToString());
        Assert.Contains("show_opened=1", uri.Query);
        Assert.Contains("show_unpaid=1", uri.Query);
        Assert.Contains("show_irregular=1", uri.Query);
        Assert.Contains("range_from=2026-06-06T04%3A00%3A00", uri.Query);
        Assert.Contains("range_to=2026-06-07T04%3A00%3A00", uri.Query);
        Assert.Contains("establishment=42", uri.Query);
    }

    [Fact]
    public async Task GetOperationsReportAsync_SendsRequiredHeadersAndParsesFlexibleNumbers()
    {
        var handler = new CapturingHandler("""
            {
                  "product_mix_data": [
                    {
                      "product_sku": "1001",
                      "product_name": "Flat White",
                      "n_comps": 0,
                      "n_items": "2",
                      "taxable_sales": "10.00",
                      "untaxable_sales": 0,
                      "tax": "2.00",
                      "price": 12.00
                    }
                  ],
                  "sales_data": { "gross_sales": 12.00, "net_sales": "10.00", "sales_tax": "2.00" },
                  "tax_data": [],
                  "discounts_data": [],
                  "voids_data": []
                }
            """);

        var client = CreateClient("https://tenant.revelup.com/", handler);

        var report = await client.GetOperationsReportAsync(new RevelOperationsRequest
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
        Assert.Equal(10m, report.ProductMixData.Single().TaxableSales);
        Assert.Equal(2m, report.ProductMixData.Single().Tax);
        Assert.Equal("0", report.ProductMixData.Single().NumberOfComps);
        Assert.Equal("12.00", report.SalesData.GrossSales);
    }

    private static RevelOperationsReportClient CreateClient(
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
        return new RevelOperationsReportClient(
            new StubHttpClientFactory(httpClient),
            options,
            NullLogger<RevelOperationsReportClient>.Instance);
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
