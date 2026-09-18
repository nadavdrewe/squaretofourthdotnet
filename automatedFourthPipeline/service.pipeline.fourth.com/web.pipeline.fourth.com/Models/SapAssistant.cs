using System.Collections.Generic;

namespace web.pipeline.fourth.com.Models;

public sealed class SapAssistantOptions
{
    public bool Enabled { get; set; }
    public string ApiKey { get; set; } = "";
    public string Model { get; set; } = "gpt-5.6-terra";
    public int MaxOutputTokens { get; set; } = 1000;
    public int DailyWorkspaceRequestLimit { get; set; } = 40;
}

public sealed class SapAssistantRequest
{
    public string Message { get; set; } = "";
    public string Mode { get; set; } = "ask";
    public string FieldId { get; set; } = "";
    public Dictionary<string, string> DraftAnswers { get; set; } = new();
    public List<SapAssistantTurn> History { get; set; } = new();
}

public sealed class SapAssistantTurn
{
    public string Role { get; set; } = "";
    public string Text { get; set; } = "";
}

public sealed class SapAssistantReply
{
    public string Answer { get; set; } = "";
    public string PromptVersion { get; set; } = "";
    public string ContextVersion { get; set; } = "";
    public bool IsFallback { get; set; }
    public List<SapAssistantFinding> Findings { get; set; } = new();
    public List<string> FollowUpQuestions { get; set; } = new();
    public List<SapAssistantSuggestion> Suggestions { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}

public sealed class SapAssistantFinding
{
    public string Kind { get; set; } = "";
    public string FieldId { get; set; } = "";
    public string Text { get; set; } = "";
}

public sealed class SapAssistantSuggestion
{
    public string FieldId { get; set; } = "";
    public string SuggestedValue { get; set; } = "";
    public string Rationale { get; set; } = "";
}

public sealed class SapAssistantException(string publicMessage, int statusCode = 502) : System.Exception(publicMessage)
{
    public string PublicMessage { get; } = publicMessage;
    public int StatusCode { get; } = statusCode;
}
