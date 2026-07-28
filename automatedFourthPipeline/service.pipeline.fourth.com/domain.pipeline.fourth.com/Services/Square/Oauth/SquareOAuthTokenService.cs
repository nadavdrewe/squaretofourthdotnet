using data.pipeline.fourth.com.Models.Credentials;
using domain.pipeline.fourth.com.Models;
using Square;
using Square.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace domain.pipeline.fourth.com.Services.Square.Oauth
{
    /// <summary>
    /// Gets and refreshes token for Oauth services
    /// </summary>
    public class SquareOAuthTokenService
    {
        public static void ApplyTokenResponse(
            BaseCredential credential,
            ObtainTokenResponse result,
            bool requireRefreshToken = false,
            string environment = null,
            IEnumerable<string> scopes = null,
            int? squareOAuthApplicationId = null)
        {
            if (credential == null)
            {
                throw new InvalidOperationException("Square credential not found.");
            }

            if (result == null || string.IsNullOrWhiteSpace(result.AccessToken))
            {
                throw new InvalidOperationException("Square OAuth token response did not include an access token.");
            }

            if (requireRefreshToken && string.IsNullOrWhiteSpace(result.RefreshToken))
            {
                throw new InvalidOperationException("Square OAuth token response did not include a refresh token.");
            }

            credential.LatestAccessToken = result.AccessToken;
            if (!string.IsNullOrWhiteSpace(result.RefreshToken))
            {
                credential.RefreshToken = result.RefreshToken;
            }

            var existingMetadata = SquareOAuthTokenMetadata.FromStoredValue(credential.SupplimentalData2);
            credential.WhenUpdatedUTC = DateTime.UtcNow;
            credential.SupplimentalData2 = SquareOAuthTokenMetadata.Serialize(new SquareOAuthTokenMetadata
            {
                SquareMerchantId = result.MerchantId,
                SquareTokenType = result.TokenType,
                SquareAccessTokenExpiresAt = result.ExpiresAt,
                SquareShortLived = result.ShortLived ?? false,
                RefreshedAtUtc = credential.WhenUpdatedUTC,
                SquareEnvironment = SquareOAuthEnvironment.Normalize(
                    environment ?? existingMetadata?.SquareEnvironment),
                SquareScopes = scopes?.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToArray()
                    ?? existingMetadata?.SquareScopes,
                SquareOAuthApplicationId = squareOAuthApplicationId ?? existingMetadata?.SquareOAuthApplicationId
            });
        }

        public static bool IsRefreshDue(BaseCredential credential, DateTime utcNow)
        {
            if (string.IsNullOrWhiteSpace(credential?.RefreshToken))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(credential.LatestAccessToken))
            {
                return true;
            }

            return SquareOAuthTokenMetadata.FromStoredValue(credential.SupplimentalData2)?.IsRefreshDue(utcNow) ?? true;
        }

        public static string GetCredentialBaseUrl(BaseCredential credential)
        {
            var metadata = SquareOAuthTokenMetadata.FromStoredValue(credential?.SupplimentalData2);
            return !string.IsNullOrWhiteSpace(credential?.BaseEndpoint)
                ? credential.BaseEndpoint
                : SquareOAuthEnvironment.GetBaseUrl(metadata?.GetEnvironment(null));
        }

        public async Task<ObtainTokenResponse> GetSquareOauthAccessToken(string clientId,
            string clientSecret,
            string code,
            string redirectUri,
            string environment)
        {
            var client = new SquareClient(clientOptions: new ClientOptions
            {
                BaseUrl = SquareOAuthEnvironment.GetBaseUrl(environment)
            });

            var body = new ObtainTokenRequest
            {
                ClientId = clientId,
                GrantType = "authorization_code",
                ClientSecret = clientSecret,
                Code = code,
                RedirectUri = redirectUri
            };

            try
            {
                ObtainTokenResponse result = await client.OAuth.ObtainTokenAsync(body);
                return result;
            }
            catch (SquareException ex)
            {
                throw new InvalidOperationException(
                    $"Square OAuth token exchange failed: {ex.Message}", ex);
            }
        }

        public async Task<ObtainTokenResponse> RefreshSquareToken(string clientId,
            string clientSecret,
            string refreshToken,
            string redirectUri = null,
            string baseUrl = null)
        {
            var client = new SquareClient(clientOptions: new ClientOptions
            {
                BaseUrl = string.IsNullOrWhiteSpace(baseUrl)
                    ? SquareOAuthEnvironment.ProductionBaseUrl
                    : baseUrl
            });

            var body = new ObtainTokenRequest
            {
                ClientId = clientId,
                GrantType = "refresh_token",
                ClientSecret = clientSecret,
                RefreshToken = refreshToken,
                RedirectUri = redirectUri
            };

            try
            {
                ObtainTokenResponse result = await client.OAuth.ObtainTokenAsync(body);
                return result;
            }
            catch (SquareException ex)
            {
                throw new InvalidOperationException(
                    $"Square OAuth token refresh failed: {ex.Message}", ex);
            }
        }
    }
}
