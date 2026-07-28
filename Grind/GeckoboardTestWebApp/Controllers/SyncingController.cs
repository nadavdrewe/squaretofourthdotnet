using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.ServiceImplemenations;
using Revel._808nd.com.Classes.ServiceImplementaitons;
using Revel._808nd.com.Classes.WebserviceReader;
using Revel._808nd.com.Classes.WebserviceReaderImplementations;
using Revel._808nd.com.Extensions;
using Revel._808nd.com.Models;
using Web.Grind._808nd.com.Controllers;

namespace GeckoboardTestWebApp.Controllers
{
    public class SyncingController : Controller
    {
        private GrindContext db = new GrindContext();
        private string RevelAPIKEY { get; } = ConfigurationManager.AppSettings["RevelAPIKEY"];
        private string RevelBaseURL { get; } = ConfigurationManager.AppSettings["RevelBaseURL"];


        public async Task<bool> TestTest()
        {
            return true;

        }

        //

        // GET: /Syncing/
        public async Task<bool> ordersUpdateAndGeckoPush()
        {

            TestController theController = new TestController();


            using (var emailer = new EmailController())
            {

                /*emailer.SendMessageNadavIgnoreSendExeceptions(
                       String.Format("Grind 7 min update service ordersUpdateAndGeckoPush started"), null, "railgunit.maintenance@gmail.com");        
//*/
                var paymentUpdate = await theController.GetPaymentsForTheLastWeekAndInsertMissingIntoDBSinglePull();
                var OiUpdate = await theController.UpdateDatabaseOrdersAndItems();

                var success = db.Database.SqlQuery<int>("sp_RemoveDuplicateOrders");
                var successAgain = db.Database.SqlQuery<int>("sp_RemoveDuplicateOrderItems");

                emailer.SendMessageNadavIgnoreSendExeceptions(
                     String.Format("Grind 7 min update service orderitems worked"), null, "railgunit.maintenance@gmail.com");
            }

            //set up TLS
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var ok = await theController.GECKOBOARD_PushAllDailyWidgets();



            return true;
        }

        public async Task<bool> OvernightWidgets()
        {
            TestController theController = new TestController();

            var paymentUpdate = await theController.RunCombinedOvernightWidgets();


            return true;
        }


        public async Task<bool> recoverMissingOrdersAndItemsForToday()
        {

            TestController theController = new TestController();
            var ok = await theController.MAINTAINANCE_Today_CombinedOrderAndOrderItemSync();
            var nook = await theController.MAINTAINANCE_UpdatePaymentsToday();


            return true;
        }

        public async Task<bool> longerTimer()
        {
            TestController theController = new TestController();
            var ok = await theController.UpdateDatabaseProductsAndCategories();
            await theController.MAINTAINANCE_Today_CombinedOrderAndOrderItemSync();
            ok = await theController.GetDiscountsAndSaveToDB();
            ok = await theController.RunCombinedOvernightWidgets();
            return true;
        }


        public async Task<bool> NotInOpeningHoursTimer()
        {
            TestController theController = new TestController();

            var ok = await theController.MAINTAINANCE_ThisWeek_OrderSync();
            ok = await theController.MAINTAINANCE_LastWeek_OrderSync();
            var ndok = await theController.UpdateDatabaseProductsAndCategories();
            ndok = await theController.GetDiscountsAndSaveToDB();
            ndok = await theController.RunCombinedOvernightWidgets();

            return true;
        }


        public async Task<int> FullCustomerSync()
        {
            var service = new CustomerService(db);
            var ok = await service.GetAllCustomersAndInsertNew(new DateTime(2013, 01, 01), DateTime.Now);
            return 0;
        }


        public async Task<int> FullCardsSync()
        {
            var service = new RewardCardServices(db);
            var ok = await service.SyncAllRewardCardsAndInsertNew(new DateTime(2013, 01, 01), DateTime.Now, 0);


            return 0;
        }

        public async Task SyncNewRewardLogs()
        {
            var service = new RewardLogService();

            var lastLog = db.RewardCardLogs.OrderByDescending(x => x.created_date).FirstOrDefault().created_date;

            var cardLogs =
                await
                    service.GetRewardLogPointsFromWebservice(db.Brands.FirstOrDefault(x => x.name.Trim().ToLower().Equals(("Shoreditch Grind").Trim().ToLower())), lastLog,
                        DateTime.Now);

            var existingLogsInDb = db.RewardCardLogs
                .Where(x => x.created_date >= lastLog && x.created_date <= DateTime.Now).ToList();


            foreach (var log in cardLogs)
            {
                var isLogExistingALready = existingLogsInDb.FirstOrDefault(x => x.id == log.id);
                if (isLogExistingALready == null)
                {
                    db.RewardCardLogs.Add(log);

                }
            }

            db.SaveChanges();
        }


        public async Task<int> FullCustomerAndCardSync()
        {
            var service = new CustomerService(db);
            var ok = await service.GetAllCustomersAndInsertNew(new DateTime(2013, 01, 01), DateTime.Now);

            var rewardservice = new RewardCardServices(db);
            var rewardok = await rewardservice.SyncAllRewardCardsAndInsertNew(new DateTime(2013, 01, 01), DateTime.Now, 0);

            return 0;
        }


