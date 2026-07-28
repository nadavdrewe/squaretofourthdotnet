using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Models;
using Revel._808nd.com.Models.ViewModels;
using Web.Grind._808nd.com.Controllers;

namespace apitests.grind._808nd.com.MVCTests
{

    [TestFixture]
    class BlackCardFormTests
    {

        private BlackCardFormController sut;

        private MockRepository mockRepo;
        
        
        
        private Mock<DbSet<RewardsCardNew>> mockSet;
        private Mock<GrindContext> mockContext;
        private DbConnection dbConnection;
        private GrindContext grindContextEffort;

        [TestFixtureSetUp]        
        public void TestFixtureSetUp()
        {
            //Moq
            mockRepo = new MockRepository(new MockBehavior
            {

            });

            mockSet = new Mock<DbSet<RewardsCardNew>>().SetupAllProperties();
            
            mockSet.Object.Add(new RewardsCardNew
            {
                DBKEY_rewardscardnew_id = 1,
                is_vip_card = false,
                number = "0000001",
                created_by = "/testvalue", 
            });

            mockContext = new Mock<GrindContext>().SetupAllProperties();            
            mockContext.Setup(x => x.RewardsCardNew).Returns(mockSet.Object);

            sut = new BlackCardFormController(mockContext.Object);
        
           // create a new DbConnection using Effort
          /*  DbConnection connection = Effort.DbConnectionFactory.CreateTransient();

            grindContextEffort = new GrindContext(dbConnection);

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


        [SetUp]
        public void SetUp()
        {


        }


        //SETUP END

        [Test]
        public async void ShouldReturnEmptyView()
        {



            var result = await sut.Create();

         
            Assert.That(result, Is.TypeOf<ViewResult>());
           

        }

        [Test]
        public async void ShouldReturnTheSameView()
        {

            var model = mockRepo.Create<ICustomerCardViewModel>();
            model.Setup(x => x.firstname).Returns("Bobby");
            model.Setup(x => x.lastname).Returns("Fisher");

             
            var result = await sut.Create();

            Assert.That(result, Is.TypeOf<ViewResult>());


        }
    }
}
