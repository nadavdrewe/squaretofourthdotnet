using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Revel._808nd.com.Classes.Logging;
using Revel._808nd.com.Classes.WebserviceReader;
using Revel._808nd.com.Interfaces;
using Revel._808nd.com.Models;
using Revel._808nd.com.Classes.WebserviceReaderImplementations;

namespace Revel._808nd.com.Classes.ServiceImplementaitons
{
    public class RewardCardServices
    {
        private RevelContextBase _db { get; set; }
        private string RevelAPIKEY { get; } = ConfigurationManager.AppSettings["RevelAPIKEY"];
        private string RevelBaseURL { get; } = ConfigurationManager.AppSettings["RevelBaseURL"];
        private string RevelCardInsertUser { get; } = ConfigurationManager.AppSettings["RevelCardInsertUser"];


        public RewardCardServices(RevelContextBase db)
        {
            _db = db;
        }

        public async Task<RewardsPointsMultiplier> GetMultiplierForCard(RewardsCardNew card, RevelDBReader dbRevelDbReader)
        {
            var customers = await dbRevelDbReader.GetRevelType<Customer>();
            var _customerService = new CustomerService(_db);

            Customer customer = null;
            {
                //try match on both Revel keys
                customer = await GetCorrectCustomerForCard(_customerService, card);

            }

            var theMultipliers = await dbRevelDbReader.GetRevelType<RewardsPointsMultiplier>();

            if (customer != null)
            {
                var suffix = customer.Email.Split('@')[1];

                var multiplier = theMultipliers.FirstOrDefault(x => x.emailSuffix == suffix);

                if (multiplier != null)
                {
                    return multiplier;
                }

            }
            else
            {
                throw new Exception("Couldn't find a customer or a multipler in GetMultiplierForCard()");

            }
            //create null object
            return new RewardsPointsMultiplier()
            {
                active = true,
                emailSuffix = "",
                multiplier = 0
            };

        }





        public async Task<List<RewardsCardNew>> GetCardsWithMultipliers(List<RewardsCardNew> cards, List<Customer> customers, List<RewardsPointsMultiplier> theMultipliers, GrindContext db)
        {

            var cardsWithMultipliers = new List<RewardsCardNew>();
            var _customerService = new CustomerService(_db);


            foreach (var card in cards)
            {
                try
                {
                    Customer customer = null;
                    customer = await GetCorrectCustomerForCard(_customerService, card);


                    //we should have a customer here
                    if (customer != null && !String.IsNullOrWhiteSpace(customer.Email) && customer.Email.Contains('@'))
                    {
                        var suffix = "";

                        try
                        {
                            suffix = customer.Email.Split('@')[1];
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Couldn't get the email suffix", ex);

                        }

                        //not expired multipliers - if so don't add card
                        var multiplier = theMultipliers.Where(x => x.expiryDate >= DateTime.Now).FirstOrDefault(x => x.emailSuffix.ToLower().Trim() == suffix.ToLower().Trim());

                        if (multiplier != null)
                        {

                            cardsWithMultipliers.Add(card);

                        }

                    }
                }
                catch (Exception exc)
                {

                    throw new Exception("there has been a a problem with the GetCardsWithMultipliers method", exc);
                }
            }


            return cardsWithMultipliers;
        }


        private async Task<Customer> GetCorrectCustomerForCard(CustomerService _customerService, RewardsCardNew card)
        {
            Customer customer = null;
            if (card != null)
            {
                //try match on both Revel keys
                if (!String.IsNullOrWhiteSpace(card.customer_revel))
                {

                    try
                    {
                        customer = await _customerService.GetFromRevelCustomerURL(card.customer_revel);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("GetCorrectMultiplierForCard() has failed to get a customer", ex);
                    }
                }
                if (customer == null)
                {
                    customer = _db.Customers.FirstOrDefault(x => x.LicNumber.Trim() == card.number.Trim());
                }
            }

            return customer;
        }

