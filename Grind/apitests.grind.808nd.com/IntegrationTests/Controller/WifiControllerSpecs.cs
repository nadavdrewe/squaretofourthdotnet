using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using api.grind._808nd.com.Controllers;
using NUnit.Framework;
using Revel._808nd.com.Classes;
using Should;
using SpecsFor;

namespace apitests.grind._808nd.com.Controller
{

    public class WifiControllerSpecs
    {
        [TestFixture]
        public class when_testing_postToController : SpecsFor<WifiLoginsController>
        {
            private IHttpActionResult result;
            protected override void Given()
            {
                SUT = new WifiLoginsController();
            }

            protected override async void When()
            {
              

            }


            [Test]
            public async Task then_it_creates_a_record()
            {
                var testLogin = new WifiLogin
                {
                    Email = "test@test.com",
                    FirstName = "Tommy",
                    LastName = "Tanker",
                    LoginDate = DateTime.Now
                };
                //post

                result = await SUT.PostWifiLogin(new WifiLogin());
                result.ShouldBeType<CreatedAtRouteNegotiatedContentResult<WifiLogin>>();
            }
        }


    }
}
