using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using web.pipeline.fourth.com.Models;
using web.pipeline.fourth.com.Services;

namespace tests.domain.pipelines.fourth.com;

public sealed class SapAssistantTests
{
    [Test]
    public void ContextIncludesSchemaAndDraftAnswersButNotContactIdentity()
    {
        var document = new SapDiscovery { Company = "Private Brand", Contact = "Named Person", Email = "private@example.com" };
        document.Answers["sap.product"] = "ECC";
        document.Answers["landscape.decisionOwner"] = "Named Owner";
        var context = SapAssistantContext.Build(document, 0, new Dictionary<string, string> { ["sap.product"] = "S/4HANA" }, Array.Empty<SapAssistantTurn>());
        Assert.That(context, Does.Contain("S/4HANA"));
        Assert.That(context, Does.Contain("Which SAP product"));
        Assert.That(context, Does.Not.Contain("Private Brand"));
        Assert.That(context, Does.Not.Contain("Named Person"));
        Assert.That(context, Does.Not.Contain("private@example.com"));
        Assert.That(context, Does.Not.Contain("Named Owner"));
        Assert.That(context, Does.Contain("provided"));
        Assert.That(SapAssistantContext.Instructions, Does.Contain("at most one SAP stock reduction"));
    }

    [Test]
    public void SuggestionsAreRestrictedToVisibleCurrentFieldsAndExactOptions()
    {
        var document = new SapDiscovery();
        document.Answers["inventory.direction"] = "No quantity synchronisation";
        var reply = new SapAssistantReply
        {
            Answer = "Review this with inventory operations.",
            Suggestions = new()
            {
                new() { FieldId = "inventory.frequency", SuggestedValue = "Hourly", Rationale = "Hidden field" },
                new() { FieldId = "inventory.direction", SuggestedValue = "Invented option", Rationale = "Invalid option" },
                new() { FieldId = "inventory.saleCount", SuggestedValue = "Reconciliation only; no second goods issue", Rationale = "Avoid double reduction" },
                new() { FieldId = "finance.tax", SuggestedValue = "Square", Rationale = "Wrong section" }
            }
        };
        var sanitized = SapAssistantContext.Sanitize(reply, document, 2);
        Assert.That(sanitized.Suggestions.Select(s => s.FieldId), Is.EqualTo(new[] { "inventory.saleCount" }));
    }

    [Test]
    public async Task ServiceUsesStatelessStructuredResponseAndSanitizesModelOutput()
    {
        var handler = new AssistantHandler();
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://api.openai.com/v1/") };
        var options = Options.Create(new SapAssistantOptions { Enabled = true, ApiKey = "test-key", Model = "gpt-5.6-terra", MaxOutputTokens = 800 });
        var service = new SapAssistantService(client, options, NullLogger<SapAssistantService>.Instance, new MemoryCache(new MemoryCacheOptions()));
        var document = new SapDiscovery { Email = "never-send@example.com" };
        document.Answers["sap.product"] = "S/4HANA";
        var reply = await service.Ask(document, 0, new SapAssistantRequest { Message = "What do I need to confirm?" }, CancellationToken.None);
        Assert.That(reply.Answer, Is.EqualTo("Confirm the release with the SAP owner."));
        Assert.That(reply.Suggestions.Single().FieldId, Is.EqualTo("sap.release"));
        Assert.That(handler.Requests, Has.Count.EqualTo(2));
        Assert.That(handler.Requests[1], Does.Contain("\"store\":false"));
        Assert.That(handler.Requests[1], Does.Contain("\"type\":\"json_schema\""));
        Assert.That(handler.Requests[1], Does.Contain("gpt-5.6-terra"));
        Assert.That(handler.Requests[1], Does.Not.Contain("never-send@example.com"));
        Assert.That(Count(handler.Requests[1], "What do I need to confirm?"), Is.EqualTo(1));
    }

    [Test]
    public void ServiceRejectsLikelyCredentialsBeforeCallingOpenAI()
    {
        var handler = new AssistantHandler();
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://api.openai.com/v1/") };
        var options = Options.Create(new SapAssistantOptions { Enabled = true, ApiKey = "test-key" });
        var service = new SapAssistantService(client, options, NullLogger<SapAssistantService>.Instance, new MemoryCache(new MemoryCacheOptions()));
        var error = Assert.ThrowsAsync<SapAssistantException>(() => service.Ask(new SapDiscovery(), 0,
            new SapAssistantRequest { Message = "client_secret=do-not-send-this-value" }, CancellationToken.None));
        Assert.That(error.StatusCode, Is.EqualTo(400));
        Assert.That(handler.Requests, Is.Empty);
    }

