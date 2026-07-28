using Microsoft.AspNet.Identity.EntityFramework;
using NUnit.Framework;
using Revel._808nd.com.Classes;
using Revel._808nd.com.DTO;
using Revel._808nd.com.Models;
using Shouldly;
using ssp.api.grind.com.Controllers;
using ssp.api.grind.com.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Web.Grind._808nd.com.Models;

namespace ssp.api.grind.com.Tests.Controllers
{
    [TestFixture]
    public class CardsCustomerControllerTests
    {

        AspNetUser user;
        CardsCustomersController SUT;
        GrindSSPAuthenticationContext authDb;
        string cardNumber = "5832428";
        string cardName = "testcreatecard@nadav.com";
        string userName = "nadav808@hotmail.com";
        int pointsToAdd = 10;
        int pointsToSubtract = 10;
        int tooManyPointsToSubtract = 100000;


        [SetUp]
        public async Task SetUp()
        {
            try
            {
                authDb = new GrindSSPAuthenticationContext();
                user = authDb.AspNetUsers.First(x => x.UserName == userName);
                SUT = new CardsCustomersController();
                var identity = new GenericIdentity(userName);
                SUT.User = new GenericPrincipal(identity, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [Test]
        public async Task Get_Card_By_Number()
        {
            try
            {
                var cardCustomerResult = await SUT.Get(cardNumber) as OkNegotiatedContentResult<CardCustomer>;
                var cardCustomer = cardCustomerResult.Content;

                //do  some tests
                cardCustomer.ShouldBe<CardCustomer>(cardCustomer);
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        [Test]
        public async Task Get_Card_By_Email()
        {

            var cardCustomerResult = await SUT.Get("", cardName) as OkNegotiatedContentResult<CardCustomer>;
            var cardCustomer = cardCustomerResult.Content;

            //do  some tests
            cardCustomer.ShouldBe<CardCustomer>(cardCustomer);
        }

        [Test]
        public async Task Increment_Card()
        {
            
            var cardCustomerResult = await SUT.Get("", cardName) as OkNegotiatedContentResult<CardCustomer>;
            var cardCustomer = cardCustomerResult.Content;
            var originalCurrentPoints = cardCustomer.Card.current_points;
        

            //do  some tests
            cardCustomer.ShouldBe<CardCustomer>(cardCustomer);

            var newCardResult = await SUT.Increment(new Requests.MutateBalanceRequest
            {
                amount = pointsToSubtract,
                cardNumber = cardNumber
            }) as OkNegotiatedContentResult<RewardsCardNew>;

            var newCard = newCardResult.Content;
            newCard.current_points.ShouldBe(originalCurrentPoints + pointsToAdd);
        }

        [Test]
        public async Task Decrement_Card()
        {
            var cardCustomerResult = await SUT.Get("", cardName) as OkNegotiatedContentResult<CardCustomer>;
            var cardCustomer = cardCustomerResult.Content;
            var originalCurrentPoints = cardCustomer.Card.current_points;


            //do  some tests
            cardCustomer.ShouldBe<CardCustomer>(cardCustomer);

            var newCardResult = await SUT.Decrement(new Requests.MutateBalanceRequest
            {
                amount = pointsToSubtract,
                cardNumber = cardNumber
            }) as OkNegotiatedContentResult<RewardsCardNew>;

            var newCard = newCardResult.Content;
            newCard.current_points.ShouldBe(originalCurrentPoints - pointsToSubtract);
        }

        [Test]
        public async Task Decrement_Card_Too_Many_Points()
        {
            var cardCustomerResult = await SUT.Get("", cardName) as OkNegotiatedContentResult<CardCustomer>;
            var cardCustomer = cardCustomerResult.Content;
            var originalCurrentPoints = cardCustomer.Card.current_points;


            //do  some tests
            cardCustomer.ShouldBe<CardCustomer>(cardCustomer);

            var newCardResult = await SUT.Decrement(new Requests.MutateBalanceRequest
            {
                amount = tooManyPointsToSubtract,
                cardNumber = cardNumber
            }) as BadRequestErrorMessageResult;

            var theError = newCardResult.Message;
            theError.ShouldBeOfType<string>();
        }
    }
}
