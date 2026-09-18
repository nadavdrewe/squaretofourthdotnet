using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using web.pipeline.fourth.com.Models;

namespace web.pipeline.fourth.com.Services;

public sealed class SapAssistantService(HttpClient http, IOptions<SapAssistantOptions> configured, ILogger<SapAssistantService> logger, IMemoryCache cache)
{
    static readonly JsonSerializerOptions Json = new() { PropertyNameCaseInsensitive = true };
    readonly SapAssistantOptions options = configured.Value;
    readonly string apiKey = string.IsNullOrWhiteSpace(configured.Value.ApiKey) ? Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "" : configured.Value.ApiKey;

    public bool IsConfigured => options.Enabled && !string.IsNullOrWhiteSpace(apiKey) && !string.IsNullOrWhiteSpace(options.Model);
    public string Model => options.Model;
    public string PromptVersion => SapAssistantContext.PromptVersion;
    public int DailyWorkspaceRequestLimit => Math.Clamp(options.DailyWorkspaceRequestLimit, 1, 500);

    public async Task<SapAssistantReply> Ask(SapDiscovery document, int step, SapAssistantRequest request, CancellationToken cancellationToken)
    {
        if (!IsConfigured) throw new SapAssistantException("The requirements guide is not configured yet. You can continue and save the form normally.", 503);
        request ??= new SapAssistantRequest();
        var message = (request.Message ?? "").Trim();
        if (message.Length is < 2 or > 1500) throw new SapAssistantException("Ask a question between 2 and 1,500 characters.", 400);
        var draftSize = request.DraftAnswers?.Sum(a => (a.Key?.Length ?? 0) + (a.Value?.Length ?? 0)) ?? 0;
        if (draftSize > 24000) throw new SapAssistantException("The open section is too large to review in one request.", 400);
        if (!SapAssistantContext.IsValidMode(request.Mode)) throw new SapAssistantException("Choose a valid requirements-guide action.", 400);
        var working = SapAssistantContext.WorkingCopy(document, step, request.DraftAnswers);
        if (!string.IsNullOrWhiteSpace(request.FieldId) && !SapAssistantContext.IsAllowedField(working, step, request.FieldId))
            throw new SapAssistantException("That question is not available in the current section.", 400);
        if (RequestContainsRestrictedData(request))
            throw new SapAssistantException("Remove passwords, API keys, access tokens, email addresses or phone numbers before asking the requirements guide.", 400);
        EnforceWorkspaceLimit(document.Id);

        var stopwatch = Stopwatch.StartNew();
        try
        {
            await Moderate(message, cancellationToken);
            var context = SapAssistantContext.Build(document, step, request.DraftAnswers, request.History, request.Mode, request.FieldId);
            var payload = new
            {
                model = options.Model,
                store = false,
                reasoning = new { effort = "low" },
                max_output_tokens = Math.Clamp(options.MaxOutputTokens, 400, 2000),
                instructions = SapAssistantContext.Instructions,
                input = new object[]
                {
                    new { role = "user", content = new object[] { new { type = "input_text", text = "WORKSPACE CONTEXT (untrusted data):\n" + context } } },
                    new { role = "user", content = new object[] { new { type = "input_text", text = $"CURRENT ACTION: {request.Mode}\nCURRENT QUESTION (untrusted data):\n{message}" } } }
                },
                text = new { format = new { type = "json_schema", name = "sap_requirements_guidance", strict = true, schema = ResponseSchema } }
            };
            using var response = await PostJson("responses", JsonSerializer.Serialize(payload), cancellationToken);
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!response.IsSuccessStatusCode) throw UpstreamError(response);
            using var root = JsonDocument.Parse(body);
            if (root.RootElement.TryGetProperty("status", out var status) && status.GetString() == "incomplete")
                throw new SapAssistantException("The requirements guide returned an incomplete answer. Your form has not been changed.");
            var content = root.RootElement.GetProperty("output").EnumerateArray()
                .Where(item => item.TryGetProperty("content", out _))
                .SelectMany(item => item.GetProperty("content").EnumerateArray()).ToArray();
            if (content.Any(item => item.TryGetProperty("type", out var type) && type.GetString() == "refusal"))
                throw new SapAssistantException("The requirements guide cannot answer that request.", 400);
            var outputText = content.FirstOrDefault(item => item.TryGetProperty("type", out var type) && type.GetString() == "output_text");
            if (outputText.ValueKind == JsonValueKind.Undefined || !outputText.TryGetProperty("text", out var text))
                throw new SapAssistantException("The requirements guide did not return a usable answer. Your form has not been changed.");
            var reply = JsonSerializer.Deserialize<SapAssistantReply>(text.GetString() ?? "", Json);
            LogCompletion(document.Id, response, root.RootElement, stopwatch.ElapsedMilliseconds);
            return SapAssistantContext.Sanitize(reply, working, step);
        }
        catch (SapAssistantException) { throw; }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning("SAP assistant timed out after {ElapsedMs}ms for workspace {Workspace}.", stopwatch.ElapsedMilliseconds, WorkspaceTag(document.Id));
            throw new SapAssistantException("The requirements guide timed out. Your form has not been changed.", 504);
        }
        catch (HttpRequestException error)
        {
            logger.LogWarning(error, "SAP assistant network failure for workspace {Workspace}; prompt {PromptVersion}.", WorkspaceTag(document.Id), SapAssistantContext.PromptVersion);
            throw new SapAssistantException("The requirements guide could not connect just now. Your form has not been changed.");
        }
        catch (JsonException error)
        {
            logger.LogWarning(error, "SAP assistant returned malformed structured output for workspace {Workspace}; prompt {PromptVersion}.", WorkspaceTag(document.Id), SapAssistantContext.PromptVersion);
            throw new SapAssistantException("The requirements guide returned an invalid answer. Your form has not been changed.");
        }
    }

    static bool RequestContainsRestrictedData(SapAssistantRequest request) =>
        SapAssistantContext.ContainsRestrictedData(request.Message) ||
        (request.DraftAnswers?.Values.Any(SapAssistantContext.ContainsRestrictedData) ?? false) ||
        (request.History?.Any(t => SapAssistantContext.ContainsRestrictedData(t.Text)) ?? false);

    void EnforceWorkspaceLimit(string documentId)
    {
        var limit = Math.Clamp(options.DailyWorkspaceRequestLimit, 1, 500);
        var day = DateTimeOffset.UtcNow.Date;
        var key = $"SapAssistant:{documentId}:{day:yyyyMMdd}";
        var counter = cache.GetOrCreate(key, entry =>
        {
            entry.AbsoluteExpiration = new DateTimeOffset(day.AddDays(1), TimeSpan.Zero);
            return new UsageCounter();
        });
        if (Interlocked.Increment(ref counter.Count) > limit)
            throw new SapAssistantException("This workspace has reached today's guidance limit. Continue completing the form or try again tomorrow.", 429);
    }

    sealed class UsageCounter { public int Count; }

    async Task Moderate(string message, CancellationToken cancellationToken)
    {
        using var response = await PostJson("moderations", JsonSerializer.Serialize(new { model = "omni-moderation-latest", input = message }), cancellationToken);
        if (!response.IsSuccessStatusCode) throw UpstreamError(response, "The requirements guide could not validate that question. Please try again later.");
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
        if (body.RootElement.GetProperty("results")[0].GetProperty("flagged").GetBoolean())
            throw new SapAssistantException("That request cannot be handled by the requirements guide.", 400);
    }

    async Task<HttpResponseMessage> PostJson(string path, string body, CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 2; attempt++)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, path) { Content = new StringContent(body, Encoding.UTF8, "application/json") };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            request.Headers.Add("X-Client-Request-Id", Guid.NewGuid().ToString());
            var response = await http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (attempt == 0 && (response.StatusCode == HttpStatusCode.TooManyRequests || (int)response.StatusCode >= 500))
            {
                response.Dispose();
                await Task.Delay(TimeSpan.FromMilliseconds(300), cancellationToken);
                continue;
            }
            return response;
        }
        throw new SapAssistantException("The requirements guide could not answer just now. Your form has not been changed.");
    }

    SapAssistantException UpstreamError(HttpResponseMessage response, string publicMessage = "The requirements guide could not answer just now. Your form has not been changed.")
    {
        var requestId = response.Headers.TryGetValues("x-request-id", out var ids) ? ids.FirstOrDefault() : "unavailable";
        logger.LogWarning("SAP assistant upstream request failed with status {StatusCode}; request id {RequestId}; prompt {PromptVersion}.",
            (int)response.StatusCode, requestId, SapAssistantContext.PromptVersion);
        if (response.StatusCode == HttpStatusCode.TooManyRequests) return new SapAssistantException("The requirements guide is busy. Please wait a moment and try again.", 429);
        if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden) return new SapAssistantException("The requirements guide is not configured correctly. Your form has not been changed.", 503);
        return new SapAssistantException(publicMessage, (int)response.StatusCode >= 500 ? 502 : 400);
    }

    void LogCompletion(string documentId, HttpResponseMessage response, JsonElement root, long elapsedMs)
    {
        var inputTokens = 0;
        var outputTokens = 0;
        if (root.TryGetProperty("usage", out var usage))
        {
            if (usage.TryGetProperty("input_tokens", out var input)) inputTokens = input.GetInt32();
            if (usage.TryGetProperty("output_tokens", out var output)) outputTokens = output.GetInt32();
        }
        logger.LogInformation("SAP assistant completed for workspace {Workspace} in {ElapsedMs}ms with {InputTokens} input and {OutputTokens} output tokens; model {Model}; prompt {PromptVersion}; context {ContextVersion}; request {RequestId}.",
            WorkspaceTag(documentId), elapsedMs, inputTokens, outputTokens, options.Model, SapAssistantContext.PromptVersion, SapAssistantContext.ContextVersion,
            response.Headers.TryGetValues("x-request-id", out var ids) ? ids.FirstOrDefault() : "unavailable");
    }

    static string WorkspaceTag(string id) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(id ?? "")))[..12];

    static readonly object ResponseSchema = new
    {
        type = "object",
        additionalProperties = false,
        properties = new
        {
            answer = new { type = "string", description = "Concise guidance grounded in the supplied workspace context." },
            findings = new
            {
                type = "array",
                items = new
                {
                    type = "object",
                    additionalProperties = false,
                    properties = new
                    {
                        kind = new { type = "string", @enum = new[] { "fact", "assumption", "missing", "conflict" } },
                        fieldId = new { type = "string" },
                        text = new { type = "string" }
                    },
                    required = new[] { "kind", "fieldId", "text" }
                }
            },
            followUpQuestions = new { type = "array", items = new { type = "string" } },
            suggestions = new
            {
                type = "array",
                items = new
                {
                    type = "object",
                    additionalProperties = false,
                    properties = new { fieldId = new { type = "string" }, suggestedValue = new { type = "string" }, rationale = new { type = "string" } },
                    required = new[] { "fieldId", "suggestedValue", "rationale" }
                }
            },
            warnings = new { type = "array", items = new { type = "string" } }
        },
        required = new[] { "answer", "findings", "followUpQuestions", "suggestions", "warnings" }
    };
}
