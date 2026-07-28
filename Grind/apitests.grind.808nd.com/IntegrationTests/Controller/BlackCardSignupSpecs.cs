using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using api.grind._808nd.com.Controllers;
using api.grind._808nd.com.Models;
using NUnit.Framework;
using Revel._808nd.com;
using Should;
using SpecsFor;

namespace apitests.grind._808nd.com.IntegrationTests.Controller
{
    [TestFixture]
    public class BlackCardSignupSpecs
    {
        static BlackCardSignupViewModel testSignup = new BlackCardSignupViewModel
        {
            cardNumber = "99999999",
            dob = "10/04/1981",
            email = "test@testertestingAPI.com",
            firstname = "NadavTest",
            lastname = "DreweTest"
        };

        public static IHttpActionResult result;

        public class when_SPEC : SpecsFor<BlackCardSignupsController>
        {


            [SetUp]
            public async Task Setup()
            {
                SUT = new BlackCardSignupsController();
                result = await SUT.PostBlackCardSignup(testSignup);
            }


            [Test]
            public void should_return_newly_created_id()
            {

                CreatedAtRouteNegotiatedContentResult<BlackCardSignup> createdRes =
                    result as CreatedAtRouteNegotiatedContentResult<BlackCardSignup>;

                var returnId = createdRes.RouteValues["id"] as int?;
                var routeName = createdRes.RouteName;

                result.ShouldBeType<CreatedAtRouteNegotiatedContentResult<BlackCardSignup>>();
                Assert.AreNotEqual(returnId, 0);
                Assert.GreaterOrEqual(returnId, 1);
            }


            [Test]
            public async Task should_return_newly_created_id_and_then_read_obect_with_same_values()
            {
                //CreatedAtRouteNegotiatedContentResult<BlackCardSignup> createdRes =
                //   result as CreatedAtRouteNegotiatedContentResult<BlackCardSignup>;

                //var returnId = createdRes.RouteValues["id"] as int?;
                //var returnEdsignUP = await SUT.GetBlackCardSignup((int)returnId) as OkNegotiatedContentResult<BlackCardSignup>;

                //returnEdsignUP.ShouldBeType<OkNegotiatedContentResult<BlackCardSignup>>();
                //returnEdsignUP.Content.cardNumber.ShouldEqual(testSignup.cardNumber);
               

            }


            [Test]
            public void should_resturn_bad_request_as_card_number_already_exists()
            {
                var secondDuplicateResult = SUT.PostBlackCardSignup(testSignup);
                //setup has already run. 
            }

        }
    }
}