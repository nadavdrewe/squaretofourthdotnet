using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using com.fourth.pipeline.pos.Services.SalesApi;
using NUnit.Framework;
using Shouldly;

namespace tests.domain.pipelines.fourth.com.FourthSales
{
    [TestFixture]
    public class FourthOAuthClientTests
    {
        [Test]
        public async Task Login_WithClientCredentials_PostsDocumentedOAuthRequest()
        {
            var handler = new CaptureHandler(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"access_token\":\"fourth-access\",\"expires_in\":3600,\"token_type\":\"Bearer\",\"refresh_token\":\"fourth-refresh\",\"scope\":\"EposGateway\"}")
            });

            using var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://fourth.example/api/eposgateway/")
            };

            var service = new FourthApiService(
                username: null,
                password: null,
                apiBaseUrl: "https://fourth.example/api/eposgateway/",
                clientId: "client-id",
                clientSecret: "client-secret",
                scope: "EposGateway",
                tokenUrl: "https://fourth.example/oauth/connect/token",
                client: httpClient);

            await service.Login();

            handler.Requests.Count.ShouldBe(1);
            handler.Requests[0].Method.ShouldBe(HttpMethod.Post);
            handler.Requests[0].RequestUri.ToString().ShouldBe("https://fourth.example/oauth/connect/token");

            var form = ParseForm(handler.RequestBodies[0]);
            form["grant_type"].ShouldBe("client_credentials");
            form["client_id"].ShouldBe("client-id");
            form["client_secret"].ShouldBe("client-secret");
            form["scope"].ShouldBe("EposGateway");

            service.AccessToken.ShouldBe("fourth-access");
            service.RefreshToken.ShouldBe("fourth-refresh");
            service.ExpiresInSeconds.ShouldBe(3600);
            httpClient.DefaultRequestHeaders.Authorization.Scheme.ShouldBe("Bearer");
            httpClient.DefaultRequestHeaders.Authorization.Parameter.ShouldBe("fourth-access");
        }

        [Test]
        public async Task Login_WhenTokenEndpointOmitted_UsesFourthRootOAuthEndpoint()
        {
            var handler = new CaptureHandler(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"access_token\":\"fourth-access\"}")
            });

            using var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://api-dev.fourth.com/prelive/api/eposgateway/")
            };

            var service = new FourthApiService(
                username: null,
                password: null,
                apiBaseUrl: "https://api-dev.fourth.com/prelive/api/eposgateway/",
                clientId: "client-id",
                clientSecret: "client-secret",
                scope: "EposGateway",
                tokenUrl: null,
                client: httpClient);

            await service.Login();

            handler.Requests[0].RequestUri.ToString().ShouldBe("https://api-dev.fourth.com/prelive/oauth/connect/token");
        }

        private static IReadOnlyDictionary<string, string> ParseForm(string body)
        {
            return body
                .Split('&', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split('=', 2))
                .ToDictionary(
                    x => Uri.UnescapeDataString(x[0].Replace("+", " ")),
                    x => Uri.UnescapeDataString(x[1].Replace("+", " ")));
        }

        private sealed class CaptureHandler : HttpMessageHandler
        {
            private readonly HttpResponseMessage _response;

            public CaptureHandler(HttpResponseMessage response)
            {
                _response = response;
            }

            public List<HttpRequestMessage> Requests { get; } = new List<HttpRequestMessage>();
            public List<string> RequestBodies { get; } = new List<string>();

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Requests.Add(request);
                RequestBodies.Add(request.Content == null
                    ? string.Empty
                    : await request.Content.ReadAsStringAsync(cancellationToken));

                return _response;
            }
        }
    }
}
