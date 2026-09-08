using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using data.pipeline.fourth.com.Models;
using domain.pipeline.fourth.com.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using shared.pipeline.fourth.com;
using web.pipeline.fourth.com.Models;
using web.pipeline.fourth.com.Services;

namespace web.pipeline.fourth.com.Controllers
{
    [AllowAnonymous]
    [EnableRateLimiting("public-onboarding")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class OnboardingController : Controller
    {
        private readonly FourthPipelineContext _context;
        private readonly UserManager<IdentityUser> _users;
        private readonly PublicOnboardingOptions _options;
        private readonly OnboardingEmailSender _email;
        private readonly ILogger<OnboardingController> _logger;
        public OnboardingController(FourthPipelineContext context, UserManager<IdentityUser> users, IOptions<PublicOnboardingOptions> options, OnboardingEmailSender email, ILogger<OnboardingController> logger)
        { _context = context; _users = users; _options = options.Value; _email = email; _logger = logger; }

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["OnboardingEnabled"] = _options.Enabled;
            return View(new PublicOnboardingInputModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ValidatePhrase(PublicOnboardingInputModel input)
        {
            ViewData["OnboardingEnabled"] = _options.Enabled;
            if (!_options.Enabled) return View("Index", input);
            var invite = await FindUsableInviteAsync(input.SetupKey);
            if (invite == null)
            {
                ModelState.AddModelError(nameof(input.SetupKey), "That invitation phrase is invalid, expired, revoked, or already used.");
                return View("Index", input);
            }
            input.ClientName = invite.CustomerName == "Unassigned customer" ? null : invite.CustomerName;
            input.Email = invite.Email;
            return View("Details", input);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(PublicOnboardingInputModel input)
        {
            input.Email = input.Email?.Trim().ToLowerInvariant();
            input.ClientName = input.ClientName?.Trim();
            if (!_options.Enabled) ModelState.AddModelError(nameof(input.SetupKey), "New client onboarding is not currently open.");
            if (string.IsNullOrWhiteSpace(input.ClientName) || input.ClientName.Length > 200) ModelState.AddModelError(nameof(input.ClientName), "Enter a client name.");
            if (!new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(input.Email)) ModelState.AddModelError(nameof(input.Email), "Enter a valid email address.");
            ViewData["OnboardingEnabled"] = _options.Enabled;
            if (!ModelState.IsValid) return View(input);
            var invite = await FindValidInviteAsync(input.SetupKey, input.Email);
            if (invite == null) { ModelState.AddModelError(nameof(input.SetupKey), "The setup key is invalid, expired, already used, or assigned to a different email."); return View(input); }
            if (await _users.FindByEmailAsync(input.Email) != null) { ModelState.AddModelError(nameof(input.Email), "That email already has access. Sign in instead."); return View(input); }

            var password = CreatePassword();
            var identity = new IdentityUser { UserName = input.Email, Email = input.Email, EmailConfirmed = true };
            var identityResult = await _users.CreateAsync(identity, password);
            if (!identityResult.Succeeded) { foreach (var e in identityResult.Errors) ModelState.AddModelError(string.Empty, e.Description); return View(input); }
            Brand brand = null;
            try
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                var now = DateTime.UtcNow;
                brand = new Brand { Name = input.ClientName, ContactEmail = input.Email, Active = true, WhenCreatedUTC = now, WhenUpdatedUTC = now, InitalSetupSytemType = shared.pipeline.fouth.com.Enums.CredentialTypes.SquareApi };
                brand.BrandIntegrations = new[] { new BrandIntegration { IntegrationType = IntegrationTypes.SquareToFourthPosSales, Active = true } }.ToList();
                brand.ClientAccesses = new[] { new ClientAccess { UserId = identity.Id, Email = input.Email, IsOwner = true, Active = true, WhenCreatedUTC = now, WhenUpdatedUTC = now } }.ToList();
                _context.Brands.Add(brand);
                await _context.SaveChangesAsync();
                invite.UseCount++;
                invite.LastUsedUTC = now;
                invite.WhenUpdatedUTC = now;
                invite.RedeemedBrandId = brand.Id;
                invite.RedeemedUserId = identity.Id;
                if (invite.CustomerName == "Unassigned customer") invite.CustomerName = input.ClientName;
                if (string.IsNullOrWhiteSpace(invite.Email)) invite.Email = input.Email;
                if (invite.UseCount >= invite.MaxUses) invite.Active = false;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                var sent = await _email.SendAsync(input.Email, input.ClientName, password);
                await SignInAsync(identity, input.Email);
                return View("Complete", new PublicOnboardingResultViewModel { BrandId = brand.Id, ClientName = input.ClientName, Email = input.Email, TemporaryPassword = password, EmailSent = sent });
            }
            catch
            {
                if (brand?.Id > 0)
                {
                    try { _context.Brands.Remove(brand); await _context.SaveChangesAsync(); }
                    catch (Exception cleanupError) { _logger.LogError(cleanupError, "Failed to roll back onboarding brand {BrandId}", brand.Id); }
                }
                var deleteResult = await _users.DeleteAsync(identity);
                if (!deleteResult.Succeeded) _logger.LogError("Failed to roll back onboarding identity {UserId}: {Errors}", identity.Id, string.Join("; ", deleteResult.Errors.Select(x => x.Description)));
                throw;
            }
        }

        private async Task<CustomerOnboardingInvite> FindValidInviteAsync(string value, string email)
        {
            var invite = await FindUsableInviteAsync(value);
            if (invite == null || (invite.Email != null && invite.Email != email)) return null;
            return invite;
        }

        private async Task<CustomerOnboardingInvite> FindUsableInviteAsync(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            var normalized = CustomerOnboardingInvitesController.NormalizePhrase(value);
            var hash = CustomerOnboardingInvitesController.Hash(normalized);
            var legacyHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
            var now = DateTime.UtcNow;
            return await _context.CustomerOnboardingInvites.SingleOrDefaultAsync(x =>
                (x.KeyHash == hash || x.KeyHash == legacyHash) && x.Active && x.UseCount < x.MaxUses &&
                (!x.ExpiresAtUTC.HasValue || x.ExpiresAtUTC > now));
        }
        private static string CreatePassword() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(18)).Replace("+", "A").Replace("/", "b").Replace("=", "9");
        private async Task SignInAsync(IdentityUser user, string email)
        {
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, user.Id), new Claim(ClaimTypes.Name, email), new Claim(ClaimTypes.Email, email) };
            await HttpContext.SignInAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(new ClaimsIdentity(claims, "SquareToFourth.Access")));
        }
    }
}
