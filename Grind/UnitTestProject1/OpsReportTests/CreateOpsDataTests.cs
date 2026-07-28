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

namespace UnitTestProject1.OpsReportTests
{
    [TestFixture]
    public class CreateOpsDataEndToEndTests : BaseCreateOpsDataTests
    {
        DateTime now;
        DateTime oneHourAgo;
        DateTime lastFixedHourStart;
        HttpClient client;
        DateTime lastFixedHourEnd;

        int testEstablishmentId = 1;

        MongoClient mongoClient;
        IMongoDatabase db;
        IMongoCollection<OpsReportHourlyWrapper> collection;


        string dbName = "grindRevelOps";
        string collectionName = "hourlyOpsReports";

        OpsReportHourlyWrapper peristenceData;
        [SetUp]
        public async Task Given()
        {

            now = DateTime.Now; //get current time
            oneHourAgo = now.AddHours(-1); //go back an hour
            lastFixedHourStart = new DateTime(oneHourAgo.Year, oneHourAgo.Month, oneHourAgo.Day, oneHourAgo.Hour, 00, 00); // fix to the start of the previous hour
            lastFixedHourEnd = lastFixedHourStart.AddHours(1);

            var RevelAPIKEY = ConfigurationManager.AppSettings["RevelAPIKEY"];
            var RevelBaseURL = ConfigurationManager.AppSettings["RevelBaseURL"];


            Establishment revOrg = new Establishment(1, "Grind",
                RevelAPIKEY,
                new Uri(RevelBaseURL));


            RevelFactory helperFactory = new RevelFactory(revOrg);
            client = helperFactory.CreateShoreditchGrindHttpClient(RevelBaseURL, RevelAPIKEY);

            mongoClient = new MongoClient(); //use local
            db = mongoClient.GetDatabase(dbName);
            collection = db.GetCollection<OpsReportHourlyWrapper>(collectionName);

            await When();
        }


        public async Task When()
        {
            //generate query string
            var query = String.Format("https://shoreditchgrind.revelup.com/reports/operations/json/?employee=&online_app=&online_app_type=&online_app_platform=&show_opened=1&show_unpaid=1&show_irregular=1&range_from={0}&range_to={1}&establishment={2}", lastFixedHourStart.ToRevelDate(), lastFixedHourEnd.ToRevelDate(), testEstablishmentId);

            //get the data from Revel as poco
            var response = await client.GetAsync(query);
            var content = await response.Content.ReadAsStringAsync();
            var poco = JsonConvert.DeserializeObject<RootObject>(content);
            //add stamps for the hour used in the query
            peristenceData = new OpsReportHourlyWrapper { containerStart = lastFixedHourStart, containerEnd = lastFixedHourEnd, opsReport = poco, establishmentId = testEstablishmentId };

        }

        [Test]
        public void Should_Create_Data_With_Correct_Start_And_End_Dates()
        {
            peristenceData.containerEnd.ShouldBe(lastFixedHourEnd);
            peristenceData.containerStart.ShouldBe(lastFixedHourStart);

        }



        [Test]
        public void Should_Persist_Data_With_Correct_Start_And_End_Dates()
        {

            collection.InsertOne(peristenceData);
            peristenceData._id.ShouldNotBeNullOrWhiteSpace();

            var savedData = collection.Find(x => x._id == peristenceData._id).FirstOrDefault();
            savedData.ShouldNotBeNull();


        }

    }
}
