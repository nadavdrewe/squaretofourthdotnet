using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Fonts;
using web.pipeline.fourth.com.Models;

namespace web.pipeline.fourth.com.Services;

public static class SapRequirementsPdf
{
    // The application is hosted on Windows/IIS; PDFsharp embeds the resolved fonts.
    static SapRequirementsPdf() => GlobalFontSettings.UseWindowsFontsUnderWindows = true;

    public static byte[] Generate(SapDiscovery source, IReadOnlyList<SapRevision> history)
    {
        var document = new Document();
        document.Info.Title = "Square to SAP requirements - " + source.Company;
        document.Info.Author = "Square to SAP";
        var normal = document.Styles[StyleNames.Normal];
        normal.Font.Name = "Arial";
        normal.Font.Size = 10;
        normal.ParagraphFormat.SpaceAfter = 7;
        normal.ParagraphFormat.KeepTogether = false;
        var heading = document.Styles[StyleNames.Heading1];
        heading.Font.Size = 22;
        heading.Font.Color = Color.FromRgb(0, 83, 135);
        heading.ParagraphFormat.SpaceAfter = 16;
        var label = document.Styles.AddStyle("Question", StyleNames.Normal);
        label.Font.Bold = true;
        label.ParagraphFormat.SpaceBefore = 10;
        label.ParagraphFormat.SpaceAfter = 4;
        label.ParagraphFormat.KeepWithNext = true;

        var section = document.AddSection();
        section.PageSetup.PageFormat = PageFormat.A4;
        section.PageSetup.TopMargin = Unit.FromCentimeter(2);
        section.PageSetup.BottomMargin = Unit.FromCentimeter(2);
        section.PageSetup.LeftMargin = Unit.FromCentimeter(2);
        section.PageSetup.RightMargin = Unit.FromCentimeter(2);
        section.Headers.Primary.AddParagraph("SQUARE / SAP     |     CUSTOMER REQUIREMENTS").Format.Font.Size = 8;
        var footer = section.Footers.Primary.AddParagraph();
        footer.Format.Font.Size = 8;
        footer.AddText("Confidential requirements | v" + source.Revision + " | Page ");
        footer.AddPageField();
        footer.AddText(" of ");
        footer.AddNumPagesField();

        void Text(string value) => section.AddParagraph(WrapTokens(value ?? ""));
        section.AddParagraph("Integration requirements", StyleNames.Heading1);
        Text(source.Company);
        Text(source.Contact + " | " + source.Email);
        Text("Status: " + source.Status + " | Revision: " + source.Revision + " | Schema: " + SapDiscoverySchema.Version);
        Text("Reference: " + source.Id);
        Text("Last saved: " + source.UpdatedUtc.ToString("yyyy-MM-dd HH:mm 'UTC'") + " | Exported: " + DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm 'UTC'"));
        Text("This document contains saved answers only. It is a requirements brief for technical review, not executable SAP configuration or evidence that a connector is deployed. Customer editor names are self-declared.");
        var issues = SapDiscoverySchema.Validate(source, true);
        Text(issues.Count == 0 ? "Required-answer validation: complete." : "Items needing attention: " + issues.Count);
        foreach (var issue in issues) Text("- " + issue);
        foreach (var phase in SapDiscoverySchema.Phases)
        {
            section.AddPageBreak();
            section.AddParagraph(phase.Title, StyleNames.Heading1);
            Text(phase.Description);
            foreach (var question in phase.Questions.Where(q => SapDiscoverySchema.Applies(q, source)))
            {
                section.AddParagraph(question.Label, "Question");
                var answer = source.Answer(question.Id);
                Text(string.IsNullOrWhiteSpace(answer) ? "Not answered" : answer);
            }
        }
        section.AddPageBreak();
        section.AddParagraph("Revision log", StyleNames.Heading1);
        Text("The complete answer snapshots are retained in the portal and JSON export. This PDF records the current saved version.");
        foreach (var revision in history)
            Text("v" + revision.Revision + " | " + revision.Status + " | " + revision.At.ToString("yyyy-MM-dd HH:mm 'UTC'") + " | " + revision.Actor);
        var renderer = new PdfDocumentRenderer { Document = document };
        renderer.RenderDocument();
        using var output = new MemoryStream();
        renderer.PdfDocument.Save(output, false);
        return output.ToArray();
    }

    // Long identifiers/URLs must wrap rather than extend outside the page margins.
    static string WrapTokens(string text) => System.Text.RegularExpressions.Regex.Replace(text, @"\S{65,}",
        match => string.Join("\u200B", Enumerable.Range(0, (match.Length + 59) / 60)
            .Select(i => match.Value.Substring(i * 60, Math.Min(60, match.Length - i * 60)))));
}
