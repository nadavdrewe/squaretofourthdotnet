using MongoDB.Bson.IO;
using MongoDB.Driver;
using Quartz;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.Utility;
using Revel._808nd.com.Models;
using Revel._808nd.com.OperationsReport.Models;
using Revel._808nd.com.OperationsReport.Mongo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace automatedreports.grind.railgunit.com.ScheduledTasks
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }
    

    public class BaseJob : IJob
    {
        protected DateTime finalDateToPullTo;
        protected List<DateTime> allFixedHourStarts = new List<DateTime>();
        protected HttpClient client;
        protected int howManyHoursBack = 0;

        protected List<Establishment> allEstablishments;

        protected GrindContext db;
        protected MongoClient mongoClient;
        protected IMongoDatabase mongoDb;
        protected IMongoCollection<OpsReportHourlyWrapper> collection;

        protected List<OpsReportHourlyWrapper> peristenceDataWrappers = new List<OpsReportHourlyWrapper>();


        protected void Bootstrap()
        {
            //for TLS
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback = new
            RemoteCertificateValidationCallback
            (
               delegate { return true; }
            );

            var RevelAPIKEY = ConfigurationManager.AppSettings["RevelAPIKEY"];
            var RevelBaseURL = ConfigurationManager.AppSettings["RevelBaseURL"];

            Establishment revOrg = new Establishment(1, "Grind",
                RevelAPIKEY,
                new Uri(RevelBaseURL));


            RevelFactory helperFactory = new RevelFactory(revOrg);
            client = helperFactory.CreateShoreditchGrindHttpClient(RevelBaseURL, RevelAPIKEY);

            db = new GrindContext();

            var _connectionString = ConfigurationManager.ConnectionStrings["GrindMongoOps"].ToString();
            var _databaseName = MongoUrl.Create(_connectionString).DatabaseName;

            //use remote db
            mongoClient = new MongoClient(_connectionString);
            mongoDb = mongoClient.GetDatabase(_databaseName);
            collection = mongoDb.GetCollection<OpsReportHourlyWrapper>(OpsMongoDbStrings.OpsReportCollectionName);


            //LOCAL - FOR TESTING
            //mongoClient = new MongoClient();//use local
            //mongoDb = mongoClient.GetDatabase(_databaseName);
            //collection = mongoDb.GetCollection<OpsReportHourlyWrapper>(OpsMongoDbStrings.OpsReportCollectionName);



        }

        protected void Init(DateTime endDateTime, int hourManyHoursBackFromEnd)
        {

            howManyHoursBack = hourManyHoursBackFromEnd;
            finalDateToPullTo = endDateTime;

        }

        protected List<DateTimeStartEndRange> CreateHourlyRanges(DateTime baseDate, int hoursBackFromBaseDate)
        {
            var listOFHours = new List<DateTimeStartEndRange>();
            for (int i = hoursBackFromBaseDate; i > 0; i--)
            {
                var someHoursAgo = baseDate.AddHours(-i); //go back x hours
                var currentFixedHourStart = new DateTime(someHoursAgo.Year, someHoursAgo.Month, someHoursAgo.Day, someHoursAgo.Hour, 00, 00); // fix to the start of the previous hour
                var currentFixedHoursEnd = currentFixedHourStart.AddHours(1);

                listOFHours.Add(new DateTimeStartEndRange { Start = currentFixedHourStart, End = currentFixedHoursEnd });
            }

            return listOFHours;
        }

        public async Task<OpsReportHourlyWrapper> PopulateOpsReportWrapperFromRevel(OpsReportHourlyWrapper wrapper)
        {

            try
            {

                var query = String.Format("https://shoreditchgrind.revelup.com/reports/operations/json/?employee=&online_app=&online_app_type=&online_app_platform=&show_opened=1&show_unpaid=1&show_irregular=1&range_from={0}&range_to={1}&establishment={2}", wrapper.containerStart.ToRevelDate(), wrapper.containerEnd.ToRevelDate(), wrapper.establishmentId);


                var response = await client.GetAsync(query);
                var content = await response.Content.ReadAsStringAsync();
                var poco = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(content);

                wrapper.opsReport = poco;
                return wrapper;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception:", ex.Message);
                throw;
            }

        }

        public void SaveOpsDataToMongo(OpsReportHourlyWrapper wrapper)
        {
            collection.InsertOne(wrapper);
        }

        protected void SendEmailNotification(string subject, string message)
        {
            //using (var client = new GmailClient("grindandco808@gmail.com", "teenpunks23"))
            //{
            //    client.Send("emailnadz@gmail.com", subject, message);
            //}

        }

        public virtual async Task Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
