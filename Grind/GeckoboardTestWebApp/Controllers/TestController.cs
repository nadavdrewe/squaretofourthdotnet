using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Excel;
using Geckoboard._808nd.com;
using GeckoboardLibrary.Classes;
using GeckoboardLibrary.Classes.WidgetItems;
using GeckoboardLibrary.Services;
using GeckoboardTestWebApp.Models;
using Revel._808nd.com.Classes;
using GeckoboardLibrary.Classes.Widgets;
using Revel._808nd.com.Classes.ServiceImplementaitons;
using Revel._808nd.com.Classes.WebserviceReader;
using Revel._808nd.com.Classes.WebserviceReaderImplementations;
using Revel._808nd.com.Extensions;
using System.Data;
using System.Data.Entity.Infrastructure;
using Revel._808nd.com.Classes.ServiceImplemenations;
using Revel._808nd.com.Interfaces;
using Revel._808nd.com.Models;
using Web.Grind._808nd.com.Controllers;
using System.Diagnostics;
/*using Revel._808nd.com.ExtensionMethods;*/

namespace GeckoboardTestWebApp.Controllers
{
    public class TestController : Controller
    {
        private GrindContext _db = new GrindContext();
        private string RevelAPIKEY { get; }
        private string RevelBaseURL { get; }
        //
        // GET: /Test/

        public TestController()
        {
            RevelAPIKEY = ConfigurationManager.AppSettings["RevelAPIKEY"];
            RevelBaseURL = ConfigurationManager.AppSettings["RevelBaseURL"];
            _db.Database.CommandTimeout = 300000;
        }


        public void TestNewLineWidget()
        {

            GeckoboardOrganisation shoreditchGrindOrganisation =
                new GeckoboardOrganisation("ab876212d31d37960e3154eb5e2bc0a0", "ShoreditchGrind");
            IGeckoboardObjectCreatorFactory factory = new GeckoboardObjectCreatorFactory(shoreditchGrindOrganisation);


            var widget = new LineV2Widget("ab876212d31d37960e3154eb5e2bc0a0",
                "https://push.geckoboard.com/v1/send/131888-5dba949e-c50a-48f3-8c1d-5de597a99a7e", "test",
                GeckoboardChartAndItemType.LineV2, 11);

            widget.type = GeckoboardChartAndItemType.LineV2;

            //setup
            var x = new LineV2XAsis
            {
                type = "",
                labels = new List<string>
                {
                    "Jan",
                    "Feb",
                    "March",
                    "April"
                },

            };


            widget.data.x_axis = x;
            widget.data.y_axis = new LineV2YAxis
            {
                format = "currency",
                unit = "USD"
            };

            var data1 = new LineV2Series
            {
                name = "test1",
                data = new List<decimal>
            {
                    1.00M,
                    2.00M,
                    3.00M,
                    4.00M

                }

            };

            var data2 = new LineV2Series
            {
                name = "test2",
                data = new List<decimal>
            {

                1.5M,
                2.5M,
                3.5M,
                5.5M
            }

            };


            widget.data.series = new List<LineV2Series>
            {
                data1,
                data2

            };

            var push = new GeckoboardPushService();
            var ok = push.Push(widget);

        }

        public async Task<bool> TestLinqPaymentOrdersItems()
        {
            Establishment revOrg = new Establishment(1, "Grind",
          RevelAPIKEY,
          new Uri(RevelBaseURL));


            RevelWebserviceDataReader webReader = new RevelWebserviceDataReader(revOrg);
            RevelDBWriter writer = new RevelDBWriter(_db);
            RevelDBReader DBReader = new RevelDBReader(revOrg);


            var db = new GrindContext();

            var sum = db.Payments.Where(x => x.created_date >= new DateTime(2013, 09, 01, 02, 00, 00))
                .Where(x => x.created_date <= new DateTime(2014, 10, 01, 02, 00, 00))
                .Sum(x => x.amount);

            return true;
        }

        public Task<List<Payment>> GetPaymentsFromDB(DateTime startDate, DateTime endDate)
        {
            Establishment revOrg = new Establishment(1, "Grind",
                RevelAPIKEY,
                new Uri(RevelBaseURL));


            RevelWebserviceDataReader webReader = new RevelWebserviceDataReader(revOrg);
            RevelDBWriter writer = new RevelDBWriter(_db);
            RevelDBReader DBReader = new RevelDBReader(revOrg);

            var payments = DBReader.GetPaymentsFixedStartTime(startDate, endDate);

            bool ok = false;

            return payments;
        }





        public async Task<bool> TestOrderItemDiscounts()
        {
            Establishment revOrg = new Establishment(1, "Grind",
              RevelAPIKEY,
              new Uri(RevelBaseURL));


            RevelWebserviceDataReader webReader = new RevelWebserviceDataReader(revOrg);
            RevelDBWriter writer = new RevelDBWriter(_db);
            RevelDBReader DBReader = new RevelDBReader(revOrg);


            bool ok = false;

            var paymentsFromDB = await DBReader.GetOrderItems(new DateTime(2014, 09, 01, 02, 00, 00), new DateTime(2014, 09, 02, 02, 00, 00));

            var discountedItems = paymentsFromDB.Where(x => x.discount_amount > 0)
                .OrderBy(x => x.parent_order_id);
            var discountCount = discountedItems.Count();

            foreach (var item in discountedItems)
            {

                var discountAmount = item.CalculateRealDiscountMoneyAmount();


            }


            return true;

        }


        public async Task<bool> GetDiscountsAndSaveToDB()
        {


            Establishment revOrg = new Establishment(1, "Grind",
                    RevelAPIKEY,
                    new Uri(RevelBaseURL));


            RevelWebserviceDataReader webReader = new RevelWebserviceDataReader(revOrg);
            RevelDBWriter writer = new RevelDBWriter(_db);
            RevelDBReader DBReader = new RevelDBReader(revOrg);


            bool ok = false;

            var paymentsFromDB = await DBReader.GetDiscounts();
            List<int> paymentIDs = (from payments in paymentsFromDB
                                    select (int)payments.id).ToList();

            var webServicePayments = await webReader.GetDiscounts();


            List<int> webServicePaymentIDs = (from payments in webServicePayments
                                              select (int)payments.id).ToList();


            //tests




            var paymentIdsToInsert = webServicePaymentIDs.Except(paymentIDs);

            GrindContext grindContext = new GrindContext();

            var discountsToInsert = new List<Discount>();

            foreach (var item in paymentIdsToInsert)
            {
                var discount = webServicePayments.Where(c => c.id == item).FirstOrDefault();
                discountsToInsert.Add(discount);

            }

            if (discountsToInsert.Any())
            {
                var paymentsOk = writer.SaveDiscounts(discountsToInsert);
            }


            return true;




        }


        public async Task<bool> MAINTAINANCE_UpdatePaymentsLastMonth()
        {

            var ok =
                await
                    GetPaymentsAndInsertMissingIntoDB(
                        new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, 1, 02, 00, 00),
                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 02, 00, 00));

