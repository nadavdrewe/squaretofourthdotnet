using NUnit.Framework;
using Square;
using Square.OAuth;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace tests.square.pipeline.fourth.com.ClientTests
{
    [TestFixture]
    [Explicit("Requires live Square OAuth credentials and authorization code.")]
    class OAuthTests
    {
        string endpoint = "/oauth2​/authorize";

        string clientId = "";
        string clientSecret = "";

        string sandBoxId = "";
            string sandBoxSecret = "";

        //tokens
        string sandboxaccessToken = "";
        string sandboxrefreshToken = "";

        [SetUp]
        public async Task SetUp()
        {

        }


        [Test]
        public async Task TestOAuthLocally()
        {
            var client = new SquareClient();

            var body = new ObtainTokenRequest
            {
                ClientId = clientId,
                GrantType = "authorization_code",
                ClientSecret = clientSecret,
                Code = "",
                RedirectUri = "http://localhost"
            };

            try
            {
                ObtainTokenResponse result = await client.OAuth.ObtainTokenAsync(body);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OAuthApi.ObtainToken: " + e.Message);
            }
        }
    }
}

