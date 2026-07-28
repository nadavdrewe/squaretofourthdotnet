using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using RevelFourthPipeline.Domain.Configuration;
using RevelFourthPipeline.Domain.Fourth;
using RevelFourthPipeline.Domain.Pipeline;
using RevelFourthPipeline.Domain.Revel;
using RevelFourthPipeline.Infrastructure.Configuration;
using RevelFourthPipeline.Infrastructure.Abstractions;
using RevelFourthPipeline.Infrastructure.Fourth;
using RevelFourthPipeline.Infrastructure.Mapping;
using RevelFourthPipeline.Infrastructure.Pipeline;
using RevelFourthPipeline.Infrastructure.Revel;

namespace RevelFourthPipeline.Tests;

public class RevelFourthPipelineRunnerTests
{
    [Fact]
    public async Task RunForRangeAsync_DryRun_PullsMapsAndBuildsXmlWithoutSubmittingToFourth()
    {
        var options = Options.Create(new RevelFourthPipelineOptions
        {
            DryRun = true,
            Revel = new RevelOptions
            {
                BaseUrl = "https://tenant.revelup.com/",
                ApiKeySecret = "test-key-secret",
                SalesReportSource = "Operations"
            },
            Fourth = new FourthOptions
            {
                Username = "fourth-user",
                Password = "fourth-pass",
                OrganisationId = "ORG1",
                DefaultLocation = "LOC1",
                DefaultRevenueCentre = "1"
            },
            Stores =
            [
                new StorePipelineOptions
                {
                    Name = "Store One",
                    RevelEstablishmentId = 1,
                    FourthLocation = "LOC1",
                    FourthRevenueCentre = "1",
                    Active = true
                }
            ]
        });

        var revelClient = new RevelOperationsReportClient(
            new StubHttpClientFactory(new HttpClient(new StaticJsonHandler("""
                {
                  "product_mix_data": [
                    {
                      "product_sku": "1001",
                      "product_name": "Flat White",
                      "n_items": "2",
                      "taxable_sales": "10.00",
                      "untaxable_sales": "0.00",
                      "tax": "2.00",
                      "price": "12.00",
                      "product_category": "Coffee"
                    }
                  ],
                  "sales_data": { "gross_sales": "12.00", "net_sales": "10.00", "sales_tax": "2.00" },
                  "tax_data": [],
                  "discounts_data": [],
                  "voids_data": []
                }
                """))),
            options,
            NullLogger<RevelOperationsReportClient>.Instance);

        var runner = new RevelFourthPipelineRunner(
            options,
            new OptionsRevelFourthIntegrationSource(options),
            revelClient,
            new ThrowingProductMixReportClient(),
            new RevelOperationsToFourthMapper(),
            new RevelProductMixToFourthMapper(),
            new FourthSalesXmlBuilder(),
            new ThrowingFourthSoapClient(),
            new InMemoryFourthSubmissionLedger(),
            NullLogger<RevelFourthPipelineRunner>.Instance);

        var results = await runner.RunForRangeAsync(
            new DateTime(2026, 6, 6, 4, 0, 0),
            new DateTime(2026, 6, 7, 4, 0, 0),
            CancellationToken.None);

        var result = Assert.Single(results);
        Assert.True(result.Succeeded);
        Assert.True(result.DryRun);
        Assert.Null(result.SubmitResult);
        Assert.Contains("<FourthHeader", result.FourthXml);
        Assert.Contains("<PLU>1001</PLU>", result.FourthXml);
        Assert.Contains("<TotalNetSales>10.00</TotalNetSales>", result.FourthXml);
        Assert.Contains("<TotalGrossSales>12.00</TotalGrossSales>", result.FourthXml);
    }