        public async Task<int> RecentCustomerAndCardSync()
        {
            using (var _db = new GrindContext())
            {
                var lastCust = _db.Customers.Max(x => x.CreatedDate);
                var lastCardID = _db.RewardsCardNew.Max(x => x.Revelid);


                var service = new CustomerService(_db);
                var ok = await service.GetAllCustomersAndInsertNew((DateTime)lastCust, DateTime.Now);

                var rewardservice = new RewardCardServices(_db);
                var rewardok = await rewardservice.SyncRewardCardsSinceLastCard(new DateTime(2013, 01, 01), DateTime.Now, lastCardID);
            }
            return 0;
        }


        public async Task<int> ResetAllRedCards()
        {

            var rewardservice = new RewardCardServices(db);
            var cardsReset = await rewardservice.ResetAllRedCards(db);

            return cardsReset.Count();

        }

        public async Task<bool> SaveDaysSinceLastVisitAndSaveYesterdaysTotalPoints()
        {

            var cards = db.RewardsCardNew.ToList();

            var rewardservice = new RewardCardServices(db);
            var dbwriter = new RevelDBWriter(db);

            await rewardservice.SaveDaysSinceLastVisit(cards, db: dbwriter);
            await rewardservice.SaveTotalPointsTodayasYesterdaysPoints(cards, db: dbwriter);

            return true;
        }

        public async Task SaveDaysSinceLastVisit()
        {
            var cards = db.RewardsCardNew.ToList();

            var rewardservice = new RewardCardServices(db);
            var dbwriter = new RevelDBWriter(db);

            await rewardservice.SaveDaysSinceLastVisit(cards, db: dbwriter);

            return;

        }

        public async Task<int> SaveYesterdaysTotalPoints()
        {
            var cards = db.RewardsCardNew.ToList();

            var rewardservice = new RewardCardServices(db);
            var dbwriter = new RevelDBWriter(db);

            var cardsUpdated = await rewardservice.SaveTotalPointsTodayasYesterdaysPoints(cards, db: dbwriter);

            return cardsUpdated;

        }

        public async Task<int> FullGiftCardSync()
        {
            var service = new GiftCardService(db);
            var ok = await service.SyncAllGiftCardsAndInsertNew(new DateTime(2013, 01, 01), DateTime.Now, 0);

            return 0;
        }



        public async Task<int> RefreshEstablishments()
        {

            var brand = db.Brands.First();

            var existingEstablishments = db.Establishments.Where(x => x.db_brand_id == brand.brand_id).ToList();


            var br = new Establishment(0, "", brand.key_secret, new Uri(brand.revel_base_url));

            var establishments = new List<Establishment>();
            using (var reader = new RevelWebserviceDataReader(br))
            {
                var instanceEstablishment = new Establishment();
                establishments = await reader.GetRevelWebserviceData<Establishment>(instanceEstablishment, instanceEstablishment.theAddress);
            }


            var establishmentsToAdd = new List<Establishment>();
            if (establishments.Count > 0)
            {
                var currentEsts = existingEstablishments;

                foreach (var est in establishments)
                {
                    //there isn't that establishment already in existence
                    if (currentEsts.Where(x => x.resource_uri == est.resource_uri).ToList().Count().Equals(0))
                    {
                        est.db_brand_id = brand.brand_id;
                        est.brand = brand.revel_base_url; //this is needed to map an establishment to a brand
                        est.is_fourth_active = true;
                        establishmentsToAdd.Add(est);
                    }
                }

                if (establishmentsToAdd.Count > 0)
                {
                    db.Establishments.AddRange(establishmentsToAdd);
                    await db.SaveChangesAsync();
                    return establishmentsToAdd.Count();
                }


            }

            return 0;
        }

        public async Task<int> Run3amRoutineWrapper()
        {
            var log = new ScheduledTaskLog();
            try
            {
                var sync = new SyncController();
                var ok = await sync.Run3amRoutine();

                //log this job
                log = new ScheduledTaskLog()
                {
                    //Detail = context.JobDetail.Dump(),
                    FireTime = DateTime.Now.ToUniversalTime(),
                    Message =
                        "3am Sync - Reset Red Card, Sync Customers/Cards, Multiply Points and Create Card Timestamps",
                    Result = 1,

                };

                using (var db = new GrindContext())
                {
                    db.ScheduledTaskLogs.Add(log);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                log = new ScheduledTaskLog()
                {
                    // Detail = context.JobDetail.Dump(),
                    FireTime = DateTime.Now.ToUniversalTime(),
                    Message =
                        "FAILED: 3am Sync - Reset Red Card, Sync Customers/Cards, Multiply Points and Create Card Timestamps" + ex.Message.ToString(),
                    Result = 0
                };

                var emailer = new EmailController();
                emailer.SyncFailed();

            }
            finally
            {

                using (var db = new GrindContext())
                {
                    db.ScheduledTaskLogs.Add(log);
                    db.SaveChanges();
                }
            }

            return 0;
        }

    }
}