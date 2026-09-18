using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using web.pipeline.fourth.com.Models;
using web.pipeline.fourth.com.Services;

namespace web.pipeline.fourth.com.Controllers;

[Route("sap")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class SapController(SapDiscoveryStore store, SapAssistantService assistant, IDataProtectionProvider protection) : Controller
{
    readonly IDataProtector protector = protection.CreateProtector("SapDiscovery.Access.v1");
    static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value ?? "")));
    bool CanRead(string id)
    {
        if (User.IsInRole("Administrator")) return true;
        try { return protector.Unprotect(Request.Cookies["SapDiscovery." + id] ?? "") == id; }
        catch (CryptographicException) { return false; }
    }
    void Grant(string id) => Response.Cookies.Append("SapDiscovery." + id, protector.Protect(id), new CookieOptions {
        HttpOnly = true, Secure = true, SameSite = SameSiteMode.Lax, MaxAge = TimeSpan.FromDays(30), Path = "/sap", IsEssential = true
    });
    [AllowAnonymous, HttpGet("")]
    public IActionResult Index() => View();
    [AllowAnonymous, HttpGet("requirements")]
    public IActionResult Start() => View();
    [AllowAnonymous, HttpPost("requirements"), ValidateAntiForgeryToken, EnableRateLimiting("public-onboarding")]
    public async Task<IActionResult> Create(string company, string contact, string email)
    {
        if (string.IsNullOrWhiteSpace(company) || company.Length > 200 || string.IsNullOrWhiteSpace(contact) || contact.Length > 200 ||
            string.IsNullOrWhiteSpace(email) || email.Length > 254 || !new EmailAddressAttribute().IsValid(email))
        { ModelState.AddModelError("", "Enter a company, contact name and valid email address."); return View("Start"); }
        var d = new SapDiscovery { Company = company.Trim(), Contact = contact.Trim(), Email = email.Trim() };
        var secret = Convert.ToHexString(RandomNumberGenerator.GetBytes(24));
        await store.Create(d, Hash(secret));
        Grant(d.Id);
        ViewData["ResumeCode"] = secret;
        return View("Created", d);
    }
    [AllowAnonymous, HttpPost("resume"), ValidateAntiForgeryToken, EnableRateLimiting("site-login")]
    public async Task<IActionResult> Resume(string reference, string code)
    {
        var id = Guid.TryParse((reference ?? "").Trim(), out var parsed) ? parsed.ToString("N") : "";
        var result = await store.Get(id.Length <= 32 ? id : "");
        if (result.Document == null || !CryptographicOperations.FixedTimeEquals(Encoding.ASCII.GetBytes(result.Hash), Encoding.ASCII.GetBytes(Hash((code ?? "").Trim().ToUpperInvariant()))))
        { ModelState.AddModelError("", "The reference or access code is incorrect."); return View("Start"); }
        Grant(id);
        return RedirectToAction(nameof(Edit), new { id, step = Math.Clamp(result.Document.LastStep, 0, SapDiscoverySchema.Phases.Length) });
    }
    [AllowAnonymous, HttpGet("requirements/{id}/{step:int=0}")]
    public async Task<IActionResult> Edit(string id, int step)
    {
        if (!CanRead(id)) return RedirectToAction(nameof(Start));
        var result = await store.Get(id);
        if (result.Document == null || step < 0 || step > SapDiscoverySchema.Phases.Length) return NotFound();
        ViewData["SapAssistantConfigured"] = assistant.IsConfigured;
        ViewData["SapAssistantModel"] = assistant.Model;
        return View(new SapEditor(result.Document, step, step == SapDiscoverySchema.Phases.Length ? await store.History(id) : null));
    }
    [AllowAnonymous, HttpPost("requirements/{id}/{step:int}"), ValidateAntiForgeryToken, RequestSizeLimit(200000)]
    public async Task<IActionResult> Save(string id, int step, int revision, string actor, string action, string assistantFields)
    {
        if (!CanRead(id)) return Forbid();
        var result = await store.Get(id);
        var d = result.Document;
        if (d == null || step < 0 || step > SapDiscoverySchema.Phases.Length) return NotFound();
        if (step < SapDiscoverySchema.Phases.Length ? action is not ("save" or "next") : action is not ("submit" or "approve" or "reopen")) return BadRequest("Invalid action for this section.");
        if (d.Revision != revision) return Conflict("Another collaborator saved a newer version. Reload the page before making changes; your older answers have not overwritten theirs.");
        if (string.IsNullOrWhiteSpace(actor) || actor.Length > 200) ModelState.AddModelError("", "Enter your name and role for the decision history.");
        if (step < SapDiscoverySchema.Phases.Length)
        {
            foreach (var q in SapDiscoverySchema.Phases[step].Questions)
                if (Request.Form.ContainsKey("answers[" + q.Id + "]"))
                    d.Answers[q.Id] = Request.Form["answers[" + q.Id + "]"].ToString().Trim();
            foreach (var q in SapDiscoverySchema.Phases.SelectMany(p => p.Questions).Where(q => !SapDiscoverySchema.Applies(q, d))) d.Answers.Remove(q.Id);
            d.Status = "Draft";
        }
        if (action == "approve" && !User.IsInRole("Administrator")) return Forbid();
        if (action == "approve" && d.Status != "Submitted") ModelState.AddModelError("", "Only a submitted questionnaire can be approved.");
        if (action == "approve")
        {
            foreach (var q in SapDiscoverySchema.Applicable(d).Where(q => q.Options != null))
                if (d.Answer(q.Id).Contains("confirm", StringComparison.OrdinalIgnoreCase) || d.Answer(q.Id).Contains("decided", StringComparison.OrdinalIgnoreCase) || d.Answer(q.Id).Contains("review", StringComparison.OrdinalIgnoreCase))
                    ModelState.AddModelError("", q.Label + ": resolve the provisional answer before approval.");
            foreach (var phase in SapDiscoverySchema.Phases)
                if (!d.Answer(phase.Id + ".actions").Equals("None", StringComparison.OrdinalIgnoreCase))
                    ModelState.AddModelError("", phase.Title + ": resolve open actions before approval.");
        }
        var errors = SapDiscoverySchema.Validate(d, action is "submit" or "approve");
        foreach (var error in errors) ModelState.AddModelError("", error);
        if (action == "submit" && Request.Form["confirm"] != "yes") ModelState.AddModelError("", "Confirm that the requirements are ready for review.");
        if (!ModelState.IsValid)
        {
            ViewData["SapAssistantConfigured"] = assistant.IsConfigured;
            ViewData["SapAssistantModel"] = assistant.Model;
            return View("Edit", new SapEditor(d, step, step == SapDiscoverySchema.Phases.Length ? await store.History(id) : null));
        }
        if (action == "submit") d.Status = "Submitted";
        if (action == "approve") d.Status = "Approved";
        if (action == "reopen") d.Status = "Draft";
        d.LastStep = action == "next" ? Math.Min(step + 1, SapDiscoverySchema.Phases.Length) : step;
        var attribution = User.IsInRole("Administrator") ? "Administrator: " + User.Identity.Name + " / " + actor : "Customer (self-declared): " + actor;
        if (step < SapDiscoverySchema.Phases.Length)
        {
            var allowed = SapDiscoverySchema.Phases[step].Questions.Select(q => q.Id).ToHashSet(StringComparer.Ordinal);
            var assisted = (assistantFields ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(allowed.Contains).Distinct(StringComparer.Ordinal).Take(6).ToArray();
            if (assisted.Length > 0) attribution += " | AI-assisted drafts accepted: " + string.Join(", ", assisted);
        }
        if (attribution.Length > 440) attribution = attribution[..440];
        if (!await store.Save(d, revision, attribution)) return Conflict("A newer version has been saved. Reload to continue.");
        TempData["Saved"] = "Saved at " + DateTimeOffset.UtcNow.ToString("HH:mm 'UTC'") + ". Status: " + d.Status + ".";
        return RedirectToAction(nameof(Edit), new { id, step = action == "next" ? Math.Min(step + 1, SapDiscoverySchema.Phases.Length) : step });
    }
    [Authorize(Roles = "Administrator"), HttpGet("admin")]
    public async Task<IActionResult> Admin()
    {
        ViewData["SapAssistantConfigured"] = assistant.IsConfigured;
        ViewData["SapAssistantModel"] = assistant.Model;
        ViewData["SapAssistantPromptVersion"] = assistant.PromptVersion;
        ViewData["SapAssistantDailyLimit"] = assistant.DailyWorkspaceRequestLimit;
        return View(await store.List());
    }
    [AllowAnonymous, HttpPost("requirements/{id}/{step:int}/assistant"), ValidateAntiForgeryToken, EnableRateLimiting("sap-assistant"), RequestSizeLimit(65536)]
    public async Task<IActionResult> Assistant(string id, int step, [FromBody] SapAssistantRequest request, CancellationToken cancellationToken)
    {
        if (!CanRead(id)) return Forbid();
        var result = await store.Get(id);
        if (result.Document == null || step < 0 || step > SapDiscoverySchema.Phases.Length) return NotFound();
        try
        {
            return Json(await assistant.Ask(result.Document, step, request, cancellationToken));
        }
        catch (SapAssistantException error)
        {
            if (error.StatusCode >= 500) return Json(SapAssistantContext.Fallback(result.Document, step, request));
            return StatusCode(error.StatusCode, new { error = error.PublicMessage });
        }
    }
    [AllowAnonymous, HttpGet("requirements/{id}/export/{format}")]
    public async Task<IActionResult> Export(string id, string format)
    {
        if (!CanRead(id)) return Forbid();
        var result = await store.Get(id);
        if (result.Document == null) return NotFound();
        var d = result.Document;
        if (format == "pdf") return File(SapRequirementsPdf.Generate(d, await store.History(id)), "application/pdf", "square-sap-requirements-" + id + ".pdf");
        if (format == "json") return File(JsonSerializer.SerializeToUtf8Bytes(new {
            schemaVersion = SapDiscoverySchema.Version, purpose = "Requirements for technical review; not executable SAP configuration", document = d,
            validation = SapDiscoverySchema.Validate(d, true), fields = SapDiscoverySchema.Applicable(d), history = await store.History(id)
        }, new JsonSerializerOptions { WriteIndented = true }), "application/json", "square-sap-requirements-" + id + ".json");
        if (format == "csv")
        {
            static string Cell(string s) { s ??= ""; if (s.TrimStart().StartsWith('=') || s.TrimStart().StartsWith('+') || s.TrimStart().StartsWith('-') || s.TrimStart().StartsWith('@') || s.StartsWith('\t') || s.StartsWith('\r')) s = "'" + s; return "\"" + s.Replace("\"", "\"\"") + "\""; }
            var rows = SapDiscoverySchema.Phases.SelectMany(p => p.Questions.Where(q => SapDiscoverySchema.Applies(q, d)).Select(q => string.Join(",", new[] { p.Title, q.Id, q.Label, d.Answer(q.Id), d.Answer(p.Id + ".decisionOwner"), d.Status }.Select(Cell))));
            return File(Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes("Section,Field ID,Question,Answer,Decision owner,Status\r\n" + string.Join("\r\n", rows))).ToArray(), "text/csv", "square-sap-decisions-" + id + ".csv");
        }
        return NotFound();
    }
}