            return true;
        }




        public async Task<bool> MAINTAINANCE_UpdatePaymentsYesterday()
        {

            var ok = await GetPaymentsAndInsertMissingIntoDB(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(-1).Day, 02, 00, 00), DateTime.Now);

            return true;
        }



        public async Task<bool> MAINTAINANCE_UpdatePaymentsToday()
        {

            var ok = await GetPaymentsAndInsertMissingIntoDB(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 02, 00, 00), DateTime.Now);

            return true;
        }



        public async Task<bool> MAINTAINANCE_UpdatePaymentManual()
        {

            var ok = await GetPaymentsAndInsertMissingIntoDB(new DateTime(2018, 04, 04, 03, 00, 00), new DateTime(2018, 04, 07, 03, 00, 00));

            return true;
        }

        public async Task<bool> GetPaymentsSinceLastPaymentInDbAndInsert()
        {
            using (var db = new GrindContext())
            {


                var lastPayment = db.Payments.Max(c => (DateTime)c.created_date);

                var ok = await GetPaymentsAndInsertMissingIntoDB(lastPayment, DateTime.Now);

                return true;

            }
        }

        public async Task<bool> MAINTAINANCE_GetPaymentsAndInsertMissingIntoDB()
        {

            var ok = await GetPaymentsAndInsertMissingIntoDB(new DateTime(2014, 10, 18, 02, 00, 00),
                new DateTime(2014, 10, 20, 02, 00, 00));

            return true;
        }



        public async Task<bool> GetPaymentsForTheLastWeekAndInsertMissingIntoDBSinglePull()
        {

            DateTime endDate = DateTime.Now;
            DateTime startDate = endDate.AddHours(-38);

            GrindContext grindContext = new GrindContext();

            grindContext.Establishments.ToList();

            Establishment revOrg = new Establishment(1, "Grind",
                         RevelAPIKEY,
                         new Uri(RevelBaseURL));


            RevelWebserviceDataReader webReader = new RevelWebserviceDataReader(revOrg);
            RevelDBWriter writer = new RevelDBWriter(grindContext);
            RevelDBReader DBReader = new RevelDBReader(revOrg);


            bool ok = false;

            List<Payment> paymentsFromDB =
                grindContext.Payments.Where(x => x.created_date >= startDate && x.created_date <= endDate).ToList();
            List<int> paymentIDs = (from payments in paymentsFromDB
                                    select (int)payments.id).ToList();

            var aPayment = new Payment();



            var query = @"/resources/Payment?format=json&created_date__gt={0}&created_date__lte={1}&limit=0";
            var startdateString = startDate.ToString("yyyy-MM-ddTHH:mm:ss");
            var endDateString = endDate.ToString("yyyy-MM-ddTHH:mm:ss");

            string webURL = String.Format(query,
                startdateString,
                endDateString);

            List<Payment> webServicePayments = await webReader.GetRevelWebserviceData<Payment>(aPayment, webURL);
            List<int> webServicePaymentIDs = (from payments in webServicePayments
                                              select (int)payments.id).ToList();


            var paymentIdsToInsert = webServicePaymentIDs.Except(paymentIDs);



            List<Payment> paymentsToInsert = new List<Payment>();

            foreach (var item in paymentIdsToInsert)
            {
                Payment paymentToInsert = webServicePayments.Where(c => c.id == item).FirstOrDefault();
                paymentsToInsert.Add(paymentToInsert);

            }


            var paymentstoInsertDupesRemoved = paymentsToInsert;
            if (paymentsToInsert.Any())
            {

                //get the ones that aren't already in the db

                foreach (var newWebPayment in paymentsToInsert)
                {
                    foreach (var existingdbpayment in paymentsFromDB)
                    {
                        if (newWebPayment.id == existingdbpayment.id)
                        {
                            var paymentToRemove =
                                paymentstoInsertDupesRemoved.Where(x => x.id == newWebPayment.id).First();
                            paymentstoInsertDupesRemoved.Remove(paymentToRemove);
                        }
                    }

                }


                if (paymentstoInsertDupesRemoved.Any())
                {
                    writer = new RevelDBWriter(new GrindContext());
                    var paymentsOk = writer.SavePayments(paymentstoInsertDupesRemoved);
                }
            }


            //now remove duplicated
            List<Payment> paymentsToBeDeletedFromDB = new List<Payment>();
            //referesh payments after inserts
            paymentsFromDB = await DBReader.GetPayments(startDate, endDate);

            //refresh reader and writer
            grindContext = new GrindContext();
            writer = new RevelDBWriter(grindContext);
            DBReader = new RevelDBReader(revOrg);

            //get all with duplicate records
            var duplicates = paymentsFromDB
                .GroupBy(x => x.id)
                .Where(e => e.Count() > 1)
                .SelectMany(g => g)
                .ToList();



            //these are distinct, delete them
            var listWithoutDupes = duplicates.GroupBy(p => p.id, (key, p) => p.FirstOrDefault())
                     .ToList();




            if (listWithoutDupes.Count > 0)
            {
                try
                {
                    grindContext.Payments.RemoveRange(listWithoutDupes);
                    grindContext.SaveChanges();

                }
                catch (Exception ex)
                {

                    foreach (var payment in listWithoutDupes)
                    {
                        grindContext.Payments.Attach(payment);
                        grindContext.Payments.Remove(payment);
                    }
                    grindContext.SaveChanges();
                }
            }


            return true;
        }



        public async Task<bool> GetPaymentsAndInsertMissingIntoDB(DateTime startDate, DateTime endDate)
        {
            GrindContext grindContext = new GrindContext();

            grindContext.Establishments.ToList();


            Establishment revOrg = new Establishment(1, "Grind",
              RevelAPIKEY,
              new Uri(RevelBaseURL));


            RevelWebserviceDataReader webReader = new RevelWebserviceDataReader(revOrg);
            RevelDBWriter writer = new RevelDBWriter(grindContext);
            RevelDBReader DBReader = new RevelDBReader(revOrg);


            bool ok = false;

            List<Payment> paymentsFromDB =
                grindContext.Payments.Where(x => x.created_date >= startDate && x.created_date <= endDate).ToList();
            List<int> paymentIDs = (from payments in paymentsFromDB
                                    select (int)payments.id).ToList();


            List<Payment> webServicePayments = await webReader.GetPayments(startDate, endDate);
            List<int> webServicePaymentIDs = (from payments in webServicePayments
                                              select (int)payments.id).ToList();


            var test = webServicePayments.Sum(x => x.amount);
            var testditch = webServicePayments.Where(x => x.establishment_id == 1).Sum(x => x.amount);
            var soho = webServicePayments.Where(x => x.establishment_id == 3).Sum(x => x.amount);
            var london = webServicePayments.Where(x => x.establishment_id == 4).Sum(x => x.amount);
            var hobly = webServicePayments.Where(x => x.establishment_id == 5).Sum(x => x.amount);
            var test2 = paymentsFromDB.Sum(x => x.amount);

            //delete same date any reinsert

            /*  grindContext.Payments.RemoveRange(paymentsFromDB);
              var okSave = grindContext.SaveChanges();

              /*grindContext.Payments.AddRange(webServicePayments);
              okSave = grindContext.SaveChanges();#1#
              foreach (var pay in webServicePayments)
              {
                  grindContext.Payments.Add(pay);
                  var saved = grindContext.SaveChanges();
                  if (saved == 0)
                  {
                      var thisdidnt = "";
                  }

              }*/

            /*
                        List<Payment> paymentsNow = await grindContext.Payments.Where(x => x.created_date >= startDate && x.created_date <= endDate).ToListAsync();
                        var testPOstInsert = grindContext.Payments.Where(x => x.created_date >= startDate && x.created_date <= endDate).Sum(x => x.amount);*/




            var paymentIdsToInsert = webServicePaymentIDs.Except(paymentIDs);



            List<Payment> paymentsToInsert = new List<Payment>();

            foreach (var item in paymentIdsToInsert)
            {
                Payment paymentToInsert = webServicePayments.Where(c => c.id == item).FirstOrDefault();
                paymentsToInsert.Add(paymentToInsert);

            }


            var paymentstoInsertDupesRemoved = paymentsToInsert;
            if (paymentsToInsert.Any())
            {

                //get the ones that aren't already in the db

                foreach (var newWebPayment in paymentsToInsert)
                {
                    foreach (var existingdbpayment in paymentsFromDB)
                    {
                        if (newWebPayment.id == existingdbpayment.id)
                        {
                            var paymentToRemove =
                                paymentstoInsertDupesRemoved.Where(x => x.id == newWebPayment.id).First();
                            paymentstoInsertDupesRemoved.Remove(paymentToRemove);
                        }
                    }

                }


                if (paymentstoInsertDupesRemoved.Any())
                {
                    writer = new RevelDBWriter(new GrindContext());
                    var paymentsOk = writer.SavePayments(paymentstoInsertDupesRemoved);
                }
            }


            //now remove duplicated
            List<Payment> paymentsToBeDeletedFromDB = new List<Payment>();
            //referesh payments after inserts
            paymentsFromDB = await DBReader.GetPayments(startDate, endDate);

            //refresh reader and writer
            grindContext = new GrindContext();
            writer = new RevelDBWriter(grindContext);
            DBReader = new RevelDBReader(revOrg);

            //get all with duplicate records
            var duplicates = paymentsFromDB
                .GroupBy(x => x.id)
                .Where(e => e.Count() > 1)
                .SelectMany(g => g)
                .ToList();



            //these are distinct, delete them
            var listWithoutDupes = duplicates.GroupBy(p => p.id, (key, p) => p.FirstOrDefault())
                     .ToList();




            if (listWithoutDupes.Count > 0)
            {
                try
                {
                    grindContext.Payments.RemoveRange(listWithoutDupes);
                    grindContext.SaveChanges();

                }
                catch (Exception ex)
                {

                    foreach (var payment in listWithoutDupes)
                    {
                        grindContext.Payments.Attach(payment);
                        grindContext.Payments.Remove(payment);
                    }
                    grindContext.SaveChanges();
                }
            }


            return true;
        }



        public async Task<bool> GECKOBOARD_PushAllDailyWidgets()
        {
            GrindContext db = new GrindContext();
            TestController tc = new TestController();


            GeckoboardOrganisation shoreditchGrindOrganisation =
                new GeckoboardOrganisation("ab876212d31d37960e3154eb5e2bc0a0", "ShoreditchGrind");
            IGeckoboardObjectCreatorFactory factory = new GeckoboardObjectCreatorFactory(shoreditchGrindOrganisation);


            //poll all new data and insert into DB
            var estSh = db.Establishments.FirstOrDefault(x => x.DBKEY_establishment_id == 13);
            estSh.api_key =
                RevelAPIKEY;
            estSh.BaseUri = new Uri(RevelBaseURL);
            var estSo = db.Establishments.FirstOrDefault(x => x.DBKEY_establishment_id == 12);
            estSo.api_key =
             RevelAPIKEY;
            estSo.BaseUri = new Uri(RevelBaseURL);

            var estLon = db.Establishments.FirstOrDefault(x => x.DBKEY_establishment_id == 15);
            estLon.api_key =
               RevelAPIKEY;
            estLon.BaseUri = new Uri(RevelBaseURL);

            var estHolb = db.Establishments.FirstOrDefault(x => x.DBKEY_establishment_id == 14);
            estHolb.api_key =
             RevelAPIKEY;
            estHolb.BaseUri = new Uri(RevelBaseURL);

            var estStrat = db.Establishments.FirstOrDefault(x => x.DBKEY_establishment_id == 25);
            estStrat.api_key =
           RevelAPIKEY;
            estStrat.BaseUri = new Uri(RevelBaseURL);

            var estRadio = db.Establishments.FirstOrDefault(x => x.DBKEY_establishment_id == 26);
            estRadio.api_key =
           RevelAPIKEY;
            estRadio.BaseUri = new Uri(RevelBaseURL);

            var estRoyalExchange = db.Establishments.FirstOrDefault(x => x.establishment_id == 6);
            estRoyalExchange.api_key =
           RevelAPIKEY;
            estRoyalExchange.BaseUri = new Uri(RevelBaseURL);

            var whitechapel = db.Establishments.FirstOrDefault(x => x.establishment_id == 9);
            whitechapel.api_key =
           RevelAPIKEY;
            whitechapel.BaseUri = new Uri(RevelBaseURL);

            var exmouth = db.Establishments.FirstOrDefault(x => x.establishment_id == 10);
            exmouth.api_key =
           RevelAPIKEY;
            exmouth.BaseUri = new Uri(RevelBaseURL);


            var Facebook = db.Establishments.FirstOrDefault(x => x.establishment_id == 11);
            Facebook.api_key =
           RevelAPIKEY;
            Facebook.BaseUri = new Uri(RevelBaseURL);


            var greenwich = db.Establishments.FirstOrDefault(x => x.establishment_id == 13);
            greenwich.api_key =
           RevelAPIKEY;
            greenwich.BaseUri = new Uri(RevelBaseURL);

            var liverpoolSt = db.Establishments.FirstOrDefault(x => x.establishment_id == 14);
            liverpoolSt.api_key =
           RevelAPIKEY;
            liverpoolSt.BaseUri = new Uri(RevelBaseURL);

            List<WidgetSetA> allSitesPreliminaryWidgetSets = new List<WidgetSetA>
            {
                new WidgetSetA
                {
                    RevelEstablishment =
                        estSh
                },
                new WidgetSetA
                {
                    RevelEstablishment =
                       estSo
                },
                new WidgetSetA
                {
                    RevelEstablishment =
                      estLon
                },
                      new WidgetSetA
                {
                    RevelEstablishment = estHolb
                },
                      new WidgetSetA()
                      {
                          RevelEstablishment = estStrat
                      },
                       new WidgetSetA()
                      {
                          RevelEstablishment = estRadio
                      },
                        new WidgetSetA
                {

                    RevelEstablishment =
                        estRoyalExchange
                },
                        new WidgetSetA
                        {
                            RevelEstablishment = exmouth
                        }
                        ,
                        new WidgetSetA
                        {
                            RevelEstablishment = whitechapel
                        },
                         new WidgetSetA
                        {
                            RevelEstablishment = Facebook
                        }
                         ,
                         new WidgetSetA
                        {
                            RevelEstablishment = greenwich
                        },
                           new WidgetSetA
                        {
                            RevelEstablishment = liverpoolSt
                        }

                 };

            WidgetSetFactory widgetSetFactory = new WidgetSetFactory();
            List<WidgetSetA> initialisedWidgetSets = new List<WidgetSetA>();

            //datasets
            var RollingPast6DaysToTodayEnd = DateTime.Now;

            var rolling6start = RollingPast6DaysToTodayEnd.AddHours(-168);
            var RollingPast6DaysToTodayStart = new DateTime(rolling6start.Year, rolling6start.Month, rolling6start.Day, rolling6start.Hour, rolling6start.Minute, rolling6start.Second);


            var RollingPast6DaysLastWeekEnd = RollingPast6DaysToTodayEnd.AddDays(-7);
            var rolling6startLastWeek = RollingPast6DaysLastWeekEnd.AddHours(-168);
            var RollingPast6DaysLastWeekStart = new DateTime(rolling6startLastWeek.Year, rolling6startLastWeek.Month, rolling6startLastWeek.Day, rolling6startLastWeek.Hour, rolling6startLastWeek.Minute, rolling6start.Second);


            var paymentsRollingPast6DaysToToday = db.Payments.Where(x => x.created_date >= RollingPast6DaysToTodayStart && x.created_date <= RollingPast6DaysToTodayEnd);
            var paymentRollingPast6DaysLastWeek = db.Payments.Where(x => x.created_date >= RollingPast6DaysLastWeekStart && x.created_date <= RollingPast6DaysLastWeekEnd);

            //init widgets
            foreach (var widgetSetA in allSitesPreliminaryWidgetSets)
            {

                try
                {
                    using (var emailer = new EmailController())
                    {

                        Stopwatch stopwatch = new Stopwatch();
                        // Begin timing.
                        stopwatch.Start();


                        //create DB reader
                        IRevelReaderAsync readerAsync = new RevelDBReader(widgetSetA.RevelEstablishment);

                        IRevelFactoryAsync revelFactory = new RevelFactoryAsyncLocalDb(readerAsync,
                            widgetSetA.RevelEstablishment);
                        widgetSetA.revelFactory = revelFactory; //set the factory as the DB implementation

                        widgetSetA.factory = factory; //gecko factory              


                        WidgetSetA widgetSetReturned =
                            await
                                widgetSetFactory.InitialiseWidgetSetADailyWidgets(widgetSetA,
                                    paymentsRollingPast6DaysToToday, paymentRollingPast6DaysLastWeek);

                        //quicker push time
                        await widgetSetFactory.PushWidgetsToGeckoboard(widgetSetReturned);

                        //email out and say we're done

                        stopwatch.Stop();
                        emailer.SendMessageNadavIgnoreSendExeceptions(
                            String.Format("Grind 7 min update service has finished for store:  {0} - it took {1} seconds",
                                widgetSetA.RevelEstablishment.establishment_id, stopwatch.Elapsed,
                                RevelBaseURL), null, "railgunit.maintenance@gmail.com");

                        initialisedWidgetSets.Add(widgetSetReturned);
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }

            }

            using (var emailer = new EmailController())
            {
                /*    emailer.SendMessageNadavIgnoreSendExeceptions("Grind 7 min update service has started parent widget set", null, "railgunit.maintenance@gmail.com");*/
                Stopwatch stopwatch = new Stopwatch();
                // Begin timing.
                stopwatch.Start();

                //PARENT WIDGET SET
                ParentWidgetSet ParentWidgetSet = new ParentWidgetSet
                {
                    RevelEstablishment =
                        new Establishment(2, "Parent",
                            RevelAPIKEY,
                            new Uri(RevelBaseURL)),

                    AllChildWidgetSets = initialisedWidgetSets
                };

                RevelFactory ParentRevelFactory = new RevelFactory(ParentWidgetSet.RevelEstablishment);
                ParentWidgetSet.revelFactory = ParentRevelFactory;
                ParentWidgetSet.factory = factory;


                ParentWidgetSet ParentWidgetSetReturned =
                    widgetSetFactory.InitialiseDailyParentWidgetSet(ParentWidgetSet, paymentsRollingPast6DaysToToday,
                        paymentRollingPast6DaysLastWeek);

                //push em out
                await widgetSetFactory.PushWidgetsToGeckoboard(ParentWidgetSetReturned);
                /*   emailer.SendMessageNadavIgnoreSendExeceptions(String.Format("Grind 7 min update service has finished for parent widget set, took: {0}", stopwatch.Elapsed), null, "railgunit.maintenance@gmail.com");*/

            }

            return true;


        }


        public async Task<List<Order>> HELPER_ReturnListOfOrdersNotInDB(List<Order> theOrdersFromWebservice,
            List<Order> theOrdersFromLocalStore)
        {

            try
            {
                var RevelEstablishment = new Establishment(1, "Shoreditch",
                    RevelAPIKEY,
                    new Uri(RevelBaseURL));

                IRevelReaderAsync DBReader = new RevelDBReader(RevelEstablishment);




                List<int> dbOrderIDs = (from orders in theOrdersFromLocalStore
                                            //GET order from DB
                                        select (int)orders.order_id).ToList();


                List<int> passedInOrdersToCheck = (from orders in theOrdersFromWebservice
                                                       // where orders.closed == true
                                                   select (int)orders.order_id).ToList();

                //now get

                var orderIdsToInsert = passedInOrdersToCheck.Except(dbOrderIDs);

                GrindContext grindContext = new GrindContext();

                List<Order> ordersToInsert = new List<Order>();

                foreach (var item in orderIdsToInsert)
                {
                    Order OrderToInsert = theOrdersFromWebservice.Where(c => c.order_id == item).FirstOrDefault();
                    ordersToInsert.Add(OrderToInsert);

                }

                return ordersToInsert;
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        public async Task<List<OrderItem>> HELPER_ReturnListOfOrderItemsNotInDB(
            List<OrderItem> theOrderItemsFromWebservice, List<OrderItem> theOrderItemsFromTheDB)
        {

            List<OrderItem> theOrderItemsToReturn = new List<OrderItem>();

            try
            {


                List<int> dbOrderItemIDs = (from orderItems in theOrderItemsFromTheDB
                                            select (int)orderItems.orderitem_id).ToList();


                List<int> webServiceOrderItemIDs = (from orderItems in theOrderItemsFromWebservice
                                                        // where orders.closed == true
                                                    select (int)orderItems.orderitem_id).ToList();

                //now get

                var orderItemIdsToInsert = webServiceOrderItemIDs.Except(dbOrderItemIDs);

                GrindContext grindContext = new GrindContext();

                List<OrderItem> orderItemsToInsert = new List<OrderItem>();

                foreach (var item in orderItemIdsToInsert)
                {
                    OrderItem OrderToInsert =
                        theOrderItemsFromWebservice.Where(c => c.orderitem_id == item).FirstOrDefault();
                    orderItemsToInsert.Add(OrderToInsert);

                }



                //check any specific orders exist in new order set
                //missing orders



                return orderItemsToInsert;

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }






        public async Task<int> GetAllDailyOrdersAndInsertAnyMissingRecords(DateTime startDate, DateTime endDate)
        {
            /*  Establishment revOrg = new Establishment(1, "Grind",
                 "be9685e8ca1847959350571318aa6f0f:da848e35fabd4f41a1bcb59268c3ad1ecef62b6c6f3e4e82a5faf443d0f8242e",
                 new Uri("https://testshoreditchgrind.revelup.com/"));*/

            Establishment revOrg = new Establishment(1, "Grind",
                RevelAPIKEY,
                new Uri(RevelBaseURL));


            RevelWebserviceDataReader webReader = new RevelWebserviceDataReader(revOrg);
            RevelDBWriter writer = new RevelDBWriter(_db);
            RevelDBReader DBReader = new RevelDBReader(revOrg);


            bool ok = false;


            //get all orders from our DB for that day
            try
            {
                List<Order> ordersFromDB = await DBReader.GetOrdersSinglePull(startDate, endDate);
                List<int> dbOrderIDs = (from orders in ordersFromDB
                                        select (int)orders.order_id).ToList();

                List<Order> webServiceOrders = await webReader.GetOrdersSinglePull(startDate, endDate);
                List<int> webServiceOrderIDs = (from orders in webServiceOrders
                                                    // where orders.closed == true
                                                select (int)orders.order_id).ToList();




                //WEBSERVICE VARS
                var web_closedORders = webServiceOrders; //.Where(x => x.closed = true);
                var web_closedAndUnpaidFalse = webServiceOrders
                    /*.Where(x => x.closed = true)
                    .Where(u => u.is_unpaid == "False")*/
                    .ToList();


                //webservice calcs
                //    var web_closedORdersSUM = web_closedORders.Sum(x => x.final_total);
                var web_closedAndUnpaidFalseSUM = web_closedAndUnpaidFalse.Sum(d => d.final_total);
                var web_closeAndUnpaidTAX = web_closedAndUnpaidFalse.Sum(x => x.tax);
                //var web_closedTaxSUM = web_closedORders.Sum(x => x.tax);


                //DB VARS                
                var db_closedORders = ordersFromDB //.Where(x => x.closed = true)
                    .ToList();
                var db_closedAndUnpaidFalse = ordersFromDB //.Where(x => x.closed = true)
                                                           //.Where(u => u.is_unpaid == "False")
                    .ToList();


                //db calcs
                //  var db_closedORdersSUM = db_closedORders.Sum(x => x.final_total);
                var db_closedAndUnpaidFalseSUM = db_closedAndUnpaidFalse.Sum(d => d.final_total);
                var db_ClosedAndUnpaidTAX = db_closedAndUnpaidFalse.Sum(x => x.tax);
                //var db_closedTaxSUM = db_closedORders.Sum(x => x.tax);




                var orderIdsToInsert = webServiceOrderIDs.Except(dbOrderIDs);

                GrindContext grindContext = new GrindContext();

                List<Order> ordersToInsert = new List<Order>();

                foreach (var item in orderIdsToInsert)
                {
                    Order OrderToInsert = webServiceOrders.Where(c => c.order_id == item).FirstOrDefault();
                    ordersToInsert.Add(OrderToInsert);

                }

                if (ordersToInsert.Any())
                {
                    grindContext.Orders.AddRange(ordersToInsert);
                    grindContext.SaveChanges();
                }

                //end insert


                //orders to be replaced
                List<Order> OrdersToBeReplacedInDB = new List<Order>();
                List<Order> OrdersToBeDeletedFromDb = new List<Order>();

                //refresh DB after insert above
                ordersFromDB = await DBReader.GetOrdersSinglePull(startDate, endDate);

                //create new context with all records
                var InsertGrindContext = new GrindContext();

                //cycle through web closed Order
                foreach (Order webOrder in web_closedAndUnpaidFalse)
                {
                    //cycle through ALL DB orders!
                    foreach (Order dbOrder in ordersFromDB)
                    {
                        if (webOrder.order_id == dbOrder.order_id)
                        {

                            // OrdersToBeDeletedFromDb.Add(webOrder);
                            if (!((webOrder.closed).Equals(dbOrder.closed))
                                ||
                                !((webOrder.is_unpaid).Equals(dbOrder.is_unpaid))
                                ||
                                !((decimal.Round(webOrder.final_total, 2)).Equals((decimal.Round(dbOrder.final_total))))
                                || !((decimal.Round(webOrder.tax)).Equals(decimal.Round(dbOrder.tax)))
                                )
                            {
                                //check if it's already added

                                var exists =
                                    OrdersToBeDeletedFromDb.Where(e => e.DBKEY_order_id.Equals(dbOrder.DBKEY_order_id))
                                        .Count();

                                if (exists < 1)
                                {
                                    try
                                    {
                                        OrdersToBeDeletedFromDb.Add(dbOrder);
                                        OrdersToBeReplacedInDB.Add(webOrder);
                                    }
                                    catch (Exception ex)
                                    {

                                        throw ex;
                                    }
                                }

                            }



                        }
                    }
                }




                //delete old
                if (OrdersToBeDeletedFromDb.Any())
                {
                    foreach (var order in OrdersToBeDeletedFromDb)
                    {

                        //check it doesn't exist first - duplicates
                        try
                        {

                            InsertGrindContext.Orders.Remove(order);

                        }

                        catch (Exception EX)
                        {
                            //need to add it probs
                            try
                            {
                                InsertGrindContext.Orders.Attach(order);
                                InsertGrindContext.Entry(order).State = EntityState.Modified;

                                InsertGrindContext.Orders.Remove(order);

                            }
                            catch (Exception ex)
                            {
                                var omg = "";
                                throw ex;
                            }
                            finally
                            {
                                //          InsertGrindContext.SaveChanges();
                            }

                        }
                    }


                    try
                    {

                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }

                    InsertGrindContext.SaveChanges();
                }

                //add new
                if (OrdersToBeReplacedInDB.Any())
                {

                    //check it doesn't exist first - duplicates
                    InsertGrindContext.Orders.AddRange(OrdersToBeReplacedInDB);
                    InsertGrindContext.SaveChanges();

                }




                return ordersToInsert.Count();
            }
            catch (Exception ex)
            {

                throw ex;
            }




        }



        public async Task<int> GetAllDailyOrderItemsAndInsertAnyMissingRecords(DateTime startDate, DateTime endDate)
        {
            var orderItemService = new OrderItemsService();
            return await orderItemService.GetAllDailyOrderItemsAndInsertAnyMissingRecords(RevelAPIKEY, RevelBaseURL, _db,
                startDate, endDate);
        }


        public async Task<int> TIMER_GetAllMissingOrdersAndOrderItemsForTodayAndInsertAnyMissingRecords()
        {
            var okReturnedRecords = 0;

            DateTime today = RevelHelper.WrapAllRevelStartingDatesInThisMethod(DateTime.Now);
            DateTime tomorrow = DateTime.Now.AddDays(1);


            //UNCOMMENT THIS FOR PRODUCTION
            DateTime startDate = new DateTime(today.Year, today.Month, today.Day, 02, 00, 00),
                endDate = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 02, 00, 00);



            try
            {
                okReturnedRecords = await OPS_GetAllMissingOrdersAndInsertAnyMissingRecords(startDate, endDate);
                okReturnedRecords = await OPS_GetAllMissingOrderItemsAndInsertAnyMissingRecords(startDate, endDate);


                //clean up any duplicate records
                var dbHelper = new RevelHelper.DbHelper();

                var cleanUpDB = dbHelper.DeleteDuplicateOrdersFromDB();
                cleanUpDB = dbHelper.DeleteDuplicateOrderItemsFromDB();

                return 0;
            }
            catch (Exception exception)
            {

                throw exception;
            }


        }

        /// <summary>
        /// Order Items
        /// </summary>
        /// <returns></returns>
        public async Task<int> OPS_GetAllMissingOrderItemsAndInsertAnyMissingRecords(DateTime startDate,
            DateTime endDate)
        {

            var test = await GetAllDailyOrderItemsAndInsertAnyMissingRecords(startDate, endDate);


            return 0;
        }



        /// <summary>
        /// Orders
        /// </summary>
        /// <returns></returns>
        public async Task<int> OPS_GetAllMissingOrdersAndInsertAnyMissingRecords(DateTime startDate, DateTime endDate)
        {
            var orderservice = new OrderService(RevelAPIKEY, RevelBaseURL, _db);
            await orderservice.UpdateOrders(startDate, endDate);

            return 0;
        }




        /// <summary>
        /// ITEMS
        /// </summary>
        /// <returns></returns>
      /*  public async Task<int> Test_GetAllDailyOrderItemsAndInsertAnyMissingRecords()
        {
            DateTime startDate = new DateTime(2014, 07, 24, 02, 00, 00),
                endDate = new DateTime(2014, 07, 28, 02, 00, 00);

            var test = await GetAllDailyOrderItemsAndInsertAnyMissingRecords(startDate, endDate);


            return 0;
        }*/



        /// <summary>
        /// ORDERS
        /// </summary>
        /// <returns></returns>
        public async Task<int> TEST_GetAllMissingOrdersAndOrderItemsForTodayAndInsertAnyMissingRecords()
        {
            var okReturnedRecords = 0;


            DateTime startDate = new DateTime(2014, 08, 04, 02, 00, 00),
                endDate = new DateTime(2014, 08, 05, 02, 00, 00);

            try
            {
                okReturnedRecords = await OPS_GetAllMissingOrdersAndInsertAnyMissingRecords(startDate, endDate);
                //           okReturnedRecords = await OPS_GetAllMissingOrderItemsAndInsertAnyMissingRecords(startDate, endDate);


                //clean up any duplicate records
                var dbHelper = new RevelHelper.DbHelper();

                var cleanUpDB = dbHelper.DeleteDuplicateOrdersFromDB();
                cleanUpDB = dbHelper.DeleteDuplicateOrderItemsFromDB();

                return 0;
            }
            catch (Exception exception)
            {

                throw exception;
            }


        }



        public async Task<int> MAINTAINANCE_LastMonth_OrderSync()
        {
            try
            {
                DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, 1, 02, 00, 00);
                DateTime endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 02, 00, 00);

                var ok = await MAINTAINANCE_GetAllMissingOrdersAndInsertAnyMissingRecords(startDate, endDate);

                return 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<int> MAINTAINANCE_Manual_OrderSync()
        {
            try
            {
                DateTime startDate = new DateTime(2016, 03, 14, 02, 00, 00);
                DateTime endDate = new DateTime(2016, 03, 15, 02, 00, 00);

                var ok = await MAINTAINANCE_GetAllMissingOrdersAndInsertAnyMissingRecords(startDate, endDate);

                return 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        //ORDERITEM SYNC

        public async Task<int> MAINTAINANCE_Today_CombinedOrderAndOrderItemSync()
        {
            try
            {
                DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 02, 00, 00);
                DateTime endDate = startDate.AddDays(1);

                var ok = await MAINTAINANCE_Today_OrderSync();
                ok = await MAINTAINANCE_Today_OrderItemSync();

                await GECKOBOARD_PushAllDailyWidgets();

                return 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        //ORDERS SYNC
        public async Task<int> MAINTAINANCE_Today_OrderSync()
        {
            try
            {
                DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 02, 00, 00);
                DateTime endDate = startDate.AddDays(1);

                var ok = await MAINTAINANCE_GetAllMissingOrdersAndInsertAnyMissingRecords(startDate, endDate);

                return 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<int> MAINTAINANCE_Yesterday_OrderSync()
        {
            try
            {
                DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(-1).Day, 02, 00, 00);
                DateTime endDate = startDate.AddDays(1);

                var ok = await MAINTAINANCE_GetAllMissingOrdersAndInsertAnyMissingRecords(startDate, endDate);

                return 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task<int> MAINTAINANCE_ThisWeek_OrderSync()
        {
            try
            {
                DateTime startDate =
                    new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 02, 00, 00).AddDays(-7);
                DateTime endDate = startDate.AddDays(8);

                var ok = await MAINTAINANCE_GetAllMissingOrdersAndInsertAnyMissingRecords(startDate, endDate);

                return 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public async Task<int> MAINTAINANCE_LastWeek_OrderSync()
        {
            try
            {
                DateTime startDate =
                    new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 02, 00, 00).AddDays(-14);
                DateTime endDate = startDate.AddDays(8);

                var ok = await MAINTAINANCE_GetAllMissingOrdersAndInsertAnyMissingRecords(startDate, endDate);

                return 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public async Task<int> MAINTAINANCE_GetAllMissingOrdersAndInsertAnyMissingRecords(DateTime startDate,
            DateTime endDate)
        {
            var okReturnedRecords = 0;


            try
            {
                okReturnedRecords = await OPS_GetAllMissingOrdersAndInsertAnyMissingRecords(startDate, endDate);


                //clean up any duplicate records
                var dbHelper = new RevelHelper.DbHelper();

                var cleanUpDB = dbHelper.DeleteDuplicateOrdersFromDB();
                cleanUpDB = dbHelper.DeleteDuplicateOrderItemsFromDB();

                return 0;
            }
            catch (Exception exception)
            {

                throw exception;
            }


        }


        //ORDERS SYNC
        public async Task<int> MAINTAINANCE_Today_OrderItemSync()
        {
            try
            {
                DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 02, 00, 00);
                DateTime endDate = startDate.AddDays(1);

                var ok = await OPS_GetAllMissingOrderItemsAndInsertAnyMissingRecords(startDate, endDate);

                //clean up any duplicate records
                var dbHelper = new RevelHelper.DbHelper();

                var cleanUpDB = dbHelper.DeleteDuplicateOrderItemsFromDB();


                return 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public async Task<int> MAINTAINANCE_Yesterday_OrderItemSync()
        {
            try
            {
                DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(-1).Day, 02, 00, 00);
                DateTime endDate = startDate.AddDays(1);

                var ok = await OPS_GetAllMissingOrderItemsAndInsertAnyMissingRecords(startDate, endDate);

                //clean up any duplicate records
                var dbHelper = new RevelHelper.DbHelper();

                var cleanUpDB = dbHelper.DeleteDuplicateOrderItemsFromDB();


                return 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



        public async Task<int> MAINTAINANCE_LastMonth_OrderItemSync()
        {
            try
            {
                DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, 1, 02, 00, 00);
                DateTime endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 02, 00, 00);

                var ok = await OPS_GetAllMissingOrderItemsAndInsertAnyMissingRecords(startDate, endDate);

                //clean up any duplicate records
                var dbHelper = new RevelHelper.DbHelper();

                var cleanUpDB = dbHelper.DeleteDuplicateOrderItemsFromDB();


                return 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public async Task<int> MAINTAINANCE_Manual_OrderItemSync()
        {
            try
            {
                DateTime startDate = new DateTime(2016, 01, 31, 02, 00, 00);
                DateTime endDate = new DateTime(2016, 02, 01, 02, 00, 00);

                var ok = await OPS_GetAllMissingOrderItemsAndInsertAnyMissingRecords(startDate, endDate);

                //clean up any duplicate records
                var dbHelper = new RevelHelper.DbHelper();

                var cleanUpDB = dbHelper.DeleteDuplicateOrderItemsFromDB();


                return 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<bool> TestRevelWebserviceReader()
        {
            Establishment revOrg = new Establishment(1, "Grind",
                RevelAPIKEY,
                new Uri(RevelBaseURL));


            RevelWebserviceDataReader webReader = new RevelWebserviceDataReader(revOrg);
            RevelDBWriter writer = new RevelDBWriter(_db);



            bool ok = false;



            /////////////TEST ORDERS
            //orders
            try
            {
                var lastOrder = Order.GetLastOrderDateTime();
                List<Order> theOrders = await webReader.GetOrdersSinglePull((DateTime)lastOrder, DateTime.Now);
                theOrders = theOrders.OrderBy(c => c.created_date).ToList();

                if (theOrders.Count > 0)
                {
                    try
                    {
                        //test if orders exist??
                        ok = writer.SaveOrders(theOrders);
                    }
                    catch (Exception ex)
                    {

                        //throw;
                    }

                }
            }
            catch (Exception ex)
            {

                //throw
            }


            return true;
        }

        public async Task<bool> TestRevelDBWebserviceReader()
        {


            Establishment revEst = new Establishment(1, "Grind",
                "be9685e8ca1847959350571318aa6f0f:da848e35fabd4f41a1bcb59268c3ad1ecef62b6c6f3e4e82a5faf443d0f8242e",
                new Uri(RevelBaseURL));

            RevelDBReader readerAsync = new RevelDBReader(revEst);
            RevelFactoryAsyncLocalDb revelFactoryAsync = new RevelFactoryAsyncLocalDb(readerAsync, revEst);


            var pandCatsWrapper = new RevelProductAndCategoryWrapper();
            var PandCTest = await revelFactoryAsync.CreateProductsAndCategories(pandCatsWrapper);

            var productCasts = await readerAsync.GetProductCategories();
            var prods = await readerAsync.GetProducts();



            var testStart = new DateTime(2014, 02, 25);
            var testEnd = new DateTime(2014, 04, 26);


            RevelOrderandOrderItemWrapper OrderWrappy = new RevelOrderandOrderItemWrapper(testStart, testEnd,
                RevelOrderandOrderItemWrapper.WrapperType.Order);

            var OandITest = await revelFactoryAsync.PopulateOrderAndItemWrapper(OrderWrappy);




            return true;
        }


        //public async Task<bool> TestSlackMessager()
        //{
        //    var slackclient = new Slack._808nd.com.Classes.SlackMessenger();
        //    var test = await slackclient.SendMessage("Test message from the app", "test", "AUser");


        //    return true;

        //}

        public void ReadExcel()
        {
            string filePath = "C:\test\test.xls";

            FileStream stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read);

            //1. Reading from a binary Excel file ('97-2003 format; *.xls)
            //     IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
            //...
            //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            //...
            //3. DataSet - The result of each spreadsheet will be created in the result.Tables
            //    DataSet result = excelReader.AsDataSet();
            //...
            //4. DataSet - Create column names from first row
            excelReader.IsFirstRowAsColumnNames = true;
            DataSet result = excelReader.AsDataSet();

            //5. Data Reader methods
            while (excelReader.Read())
            {
                //excelReader.GetInt32(0);
            }

            //6. Free resources (IExcelDataReader is IDisposable)
            excelReader.Close();


        }



        /*  public void TestList()
        {
            GeckoboardOrganisation shoreditchGrindOrganisation =
                   new GeckoboardOrganisation("ab876212d31d37960e3154eb5e2bc0a0", "ShoreditchGrind");
            GeckoboardObjectCreatorFactory factory = new GeckoboardObjectCreatorFactory(shoreditchGrindOrganisation);

            List<Item_List> items = new List<Item_List>
            {
                new Item_List{
                    description = "Test description",
                    label = new Label_List{
                        name="Test Name"
                    },
                    title = new Title_List
                    {
                        highlight = false,
                        text = "this is some test text"

                    }
                }

            };

            var list = factory.CreateList("TestList", "https://push.geckoboard.com/v1/send/54410-6733ba78-c1be-469d-b55a-2bb2394410e2", items);

            GeckoboardPushService push = new GeckoboardPushService();
            var ok = push.Push(list);

        }

        public void TestText()
        {

            GeckoboardOrganisation shoreditchGrindOrganisation =
                   new GeckoboardOrganisation("ab876212d31d37960e3154eb5e2bc0a0", "ShoreditchGrind");
            GeckoboardObjectCreatorFactory factory = new GeckoboardObjectCreatorFactory(shoreditchGrindOrganisation);

            List<Item_Text> items = new List<Item_Text>
            {
                new Item_Text{text ="Test", type=0},
                new Item_Text{text = "test2", type=1},
                new Item_Text{text = "test 3", type=2}
            };

            var text = factory.CreateText("testText", "https://push.geckoboard.com/v1/send/54410-4d324ef1-a9b5-46a6-aa00-3f6089c5e816", items);

            GeckoboardPushService push = new GeckoboardPushService();

            var test = push.Push(text);

        }
     

        public void TestBullet()
        {

            GeckoboardOrganisation shoreditchGrindOrganisation =
                   new GeckoboardOrganisation("ab876212d31d37960e3154eb5e2bc0a0", "ShoreditchGrind");
            GeckoboardObjectCreatorFactory factory = new GeckoboardObjectCreatorFactory(shoreditchGrindOrganisation);

            Dictionary<int, int> BudgetOldSt2014 = new Dictionary<int, int>();
            BudgetOldSt2014.Add(1, 74234);
            BudgetOldSt2014.Add(2, 74234);
            BudgetOldSt2014.Add(3, 76554);
            BudgetOldSt2014.Add(4, 76554);
            BudgetOldSt2014.Add(5, 76554);
            BudgetOldSt2014.Add(6, 78874);
            BudgetOldSt2014.Add(7, 78874);
            BudgetOldSt2014.Add(8, 78874);
            BudgetOldSt2014.Add(9, 78874);
            BudgetOldSt2014.Add(10, 81194);
            BudgetOldSt2014.Add(11, 81194);
            BudgetOldSt2014.Add(12, 76554);

            var currentMonthly = 123;
            var sameMonthLastYear = 123;
            var monthBudget = BudgetOldSt2014.Where(x => x.Key == DateTime.Now.Month).First().Value;

            BulletItem item = new BulletItem
            {
                label = "TestBullet",
                sublabel = "TestSublabel",


                axis = new BulletAxis
                {
                    point = new List<string>
                   {
                      "0","25000","40000","55000","70000","85000"                      
                   }
                },

                range = new List<BulletRange>
               {
                   new BulletRange{color="red", start=0, end=monthBudget-1}, 
                           new BulletRange{color="green", start=monthBudget, end=monthBudget + 10000}

               },

                measure = new BulletMeasure
                {
                    current = new BulletMeasureItem { start = "0", end = currentMonthly.ToString() },
                    //projected = new BulletMeasureItem { start="100", end=monthBudget.ToString() }

                },

                comparative = new BulletComparative { point = sameMonthLastYear.ToString() }
            };

            var BulletMonthlyBudget = factory.CreateBullet("BulletMonthlyBudget", "https://push.geckoboard.com/v1/send/54410-e8aaae74-f67c-441c-81d3-6998a49b2f78", "horizontal", item);

            GeckoboardPushService push = new GeckoboardPushService();

            var test = push.Push(BulletMonthlyBudget);
        }


        public void TestCreateLineWidget()
        {

            //21 hours in a Grind working day

            List<decimal> items = new List<decimal>();
            for (int i = 0; i < 6; i++)
            {
                int q = 0;
                items.Add(100 + q);
                q += 10;
            }

            List<string> axisX = new List<string>(); //cash
            List<decimal> axisY = new List<decimal>(); //cash
            for (int y = 0; y <= 500; y = y + 100)
            {
                axisY.Add(Convert.ToDecimal(y));
            }

            //for(int y = 6; y <= 23; y++)
            //{
            //    axisX.Add(y.ToString());
            //}
            axisX.Add("0");
            axisX.Add("1");
            axisX.Add("2");
            axisX.Add("3");

            LineSettings settings = new LineSettings
            {
                axisy = axisY,
                axisx = axisX,
                colour = null

            };

            //create a new Factory implementation   
            GeckoboardOrganisation shoreditchGrindOrganisation =
                new GeckoboardOrganisation("ab876212d31d37960e3154eb5e2bc0a0", "ShoreditchGrind");
            IGeckoboardObjectCreatorFactory factory = new GeckoboardObjectCreatorFactory(shoreditchGrindOrganisation);



            Line line = factory.CreateLine("testLine", "https://push.geckoboard.com/v1/send/54410-819b588f-87b8-471c-9967-63d4efbce153", items, settings);

            //convert the widgets to JSON via other service
            IGeckoboardPushService pushService = new GeckoboardPushService();
            var response = pushService.Push(line);



        }

        public void TestRevelDate()
        {
            DateTime Today = new DateTime(2014, 01, 02);

            RevelHelper.WrapAllRevelStartingDatesInThisMethod(Today);



        }*/


        public async Task UpdateUsers()
        {
            Establishment revOrg = new Establishment(1, "Grind",
             RevelAPIKEY,
             new Uri(RevelBaseURL));

            RevelFactory revelFactory = new RevelFactory(revOrg);

            RevelWebserviceDataReader webReader = new RevelWebserviceDataReader(revOrg);
            RevelDBWriter writer = new RevelDBWriter(_db);
            IRevelReaderAsync DBReader = new RevelDBReader(revOrg);

            var user = new User();
            var users = await webReader.GetRevelWebserviceData(user, user.theAddress);

            _db.Users.AddRange(users);
            _db.SaveChanges();
        }


        public async Task<bool> UpdateDatabaseProductsAndCategories()
        {
            Establishment revOrg = new Establishment(1, "Grind",
                RevelAPIKEY,
                new Uri(RevelBaseURL));

            RevelFactory revelFactory = new RevelFactory(revOrg);

            IRevelReaderAsync webReader = new RevelWebserviceDataReader(revOrg);
            RevelDBWriter writer = new RevelDBWriter(_db);
            IRevelReaderAsync DBReader = new RevelDBReader(revOrg);


            bool ok = false;

            //cats           

            try
            {

                var pcOk = await ProductCategory.CompareProductCategoriesAndInsertNewIntoDB(DBReader, webReader, writer);
                var pOk = await Product.CompareProductsDeleteOldAndInsertNewIntoDB(DBReader, webReader, writer);

            }
            catch (Exception)
            {

                throw;
            }


            return true;
        }

        public ActionResult DoNothing()
        {
            return View();
        }


        public async Task<IEnumerable<OrderItem>> UpdateDatabaseOrderItemsDeleteExistingInRangeReturnErrors(DateTime start, DateTime end)
        {
            Establishment revOrg = new Establishment(1, "Grind",
                RevelAPIKEY,
                new Uri(RevelBaseURL));


            RevelWebserviceDataReader webReader = new RevelWebserviceDataReader(revOrg);
            RevelDBWriter writer = new RevelDBWriter(_db);
            RevelDBReader DBReader = new RevelDBReader(revOrg);


            try
            {
                bool ok = false;

                //get new ordersItems in range
                var query = String.Format("/resources/OrderItem?format=json&created_date__gt={0}&created_date__lte={1}&limit=0", start.ToString("yyyy-MM-ddTHH:mm:ss"),
                    end.ToString("yyyy-MM-ddTHH:mm:ss"));

                List<OrderItem> oi = await webReader.GetRevelWebserviceData(new OrderItem(), query);
                List<OrderItem> errorItems = new List<OrderItem>();

                var oiFullyAssigned = OrderItem.AssignProductASKUAndEstablishmentToOrderItems(_db.Products.ToList(), oi,
                    out errorItems);

                //delete old orders in range
                var existing = _db.OrderItems.Where(x => x.created_date >= start && x.created_date <= end).ToList();

                _db.OrderItems.RemoveRange(existing);
                _db.OrderItems.AddRange(oiFullyAssigned);
                _db.SaveChanges();

                return errorItems;
            }
            catch (Exception ex)
            {

                throw new Exception("Unable to update orderItems");
            }
            //
        }


        public async Task<bool> UpdateDatabaseOrdersAndItems()
        {
            Establishment revOrg = new Establishment(1, "Grind",
                RevelAPIKEY,
                new Uri(RevelBaseURL));



            RevelWebserviceDataReader webReader = new RevelWebserviceDataReader(revOrg);
            RevelDBWriter writer = new RevelDBWriter(_db);
            RevelDBReader DBReader = new RevelDBReader(revOrg);


            bool ok = false;

            //orders
            try
            {
                var lastOrder = (DateTime)Order.GetLastOrderDateTime();

                //give it some extra time
                lastOrder = lastOrder.AddHours(-2);

                var query = String.Format("/resources/Order?format=json&created_date__gt={0}&created_date__lte={1}&limit=0", lastOrder.ToString("yyyy-MM-ddTHH:mm:ss"),
                    DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));

                List<Order> theOrders = await webReader.GetRevelWebserviceData(new Order(), query);
                theOrders = theOrders.OrderBy(c => c.created_date).ToList();
                var OrdersWithServiceCharge = theOrders.Where(x => x.service_charge > 0).ToList();

                var ordersInDB = await DBReader.GetOrdersSinglePull((DateTime)lastOrder, DateTime.Now);
                //get all the orders in the DB

                List<Order> OrdersNotinDB = await HELPER_ReturnListOfOrdersNotInDB(theOrders, ordersInDB);

                if (OrdersNotinDB.Count > 0)
                {
                    try
                    {
                        //test if orders exist??
                        ok = writer.SaveOrders(theOrders);
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }

                    //clean up any duplicate orders
                    var dbHelper = new RevelHelper.DbHelper();
                    var cleanDB = dbHelper.DeleteDuplicateOrdersFromDB();

                    //just make sure to delete all the dupes
                }

            }
            catch (Exception ex)
            {

                //throw
            }
            //orderitems
            try
            {
                var lastOrderItemDateTime = OrderItem.GetLastOrderItemDateTime();

                var lastOI = (DateTime)lastOrderItemDateTime;
                lastOI = lastOI.AddHours(-2);

                var query = String.Format("/resources/OrderItem?format=json&created_date__gt={0}&created_date__lte={1}&limit=0", lastOI.ToString("yyyy-MM-ddTHH:mm:ss"),
                   DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));

                List<OrderItem> oi = await webReader.GetRevelWebserviceData(new OrderItem(), query);

                List<OrderItem> errorItems;

                var oiFullyAssigned = OrderItem.AssignProductASKUAndEstablishmentToOrderItems(_db.Products.ToList(), oi,
                    out errorItems);

                if (errorItems.Count > 0)
                {

                }

                oiFullyAssigned = oiFullyAssigned.OrderBy(c => c.created_date).ToList();

                var orderItemsInDB = _db.OrderItems.AsNoTracking()
                    .Where(x => x.created_date >= lastOI)
                    .Where(x => x.created_date <= DateTime.Now).ToList();

                List<OrderItem> OrderItemsNotinDB = await HELPER_ReturnListOfOrderItemsNotInDB(oiFullyAssigned, orderItemsInDB);

                if (OrderItemsNotinDB.Count > 0)
                {
                    try
                    {
                        _db.OrderItems.AddRange(OrderItemsNotinDB);
                        _db.SaveChanges();
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }

                }
                else
                {

                }

                //go through existing orders and if there are changes update
                var itemsModified = new List<OrderItem>();

                foreach (var existingItem in orderItemsInDB)
                {
                    var itemFromServiceToCompareTo =
                        oiFullyAssigned.FirstOrDefault(x => x.orderitem_id == existingItem.orderitem_id);

                    if (itemFromServiceToCompareTo != null)
                    {

                        if (
                            decimal.Round(itemFromServiceToCompareTo.pure_sales, 3) != decimal.Round(existingItem.pure_sales, 3)
                            || itemFromServiceToCompareTo.ervc_type != existingItem.ervc_type
                            /* || itemFromServiceToCompareTo.discount_reason != existingItem.discount_reason*/
                            /*|| itemFromServiceToCompareTo.discount_amount != existingItem.discount_amount
                            || itemFromServiceToCompareTo.quantity != existingItem.quantity*/
                            )
                        {

                            itemsModified.Add(existingItem);

                            existingItem.pure_sales = itemFromServiceToCompareTo.pure_sales;
                            existingItem.ervc_type = itemFromServiceToCompareTo.ervc_type;
                            existingItem.discount_amount = itemFromServiceToCompareTo.discount_amount;
                            existingItem.discount_reason = itemFromServiceToCompareTo.discount_reason;
                            existingItem.quantity = itemFromServiceToCompareTo.quantity;
                            existingItem.tax_amount = itemFromServiceToCompareTo.tax_amount;

                            _db.OrderItems
                                .Attach(existingItem);
                            _db.Entry(existingItem).Property(x => x.pure_sales).IsModified = true;
                            _db.Entry(existingItem).Property(x => x.ervc_type).IsModified = true;
                            _db.Entry(existingItem).Property(x => x.discount_amount).IsModified = true;
                            _db.Entry(existingItem).Property(x => x.discount_reason).IsModified = true;
                            _db.Entry(existingItem).Property(x => x.quantity).IsModified = true;
                            _db.Entry(existingItem).Property(x => x.tax_amount).IsModified = true;

                        }
                    }

                }

                if (itemsModified.Count > 0)
                {
                    _db.SaveChanges();
                }

                /*   var dbHelper = new RevelHelper.DbHelper();
                   var cleanDB = dbHelper.DeleteDuplicateOrderItemsFromDB();*/

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return true;

        }



        public async Task<ActionResult> PopulateDBFromTestGrind()
        {

            Establishment revOrg = new Establishment(1, "Grind",
                RevelAPIKEY,
                new Uri(RevelBaseURL));


            RevelFactory revelFactory = new RevelFactory(revOrg);

            RevelWebserviceDataReader webReader = new RevelWebserviceDataReader(revOrg);

            //     List<ProductCategory> cats = await webReader.GetProductCategories();

            //     List<ProductCategory> catsDupes = new List<ProductCategory>();

            //       List<Product> prods = await revelFactory.CreateProductsNoEstablishment();

            //     cats = cats.Where(x => x.active.Equals(true)).ToList();

            RevelDBWriter writer = new RevelDBWriter(_db);







            /*  if (cats.Count > 0)
            {
                var ok = writer.SaveProductCategories(cats);
            }

            else
            {
                //log no orders
                var whathappened = "";
            }*/

            /*
                        if (prods.Count > 0)
                        {
                            var ok = writer.SaveProducts(prods);
                        }

                        else
                        {
                            //log no orders
                            var whathappened = "";
                        }*/



            var lastOrderItemDateTime = OrderItem.GetLastOrderItemDateTime();
            var lastORder = (DateTime)Order.GetLastOrderDateTime();

            //list of dates to populate
            // var startDate = new DateTime(2014, 06, 21, 02, 00, 00);
            var endDate = new DateTime(2014, 07, 16, 03, 00, 00);
            /*
                        var currentDate = startDate;
                        List<DateTime> allDatesToPopulate = new List<DateTime>();

                        while (currentDate <= endDate)
                        {
                            //add the date to the list
                            allDatesToPopulate.Add(currentDate);

                            //increment
                            currentDate = currentDate.AddDays(1);
                        }*/

            //ORDERS
            //pull all orders for that day then dump into the DB
            /*try
             {


                 List<Order> returnedOrders = await revelFactory.CreateOrdersNoEstablishment(lastORder, endDate);

                     writer = new RevelDBWriter();

                     if (returnedOrders.Count > 0)
                     {
                         var ok = writer.SaveOrders(returnedOrders);
                     }

                     else
                     {
                         //log no orders
                         var whathappened = "";
                     }

                
             }
             catch (Exception ex)
             {
                
                 throw ex;
             }*/


            //ORDERITEMS
            try
            {


                List<OrderItem> returnedOrderItem =
                    await revelFactory.CreateOrderItemsNoEstablishment((DateTime)lastOrderItemDateTime, endDate);

                writer = new RevelDBWriter(_db);

                if (returnedOrderItem.Count > 0)
                {
                    var ok = writer.SaveOrderItems(returnedOrderItem);
                }

                else
                {
                    //log no orders
                    var whathappened = "";
                }


            }
            catch (Exception ex)
            {

                throw ex;
            }

            //            ViewBag.cats = cats;*/

            return View();
        }

        public async Task<bool> RunOvernight()
        {
            Establishment revOrg = new Establishment(1, "Grind",
                RevelAPIKEY,
                new Uri(RevelBaseURL));
            RevelFactory revelFactory = new RevelFactory(revOrg);
            RevelProductAndCategoryWrapper pcWrapper = new RevelProductAndCategoryWrapper();
            //GECKOSETUP
            //create a new Factory implementation   
            GeckoboardOrganisation shoreditchGrindOrganisation =
                new GeckoboardOrganisation("ab876212d31d37960e3154eb5e2bc0a0", "ShoreditchGrind");
            IGeckoboardObjectCreatorFactory factory = new GeckoboardObjectCreatorFactory(shoreditchGrindOrganisation);
            IGeckoboardPushService pushService = new GeckoboardPushService();

            //populate collections
            await revelFactory.CreateProductsAndCategories(pcWrapper);


            //ITEM - 30 DAYS TO LAST FULL DAY, VS 30 DAYS PREVIOUS, VS SAME 30 DAYS LAST YEAR 

            //30 days previous
            DateTime ThirtyDaysToYesterdayEnd =
                RevelHelper.WrapAllRevelStartingDatesInThisMethod(DateTime.Now.AddDays(-1));
            DateTime ThirtyDaysToYesterdayStart = ThirtyDaysToYesterdayEnd.AddDays(-30);

            //30 days before that
            DateTime ThirtyDaysPreviousEnd =
                RevelHelper.WrapAllRevelStartingDatesInThisMethod(ThirtyDaysToYesterdayStart.AddDays(-1));
            DateTime ThirtyDaysPreviousStart = ThirtyDaysPreviousEnd.AddDays(-30);

            //30 days previous, last year
            DateTime ThirtyDaysToYesterdayEndLastYear =
                RevelHelper.WrapAllRevelStartingDatesInThisMethod(ThirtyDaysToYesterdayEnd.AddYears(-1));
            DateTime ThirtyDaysToYesterdayStartLastYear = ThirtyDaysToYesterdayEndLastYear.AddDays(-30);


            RevelOrderandOrderItemWrapper last30 = new RevelOrderandOrderItemWrapper(ThirtyDaysToYesterdayStart,
                ThirtyDaysToYesterdayEnd, RevelOrderandOrderItemWrapper.WrapperType.Order);
            await revelFactory.PopulateOrderAndItemWrapper(last30);
            RevelOrderandOrderItemWrapper last30Previous = new RevelOrderandOrderItemWrapper(ThirtyDaysPreviousStart,
                ThirtyDaysPreviousEnd, RevelOrderandOrderItemWrapper.WrapperType.Order);
            await revelFactory.PopulateOrderAndItemWrapper(last30Previous);
            RevelOrderandOrderItemWrapper last30LastYear =
                new RevelOrderandOrderItemWrapper(ThirtyDaysToYesterdayStartLastYear, ThirtyDaysToYesterdayEndLastYear,
                    RevelOrderandOrderItemWrapper.WrapperType.Order);
            await revelFactory.PopulateOrderAndItemWrapper(last30LastYear);

            //create item
            BulletItem last30Item = new BulletItem
            {
                label = "Last 30 days (GROSS£)",
                sublabel = "vs previous 30 days (red/green) and same 30 days last year (line)",

                axis = new BulletAxis
                {
                    point = new List<string>
                    {
                        "0",
                        "25000",
                        "40000",
                        "55000",
                        "70000",
                        "90000"
                    }
                },

                range = new List<BulletRange>
                {
                    new BulletRange
                    {
                        color = "red",
                        start = 0,
                        end = Convert.ToInt32(last30Previous.GetOrderTotalPoundsGross())
                    },
                    new BulletRange
                    {
                        color = "green",
                        start = Convert.ToInt32(last30Previous.GetOrderTotalPoundsGross()) + 1,
                        end = 90000
                    }

                },

                measure = new BulletMeasure
                {
                    current = new BulletMeasureItem { start = "0", end = last30.GetOrderTotalPoundsGross().ToString() },
                    projected = new BulletMeasureItem { start = "", end = "" }

                },

                comparative = new BulletComparative { point = last30LastYear.GetOrderTotalPoundsGross().ToString() }
            };


            var BulletLast30 = factory.CreateBullet(1, "BulletLast30",
                "https://push.geckoboard.com/v1/send/54410-e3761368-a0f7-4051-9a51-ace158885c63", "horizontal",
                last30Item);
            var last30push = pushService.Push(BulletLast30);
            //end





            //ITEM ZERO
            //dates
            DateTime LastWeekStart = DateTimeExtensions.StartOfWeek(DateTime.Now.AddDays((-7)), DayOfWeek.Monday);
            DateTime LastWeekEnd = LastWeekStart.AddDays(6);

            DateTime WeekBeforeLastStart = DateTime.Now.AddDays(-20);
            DateTime WeekBeforeLastStartEnd = WeekBeforeLastStart.AddDays(6);

            RevelOrderandOrderItemWrapper lastWeek = new RevelOrderandOrderItemWrapper(LastWeekStart, LastWeekEnd,
                RevelOrderandOrderItemWrapper.WrapperType.Order);
            RevelOrderandOrderItemWrapper weekBeforeLast = new RevelOrderandOrderItemWrapper(WeekBeforeLastStart,
                WeekBeforeLastStartEnd, RevelOrderandOrderItemWrapper.WrapperType.Order);

            await revelFactory.PopulateOrderAndItemWrapper(lastWeek);
            await revelFactory.PopulateOrderAndItemWrapper(weekBeforeLast);

            int LastWeekGross = (int)lastWeek.GetOrderTotalPoundsGross();
            int weekBeforeGross = (int)weekBeforeLast.GetOrderTotalPoundsGross();

            NumberSecondaryStat LastWeekNetVSweekBeforeNet = factory.CreateNumberSecondaryStat(1,
                "LastWeekNetVSweekBeforeNet",
                "https://push.geckoboard.com/v1/send/54410-50711c58-6f95-44f2-9c03-4bf7f971891d",
                "LastCompleteWeekNet £", LastWeekGross,
                "CompleteWeekBeforeNet", weekBeforeGross);

            await pushService.Push(LastWeekNetVSweekBeforeNet);


            //////////ITEM ONE//////// --Yesterday vs same day last week            
            //yesterday to today
            //setup
            RevelOrderandOrderItemWrapper yesterdaysOrders = new RevelOrderandOrderItemWrapper(
                DateTime.Now.AddDays(-1), DateTime.Now, RevelOrderandOrderItemWrapper.WrapperType.Full);
            RevelOrderandOrderItemWrapper yesterdaysLastWeekOrders =
                new RevelOrderandOrderItemWrapper(DateTime.Now.AddDays(-8), DateTime.Now.AddDays(-7),
                    RevelOrderandOrderItemWrapper.WrapperType.Full);
            await revelFactory.PopulateOrderAndItemWrapper(yesterdaysOrders);
            await revelFactory.PopulateOrderAndItemWrapper(yesterdaysLastWeekOrders);

            //process
            var yesterSalesGross = (int)yesterdaysOrders.Orders.Sum(x => x.final_total);

            var yesterLastweekSalesGross = (int)yesterdaysLastWeekOrders.Orders.Sum(x => x.final_total);


            //push
            NumberSecondaryStat yesterdayVSYesterdayLastWeek = factory.CreateNumberSecondaryStat(1,
                "yesterdayVSYesterdayLastWeek",
                "https://push.geckoboard.com/v1/send/54410-fe62415b-48bb-4755-9130-b93b6ffe11cc", "Yesterday £",
                yesterSalesGross,
                "Yesterday Last Week", yesterLastweekSalesGross);

            await pushService.Push(yesterdayVSYesterdayLastWeek);


            //ITEM TWO LAST MONTH

            int LastMonth = DateTime.Now.AddMonths(-1).Month;
            DateTime FirstDayOfLastMonth = new DateTime(2014, LastMonth, 01);
            DateTime FirstDayOfThisMonth = new DateTime(2014, DateTime.Now.Month, 01);

            RevelOrderandOrderItemWrapper lastMonth = new RevelOrderandOrderItemWrapper(FirstDayOfLastMonth,
                FirstDayOfThisMonth, RevelOrderandOrderItemWrapper.WrapperType.Order);

            await revelFactory.PopulateOrderAndItemWrapper(lastMonth);


            NumberSecondaryStat lastMonthVSBudget = factory.CreateNumberSecondaryStat(2, "LastWeekNetVSweekBeforeNet",
                "https://push.geckoboard.com/v1/send/54410-91b9e90d-24eb-42c8-b9e4-ef5a999fa3a5", "Last Month £",
                (int)lastMonth.GetOrderTotalPoundsNet(),
                "Budget", 73130);

            await pushService.Push(lastMonthVSBudget);








            /////////////////
            //ITEM 3 - LAST MONTH VS LAST YEAR BUDGET BULLET CHART
            ////////////////
            int lastMonthLastYearInt = DateTime.Now.AddMonths(-1).AddYears(-1).Month;

            DateTime FirstDayOfLastMonthLastYear = new DateTime(DateTime.Now.AddYears(-1).Year, lastMonthLastYearInt, 01);
            DateTime FirstDayOfThisMonthLastYear = new DateTime(DateTime.Now.AddYears(-1).Year, DateTime.Now.Month, 01);

            RevelOrderandOrderItemWrapper lastMonthLastYear =
                new RevelOrderandOrderItemWrapper(FirstDayOfLastMonthLastYear,
                    FirstDayOfThisMonthLastYear, RevelOrderandOrderItemWrapper.WrapperType.Order);

            await revelFactory.PopulateOrderAndItemWrapper(lastMonthLastYear);

            Dictionary<int, int> BudgetOldSt2014 = new Dictionary<int, int>();
            BudgetOldSt2014.Add(1, 74234);
            BudgetOldSt2014.Add(2, 74234);
            BudgetOldSt2014.Add(3, 76554);
            BudgetOldSt2014.Add(4, 76554);
            BudgetOldSt2014.Add(5, 76554);
            BudgetOldSt2014.Add(6, 78874);
            BudgetOldSt2014.Add(7, 78874);
            BudgetOldSt2014.Add(8, 78874);
            BudgetOldSt2014.Add(9, 78874);
            BudgetOldSt2014.Add(10, 81194);
            BudgetOldSt2014.Add(11, 81194);
            BudgetOldSt2014.Add(12, 76554);

            var currentMonthly = lastMonth.GetOrderTotalPoundsNet();
            var sameMonthLastYear = lastMonthLastYear.GetOrderTotalPoundsNet();

            var monthBudget = BudgetOldSt2014.Where(x => x.Key == DateTime.Now.Month).First().Value;

            BulletItem item = new BulletItem
            {
                label = "Last Month(NET£)",
                sublabel = "vs Last Month Last Year(NET£) and Budget (Green)",

                axis = new BulletAxis
                {
                    point = new List<string>
                    {
                        "0",
                        "25000",
                        "40000",
                        "55000",
                        "70000",
                        "90000"
                    }
                },

                range = new List<BulletRange>
                {
                    new BulletRange {color = "red", start = 0, end = monthBudget - 1},
                    new BulletRange {color = "green", start = monthBudget, end = 90000}

                },

                measure = new BulletMeasure
                {
                    current = new BulletMeasureItem { start = "0", end = currentMonthly.ToString() },
                    projected = new BulletMeasureItem { }

                },

                comparative = new BulletComparative { point = sameMonthLastYear.ToString() }
            };



            var BulletMonthlyBudget = factory.CreateBullet(1, "BulletMonthlyBudget",
                "https://push.geckoboard.com/v1/send/54410-86f28cca-d21a-4bec-8a69-995875b4a903", "horizontal", item);
            var test = pushService.Push(BulletMonthlyBudget);




            //ITEM FOUR - 365 day period

            //dates
            DateTime ThisYearStart = new DateTime(DateTime.Now.Year, 01, 01);
            DateTime YTDYesterday = DateTime.Now.AddDays(-1).AddYears(-1);

            DateTime LastYearStart = ThisYearStart.AddYears(-1);
            DateTime LastYearYesterday = YTDYesterday.AddYears(-1);

            //wrappers
            RevelOrderandOrderItemWrapper ThisYearStartToTodayWrapper = new RevelOrderandOrderItemWrapper(
                ThisYearStart, YTDYesterday, RevelOrderandOrderItemWrapper.WrapperType.Order);
            RevelOrderandOrderItemWrapper LastYearStartToTodayWrapper = new RevelOrderandOrderItemWrapper(
                LastYearStart, LastYearYesterday, RevelOrderandOrderItemWrapper.WrapperType.Order);

            await revelFactory.PopulateOrderAndItemWrapper(ThisYearStartToTodayWrapper);
            await revelFactory.PopulateOrderAndItemWrapper(LastYearStartToTodayWrapper);

            NumberSecondaryStat ThisYearStartToTodayWidget = factory.CreateNumberSecondaryStat(1, "ThisYearStartToToday",
                "https://push.geckoboard.com/v1/send/54410-d0e5bbf4-7176-456c-886b-197b8ebce1b5",
                "YTDYesterday", (int)ThisYearStartToTodayWrapper.GetOrderTotalPoundsGross(), "YTD Yesterday Last Year",
                (int)LastYearStartToTodayWrapper.GetOrderTotalPoundsGross());

            await pushService.Push(ThisYearStartToTodayWidget);



            return true;
        }


        public async Task<bool> RunSohoOvernight()
        {
            Establishment revOrg = new Establishment(3, "Grind",
                RevelAPIKEY,
                new Uri(RevelBaseURL));
            RevelFactory revelFactory = new RevelFactory(revOrg);
            RevelProductAndCategoryWrapper pcWrapper = new RevelProductAndCategoryWrapper();
            //GECKOSETUP
            //create a new Factory implementation   
            GeckoboardOrganisation shoreditchGrindOrganisation =
                new GeckoboardOrganisation("ab876212d31d37960e3154eb5e2bc0a0", "ShoreditchGrind");
            IGeckoboardObjectCreatorFactory factory = new GeckoboardObjectCreatorFactory(shoreditchGrindOrganisation);
            IGeckoboardPushService pushService = new GeckoboardPushService();

            //populate collections
            await revelFactory.CreateProductsAndCategories(pcWrapper);


            //ITEM ZERO
            //dates
            DateTime LastWeekStart = DateTimeExtensions.StartOfWeek(DateTime.Now.AddDays((-7)), DayOfWeek.Monday);
            DateTime LastWeekEnd = LastWeekStart.AddDays(6);

            DateTime WeekBeforeLastStart = DateTime.Now.AddDays(-20);
            DateTime WeekBeforeLastStartEnd = WeekBeforeLastStart.AddDays(6);

            RevelOrderandOrderItemWrapper lastWeek = new RevelOrderandOrderItemWrapper(LastWeekStart, LastWeekEnd,
                RevelOrderandOrderItemWrapper.WrapperType.Order);
            RevelOrderandOrderItemWrapper weekBeforeLast = new RevelOrderandOrderItemWrapper(WeekBeforeLastStart,
                WeekBeforeLastStartEnd, RevelOrderandOrderItemWrapper.WrapperType.Order);

            await revelFactory.PopulateOrderAndItemWrapper(lastWeek);
            await revelFactory.PopulateOrderAndItemWrapper(weekBeforeLast);

            int LastWeekGross = (int)lastWeek.GetOrderTotalPoundsGross();
            int weekBeforeGross = (int)weekBeforeLast.GetOrderTotalPoundsGross();

            NumberSecondaryStat LastWeekNetVSweekBeforeNet = factory.CreateNumberSecondaryStat(1,
                "LastWeekNetVSweekBeforeNet",
                "https://push.geckoboard.com/v1/send/54410-eb8d18c0-b4da-0131-6650-22000a1fabf9",
                "LastCompleteWeekNet £", LastWeekGross,
                "CompleteWeekBeforeNet", weekBeforeGross);

            await pushService.Push(LastWeekNetVSweekBeforeNet);


            //////////ITEM ONE//////// --Yesterday vs same day last week            
            //yesterday to today
            //setup
            RevelOrderandOrderItemWrapper yesterdaysOrders = new RevelOrderandOrderItemWrapper(
                DateTime.Now.AddDays(-1), DateTime.Now, RevelOrderandOrderItemWrapper.WrapperType.Full);
            RevelOrderandOrderItemWrapper yesterdaysLastWeekOrders =
                new RevelOrderandOrderItemWrapper(DateTime.Now.AddDays(-8), DateTime.Now.AddDays(-7),
                    RevelOrderandOrderItemWrapper.WrapperType.Full);
            await revelFactory.PopulateOrderAndItemWrapper(yesterdaysOrders);
            await revelFactory.PopulateOrderAndItemWrapper(yesterdaysLastWeekOrders);

            //process
            var yesterSalesGross = (int)yesterdaysOrders.Orders.Sum(x => x.final_total);

            var yesterLastweekSalesGross = (int)yesterdaysLastWeekOrders.Orders.Sum(x => x.final_total);


            //push
            NumberSecondaryStat yesterdayVSYesterdayLastWeek = factory.CreateNumberSecondaryStat(2,
                "yesterdayVSYesterdayLastWeek",
                "https://push.geckoboard.com/v1/send/54410-eb8bfc20-b4da-0131-664f-22000a1fabf9", "Yesterday £",
                yesterSalesGross,
                "Yesterday Last Week", yesterLastweekSalesGross);

            await pushService.Push(yesterdayVSYesterdayLastWeek);


            //ITEM TWO LAST MONTH

            int LastMonth = DateTime.Now.AddMonths(-1).Month;
            DateTime FirstDayOfLastMonth = new DateTime(2014, LastMonth, 01);
            DateTime FirstDayOfThisMonth = new DateTime(2014, DateTime.Now.Month, 01);

            RevelOrderandOrderItemWrapper lastMonth = new RevelOrderandOrderItemWrapper(FirstDayOfLastMonth,
                FirstDayOfThisMonth, RevelOrderandOrderItemWrapper.WrapperType.Order);

            await revelFactory.PopulateOrderAndItemWrapper(lastMonth);


            NumberSecondaryStat lastMonthVSBudget = factory.CreateNumberSecondaryStat(3, "LastWeekNetVSweekBeforeNet",
                "https://push.geckoboard.com/v1/send/54410-eb8e5300-b4da-0131-6651-22000a1fabf9", "Last Month £",
                (int)lastMonth.GetOrderTotalPoundsNet(),
                "Budget", 73130);

            await pushService.Push(lastMonthVSBudget);









            //ITEM THREE - 365 day period

            //dates
            DateTime ThisYearStart = new DateTime(DateTime.Now.Year, 01, 01);
            DateTime YTDYesterday = DateTime.Now.AddDays(-1).AddYears(-1);

            DateTime LastYearStart = ThisYearStart.AddYears(-1);
            DateTime LastYearYesterday = YTDYesterday.AddYears(-1);

            //wrappers
            RevelOrderandOrderItemWrapper ThisYearStartToTodayWrapper = new RevelOrderandOrderItemWrapper(
                ThisYearStart, YTDYesterday, RevelOrderandOrderItemWrapper.WrapperType.Order);
            RevelOrderandOrderItemWrapper LastYearStartToTodayWrapper = new RevelOrderandOrderItemWrapper(
                LastYearStart, LastYearYesterday, RevelOrderandOrderItemWrapper.WrapperType.Order);

            await revelFactory.PopulateOrderAndItemWrapper(ThisYearStartToTodayWrapper);
            await revelFactory.PopulateOrderAndItemWrapper(LastYearStartToTodayWrapper);

            NumberSecondaryStat ThisYearStartToTodayWidget = factory.CreateNumberSecondaryStat(4, "ThisYearStartToToday",
                "https://push.geckoboard.com/v1/send/54410-d0e5bbf4-7176-456c-886b-197b8ebce1b5",
                "YTDYesterday", (int)ThisYearStartToTodayWrapper.GetOrderTotalPoundsGross(), "YTD Yesterday Last Year",
                (int)LastYearStartToTodayWrapper.GetOrderTotalPoundsGross());

            await pushService.Push(ThisYearStartToTodayWidget);
            //ITEM 4 


            return true;
        }


        public async Task<bool> TestSohoRealWidgetsDaily()
        {
            /*

                        //REVEL SETUP

                        Establishment RevelEstablishment = new Establishment(3, "Grind", RevelAPIKEY, new Uri(RevelBaseURL));
                        RevelFactory revelFactory = new RevelFactory(RevelEstablishment);


                        RevelProductAndCategoryWrapper pcWrapper = new RevelProductAndCategoryWrapper();
                        //GECKOSETUP
                        //create a new Factory implementation   
                        GeckoboardOrganisation shoreditchGrindOrganisation =
                            new GeckoboardOrganisation("ab876212d31d37960e3154eb5e2bc0a0", "ShoreditchGrind");
                        IGeckoboardObjectCreatorFactory factory = new GeckoboardObjectCreatorFactory(shoreditchGrindOrganisation);
                        GeckoboardPushService pushService = new GeckoboardPushService();

                        //populate collections
                        //Prods and Cats
                        await revelFactory.CreateProductsAndCategories(pcWrapper);

                        List<Product> alcoholProducts = pcWrapper.GetProductsThatAreAlcohol(RevelEstablishment.establishment_id);
                        List<Product> foodProducts = pcWrapper.GetProductsThatAreFood(RevelEstablishment.establishment_id);
                        List<Product> hotDrinkProducts = pcWrapper.GetProductsThatAreHotDrinks(RevelEstablishment.establishment_id);


                        //end
                        //////////////
                        ///TODAY
                        //Orders
                        var today = RevelHelper.WrapAllRevelStartingDatesInThisMethod(DateTime.Now);

                        RevelOrderandOrderItemWrapper TodaysOrdersSoFar = new RevelOrderandOrderItemWrapper(today, today.AddDays(1), RevelOrderandOrderItemWrapper.WrapperType.Full);
                        RevelOrderandOrderItemWrapper YesterdaysOrders = new RevelOrderandOrderItemWrapper(today.AddDays(-1), today, RevelOrderandOrderItemWrapper.WrapperType.Full);
                        await revelFactory.PopulateOrderAndItemWrapper(TodaysOrdersSoFar);
                        await revelFactory.PopulateOrderAndItemWrapper(YesterdaysOrders);

                        List<OrderItem> anythingElse = new List<OrderItem>();
                        var TodaysBreakdown = pcWrapper.GetProductCategoryBreakdown(TodaysOrdersSoFar.OrderItems);
                        int NoOfHotDrinks = 0;
                        int NoOfSoftDrinks = 0;

                        decimal valueOfFoodSales = 0.00M;
                        decimal valueOfAlcoholSales = 0.00M;
                        decimal valueOFSoftDrinkSales = 0.00M;
                        decimal valueOfFoodInitialPrice = 0.00M;
                        decimal valueOfAlcoholInitialPrice = 0.00M;

                        decimal valueOfAlcoholPlusTax = 0.00M;
                        decimal valueOfFoodPlusTax = 0.00M;

                        foreach (var item in TodaysOrdersSoFar.OrderItems)
                        {
                            if (pcWrapper.isItemAlcohol(item, RevelEstablishment.establishment_id))
                            {
                                valueOfAlcoholSales += (item.price);
                                valueOfAlcoholInitialPrice += (item.initial_price);
                                valueOfAlcoholPlusTax += (item.total_price_after_tax);
                            }
                            else if (pcWrapper.isItemFood(item, RevelEstablishment.establishment_id))
                            {
                                valueOfFoodSales += (item.price);
                                valueOfFoodInitialPrice += (item.initial_price);
                                valueOfFoodPlusTax += (item.total_price_after_tax);
                            }
                            else if (pcWrapper.isItemHotDrink(item, RevelEstablishment.establishment_id))
                            {
                                NoOfHotDrinks += 1;

                            }
                            else if (pcWrapper.isItemSoftDrink(item, RevelEstablishment.establishment_id))
                            {
                                NoOfSoftDrinks += 1;
                                valueOFSoftDrinkSales += item.price;
                            }
                            else
                            {
                                anythingElse.Add(item);
                            }

                        }
                        ///////END TODAY

                        ///////////////////////
                        ////SAME DAY LAST WEEK
                        //////////////////////

                        DateTime TodaySameDayLastWeekMinusOne = RevelHelper.WrapAllRevelStartingDatesInThisMethod(DateTime.Now.AddDays(-7));
                        DateTime TodaySameDayLastWeek = RevelHelper.WrapAllRevelStartingDatesInThisMethod(DateTime.Now.AddDays(-7));

                        //wrappers

                        RevelOrderandOrderItemWrapper TodaySameDayLastWeekWrapper = new RevelOrderandOrderItemWrapper(TodaySameDayLastWeekMinusOne, TodaySameDayLastWeek, RevelOrderandOrderItemWrapper.WrapperType.Full_Time);

                        await revelFactory.PopulateOrderAndItemWrapper(TodaySameDayLastWeekWrapper);

                        //vars
                        List<OrderItem> sameDayLastWeekanythingElse = new List<OrderItem>();
                        var sameDayLastWeekBreakdown = pcWrapper.GetProductCategoryBreakdown(TodaysOrdersSoFar.OrderItems);
                        int sameDayLastWeekNoOfHotDrinks = 0;
                        int sameDayLastWeekNoOfSoftDrinks = 0;

                        decimal sameDayLastWeekvalueOfFoodSales = 0.00M;
                        decimal sameDayLastWeekvalueOfAlcoholSales = 0.00M;
                        decimal sameDayLastWeekValueOfSoftDrinkSales = 0.00M;
                        decimal sameDayLastWeekvalueOfFoodInitialPrice = 0.00M;
                        decimal sameDayLastWeekvalueOfAlcoholInitialPrice = 0.00M;

                        decimal sameDayLastWeekvalueOfAlcoholPlusTax = 0.00M;
                        decimal sameDayLastWeekvalueOfFoodPlusTax = 0.00M;

                        //end vars
                        foreach (var item in TodaySameDayLastWeekWrapper.OrderItems)
                        {
                            if (pcWrapper.isItemAlcohol(item, RevelEstablishment.establishment_id))
                            {
                                sameDayLastWeekvalueOfAlcoholSales += (item.price);
                                sameDayLastWeekvalueOfAlcoholInitialPrice += (item.initial_price);
                                sameDayLastWeekvalueOfAlcoholPlusTax += (item.total_price_after_tax);
                            }
                            else if (pcWrapper.isItemFood(item, RevelEstablishment.establishment_id))
                            {
                                sameDayLastWeekvalueOfFoodSales += (item.price);
                                sameDayLastWeekvalueOfFoodInitialPrice += (item.initial_price);
                                sameDayLastWeekvalueOfFoodPlusTax += (item.total_price_after_tax);
                            }
                            else if (pcWrapper.isItemHotDrink(item, RevelEstablishment.establishment_id))
                            {
                                sameDayLastWeekNoOfHotDrinks += 1;
                            }
                            else if (pcWrapper.isItemSoftDrink(item, RevelEstablishment.establishment_id))
                            {
                                sameDayLastWeekNoOfSoftDrinks += 1;
                                sameDayLastWeekValueOfSoftDrinkSales += item.price;
                            }
                            else
                            {
                                sameDayLastWeekanythingElse.Add(item);
                            }

                        }

                        ///////////////////////////
                        //END WRAPPERS
                        /////////////////////////

                        //////WIDGETS
                        ///////////////ITEM ONE
                        ////dates


                        NumberSecondaryStat ThisYearStartToTodayWidget = factory.CreateNumberSecondaryStat("TodaySameDayLastWeek",
                            "https://push.geckoboard.com/v1/send/54410-eb839460-b4da-0131-6648-22000a1fabf9",
                            "Today", (int)TodaysOrdersSoFar.GetOrderTotalPoundsGross(), "Same Day Last Week",
                            (int)TodaySameDayLastWeekWrapper.GetOrderTotalPoundsGross());

                        await pushService.Push(ThisYearStartToTodayWidget);



                        /////////////ITEM TWO
                        Text AvgSpend = factory.CreateText("TodaysOrders", "https://push.geckoboard.com/v1/send/54410-310098b7-f683-4899-9376-c5468d7b8392", new List<Item_Text>());
                        await pushService.Push(AvgSpend,
                            GeckoboardPushService.ConvertToSingleFieldTextWidgetJSON(shoreditchGrindOrganisation.api_key,
                                Decimal.Round(TodaysOrdersSoFar.GetAvgSpendGross(), 2).ToString()));


                        /////////////ITEM THREE
                        NumberSecondaryStat NoOfOrdersToday = factory.CreateNumberSecondaryStat("NoOfOrdersToday",
                           "https://push.geckoboard.com/v1/send/54410-7d2e238b-4ff1-4b5e-a19b-9136c0d01cd9",
                           "Today", (int)TodaysOrdersSoFar.Orders.Count(), "Same Day Last Week",
                           (int)TodaySameDayLastWeekWrapper.Orders.Count());

                        await pushService.Push(NoOfOrdersToday);

                        /////////////ITEM FOUR
                        NumberSecondaryStat AlcoholSalesToday = factory.CreateNumberSecondaryStat("AlcoholSalesToday",
                       "https://push.geckoboard.com/v1/send/54410-bda512ac-549b-4109-8b7e-3e3bb65b5284",
                       "Today", Convert.ToInt32(valueOfAlcoholSales), "Same Day Last Week",
                       Convert.ToInt32(sameDayLastWeekvalueOfAlcoholSales));

                        await pushService.Push(AlcoholSalesToday);


                        /////////////ITEM FIVE
                        NumberSecondaryStat NoOfHotDrinksWidget = factory.CreateNumberSecondaryStat("NoOfHotDrinks",
                         "https://push.geckoboard.com/v1/send/54410-0b8503bb-6209-4565-9789-204dad45f2ef",
                         "Today", NoOfHotDrinks, "Same Day Last Week",
                         sameDayLastWeekNoOfHotDrinks);

                        await pushService.Push(NoOfHotDrinksWidget);

                        /////////////ITEM SIX
                        NumberSecondaryStat FoodSalesToday = factory.CreateNumberSecondaryStat("FoodSalesToday",
                         "https://push.geckoboard.com/v1/send/54410-fca4c3a6-d19c-4cf7-a9aa-3543deb76607",
                         "Today", Convert.ToInt32(valueOfFoodSales), "Same Day Last Week",
                         Convert.ToInt32(sameDayLastWeekvalueOfFoodSales));

                        await pushService.Push(FoodSalesToday);

                        /////////////ITEM SEVEN
                        Text LastUpdated = factory.CreateText("TodaysOrders", "https://push.geckoboard.com/v1/send/54410-4d5be68f-046b-478c-87c7-61a4ba635a71", new List<Item_Text>());
                        await pushService.Push(LastUpdated,
                            GeckoboardPushService.ConvertToSingleFieldTextWidgetJSON(shoreditchGrindOrganisation.api_key,
                                DateTime.Now.ToString()));


                        /////////////ITEM EIGHT
                        NumberSecondaryStat NoOfSoftDrinksWidget = factory.CreateNumberSecondaryStat("NoOfSoftDrinks",
                         "https://push.geckoboard.com/v1/send/54410-1a234e47-3d67-4bf6-b6f6-851f87cfba6e",
                         "Today", Convert.ToInt32(valueOFSoftDrinkSales), "Same Day Last Week",
                         Convert.ToInt32(sameDayLastWeekValueOfSoftDrinkSales));

                        await pushService.Push(NoOfSoftDrinksWidget);

  

                        ///ITEM NINE
                        Dictionary<int, decimal> HourAndSpend = new Dictionary<int, decimal>();

                        //do the hours from 6 am - 24.00
                        for (int i = 6; i < 24; i++)
                        {
                            var currentAccumulatedHourlySpend = 0.00M;
                            int currentHour = i;

                            //if the first two digits of the order match our range
                            foreach (var order in TodaysOrdersSoFar.Orders)
                            {
                                int hourOfOrder = Convert.ToInt16(order.created_date.ToString("HH"));

                                if (hourOfOrder.Equals(currentHour))
                                { currentAccumulatedHourlySpend += order.final_total; }

                            }

                            //we've done the hour, now add to the dictionary
                            HourAndSpend.Add(currentHour, currentAccumulatedHourlySpend);
                        }

                        //do the hours from 24.00 - 03.00
                        for (int i = 0; i <= 3; i++)
                        {
                            var currentAccumulatedHourlySpend = 0.00M;
                            int currentHour = i;

                            //if the first two digits of the order match our range
                            foreach (var order in TodaysOrdersSoFar.Orders)
                            {
                                int hourOfOrder = Convert.ToInt16(order.created_date.ToString("HH"));

                                if (hourOfOrder.Equals(currentHour))
                                { currentAccumulatedHourlySpend += order.final_total; }

                            }

                            //we've done the hour, now add to the dictionary
                            HourAndSpend.Add(currentHour, currentAccumulatedHourlySpend);
                        }

                        //create widget items
                        List<string> axisX = new List<string>(); //cash
                        List<decimal> axisY = new List<decimal>(); //time
                        for (int y = 0; y <= 250; y = y + 50)
                        {
                            axisY.Add(Convert.ToDecimal(y));
                        }

                        foreach (var item in HourAndSpend.Keys)
                        {
                            axisX.Add(item.ToString());
                        }


                        LineSettings settings = new LineSettings
                        {
                            axisy = axisY,
                            axisx = axisX,
                            colour = null

                        };

                        //create items
                        List<decimal> items = new List<decimal>();
                        foreach (var item in HourAndSpend.Values)
                        {
                            items.Add(item);
                        }

                        //create widget
                        Line line = factory.CreateLine("testLine", "https://push.geckoboard.com/v1/send/54410-ba257efc-5d17-470e-a30f-65e556d2996c", items, settings);
                        await pushService.Push(line);
            */




            return true;

        }




        public async Task<bool> TestWrapper()
        {

            //create prod and cat wrapper


            Establishment revOrg = new Establishment(1, "Grind",
                RevelAPIKEY,
                new Uri(RevelBaseURL));

            RevelFactory revelFactory = new RevelFactory(revOrg);

            RevelProductAndCategoryWrapper pcWrapper = new RevelProductAndCategoryWrapper();
            await revelFactory.CreateProductsAndCategories(pcWrapper);

            RevelOrderandOrderItemWrapper wrap = new RevelOrderandOrderItemWrapper(new DateTime(2014, 03, 26),
                new DateTime(2014, 03, 27), RevelOrderandOrderItemWrapper.WrapperType.Full);
            await revelFactory.PopulateOrderAndItemWrapper(wrap);

            Dictionary<string, int> productBreakdown = new Dictionary<string, int>();
            List<OrderItem> OrderItemErrorList = new List<OrderItem>();


            var breakdown = pcWrapper.GetProductCategoryBreakdown(wrap.OrderItems);

            //  RevelOrderandOrderItemWrapper wrap2 = new RevelOrderandOrderItemWrapper(new DateTime(2014, 03, 26), new DateTime(2014, 03, 30));
            //   await revelFactory.CreateOrderAndSubsequentItems(wrap2);

            return true;

        }


        public async Task<bool> TestRevelCreateProduct()
        {
            //create products

            Establishment revOrg = new Establishment(1, "Grind",
                RevelAPIKEY,
                new Uri(RevelBaseURL));

            RevelFactory revelFactory = new RevelFactory(revOrg);

            //try and create some collections with the factory

            List<Product> products = new List<Product>();

            var eh = await revelFactory.CreateProducts(products);

            return true;
        }

        public async Task<bool> TestRevelCreateProductCategory()
        {
            //create products

            Establishment revOrg = new Establishment(1, "Grind",
                RevelAPIKEY,
                new Uri(RevelBaseURL));

            RevelFactory revelFactory = new RevelFactory(revOrg);

            //try and create some collections with the factory

            List<ProductCategory> ProdCats = new List<ProductCategory>();

            var eh = await revelFactory.CreateProductCategories(ProdCats);

            return true;
        }

        public async Task<bool> TestRevelCreateOrders()
        {
            //create products

            Establishment revOrg = new Establishment(1, "Grind",
                RevelAPIKEY,
                new Uri(RevelBaseURL));

            RevelFactory revelFactory = new RevelFactory(revOrg);

            //try and create some collections with the factory

            List<Order> Orders = new List<Order>();

            var eh = await revelFactory.CreateOrders(new DateTime(2014, 03, 26), new DateTime(2014, 03, 27), Orders);

            return true;
        }

        public async Task<bool> TestRevelCreateOrderItems()
        {
            //create products

            Establishment revOrg = new Establishment(1, "Grind",
                RevelAPIKEY,
                new Uri(RevelBaseURL));

            RevelFactory revelFactory = new RevelFactory(revOrg);

            //try and create some collections with the factory

            List<OrderItem> OrderItems = new List<OrderItem>();

            var eh =
                await revelFactory.CreateOrderItems(new DateTime(2014, 03, 26), new DateTime(2014, 03, 27), OrderItems);

            return true;
        }






        /* public void TestFactory()
        {
            //create a new Factory implementation   
            GeckoboardOrganisation shoreditchGrindOrganisation =
                new GeckoboardOrganisation("ab876212d31d37960e3154eb5e2bc0a0", "ShoreditchGrind");
            IGeckoboardObjectCreatorFactory factory = new GeckoboardObjectCreatorFactory(shoreditchGrindOrganisation);

            //create some widgets with our factory
            NumberSecondaryStat testNumberSecondaryStat = factory.CreateNumberSecondaryStat("TestChart",
                "https://push.geckoboard.com/v1/send/54410-1f993f31-fe95-4336-a917-96181a7bff74", "FirstTestStat", 120,
                "SecondTestStat", 60);

            //convert the widgets to JSON via other service
            IGeckoboardPushService pushService = new GeckoboardPushService();
            pushService.Push(testNumberSecondaryStat);


            List<Item_Text> someItems = new List<Item_Text>();
            someItems.Add(new Item_Text("Item one", 0));
            someItems.Add(new Item_Text("Item Two", 0));
            someItems.Add(new Item_Text("Item Three", 0));

            Text textTest = factory.CreateText("testText",
                "https://push.geckoboard.com/v1/send/54410-26212ab8-e27a-4120-8f09-3ba08bbe0663", someItems);

            pushService.Push(textTest);



        }*/




        public async Task<bool> RunPicadillyOvernight()
        {
            Establishment revOrg = new Establishment(4, "Grind",
                RevelAPIKEY,
                new Uri(RevelBaseURL));
            RevelFactory revelFactory = new RevelFactory(revOrg);
            RevelProductAndCategoryWrapper pcWrapper = new RevelProductAndCategoryWrapper();
            //GECKOSETUP
            //create a new Factory implementation   
            GeckoboardOrganisation shoreditchGrindOrganisation =
                new GeckoboardOrganisation("ab876212d31d37960e3154eb5e2bc0a0", "ShoreditchGrind");
            IGeckoboardObjectCreatorFactory factory = new GeckoboardObjectCreatorFactory(shoreditchGrindOrganisation);
            IGeckoboardPushService pushService = new GeckoboardPushService();

            //populate collections
            await revelFactory.CreateProductsAndCategories(pcWrapper);


            //ITEM ZERO
            //dates
            DateTime LastWeekStart = DateTimeExtensions.StartOfWeek(DateTime.Now.AddDays((-7)), DayOfWeek.Monday);
            DateTime LastWeekEnd = LastWeekStart.AddDays(6);

            DateTime WeekBeforeLastStart = DateTime.Now.AddDays(-20);
            DateTime WeekBeforeLastStartEnd = WeekBeforeLastStart.AddDays(6);

            RevelOrderandOrderItemWrapper lastWeek = new RevelOrderandOrderItemWrapper(LastWeekStart, LastWeekEnd,
                RevelOrderandOrderItemWrapper.WrapperType.Order);
            RevelOrderandOrderItemWrapper weekBeforeLast = new RevelOrderandOrderItemWrapper(WeekBeforeLastStart,
                WeekBeforeLastStartEnd, RevelOrderandOrderItemWrapper.WrapperType.Order);

            await revelFactory.PopulateOrderAndItemWrapper(lastWeek);
            await revelFactory.PopulateOrderAndItemWrapper(weekBeforeLast);

            int LastWeekGross = (int)lastWeek.GetOrderTotalPoundsGross();
            int weekBeforeGross = (int)weekBeforeLast.GetOrderTotalPoundsGross();

            NumberSecondaryStat LastWeekNetVSweekBeforeNet = factory.CreateNumberSecondaryStat(1,
                "LastWeekNetVSweekBeforeNet",
                "https://push.geckoboard.com/v1/send/54410-7f11b010-c590-0131-a65d-22000a1e86ad",
                "LastCompleteWeekNet £", LastWeekGross,
                "CompleteWeekBeforeNet", weekBeforeGross);

            await pushService.Push(LastWeekNetVSweekBeforeNet);


            //////////ITEM ONE//////// --Yesterday vs same day last week            
            //yesterday to today
            //setup
            RevelOrderandOrderItemWrapper yesterdaysOrders = new RevelOrderandOrderItemWrapper(
                DateTime.Now.AddDays(-1), DateTime.Now, RevelOrderandOrderItemWrapper.WrapperType.Full);
            RevelOrderandOrderItemWrapper yesterdaysLastWeekOrders =
                new RevelOrderandOrderItemWrapper(DateTime.Now.AddDays(-8), DateTime.Now.AddDays(-7),
                    RevelOrderandOrderItemWrapper.WrapperType.Full);
            await revelFactory.PopulateOrderAndItemWrapper(yesterdaysOrders);
            await revelFactory.PopulateOrderAndItemWrapper(yesterdaysLastWeekOrders);

            //process
            var yesterSalesGross = (int)yesterdaysOrders.Orders.Sum(x => x.final_total);

            var yesterLastweekSalesGross = (int)yesterdaysLastWeekOrders.Orders.Sum(x => x.final_total);


            //push
            NumberSecondaryStat yesterdayVSYesterdayLastWeek = factory.CreateNumberSecondaryStat(2,
                "yesterdayVSYesterdayLastWeek",
                "https://push.geckoboard.com/v1/send/54410-7f1085a0-c590-0131-a65c-22000a1e86ad", "Yesterday £",
                yesterSalesGross,
                "Yesterday Last Week", yesterLastweekSalesGross);

            await pushService.Push(yesterdayVSYesterdayLastWeek);


            //ITEM TWO LAST MONTH

            int LastMonth = DateTime.Now.AddMonths(-1).Month;
            DateTime FirstDayOfLastMonth = new DateTime(2014, LastMonth, 01);
            DateTime FirstDayOfThisMonth = new DateTime(2014, DateTime.Now.Month, 01);

            RevelOrderandOrderItemWrapper lastMonth = new RevelOrderandOrderItemWrapper(FirstDayOfLastMonth,
                FirstDayOfThisMonth, RevelOrderandOrderItemWrapper.WrapperType.Order);

            await revelFactory.PopulateOrderAndItemWrapper(lastMonth);


            NumberSecondaryStat lastMonthVSBudget = factory.CreateNumberSecondaryStat(3, "LastWeekNetVSweekBeforeNet",
                "https://push.geckoboard.com/v1/send/54410-7f13d5a0-c590-0131-a65e-22000a1e86ad", "Last Month £",
                (int)lastMonth.GetOrderTotalPoundsNet(),
                "Budget", 0);

            await pushService.Push(lastMonthVSBudget);


            //ITEM THREE - 365 day period

            //dates
            DateTime ThisYearStart = new DateTime(DateTime.Now.Year, 01, 01);
            DateTime YTDYesterday = DateTime.Now.AddDays(-1).AddYears(-1);

            DateTime LastYearStart = ThisYearStart.AddYears(-1);
            DateTime LastYearYesterday = YTDYesterday.AddYears(-1);

            //wrappers
            RevelOrderandOrderItemWrapper ThisYearStartToTodayWrapper = new RevelOrderandOrderItemWrapper(
                ThisYearStart, YTDYesterday, RevelOrderandOrderItemWrapper.WrapperType.Order);
            RevelOrderandOrderItemWrapper LastYearStartToTodayWrapper = new RevelOrderandOrderItemWrapper(
                LastYearStart, LastYearYesterday, RevelOrderandOrderItemWrapper.WrapperType.Order);

            await revelFactory.PopulateOrderAndItemWrapper(ThisYearStartToTodayWrapper);
            await revelFactory.PopulateOrderAndItemWrapper(LastYearStartToTodayWrapper);

            NumberSecondaryStat ThisYearStartToTodayWidget = factory.CreateNumberSecondaryStat(4, "ThisYearStartToToday",
                "https://push.geckoboard.com/v1/send/54410-7f1aec20-c590-0131-a662-22000a1e86ad",
                "YTDYesterday", (int)ThisYearStartToTodayWrapper.GetOrderTotalPoundsGross(), "YTD Yesterday Last Year",
                (int)LastYearStartToTodayWrapper.GetOrderTotalPoundsGross());

            await pushService.Push(ThisYearStartToTodayWidget);
            //ITEM 4 


            return true;
        }


        public async Task<bool> RunCombinedDailyWidgets()
        {

            TestController cont = new TestController();
            var ok = await cont.UpdateDatabaseOrdersAndItems();

            ok = await cont.GECKOBOARD_PushAllDailyWidgets();


            return true;
        }


        public async Task<bool> RunCombinedOvernightWidgets()
        {

            GeckoboardOrganisation shoreditchGrindOrganisation =
            new GeckoboardOrganisation("ab876212d31d37960e3154eb5e2bc0a0", "ShoreditchGrind");
            IGeckoboardObjectCreatorFactory factory = new GeckoboardObjectCreatorFactory(shoreditchGrindOrganisation);

            List<WidgetSetA> allSitesPreliminaryWidgetSets = new List<WidgetSetA>
            {
                new WidgetSetA
                {
                    RevelEstablishment =
                        new Establishment(1, "Shoreditch",
                            RevelAPIKEY,
                            new Uri(RevelBaseURL))
                },
                new WidgetSetA
                {
                    RevelEstablishment =
                        new Establishment(3, "Soho",
                            RevelAPIKEY,
                            new Uri(RevelBaseURL))
                },
                new WidgetSetA
                {
                    RevelEstablishment =
                        new Establishment(4, "London",
                            RevelAPIKEY,
                            new Uri(RevelBaseURL))
                },
                    new WidgetSetA
                {
                    RevelEstablishment =
                        new Establishment(5, "Holborn",
                            RevelAPIKEY,
                            new Uri(RevelBaseURL))
                },
                        new WidgetSetA
                {
                    RevelEstablishment =
                        new Establishment(7, "Covent",
                            RevelAPIKEY,
                            new Uri(RevelBaseURL))
                },

               new WidgetSetA {
                    RevelEstablishment =
                        new Establishment(10, "Exmouth",
                            RevelAPIKEY,
                            new Uri(RevelBaseURL))
                },

                       new WidgetSetA {
                    RevelEstablishment =
                        new Establishment(8, "Radio",
                            RevelAPIKEY,
                            new Uri(RevelBaseURL))
                },
                             new WidgetSetA {
                    RevelEstablishment =
                        new Establishment(6, "Royal Exchange",
                            RevelAPIKEY,
                            new Uri(RevelBaseURL))
                },
                             new WidgetSetA {
                    RevelEstablishment =
                        new Establishment(9, "Whitechapel",
                            RevelAPIKEY,
                            new Uri(RevelBaseURL))
                }
                             ,
                             new WidgetSetA {
                    RevelEstablishment =
                        new Establishment(11, "Facebook",
                            RevelAPIKEY,
                            new Uri(RevelBaseURL))
                }  ,
                             new WidgetSetA {
                    RevelEstablishment =
                        new Establishment(13, "Greenwich",
                            RevelAPIKEY,
                            new Uri(RevelBaseURL))
                } ,
                             new WidgetSetA {
                    RevelEstablishment =
                        new Establishment(14, "Liverpool Street",
                            RevelAPIKEY,
                            new Uri(RevelBaseURL))
                }


            };

            WidgetSetFactory widgetSetFactory = new WidgetSetFactory();


            List<WidgetSetA> initialisedWidgetSets = new List<WidgetSetA>();


            var mostRecentMonday = DateTimeExtensions.StartOfWeek(DateTime.Now, DayOfWeek.Monday);
            var _24weeksAgo = mostRecentMonday.AddDays(-168);

            var last24weeksOrderItems = new List<OrderItem>();
            var maxDateTimeOrder = mostRecentMonday;
            var currentMinimum = _24weeksAgo;



            using (var mailer = new EmailController())
            {

                mailer.SendMessageNadavIgnoreSendExeceptions("overnight push - starting to cycle through items", null,
                    "railgunit.maintenance@gmail.com");
            }

            /*
                        //gather items
                        while (currentMinimum < mostRecentMonday)
                        {

                            var itemsReturned = _db.OrderItems.AsNoTracking().OrderBy(x => x.created_date)
                                .Where(x => x.created_date > currentMinimum && x.created_date <= mostRecentMonday).ToList();

                            if (itemsReturned.Count().Equals(0))
                            {
                                break;
                            }

                            last24weeksOrderItems.AddRange(itemsReturned);
                            currentMinimum = (DateTime)itemsReturned.Max(x => x.created_date);

                            using (var mailer = new EmailController())
                            {
                                mailer.SendMessageNadav(
                                    "overnight push - starting to cycle through items, on date " + currentMinimum, null,
                                    "railgunit.maintenance@gmail.com");
                            }

                        }*/
            /*
                        using (var mailer = new EmailController())
                        {
                            mailer.SendMessageNadav(
                                   "overnight push - managed to get all the items " + currentMinimum, null,
                                   "railgunit.maintenance@gmail.com");
                        }*/


            //do the 24 week graph then dispose item
            /*      foreach (var widgetSetA in allSitesPreliminaryWidgetSets)
              {

                  using (var mailer = new EmailController())
                  {

                      mailer.SendMessageNadav("overnight push now doing overnight widgets store:" + widgetSetA.RevelEstablishment.establishment_id, null,
                          "railgunit.maintenance@gmail.com");
                  }


                  IRevelReaderAsync readerAsync = new RevelDBReader(widgetSetA.RevelEstablishment);

                  IRevelFactoryAsync revelFactory = new RevelFactoryAsyncLocalDb(readerAsync,
                      widgetSetA.RevelEstablishment);
                  widgetSetA.revelFactory = revelFactory; //set the factory as the DB implementation

                  widgetSetA.factory = factory; //gecko factory     

                  await widgetSetFactory.Push24WeekOrderItemWidget(_db, widgetSetA, last24weeksOrderItems);
              }*/

            //then do parent

            /*
                        List<Establishment> establishmentForIdentificationService = new List<Establishment>();

                        foreach (var widgetsetA in allSitesPreliminaryWidgetSets)
                        {
                            establishmentForIdentificationService.Add(widgetsetA.RevelEstablishment);
                        }

                        var indentificationService = new RevelProductAndCategoryWrapper(establishmentForIdentificationService);
                        indentificationService.Initialise(_db);

                        var widget = new LineV2Widget("ab876212d31d37960e3154eb5e2bc0a0",
                            "https://push.geckoboard.com/v1/send/95518-b6774dfe-d7f3-470d-bd16-df22ebcf0fe8", "test",
                            GeckoboardChartAndItemType.LineV2, 11);

                        var _24WeekWidget = LineChartV2RevelFactory.Initialise24WeekLineV2WidgetData(widget, indentificationService,
                            establishmentForIdentificationService, last24weeksOrderItems);


                        var push = new GeckoboardPushService();
                        var ok = await push.Push(_24WeekWidget);

                        last24weeksOrderItems.Clear();
                        last24weeksOrderItems = null;
            */

            //init widgets
            var allProductClasses = _db.ProductClasses.ToList();
            foreach (var widgetSetA in allSitesPreliminaryWidgetSets)
            {
                Console.WriteLine(widgetSetA.RevelEstablishment.RevelOrganiationName + " started");

                IRevelReaderAsync readerAsync = new RevelDBReader(widgetSetA.RevelEstablishment);

                IRevelFactoryAsync revelFactory = new RevelFactoryAsyncLocalDb(readerAsync,
                    widgetSetA.RevelEstablishment);
                widgetSetA.revelFactory = revelFactory; //set the factory as the DB implementation

                widgetSetA.factory = factory; //gecko factory                

                WidgetSetA widgetSetReturned = await widgetSetFactory.InitialiseWidgetSetAOvernightWidgets(widgetSetA, allProductClasses);

                using (var mailer = new EmailController())
                {

                    mailer.SendMessageNadavIgnoreSendExeceptions("overnight push now doing overnight widgets store:" + widgetSetA.RevelEstablishment.establishment_id, null,
                        "railgunit.maintenance@gmail.com");
                }

                //quicker push time
                await widgetSetFactory.PushWidgetsToGeckoboard(widgetSetReturned);
                Console.WriteLine(widgetSetA.RevelEstablishment.RevelOrganiationName + " finshed");
                initialisedWidgetSets.Add(widgetSetReturned);

            }


            //PARENT WIDGET SET
            ParentWidgetSet ParentWidgetSet = new ParentWidgetSet
            {
                RevelEstablishment =
                    new Establishment(2, "Parent",
                        RevelAPIKEY,
                        new Uri(RevelBaseURL)),

                AllChildWidgetSets = initialisedWidgetSets
            };

            RevelFactory ParentRevelFactory = new RevelFactory(ParentWidgetSet.RevelEstablishment);
            ParentWidgetSet.revelFactory = ParentRevelFactory;
            ParentWidgetSet.factory = factory;
            ParentWidgetSet ParentWidgetSetReturned =
                await widgetSetFactory.InitialiseOvernightParentWidgetSet(ParentWidgetSet, last24weeksOrderItems);
            //push em out
            await widgetSetFactory.PushWidgetsToGeckoboard(ParentWidgetSetReturned);

            Console.WriteLine("Parent finshed");

            return true;
        }




        public void Test_DeleteDuplicateOrdersFromDB()
        {
            var helper = new RevelHelper.DbHelper();

            var ok = helper.DeleteDuplicateOrdersFromDB();

        }

        public void Test_DeleteDuplicateOrderItemsFromDB()
        {
            var helper = new RevelHelper.DbHelper();

            var ok = helper.DeleteDuplicateOrderItemsFromDB();

        }

        public async Task<int> TEST_ParseOrderItems()
        {

            var Yok = RevelHelper.ReturnYesterdayIfDateTimeNowBetween12am_3am(new DateTime(2014, 01, 02));


            /*Establishment revOrg = new Establishment(1, "Grind",
             "be9685e8ca1847959350571318aa6f0f:da848e35fabd4f41a1bcb59268c3ad1ecef62b6c6f3e4e82a5faf443d0f8242e",
             new Uri("https://testshoreditchgrind.revelup.com/"));*/
            Establishment revOrg = new Establishment(1, "Grind",
                RevelAPIKEY,
                new Uri(RevelBaseURL));


            var tc = new TestController();
            var startDate = new DateTime(2014, 08, 23, 02, 00, 00);
            var endDate = startDate.AddDays(1);

            RevelWebserviceDataReader webReader = new RevelWebserviceDataReader(revOrg);
            RevelDBWriter writer = new RevelDBWriter(_db);
            RevelDBReader DBReader = new RevelDBReader(revOrg);


            bool ok = false;


            //get all orders from our DB for that day

            List<OrderItem> orderItemsFromWeb = await DBReader.GetOrderItems(startDate, endDate);


            var theCokes = orderItemsFromWeb.Where(e => e.product_id == 176).ToList(); //coke


            var cokePRice = theCokes.Select(e => e.pure_sales).ToList();
            var cokepricesSUm = theCokes.Sum(f => f.pure_sales);
            var roundedIndividualPrices = theCokes.Sum(f => Math.Round(f.pure_sales, 2));
            var roundedTotalPrice = Math.Round(cokepricesSUm, 2);
            /*
                        foreach (var @decimal in cokePRice)
                        {   
                            var value = Math.Truncate(100 * @decimal) / 100;
                            roundedPrices.Add(value);           
                        }s*/

            return 0;
        }



        public async Task<int> TestCreateCustomerAndLoyaltyCardNewFromRevel()
        {


            /*Establishment revOrg = new Establishment(1, "Grind",
           "be9685e8ca1847959350571318aa6f0f:da848e35fabd4f41a1bcb59268c3ad1ecef62b6c6f3e4e82a5faf443d0f8242e",
           new Uri("https://testshoreditchgrind.revelup.com/"));*/
            Establishment revOrg = new Establishment(1, "Grind",
                RevelAPIKEY,
                new Uri(RevelBaseURL));


            var tc = new TestController();



            RevelWebserviceDataReader webReader = new RevelWebserviceDataReader(revOrg);
            RevelDBWriter writer = new RevelDBWriter(_db);
            RevelDBReader DBReader = new RevelDBReader(revOrg);


            bool ok = false;

            var startDate = new DateTime(2013, 01, 01, 02, 00, 00);
            var endDate = startDate.AddYears(2);
            /*var cards = await webReader.GetRevelWebserviceData(new RewardsCardNew(), startDate, endDate);
            writer.SaveRewardsCardNew(cards);*/


            var customer = new Customer();
            var URL = String.Format(customer.theAddress, startDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                    endDate.ToString("yyyy-MM-ddTHH:mm:ss"));

            var customers = await webReader.GetRevelWebserviceData(new Customer(), URL);
            writer.SaveCustomers(customers);





            return 0;


        }

        public async Task<int> TestCustomerUpdate()
        {
            var service = new CustomerService(_db);
            var ok = await service.GetAllCustomersAndInsertNew(DateTime.Now.AddYears(-4), DateTime.Now);
            return 0;
        }


        public async Task<int> UpdateExistingCustomers()
        {

            return 0;
        }


        public async Task<int> TestSaveNewRewardCardsAndUpdateExisting()
        {

            using (var _db = new GrindContext())
            {

                var lastCardID = _db.RewardsCardNew.Max(x => x.Revelid);
                var service = new RewardCardServices(_db);
                var ok = await service.SyncAllRewardCardsAndInsertNew(new DateTime(2010, 01, 01), DateTime.Now, lastCardID);
            }

            return 0;
        }

        public async Task<int> UpdateExistingRewardCards()
        {

            return 0;
        }



        public ActionResult TestPOSTCustomer()
        {

            return View();
        }


        public void Test24WeekCalendar()
        {

            var mostRecentMonday = DateTimeExtensions.StartOfWeek(DateTime.Now, DayOfWeek.Monday);

            var _24weeksAgo = mostRecentMonday.AddDays(-168);


            var startOfEachWeekForPast24 = new List<DatePeriod>();
            var currentLoopDate = _24weeksAgo;
            var coffee = new List<LineGraphOrderItemBreakdown>();
            var food = new List<LineGraphOrderItemBreakdown>();
            var beverage = new List<LineGraphOrderItemBreakdown>();

            do
            {
                var endLoopDate = currentLoopDate.AddDays(7);
                startOfEachWeekForPast24.Add(new DatePeriod
                {
                    PeriodStart =
                        new DateTime(currentLoopDate.Year, currentLoopDate.Month, currentLoopDate.Day, 02, 00, 00),
                    PeriodEnd = new DateTime(endLoopDate.Year, endLoopDate.Month, endLoopDate.Day, 23, 59, 59)

                });

                currentLoopDate = currentLoopDate.AddDays(7);
            } while (currentLoopDate < mostRecentMonday);

            var test = "";

        }


        public async Task<int> TestLoyaltyCardRefresh()
        {
            var service = new RewardCardServices(_db);
            var listOfCardsToRefresh = _db.RewardsCardNew.Where(c => c.DBKEY_rewardscardnew_id == 38916).ToList();

            var letsRefresh = await service.ResetInvestorCards(listOfCardsToRefresh, _db);

            return 0;


        }

        public async Task<bool> RunYesterdayWidgets()
        {
            GeckoboardOrganisation geckoOrg =
           new GeckoboardOrganisation("ab876212d31d37960e3154eb5e2bc0a0", "ShoreditchGrind");
            IGeckoboardObjectCreatorFactory factory = new GeckoboardObjectCreatorFactory(geckoOrg);

            List<GeckoboardObject> widgets = new List<GeckoboardObject>();

            var productClasses = _db.ProductClasses.ToList();

            var geckoFactory = new GeckoboardObjectCreatorFactory(geckoOrg);
            var push = new GeckoboardPushService();

            var todayPreFix = DateTime.Now;
            var today = new DateTime(todayPreFix.Year, todayPreFix.Month, todayPreFix.Day, 02, 00, 00);
            var yesterday = today.AddDays(-1);
            var yesterdayLastWeek = yesterday.AddDays(-7);
            var yesterdayLastWeekPlusOneday = yesterdayLastWeek.AddDays(1);

            var yesterdayItems = _db.OrderItems.Where(x => x.created_date >= yesterday && x.created_date <= today)
                .ToList();

            var yesterdayLastWeekItems = _db.OrderItems.Where(x => x.created_date >= yesterdayLastWeek && x.created_date <= yesterdayLastWeekPlusOneday).ToList();

            var booze = new List<OrderItem>();
            var food = new List<OrderItem>();
            var hotDrinks = new List<OrderItem>();
            var softDrinks = new List<OrderItem>();

            var boozeLastWeek = new List<OrderItem>();
            var foodLastWeek = new List<OrderItem>();
            var hotDrinksLastWeek = new List<OrderItem>();
            var softDrinksLastWeek = new List<OrderItem>();

            IList<Product> errors = new List<Product>();
            var pcWrapper = new RevelProductAndCategoryWrapper(_db.Establishments.ToList());
            pcWrapper.Initialise(_db, productClasses);
            List<Product> foodProducts = pcWrapper.GetProductsThatAreFoodByClass(productClasses, out errors);
            List<Product> hotDrinksProducts = pcWrapper.GetProductsThatAreHotDrinksByClass(productClasses, out errors);
            List<Product> alcoholProducts = pcWrapper.GetProductsThatAreAlcoholByClass(productClasses, out errors);
            List<Product> softDrinkProducts = pcWrapper.GetProductsThatAreSoftDrinksByClass(productClasses, out errors);

            var paymentsYesterday = _db.Payments.Where(x => x.created_date >= yesterday && x.created_date <= today)
                .ToList();
            var paymentYesterdayLastWeek = _db.Payments.Where(x => x.created_date >= yesterdayLastWeek && x.created_date <= yesterdayLastWeekPlusOneday)
                .ToList();



            foreach (var item in yesterdayItems)
            {
                if (pcWrapper.isItemAlcohol(item, alcoholProducts, out errors))
                {
                    booze.Add(item);
                }
                if (pcWrapper.isItemFood(item, foodProducts, out errors))
                {
                    food.Add(item);
                }
                if (pcWrapper.isItemSoftDrink(item, softDrinkProducts, out errors))
                {
                    softDrinks.Add(item);
                }
                if (pcWrapper.isItemHotDrink(item, hotDrinksProducts, out errors))
                {
                    hotDrinks.Add(item);
                }
            }

            foreach (var item in yesterdayLastWeekItems)
            {
                if (pcWrapper.isItemAlcohol(item, alcoholProducts, out errors))
                {
                    boozeLastWeek.Add(item);
                }
                if (pcWrapper.isItemFood(item, foodProducts, out errors))
                {
                    foodLastWeek.Add(item);
                }
                if (pcWrapper.isItemSoftDrink(item, softDrinkProducts, out errors))
                {
                    softDrinksLastWeek.Add(item);
                }
                if (pcWrapper.isItemHotDrink(item, hotDrinksProducts, out errors))
                {
                    hotDrinksLastWeek.Add(item);
                }
            }
            //yesterday total
            NumberSecondaryStat allStores = geckoFactory.CreateNumberSecondaryStat(103, "allStores",
          "https://push.geckoboard.com/v1/send/54410-feff39a0-6528-0133-c7e4-22000b4908e7", "Total Stores £", (int)(paymentsYesterday.Sum(x => x.amount)),
          "Budget", (int)paymentYesterdayLastWeek.Sum(x => x.amount));
            widgets.Add(allStores);
            //Shore
            NumberSecondaryStat shoreTotal = geckoFactory.CreateNumberSecondaryStat(103, "shoreTotal",
          "https://push.geckoboard.com/v1/send/54410-ff0adf10-6528-0133-c7ec-22000b4908e7", "Total Stores £", (int)(paymentsYesterday.Where(x => x.establishment_id.Equals(1)).Sum(x => x.amount)),
          "Budget", (int)paymentYesterdayLastWeek.Where(x => x.establishment_id.Equals(1)).Sum(x => x.amount));
            widgets.Add(shoreTotal);

            //Soho
            NumberSecondaryStat sohoTotal = geckoFactory.CreateNumberSecondaryStat(103, "sohoTotal",
          "https://push.geckoboard.com/v1/send/54410-ff0c0a70-6528-0133-c7ed-22000b4908e7", "Total Stores £", (int)(paymentsYesterday.Where(x => x.establishment_id.Equals(3)).Sum(x => x.amount)),
          "Budget", (int)paymentYesterdayLastWeek.Where(x => x.establishment_id.Equals(3)).Sum(x => x.amount));
            widgets.Add(sohoTotal);

            //Lon
            NumberSecondaryStat londonTotal = geckoFactory.CreateNumberSecondaryStat(103, "londonTotal",
          "https://push.geckoboard.com/v1/send/54410-ff138360-6528-0133-c7f4-22000b4908e7", "Total Stores £", (int)(paymentsYesterday.Where(x => x.establishment_id.Equals(4)).Sum(x => x.amount)),
          "Budget", (int)paymentYesterdayLastWeek.Where(x => x.establishment_id.Equals(4)).Sum(x => x.amount));
            widgets.Add(londonTotal);

            //Holborn
            NumberSecondaryStat holbornTotal = geckoFactory.CreateNumberSecondaryStat(103, "holbornTotal",
          "https://push.geckoboard.com/v1/send/54410-ff0dd3f0-6528-0133-c7ee-22000b4908e7", "Total Stores £", (int)(paymentsYesterday.Where(x => x.establishment_id.Equals(5)).Sum(x => x.amount)),
          "Budget", (int)paymentYesterdayLastWeek.Where(x => x.establishment_id.Equals(5)).Sum(x => x.amount));
            widgets.Add(holbornTotal);

            //Strat
            NumberSecondaryStat stratTotal = geckoFactory.CreateNumberSecondaryStat(103, "stratTotal",
          "https://push.geckoboard.com/v1/send/54410-ff17c260-6528-0133-c7f8-22000b4908e7", "Total Stores £", (int)(paymentsYesterday.Where(x => x.establishment_id.Equals(7)).Sum(x => x.amount)),
          "Budget", (int)paymentYesterdayLastWeek.Where(x => x.establishment_id.Equals(7)).Sum(x => x.amount));
            widgets.Add(stratTotal);


            //Radio
            NumberSecondaryStat radioTotal = geckoFactory.CreateNumberSecondaryStat(103, "radioTotal",
          "https://push.geckoboard.com/v1/send/54410-ff199130-6528-0133-c7f9-22000b4908e7", "Total Stores £", (int)(paymentsYesterday.Where(x => x.establishment_id.Equals(8)).Sum(x => x.amount)),
          "Budget", (int)paymentYesterdayLastWeek.Where(x => x.establishment_id.Equals(8)).Sum(x => x.amount));
            widgets.Add(radioTotal);

            //Royal Ex
            NumberSecondaryStat royalexchangeTOtal = geckoFactory.CreateNumberSecondaryStat(103, "royalExchangeTotal",
          "https://push.geckoboard.com/v1/send/166053-f117281a-e98c-4942-be78-0fe374826bd9", "Total Stores £", (int)(paymentsYesterday.Where(x => x.establishment_id.Equals(6)).Sum(x => x.amount)),
          "Budget", (int)paymentYesterdayLastWeek.Where(x => x.establishment_id.Equals(6)).Sum(x => x.amount));
            widgets.Add(royalexchangeTOtal);


            //Face
            NumberSecondaryStat exmouthTotal = geckoFactory.CreateNumberSecondaryStat(103, "exmouthTotal",
          "https://push.geckoboard.com/v1/send/51912-6b361c00-c894-0134-9659-22000b248df5", "Total Stores £", (int)(paymentsYesterday.Where(x => x.establishment_id.Equals(10)).Sum(x => x.amount)),
          "Budget", (int)paymentYesterdayLastWeek.Where(x => x.establishment_id.Equals(10)).Sum(x => x.amount));
            widgets.Add(exmouthTotal);

            //Greenwich
            NumberSecondaryStat greenwichTotal = geckoFactory.CreateNumberSecondaryStat(103, "greenwichTotal",
          "https://push.geckoboard.com/v1/send/51912-0bda19f0-c994-0136-9a79-0e6652efdba6", "Total Stores £", (int)(paymentsYesterday.Where(x => x.establishment_id.Equals(13)).Sum(x => x.amount)),
          "Budget", (int)paymentYesterdayLastWeek.Where(x => x.establishment_id.Equals(13)).Sum(x => x.amount));
            widgets.Add(greenwichTotal);

            //LIVERPOOL ST
            NumberSecondaryStat liverpoolStTotal = geckoFactory.CreateNumberSecondaryStat(103, "liverpoolStTotal",
          "https://push.geckoboard.com/v1/send/51912-42ee39e0-3aa8-0137-06ae-0201c52e3c5c", "Total Stores £", (int)(paymentsYesterday.Where(x => x.establishment_id.Equals(14)).Sum(x => x.amount)),
          "Budget", (int)paymentYesterdayLastWeek.Where(x => x.establishment_id.Equals(14)).Sum(x => x.amount));
            widgets.Add(liverpoolStTotal);

            //Hot Drinks
            NumberSecondaryStat hotDrinksWidget = geckoFactory.CreateNumberSecondaryStat(103, "hotDrinksWidget",
          "https://push.geckoboard.com/v1/send/54410-ff03e300-6528-0133-c7e6-22000b4908e7", "Total Stores £", (int)(hotDrinks.Sum(x => x.quantity)),
          "Budget", (int)(hotDrinksLastWeek.Sum(x => x.quantity)));
            widgets.Add(hotDrinksWidget);

            NumberSecondaryStat softDrinksWidget = geckoFactory.CreateNumberSecondaryStat(103, "hotDrinksWidget",
      "https://push.geckoboard.com/v1/send/54410-ff049b20-6528-0133-c7e7-22000b4908e7", "Total Stores £", (int)(softDrinks.Sum(x => x.price)),
      "Budget", (int)(softDrinksLastWeek.Sum(x => x.price)));
            widgets.Add(softDrinksWidget);

            NumberSecondaryStat foodWidget = geckoFactory.CreateNumberSecondaryStat(103, "hotDrinksWidget",
      "https://push.geckoboard.com/v1/send/54410-ff06ec30-6528-0133-c7e9-22000b4908e7", "Total Stores £", (int)(food.Sum(x => x.price)),
      "Budget", (int)(foodLastWeek.Sum(x => x.price)));
            widgets.Add(foodWidget);

            NumberSecondaryStat alcoholWidget = geckoFactory.CreateNumberSecondaryStat(103, "hotDrinksWidget",
      "https://push.geckoboard.com/v1/send/54410-ff05ee50-6528-0133-c7e8-22000b4908e7", "Total Stores £", (int)(booze.Sum(x => x.price)),
      "Budget", (int)(boozeLastWeek.Sum(x => x.price)));
            widgets.Add(alcoholWidget);





            foreach (var widget in widgets)
            {
                var ok = await push.Push(widget);
            }

            return true;

        }

        public async Task GetSomeRewardLogs()
        {

            var service = new RewardLogService();

            var cardLogs =
                await
                    service.GetRewardLogPointsFromWebservice(_db.Brands.Where(x => x.name.Trim().ToLower().Equals(("Shoreditch Grind").Trim().ToLower())).FirstOrDefault(), new DateTime(2015, 11, 01),
                        new DateTime(2015, 12, 01));

            _db.RewardCardLogs.AddRange(cardLogs);
            _db.SaveChanges();
        }




    }
}