using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using NUnit.Framework;

namespace tests.domain.pipelines.fourth.com;

public sealed class SapAssistantEvaluationCatalogTests
{
    [Test]
    public void EvaluationCatalogContainsFortyUniqueScenarios()
    {
        using var catalog = JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "sap-assistant-evaluation.json")));
        var scenarios = catalog.RootElement.EnumerateArray().ToArray();
        Assert.That(scenarios, Has.Length.EqualTo(40));
        Assert.That(scenarios.Select(s => s.GetProperty("id").GetString()).Distinct().ToArray(), Has.Length.EqualTo(40));
        Assert.That(scenarios.All(s => !string.IsNullOrWhiteSpace(s.GetProperty("prompt").GetString())), Is.True);
        Assert.That(scenarios.All(s => !string.IsNullOrWhiteSpace(s.GetProperty("must").GetString())), Is.True);
        Assert.That(scenarios.All(s => !string.IsNullOrWhiteSpace(s.GetProperty("mustNot").GetString())), Is.True);
    }
}
