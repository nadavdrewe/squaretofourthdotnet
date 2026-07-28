using MongoDB.Driver;
using Newtonsoft.Json;
using Quartz;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.Utility;
using Revel._808nd.com.Models;
using Revel._808nd.com.OperationsReport.Factory;
using Revel._808nd.com.OperationsReport.Models;
using Revel._808nd.com.OperationsReport.Mongo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace xeroservice.grind.railgunit.com.ScheduledTasks
{
    public class _DailyOpsReportDownload : BaseJob
    {
        DateTime now;
        List<DateTime> allFixedHourStarts = new List<DateTime>();
        HttpClient client;
        int howManyHoursBack = 10; //set this

        List<Establishment> allEstablishments;

        GrindContext db;
        MongoClient mongoClient;
        IMongoDatabase mongoDb;
        IMongoCollection<OpsReportHourlyWrapper> collection;


        List<OpsReportHourlyWrapper> peristenceDataWrappers = new List<OpsReportHourlyWrapper>();

        public override async Task Execute(IJobExecutionContext context)
        {

            //THIS JOB IS TRIGGERED EVERY DAY AT 4.30AM - DOES THE LAST 48 HOURS IN HOURLY INCREMENTS - 4am / 4am
            //CHECKS RECORDS FOR (UP TO) THE THE LAST 48 HOURS EXIST - IF NOT, WILL SYNC THEM

            db = new GrindContext();
            allEstablishments = db.Establishments.Where(x => x.establishment_id != 1).ToList();

            var RevelAPIKEY = ConfigurationManager.AppSettings["RevelAPIKEY"];
            var RevelBaseURL = ConfigurationManager.AppSettings["RevelBaseURL"];

            Establishment revOrg = new Establishment(1, "Grind",
                RevelAPIKEY,
                new Uri(RevelBaseURL));


            RevelFactory helperFactory = new RevelFactory(revOrg);
            client = helperFactory.CreateShoreditchGrindHttpClient(RevelBaseURL, RevelAPIKEY);


            var _connectionString = ConfigurationManager.ConnectionStrings["GrindMongoOps"].ToString();
            var _databaseName = MongoUrl.Create(_connectionString).DatabaseName;
            mongoClient = new MongoClient(_connectionString);//use local
            mongoDb = mongoClient.GetDatabase(_databaseName);
            collection = mongoDb.GetCollection<OpsReportHourlyWrapper>(OpsMongoDbStrings.OpsReportCollectionName);

            now = DateTime.Now; //get current time
                                //generate start dates

            peristenceDataWrappers = OpsReportHourlyWrapperFactory.Create(now, howManyHoursBack, allEstablishments.Select(x => x.establishment_id).ToList()).ToList();


            //test if record exists already - if not, pull it
            foreach (var wrapper in peristenceDataWrappers)
            {

                var doesExist = collection.Find(x => x.containerStart == wrapper.containerStart && x.establishmentId == wrapper.establishmentId);
                if (doesExist == null)
                {
                    var query = String.Format("https://shoreditchgrind.revelup.com/reports/operations/json/?employee=&online_app=&online_app_type=&online_app_platform=&show_opened=1&show_unpaid=1&show_irregular=1&range_from={0}&range_to={1}&establishment={2}", wrapper.containerStart.ToRevelDate(), wrapper.containerEnd.ToRevelDate(), wrapper.establishmentId);
                    var response = await client.GetAsync(query);
                    var content = await response.Content.ReadAsStringAsync();
                    var poco = JsonConvert.DeserializeObject<RootObject>(content);

                    wrapper.opsReport = poco;

                    collection.InsertOne(wrapper);

                }

            }


        }
    }
}
