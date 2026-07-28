using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using RevelFourthPipeline.Domain.Configuration;
using RevelFourthPipeline.Domain.Fourth;
using RevelFourthPipeline.Infrastructure.Fourth;

namespace RevelFourthPipeline.Tests;

public class FourthSoapClientTests
{
    [Fact]
    public async Task LoginAndSubmitSales_UseSoapActionsAndParseResults()
    {
        var handler = new QueueHandler(
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

        var client = new FourthSoapClient(
            new HttpClient(handler),
            Options.Create(new RevelFourthPipelineOptions
            {
                Fourth = new FourthOptions
                {
                    SoapEndpoint = "http://ws.fourthhospitality.com/fhapi.asmx"
                }
            }),
            NullLogger<FourthSoapClient>.Instance);

        var token = await client.LoginAsync("user", "pass", CancellationToken.None);
        var result = await client.SubmitSalesAsync(
            token,
            "<FourthHeader><OrganisationHeader /></FourthHeader>",
            CancellationToken.None);

        Assert.Equal("session-123", token.SessionId);
        Assert.Equal(1d, result.ResultCode);
        Assert.Equal(
            ["\"http://ws.fourthhospitality.com/Login\"", "\"http://ws.fourthhospitality.com/SubmitSales\""],
            handler.SoapActions);
        Assert.Contains("<SessionID>session-123</SessionID>", handler.RequestBodies[1]);
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
}
