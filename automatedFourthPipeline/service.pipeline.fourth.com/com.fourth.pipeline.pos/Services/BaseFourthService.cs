using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace com.fourth.pipeline.pos.Services
{
    public class BaseFourthService
    {
        private const string PasswordGrantType = "password";
        private const string ClientCredentialsGrantType = "client_credentials";
        private const string RefreshTokenGrantType = "refresh_token";

        private readonly string _username;
        private readonly string _password;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _scope;
        private readonly string _grantType;
        private string _tokenUrl;

        protected HttpClient _client;

        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int? ExpiresInSeconds { get; set; }

        public BaseFourthService(string username, string password, string baseUrl)
            : this(
                username,
                password,
                baseUrl,
                clientId: null,
                clientSecret: null,
                scope: null,
                tokenUrl: GenerateLegacyTokenUrl(baseUrl),
                grantType: PasswordGrantType)
        {
        }

        public BaseFourthService(string username, string password, string url, HttpClient client)
            : this(username, password, url)
        {
            _client = client;
        }

        public BaseFourthService(
            string username,
            string password,
            string apiBaseUrl,
            string clientId,
            string clientSecret,
            string scope,
            string tokenUrl,
            string grantType = ClientCredentialsGrantType)
        {
            _username = username;
            _password = password;
            _clientId = clientId;
            _clientSecret = clientSecret;
            _scope = scope;
            _grantType = string.IsNullOrWhiteSpace(grantType) ? ClientCredentialsGrantType : grantType;
            _tokenUrl = string.IsNullOrWhiteSpace(tokenUrl) ? GenerateOAuthTokenUrl(apiBaseUrl) : tokenUrl;

            _client = new HttpClient
            {
                BaseAddress = new Uri(apiBaseUrl)
            };
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public BaseFourthService(
            string username,
            string password,
            string apiBaseUrl,
            string clientId,
            string clientSecret,
            string scope,
            string tokenUrl,
            HttpClient client,
            string grantType = ClientCredentialsGrantType)
            : this(username, password, apiBaseUrl, clientId, clientSecret, scope, tokenUrl, grantType)
        {
            _client = client;
            if (_client.BaseAddress == null)
            {
                _client.BaseAddress = new Uri(apiBaseUrl);
            }
        }

        public bool IsLoggedIn()
        {
            return !string.IsNullOrWhiteSpace(AccessToken);
        }

        public async Task<HttpResponseMessage> Login(string tokenUrl = "")
        {
            if (!string.IsNullOrWhiteSpace(tokenUrl))
            {
                _tokenUrl = tokenUrl;
            }

            try
            {
                using var content = new FormUrlEncodedContent(CreateTokenRequest());
                var response = await _client.PostAsync(_tokenUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Fourth OAuth returned {(int)response.StatusCode} {response.StatusCode}: {responseContent}");
                }

                var responseJson = JToken.Parse(responseContent);
                var accessToken = responseJson.Value<string>("access_token");
                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    throw new Exception("Fourth OAuth response did not include an access_token.");
                }

                AccessToken = accessToken;
                RefreshToken = responseJson.Value<string>("refresh_token");
                ExpiresInSeconds = responseJson.Value<int?>("expires_in");
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to log in to Fourth", ex);
            }
        }

        public async Task<HttpResponseMessage> RefreshLogin(string refreshToken, string tokenUrl = "")
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new ArgumentException("A refresh token is required.", nameof(refreshToken));
            }

            if (!string.IsNullOrWhiteSpace(tokenUrl))
            {
                _tokenUrl = tokenUrl;
            }

            try
            {
                using var content = new FormUrlEncodedContent(CreateRefreshTokenRequest(refreshToken));
                var response = await _client.PostAsync(_tokenUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Fourth OAuth refresh returned {(int)response.StatusCode} {response.StatusCode}: {responseContent}");
                }

                var responseJson = JToken.Parse(responseContent);
                var accessToken = responseJson.Value<string>("access_token");
                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    throw new Exception("Fourth OAuth refresh response did not include an access_token.");
                }

                AccessToken = accessToken;
                RefreshToken = responseJson.Value<string>("refresh_token") ?? refreshToken;
                ExpiresInSeconds = responseJson.Value<int?>("expires_in");
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to refresh Fourth login", ex);
            }
        }

        private IReadOnlyDictionary<string, string> CreateTokenRequest()
        {
            var grantType = string.IsNullOrWhiteSpace(_grantType)
                ? (string.IsNullOrWhiteSpace(_clientId) ? PasswordGrantType : ClientCredentialsGrantType)
                : _grantType;

            var request = new Dictionary<string, string>
            {
                ["grant_type"] = grantType
            };

            if (grantType == ClientCredentialsGrantType)
            {
                request["client_id"] = _clientId;
                request["client_secret"] = _clientSecret;
                AddScopeIfPresent(request);
                return request;
            }

            request["username"] = _username;
            request["password"] = _password;
            AddClientCredentialsIfPresent(request);
            AddScopeIfPresent(request);
            return request;
        }

        private IReadOnlyDictionary<string, string> CreateRefreshTokenRequest(string refreshToken)
        {
            var request = new Dictionary<string, string>
            {
                ["grant_type"] = RefreshTokenGrantType,
                ["refresh_token"] = refreshToken
            };

            AddClientCredentialsIfPresent(request);
            return request;
        }

        private void AddClientCredentialsIfPresent(IDictionary<string, string> request)
        {
            if (!string.IsNullOrWhiteSpace(_clientId))
            {
                request["client_id"] = _clientId;
            }

            if (!string.IsNullOrWhiteSpace(_clientSecret))
            {
                request["client_secret"] = _clientSecret;
            }
        }

        private void AddScopeIfPresent(IDictionary<string, string> request)
        {
            if (!string.IsNullOrWhiteSpace(_scope))
            {
                request["scope"] = _scope;
            }
        }

        private static string GenerateLegacyTokenUrl(string baseAddress)
        {
            return $"{baseAddress.TrimEnd('/')}/Token";
        }

        private static string GenerateOAuthTokenUrl(string baseAddress)
        {
            var uri = new Uri(baseAddress.TrimEnd('/') + "/");
            var path = uri.AbsolutePath.TrimEnd('/');
            var apiIndex = path.IndexOf("/api/", StringComparison.OrdinalIgnoreCase);
            var rootPath = apiIndex >= 0
                ? path.Substring(0, apiIndex)
                : path;
            var builder = new UriBuilder(uri)
            {
                Path = $"{rootPath.TrimEnd('/')}/oauth/connect/token",
                Query = string.Empty
            };

            return builder.Uri.ToString();
        }
    }
}