    [Fact]
    public async Task RunForRangeAsync_DryRunWithFourthLoginValidation_LogsInButDoesNotSubmit()
    {
        var options = Options.Create(new RevelFourthPipelineOptions
        {
            DryRun = true,
            ValidateFourthLoginInDryRun = true,
            Revel = new RevelOptions
            {
                BaseUrl = "https://tenant.revelup.com/",
                ApiKeySecret = "test-key-secret",
                SalesReportSource = "ProductMix"
            },
            Fourth = new FourthOptions
            {
                Username = "fourth-user",
                Password = "fourth-pass",
                OrganisationId = "ORG1",
                DefaultLocation = "LOC1",
                DefaultRevenueCentre = "1"
            },
            Stores =
            [
                new StorePipelineOptions
                {
                    Name = "Store One",
                    RevelEstablishmentId = 1,
                    FourthLocation = "LOC1",
                    FourthRevenueCentre = "1",
                    Active = true
                }
            ]
        });

        var productMixClient = new RevelProductMixReportClient(
            new StubHttpClientFactory(new HttpClient(new StaticJsonHandler("""
                {
                  "productmix": [
                    {
                      "product_sku": "1001",
                      "product_name": "Flat White",
                      "row_type": "Product",
                      "n_items": "2",
                      "taxable_sales": "10.00",
                      "untaxable_sales": "0.00",
                      "tax": "2.00",
                      "price": "12.00",
                      "product_category": "Coffee"
                    }
                  ]
                }
                """))),
            options,
            NullLogger<RevelProductMixReportClient>.Instance);

        var fourthClient = new RecordingFourthSoapClient();

        var runner = new RevelFourthPipelineRunner(
            options,
            new OptionsRevelFourthIntegrationSource(options),
            new ThrowingOperationsReportClient(),
            productMixClient,
            new RevelOperationsToFourthMapper(),
            new RevelProductMixToFourthMapper(),
            new FourthSalesXmlBuilder(),
            fourthClient,
            new InMemoryFourthSubmissionLedger(),
            NullLogger<RevelFourthPipelineRunner>.Instance);

        var results = await runner.RunForRangeAsync(
            new DateTime(2026, 6, 6, 4, 0, 0),
            new DateTime(2026, 6, 7, 4, 0, 0),
            CancellationToken.None);

        var result = Assert.Single(results);
        Assert.True(result.Succeeded);
        Assert.True(result.DryRun);
        Assert.True(result.FourthLoginValidated);
        Assert.Equal(1, fourthClient.LoginCalls);
        Assert.Equal(0, fourthClient.SubmitSalesCalls);
        Assert.Contains("<PLU>1001</PLU>", result.FourthXml);
    }