        public async Task<List<RewardsCardNew>> DoubleCardPointsSinceTimestamps(List<RewardsPointsMultiplier> theMultipliers,
            List<RewardsCardDailyPoints> pointsTimestamps, List<RewardsCardNew> cards, RevelDBReader dbReader, GrindContext db)
        {

            var cardsToReturn = new List<RewardsCardNew>();

            var pointsService = new RewardCardDailyPointsService();
            var yesterdaysPoints = pointsTimestamps;


            /*   var cardImTestng = cards.First(x => x.number == "00026808");*/

            foreach (var rewardsCardNew in cards)
            {




                var yesterdayCard =
                    yesterdaysPoints.FirstOrDefault(
                        x => x.card_number.ToLower().Trim() == rewardsCardNew.number.ToLower().Trim());

                //if we don't have a comparison we can't work out the new points
                if (yesterdayCard != null)
                {
                    //THIS DOESN'T WORK
                    var multiplier = await GetMultiplierForCard(rewardsCardNew, dbReader);


                    var originalPoints = 0;
                    var pointsAdded = 0;
                    var newPointsTotal = 0;

                    //if we can find a multiplier, do the maths
                    if (multiplier.multiplier != 0)
                    {
                        //try and get yesterday's points total and compute the difference

                        //how many points have been added since yesterday? 
                        var pointsIncrease = rewardsCardNew.total_points - yesterdayCard.total_points_on_date;

                        if (pointsIncrease > 0)
                        {
                            pointsAdded = pointsIncrease * multiplier.multiplier;

                            rewardsCardNew.current_points += pointsAdded;
                            rewardsCardNew.total_points += pointsAdded;

                            cardsToReturn.Add(rewardsCardNew);
                        }
                    }

                }

            }

            return cardsToReturn;

        }

        public async Task<List<RewardsCardNew>> RunPointsMultiplierForSelectedEmailAddresses(List<RewardsPointsMultiplier> theMultipliers,
            RevelDBReader dbRevelDbReader)
        {
            var cardsWithMultipliers = new List<RewardsCardNew>();

            var cards = await dbRevelDbReader.GetRevelType<RewardsCardNew>();
            var customers = await dbRevelDbReader.GetRevelType<Customer>();

            foreach (var card in cards)
            {

                var customer = customers.Where(x => x.RefNumber == card.number).FirstOrDefault();

                if (customer != null)
                {
                    var suffix = customer.Email.Split('@');


                    foreach (var rewardsPointsMultiplier in theMultipliers)
                    {
                        if (rewardsPointsMultiplier.emailSuffix.ToLower() == suffix[1].ToLower())
                        {
                            //do the multiplication
                            var pointsSinceYesterday = card.total_points - card.yesterdaysTotalPoints;

                            //test 1
                            if (pointsSinceYesterday > 0 && pointsSinceYesterday != null)
                            //NEED TO CHECK THAT IT HASN'T ALREADY BEEN MULTIPLIED TODAY
                            //NEED TO CHECK IT'S NOT NULL THE MULTIPLIER
                            {
                                var pointsToAdd = (int)(pointsSinceYesterday * rewardsPointsMultiplier.multiplier);

                                card.current_points += pointsToAdd;
                                card.total_points += pointsToAdd;
                                card.pointsMultiplierLastRun = DateTime.Now;


                            }

                        }



                    }


                }

                return cardsWithMultipliers;
            }

            throw new Exception();
        }





        private async Task<int> CalculateDaysSinceLastVisit(RewardCardLog log)
        {
            var daysSinceVisit = 0;
            var now = DateTime.Now;

            var daysSinceVisitTimespan = now - log.created_date;
            daysSinceVisit = daysSinceVisitTimespan.Days;

            return daysSinceVisit;
        }

        private async Task<int> CalculateDaysSinceLastVisit(RewardsCardNew card)
        {
            var daysSinceVisit = 0;
            var now = DateTime.Now;

            var daysSinceVisitTimespan = now - card.updated_date;
            daysSinceVisit = daysSinceVisitTimespan.Days;

            return daysSinceVisit;
        }

