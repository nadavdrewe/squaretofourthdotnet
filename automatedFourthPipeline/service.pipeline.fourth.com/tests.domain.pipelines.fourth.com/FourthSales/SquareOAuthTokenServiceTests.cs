using System;
using System.Text.Json;
using data.pipeline.fourth.com.Models.Credentials;
using domain.pipeline.fourth.com.Services.Square.Oauth;
using NUnit.Framework;
using Shouldly;
using Square;

namespace tests.domain.pipelines.fourth.com.FourthSales
{
    [TestFixture]
    public class SquareOAuthTokenServiceTests
    {
        [Test]
        public void ApplyTokenResponse_StoresAccessRefreshAndMetadata()
        {
            var credential = new BaseCredential
            {
                RefreshToken = "old-refresh"
            };

            var response = new ObtainTokenResponse
            {
                AccessToken = "new-access",
                RefreshToken = "new-refresh",
                MerchantId = "merchant-1",
                TokenType = "bearer",
                ExpiresAt = "2026-04-30T00:00:00Z",
                ShortLived = false
            };

            SquareOAuthTokenService.ApplyTokenResponse(credential, response, requireRefreshToken: true);

            credential.LatestAccessToken.ShouldBe("new-access");
            credential.RefreshToken.ShouldBe("new-refresh");
            credential.WhenUpdatedUTC.ShouldBeGreaterThan(DateTime.UtcNow.AddMinutes(-1));

            using var metadata = JsonDocument.Parse(credential.SupplimentalData2);
            metadata.RootElement.GetProperty("squareMerchantId").GetString().ShouldBe("merchant-1");
            metadata.RootElement.GetProperty("squareTokenType").GetString().ShouldBe("bearer");
            metadata.RootElement.GetProperty("squareAccessTokenExpiresAt").GetString().ShouldBe("2026-04-30T00:00:00Z");
            metadata.RootElement.GetProperty("squareShortLived").GetBoolean().ShouldBeFalse();
            metadata.RootElement.GetProperty("refreshedAtUtc").GetDateTime().ShouldBeGreaterThan(DateTime.UtcNow.AddMinutes(-1));
        }

        [Test]
        public void ApplyTokenResponse_PreservesExistingRefreshToken_WhenRefreshResponseOmitsIt()
        {
            var credential = new BaseCredential
            {
                RefreshToken = "existing-refresh"
            };

            var response = new ObtainTokenResponse
            {
                AccessToken = "refreshed-access"
            };

            SquareOAuthTokenService.ApplyTokenResponse(credential, response);

            credential.LatestAccessToken.ShouldBe("refreshed-access");
            credential.RefreshToken.ShouldBe("existing-refresh");
        }

        [Test]
        public void ApplyTokenResponse_ThrowsWhenInitialOAuthResponseHasNoRefreshToken()
        {
            var credential = new BaseCredential();
            var response = new ObtainTokenResponse
            {
                AccessToken = "new-access"
            };

            Should.Throw<InvalidOperationException>(() =>
                SquareOAuthTokenService.ApplyTokenResponse(credential, response, requireRefreshToken: true));
        }

        [Test]
        public void ApplyTokenResponse_StoresEnvironmentAndScopes()
        {
            var credential = new BaseCredential();
            var response = new ObtainTokenResponse { AccessToken = "access", RefreshToken = "refresh" };

            SquareOAuthTokenService.ApplyTokenResponse(
                credential,
                response,
                requireRefreshToken: true,
                environment: SquareOAuthEnvironment.Sandbox,
                scopes: new[] { "ORDERS_READ", "PAYMENTS_READ" });

            using var metadata = JsonDocument.Parse(credential.SupplimentalData2);
            metadata.RootElement.GetProperty("squareEnvironment").GetString().ShouldBe(SquareOAuthEnvironment.Sandbox);
            metadata.RootElement.GetProperty("squareScopes").GetArrayLength().ShouldBe(2);
        }

        [Test]
        public void ApplyTokenResponse_PreservesSelectedOAuthApplicationAcrossRefresh()
        {
            var credential = new BaseCredential();
            var initialResponse = new ObtainTokenResponse
            {
                AccessToken = "initial-access",
                RefreshToken = "refresh"
            };

            SquareOAuthTokenService.ApplyTokenResponse(
                credential,
                initialResponse,
                requireRefreshToken: true,
                environment: SquareOAuthEnvironment.Production,
                squareOAuthApplicationId: 42);

            SquareOAuthTokenService.ApplyTokenResponse(
                credential,
                new ObtainTokenResponse { AccessToken = "refreshed-access" });

            var metadata = SquareOAuthTokenMetadata.FromStoredValue(credential.SupplimentalData2);
            metadata.SquareOAuthApplicationId.ShouldBe(42);
        }

        [Test]
        public void IsRefreshDue_RefreshesEverySevenDays()
        {
            var credential = new BaseCredential
            {
                LatestAccessToken = "access",
                RefreshToken = "refresh",
                SupplimentalData2 = "{\"refreshedAtUtc\":\"2026-01-01T00:00:00Z\"}"
            };

            SquareOAuthTokenService.IsRefreshDue(credential, new DateTime(2026, 1, 8, 0, 0, 0, DateTimeKind.Utc)).ShouldBeTrue();
        }
    }
}
