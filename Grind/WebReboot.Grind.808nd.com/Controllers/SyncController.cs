using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.Logging;
using Revel._808nd.com.Classes.ServiceImplementaitons;
using Revel._808nd.com.Classes.WebserviceReaderImplementations;
using Revel._808nd.com.Classes.WebserviceWriter;
using Revel._808nd.com.Extensions;
using Revel._808nd.com.Interfaces;
using Revel._808nd.com.Models;


namespace Web.Grind._808nd.com.Controllers
{
    [Authorize(Roles = "admin")]
    public class SyncController : Controller
    {

        private GrindContext _db = new GrindContext();
        private string RevelAPIKEY { get; } = ConfigurationManager.AppSettings["RevelAPIKEY"];
        private string RevelBaseURL { get; } = ConfigurationManager.AppSettings["RevelBaseURL"];
        //
        // GET: /Sync/
        public SyncController()
        {
            _db = new GrindContext();
        }

        public async Task<int> FullCustomerSync()
        {
            var service = new CustomerService(_db);
            var ok = await service.GetAllCustomersAndInsertNew(new DateTime(2013, 01, 01), DateTime.Now);
            return 0;
        }


        public async Task<int> FullCardsSync()
        {
            var service = new RewardCardServices(_db);
            var ok = await service.SyncAllRewardCardsAndInsertNew(new DateTime(2013, 01, 01), DateTime.Now, 0);


            return 0;
        }

        public async Task<int> FullCustomerAndCardSync()
        {
            var service = new CustomerService(_db);
            var ok = await service.GetAllCustomersAndInsertNew(new DateTime(2013, 01, 01), DateTime.Now);

            var rewardservice = new RewardCardServices(_db);
            var rewardok = await rewardservice.SyncAllRewardCardsAndInsertNew(new DateTime(2013, 01, 01), DateTime.Now, 0);

            return 0;
        }


        public async Task<int> FullGiftCardSync(GrindContext _db)
        {

            var service = new GiftCardService(_db);
            var ok = await service.SyncAllGiftCardsAndInsertNew(new DateTime(2013, 01, 01), DateTime.Now, 0);


            return 0;
        }


