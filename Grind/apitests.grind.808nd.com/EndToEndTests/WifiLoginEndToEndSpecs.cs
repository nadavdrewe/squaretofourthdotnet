using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using api.grind._808nd.com;
using api.grind._808nd.com.Controllers;
using Microsoft.Owin.Hosting;
using NUnit.Framework;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Models;
using Should;
using SpecsFor;


namespace apitests.grind._808nd.com.EndToEndTests
{
    [TestFixture]
    public class WifiLoginEndToEndSpecs
    {


        public class when_adding_a_new_wifiLogin_through_the_api
        {
            private static HttpServer server;
            private const string baseApiUrl = "http://*:9443/";
            private GrindContext db;
            private HttpClient client;
            int recordsPriorCount = 0;
            private HttpResponseMessage result;



            [TestFixtureSetUp]
            public void Setup()
            {
                db = new GrindContext();

                var config = new HttpConfiguration();
                config.MapHttpAttributeRoutes();
                config.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "api/{controller}/{id}",
                    defaults: new { id = RouteParameter.Optional }
                );
                config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
                server = new HttpServer(config);

                client = new HttpClient(server);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.BaseAddress = new Uri("http://baseaddress.com");

                recordsPriorCount = db.WifiLogins.Count();



            }


            [Test]
            public async Task then_it_should_return_200_OK()
            {
                //act
                var wifiLogon = new WifiLogin
                {
                    Email = "test@test.com",
                    FirstName = "test",
                    LastName = "tester"
                };
                result = client.PostAsJsonAsync("api/WifiLogins", wifiLogon).Result;

                //assert
                result.IsSuccessStatusCode.ShouldBeTrue();
            }


            [TestFixtureTearDown]
            public void Teardown()
            {
                db.Dispose();
                client.Dispose();
                server.Dispose();
            }

        }

    }
}