        public async Task<int> SaveTotalPointsTodayasYesterdaysPoints(List<RewardsCardNew> cards, RevelDBWriter db)
        {
            List<RewardsCardNew> cardsToUpdate = new List<RewardsCardNew>();

            try
            {
                foreach (var card in cards)
                {
                    card.yesterdaysTotalPoints = card.total_points;
                    card.yesterdaysTotalPointsWhenCreated = DateTime.Now;
                    cardsToUpdate.Add(card);
                }


                var ok = await db.UpdateRevelType(cards);

                if (ok > 0)
                {
                    return ok;
                }

                return -1;

            }
            catch (Exception ex)
            {

                throw new Exception("Updating days since last visit was unsuccessful", ex);
            }



        }

        public async Task SaveDaysSinceLastVisit(List<RewardsCardNew> cards, RevelDBWriter db)
        {
            List<RewardsCardNew> cardsThatHaveNeverVisited = new List<RewardsCardNew>();
            List<RewardsCardNew> cardsToUpdate = new List<RewardsCardNew>();

            try
            {

                //REMOVE ID
                foreach (var card in cards)
                {
                    var lastLog = _db.RewardCardLogs.Where(x => x.reward_card_id == card.Revelid).OrderByDescending(x => x.created_date).FirstOrDefault();


                    //id no loG USE UPDATED
                    if (lastLog != null)
                    {
                        card.days_since_last_visit = await CalculateDaysSinceLastVisit(lastLog);
                        cardsToUpdate.Add(card);

                    }
                    else
                    {
                        cardsThatHaveNeverVisited.Add(card);
                        card.days_since_last_visit = await CalculateDaysSinceLastVisit(card);
                        cardsToUpdate.Add(card);
                    }
                }

                if (cardsToUpdate.Any())
                {
                    var ok = await db.UpdateRevelType(cardsToUpdate);
                }

                return;


            }
            catch (Exception ex)
            {

                throw new Exception("Updating days since last visit was unsuccessful", ex);
            }



        }


        public async Task<int> SyncRewardCardsSinceLastCard(DateTime start, DateTime end, int lastRewardsCard)
        {
            var revOrg = new Establishment(1, "Grind",
                RevelAPIKEY,
                 new Uri(RevelBaseURL));

            RevelWebserviceDataReader webReader = new RevelWebserviceDataReader(revOrg);
            RevelDBWriter writer = new RevelDBWriter(_db);
            RevelDBReader DBReader = new RevelDBReader(revOrg);


            //////////////////
            //ADDNEW
            //////////////////

            var cardAsType = new RewardsCardNew();
            List<RewardsCardNew> existingRewardsCardNews = await DBReader.GetRevelType<RewardsCardNew>();
            List<RewardsCardNew> webServiceexistingRewardsCardNews = await webReader.GetRevelWebserviceData(cardAsType,
                 String.Format(cardAsType.theAddress, start.ToString("yyyy-MM-ddTHH:mm:ss"),
                    end.ToString("yyyy-MM-ddTHH:mm:ss")));

            IEnumerable<int> RewardsCardNewIDsToInsert;

            GetNewRewardsCardNewIDs(existingRewardsCardNews, webServiceexistingRewardsCardNews, out RewardsCardNewIDsToInsert);

            //does this work????


            List<RewardsCardNew> RewardsCardNewsToInsert = new List<RewardsCardNew>();

            foreach (var item in RewardsCardNewIDsToInsert)
            {
                RewardsCardNew RewardsCardNewToInsert = webServiceexistingRewardsCardNews.Where(c => c.Revelid == item).FirstOrDefault();
                RewardsCardNewsToInsert.Add(RewardsCardNewToInsert);
            }

            var howMany = writer.SaveRevelType(RewardsCardNewsToInsert);



            return 0;

        }