    [Fact]
    public async Task RunForRangeAsync_WhenNotDryRun_PullsMapsBuildsXmlAndSubmitsToFourthSoap()
    {
        var options = Options.Create(new RevelFourthPipelineOptions
        {
            DryRun = false,
            Revel = new RevelOptions
            {
                BaseUrl = "https://tenant.revelup.com/",
                ApiKeySecret = "test-key-secret",
                SalesReportSource = "Operations"
            },
            Fourth = new FourthOptions
            {
                SoapEndpoint = "http://ws.fourthhospitality.com/fhapi.asmx",
                Username = "fourth-user",
                Password = "fourth-pass",
                OrganisationId = "ORG1",
                DefaultLocation = "LOC1",
                DefaultRevenueCentre = "1"
            },
            Stores =
            [
                new StorePipelineOptions
                {
                    Name = "Store One",
                    RevelEstablishmentId = 1,
                    FourthLocation = "LOC1",
                    FourthRevenueCentre = "1",
                    Active = true
                }
            ]
        });

        var revelClient = new RevelOperationsReportClient(
            new StubHttpClientFactory(new HttpClient(new StaticJsonHandler("""
                {
                  "product_mix_data": [
                    {
                      "product_sku": "1001",
                      "product_name": "Flat White",
                      "n_items": "2",
                      "taxable_sales": "10.00",
                      "untaxable_sales": "0.00",
                      "tax": "2.00",
                      "price": "12.00",
                      "product_category": "Coffee"
                    }
                  ],
                  "sales_data": { "gross_sales": "12.00", "net_sales": "10.00", "sales_tax": "2.00" },
                  "tax_data": [],
                  "discounts_data": [],
                  "voids_data": []
                }
                """))),
            options,
            NullLogger<RevelOperationsReportClient>.Instance);

        var fourthHandler = new QueueHandler(
            """
            <soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
              <soap:Body>
                <LoginResponse xmlns="http://ws.fourthhospitality.com/">
                  <LoginResult><SessionID>session-123</SessionID></LoginResult>
                </LoginResponse>
              </soap:Body>
            </soap:Envelope>
            """,
            """
            <soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
              <soap:Body>
                <SubmitSalesResponse xmlns="http://ws.fourthhospitality.com/">
                  <SubmitSalesResult>1</SubmitSalesResult>
                </SubmitSalesResponse>
              </soap:Body>
            </soap:Envelope>
            """);

        var fourthClient = new FourthSoapClient(
            new HttpClient(fourthHandler),
            options,
            NullLogger<FourthSoapClient>.Instance);

        var runner = new RevelFourthPipelineRunner(
            options,
            new OptionsRevelFourthIntegrationSource(options),
            revelClient,
            new ThrowingProductMixReportClient(),
            new RevelOperationsToFourthMapper(),
            new RevelProductMixToFourthMapper(),
            new FourthSalesXmlBuilder(),
            fourthClient,
            new InMemoryFourthSubmissionLedger(),
            NullLogger<RevelFourthPipelineRunner>.Instance);

        var results = await runner.RunForRangeAsync(
            new DateTime(2026, 6, 6, 4, 0, 0),
            new DateTime(2026, 6, 7, 4, 0, 0),
            CancellationToken.None);

        var result = Assert.Single(results);
        Assert.True(result.Succeeded);
        Assert.False(result.DryRun);
        Assert.Equal(1d, result.SubmitResult?.ResultCode);
        Assert.Contains("<PLU>1001</PLU>", result.FourthXml);
        Assert.Equal(
            ["\"http://ws.fourthhospitality.com/Login\"", "\"http://ws.fourthhospitality.com/SubmitSales\""],
            fourthHandler.SoapActions);
        Assert.Contains("<SessionID>session-123</SessionID>", fourthHandler.RequestBodies[1]);
        Assert.Contains("<SubmitSales", fourthHandler.RequestBodies[1]);
        Assert.Contains("<FourthHeader", fourthHandler.RequestBodies[1]);
    }

    [Fact]
    public async Task RunForRangeAsync_WhenNotDryRunAndPayloadAlreadySubmitted_SkipsDuplicateSubmit()
    {
        var options = Options.Create(new RevelFourthPipelineOptions
        {
            DryRun = false,
            Revel = new RevelOptions
            {
                BaseUrl = "https://tenant.revelup.com/",
                ApiKeySecret = "test-key-secret",
                SalesReportSource = "ProductMix"
            },
            Fourth = new FourthOptions
            {
                Username = "fourth-user",
                Password = "fourth-pass",
                OrganisationId = "ORG1",
                DefaultLocation = "LOC1",
                DefaultRevenueCentre = "1"
            },
            Stores =
            [
                new StorePipelineOptions
                {
                    Name = "Store One",
                    RevelEstablishmentId = 1,
                    FourthLocation = "LOC1",
                    FourthRevenueCentre = "1",
                    Active = true
                }
            ]
        });

        var productMixClient = new RevelProductMixReportClient(
            new StubHttpClientFactory(new HttpClient(new StaticJsonHandler("""
                {
                  "productmix": [
                    {
                      "product_sku": "1001",
                      "product_name": "Flat White",
                      "row_type": "Product",
                      "n_items": "2",
                      "taxable_sales": "10.00",
                      "untaxable_sales": "0.00",
                      "tax": "2.00",
                      "price": "12.00",
                      "product_category": "Coffee"
                    }
                  ]
                }
                """))),
            options,
            NullLogger<RevelProductMixReportClient>.Instance);

        var fourthClient = new RecordingFourthSoapClient();
        var ledger = new InMemoryFourthSubmissionLedger { AlreadySubmitted = true };

        var runner = new RevelFourthPipelineRunner(
            options,
            new OptionsRevelFourthIntegrationSource(options),
            new ThrowingOperationsReportClient(),
            productMixClient,
            new RevelOperationsToFourthMapper(),
            new RevelProductMixToFourthMapper(),
            new FourthSalesXmlBuilder(),
            fourthClient,
            ledger,
            NullLogger<RevelFourthPipelineRunner>.Instance);

        var results = await runner.RunForRangeAsync(
            new DateTime(2026, 6, 6, 4, 0, 0),
            new DateTime(2026, 6, 7, 4, 0, 0),
            CancellationToken.None);

        var result = Assert.Single(results);
        Assert.True(result.Succeeded);
        Assert.False(result.DryRun);
        Assert.Equal("Duplicate live submission skipped by run ledger.", result.Message);
        Assert.Equal(0, fourthClient.LoginCalls);
        Assert.Equal(0, fourthClient.SubmitSalesCalls);
        Assert.Empty(ledger.Recorded);
    }

