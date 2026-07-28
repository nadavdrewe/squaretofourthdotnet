using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using api.grind._808nd.com;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Revel._808nd.com;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Models;
using api.grind._808nd.com.Controllers;


namespace apitests.grind._808nd.com
{
    [TestFixture]
    public class RewardCardsControllerTests
    {

        private Mock<DbSet<RewardsCardNew>> mockSet;
        private Mock<GrindContext> mockContext;
        private DbConnection dbConnection;
        private GrindContext grindContextEffort;

        [SetUp]
        public void SetupFixture()
        {
            var cardsList = new List<RewardsCardNew>
            {
                new RewardsCardNew
                {
                    DBKEY_rewardscardnew_id = 1,
                is_vip_card = false,
                number = "0000001",
                created_by = "/testvalue",               
                }
                
            }.AsQueryable();


            mockSet = new Mock<DbSet<RewardsCardNew>>();
            mockSet.As<IQueryable<RewardsCardNew>>().Setup(x => x.Provider).Returns(cardsList.Provider);
            mockSet.As<IQueryable<RewardsCardNew>>().Setup(x => x.Expression).Returns(cardsList.Expression);
            mockSet.As<IQueryable<RewardsCardNew>>().Setup(x => x.ElementType).Returns(cardsList.ElementType);
            mockSet.As<IQueryable<RewardsCardNew>>().Setup(x => x.GetEnumerator()).Returns(cardsList.GetEnumerator);


            mockContext = new Mock<GrindContext>();
            mockContext.Setup(x => x.RewardsCardNew).Returns(mockSet.Object);



            // create a new DbConnection using Effort
         /*   DbConnection connection = Effort.DbConnectionFactory.CreateTransient();
            grindContextEffort = new GrindContext(connection);

            grindContextEffort.Database.CreateIfNotExists();



            grindContextEffort.RewardsCardNew.Add(new RewardsCardNew()
            {
                DBKEY_rewardscardnew_id = 1,
                is_vip_card = false,
                number = "0000001",
                created_by = "/testvalue",
            });


            grindContextEffort.RewardsCardNew.Add(new RewardsCardNew
                     {
                         DBKEY_rewardscardnew_id = 2,
                         is_vip_card = false,
                         number = "0000002",
                         created_by = "/testvalue",
                     });

            grindContextEffort.RewardsCardNew.Add(new RewardsCardNew
            {
                DBKEY_rewardscardnew_id = 3,
                is_vip_card = false,
                number = "0000003",
                created_by = "/testvalue",
            });

            grindContextEffort.RewardsCardNew.Add(new RewardsCardNew
            {
                DBKEY_rewardscardnew_id = 4,
                is_vip_card = false,
                number = "0000004",
                created_by = "/testvalue",
            });

            grindContextEffort.Customers.Add(new Customer
            {
                Email = "test@test.com",
                LoyaltyNumber = "0000004",
                FirstName = "Mr Test",
                LastName = "Customer"
            });

            grindContextEffort.SaveChanges();*/
        }


        [Test]
        public void ShouldReturnRewardCard()
        {


            var sut = new RewardCardsController(mockContext.Object);

            IHttpActionResult result = sut.Get(1);

            var contextResult = result as OkNegotiatedContentResult<RewardsCardNew>;

            Assert.That(contextResult.Content, Is.InstanceOf<RewardsCardNew>());
            Assert.That(contextResult.Content, Is.TypeOf<RewardsCardNew>());
            Assert.That(contextResult.Content.DBKEY_rewardscardnew_id, Is.EqualTo(1));
           
        }


        [Test]
        public async void ShouldReturnRewardCardGivenNumber()
        {

            var sut = new RewardCardsController(mockContext.Object);

            IHttpActionResult result = await sut.Get("0000001");
            var content = result as OkNegotiatedContentResult<object>;
            var card = content.Content as RewardsCardNew;

            Assert.That(card.number, Is.EqualTo("0000001"));

        }

        [Test]
        public async void ShouldReturnRewardCardGivenCustomerEmail()
        {

            var sut = new RewardCardsController(mockContext.Object);

            var result = await sut.Get("0000001") as IHttpActionResult;
            
            var content = result as OkNegotiatedContentResult<object>;

            var card = content.Content as RewardsCardNew;


            Assert.That(card.number, Is.EqualTo("0000001"));
            Assert.That(card.is_vip_card, Is.EqualTo(false));

        }

        [Test]
        public async void Should_Return_404NotFound_If_CardDoesntExist()
        {

            var sut = new RewardCardsController(mockContext.Object);

            IHttpActionResult result = sut.Get(0);
            var contentResult = result;

            Assert.That(contentResult, Is.InstanceOf<NotFoundResult>());

        }


        public void Should_Return200_When_ReturningCard()
        {

            var sut = new RewardCardsController(mockContext.Object);

            IHttpActionResult result = sut.Get(1);
            var contentResult = result as OkNegotiatedContentResult<RewardsCardNew>;

            Assert.That(result, Is.InstanceOf<OkResult>());

        }

    }
}
