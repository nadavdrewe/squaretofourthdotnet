using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace tests.com.fourth.pipeline.pos.LoginTests
{
    [TestFixture]
    public class LoginTest
    {
        HttpClient client;

        string STAGEUserName = "Blue_bird";
        string STAGEuserLogin = "H<2CvmVr%m}+FqL&";
        string STAGEtokenURL = "https://api-dev.fourth.com/prelive/api/eposgateway/Token";
        string payload = "username={0}&password={1}&grant_type=password";

        [SetUp]
        public async Task SetUp()
        {

            client = new HttpClient();

        }

        [Test]
        public void Test()
        {
            Assert.Pass();

        }

        [Test]
        [Explicit("Requires live Fourth stage credentials and endpoint availability.")]
        public async Task Login()
        {
            var formattedPayload = String.Format(payload, STAGEUserName, STAGEuserLogin);
            client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));


            StringContent conentToPost = new StringContent(formattedPayload);
            conentToPost.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = await client.PostAsync(STAGEtokenURL, conentToPost);
            var responseCOntent = await response.Content.ReadAsStringAsync();

            dynamic myObject = JToken.Parse(responseCOntent);

            // Log sessionA first order
            var token = myObject.access_token;
            Type type = typeof(string);
            var finalToken = System.Convert.ChangeType(token.ToString(), type);
        }
    }
}