    [TestCase("someone@example.com")]
    [TestCase("+44 7700 900123")]
    [TestCase("api_key=do-not-send-this-value")]
    public void ServiceRejectsRestrictedDataAnywhereInRequest(string restricted)
    {
        var handler = new AssistantHandler();
        var service = new SapAssistantService(new HttpClient(handler) { BaseAddress = new Uri("https://api.openai.com/v1/") },
            Options.Create(new SapAssistantOptions { Enabled = true, ApiKey = "test-key" }), NullLogger<SapAssistantService>.Instance,
            new MemoryCache(new MemoryCacheOptions()));
        var request = new SapAssistantRequest { Message = "Review the current answers", History = new() { new() { Role = "user", Text = restricted } } };
        var error = Assert.ThrowsAsync<SapAssistantException>(() => service.Ask(new SapDiscovery(), 0, request, CancellationToken.None));
        Assert.That(error.StatusCode, Is.EqualTo(400));
        Assert.That(handler.Requests, Is.Empty);
    }

    [Test]
    public void InvalidFieldFocusIsRejectedBeforeCallingOpenAI()
    {
        var handler = new AssistantHandler();
        var service = new SapAssistantService(new HttpClient(handler) { BaseAddress = new Uri("https://api.openai.com/v1/") },
            Options.Create(new SapAssistantOptions { Enabled = true, ApiKey = "test-key" }), NullLogger<SapAssistantService>.Instance,
            new MemoryCache(new MemoryCacheOptions()));
        var error = Assert.ThrowsAsync<SapAssistantException>(() => service.Ask(new SapDiscovery(), 0,
            new SapAssistantRequest { Message = "Help with this", Mode = "field", FieldId = "finance.tax" }, CancellationToken.None));
        Assert.That(error.StatusCode, Is.EqualTo(400));
        Assert.That(handler.Requests, Is.Empty);
    }

    [Test]
    public async Task ServiceEnforcesDailyWorkspaceLimitBeforeAnotherApiCall()
    {
        var handler = new AssistantHandler();
        var service = new SapAssistantService(new HttpClient(handler) { BaseAddress = new Uri("https://api.openai.com/v1/") },
            Options.Create(new SapAssistantOptions { Enabled = true, ApiKey = "test-key", DailyWorkspaceRequestLimit = 1 }),
            NullLogger<SapAssistantService>.Instance, new MemoryCache(new MemoryCacheOptions()));
        var document = new SapDiscovery();
        await service.Ask(document, 0, new SapAssistantRequest { Message = "Review this section" }, CancellationToken.None);
        var error = Assert.ThrowsAsync<SapAssistantException>(() => service.Ask(document, 0,
            new SapAssistantRequest { Message = "Review it again" }, CancellationToken.None));
        Assert.That(error.StatusCode, Is.EqualTo(429));
        Assert.That(handler.Requests, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task ServiceRetriesOneTransientUpstreamFailure()
    {
        var handler = new TransientHandler();
        var service = new SapAssistantService(new HttpClient(handler) { BaseAddress = new Uri("https://api.openai.com/v1/") },
            Options.Create(new SapAssistantOptions { Enabled = true, ApiKey = "test-key" }), NullLogger<SapAssistantService>.Instance,
            new MemoryCache(new MemoryCacheOptions()));
        var reply = await service.Ask(new SapDiscovery(), 0, new SapAssistantRequest { Message = "Review this section" }, CancellationToken.None);
        Assert.That(reply.Answer, Is.Not.Empty);
        Assert.That(handler.CallCount, Is.EqualTo(3));
    }

    static int Count(string value, string part) => (value.Length - value.Replace(part, "").Length) / part.Length;

    sealed class AssistantHandler : HttpMessageHandler
    {
        public List<string> Requests { get; } = new();
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(await request.Content.ReadAsStringAsync(cancellationToken));
            var json = request.RequestUri.AbsolutePath.EndsWith("/moderations", StringComparison.Ordinal)
                ? "{\"results\":[{\"flagged\":false}]}"
                : "{\"status\":\"completed\",\"usage\":{\"input_tokens\":100,\"output_tokens\":50},\"output\":[{\"content\":[{\"type\":\"output_text\",\"text\":\"{\\\"answer\\\":\\\"Confirm the release with the SAP owner.\\\",\\\"findings\\\":[],\\\"followUpQuestions\\\":[],\\\"suggestions\\\":[{\\\"fieldId\\\":\\\"sap.release\\\",\\\"suggestedValue\\\":\\\"2023 FPS02 - confirm with owner\\\",\\\"rationale\\\":\\\"The release controls available interfaces.\\\"}],\\\"warnings\\\":[]}\"}]}]}";
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json, Encoding.UTF8, "application/json") };
        }
    }

    sealed class TransientHandler : HttpMessageHandler
    {
        public int CallCount { get; private set; }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CallCount++;
            if (CallCount == 1) return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError));
            var json = request.RequestUri.AbsolutePath.EndsWith("/moderations", StringComparison.Ordinal)
                ? "{\"results\":[{\"flagged\":false}]}"
                : "{\"status\":\"completed\",\"output\":[{\"content\":[{\"type\":\"output_text\",\"text\":\"{\\\"answer\\\":\\\"Review complete.\\\",\\\"findings\\\":[],\\\"followUpQuestions\\\":[],\\\"suggestions\\\":[],\\\"warnings\\\":[]}\"}]}]}";
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json, Encoding.UTF8, "application/json") });
        }
    }
}