        public async Task<int> RUnFullGiftCardSync()
        {

            var service = new GiftCardService(new GrindContext());
            var ok = await service.SyncAllGiftCardsAndInsertNew(new DateTime(2013, 01, 01), DateTime.Now, 0);


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




        public async Task<IEnumerable<IIdentifiable>> ResetAllRedCards(GrindContext _db)
        {

            var rewardservice = new RewardCardServices(_db);
            var cardsReset = await rewardservice.ResetAllRedCards(_db);


            if (cardsReset.Any())
            {
                foreach (var card in cardsReset)
                {
                    _db.Set<RewardsCardNew>().AddOrUpdate(card);
                    _db.Entry(card).State = EntityState.Modified;

                }
                _db.SaveChanges();

            }

            return cardsReset;

        }


        public async Task SaveDaysSinceLastVisit(GrindContext db)
        {

            var cards = db.RewardsCardNew.ToList();

            var rewardservice = new RewardCardServices(db);
            var dbwriter = new RevelDBWriter(db);

            await rewardservice.SaveDaysSinceLastVisit(cards, db: dbwriter);

            return;

        }

        public async Task RefreshProducts()
        {
            Establishment revOrg = new Establishment(1, "Grind",
             RevelAPIKEY,
             new Uri(RevelBaseURL));

            RevelFactory revelFactory = new RevelFactory(revOrg);

            IRevelReaderAsync webReader = new Revel._808nd.com.Classes.WebserviceReader.RevelWebserviceDataReader(revOrg);
            RevelDBWriter writer = new RevelDBWriter(_db);
            IRevelReaderAsync DBReader = new RevelDBReader(revOrg);


            bool ok = false;

            //cats          
            try
            {
                // var pcOk = await ProductCategory.CompareProductCategoriesAndInsertNewIntoDB(DBReader, webReader, writer);
                await Product.CompareProductsDeleteOldAndInsertNewIntoDB(DBReader, webReader, writer);

            }
            catch (Exception)
            {

                throw;
            }

        }


        public async Task<string> ManuallyCreateRewardCardDailyPointsForToday()
        {
            try
            {
                var service = new RewardCardDailyPointsService();
                var saved = await service.CreateRewardCardDailyPointsToday(new GrindContext(), DateTime.Now);

                return "Done - All points created";
            }
            catch (Exception ex)
            {
                return ex.Message + " - " + ex.InnerException;
                throw;
            }
        }



        public async Task<int> CreateRewardCardDailyPointsToday(GrindContext _db, DateTime todaysDate)
        {
            var service = new RewardCardDailyPointsService();
            var saved = await service.CreateRewardCardDailyPointsToday(_db, todaysDate);

            return saved;
        }

        public async Task<IEnumerable<RewardsCardNew>> HardLimitAllStaffCardsTo0Points(GrindContext grindContext)
        {
            List<RewardsCardNew> cardsUpdated = new List<RewardsCardNew>();
            Establishment revOrg = new Establishment(1, "Grind",
            RevelAPIKEY,
            new Uri(RevelBaseURL));

            RevelFactory revelFactory = new RevelFactory(revOrg);
            var writer = new WebserviceDataWriter(revOrg, db: _db);

            var startRange = 80000000;
            var endRange = 80001000;
            var rangeOfStringCardNumbers = new List<string>();

            for (int i = startRange; i <= endRange; i++)
            {
                rangeOfStringCardNumbers.Add(i.ToString());
            }

            var rangeOfCards = _db.RewardsCardNew
                .Where(x => rangeOfStringCardNumbers.Contains(x.number))
                .ToList();

            var messageString = "";
            foreach (var card in rangeOfCards)
            {
                card.current_points = 0;
                //reset
                var okRevel = await writer.UpdateRewardCard(card);
                if (okRevel == 0)
                {
                    cardsUpdated.Add(card);
                    messageString += card.number + ", ";
                }
            }

            //email out
            using (var mailservice = new EmailController())
            {
                mailservice.SendMessage("emailnadz@gmail.com", "Cards Limited To 0 Points", messageString);
            }
            return cardsUpdated;
        }


        public async Task<bool> Run3amRoutine()
        {
            var emailerFinish = new EmailController();
            var cardsReset = new List<ILoggableCollection>();
         
            var sync = new SyncController();
            var test = new Test1Controller();
            var rewardService = new RewardCardServices(_db);

            cardsReset.Add(new LoggableCollection(await sync.ResetAllRedCards(_db), RevelBaseURL, DateTime.Now, "Red Cards"));
            var messageCards = "3am Card routine red card reset ok";
            emailerFinish.CustomEmailMessage(messageCards);

            //weekly
            if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
            {
                var cardsToReset = await _db.RewardsCardNew.Where(x => x.LoyaltyCardType.id == 1 && x.Active == true).ToListAsync();
                var cardsToResetHardLimit = await _db.RewardsCardNew.Where(x => x.LoyaltyCardType.id == 7 && x.Active == true).ToListAsync();

                if (cardsToReset.Count > 0)
                {
                    var resetLoyaltyCardsWeekly = await rewardService.ResetInvestorCards(cardsToReset, _db);
                    var resestLoylatyCardHardLimit = await rewardService.ResetInvestorCardsHardLimit(cardsToResetHardLimit, _db);

                    cardsReset.Add(new LoggableCollection(resetLoyaltyCardsWeekly, RevelBaseURL, DateTime.Now, "Weekly Investor Cards"));
                    var emailer = new EmailController();
                    var message = String.Format("Weekly Investor cards reset:{0} succesfully", resetLoyaltyCardsWeekly.Count());
                    var message2 = String.Format("Weekly Hard Limit Wave 2 Investor cards reset:{0} succesfully", resestLoylatyCardHardLimit.Count());


                    emailer.CustomEmailMessage(message);
                    emailer.CustomEmailMessage(message2);
                }

            }

            if (DateTime.Now.Day == 1)
            {
                var cardsToReset = await _db.RewardsCardNew.Where(x => x.LoyaltyCardType.id == 2 && x.Active == true).ToListAsync();
                var resetLoyaltyCardsMonthly = await rewardService.ResetInvestorCards(cardsToReset, _db);

                cardsReset.Add(new LoggableCollection(resetLoyaltyCardsMonthly, RevelBaseURL, DateTime.Now, "Monthly Investor Cards"));
                var emailer = new EmailController();
                var message = String.Format("Monthly Investor cards reset:{0} succesfully", resetLoyaltyCardsMonthly);
                emailer.CustomEmailMessage(message);
            }


            var ok2 = await sync.FullGiftCardSync(_db);
            var createCardTimestamp = await sync.CreateRewardCardDailyPointsToday(_db, DateTime.Now.ToUniversalTime());
            await test.NewPointDoublingRouting();
            var messageFinishPointsDouble = "Points doubling routine complete";
            emailerFinish.CustomEmailMessage(messageFinishPointsDouble);

            var cardsUpdatedString = "";
            foreach (var loggableList in cardsReset)
            {
                cardsUpdatedString += loggableList.WhenLogged;
                cardsUpdatedString += loggableList.CollectionDescription + ". Cards:  ";
                foreach (var card in loggableList.TheCollection)
                {
                    cardsUpdatedString += card.Identifier + ", ";
                }
            }

            var messageFinish = String.Format("3am Card routine run ok :{0}. The red cards reset were " + cardsUpdatedString, ConfigurationManager.AppSettings["RevelBaseURL"]);
            emailerFinish.CustomEmailMessage(messageFinish);
        
            rewardService.SetAllExpiredRedCards();

            await sync.HardLimitAllStaffCardsTo0Points(_db); //hard limit 1000 staff cards each night
            await sync.SaveDaysSinceLastVisit(_db);
            //return a list of ILoggable collection for each item that was reset in the routine

            return true;
        }





        public async Task<int> RunPointsMultiplierForSelectedEmailsSinceLastMultiplier(GrindContext _db)
        {
            var est = new Establishment(1, "Grind",
            RevelAPIKEY,
             new Uri(RevelBaseURL));

            //make sure we've got today's data ready for tomorrow!!!!
            var cardServices = new RewardCardServices(_db);
            var pointsServices = new RewardCardDailyPointsService();
            var transactionLogService = new RewardCardPointsTransactionLogService();
            var writer = new WebserviceDataWriter(est, _db);

            var now = DateTime.Now.ToUniversalTime();
            var yesterday = now.AddDays(-1);

            var cards = _db.RewardsCardNew.ToList();
            var customers = _db.Customers.ToList();

            var transactionsToday = transactionLogService.GetTransactionsForADate(now, _db);

            //get most recent date
            var maxStamp = _db.RewardsCardDailyPoints.Where(x => x.date != null).Max(x => x.date);
            var maxStampStart = new DateTime(maxStamp.Year, maxStamp.Month, maxStamp.Day, 00, 00, 00);
            var maxStampFinish = new DateTime(maxStamp.Year, maxStamp.Month, maxStamp.Day, 23, 59, 59);

            var mostRecentTimestamps =
                _db.RewardsCardDailyPoints.Where(x => x.date > maxStampStart && x.date < maxStampFinish).ToList();

            var dbReader = new RevelDBReader(est);

            var multipliers = _db.RewardsPointsMultiplier.ToList();

            //potentially the cards that need multiplying
            var cardstoMultiply = await cardServices.GetCardsWithMultipliers(cards, customers, multipliers, _db);
            cardstoMultiply = cardstoMultiply.Where(x => x.is_vip_card != true).Where(x => x.LoyaltyCardType == null).ToList(); //no VIP cards



            var cardsToMultiplyBeforeModding = new List<RewardsCardNew>();

            foreach (var rewardsCardNew in cardstoMultiply)
            {

                cardsToMultiplyBeforeModding.Add(new RewardsCardNew()
                {
                    current_points = rewardsCardNew.current_points,
                    total_points = rewardsCardNew.total_points,
                    Revelid = rewardsCardNew.Revelid,
                    number = rewardsCardNew.number
                });
            }



            //check we've got a previous stamp to check aagainst, oftherwise we can forget the whole thing


            //see what we haven't got a transaction for for the date we're doing
            var cardsThatHaventBeenDoubledTodayAlready = new List<RewardsCardNew>();
            var cardErrors = new List<RewardsCardNew>();
            foreach (var rewardsCardNew in cardstoMultiply)
            {
                try
                {


                    var stampForthisCard =
                               mostRecentTimestamps.FirstOrDefault(x => x.card_number.ToLower() == rewardsCardNew.number.ToLower());

                    RewardsCardDailyPoints test = new RewardsCardDailyPoints();
                    test = stampForthisCard;

                    if (test != null)
                    {
                        //check we haven't done a transaction today already
                        var transaction = transactionsToday.Where(x => x.WhenCreated.Date == now.Date)
                            .Where(x => x.card_number.ToLower().Trim() == rewardsCardNew.number.ToLower().Trim())
                            .FirstOrDefault();

                        if (transaction == null)
                        {
                            //check if the points are different since last point in time, if so add to list for doubling
                            var pointsLasttime =
                                mostRecentTimestamps.Where(x => x.card_number.ToLower() == rewardsCardNew.number.ToLower())
                                    .FirstOrDefault();

                            if (rewardsCardNew.total_points != pointsLasttime.total_points_on_date)
                            {
                                cardsThatHaventBeenDoubledTodayAlready.Add(rewardsCardNew);
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    //log this card
                    cardErrors.Add(rewardsCardNew);
                }
            }

            //run the routine
            var cardsAfterMultiplication =
                await
                    cardServices.DoubleCardPointsSinceTimestamps(multipliers, mostRecentTimestamps, cardsThatHaventBeenDoubledTodayAlready,
                        dbReader, _db);



            var logs = new List<RewardCardPointsTransactionLog>();

            foreach (var rewardsCardNew in cardsAfterMultiplication)
            {


            }
            //CHECK THERE HASN'T BEEN AN UPDATE ALREADY TODAY

            //do the revel update
            if (cardsAfterMultiplication.Count > 0)
            {
                foreach (RewardsCardNew rewardsCardNew in cardsAfterMultiplication)
                {
                    if (await writer.UpdateRewardCard(rewardsCardNew) == 0)
                    {
                        try
                        {
                            var oldCard =
                             cardsToMultiplyBeforeModding.FirstOrDefault(
                                 x => x.Revelid == rewardsCardNew.Revelid);

                            logs.Add(transactionLogService.CreateRewardCardPointsTransactionLog(now, oldCard, rewardsCardNew));

                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Error updating", ex);

                        }

                    }
                }

                var cardUpdate = _db.SaveChanges();
                //save into local DB

                //log all transactions
                ((DbSet<RewardCardPointsTransactionLog>)_db.RewardCardPointsTransactionLogs).AddRange(logs);
                var ok = _db.SaveChanges();
            }

            return cardsAfterMultiplication.Count;



            return 0;
        }



        public async Task<int> RunPointsMultiplierForSelectedEmails()
        {
            var est = new Establishment(1, "Grind",
           RevelAPIKEY,
            new Uri(RevelBaseURL));

            //make sure we've got today's data ready for tomorrow!!!!
            var cardServices = new RewardCardServices(_db);
            var pointsServices = new RewardCardDailyPointsService();
            var transactionLogService = new RewardCardPointsTransactionLogService();
            var writer = new WebserviceDataWriter(est, _db);

            var now = DateTime.Now.ToUniversalTime();
            var yesterday = now.AddDays(-1);

            var cards = _db.RewardsCardNew.ToList();
            var customers = _db.Customers.ToList();
            var transactionsToday = transactionLogService.GetTransactionsForADate(now, _db);


            var yesterdayStart = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 00, 00, 00);
            var yesterdayFinish = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 23, 59, 59);


            var yesterdaysTimestamps =
                _db.RewardsCardDailyPoints.Where(x => x.date > yesterdayStart && x.date < yesterdayFinish).ToList();

            var dbReader = new RevelDBReader(est);

            var multipliers = _db.RewardsPointsMultiplier.ToList();

            //potentially the cards that need multiplying
            var cardstoMultiply = await cardServices.GetCardsWithMultipliers(cards, customers, multipliers, _db);
            cardstoMultiply = cardstoMultiply.Where(x => x.is_vip_card != true).ToList(); //no VIP cards

            var cardsToMultiplyBeforeModding = new List<RewardsCardNew>();

            //create this for logs, need seperate cloned objects with original values
            foreach (var rewardsCardNew in cardstoMultiply)
            {

                cardsToMultiplyBeforeModding.Add(new RewardsCardNew()
                {
                    current_points = rewardsCardNew.current_points,
                    total_points = rewardsCardNew.total_points,
                    Revelid = rewardsCardNew.Revelid,
                    number = rewardsCardNew.number
                });
            }


            //check we've got a previous stamp to check aagainst, oftherwise we can forget the whole thing



            //see what we haven't got a transaction for for the date we're doing
            var cardsThatHaventBeenDoubledTodayAlready = new List<RewardsCardNew>();

            foreach (var rewardsCardNew in cardstoMultiply)
            {
                var stampForthisCard =
                    yesterdaysTimestamps.FirstOrDefault(x => x.card_number.ToLower() == rewardsCardNew.number.ToLower());

                RewardsCardDailyPoints test = new RewardsCardDailyPoints();
                test = stampForthisCard;

                if (test != null)
                {
                    var transaction = transactionsToday.Where(x => x.WhenCreated.Date == now.Date)
                        .Where(x => x.card_number.ToLower().Trim() == rewardsCardNew.number.ToLower().Trim())
                        .FirstOrDefault();

                    if (transaction == null)
                    {
                        //check if the points are different since last point in time, if so add to list for doubling
                        var pointsYesterday =
                            yesterdaysTimestamps.Where(x => x.card_number.ToLower() == rewardsCardNew.number.ToLower())
                                .FirstOrDefault();

                        if (rewardsCardNew.total_points != pointsYesterday.total_points_on_date)
                        {
                            cardsThatHaventBeenDoubledTodayAlready.Add(rewardsCardNew);
                        }
                    }

                }



            }




            //run the routine
            var cardsAfterMultiplication =
                await
                    cardServices.DoubleCardPointsSinceTimestamps(multipliers, yesterdaysTimestamps, cardsThatHaventBeenDoubledTodayAlready,
                        dbReader, _db);



            var logs = new List<RewardCardPointsTransactionLog>();

            foreach (var rewardsCardNew in cardsAfterMultiplication)
            {


            }
            //CHECK THERE HASN'T BEEN AN UPDATE ALREADY TODAY

            //do the revel update

            if (cardsAfterMultiplication.Count > 0)
            {
                foreach (RewardsCardNew rewardsCardNew in cardsAfterMultiplication)
                {
                    if (await writer.UpdateRewardCard(rewardsCardNew) == 0)
                    {
                        try
                        {
                            var oldCard =
                             cardsToMultiplyBeforeModding.FirstOrDefault(
                                 x => x.Revelid == rewardsCardNew.Revelid);

                            logs.Add(transactionLogService.CreateRewardCardPointsTransactionLog(now, oldCard, rewardsCardNew));

                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Error updating", ex);

                        }

                    }
                }

                var cardUpdate = _db.SaveChanges();
                //save into local DB

                //log all transactions
                ((DbSet<RewardCardPointsTransactionLog>)_db.RewardCardPointsTransactionLogs).AddRange(logs);
                var ok = _db.SaveChanges();
            }

            return cardsAfterMultiplication.Count;

            return 0;

        }


    }




}