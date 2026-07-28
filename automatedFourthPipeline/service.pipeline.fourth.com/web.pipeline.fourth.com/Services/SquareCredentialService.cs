using System;
using System.Threading.Tasks;
using data.pipeline.fourth.com.Models.Credentials;
using domain.pipeline.fourth.com.Models;
using domain.pipeline.fourth.com.Services.Square.Oauth;
using Square;

namespace web.pipeline.fourth.com.Services
{
    public class SquareCredentialService
    {
        private readonly FourthPipelineContext _context;
        private readonly SquareOAuthTokenService _squareOAuthTokenService;
        private readonly SquareOAuthConfigurationService _squareOAuthConfigurationService;

        public SquareCredentialService(
            FourthPipelineContext context,
            SquareOAuthTokenService squareOAuthTokenService,
            SquareOAuthConfigurationService squareOAuthConfigurationService)
        {
            _context = context;
            _squareOAuthTokenService = squareOAuthTokenService;
            _squareOAuthConfigurationService = squareOAuthConfigurationService;
        }

        public async Task<string> GetAccessTokenAsync(BaseCredential credential)
        {
            if (credential == null)
            {
                throw new InvalidOperationException("Square credential not found.");
            }

            if (SquareOAuthTokenService.IsRefreshDue(credential, DateTime.UtcNow))
            {
                var metadata = SquareOAuthTokenMetadata.FromStoredValue(credential.SupplimentalData2);
                var environment = metadata?.GetEnvironment(credential.BaseEndpoint);
                var application = await _squareOAuthConfigurationService.GetApplicationForCredentialAsync(credential);
                var applicationId = metadata?.SquareOAuthApplicationId;
                if (applicationId.HasValue && application == null)
                {
                    throw new InvalidOperationException(
                        $"Square OAuth application {applicationId.Value} no longer exists.");
                }
                var isValid = application != null
                    ? _squareOAuthConfigurationService.TryValidate(application, out var settings, out var configurationError)
                    : _squareOAuthConfigurationService.TryValidate(environment, out settings, out configurationError);
                if (!isValid)
                {
                    throw new InvalidOperationException(configurationError);
                }

                var refreshResponse = await _squareOAuthTokenService.RefreshSquareToken(
                    settings.ClientId,
                    settings.ClientSecret,
                    credential.RefreshToken,
                    baseUrl: GetApiBaseUrl(credential));

                SquareOAuthTokenService.ApplyTokenResponse(credential, refreshResponse);
                _context.Update(credential);
                await _context.SaveChangesAsync();
            }

            if (string.IsNullOrWhiteSpace(credential.LatestAccessToken))
            {
                throw new InvalidOperationException("Square credential does not have a usable access token.");
            }

            return credential.LatestAccessToken;
        }

        public async Task<SquareClient> CreateClientAsync(BaseCredential credential)
        {
            var accessToken = await GetAccessTokenAsync(credential);
            return new SquareClient(accessToken, new ClientOptions { BaseUrl = GetApiBaseUrl(credential) });
        }

        public string GetApiBaseUrl(BaseCredential credential)
        {
            return SquareOAuthTokenService.GetCredentialBaseUrl(credential);
        }
    }
}
