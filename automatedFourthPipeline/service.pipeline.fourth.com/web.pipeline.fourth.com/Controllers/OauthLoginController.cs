using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using data.pipeline.fourth.com.Models.Credentials;
using domain.pipeline.fourth.com.Models;
using domain.pipeline.fourth.com.Services.Square.Oauth;
using web.pipeline.fourth.com.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace web.pipeline.fourth.com.Controllers
{
    [Authorize]
    [EnableRateLimiting("square-oauth")]
    public class OauthLoginController : Controller
    {
        private static readonly string[] RequiredScopes =
        {
            "MERCHANT_PROFILE_READ", "ORDERS_READ", "PAYMENTS_READ", "ITEMS_READ", "EMPLOYEES_READ", "TIMECARDS_READ"
        };

        private readonly FourthPipelineContext _context;
        private readonly SquareOAuthConfigurationService _squareOAuthConfigurationService;

        public OauthLoginController(FourthPipelineContext context, SquareOAuthConfigurationService squareOAuthConfigurationService)
        {
            _context = context;
            _squareOAuthConfigurationService = squareOAuthConfigurationService;
        }

        [HttpGet]
        public async Task<IActionResult> Authorize(int? brandId)
        {
            ViewData["brandList"] = new SelectList(await _context.Brands.OrderBy(x => x.Name).ToListAsync(), "Id", "Name", brandId);
            var applications = await _context.SquareOAuthApplications
                .Where(x => x.Active)
                .OrderBy(x => x.Environment)
                .ThenBy(x => x.Name)
                .Select(x => new { x.Id, DisplayName = x.Name + " - " + x.Environment })
                .ToListAsync();
            ViewData["squareApplicationList"] = new SelectList(applications, "Id", "DisplayName");
            ViewData["hasSquareApplications"] = applications.Count > 0;
            return View("TryCiprianiAuth");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Authorize(int brandId, int squareOAuthApplicationId)
        {
            var application = await _squareOAuthConfigurationService.GetApplicationAsync(squareOAuthApplicationId);
            if (!_squareOAuthConfigurationService.TryValidate(application, out var settings, out var configurationError))
            {
                TempData["Error"] = configurationError;
                return RedirectToAction(nameof(Authorize));
            }
            var normalizedEnvironment = SquareOAuthEnvironment.Normalize(application.Environment);

            var brand = _context.Brands.FirstOrDefault(x => x.Id == brandId);
            if (brand == null)
            {
                TempData["Error"] = "Selected brand not found.";
                return RedirectToAction(nameof(Authorize));
            }

            var nonce = Guid.NewGuid().ToString("N");
            var pendingAuthorization = new SquareOAuthPendingAuthorization
            {
                Nonce = nonce,
                Environment = normalizedEnvironment,
                Scopes = RequiredScopes,
                SquareOAuthApplicationId = application.Id,
                CreatedAtUtc = DateTime.UtcNow
            };

            var credential = new BaseCredential
            {
                Active = false,
                ClientId = settings.ClientId,
                ClientSecret = null,
                BaseEndpoint = SquareOAuthEnvironment.GetBaseUrl(normalizedEnvironment),
                BrandId = brand.Id,
                CredentialType = shared.pipeline.fouth.com.Enums.CredentialTypes.SquareApi,
                WhenCreatedUTC = DateTime.UtcNow,
                WhenUpdatedUTC = DateTime.UtcNow,
                SupplimentalData1 = JsonSerializer.Serialize(pendingAuthorization)
            };
            _context.CredentialsPool.Add(credential);
            await _context.SaveChangesAsync();

            var state = $"{credential.Id}_{nonce}";
            var query = QueryString.Create(new[]
            {
                new KeyValuePair<string, string>("client_id", settings.ClientId),
                new KeyValuePair<string, string>("redirect_uri", settings.RedirectUri),
                new KeyValuePair<string, string>("scope", string.Join(" ", RequiredScopes)),
                new KeyValuePair<string, string>("state", state)
            });
            if (normalizedEnvironment == SquareOAuthEnvironment.Production)
            {
                query = query.Add("session", "false");
            }

            return Redirect($"{SquareOAuthEnvironment.GetBaseUrl(normalizedEnvironment)}/oauth2/authorize{query}");
        }
    }
}