        public async Task<int> SyncAllRewardCardsAndInsertNew(DateTime start, DateTime end, int lastRewardsCard)
        {

            var revOrg = new Establishment(1, "Grind",
                          RevelAPIKEY,
                           new Uri(RevelBaseURL));

            RevelWebserviceDataReader webReader = new RevelWebserviceDataReader(revOrg);
            RevelDBWriter writer = new RevelDBWriter(_db);
            RevelDBReader DBReader = new RevelDBReader(revOrg);


            //////////////////
            //ADDNEW
            //////////////////

            var cardAsType = new RewardsCardNew("/resources/RewardsCardNew?format=json&limit=0");
            List<RewardsCardNew> existingRewardsCardNews = await DBReader.GetRevelType<RewardsCardNew>();
            List<RewardsCardNew> webServiceexistingRewardsCardNews = await webReader.GetRevelWebserviceData(
                cardAsType,
                 String.Format(cardAsType.theAddress, start.ToString("yyyy-MM-ddTHH:mm:ss"),
                    end.ToString("yyyy-MM-ddTHH:mm:ss"))

                );

            var cardswithCustomerURLs = webServiceexistingRewardsCardNews
                .Where(x => x.customer_revel != null).ToList();
            //update all customer urls
            var CardsWithURLTOUpdate = new List<RewardsCardNew>();
            foreach (var cardWURL in cardswithCustomerURLs)
            {
                var existingCard = existingRewardsCardNews.FirstOrDefault(x => x.Revelid == cardWURL.Revelid);

                if (existingCard != null)
                {
                    if (existingCard.customer_revel != cardWURL.customer_revel)
                    {
                        existingCard.customer_revel = cardWURL.customer_revel;
                        CardsWithURLTOUpdate.Add(existingCard);
                    }


                }
            }

            if (CardsWithURLTOUpdate.Any())
            {
                var updated = await writer.UpdateRevelType(CardsWithURLTOUpdate);
            }


            IEnumerable<int> RewardsCardNewIDsToInsert;

            GetNewRewardsCardNewIDs(existingRewardsCardNews, webServiceexistingRewardsCardNews, out RewardsCardNewIDsToInsert);

            //does this work????


            List<RewardsCardNew> RewardsCardNewsToInsert = new List<RewardsCardNew>();

            foreach (var item in RewardsCardNewIDsToInsert)
            {
                RewardsCardNew RewardsCardNewToInsert = webServiceexistingRewardsCardNews.Where(c => c.Revelid == item).FirstOrDefault();
                RewardsCardNewsToInsert.Add(RewardsCardNewToInsert);
            }

            var howMany = writer.SaveRevelType(RewardsCardNewsToInsert);


            /////////////////
            //sync points
            /////////////////
            var cardToUpdate = new List<RewardsCardNew>();



            //reget the cards with new ones included

            existingRewardsCardNews = await DBReader.GetRevelType<RewardsCardNew>();

            foreach (var card in webServiceexistingRewardsCardNews)
            {


                //get the same record from DB
                var dbCard = existingRewardsCardNews.Find(x => x.Revelid == card.Revelid);
                var dbCardBackup = dbCard;

                //compare points, total visits, ermmm
                //if it's different update the points
                if (!dbCard.current_points.Equals(card.current_points)
                    || (!dbCard.total_visits.Equals(card.total_visits))
                    || (!dbCard.total_purchases.Equals(card.total_purchases))
                    || (!dbCard.number.Equals(card.number))
                    || (!dbCard.total_points.Equals(card.total_points))
                    || (!dbCard.Revelid.Equals(card.Revelid))
                    || (!dbCard.resource_uri.Equals(card.resource_uri))

                    )
                {


                    //what do we need to update from REVEL - POINTS, TOTAL POINTS, 
                    dbCard.Revelid = card.Revelid;
                    dbCard.current_points = card.current_points;
                    dbCard.total_points = card.total_points;
                    dbCard.total_visits = card.total_visits;
                    dbCard.total_purchases = card.total_purchases;
                    dbCard.ResourceUri = card.ResourceUri;
                    dbCard.created_by = card.created_by;
                    dbCard.created_date = card.created_date;
                    dbCard.updated_date = card.updated_date;
                    dbCard.customer_revel = card.customer_revel;
                    dbCard.payment_type = card.payment_type;
                    dbCard.establishment = card.establishment;


                    cardToUpdate.Add(dbCard);
                }

            }

            //do the update
            try
            {
                if (cardToUpdate.Any())
                {
                    var updated = await writer.UpdateRevelType(cardToUpdate);
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

            //log these transactions


            return 0;
        }




        public static IEnumerable<int> GetNewRewardsCardNewIDs(List<RewardsCardNew> existingRewardsCardNews, List<RewardsCardNew> webServiceexistingRewardsCardNews,
       out IEnumerable<int> webServiceRewardsCardNewIDs)
        {
            List<int> existingRewardsCardNewIDs = (from RewardsCardNews in existingRewardsCardNews
                                                   select (int)RewardsCardNews.Revelid).ToList();


            List<int> webRewardsCardNewIDs = (from RewardsCardNews in webServiceexistingRewardsCardNews
                                              select (int)RewardsCardNews.Revelid).ToList();

            var test = webRewardsCardNewIDs.Except(existingRewardsCardNewIDs);

            webServiceRewardsCardNewIDs = test;

            return webServiceRewardsCardNewIDs;


        }


        public async Task<List<IIdentifiable>> ResetInvestorCardsHardLimit(List<RewardsCardNew> cards, GrindContext db)
        {

            var est = new Establishment(1, "Grind",
                RevelAPIKEY,
                 new Uri(RevelBaseURL));


            var writer = new WebserviceDataWriter(est, db: (GrindContext)db);

            List<IIdentifiable> cardsUpdated = new List<IIdentifiable>();


            foreach (var rewardsCardNew in cards)
            {
                //set up variables
                var pointsToRefresh = rewardsCardNew.vip_points_refresh;

                var okRevel = -1;
                var oldCurrent = rewardsCardNew.current_points;
                var oldTotal = rewardsCardNew.total_points;


                //SET STANDARD VARS
                rewardsCardNew.vip_points_last_refreshed = DateTime.Now;
                rewardsCardNew.updated_date = DateTime.Now;
                rewardsCardNew.updated_by = RevelCardInsertUser;
                rewardsCardNew.created_by = RevelCardInsertUser;

                //GREEN PATH

                //either way, just add the points
                rewardsCardNew.current_points = pointsToRefresh;
                rewardsCardNew.total_points += pointsToRefresh;

                okRevel = await writer.UpdateRewardCard(rewardsCardNew);

                if (okRevel == 0)
                {
                    cardsUpdated.Add(rewardsCardNew);
                    //save the logs
                    _db.RewardCardPointsTransactionLogs.Add(new RewardCardPointsTransactionLog
                    {
                        WhenCreated = DateTime.Now,
                        card_number = rewardsCardNew.number,
                        orginal_points_current = oldCurrent,
                        orginal_points_total = oldTotal,
                        multiplier = 0,
                        new_points_current = rewardsCardNew.current_points,
                        new_points_total = rewardsCardNew.total_points,
                        pointsAdded = pointsToRefresh,
                        pointSetToRefreshInBucket = rewardsCardNew.vip_points_refresh
                    });

                    _db.RewardsCardNew.Attach(rewardsCardNew);
                    _db.Entry(rewardsCardNew).State = EntityState.Modified;

                }
                else
                {
                    //it failed, log this somewhere and call for help!!!
                    _db.ScheduledTaskLogs.Add(new ScheduledTaskLog
                    {
                        Message = "Investor card failed to refresh - Couldn't update in Revel. Check detail for card number",
                        Detail = rewardsCardNew.number,
                        FireTime = DateTime.Now
                    });
                }

                _db.SaveChanges();
            }

            return cardsUpdated;

        }



        public async Task<List<IIdentifiable>> ResetInvestorCards(List<RewardsCardNew> cards, GrindContext db)
        {

            var est = new Establishment(1, "Grind",
                RevelAPIKEY,
                 new Uri(RevelBaseURL));


            var writer = new WebserviceDataWriter(est, db: (GrindContext)db);
            List<IIdentifiable> cardsUpdated = new List<IIdentifiable>();


            foreach (var rewardsCardNew in cards)
            {


                //set up variables
                var pointsToRefresh = rewardsCardNew.vip_points_refresh;


                var okRevel = -1;
                var oldCurrent = rewardsCardNew.current_points;
                var oldTotal = rewardsCardNew.total_points;

                if (pointsToRefresh != 0)
                {
                    //SET STANDARD VARS
                    rewardsCardNew.vip_points_last_refreshed = DateTime.Now;
                    rewardsCardNew.updated_date = DateTime.Now;
                    rewardsCardNew.updated_by = RevelCardInsertUser;
                    rewardsCardNew.created_by = RevelCardInsertUser;

                    //GREEN PATH
                    if (rewardsCardNew.current_points <= 100)
                    {

                        //either way, just add the points
                        rewardsCardNew.current_points += pointsToRefresh;
                        rewardsCardNew.total_points += pointsToRefresh;

                    } //AMBER PATH
                    else if (rewardsCardNew.current_points > 100 && rewardsCardNew.current_points <= 120)
                    {
                        var pointsToAdd = (120 - rewardsCardNew.current_points);
                        pointsToRefresh = pointsToAdd;
                        rewardsCardNew.current_points += pointsToAdd;
                        rewardsCardNew.total_points += pointsToAdd;
                    } //RED PATH
                    else if (rewardsCardNew.current_points > 120)
                    {
                        rewardsCardNew.current_points = 120;
                        pointsToRefresh = 0;
                    }

                    okRevel = await writer.UpdateRewardCard(rewardsCardNew);
                }
                else //don't need to do an update
                {
                    okRevel = 0;
                }


                if (okRevel == 0)
                {
                    cardsUpdated.Add(rewardsCardNew);
                    //save the logs
                    _db.RewardCardPointsTransactionLogs.Add(new RewardCardPointsTransactionLog
                    {
                        WhenCreated = DateTime.Now,
                        card_number = rewardsCardNew.number,
                        orginal_points_current = oldCurrent,
                        orginal_points_total = oldTotal,
                        multiplier = 0,
                        new_points_current = rewardsCardNew.current_points,
                        new_points_total = rewardsCardNew.total_points,
                        pointsAdded = pointsToRefresh,
                        pointSetToRefreshInBucket = rewardsCardNew.vip_points_refresh

                    });

                    _db.RewardsCardNew.Attach(rewardsCardNew);
                    _db.Entry(rewardsCardNew).State = EntityState.Modified;


                }
                else
                {
                    //it failed, log this somewhere and call for help!!!
                    _db.ScheduledTaskLogs.Add(new ScheduledTaskLog
                    {
                        Message = "Investor card failed to refresh - Couldn't update in Revel. Check detail for card number",
                        Detail = rewardsCardNew.number,
                        FireTime = DateTime.Now
                    });
                }

                _db.SaveChanges();
            }

            return cardsUpdated;

        }

        public void SetAllExpiredRedCards()
        {

            try
            {
                var expService = new ExpiryNoficationService();


                var activeCards = this._db.RewardsCardNew.Where(x => x.is_vip_card == true && x.Active == true).ToList();
                var redCardsExpired = expService.GetInstancesPastExpiryDate(activeCards) as List<RewardsCardNew>;

                if (redCardsExpired.Any())
                {
                    foreach (var card in redCardsExpired)
                    {

                        card.Active = false;
                        this._db.RewardsCardNew.Attach(card);
                        _db.Entry(card).State = EntityState.Modified;

                    }

                    var log = new ScheduledTaskLog()
                    {
                        // Detail = context.JobDetail.Dump(),
                        FireTime = DateTime.Now.ToUniversalTime(),
                        Message =
                       "SUCCESS: Red Card Set Expired Cards: No of cards:" + redCardsExpired.Count(),
                        Result = 0,

                    };

                    foreach (var card in redCardsExpired)
                    {
                        log.Detail += card.number + ", ";
                    }

                    _db.ScheduledTaskLogs.Add(log);
                    _db.SaveChanges();

                    expService.NotifyExpired(redCardsExpired, "<h1>Hi David, the following red cards have expired and have been disabled:</h1>", "Grind: Red Cards have expired", new List<string> { "emailnadz@gmail.com", "david@grindandco.com" }, _db);

                }
            }
            catch (Exception ex)
            {
                var log = new ScheduledTaskLog()
                {
                    // Detail = context.JobDetail.Dump(),
                    FireTime = DateTime.Now.ToUniversalTime(),
                    Message =
                       "FAILED: Red Card Set Expired Cards" + ex.Message.ToString(),
                    Result = 0
                };


            }




        }

        public async Task<List<RewardsCardNew>> ResetAllRedCards(GrindContext _db)
        {

            var est = new Establishment(1, "Grind",
       RevelAPIKEY,
        new Uri(RevelBaseURL));

            var cards = _db.RewardsCardNew.Where(x => x.is_vip_card == true)
                .Where(x => x.Active == true)
                .ToList();


            var writer = new WebserviceDataWriter(est, db: _db);


            List<RewardsCardNew> cardsUpdated = new List<RewardsCardNew>();


            foreach (var rewardsCardNew in cards)
            {
                rewardsCardNew.current_points = rewardsCardNew.vip_points_refresh;
                rewardsCardNew.total_points += rewardsCardNew.vip_points_refresh;
                rewardsCardNew.vip_points_last_refreshed = DateTime.Now;
                rewardsCardNew.updated_date = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));

                var okRevel = await writer.UpdateRewardCard(rewardsCardNew);

                if (okRevel == 0)
                {
                    cardsUpdated.Add(rewardsCardNew);
                }

            }

            //return cards succeeded and cards failed - all you need is identifiers return interface

            return cardsUpdated;


        }

