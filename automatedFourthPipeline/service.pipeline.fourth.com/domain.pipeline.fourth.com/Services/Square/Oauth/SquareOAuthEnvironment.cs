using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;

namespace domain.pipeline.fourth.com.Services.Square.Oauth
{
    public static class SquareOAuthEnvironment
    {
        public const string Sandbox = "Sandbox";
        public const string Production = "Production";
        public const string SandboxBaseUrl = "https://connect.squareupsandbox.com";
        public const string ProductionBaseUrl = "https://connect.squareup.com";

        public static string Normalize(string environment)
        {
            return string.Equals(environment, Sandbox, StringComparison.OrdinalIgnoreCase)
                ? Sandbox
                : Production;
        }

        public static string GetBaseUrl(string environment)
        {
            return Normalize(environment) == Sandbox ? SandboxBaseUrl : ProductionBaseUrl;
        }

        public static string GetEnvironmentFromBaseUrl(string baseUrl)
        {
            return !string.IsNullOrWhiteSpace(baseUrl) &&
                   baseUrl.StartsWith(SandboxBaseUrl, StringComparison.OrdinalIgnoreCase)
                ? Sandbox
                : Production;
        }
    }

    public sealed class SquareOAuthPendingAuthorization
    {
        public string Nonce { get; set; }
        public string Environment { get; set; }
        public string[] Scopes { get; set; }
        public int? SquareOAuthApplicationId { get; set; }
        public DateTime CreatedAtUtc { get; set; }

        public static SquareOAuthPendingAuthorization FromStoredValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            try
            {
                var pending = JsonSerializer.Deserialize<SquareOAuthPendingAuthorization>(value);
                if (!string.IsNullOrWhiteSpace(pending?.Nonce))
                {
                    pending.Environment = SquareOAuthEnvironment.Normalize(pending.Environment);
                    return pending;
                }
            }
            catch (JsonException)
            {
                // Accept the pre-environment nonce format while an in-flight legacy OAuth request completes.
            }

            return new SquareOAuthPendingAuthorization
            {
                Nonce = value,
                Environment = SquareOAuthEnvironment.Production,
                Scopes = Array.Empty<string>(),
                CreatedAtUtc = DateTime.MinValue
            };
        }
    }

    public sealed class SquareOAuthTokenMetadata
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        public string SquareMerchantId { get; set; }
        public string SquareTokenType { get; set; }
        public string SquareAccessTokenExpiresAt { get; set; }
        public bool SquareShortLived { get; set; }
        public DateTime RefreshedAtUtc { get; set; }
        public string SquareEnvironment { get; set; }
        public string[] SquareScopes { get; set; }
        public int? SquareOAuthApplicationId { get; set; }

        public static SquareOAuthTokenMetadata FromStoredValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            try
            {
                return JsonSerializer.Deserialize<SquareOAuthTokenMetadata>(value, JsonOptions);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        public static string Serialize(SquareOAuthTokenMetadata metadata)
        {
            return JsonSerializer.Serialize(metadata, JsonOptions);
        }

        public bool IsRefreshDue(DateTime utcNow)
        {
            if (DateTimeOffset.TryParse(
                    SquareAccessTokenExpiresAt,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal,
                    out var expiresAt) &&
                expiresAt <= utcNow.AddDays(1))
            {
                return true;
            }

            return RefreshedAtUtc == default || RefreshedAtUtc <= utcNow.AddDays(-7);
        }

        public string GetEnvironment(string baseEndpoint)
        {
            return !string.IsNullOrWhiteSpace(SquareEnvironment)
                ? SquareOAuthEnvironment.Normalize(SquareEnvironment)
                : SquareOAuthEnvironment.GetEnvironmentFromBaseUrl(baseEndpoint);
        }
    }
}
