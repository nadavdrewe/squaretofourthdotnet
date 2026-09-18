using System.Linq;
using NUnit.Framework;
using web.pipeline.fourth.com.Models;

namespace tests.domain.pipelines.fourth.com;

public sealed class SapDiscoverySchemaTests
{
    [Test]
    public void OnlyRelevantFollowUpQuestionsApply()
    {
        var d = new SapDiscovery();
        d.Answers["inventory.direction"] = "No quantity synchronisation";
        d.Answers["posting.model"] = "Aggregated inventory and finance";
        d.Answers["finance.payouts"] = "Existing finance process handles settlement";
        var ids = SapDiscoverySchema.Applicable(d).Select(q => q.Id).ToArray();
        Assert.That(ids, Does.Not.Contain("inventory.frequency"));
        Assert.That(ids, Does.Not.Contain("inventory.quantity"));
        Assert.That(ids, Does.Not.Contain("posting.recovery"));
        Assert.That(ids, Does.Not.Contain("finance.clearing"));
        Assert.That(ids, Does.Contain("finance.settlementOwner"));
        d.Answers["inventory.direction"] = "SAP to Square";
        Assert.That(SapDiscoverySchema.Applicable(d).Select(q => q.Id), Does.Contain("inventory.quantity"));

        d.Answers["detail.customer"] = "Generic cash customer";
        Assert.That(SapDiscoverySchema.Applicable(d).Select(q => q.Id), Does.Not.Contain("detail.customerRule"));
        d.Answers["detail.customer"] = "Identified Business Partner";
        Assert.That(SapDiscoverySchema.Applicable(d).Select(q => q.Id), Does.Contain("detail.customerRule"));

        d.Answers["mapping.actions"] = "None";
        Assert.That(SapDiscoverySchema.Applicable(d).Select(q => q.Id), Does.Not.Contain("mapping.actionOwner"));
        Assert.That(SapDiscoverySchema.Applicable(d).Select(q => q.Id), Does.Not.Contain("mapping.due"));
        d.Answers["mapping.actions"] = "Confirm finance mappings";
        Assert.That(SapDiscoverySchema.Applicable(d).Select(q => q.Id), Does.Contain("mapping.actionOwner"));
        Assert.That(SapDiscoverySchema.Applicable(d).Select(q => q.Id), Does.Contain("mapping.due"));
    }

    [Test]
    public void ResumePositionSurvivesSerializationAndLegacyDocumentsDefaultToStart()
    {
        var d = new SapDiscovery { LastStep = 4 };
        var json = System.Text.Json.JsonSerializer.Serialize(d);
        Assert.That(System.Text.Json.JsonSerializer.Deserialize<SapDiscovery>(json).LastStep, Is.EqualTo(4));
        Assert.That(System.Text.Json.JsonSerializer.Deserialize<SapDiscovery>("{}").LastStep, Is.Zero);
    }

    [Test]
    [Platform("Win")]
    public void PdfExportPaginatesLongAnswersAndProducesAReadableDocument()
    {
        var d = new SapDiscovery { Company = "PDF verification / Example restaurant", Contact = "SAP owner", Email = "test@example.invalid" };
        foreach (var q in SapDiscoverySchema.Phases.SelectMany(p => p.Questions))
            d.Answers[q.Id] = q.Options?.FirstOrDefault() ?? "Requirement reviewed with the responsible owner.";
        d.Answers["mapping.products"] = string.Join("\n", Enumerable.Range(1, 100).Select(i => "SKU-" + i + " | MATERIAL-" + i + " | EA | approved product mapping"));
        d.Answers["landscape.evidence"] = "https://example.invalid/" + new string('a', 400);
        var bytes = web.pipeline.fourth.com.Services.SapRequirementsPdf.Generate(d, System.Array.Empty<SapRevision>());
        Assert.That(System.Text.Encoding.ASCII.GetString(bytes, 0, 5), Is.EqualTo("%PDF-"));
        using var stream = new System.IO.MemoryStream(bytes);
        using var pdf = PdfSharp.Pdf.IO.PdfReader.Open(stream, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
        Assert.That(pdf.PageCount, Is.GreaterThanOrEqualTo(8));
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(TestContext.CurrentContext.WorkDirectory, "sap-requirements-verification.pdf"), bytes);
    }
    [Test]
    public void FieldIdentifiersAreUniqueAndConditionsReferenceRealChoices()
    {
        var fields = SapDiscoverySchema.Phases.SelectMany(p => p.Questions).ToArray();
        Assert.That(fields.Select(q => q.Id).Distinct().Count(), Is.EqualTo(fields.Length));
        foreach (var q in fields.Where(q => q.When != null))
        {
            var dependency = fields.Single(f => f.Id == q.When);
            if (dependency.Options != null)
                Assert.That(dependency.Options, Does.Contain(q.EqualsValue));
            else
                Assert.That(q.NotEquals && q.EqualsValue == "None", Is.True,
                    $"Free-text condition on {q.Id} must use the supported non-empty/non-None rule.");
        }
    }

    [Test]
    public void QuestionGroupsCoverEveryFieldExactlyOnce()
    {
        Assert.That(SapDiscoverySchema.Version, Is.EqualTo(3));
        foreach (var phase in SapDiscoverySchema.Phases)
        {
            var grouped = SapDiscoverySchema.GroupsFor(phase).SelectMany(group => group.FieldIds).ToArray();
            Assert.That(grouped.Distinct().Count(), Is.EqualTo(grouped.Length), phase.Id + " contains a field in more than one group.");
            Assert.That(grouped, Is.EquivalentTo(phase.Questions.Select(question => question.Id)), phase.Id + " group coverage is incomplete.");
        }
    }
    [Test]
    public void DraftAllowsMissingAnswersButSubmissionDoesNot()
    {
        Assert.That(SapDiscoverySchema.Validate(new(), false), Is.Empty);
        Assert.That(SapDiscoverySchema.Validate(new(), true), Is.Not.Empty);
    }
    [TestCase("posting.invoices", "Yes")]
    [TestCase("finance.documents", "Receipt-level billing")]
    [TestCase("detail.level", "Receipt line")]
    public void AggregatePostingRejectsReceiptRequirements(string field, string value)
    {
        var d = new SapDiscovery();
        d.Answers["posting.model"] = "Aggregated inventory and finance";
        d.Answers[field] = value;
        Assert.That(SapDiscoverySchema.Validate(d, false), Is.Empty, "Drafts may contain decisions still being reconciled.");
        Assert.That(SapDiscoverySchema.Validate(d, true), Has.Some.Contains("conflicts"));
    }
    [Test]
    public void HiddenFieldsAreNotRequiredAndOpenActionsNeedOwners()
    {
        var d = new SapDiscovery();
        d.Answers["inventory.owner"] = "SAP";
        Assert.That(SapDiscoverySchema.Applicable(d).Select(q => q.Id), Does.Not.Contain("inventory.boundary"));
        d.Answers["inventory.owner"] = "Split by location";
        Assert.That(SapDiscoverySchema.Applicable(d).Select(q => q.Id), Does.Contain("inventory.boundary"));
        d.Answers["inventory.actions"] = "Check stock owner";
        Assert.That(SapDiscoverySchema.Validate(d, true), Has.Some.Contains("open actions need an owner"));
    }
    [Test]
    public void InvalidOptionsAndNegativeVolumesAreRejected()
    {
        var d = new SapDiscovery();
        d.Answers["sap.product"] = "invented";
        d.Answers["volume.daily"] = "-1";
        Assert.That(SapDiscoverySchema.Validate(d, false), Has.Count.EqualTo(2));
    }
}
