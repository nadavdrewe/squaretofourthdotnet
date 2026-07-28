using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RevelFourthPipeline.Domain.Configuration;
using RevelFourthPipeline.Domain.Fourth;
using RevelFourthPipeline.Infrastructure.Abstractions;

namespace RevelFourthPipeline.Infrastructure.Fourth;

public sealed class FourthSoapClient(
    HttpClient httpClient,
    IOptions<RevelFourthPipelineOptions> options,
    ILogger<FourthSoapClient> logger)
    : IFourthSoapClient
{
    private static readonly XNamespace Soap = "http://schemas.xmlsoap.org/soap/envelope/";
    private static readonly XNamespace Fourth = "http://ws.fourthhospitality.com/";

    private readonly FourthOptions _options = options.Value.Fourth;

    public async Task<FourthAuthenticationToken> LoginAsync(
        string userName,
        string password,
        CancellationToken cancellationToken)
    {
        var envelope = new XDocument(
            new XElement(Soap + "Envelope",
                new XAttribute(XNamespace.Xmlns + "soap", Soap),
                new XElement(Soap + "Body",
                    new XElement(Fourth + "Login",
                        new XElement(Fourth + "userName", userName),
                        new XElement(Fourth + "password", password)))));

        var rawResponse = await PostSoapAsync("Login", envelope, cancellationToken).ConfigureAwait(false);
        var document = XDocument.Parse(rawResponse);
        ThrowIfSoapFault(document);

        var sessionId = document.Descendants(Fourth + "SessionID").FirstOrDefault()?.Value;
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            throw new InvalidOperationException("Fourth login did not return an AuthenticationHeader SessionID.");
        }

        logger.LogInformation("Fourth SOAP login succeeded.");
        return new FourthAuthenticationToken
        {
            SessionId = sessionId,
            RawResponse = rawResponse
        };
    }

    public async Task<FourthSubmitResult> SubmitSalesAsync(
        FourthAuthenticationToken token,
        string salesXml,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(token.SessionId))
        {
            throw new InvalidOperationException("Fourth authentication token is missing a SessionID.");
        }

        var salesRoot = XDocument.Parse(salesXml).Root
                        ?? throw new InvalidOperationException("Fourth sales XML has no root element.");

        var envelope = new XDocument(
            new XElement(Soap + "Envelope",
                new XAttribute(XNamespace.Xmlns + "soap", Soap),
                new XElement(Soap + "Header",
                    new XElement(Fourth + "AuthenticationHeader",
                        new XElement(Fourth + "SessionID", token.SessionId))),
                new XElement(Soap + "Body",
                    new XElement(Fourth + "SubmitSales",
                        new XElement(Fourth + "sales", salesRoot)))));

        var rawResponse = await PostSoapAsync("SubmitSales", envelope, cancellationToken).ConfigureAwait(false);
        var document = XDocument.Parse(rawResponse);
        ThrowIfSoapFault(document);

        var resultText = document.Descendants(Fourth + "SubmitSalesResult").FirstOrDefault()?.Value;
        if (!double.TryParse(resultText, NumberStyles.Any, CultureInfo.InvariantCulture, out var resultCode))
        {
            throw new InvalidOperationException("Fourth SubmitSales did not return a numeric SubmitSalesResult.");
        }

        logger.LogInformation("Fourth SubmitSales returned {ResultCode}.", resultCode);
        return new FourthSubmitResult
        {
            ResultCode = resultCode,
            RawResponse = rawResponse
        };
    }

    private async Task<string> PostSoapAsync(string action, XDocument envelope, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.SoapEndpoint))
        {
            throw new InvalidOperationException("Fourth SOAP endpoint is not configured.");
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, _options.SoapEndpoint);
        request.Headers.Add("SOAPAction", $"\"http://ws.fourthhospitality.com/{action}\"");

        var xml = envelope.Declaration is null
            ? envelope.ToString(SaveOptions.DisableFormatting)
            : envelope.ToString(SaveOptions.DisableFormatting);

        request.Content = new StringContent(xml, Encoding.UTF8);
        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("text/xml; charset=utf-8");

        using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var rawResponse = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError(
                "Fourth SOAP action {Action} failed with status {StatusCode}. Body length: {Length}",
                action,
                response.StatusCode,
                rawResponse.Length);

            throw new HttpRequestException(
                $"Fourth SOAP action {action} failed with status {(int)response.StatusCode} {response.ReasonPhrase}.");
        }

        return rawResponse;
    }

    private static void ThrowIfSoapFault(XDocument document)
    {
        var fault = document.Descendants(Soap + "Fault").FirstOrDefault();
        if (fault is null)
        {
            return;
        }

        var faultString = fault.Element("faultstring")?.Value
                          ?? fault.Descendants().FirstOrDefault(x => x.Name.LocalName == "faultstring")?.Value
                          ?? fault.Value;

        throw new InvalidOperationException($"Fourth SOAP fault: {faultString}");
    }
}
