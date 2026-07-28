using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using domain.pipeline.fourth.com.Models;
using domain.pipeline.fourth.com.Services.Square.Oauth;
using web.pipeline.fourth.com.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Square;
using web.pipeline.fourth.com.Models;

namespace web.pipeline.fourth.com.Controllers
{
    [AllowAnonymous]
    [EnableRateLimiting("square-oauth")]
    [Route("oauthredirect")]
    [ApiController]
    public class OauthRedirectController : ControllerBase
    {
        private readonly FourthPipelineContext _context;
        private readonly SquareOAuthTokenService _squareOAuthTokenService;
        private readonly SquareOAuthConfigurationService _squareOAuthConfigurationService;

        public OauthRedirectController(
            FourthPipelineContext context,
            SquareOAuthTokenService squareOAuthTokenService,
            SquareOAuthConfigurationService squareOAuthConfigurationService)
        {
            _squareOAuthTokenService = squareOAuthTokenService;
            _context = context;
            _squareOAuthConfigurationService = squareOAuthConfigurationService;
        }

        [HttpGet]
        [Route("accept")]
        public async Task<IActionResult> Accept(string code, string response_type, string state, string error)
        {
            if (!string.IsNullOrEmpty(error))
            {
                return BadRequest("Square authorization was cancelled or failed. Start the authorization again.");
            }

            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(state))
            {
                return BadRequest("Square did not return an authorization code and state parameter.");
            }

            var parts = state.Split('_', 2);
            if (parts.Length != 2 || !int.TryParse(parts[0], out var credentialId))
            {
                return BadRequest("Invalid state parameter.");
            }

            var credential = await _context.CredentialsPool.FirstOrDefaultAsync(x => x.Id == credentialId);
            if (credential == null)
            {
                return BadRequest("OAuth request was not found.");
            }

            var pendingAuthorization = SquareOAuthPendingAuthorization.FromStoredValue(credential.SupplimentalData1);
            if (pendingAuthorization == null || !NonceMatches(pendingAuthorization.Nonce, parts[1]))
            {
                return BadRequest("State validation failed. Start the Square authorization again.");
            }

            if (pendingAuthorization.CreatedAtUtc != DateTime.MinValue &&
                pendingAuthorization.CreatedAtUtc < DateTime.UtcNow.AddMinutes(-10))
            {
                return BadRequest("The Square authorization request expired. Start it again.");
            }

            var environment = SquareOAuthEnvironment.Normalize(pendingAuthorization.Environment);
            SquareOAuthEnvironmentOptions settings;
            if (pendingAuthorization.SquareOAuthApplicationId is int applicationId)
            {
                var application = await _squareOAuthConfigurationService.GetApplicationAsync(applicationId, requireActive: false);
                if (!_squareOAuthConfigurationService.TryValidate(application, out settings, out _))
                {
                    return StatusCode(500, "The selected Square OAuth application is no longer available.");
                }
                environment = SquareOAuthEnvironment.Normalize(application.Environment);
            }
            else if (!_squareOAuthConfigurationService.TryValidate(environment, out settings, out _))
            {
                return StatusCode(500, "Square OAuth is not configured on this server.");
            }

            // Consume state before exchanging Square's one-time authorization code.
            credential.SupplimentalData1 = null;
            await _context.SaveChangesAsync();

            ObtainTokenResponse result;
            try
            {
                result = await _squareOAuthTokenService.GetSquareOauthAccessToken(
                    settings.ClientId,
                    settings.ClientSecret,
                    code,
                    settings.RedirectUri,
                    environment);

                var verificationClient = new SquareClient(result.AccessToken, new ClientOptions
                {
                    BaseUrl = SquareOAuthEnvironment.GetBaseUrl(environment)
                });
                await verificationClient.Locations.ListAsync();
            }
            catch (InvalidOperationException)
            {
                return StatusCode(502, "Square rejected the authorization code. Start the authorization again.");
            }
            catch (SquareException)
            {
                return StatusCode(502, "Square issued a token, but the connection test failed. Start the authorization again.");
            }

            var brand = await _context.Brands
                .Include(x => x.BrandCredentials)
                .FirstOrDefaultAsync(x => x.Id == credential.BrandId);
            if (brand == null)
            {
                return BadRequest("Brand not found for this OAuth request.");
            }

            // The sales worker selects one active Square credential per brand. Keep that selection deterministic.
            foreach (var existingCredential in brand.BrandCredentials.Where(x =>
                         x.CredentialType == shared.pipeline.fouth.com.Enums.CredentialTypes.SquareApi &&
                         x.Id != credential.Id))
            {
                existingCredential.Active = false;
            }

            credential.ClientId = settings.ClientId;
            credential.ClientSecret = null;
            credential.BaseEndpoint = SquareOAuthEnvironment.GetBaseUrl(environment);
            credential.Active = true;
            SquareOAuthTokenService.ApplyTokenResponse(
                credential,
                result,
                requireRefreshToken: true,
                environment: environment,
                scopes: pendingAuthorization.Scopes,
                squareOAuthApplicationId: pendingAuthorization.SquareOAuthApplicationId);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "ClientSetup", new { oauth = "connected", brandId = brand.Id });
        }

        private static bool NonceMatches(string expectedNonce, string receivedNonce)
        {
            if (string.IsNullOrWhiteSpace(expectedNonce) || string.IsNullOrWhiteSpace(receivedNonce))
            {
                return false;
            }

            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(expectedNonce),
                Encoding.UTF8.GetBytes(receivedNonce));
        }
    }
}
