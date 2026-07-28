using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime.Tree;
using Aspose.Cells;
using Revel._808nd.com.Classes;
using Web.Grind._808nd.com.Models;
using Web.Grind._808nd.com.Services;
using Web.Grind._808nd.com.Controllers;
using Web.Grind._808nd.MailChimp;
using WebGrease.Css.Extensions;
using Revel._808nd.com;
using FileFormatType = Aspose.Cells.GridWeb.Data.FileFormatType;
using Web.Grind._808nd.com.Services;
using Aspose.Cells;
using Aspose.Cells.GridWeb;
using Establishment = Revel._808nd.com.Classes.Establishment;
using GrindContext = Revel._808nd.com.Models.GrindContext;
using RewardsCardNew = Revel._808nd.com.Classes.RewardsCardNew;
using HtmlAgilityPack;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using CsvHelper;
using Revel._808nd.com.Classes.ServiceImplementaitons;
using Revel._808nd.com.Classes.WebserviceReader;
using Revel._808nd.com.Classes.WebserviceReaderImplementations;
using Revel._808nd.com.Classes.Reporting.ReportingFactory;
using LinqToExcel;
using Revel._808nd.com.Extensions;
using Revel._808nd.com.Interfaces;
using Slack._808nd.com.Classes;
using WebReboot.Grind._808nd.com.Models.Import;
using System.Diagnostics;
using Revel._808nd.com.Classes.Logging;
using Revel._808nd.com.Models;

namespace Web.Grind._808nd.com.Controllers
{


    [Authorize]
    public class Test1Controller : Controller
    {
        private GrindContext db = new GrindContext();
        private string RevelCardInsertUser { get; } = ConfigurationManager.AppSettings["RevelCardInsertUser"];
        private string RevelAPIKEY { get; } = ConfigurationManager.AppSettings["RevelAPIKEY"];
        private string RevelBaseURL { get; } = ConfigurationManager.AppSettings["RevelBaseURL"];

        public Test1Controller()
        {
            db = new GrindContext();
        }

        public async Task MyTest()
        {


        }


        public class BudgetModel
        {
            public string Grind { get; set; }
            public DateTime DateAndHour { get; set; }
            public string Budget { get; set; }

        }

        public async Task GenerateDansBudgets2018()
        {

            var ests = db.Establishments.Where(x => x.establishment_id != 2).ToList();

            var dateRanges = new List<DateTime>();
            var start = new DateTime(2019, 04, 01);
            var end = new DateTime(2020, 04, 26, 00, 00, 00);

            var current = start;

            while (current <= end)
            {
                dateRanges.Add(current);
                current = current.AddMinutes(15);
            }

            //gen csv
            ests.ForEach(x =>
            {
                List<BudgetModel> budgets = new List<BudgetModel>();
                dateRanges.OrderBy(d => d).ForEach(daterange =>
                  {
                      budgets.Add(new BudgetModel
                      {
                          Grind = x.name,
                          DateAndHour = daterange,
                          Budget = ""
                      });

                  });

                //OUTPUT 
                var genFilenaem = String.Format(@"c:\test\{0}_Budget2019_2020.csv", x.name);
                using (var writer = new StreamWriter(genFilenaem))
                {
                    var csv = new CsvWriter(writer);
                    //write a header row first
                    WriteHeaderRow(csv);
                    foreach (var item in budgets)
                    {
                        WriteCSVRow(csv, item);
                    }

                    csv.Flush();
                }


            });

        }

        private void WriteHeaderRow(CsvWriter csv)
        {
            csv.WriteField("Grind");
            csv.WriteField("Date");
            csv.WriteField("Budget");


            csv.NextRecord();
        }

        private void WriteCSVRow(CsvWriter csv, BudgetModel rowPoco)
        {

            csv.WriteField(rowPoco.Grind);
            csv.WriteField(rowPoco.DateAndHour.ToString("yyyy-MM-dd HH:mm:ss"));
            csv.WriteField(rowPoco.Budget);
            csv.NextRecord();

        }
        private IEnumerable<InvestorCardIImportSignup> ImportExcelSheetsAsCustomers()
        {
            //var excel = new ExcelQueryFactory(@"c:\test\investors.xlsx");
            IList<InvestorCardIImportSignup> customers = new List<InvestorCardIImportSignup>();

            //try
            //{

            //    var customerData = excel.Worksheet("investors")
            //        .ToList();



            //    foreach (var row in customerData)
            //    {
            //        customers.Add(new InvestorCardIImportSignup
            //        {
            //            FirstName = row["first"],
            //            Last = row["last"],
            //            CardNumber = row["number"],
            //            IntialPoints = 10
            //        });
            //    }
            //}
            //catch (Exception ex)
            //{

            //    throw;
            //}



            using (var sr = new StreamReader(@"c:\test\investors.csv"))
            {
                var reader = new CsvReader(sr);
                // reader.Configuration.IgnoreHeaderWhiteSpace = true;
                //CSVReader will now read the whole file into an enumerable
                var records = reader.GetRecords<InvestorCardIImportSignup>();
                customers = reader.GetRecords<InvestorCardIImportSignup>().ToList();
                //First 5 records in CSV file will be printed to the Output Window

            }

            return customers;
        }


        public async Task AddCardsFromExcel()
        {
            var excel = new ExcelQueryFactory(@"c:\test\grind.xlsx");

            var customerData = excel.Worksheet("Derwent")
                .Where(x => x["Number"] != null)
                .ToList();


            IList<Customer> customers = new List<Customer>();

            foreach (var row in customerData)
            {
                customers.Add(new Customer
                {
                    FirstName = row["First"],
                    LastName = row["Last"],
                    Email = row["Email"],
                    LicNumber = row["Number"],
                    Notes = row["Notes"]


                });
            }


        }

        public async Task RefreshEst()
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

        }


        //BUDGETS IMPORT

        class BudgetCSVImport
        {
            public int Grind { get; set; }
            public DateTime Date { get; set; }
            public Decimal Budget { get; set; }
        }

        public async Task Import2019Budgets()
        {
            using (var sr = new StreamReader(@"c:\test\budgetImport.csv"))
            {
                var reader = new CsvReader(sr);
                // reader.Configuration.IgnoreHeaderWhiteSpace = true;
                //CSVReader will now read the whole file into an enumerable
                var records = reader.GetRecords<BudgetCSVImport>();

                var budgets = records.Select(x => new Budget2019
                {
                    Amount = x.Budget,
                    BudgetDate = x.Date,
                    EstablishmentId = x.Grind
                }).ToList();

                //add to db
                using (var db = new BudgetContext())
                {

                    db.Budget2019s.AddRange(budgets);
                    db.SaveChanges();
                }


            }


            var DONE = "";
        }

        public async Task TestRewardCardLogStoresVisited()
        {
            //get all the cards

            /*       var allCards = RewardsCardNew.GetRewardCardsNewAndCustomer();*/

            var number = "11000051";

            var testCard = db.RewardsCardNew.Where(x => x.LoyaltyCardType != null).ToList();
            var testCustomers = db.Customers.AsNoTracking().ToList();

            foreach (var c in testCard)
            {
                c.Customer = RewardsCardNew.FindCustomerForCard(c, testCustomers);
            }


            var allCards = testCard;

            var cardsModified = new List<RewardsCardNew>();
            var ests = db.Establishments.AsNoTracking().ToList();
            //when did we last run one of these?

            //db.SystemLogs

            //GO through the wifi logons and update
            foreach (var card in allCards)
            {
                bool modified = false;
                if (!String.IsNullOrEmpty(card.Customer?.Email))
                {
                    //get current list of establishments visited
                    var estsVisited = card.GetStoresVisted();

                    var storesVisitedViaWifiLogs =
                        db.WifiLogins.AsNoTracking()
                            .Where(x => x.Email == card.Customer.Email && !String.IsNullOrEmpty(x.Site))
                             .Select(x => x.Site)
                            .Distinct()
                            .OrderBy(x => x)
                            .ToList();

                    var storesVisitedViaBlackCardLogs =
                        db.RewardCardLogs.Where(x => x.reward_card_id == card.Revelid)
                            .AsNoTracking()
                            .Select(x => x.establishment)
                            .Distinct()
                            .OrderBy(x => x)
                            .ToList();

                    foreach (var store in storesVisitedViaWifiLogs)
                    {
                        var theStore = store.ToLower().Trim();

                        var exist = estsVisited.FirstOrDefault(x => x.Equals(theStore));

                        if (exist == null)
                        {
                            card.AddNewStoreVisited(theStore);
                            modified = true;
                        }
                    }
                    //end logons


                    //update the stores vistied incase one was added above
                    estsVisited = card.GetStoresVisted();
                    foreach (var est in storesVisitedViaBlackCardLogs)
                    {
                        var theStore = ests.FirstOrDefault(x => x.resource_uri.ToLower().Trim() == est.ToLower().Trim())?.name;

                        if (!String.IsNullOrWhiteSpace(theStore))
                        {
                            var exist = estsVisited.FirstOrDefault(x => x.Equals(theStore.ToLower().Trim()));
                            if (exist == null)
                            {
                                card.AddNewStoreVisited(theStore);
                                modified = true;
                            }
                        }

                    }


                    if (modified)
                    {
                        cardsModified.Add(card);
                        db.Entry(card).State = EntityState.Modified;
                        //update the card and save it

                    }


                }
            }

            db.SaveChanges();



            //go through logs and do the same





            //see when we last ran one of these


        }



        public async Task<ActionResult> TestRewardCardLogSummaries()
        {
            var factory = new RewardCardLogsReportFactory();
            var summaries =
                factory.GetLogPointsVsOrdersForDateRange(new ReportContext
                {
                    StartDate = new DateTime(2016, 01, 01, 03, 00, 00),
                    EndDate = new DateTime(2016, 09, 15, 03, 00, 00)
                });

            return View(summaries);
        }

        public async Task<ActionResult> TestDataTablesNewTemplate()
        {
            return View();
        }
        //
        // GET: /Test/
        public async Task ResetAllRedCard()
        {
            var _db = new GrindContext();

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

            return;

        }
        public ActionResult TestDatePickers()
        {
            return View();
        }

        public ActionResult TestNewGrindBlackCardForm()
        {
            return View();
        }