        public async Task<RewardsCardNew> GetByCustomerEmail(string customerEmail)
        {

            try
            {
                var cusList = _db.Customers
                    .Where(x => x.Email.Trim().ToLower().Equals(customerEmail.Trim().ToLower()))
                    // .Where(x => x.LoyaltyNumber != "")                        
                    .ToList();

                foreach (var customer in cusList)
                {
                    //match the first one that actually HAS a card
                    RewardsCardNew card = null;

                    if (customer != null)
                    {
                        var customerNumber = customer.LicNumber;

                        //try and get the card by URI
                        card = _db.RewardsCardNew.FirstOrDefault(x => x.customer_revel == customer.ResourceUri);

                        if (card != null)
                        {
                            card.Customer = customer;
                            return card;
                        }

                        //fallback and get it by number
                        if (customer.LicNumber != null)
                        {
                            card = _db.RewardsCardNew.FirstOrDefault(x => x.number == customerNumber);

                            if (card != null)
                            {
                                card.Customer = customer;
                                return card;
                            }
                        }
                    }


                }
                return null;
            }
            catch (Exception ex)
            {

                throw new Exception("Couldn't get the loyalty card by customer email", ex);
            }

        }



        public RewardsCardNew GetByNumber(string number)
        {
            //project card number original and trimmed
            //check every card against the trimmed number 
            //if match
            var allCards = _db.RewardsCardNew.AsQueryable();

            //try without strippping zeros
            foreach (var card in allCards)
            {
                try
                {
                    try
                    {
                        if (number == card.number)
                        { return card; }

                    }
                    catch (Exception)
                    {

                        if (card.number == number)
                        { return card; }
                    }
                }
                catch (Exception ex)
                {

                    throw new Exception("Customer Service - GetByNumber has errored out", ex);
                }



            }

            //IF WE CAN'T MATCH, STRIP 00s
            foreach (var card in allCards)
            {

                try
                {
                    try
                    {
                        var numberStrippedLeadingZeros = number.TrimStart('0');
                        var cardNumberTrimmed = card.number.TrimStart('0');
                        if (numberStrippedLeadingZeros == cardNumberTrimmed)
                        { return card; }

                    }
                    catch (Exception)
                    {

                        if (card.number == number)
                        { return card; }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Customer Service - GetByNumber has errored out", ex);
                }
            }


            return null;

        }


        /// <summary>
        /// Returns a positive or negative int depending on how many points have been added / spent since the last timestamp.
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetNumberOfPointsAddedSinceLastTimeStamp(RewardsCardNew card)
        {
            var numberOfPoints = 0;



            return numberOfPoints;
        }




    }
}