    private sealed class StubHttpClientFactory(HttpClient client) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => client;
    }

    private sealed class StaticJsonHandler(string responseBody) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseBody)
            });
        }
    }

    private sealed class QueueHandler(params string[] responses) : HttpMessageHandler
    {
        private readonly Queue<string> _responses = new(responses);

        public List<string> SoapActions { get; } = [];
        public List<string> RequestBodies { get; } = [];

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            SoapActions.Add(request.Headers.GetValues("SOAPAction").Single());
            RequestBodies.Add(await request.Content!.ReadAsStringAsync(cancellationToken));

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_responses.Dequeue())
            };
        }
    }

    private sealed class ThrowingFourthSoapClient : IFourthSoapClient
    {
        public Task<FourthAuthenticationToken> LoginAsync(
            string userName,
            string password,
            CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("Fourth should not be called during dry-run.");
        }

        public Task<FourthSubmitResult> SubmitSalesAsync(
            FourthAuthenticationToken token,
            string salesXml,
            CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("Fourth should not be called during dry-run.");
        }
    }

    private sealed class ThrowingProductMixReportClient : IRevelProductMixReportClient
    {
        public Uri BuildProductMixReportUri(RevelProductMixRequest request)
        {
            throw new InvalidOperationException("Product mix should not be called in operations-source tests.");
        }

        public Task<ProductMixReport> GetProductMixReportAsync(
            RevelProductMixRequest request,
            CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("Product mix should not be called in operations-source tests.");
        }
    }

    private sealed class ThrowingOperationsReportClient : IRevelOperationsReportClient
    {
        public Uri BuildOperationsReportUri(RevelOperationsRequest request)
        {
            throw new InvalidOperationException("Operations should not be called in product-mix-source tests.");
        }

        public Task<OperationsReport> GetOperationsReportAsync(
            RevelOperationsRequest request,
            CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("Operations should not be called in product-mix-source tests.");
        }
    }

    private sealed class RecordingFourthSoapClient : IFourthSoapClient
    {
        public int LoginCalls { get; private set; }
        public int SubmitSalesCalls { get; private set; }

        public Task<FourthAuthenticationToken> LoginAsync(
            string userName,
            string password,
            CancellationToken cancellationToken)
        {
            LoginCalls++;
            return Task.FromResult(new FourthAuthenticationToken { SessionId = "session-123" });
        }

        public Task<FourthSubmitResult> SubmitSalesAsync(
            FourthAuthenticationToken token,
            string salesXml,
            CancellationToken cancellationToken)
        {
            SubmitSalesCalls++;
            return Task.FromResult(new FourthSubmitResult { ResultCode = 1d });
        }
    }

    private sealed class InMemoryFourthSubmissionLedger : IFourthSubmissionLedger
    {
        public bool AlreadySubmitted { get; init; }
        public List<FourthSubmissionLedgerEntry> Recorded { get; } = [];

        public Task<bool> HasSuccessfulSubmissionAsync(
            FourthSubmissionLedgerEntry entry,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(AlreadySubmitted);
        }

        public Task RecordSuccessfulSubmissionAsync(
            FourthSubmissionLedgerEntry entry,
            CancellationToken cancellationToken)
        {
            Recorded.Add(entry);
            return Task.CompletedTask;
        }
    }
}