        public async Task<bool> CreateAllInvestorCardsInRevelAndLocal()
        {
            var cardsToCreate = db.InvestorCardHolders.Where(x => x.HasBeenAdded == false).ToList();

            foreach (var card in cardsToCreate)
            {
                //check there isn't a card created
                var existingCard =
                    db.RewardsCardNew
                        .FirstOrDefault(x => x.number.Trim().ToLower() == card.CardNumber.Trim().ToLower());

                if (existingCard == null)
                {

                    var customer = new Customer
                    {
                        FirstName = card.FirstName,
                        LastName = card.LastName,
                        CreatedBy = RevelCardInsertUser,
                        UpdatedBy = RevelCardInsertUser,
                        Email = card.Email,
                        LicNumber = card.CardNumber.Trim().ToLower(),
                        LoyaltyNumber = card.CardNumber.Trim().ToLower(),
                        LoyaltyRefId = card.CardNumber.Trim().ToLower(),
                        RefNumber = card.CardNumber.Trim().ToLower(),
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                    };

                    var rewardscardnew = new RewardsCardNew
                    {
                        number = card.CardNumber.Trim().ToLower(),
                        created_by = RevelCardInsertUser,
                        created_date = DateTime.Now,
                        current_points = Convert.ToInt32(card.InitialLoad),
                        total_points = Convert.ToInt32(card.InitialLoad),
                        total_purchases = 0,
                        total_visits = 0,
                        is_vip_card = false,
                        establishment = null,
                        updated_by = RevelCardInsertUser,
                        updated_date = DateTime.Now,
                        payment_type = 4,
                        vip_points_refresh = 20,
                        days_since_last_visit = 0,
                        notes = "Investor Card"

                    };

                    //set loyalty type
                    if (card.WeeklyLoad != null && card.WeeklyLoad > 0)
                    {
                        rewardscardnew.LoyaltyCardType =
                            db.LoyaltyCardTypes.First(x => x.name.ToLower().Contains("weekly"));

                    }
                    else if (card.MonthlyLoad != null && card.MonthlyLoad > 0)
                    {
                        rewardscardnew.LoyaltyCardType =
                            db.LoyaltyCardTypes.First(x => x.name.ToLower().Contains("monthly"));
                    }
                    else
                    {
                        rewardscardnew.LoyaltyCardType =
                           db.LoyaltyCardTypes.First(x => x.name.ToLower().Contains("one-off"));

                    }

                    rewardscardnew.days_since_last_visit = 0;

                    /*///
                    /// 
                    /// Make the call to Revel*/
                    var est = new Establishment(1, "Grind",
                        RevelAPIKEY,
                        new Uri(RevelBaseURL));


                    var webReader = new RevelWebserviceDataReader(est);
                    var writer = new WebserviceDataWriter(est, db);

                    //create the customer
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {

                            if (await writer.CreateCustomer(customer) == 0)
                            {
                                //get that customer 
                                var createdCustomer = await webReader.GetRevelWebserviceItem(new Customer(), customer.ResourceUri);

                                customer.Uuid = createdCustomer.Uuid;
                                rewardscardnew.customer_revel = createdCustomer.ResourceUri;
                                rewardscardnew.Customer = customer;

                                db.Customers.Add(customer);
                                var ok = db.SaveChanges();

                                dbContextTransaction.Commit();

                            }
                            else
                            {
                                throw new Exception("Couldn't create Revel Customer");
                            }
                        }
                        catch (Exception)
                        {
                            dbContextTransaction.Rollback();
                            throw new Exception("Couldnt' create Revel Customer");
                        }
                    }


                    using (var transactionScope = db.Database.BeginTransaction())
                    {
                        try
                        {
                            if ((await writer.CreateRewardCard(rewardscardnew)) == 0)
                            {
                                db.RewardsCardNew.Add(rewardscardnew);
                                var ok = db.SaveChanges();

                                transactionScope.Commit();
                            }
                            else
                            {
                                throw new Exception("Couldnt' create Revel Card");
                            }
                        }
                        catch (Exception)
                        {
                            transactionScope.Rollback();
                            throw new Exception("Couldnt' create Revel Card");
                        }
                    }


                    /*    using (var transactionScope = db.Database.BeginTransaction())
                        {

                            var giftcard = new GiftCard
                            {
                                remaining_balance = 0,
                                payment_type = 4,
                                created_date = DateTime.Now,
                                created_by = RevelCardInsertUser,
                                updated_by = RevelCardInsertUser,
                                updated_date = DateTime.Now,
                                LinkingRevelCustomerID = customer.RevelId,
                                LinkingRevelRewardsCardNewID = rewardscardnew.Revelid,
                                initial_value = 0,
                                establishment = rewardscardnew.establishment,
                                number = rewardscardnew.number,
                                customer = customer.ResourceUri,
                                theCustomer = customer

                            };

                            giftcard.theCustomer = customer;
                            giftcard.RewardsCardNew = rewardscardnew;

                            var webCreate = await writer.CreateGiftCard(giftcard);
                            if (webCreate.Equals(0))
                            {
                                db.GiftCards.Add(giftcard);
                                db.SaveChanges();

                            }



                        }*/

                }

                //done - write to db
                card.HasBeenAdded = true;
                db.Entry(card).State = EntityState.Modified;
                db.SaveChanges();
            }

