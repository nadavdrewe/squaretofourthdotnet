using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Net.Http;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Models;
using System.Configuration;
using Should;

namespace UnitTestProject1.RevelApiTests
{
    [TestFixture]
    public class HTTPClientPATCHTests
    {
        HttpClient client { get; set; }
        private GrindContext db = new GrindContext(); //get existing values
        private string RevelCardInsertUser { get; } = ConfigurationManager.AppSettings["RevelCardInsertUser"];
        private string RevelAPIKEY { get; } = ConfigurationManager.AppSettings["RevelAPIKEY"];
        private string RevelBaseURL { get; } = ConfigurationManager.AppSettings["RevelBaseURL"];

        private WebserviceDataWriter SUT;

        private List<RewardsCardNew> cards;

        private string result;

        [SetUp]
        public async Task Given()
        {
          
            
        }


        //tests
        [Test]
        public async Task ThenResultShouldNotBeEmpty()
        {
            var est = new Establishment(1, "Grind",
                 RevelAPIKEY,
                  new Uri(RevelBaseURL));

            SUT = new WebserviceDataWriter(est, db);

            cards = db.RewardsCardNew.Take(4).ToList();
            result = await SUT.BulkUpdate(cards, "/resources/RewardsCardNew/");
            result.ShouldNotBeEmpty();
        }


    }
}
