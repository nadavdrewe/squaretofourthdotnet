using System;
using System.Threading.Tasks;
using data.pipeline.fourth.com.Models.Credentials;
using domain.pipeline.fourth.com.Models;
using domain.pipeline.fourth.com.Services.Square.Oauth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using web.pipeline.fourth.com.Models;

namespace web.pipeline.fourth.com.Services
{
    public sealed class SquareOAuthConfigurationService
    {
        private readonly SquareOAuthOptions _options;
        private readonly FourthPipelineContext _context;

        public SquareOAuthConfigurationService(
            IOptions<SquareOAuthOptions> options,
            FourthPipelineContext context)
        {
            _options = options.Value;
            _context = context;
        }

        public SquareOAuthEnvironmentOptions GetSettings(string environment)
        {
            var normalizedEnvironment = SquareOAuthEnvironment.Normalize(environment);
            var settings = normalizedEnvironment == SquareOAuthEnvironment.Sandbox
                ? _options.Sandbox
                : _options.Production;

            return settings;
        }

        public bool TryValidate(string environment, out SquareOAuthEnvironmentOptions settings, out string error)
        {
            settings = GetSettings(environment);
            if (string.IsNullOrWhiteSpace(settings.ClientId) ||
                string.IsNullOrWhiteSpace(settings.ClientSecret) ||
                string.IsNullOrWhiteSpace(settings.RedirectUri))
            {
                error = $"Square {SquareOAuthEnvironment.Normalize(environment)} OAuth is not configured. Add its client ID, client secret, and redirect URI first.";
                return false;
            }

            return TryValidateRedirectUri(settings, out error);
        }

        public async Task<SquareOAuthApplication> GetApplicationAsync(int id, bool requireActive = true)
        {
            return await _context.SquareOAuthApplications.FirstOrDefaultAsync(x =>
                x.Id == id && (!requireActive || x.Active));
        }

        public async Task<SquareOAuthApplication> GetApplicationForCredentialAsync(BaseCredential credential)
        {
            var metadata = SquareOAuthTokenMetadata.FromStoredValue(credential?.SupplimentalData2);
            if (metadata?.SquareOAuthApplicationId is int applicationId)
            {
                return await GetApplicationAsync(applicationId, requireActive: false);
            }

            if (string.IsNullOrWhiteSpace(credential?.ClientId))
            {
                return null;
            }

            var environment = metadata?.GetEnvironment(credential.BaseEndpoint)
                ?? SquareOAuthEnvironment.GetEnvironmentFromBaseUrl(credential.BaseEndpoint);
            return await _context.SquareOAuthApplications.FirstOrDefaultAsync(x =>
                x.Active && x.ApplicationId == credential.ClientId && x.Environment == environment);
        }

        public bool TryValidate(
            SquareOAuthApplication application,
            out SquareOAuthEnvironmentOptions settings,
            out string error)
        {
            if (application == null)
            {
                settings = null;
                error = "The selected Square OAuth application was not found or is inactive.";
                return false;
            }

            settings = new SquareOAuthEnvironmentOptions
            {
                ClientId = application.ApplicationId,
                ClientSecret = application.ClientSecret,
                RedirectUri = application.RedirectUri
            };

            if (string.IsNullOrWhiteSpace(application.Name) ||
                string.IsNullOrWhiteSpace(application.ApplicationId) ||
                string.IsNullOrWhiteSpace(application.ClientSecret) ||
                string.IsNullOrWhiteSpace(application.RedirectUri))
            {
                error = "The selected Square OAuth application is incomplete.";
                return false;
            }

            return TryValidateRedirectUri(settings, out error);
        }

        private static bool TryValidateRedirectUri(SquareOAuthEnvironmentOptions settings, out string error)
        {
            if (!Uri.TryCreate(settings.RedirectUri, UriKind.Absolute, out var redirectUri) ||
                redirectUri.Scheme != Uri.UriSchemeHttps ||
                !string.Equals(redirectUri.AbsolutePath.TrimEnd('/'), "/oauthredirect/accept", StringComparison.OrdinalIgnoreCase))
            {
                error = "Square OAuth redirect URI must be an HTTPS URL ending in /oauthredirect/accept.";
                return false;
            }

            error = null;
            return true;
        }
    }
}