            return true;
        }



        public
                async Task<bool> InsertRevelURLINallCustomers()
        {
            var start = new DateTime(2013, 01, 01);
            var end = DateTime.Now;

            var revOrg = new Establishment(1, "Grind",
        RevelAPIKEY,
         new Uri(RevelBaseURL));

            RevelWebserviceDataReader webReader = new RevelWebserviceDataReader(revOrg);
            RevelDBWriter writer = new RevelDBWriter(db);
            RevelDBReader DBReader = new RevelDBReader(revOrg);


            //get existing from DB and check what we don't have



            List<Customer> existingCustomers = await DBReader.GetRevelType<Customer>();

            List<Customer> webServiceexistingCustomers =
                await webReader.GetRevelWebserviceData(new Customer(),
                 String.Format(new Customer().theAddress, start.ToString("yyyy-MM-ddTHH:mm:ss"),
                    end.ToString("yyyy-MM-ddTHH:mm:ss"))
                );

            foreach (var customer in existingCustomers)
            {
                var revelCustomer = webServiceexistingCustomers.FirstOrDefault(x => x.RevelId == customer.RevelId);

                if (revelCustomer != null)
                {

                }
            }
            return true;
        }
        /*   public async Task<int> CreateAllMissingCards()
           {
               Web.Grind._808nd.com.Models.GrindContext localContext = new Models.GrindContext();



                     var est = new Establishment(1, "Grind",
                   RevelAPIKEY,
                    new Uri(RevelBaseURL));

               var writer = new WebserviceDataWriter(est, db);

               var cards = new List<CARDSTOREPLICATE>();

               cards = localContext.CARDSTOREPLICATEs.ToList();



               //check not in DB
               var cardsFromlocal = db.RewardsCardNew.ToList();

               List<RewardsCardNew> cardsToCreate = new List<RewardsCardNew>();

               foreach (var cardstoreplicate in cards)
               {
                   var card =
                       cardsFromlocal.First(
                           x => x.DBKEY_rewardscardnew_id == cardstoreplicate.DBKEY_rewardscardnew_id);
                   cardsToCreate.Add(card);
               }




               foreach (var rewardsCardNew in cardsToCreate)
               {

                   rewardsCardNew.number = rewardsCardNew.number.Trim();

                   rewardsCardNew.days_since_last_visit = 0;
                   rewardsCardNew.created_date = DateTime.Now;
                   rewardsCardNew.updated_date = DateTime.Now;

                   rewardsCardNew.Revelid = 0;
                   rewardsCardNew.resource_uri = "";

                   if (rewardsCardNew.establishment == null)
                   {
                       rewardsCardNew.establishment = "/enterprise/Establishment/1/";
                   }

             /*      var serviceWITHCSUTOEMR = new CombinedCardCustomerController();
                   var ok = await serviceWITHCSUTOEMR.Create(new Customer(), rewardsCardNew);#1#

                   var addCardtest = await writer.CreateRewardCard(rewardsCardNew);

                   if (addCardtest == 0)
                   {
                       localContext.CARDSTOREPLICATEs.Remove(
                           cards.Where(x => x.DBKEY_rewardscardnew_id == rewardsCardNew.DBKEY_rewardscardnew_id)
                               .FirstOrDefault());

                       localContext.SaveChanges();
                       System.Threading.Thread.Sleep(1300);
                   }
                   else
                   {
                       var whatttt = rewardsCardNew.number;
                   }

               }

               return 0;
           }
        */

        public async Task<bool> TestPullGrindTotal()
        {
            var document = new HtmlDocument();
            var webGet = new HtmlWeb();
            document = webGet.Load("https://www.crowdcube.com/investment/grind-bond-19689");

            /*var node = document.DocumentNode.Descendants("strong")
                .Where(d => d.Attributes.Contains("class")
            && d.Attributes["class"].Value.Contains("highlight")).FirstOrDefault();*/

            var node = document.DocumentNode.Descendants("div")
                .Where(d => d.Attributes.Contains("class")
            && d.Attributes["class"].Value.Contains("cc-pitch__raised")).FirstOrDefault().ChildNodes[1];


            var moneyValue = node.InnerText.Replace('"', ' ').Replace(',', ' ').Replace(" ", string.Empty).Replace("£", "").Trim(' ');

            var existingTOtal = db.Fundings.FirstOrDefault();
            var oldAmount = existingTOtal.Amount;

            if (Convert.ToInt32(existingTOtal.Amount) != Convert.ToInt32(moneyValue))
            {
                existingTOtal.Amount = moneyValue;
                db.Entry(existingTOtal).State = EntityState.Modified;
                db.SaveChanges();


                //push to slack
                var slack = new SlackMessenger();
                var ok = await slack.SendMessage("The Grind Crowdcube was added to, it was £" + oldAmount + " and it's now £" + moneyValue, "grindbond", "GrindBond");

                return true;
            }



            return false;



        }

        public async Task<int> CreateRewardCardONLY()
        {
            var rewardscardnew = new RewardsCardNew
            {
                updated_by = "/enterprise/User/203/",
                created_by = "/enterprise/User/203/",
                created_date = DateTime.Now,
                updated_date = DateTime.Now,
                resource_uri = "",
                number = "99987654",
                payment_type = 4,
                establishment = "/enterprise/Establishment/1/",
                total_points = 0,
                total_purchases = 0.0M,
                total_visits = 0,
                current_points = 0

            };

            var est = new Establishment(1, "Grind",
            RevelAPIKEY,
             new Uri(RevelBaseURL));

            var writer = new WebserviceDataWriter(est, db);


            var addCard = await writer.CreateRewardCard(rewardscardnew);

            if (addCard == 0)
            {

                /*db.RewardsCardNew.Add(rewardscardnew);
                var saveCount = db.SaveChanges();
                if (saveCount > 0)
                {



                }*/
            }

            return 0;
        }


        public int TestMailChimpPush()
        {
            GrindContext db = new GrindContext();

            var cus = db.Customers.Find(18243);
            var card = db.RewardsCardNew.Find(22298);

            MailChimpGrind chimp = new MailChimpGrind();

            cus.Email = "Test@mailchimppush.com";

            var ok = chimp.PushCardSignUp(cus);



            return 0;
        }


        public async Task<int> TestBULKRevelPATCH()
        {
            var est = new Establishment(1, "Grind",
                RevelAPIKEY,
                 new Uri(RevelBaseURL));

            var writer = new WebserviceDataWriter(est, db);

            var cards = new List<RewardsCardNew>();

            cards = db.RewardsCardNew.Take(4).ToList();


            var ok = await writer.BulkUpdate(cards, "/resources/RewardsCardNew/");


            return 0;
        }

        public ActionResult TestPOSTCard()
        {

            return View();
        }

        public async Task<int> TestDeleteCustomer()
        {
            var est = new Establishment(1, "Grind",
                RevelAPIKEY,
                 new Uri(RevelBaseURL));

            using (var db = new GrindContext())
            {
                var c = db.Customers.First(x => x.RevelId == 2382);

                var service = new WebserviceDataWriter(est, db);

                var ok = await service.DeleteRevelItem(c);
            }

            return 0;
        }


        public async Task<int> TestUpdateRedCards()
        {
            var est = new Establishment(1, "Grind",
                RevelAPIKEY,
                 new Uri(RevelBaseURL));

            using (var db = new GrindContext())
            {
                var r = db.RewardsCardNew.First(x => x.Revelid == 4189);
                r.total_points = 1200;
                r.current_points = 1200;
                r.total_purchases = 1000.00M;
                r.total_visits = 1000;

                var service = new WebserviceDataWriter(est, db);

                var ok = await service.UpdateRewardCard(r);

                return 0;
            }

        }


        /*   public async Task<int> TestUpdateRewardsCard(RewardsCardNew r)
           {
               using (var db = new GrindContext())
               {

                   var service = new WebserviceDataWriter(db);

                   var ok = await service.UpdateRewardCard(r);

                   return 0;
               }
           }
        */

        public async Task<int> TestUpdateRewardsCard()
        {
            var est = new Establishment(1, "Grind",
                 RevelAPIKEY,
                  new Uri(RevelBaseURL));

            using (var db = new GrindContext())
            {
                var r = db.RewardsCardNew.First(x => x.Revelid == 4185);
                r.total_points = 1200;
                r.current_points = 1200;
                r.total_purchases = 1000.00M;
                r.total_visits = 1000;

                var service = new WebserviceDataWriter(est, db);

                var ok = await service.UpdateRewardCard(r);

                return 0;
            }
        }


        public async Task<int> TestGetAllREVELCustomer()
        {
            var service = new CustomerService(db);
            var ok = await service.GetAllCustomersAndInsertNew(new DateTime(2013, 01, 01), DateTime.Now);

            return 0;
        }

        public async Task<int> TestCreateCustomer()
        {
            var est = new Establishment(1, "Grind",
                RevelAPIKEY,
                 new Uri(RevelBaseURL));

            RevelFactory factory = new RevelFactory(est);


            using (var db = new GrindContext())
            {

                var c = db.Customers.First(x => x.DBKEY_customer_id == 14545);
                c.Uuid = "";
                c.ResourceUri = "";




                var service = new WebserviceDataWriter(est, db);

                var ok = await service.CreateCustomer(c);
            }

            return 0;

        }


        public async Task<int> TestUpdateCustomer()
        {
            var establishment = new Establishment(1, "Grind",
                   RevelAPIKEY,
                    new Uri(RevelBaseURL));



            using (var db = new GrindContext())
            {
                var c = db.Customers.First(x => x.DBKEY_customer_id == 14545);
                c.FirstName = "jacknicholson";

                var service = new WebserviceDataWriter(establishment, db);

                var ok = await service.UpdateCustomer(c);
            }

            return 0;

        }

        public ActionResult TestPageRender()
        {

            return View();

        }

        [HttpPost]
        public void TestPost()
        {

            var test = "test";

            var web = (GridWeb)Session["WebGrid"];
            web.WebWorksheets.RunAllFormulas();

        }


        [HttpGet]
        public bool SaveCurrentWorksheet()
        {

            var GridWeb1 = (GridWeb)Session["WebGrid"];
            string path = (string)Session["activeFilename"];

            GridWeb1.WebWorksheets.SaveToExcelFile(@"d:\test\donkey.xlxs", FileFormatType.Excel2007);

            return true;

        }

        public void TestFunctionSubmit()
        {




        }

        public void CheckExpiredRedCardsAndNotify()
        {
            var service = new Revel._808nd.com.Classes.ServiceImplementaitons.RewardCardServices(db);
            service.SetAllExpiredRedCards();
        }


        public async Task TestInvestorCardPointAddition()
        {


            using (var _db = new GrindContext())
            {


                var cardsToReset = db.RewardsCardNew.Where(x => x.DBKEY_rewardscardnew_id == 36486).ToList();
                var rewardService = new RewardCardServices(_db);

                var resetLoyaltyCardsWeekly = await rewardService.ResetInvestorCards(cardsToReset, _db);

                var emailer = new EmailController();
                var message = String.Format("Weekly Investor cards reset:{0} succesfully", resetLoyaltyCardsWeekly);
                emailer.CustomEmailMessage(message);
            }


        }


        class CardOldNewValue
        {
            public string CardNumber { get; set; }
            public string RestoredValueCurrent { get; set; }
            public string RestoredValueTotal { get; set; }
            public string CurrentValue { get; set; }

        }

        class CardAndPointsToRestore
        {
            public int pointsToRestore { get; set; }
            public string cardNumber { get; set; }
            //public int currentPoints { get; set; }
            //public int totalPoints { get; set; }

        }


        public IEnumerable<string> GetNumbersToRestore()
        {
            return new List<string> {
                "0207726",
                "00100738",
                "00135382",
                "00100549",
                "00136053",
                "00136221",
                "00136222",
                "00126510",
                "00141855",
                "000620047",
                "00106794",
                "00125653",
                "00126588",
                "00106938",
                "00114338",

            };

        }

        [AllowAnonymous]
        public async Task UndoHardLimit100PointsForTed2ndTry()
        {


            var est = new Establishment(1, "Grind",
               RevelAPIKEY,
                new Uri(RevelBaseURL));

            var db = new GrindContext();
            var writer = new WebserviceDataWriter(est, db);

            //get diffs from 16/07 - 17/07 when limit occurred
            var pointsDiffFor16th = db.RewardsCardDailyPoints.Where(x => x.date > new DateTime(2019, 07, 16) && x.date < new DateTime(2019, 07, 17)).ToList();
            var pointsDiffFor17th = db.RewardsCardDailyPoints.Where(x => x.date > new DateTime(2019, 07, 17) && x.date < new DateTime(2019, 07, 18)).ToList();

            List<CardAndPointsToRestore> restores = new List<CardAndPointsToRestore>();
            foreach (var pointStamp in pointsDiffFor16th)
            {
                if (pointStamp.card_number == "00032741")
                {
                    var stopHere = "";
                }

                var stampOn17th = pointsDiffFor17th.FirstOrDefault(x => x.card_number == pointStamp.card_number);
                if (stampOn17th != null)
                {
                    var howMuchWeLost = pointStamp.current_points_on_date - stampOn17th.current_points_on_date;
                    //var cardNow = db.RewardsCardNew.First(x => x.number == stampOn17th.card_number);

                    restores.Add(new CardAndPointsToRestore
                    {
                        cardNumber = pointStamp.card_number,
                        pointsToRestore = howMuchWeLost,
                        //currentPoints = cardNow.current_points,
                        //totalPoints = cardNow.total_points
                    });
                }
                else
                {
                    var cardNumberThatsFUckedUp = pointStamp.card_number;
                }

            }

            var ordered = restores.Where(x => x.pointsToRestore > 0).OrderByDescending(x => x.pointsToRestore).ToList();
            var done = "";

            using (TextWriter writer2 = new StreamWriter(@"C:\test\pointRestoresForHardlimitedCards.csv"))
            {
                var csv = new CsvWriter(writer2);
                csv.WriteRecords(ordered); // where values implements IEnumerable
            }
            //end

            //now filter the ones that WEREN'T restored on the day

            //now we've got the ones we need to update - foreach - add to cuurent points - check if current points + addedpoints > total points
            //if so - update  totalPoints to currentPoints+addedPoints

            var cardsToRestore = GetNumbersToRestore();

            foreach (var cardNumberFromSpreadsheet in cardsToRestore)
            {

                try
                {
                    var number = cardNumberFromSpreadsheet;
                    var realCard = db.RewardsCardNew.FirstOrDefault(x => x.number == number);
                    if (realCard == null)
                    {
                        throw new Exception("We got a problem with: " + realCard.number);
                    }
                    else
                    {
                        var hasRestoreAlready = db.SystemLogs.FirstOrDefault(x => x.Type == realCard.number);
                        if (hasRestoreAlready == null)
                        {
                            //do the restore
                            var restore = restores.First(x => x.cardNumber == realCard.number);
                            var pointsToREstoreToThisCards = restore.pointsToRestore;


                            var currentPoints = realCard.current_points;
                            var newCurrentPoints = Convert.ToInt32(pointsToREstoreToThisCards + realCard.current_points);
                            var newTotalPoints = newCurrentPoints > realCard.total_points ? newCurrentPoints : realCard.total_points;

                            //do the update
                            realCard.current_points = newCurrentPoints;
                            realCard.total_points = newTotalPoints;

                            var result = await writer.UpdateRewardCard(realCard);
                            //log it
                            var log = new SystemLog
                            {
                                Note = String.Format("Restored Card {0} from {1} to {2} - added {3}, total points was set as {4}", realCard.number, currentPoints, newCurrentPoints, pointsToREstoreToThisCards, newTotalPoints),
                                @Type = realCard.number,
                                WhenCreated = DateTime.Now,
                                WhoTriggered = "Nadav Manual Restore",
                            };
                            db.SystemLogs.Add(log);
                            db.SaveChanges();

                            //next
                        }
                    }
                }
                catch (Exception ex)
                {
                    var fucked = cardNumberFromSpreadsheet;
                    throw;
                }
            }

            var aert = "Thanks";
        }


        public async Task UndoHardLimit100PointsForTed()
        {

            //var est = new Establishment(1, "Grind",
            //   RevelAPIKEY,
            //    new Uri(RevelBaseURL));

            //var oldDb = new GrindContext("OldGrindRestoredForTedContext");
            //var db = new GrindContext();
            //var writer = new WebserviceDataWriter(est, db);

            //var allCardsToReste = oldDb.RewardsCardNew.Where(X => X.current_points > 100).ToList();

            //List<CardOldNewValue> oldNewVals = new List<CardOldNewValue>();
            //foreach (var oldCard in allCardsToReste)
            //{
            //    var currentCard = await db.RewardsCardNew.FirstAsync(x => x.DBKEY_rewardscardnew_id == oldCard.DBKEY_rewardscardnew_id);
            //    oldNewVals.Add(new CardOldNewValue
            //    {
            //        CardNumber = oldCard.number,
            //        CurrentValue = currentCard.current_points.ToString(),
            //        RestoredValueCurrent = oldCard.current_points.ToString(),
            //        RestoredValueTotal = oldCard.total_points.ToString()
            //    });


            //RESTORE CODE
            //currentCard.current_points = oldCard.current_points;
            //currentCard.total_points = oldCard.total_points;

            ////update
            //var result = await writer.UpdateRewardCard(currentCard);
            //}

            //using (TextWriter writer2 = new StreamWriter(@"C:\test\cardUnlimitedValuesCurrentAndRestored.csv"))
            //{
            //    var csv = new CsvWriter(writer2);
            //    csv.WriteRecords(oldNewVals); // where values implements IEnumerable
            //}
            ////end
            //var done = "";

        }

        [AllowAnonymous]
        public async Task Test_Reset_Weekly_Investors()
        {
            var cardsReset = 0;
            using (var _db = new GrindContext())
            {
                try
                {
                    var rewardService = new RewardCardServices(_db);

                    var cardsToReset =
                        await _db.RewardsCardNew.Where(x => x.LoyaltyCardType.id == 7
                        //&& x.vip_points_last_refreshed
                        //< new DateTime(2020, 01, 06, 00, 00, 00)
                        && x.Active == true).ToListAsync();

                    var cardIDwant = cardsToReset.Where(x => x.DBKEY_rewardscardnew_id == 59741).ToList();

                    if (cardsToReset.Count > 0)
                    {
                        var resetLoyaltyCardsWeekly = await rewardService.ResetInvestorCards(cardsToReset, _db);

                        var emailer = new EmailController();
                        var message = String.Format("Weekly Investor cards reset:{0} succesfully", resetLoyaltyCardsWeekly);
                        emailer.CustomEmailMessage(message);
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }

            }
        }

        public void Test_Create_CSV()
        {
            var end = DateTime.Now.AddDays(-1);
            var start = end.AddDays(-91);
            var listOfFiles = new List<string>();
            var establihsments = db.Establishments.Where(x => x.establishment_id != 2).ToList();
            var factory = new OrderItemReportFactory();

            foreach (var est in establihsments)
            {

                var orderItemsOUtPut = new List<OrderItem>();
                var reportData = factory.CreateProductOrderItemSummaryReport(new ReportContext
                {
                    StartDate = start,
                    EndDate = end,
                    NoOfDaysInEachReportingPeriod = 7,
                    IdOfStore = est.establishment_id
                }, new GrindContext(), out orderItemsOUtPut, "");

                var csvPath = String.Format("c:\\temp\\{0}_{1}.csv", est.name, DateTime.Now.ToLongDateString());
                using (var csv = new CsvWriter(new StreamWriter(csvPath)))
                {
                    csv.Configuration.HasHeaderRecord = true;
                    csv.Configuration.Delimiter = ",";
                    csv.WriteRecords(reportData);
                }

                listOfFiles.Add(csvPath);
            }


            //mailout
            var to = new List<string>();
            to.Add("dan@grindandco.com");
            to.Add("emailnadz@gmail.com");
            MailService mail = new MailService(to, "Weekly Item Report", null, listOfFiles);
            mail.SendEmail();

            return;
        }


        public async Task UpdateUsers()
        {
            Establishment revOrg = new Establishment(1, "Grind",
           RevelAPIKEY,
           new Uri(RevelBaseURL));

            RevelFactory revelFactory = new RevelFactory(revOrg);

            RevelWebserviceDataReader webReader = new RevelWebserviceDataReader(revOrg);
            RevelDBWriter writer = new RevelDBWriter(db);
            IRevelReaderAsync DBReader = new RevelDBReader(revOrg);

            var user = new User();
            var users = await webReader.GetRevelWebserviceData(user, user.theAddress);

            db.Users.AddRange(users);
            db.SaveChanges();


        }

        public async Task CreateCustomersWithNoCards()
        {

            var customersForWhichWeNEedToAddCards = new List<Customer>();
            //all number since then
            var allCardNumbers = db.RewardsCardNew
                .Where(x => x.created_by == "/enterprise/User/203/")
                .Where(x => x.created_date >= new DateTime(2016, 05, 23, 14, 31, 00))
                .Select(x => x.number.ToLower().Trim()).ToList();

            //all customers siince then
            var allCustomers = db.Customers
                 .Where(x => x.CreatedBy == "/enterprise/User/203/")
                 .Where(x => x.CreatedDate >= new DateTime(2016, 05, 23, 14, 31, 00))
                 .Select(x => x.Email)
                 .Distinct().ToList();


            var cusListProper = new List<Customer>();

            foreach (var cus in allCustomers)
            {
                var pulledCus = db.Customers.First(x => x.Email == cus);
                cusListProper.Add(pulledCus);
            }

            foreach (var customer in cusListProper)
            {

                try
                {
                    if (customer.LicNumber != null)
                    {
                        var isCardthere = allCardNumbers.FirstOrDefault(x => x == customer.LicNumber.ToLower().Trim());

                        if (isCardthere == null)
                        {
                            customersForWhichWeNEedToAddCards.Add(customer);
                        }

                    }
                }
                catch (Exception ex)
                {

                    throw;
                }


            }


            //foreach customer - create a card

            //NOW CREATE THE CARDS
            var cardsthatAlreadyExist = new List<RewardsCardNew>();


            foreach (var customer in customersForWhichWeNEedToAddCards)
            {

                var rewardscardnew = new RewardsCardNew
                {
                    updated_by = "/enterprise/User/203/",
                    created_by = "/enterprise/User/203/",
                    created_date = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")),
                    updated_date = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")),
                    number = customer.LicNumber.Trim(),
                    customer_revel = customer.ResourceUri,
                    current_points = 0,
                    days_since_last_visit = 0,
                    is_vip_card = false,
                    total_points = 0,
                    total_visits = 0,
                    total_purchases = 0,
                    Customer = customer

                };




                if (rewardscardnew.LoyaltyCardType != null)
                {
                    if (rewardscardnew.LoyaltyCardType.id == 0)
                    {
                        rewardscardnew.LoyaltyCardType = null;
                    }
                    else
                    {
                        var typeid = rewardscardnew.LoyaltyCardType.id;
                        var cardType = db.LoyaltyCardTypes.First(x => x.id == typeid);
                        rewardscardnew.LoyaltyCardType = cardType;
                    }

                }


                var est = new Establishment(1, "Grind",
                               RevelAPIKEY,
                               new Uri(RevelBaseURL));
                //create the customer, assign loyalty card number, any other atts




                //create in Revel
                var writer = new WebserviceDataWriter(est, db);


                var addCard = await writer.CreateRewardCard(rewardscardnew);

                if (addCard == 0)
                {

                    db.RewardsCardNew.Add(rewardscardnew);
                    var saveCount = db.SaveChanges();

                    if (saveCount > 0)
                    {
                        //it worked, hooray

                    }
                }
                else
                {
                    cardsthatAlreadyExist.Add(rewardscardnew);
                }

            }

            return;
        }


        public async Task ChangeCardNumber()
        {

            var theOldCardNumber = "010025";
            var theNewCardNumber = "00039314";
            var est = new Establishment(1, "Grind",
               RevelAPIKEY,
                new Uri(RevelBaseURL));

            var writer = new WebserviceDataWriter(est, db);

            //db setup work
            //get all cards from db, check there is only one with number
            //get all attached customers and giftcards
            var thePossibleCards = db.RewardsCardNew.Where(x => x.number.Trim() == theOldCardNumber).ToList();
            var theNewCardSuggestion = db.RewardsCardNew.Where(x => x.number.Trim() == theNewCardNumber).ToList();

            if (thePossibleCards.Count != 1)
            {
                throw new Exception("there number of cards matching the card number is: " + thePossibleCards.Count);

            }
            if (theNewCardSuggestion.Count > 0)
            {
                throw new Exception("there is already a card matching your new number");

            }

            var theCard = thePossibleCards.First();

            //log all potential operations

            //revel work
            //change card number - log it
            //if success, change all customer numbers (remember, there are multiple fields)
            //if sucess, change all gift card number


            var allCustomers = db.Customers.Where(x => x.LicNumber.Trim().Contains(theOldCardNumber)).ToList();
            if (allCustomers.Count == 0)
            {
                allCustomers = db.Customers.Where(x => x.ResourceUri.Trim().Contains(theCard.customer_revel)).ToList();
            }
            var allGiftCards = db.GiftCards.Where(x => x.number.Trim().Contains(theOldCardNumber)).ToList();
            if (allGiftCards.Count == 0)
            {
                allGiftCards = db.GiftCards.Where(x => x.customer.Trim().Contains(theCard.customer_revel)).ToList();
            }

            var test = 1;

            if (allCustomers.Count > 0 && allGiftCards.Count > 0 && theCard != null)
            {
                //we have everything to change all the numbers
                /*  db.SystemLogs.Add(new SystemLog
                  {
                      WhenCreated = DateTime.Now,
                      Type = "CARD_NUMBER_CHANGED",
                      Note = String.Format("Card: Number:{0} - Id: " + rewardscardnew.number + " was created had 20 points added")
                  });

                  db.SaveChanges();
    */

                //card
                theCard.number = theNewCardNumber;
                //customer
                foreach (var cus in allCustomers)
                {
                    cus.LoyaltyNumber = theNewCardNumber;
                    cus.LicNumber = theNewCardNumber;
                    cus.PhoneNumber = theNewCardNumber;
                    cus.RefNumber = theNewCardNumber;
                }

                //gift cards
                foreach (var gift in allGiftCards)
                {
                    gift.number = theNewCardNumber;
                }


                //revel changing
                if (await writer.UpdateRewardCard(theCard) == 0)
                {
                    //log it and do the customers
                    foreach (var cus in allCustomers)
                    {
                        if (await writer.UpdateCustomer(cus) != 0)
                        {
                            throw new Exception("Couldn't update customer");
                        }

                    }
                    foreach (var gcard in allGiftCards)
                    {
                        if (await writer.UpdateGiftCard(gcard) != 0)
                        {
                            throw new Exception("Couldn't update gift");
                        }
                    }
                }
                else
                {
                    throw new Exception("Couldn't update reward card");
                }

                //that's it
            }
        }

        public async Task TransformCSVFromRevelToFourth()
        {

        }


        public async Task UpdatePointsOnExisting()
        {
            //FOR EACH - ADD X POINTS / WIPE GIFT BALANCE
            var startingCardNumber = 39769;
            var endingCardNumber = 39769;


            var service = new CombinedCardCustomerController(db);

            var est = new Establishment(1, "Grind",
                RevelAPIKEY,
                 new Uri(RevelBaseURL));

            var writer = new WebserviceDataWriter(est, db);

            for (int i = startingCardNumber; i <= endingCardNumber; i++)
            {
                var numbertOInsert = "000" + i;

                var existing = db.RewardsCardNew.FirstOrDefault(x => x.number.Equals(numbertOInsert));
                var giftCard = db.GiftCards.FirstOrDefault(x => x.number.Equals(numbertOInsert));

                if (existing != null)
                {

                    existing.current_points = 50;
                    existing.total_points = 50;

                    var result = await writer.UpdateRewardCard(existing);
                    if (result == 0)
                    {
                        //wipe gift balance
                        giftCard.remaining_balance = 0;
                        await writer.UpdateGiftCard(giftCard);
                    }
                    else
                    {
                        var didntWork = existing;

                    }

                }
                else
                {

                    var didneExcist = existing;

                }

            }
        }

        public async Task ChangeLoyaltyCardType()
        {
            //get collection 
            var newCustomers = (List<InvestorCardIImportSignup>)ImportExcelSheetsAsCustomers();
            var cardsToupdate = new List<RewardsCardNew>();
            List<InvestorCardIImportSignup> missingCards = new List<InvestorCardIImportSignup>();
            var cartype = db.LoyaltyCardTypes.First(x => x.id == 7);

            foreach (var item in newCustomers)
            {
                var existing = db.RewardsCardNew.FirstOrDefault(x => x.number.Equals(item.number));
                if (existing != null)
                {
                    existing.Active = true;
                    existing.LoyaltyCardType = cartype;
                    db.RewardsCardNew.AddOrUpdate(existing);
                }
                else
                {
                    var ohShit = "";
                    missingCards.Add(item);
                }

            }

            db.SaveChanges();
        }


        public async Task HardLimitCurrentPointsTo20ForLoyaltyType1Cards()
        {
            var est = new Establishment(1, "Grind",
            RevelAPIKEY,
             new Uri(RevelBaseURL));

            var writer = new WebserviceDataWriter(est, db);

            var loyaltyType = db.LoyaltyCardTypes.First(x => x.id == 1);
            var allcards = db.RewardsCardNew.Where(x => x.LoyaltyCardType.id == 1).ToList();

            //hard limit in Revel
            foreach (var card in allcards)
            {
                card.current_points = 20;
                await writer.UpdateRewardCard(card);
            }

            var test = "done";
        }


        //import form CSV     
        public async Task UpdateBalanceOnGiftCards()
        {
            var est = new Establishment(1, "Grind",
                RevelAPIKEY,
                 new Uri(RevelBaseURL));
            var writer = new WebserviceDataWriter(est, db);

            var cardNumbers = new List<int>
            {
50000051,
50000053,
50000063,
50000064,
50000065,
50000066,
50000067,
50000068,
50000069,
50000070,
50000071,
50000072,
50000073,
50000074,
50000075,
50000076,
50000077,
50000078,
50000079,
50000080,
50000081,
50000082,
50000083,
50000084,
50000085,
50000086,
50000087,
50000160,
50000162,
50000163,
50000182,
50000184,
50000185,
50000186,
50000187,
50000190,
50000191,
50000192,
50000193,
50000194,
50000195,
50000196,
50000197,
50000198,
50000199,
50000201,
50000202,
50000204,
50000205,
50000207,
50000332,
50000474,
50000477,
50000478,
50000479,
50000480,
50000481,
50000482,
50000483,
50000484,
50000485,
50000486,
50000487,
50000488,
50000489,
50000490,
50000491,
50000492,
50000493,
50000494,
50000495,
50000496,
50000497,
50000498,
50000499,
50000500,
50000956,
50000957,
50000958,
50000959,
50000960,
50000961,
50000962,
50000963,
50000964,
50000965,
50000966,
50000967,
50000968,
50000984,
50000985,
50000986,
50001031,
50000004,
50000005,
50000006,
50000007,
50000008,
50000009,
50000010,
50000011,
50000012,
50000013,
50000014,
50000015,
50000016,
50000017,
50000018,
50000019,
50000020,
50000021,
50000022,
50000023,
50000024,
50000025,
50000026,
50000027,
50000028,
50000029,
50000030,
50000031,
50000032,
50000033,
50000034,
50000035,
50000036,
50000037,
50000038,
50000040,
50000041,
50000042,
50000043,
50000044,
50000045,
50000046,
50000047,
50000048,
50000049,
50000050,
50000054,
50000055,
50000056,
50000057,
50000058,
50000059,
50000060,
50000061,
50000062,
50000674,
50001001,
50001002,
50001003,
50001004,
50001005,
50001006,
50001007,
50001008,
50001009,
50001010,
50001011,
50001013,
50001014,
50001015,
50001016,
50001017,
50001018,
50001019,
50001020,
50001021,
50001022,
50001023,
50001024,
50001025,
50001026,
50001027,
50001028,
50001029,
50001030,
50001031,
50001032,
50001033,
50001034,
50001035,
50001037,
50001038,
50001039,
50001039,
50001040,
50001041,
50001042,
50001043,
50001044,
50001045,
50001046,
50001047,
50001048,
50001049,
50001050,
50001051,
50001052,
50001053,
50001054,
50001055,
50001056,
50001057,
50001058,
50001059,
50001060,
50001061,
50001062,
50001063,
50001064,
50001065,
50001066,
50001067,
50001068,
50001069,
50001070,
50001071,
50001072,
50001073,
50001074,
50001075,
50001076,
50001077,
50001078,
50000088,
50000089,
50000090,
50000091,
50000092,
50000093,
50000094,
50000095,
50000096,
50000097,
50000098,
50000099,
50000100,
50000101,
50000102,
50000103,
50000104,
50000105,
50000106,
50000107,
50000108,
50000109,
50000110,
50000111,
50000112,
50000113,
50000114,
50000115,
50000116,
50000117,
50000118,
50000119,
50000120,
50000121,
50000122,
50000123,
50000124,
50000125,
50000126,
50000127,
50000128,
50000129,
50000130,
50000131,
50000132,
50000133,
50000134,
50000135,
50000136,
50000137,
50000138,
50000139,
50000140,
50000141,
50000142,
50000143,
50000145,
50000164,
50000165,
50000166,
50000167,
50000168,
50000169,
50000170,
50000171,
50000172,
50000173,
50000174,
50000175,
50000176,
50000177,
50000178,
50000179,
50000180,
50000181,
50000183,
50001378,
50001379,
50001380,
50001381,
50001382,
50001383,
50001384,
50001385,
50001386,
50001451,
50001452,
50001453,
50001454,
50001455,
50001456,
50001457,
50001458,
50001459,
50001460,
50001461,
50001462,
50001463,
50001464,
50001465,
50001466,
50001467,
50001468,
50001469,
50001470,
50001471,
50001472,
50001473,
50001474,
50001475,
50001476,
50001477,
50001478,
50001479,
50001480,
50001481,
50001482,
50001483,
50001484,
50001485,
50001486,
50001487,
50001488,
50001489,
50001490,
50001491,
50001492,
50001493,
50001494,
50001495,
50001496,
50001497,
50001498,
50001499,
50001500,
50001136,
50001143,
50001144,
50001155,
50001249,
50001250,
50001251,
50001252,
50001253,
50001260,
50001261,
50001262,
50001263,
50001321,
50001322,
50001323,
50001324,
50001325,
50001326,
50001327,
50001328,
50001329,
50001330,
50001331,
50001332,
50001333,
50001334,
50001335,
50001336,
50001337,
50001338,
50001339,
50001340,
50001341,
50001342,
50001343,
50001344,
50001345,
50001346,
50001347,
50001348,
50001349,
50001350,
50001351,
50001352,
50001353,
50001354,
50001355,
50001356,
50001357,
50001358,
50001359,
50001360,
50001361,
50001362,
50001363,
50001364,
50001365,
50001366,
50001367,
50001368,
50001369,
50001370,
50001371,
50001372,
50001373,
50001374,
50001375,
50001376,
50001377,
50001387,
50001388,
50001389,
50001390,
50001391,
50001392,
50001393,
50001394,
50001395,
50001396,
50001397,
50001398,
50001399,
50001400,
50001401,
50001402,
50001403,
50001404,
50001405,
50001406,
50001407,
50001408,
50001409,
50001410,
50001411,
50001412,
50001413,
50001414,
50001415,
50001416,
50001417,
50001418,
50001419,
50001420,
50001421,
50001422,
50001423,
50001424,
50001425,
50001426,
50001427,
50001428,
50001429,
50001430,
50001431,
50001432,
50001433,
50001434,
50001435,
50001436,
50001437,
50001438,
50001439,
50001440,
50001441,
50001442,
50001443,
50001444,
50001445,
50001446,
50001447,
50001448,
50001449,
50001450,
50000576,
50000577,
50000578,
50000579,
50000580,
50000581,
50000582,
50000583,
50000584,
50000585,
50000586,
50000587,
50000588,
50000589,
50000590,
50000591,
50000592,
50000593,
50000594,
50000595,
50000596,
50000597,
50000598,
50000599,
50000600,
50000601,
50000602,
50000603,
50000604,
50000605,
50000606,
50000607,
50000608,
50000609,
50000610,
50000611,
50000612,
50000613,
50000614,
50000615,
50000616,
50000617,
50000618,
50000619,
50000620,
50000621,
50000622,
50000623,
50000624,
50000625,
50000626,
50000627,
50000628,
50000630,
50000631,
50000632,
50000633,
50000634,
50000635,
50000636,
50000637,
50000638,
50000639,
50000640,
50000641,
50000642,
50000643,
50000644,
50000645,
50000646,
50000647,
50000648,
50000649,
50000650,
50000651,
50000652,
50000653,
50000654,
50000655,
50000656,
50000657,
50000658,
50000659,
50000660,
50000661,
50000662,
50000663,
50000664,
50000665,
50000666,
50000667,
50000668,
50000669,
50000670,
50000671,
50000672,
50000673,
50000675,
50000676,
50000677,
50000678,
50000679,
50000680,
50000681,
50000682,
50000683,
50000684,
50000685,
50000686,
50000687,
50000688,
50000689,
50000690,
50000691,
50000692,
50000693,
50000694,
50000695,
50000696,
50000697,
50000698,
50000699,
50000700,
50000701,
50000702,
50000703,
50000704,
50000705,
50000706,
50000707,
50000708,
50000709,
50000710,
50000711,
50000712,
50000714,
50000715,
50000716,
50000717,
50000718,
50000719,
50000720,
50000721,
50000722,
50000723,
50000724,
50000725,
50000726,
50000727,
50000728,
50000729,
50000730,
50000731,
50000733,
50000736,
50000737,
50000738,
50000739,
50000741,
50000742,
50000743,
50000744,
50000745,
50000746,
50000747,
50000748,
50000749,
50000750,
50000751,
50000752,
50000753,
50000754,
50000755,
50000756,
50000757,
50000758,
50000759,
50000760,
50000761,
50000762,
50000763,
50000764,
50000765,
50000766,
50000767,
50000768,
50000769,
50000770,
50000771,
50000772,
50000774,
50000775,
50000776,
50000778,
50000779,
50000780,
50000781,
50000782,
50000783,
50000784,
50000785,
50000786,
50000787,
50000788,
50000789,
50000790,
50000791,
50000792,
50000793,
50000794,
50000795,
50000796,
50000797,
50000798,
50000799,
50000800,
50000801,
50000802,
50000803,
50000804,
50000805,
50000806,
50000807,
50000808,
50000809,
50000810,
50000811,
50000812,
50000813,
50000814,
50000815,
50000856,
50000857,
50000858,
50000859,
50000860,
50000861,
50000863,
50000864,
50000865,
50000866,
50000867,
50000868,
50007777
            };

            var newBalance = 0.00M;
            var numberOfPoints = 0;
            var service = new CombinedCardCustomerController(db);

            foreach (var item in cardNumbers)
            //for (int i = startingCardNumber; i <= endingCardNumber; i++)
            {
                //var numbertOInsert = "000" + i;
                var numbertOInsert = item.ToString();
                //     var existing = db.RewardsCardNew.FirstOrDefault(x => x.number.Equals(numbertOInsert));
                var existingGift = db.GiftCards.FirstOrDefault(x => x.number.Equals(numbertOInsert));
                if (existingGift != null)
                {
                    existingGift.remaining_balance = newBalance;
                    await writer.UpdateGiftCard(existingGift);
                }
            }
            //        await writer.UpdateGiftCard(existingGift);
            //    if (existing == null)
            //    {
            //        var customer = (new Customer
            //        {
            //            FirstName = "",
            //            LastName = "",
            //            Email = "",
            //            LicNumber = numbertOInsert,
            //            RefNumber = numbertOInsert,
            //            LoyaltyNumber = numbertOInsert,
            //            Notes = "Created by Nadav for Antonia Gift Card Batch 27/01/2020"
            //        });

            //        customer.CreatedBy = "/enterprise/User/203/";
            //        customer.UpdatedBy = "/enterprise/User/203/";
            //        customer.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
            //        customer.UpdatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));

            //        var rewardscardnew = new RewardsCardNew
            //        {
            //            updated_by = "/enterprise/User/203/",
            //            created_by = "/enterprise/User/203/",
            //            created_date = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")),
            //            updated_date = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")),
            //            number = customer.LoyaltyNumber,
            //            customer_revel = customer.ResourceUri,
            //            current_points = numberOfPoints,
            //            days_since_last_visit = 0,
            //            is_vip_card = false,
            //            Active = true,
            //            vip_points_refresh = 0,
            //            total_points = numberOfPoints,
            //            total_visits = 0,
            //            total_purchases = 0,
            //            Customer = customer,
            //            notes = customer.Notes,
            //            LoyaltyCardType = null
            //        };


            //        var result = await service.Create(customer, rewardscardnew, "0");


            //        db.SystemLogs.Add(new SystemLog
            //        {
            //            WhenCreated = DateTime.Now,
            //            Type = "CARD_ADDED",
            //            Note = "Card:" + rewardscardnew.number + " was created had " + numberOfPoints + " points added"
            //        });

            //        db.SaveChanges();


            //    }
            //    else
            //    {
            //        var alreadyExists = numbertOInsert;

            //        existingGift.remaining_balance = newBalance;
            //        await writer.UpdateGiftCard(existingGift);
            //    }

            //}




            //foreach (var i in cardNumbers)
            //{

            //    var numberOfCards = i.ToString();
            //    var giftCard = db.GiftCards.FirstOrDefault(x => x.number == numberOfCards);
            //    if (giftCard == null)
            //    {
            //        var stopItsNull = "";
            //    }
            //    else
            //    {
            //        giftCard.remaining_balance = newBalance;
            //        await writer.UpdateGiftCard(giftCard);
            //    }


            //}
        }

        public async Task UpdateCardsForTed()
        {
            //120565 - 120716
            //120164 - 120177
            //120207 - 120239
            //120240 - 120260
            //120275 - 120296

            var startingCardNumber = 155501;
            var endingCardNumber = 155550; //43271; //43231 first batch

            var numberOfPoints = 0;
            var service = new CombinedCardCustomerController(db);

            for (int i = startingCardNumber; i <= endingCardNumber; i++)
            {
                var numbertOInsert = "00" + i.ToString();
                //var existingGift = db.GiftCards.FirstOrDefault(x => x.number == i.ToString());
                var existing = db.RewardsCardNew.FirstOrDefault(x => x.number.Equals(i.ToString()));
                var existingGift = db.GiftCards.FirstOrDefault(x => x.number == i.ToString());
                //var existingOTherNumber = db.RewardsCardNew.FirstOrDefault(x => x.number.Equals(numbertOInsert));
                //var existingGiftOtherNmber = db.GiftCards.FirstOrDefault(x => x.number == numbertOInsert);


                if (existing != null)
                {
                    var sotp = "";
                }

                if (existingGift != null)
                {
                    var sotp = "";
                    existingGift.number = numbertOInsert;
                }
                else
                {
                    existingGift = db.GiftCards.FirstOrDefault(x => x.number == numbertOInsert.ToString());
                }


                if (existing != null)
                {
                    try
                    {
                        var existingCustomer = db.Customers.FirstOrDefault(x => x.LicNumber == existing.number);
                        if (existingCustomer == null)
                        {
                            existingCustomer = db.Customers.FirstOrDefault(x => x.LicNumber == numbertOInsert);
                        }
                        if (existingCustomer == null)
                        {
                            var stoptheresaniSsue = "";
                        }

                        //change numbers

                        existing.number = numbertOInsert;
                        //customer 
                        existingCustomer.LicNumber = numbertOInsert;
                        existingCustomer.PhoneNumber = numbertOInsert;
                        existingCustomer.RefNumber = numbertOInsert;
                        existingCustomer.LoyaltyRefId = numbertOInsert;
                        existingCustomer.LoyaltyNumber = numbertOInsert;


                        await service.Edit(existingCustomer, existing, existingGift, null);
                    }
                    catch (Exception ex)
                    {
                        var cardExcpet = numbertOInsert;
                        throw;
                    }


                }
            }

        }



        //USE CORRECT SHEET!
        //DOING 10 POINTS A DAY
        public async Task CreateCardsForTed()
        {
            //get collection 
            // var newCustomers = (List<InvestorCardIImportSignup>)ImportExcelSheetsAsCustomers();

            //var loyaltyCardType = db.LoyaltyCardTypes.First(x => x.id == 1);
            var startingCardNumber = 00155501;
            var endingCardNumber = 00155550; //43271; //43231 first batch

            var numberOfPoints = 10;
            var service = new CombinedCardCustomerController(db);

            //foreach (var item in listOfCardNumbers)
            for (int i = startingCardNumber; i <= endingCardNumber; i++)
            {
                //var numbertOInsert = "000" + i;
                var numbertOInsert = i.ToString();
                var existing = db.RewardsCardNew.FirstOrDefault(x => x.number.Equals(numbertOInsert));

                if (existing == null)
                {
                    var customer = (new Customer
                    {
                        FirstName = "",
                        LastName = "",
                        Email = "",
                        LicNumber = numbertOInsert,
                        RefNumber = numbertOInsert,
                        LoyaltyNumber = numbertOInsert,
                        Notes = "Created by Ted for Codestorm 8/10/2019"
                    });

                    customer.CreatedBy = "/enterprise/User/203/";
                    customer.UpdatedBy = "/enterprise/User/203/";
                    customer.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
                    customer.UpdatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));

                    var rewardscardnew = new RewardsCardNew
                    {
                        updated_by = "/enterprise/User/203/",
                        created_by = "/enterprise/User/203/",
                        created_date = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")),
                        updated_date = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")),
                        number = customer.LoyaltyNumber,
                        customer_revel = customer.ResourceUri,
                        current_points = numberOfPoints,
                        days_since_last_visit = 0,
                        is_vip_card = false,
                        Active = true,
                        vip_points_refresh = 0,
                        total_points = numberOfPoints,
                        total_visits = 0,
                        total_purchases = 0,
                        Customer = customer,
                        notes = customer.Notes,
                        LoyaltyCardType = null
                    };


                    var result = await service.Create(customer, rewardscardnew, "0");


                    db.SystemLogs.Add(new SystemLog
                    {
                        WhenCreated = DateTime.Now,
                        Type = "CARD_ADDED",
                        Note = "Card:" + rewardscardnew.number + " was created had " + numberOfPoints + " points added"
                    });

                    db.SaveChanges();


                }
                else
                {
                    var alreadyExists = numbertOInsert;
                }

            }

            //it's over
            return;
        }



        public async Task AddPointsToCardRange()
        {

            var est = new Establishment(1, "Grind",
                RevelAPIKEY,
                 new Uri(RevelBaseURL));

            var pointsToAdd = 20;

            var writer = new WebserviceDataWriter(est, db);
            var startingCardNumber = 00028757;
            var endingCardNumber = 00028855;

            using (var _db = new GrindContext())
            {

                for (int i = startingCardNumber; i <= endingCardNumber; i++)
                {
                    var compositeNumber = "000" + i.ToString();
                    var card = _db.RewardsCardNew.FirstOrDefault(x => x.number == compositeNumber);

                    if (card != null)
                    {
                        card.total_points += pointsToAdd;
                        card.current_points += pointsToAdd;

                        card.updated_by = RevelCardInsertUser;
                        card.updated_date = DateTime.Now;

                        if (await writer.UpdateRewardCard(card) != 0)
                        {
                            var thecardThatfuckedup = card;
                        }
                        else
                        {
                            //it worked
                            db.SystemLogs.Add(new SystemLog
                            {
                                Type = "CARD_UPDATED",
                                Note = "Card:" + card.number + " had 20 points added"
                            });

                            db.SaveChanges();
                        }

                    }
                }

                var stop = "";
            }

            /*  using (var _db = new GrindContext())
            {


                var cardsToReset = db.RewardsCardNew.Where(x => x.DBKEY_rewardscardnew_id == 36486).ToList();
                var rewardService = new RewardCardServices(_db);

                var resetLoyaltyCardsWeekly = await rewardService.ResetInvestorCards(cardsToReset, _db);

                var emailer = new EmailController();
                var message = String.Format("Weekly Investor cards reset:{0} succesfully", resetLoyaltyCardsWeekly);
                emailer.CustomEmailMessage(message);
            }
        */

        }

        //public async Task TestResetAllRedCards()
        //{
        //    var controller = new SyncController();
        //    var cardsReset = new List<ILoggableCollection>();
        //    cardsReset.Add(new LoggableCollection(await controller.ResetAllRedCards(db), RevelBaseURL, DateTime.Now, "Red Cards"));

        //    //send email
        //    var cardsUpdatedString = "";
        //    foreach (var loggableList in cardsReset)
        //    {
        //        cardsUpdatedString += loggableList.WhenLogged;
        //        cardsUpdatedString += loggableList.CollectionDescription + ". Cards:  ";
        //        foreach (var card in loggableList.TheCollection)
        //        {
        //            cardsUpdatedString += card.Identifier + ", ";
        //        }

        //    }
        //    var emailerFinish = new EmailController();
        //    var messageFinish = String.Format("3am Card routine run ok :{0}. The cards updated were " + cardsUpdatedString, ConfigurationManager.AppSettings["RevelBaseURL"]);
        //    emailerFinish.CustomEmailMessage(messageFinish);

        //}

        //public async Task TestSetAllExpiredRedCards()
        //{
        //    var rewardService = new RewardCardServices(db);
        //    rewardService.SetAllExpiredRedCards();
        //}


        public async Task NewPointDoublingRouting()
        {


            var emailer = new EmailController();
            var est = new Establishment(1, "Grind",
                RevelAPIKEY,
                 new Uri(RevelBaseURL));
            ////define date ranges and cards ranges
            ////get all cards that have multipliers
            var cardServices = new RewardCardServices(db);

            var multipliers = db.RewardsPointsMultiplier.ToList();
            var multiplierStrings = multipliers.Select(x => x.emailSuffix).ToList();

            //GET CUSTOMERS
            var customers = db.Customers.Where(item => multiplierStrings.Any(stringToCheck => item.Email.Contains(stringToCheck))).ToList();
            //get cards for each customer      
            var cardstoMultiply = new List<RewardsCardNew>();
            foreach (var item in customers)
            {
                var cardForCustomer = await cardServices.GetByCustomerEmail(item.Email);
                if (cardForCustomer == null)
                {
                    var thisAintGood = "";
                }
                else
                {
                    if (cardForCustomer.is_vip_card != true && cardForCustomer.LoyaltyCardType == null)
                        cardstoMultiply.Add(cardForCustomer);
                }

            }
            //remove any duplicate cards
            cardstoMultiply = cardstoMultiply.Distinct().ToList();
            var webServiceDataWriter = new WebserviceDataWriter(est, db);

            //get cards for each customer           
            //var cardstoMultiply = await cardServices.GetCardsWithMultipliers(cards, customers, multipliers, db);
            //cardstoMultiply = cardstoMultiply.Where(x => (x?.is_vip_card ?? false) != true).Where(x => x.LoyaltyCardType == null).ToList(); //no vip cards

            //test just one card - 


            ////DONE: 90002423, 00041910
            //var cardstoMultiply = new List<RewardsCardNew>();
            //cardstoMultiply = db.RewardsCardNew.ToList();


            //save string of all cards
            var fullCardsDelmit = "";
            foreach (var c in cardstoMultiply)
            {
                fullCardsDelmit += c.number + ",";
            }

            //db.SystemLogs.Add(new SystemLog
            //{
            //    Type = "ALL_CARDS_WITH MULTIPLERS ON " + DateTime.Now.ToLongDateString(),
            //    Note = fullCardsDelmit,
            //    WhenCreated = DateTime.Now,
            //    WhoTriggered = "Nadav"
            //});
            //var saveRest = db.SaveChanges();

            foreach (var card in cardstoMultiply)
            {
                var cardNumberToCheck = card.number;
                var startDateToCheck = DateTime.Now.AddDays(-2);
                //var endDateToCheck = new DateTime(2017, 01, 01, 05, 00, 00);

                var multiplierLogsForThisCardSinceEver = db.RewardCardPointsTransactionLogs
                    .Where(x => x.card_number == cardNumberToCheck)
                    .Where(x => x.WhenCreated >= startDateToCheck)
                    .ToList(); //actyual multiplications

                var pointsLoggedForThisCardSinceEvery = db.RewardsCardDailyPoints
                    .Where(x => x.card_number == cardNumberToCheck)
                    .Where(x => x.date >= startDateToCheck)
                    .ToList(); //all logs
                               //check if there has been and update by diffing starting with the earlierst in dailyLogsAndCycling

                var daysThereWasNotNextLogFor = new List<RewardsCardDailyPoints>(); //should only be latest day
                var daysWhereDoublingDidOccur = new List<RewardCardPointsTransactionLog>();
                var pointsThatNeedDoublingForThisCard = 0; //total of points added since date that weren't doubled
                var newLogsToAdd = new List<RewardCardPointsTransactionLog>();

                foreach (var log in pointsLoggedForThisCardSinceEvery.OrderBy(x => x.date))
                {
                    var nextLog = pointsLoggedForThisCardSinceEvery.Where(x => x.date > log.date && x.date <= log.date.AddHours(30)).FirstOrDefault(); //did a doubling occur in 30 hours since log?

                    if (nextLog != null)
                    {
                        var currentPointsNumber = log.total_points_on_date;
                        var nextPointsNumber = nextLog.total_points_on_date;

                        if (!currentPointsNumber.Equals(nextPointsNumber))
                        {
                            //was there a log made for this double - match on nextLog date
                            var didAADoubleOccurToday = multiplierLogsForThisCardSinceEver.Where(x => x.WhenCreated.ToString("dd/MM/yyyy") == nextLog.date.ToString("dd/MM/yyyy")).FirstOrDefault();

                            if (didAADoubleOccurToday == null) //no double occurred
                            {
                                var previousDoubleSoWeCanRemoveThosePoints = multiplierLogsForThisCardSinceEver.Where(x => x.WhenCreated.ToString("dd/MM/yyyy") == nextLog.date.AddDays(-1).ToString("dd/MM/yyyy")).FirstOrDefault();
                                //get yesterday's double and remove those point
                                //total points have been added to, how many, and add to total   
                                //WHAT POINTS WERE ADDED YESTEDAY - REMOVE THOSE!!!

                                var pointsAdded = nextPointsNumber - currentPointsNumber;
                                if (previousDoubleSoWeCanRemoveThosePoints != null)
                                {
                                    pointsAdded = pointsAdded - previousDoubleSoWeCanRemoveThosePoints.pointsAdded;
                                }

                                pointsThatNeedDoublingForThisCard += pointsAdded; // THIS IS WHERE WE ADD THEM TO THE BUCKET

                                if (pointsThatNeedDoublingForThisCard > 0)
                                {


                                }
                                //also create a log for this day
                                var multiplier = await cardServices.GetMultiplierForCard(card, new RevelDBReader(est));
                                var newLogToAdd = new RewardCardPointsTransactionLog
                                {
                                    card_number = cardNumberToCheck,
                                    orginal_points_current = log.current_points_on_date,
                                    orginal_points_total = log.total_points_on_date,
                                    pointsAdded = pointsAdded,
                                    multiplier = multiplier.multiplier,
                                    new_points_current = nextLog.current_points_on_date,
                                    new_points_total = nextLog.total_points_on_date,
                                    WhenCreated = new DateTime(nextLog.date.Year, nextLog.date.Month, nextLog.date.Day, 03, 00, 00)
                                };

                                newLogsToAdd.Add(newLogToAdd);
                            }
                            else
                            {
                                //a double did occur, great, add it to the list and ignore
                                daysWhereDoublingDidOccur.Add(didAADoubleOccurToday);
                            }


                        }
                    }
                    else
                    {
                        //couldn't find a match - should only be one of these per card
                        daysThereWasNotNextLogFor.Add(log);
                    }


                }


                //ACTUAL TRANSACTION
                //end cycle
                //should have totalled up all points added for this card, if there are any, find the multiplier and multiply them
                if (pointsThatNeedDoublingForThisCard > 0)
                {

                    //run them over the multiplier
                    var multiplier = await cardServices.GetMultiplierForCard(card, new RevelDBReader(est));

                    if (multiplier == null)
                    {
                        throw new Exception("Couldn't find the multiplier!");
                    }

                    //SUBRACT ADD THE POINTS WE ADDED FIRST FROM THE FINAL TOTAL
                    var originalPointsAdded = pointsThatNeedDoublingForThisCard;
                    var actualPointsTotal = (pointsThatNeedDoublingForThisCard * multiplier.multiplier) - originalPointsAdded; //ONLY WANT TO ADD SAME POINTS AGAIN, NOT DOUBLE THEM
                    if (actualPointsTotal > 40)
                    {
                        actualPointsTotal = 40;
                        string errorMax = String.Format("<p>MAX DOUBLING WAS REACHED for card {0}. Please check this wasn't an error.</p>", card.number);
                        emailer.SendMessageGrindErrorAndNadavIgnoreSendExeceptions("GRIND AND CO - DANGER - POINTS DOUBLING MAX REACHED!!", errorMax);
                    }
                    //do a multiplication and log it, save all the new logs so it won't get multiplied again. 

                    //update the card
                    card.current_points += actualPointsTotal;
                    card.total_points += actualPointsTotal;
                    card.updated_date = DateTime.Now;


                    //  endResultList.Add(new KeyValuePair<string, string>(card.number, actualPointsTotal.ToString()));

                    //make call to revel for post card
                    //if success save DB logs for points multiplier
                    //save card in DB

                    if (await webServiceDataWriter.UpdateRewardCard(card) == 0)
                    {
                        //successful

                        db.RewardCardPointsTransactionLogs.AddRange(newLogsToAdd); // add new log
                        db.SystemLogs.Add(new SystemLog
                        {
                            Type = "POINTS_UPDATE_MULTIPLIER",
                            Note = "Card:" + cardNumberToCheck + " will have " + actualPointsTotal + " points douubled over " + newLogsToAdd.Count + "visits",
                            WhenCreated = DateTime.Now,
                            WhoTriggered = "Nadav"
                        });
                        var rsult = db.SaveChanges();


                    }
                    else
                    {
                        db.SystemLogs.Add(new SystemLog
                        {
                            Type = "POINTS_UPDATE_MULTIPLIER_FAILURE",
                            Note = "Card:" + cardNumberToCheck + " failed as Revel couldn't update",
                            WhenCreated = DateTime.Now,
                            WhoTriggered = "Nadav"
                        });
                        var rsult = db.SaveChanges();

                        string errorFail = String.Format("<p>Card {0} double failed as couldn't update in Revel. Please check this error.</p>", card.number);
                        emailer.SendMessageGrindErrorAndNadavIgnoreSendExeceptions("GRINDAND CO - DANGER - POINTS DOUBLING MULTIPLIER FAILURE", errorFail);


                    }

                    //TEMP LOG


                }
                else
                {

                    if (newLogsToAdd.Count > 0)
                    {
                        db.RewardCardPointsTransactionLogs.AddRange(newLogsToAdd); // add new log
                        var rsult = db.SaveChanges();

                    }

                }

            }
        }





        public async Task SaveDaysTestWithNewRoutines()
        {
            var sync = new SyncController();

            await sync.SaveDaysSinceLastVisit(db);

        }

        public class CardUpdate
        {
            public CardUpdate(string _number, int points)
            {
                number = _number;
                newPoints = points;
            }
            public string number { get; set; }
            public int newPoints { get; set; }

        }

        public async Task UpdateCorruptedPointsOnAllSuffixCards()
        {
            var listOFUpdates = new List<CardUpdate>();
            listOFUpdates.Add(new CardUpdate("00035170", 84));
            listOFUpdates.Add(new CardUpdate("10000041", 90));
            listOFUpdates.Add(new CardUpdate("00041750", 8));
            listOFUpdates.Add(new CardUpdate("00046858", 90));
            listOFUpdates.Add(new CardUpdate("00041945", 90));
            listOFUpdates.Add(new CardUpdate("90003871", 90));
            listOFUpdates.Add(new CardUpdate("00041912", 70));
            listOFUpdates.Add(new CardUpdate("90002410", 90));
            listOFUpdates.Add(new CardUpdate("00041656", 80));
            listOFUpdates.Add(new CardUpdate("00020440", 78));
            listOFUpdates.Add(new CardUpdate("00041929", 98));
            listOFUpdates.Add(new CardUpdate("90000085", 103));
            listOFUpdates.Add(new CardUpdate("00033342", 88));
            listOFUpdates.Add(new CardUpdate("90000229", 72));
            listOFUpdates.Add(new CardUpdate("00041507", 34));
            listOFUpdates.Add(new CardUpdate("00041831", 62));
            listOFUpdates.Add(new CardUpdate("00041728", 48));
            listOFUpdates.Add(new CardUpdate("00041828", 18));
            listOFUpdates.Add(new CardUpdate("00037890", 76));
            listOFUpdates.Add(new CardUpdate("00041956", 130));
            listOFUpdates.Add(new CardUpdate("00024522", 120));
            listOFUpdates.Add(new CardUpdate("00041947", 38));
            listOFUpdates.Add(new CardUpdate("90002953", 80));
            listOFUpdates.Add(new CardUpdate("00041880", 28));
            listOFUpdates.Add(new CardUpdate("00041988", 60));
            listOFUpdates.Add(new CardUpdate("00041915", 20));
            listOFUpdates.Add(new CardUpdate("00041654", 22));
            listOFUpdates.Add(new CardUpdate("00026938", 80));
            listOFUpdates.Add(new CardUpdate("00041680", 30));
            listOFUpdates.Add(new CardUpdate("00041908", 10));
            listOFUpdates.Add(new CardUpdate("00041974", 10));
            listOFUpdates.Add(new CardUpdate("00041843", 30));
            listOFUpdates.Add(new CardUpdate("00041925", 20));
            listOFUpdates.Add(new CardUpdate("00041517", 22));
            listOFUpdates.Add(new CardUpdate("00041906", 26));
            listOFUpdates.Add(new CardUpdate("00041931", 24));
            listOFUpdates.Add(new CardUpdate("00041910", 12));
            listOFUpdates.Add(new CardUpdate("00028751", 32));
            listOFUpdates.Add(new CardUpdate("00041920", 16));
            listOFUpdates.Add(new CardUpdate("00025013", 100));
            listOFUpdates.Add(new CardUpdate("00026942", 86));
            listOFUpdates.Add(new CardUpdate("00041810", 38));
            listOFUpdates.Add(new CardUpdate("00041796", 12));
            listOFUpdates.Add(new CardUpdate("00041808", 10));
            listOFUpdates.Add(new CardUpdate("90003750", 18));
            listOFUpdates.Add(new CardUpdate("00030970", 90));
            listOFUpdates.Add(new CardUpdate("00041756", 28));
            listOFUpdates.Add(new CardUpdate("00035169", 76));
            listOFUpdates.Add(new CardUpdate("00036735", 18));
            listOFUpdates.Add(new CardUpdate("00041740", 16));
            listOFUpdates.Add(new CardUpdate("00041885", 22));
            listOFUpdates.Add(new CardUpdate("00044818", 26));
            listOFUpdates.Add(new CardUpdate("00041559", 12));
            listOFUpdates.Add(new CardUpdate("00041894", 8));
            listOFUpdates.Add(new CardUpdate("00020038", 74));
            listOFUpdates.Add(new CardUpdate("90002392", 24));
            listOFUpdates.Add(new CardUpdate("00041821", 10));
            listOFUpdates.Add(new CardUpdate("00041546", 10));
            listOFUpdates.Add(new CardUpdate("00041888", 34));
            listOFUpdates.Add(new CardUpdate("00041837", 12));
            listOFUpdates.Add(new CardUpdate("00041637", 8));
            listOFUpdates.Add(new CardUpdate("00041978", 10));
            listOFUpdates.Add(new CardUpdate("00041820", 24));
            listOFUpdates.Add(new CardUpdate("00041876", 12));
            listOFUpdates.Add(new CardUpdate("00041812", 20));
            listOFUpdates.Add(new CardUpdate("00041692", 30));
            listOFUpdates.Add(new CardUpdate("00041639", 30));
            listOFUpdates.Add(new CardUpdate("00022539", 62));
            listOFUpdates.Add(new CardUpdate("00041846", 36));
            listOFUpdates.Add(new CardUpdate("00041745", 34));
            listOFUpdates.Add(new CardUpdate("00035175", 40));
            listOFUpdates.Add(new CardUpdate("00041859", 18));
            listOFUpdates.Add(new CardUpdate("00035048", 30));
            listOFUpdates.Add(new CardUpdate("90003818", 30));
            listOFUpdates.Add(new CardUpdate("00036722", 28));
            listOFUpdates.Add(new CardUpdate("00038856", 68));
            listOFUpdates.Add(new CardUpdate("00037869", 62));
            listOFUpdates.Add(new CardUpdate("00020438", 22));
            listOFUpdates.Add(new CardUpdate("00037842", 20));
            listOFUpdates.Add(new CardUpdate("90003809", 18));
            listOFUpdates.Add(new CardUpdate("00041676", 18));
            listOFUpdates.Add(new CardUpdate("00026770", 30));
            listOFUpdates.Add(new CardUpdate("90000011", 60));
            listOFUpdates.Add(new CardUpdate("00026940", 28));
            listOFUpdates.Add(new CardUpdate("90000006", 62));
            listOFUpdates.Add(new CardUpdate("00020197", 38));
            listOFUpdates.Add(new CardUpdate("00035051", 46));
            listOFUpdates.Add(new CardUpdate("90003817", 18));
            listOFUpdates.Add(new CardUpdate("00021912", 48));
            listOFUpdates.Add(new CardUpdate("90002423", 50));
            listOFUpdates.Add(new CardUpdate("90001860", 42));
            listOFUpdates.Add(new CardUpdate("00041860", 8));
            listOFUpdates.Add(new CardUpdate("00020740", 18));
            listOFUpdates.Add(new CardUpdate("00036750", 20));
            listOFUpdates.Add(new CardUpdate("00041815", 18));
            listOFUpdates.Add(new CardUpdate("00020153", 38));
            listOFUpdates.Add(new CardUpdate("00025237", 32));
            listOFUpdates.Add(new CardUpdate("00020265", 22));
            listOFUpdates.Add(new CardUpdate("00041755", 0));
            listOFUpdates.Add(new CardUpdate("00041674", 10));
            listOFUpdates.Add(new CardUpdate("00041975", 0));
            listOFUpdates.Add(new CardUpdate("00037884", 0));
            listOFUpdates.Add(new CardUpdate("90003075", 8));
            listOFUpdates.Add(new CardUpdate("90002446", 8));
            listOFUpdates.Add(new CardUpdate("00020645", 30));
            listOFUpdates.Add(new CardUpdate("00022891", 32));
            listOFUpdates.Add(new CardUpdate("00041922", 22));
            listOFUpdates.Add(new CardUpdate("00041886", 20));
            listOFUpdates.Add(new CardUpdate("00041942", 12));
            listOFUpdates.Add(new CardUpdate("00041999", 12));
            listOFUpdates.Add(new CardUpdate("90003772", 0));
            listOFUpdates.Add(new CardUpdate("00020427", 31));
            listOFUpdates.Add(new CardUpdate("00038767", 8));
            listOFUpdates.Add(new CardUpdate("00026941", 27));
            listOFUpdates.Add(new CardUpdate("00028669", 10));
            listOFUpdates.Add(new CardUpdate("00020043", 24));
            listOFUpdates.Add(new CardUpdate("00020424", 20));
            listOFUpdates.Add(new CardUpdate("00020554", 19));
            listOFUpdates.Add(new CardUpdate("00020673", 17));
            listOFUpdates.Add(new CardUpdate("90002403", 16));



            var service = new CombinedCardCustomerController(db);

            var est = new Establishment(1, "Grind",
                RevelAPIKEY,
                 new Uri(RevelBaseURL));

            var writer = new WebserviceDataWriter(est, db);


            foreach (var card in listOFUpdates)
            {
                var numberToAugment = card.number;
                var existing = db.RewardsCardNew.FirstOrDefault(x => x.number.Equals(numberToAugment));

                if (db.SystemLogs.Where(x => x.Note == existing.number).FirstOrDefault() == null)
                {
                    if (existing != null)
                    {
                        var old = existing.current_points.ToString();
                        existing.current_points = card.newPoints;
                        if (await writer.UpdateRewardCard(existing) == 0)
                        {
                            db.SystemLogs.Add(new SystemLog
                            {

                                Note = existing.number,
                                Type = "NADAV ONE OFF POINTS REDUCTION - SPECIAL CASE, down to " + existing.current_points + " points from " + old,
                                WhenCreated = DateTime.Now
                            });
                            db.SaveChanges();
                        }
                        else
                        {
                            var didntWork = existing;

                        }

                    }

                }
                else
                {
                    var alreadyDIdThis = card.number;
                }


            }

            var allCOmpleted = "";


        }

    }


}


