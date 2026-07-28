using Newtonsoft.Json;
using NUnit.Framework;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.Utility;
using Revel._808nd.com.OperationsReport.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using MongoDB.Driver;
using Revel._808nd.com.Models;
using Revel._808nd.com.OperationsReport.Factory;

namespace UnitTestProject1.OpsReportTests
{
    [TestFixture]
    public class BaseCreateOpsDataTests
    {
        DateTime now;     
        List<DateTime> allFixedHourStarts = new List<DateTime>();
        HttpClient client;
        int howManyHoursBack = 4;

        List<Establishment> allEstablishments;

        GrindContext db;
        MongoClient mongoClient;
        IMongoDatabase mongoDb;
        IMongoCollection<OpsReportHourlyWrapper> collection;

        string dbName = "grindRevelOps";
        string collectionName = "hourlyOpsReports";

        List<OpsReportHourlyWrapper> peristenceDataWrappers = new List<OpsReportHourlyWrapper>();
        [SetUp]
        public async Task Given()
        {


            db = new GrindContext("GrindLiveContext");
            allEstablishments = db.Establishments.Where(x => x.establishment_id == 1).ToList();

            var RevelAPIKEY = ConfigurationManager.AppSettings["RevelAPIKEY"];
            var RevelBaseURL = ConfigurationManager.AppSettings["RevelBaseURL"];

            Establishment revOrg = new Establishment(1, "Grind",
                RevelAPIKEY,
                new Uri(RevelBaseURL));


            RevelFactory helperFactory = new RevelFactory(revOrg);
            client = helperFactory.CreateShoreditchGrindHttpClient(RevelBaseURL, RevelAPIKEY);

            mongoClient = new MongoClient(); //use local
            mongoDb = mongoClient.GetDatabase(dbName);
            collection = mongoDb.GetCollection<OpsReportHourlyWrapper>(collectionName);

            now = DateTime.Now; //get current time
                                //generate start dates

            peristenceDataWrappers = OpsReportHourlyWrapperFactory.Create(now, howManyHoursBack, allEstablishments.Select(x => x.establishment_id).ToList()).ToList();

            await When();
        }


        public async Task When()
        {
            //generate query string

            foreach (var wrapper in peristenceDataWrappers)
            {
                var query = String.Format("https://shoreditchgrind.revelup.com/reports/operations/json/?employee=&online_app=&online_app_type=&online_app_platform=&show_opened=1&show_unpaid=1&show_irregular=1&range_from={0}&range_to={1}&establishment={2}", wrapper.containerStart.ToRevelDate(), wrapper.containerEnd.ToRevelDate(), wrapper.establishmentId);
                var response = await client.GetAsync(query);
                var content = await response.Content.ReadAsStringAsync();
                var poco = JsonConvert.DeserializeObject<RootObject>(content);

                wrapper.opsReport = poco;

            }

        }

        [Test]
        public void Should_Have_Vaid_Ops_Data_For_All_Dates_And_All_Stores()
        {

            foreach (var startDate in allFixedHourStarts)
            {

                foreach (var est in allEstablishments)
                {
                    var correctWrapper = peristenceDataWrappers.First(x => x.containerStart.Equals(startDate) && x.establishmentId == est.establishment_id);
                    correctWrapper.opsReport.ShouldNotBeNull();
                }
            }
        }


    }
}
