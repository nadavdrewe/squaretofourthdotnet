using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeckoboardTestWebApp.Controllers;
using gw = GeckoboardTestWebApp;
using Geckoboard._808nd.com;
using System.Web;
using System.Web.Mvc;
using Nito.AsyncEx;
using Revel._808nd.com.Classes;

namespace Console.Geckoboard._808nd.com
{
    class Program
    {


        static void Main(string[] args)
        {
            AsyncContext.Run(() => MainAsync(args));
        }

        //Night
        public async static Task<int> MainAsync(string[] args)
        {
            System.Console.WriteLine("Welcome to Geckoboard Command Line:");
            var x = 1;

            if (args[0].ToLower() == "day")
            {
                if (RevelHelper.IsDateTimeCurrentlyWithinOpeningHours())
                {
                    SyncingController theController = new SyncingController();

                    var ok = await theController.ordersUpdateAndGeckoPush();
                }


            }

            if (args[0].ToLower() == "daysync")
            {

                if (RevelHelper.IsDateTimeCurrentlyWithinOpeningHours())
                {
                    SyncingController theController = new SyncingController();
                    TestController tc = new TestController();

                    await tc.MAINTAINANCE_UpdatePaymentsToday();
                    await tc.MAINTAINANCE_Today_CombinedOrderAndOrderItemSync();
                    await tc.GECKOBOARD_PushAllDailyWidgets();
                }

            }

            else if (args[0].ToLower() == "night")
            {

                gw.Controllers.TestController tc = new TestController();

                await tc.RunCombinedOvernightWidgets();

            }

            else if (args[0].ToLower() == "yestsync")
            {

                TestController tc = new TestController();
                await tc.UpdateDatabaseProductsAndCategories();

                var ok = await tc.MAINTAINANCE_UpdatePaymentsYesterday();
                await tc.MAINTAINANCE_Yesterday_OrderSync();


            }

            else if (args[0].ToLower() == "monthsync")
            {

                TestController tc = new TestController();
                await tc.UpdateDatabaseProductsAndCategories();
                await tc.GetDiscountsAndSaveToDB();

                var ok = await tc.MAINTAINANCE_UpdatePaymentsLastMonth();
                await tc.MAINTAINANCE_LastMonth_OrderSync();
                await tc.MAINTAINANCE_LastMonth_OrderItemSync();

            }

            else if (args[0].ToLower() == "cats")
            {

                TestController tc = new TestController();
                await tc.UpdateDatabaseProductsAndCategories();
                await tc.GetDiscountsAndSaveToDB();


            }

            else if (args[0].ToLower() == "nightsync")
            {
                TestController tc = new TestController();
                await tc.UpdateDatabaseProductsAndCategories();
                await tc.GetDiscountsAndSaveToDB();

                var ok = await tc.MAINTAINANCE_UpdatePaymentsYesterday();
                await tc.MAINTAINANCE_Yesterday_OrderSync();

                await tc.RunCombinedOvernightWidgets();
            }

            else if (args[0].ToLower() == "customercard")
            {
                SyncingController sc = new SyncingController();
                var ok = await sc.FullCustomerAndCardSync();

            }
            else if (args[0].ToLower() == "customer")
            {
                SyncingController sc = new SyncingController();
                var ok = await sc.FullCustomerSync();

            }
            else if (args[0].ToLower() == "card")
            {
                SyncingController sc = new SyncingController();
                var ok = await sc.FullCardsSync();


            }
            else if (args[0].ToLower() == "resetcards")
            {
                SyncingController sc = new SyncingController();
                var ok = await sc.ResetAllRedCards();

            }
            else if (args[0].ToLower() == "cardsovernight")
            {
                SyncingController sc = new SyncingController();
                var ok = await sc.SaveDaysSinceLastVisit();              
                var ok2 = sc.FullGiftCardSync();
            }
            else if (args[0].ToLower() == "3am")
            {
                  SyncingController sc = new SyncingController();
                 var ok = await sc.Run3amRoutineWrapper();
            }



            return 0;
        }




      //public async static Task<int> MainAsync(string[] args)
      //          {
      //              System.Console.WriteLine("Welcome to GrindandCo Geckoboard Command Line:");
      //              var x = 1;
      //              do
      //              {                
      //                  System.Console.WriteLine("$:\\");
      //                  var read = System.Console.ReadLine();
      //                  if (read.ToLower() == "update")
      //                  {
      //                      gw.Controllers.TestController tc = new TestController();
      //                      var ok = await tc.UpdateDatabaseProductsAndCategories();
      //                      System.Console.WriteLine("Update");

      //                  }
      //                  if (read.ToLower() == "cat")
      //                  {
      //                      gw.Controllers.TestController tc = new TestController();
      //                      var ok = await tc.UpdateDatabaseProductsAndCategories();
      //                      System.Console.WriteLine("Prod cats and prods complete");

      //                  }

      //                  if (read.ToLower() == "orders")
      //                  {
      //                      gw.Controllers.TestController tc = new TestController();
      //                      var ok = await tc.MAINTAINANCE_Today_OrderSync();
      //                      System.Console.WriteLine("Today - Update Order Complete");
                    
      //                  }
      //                  if (read.ToLower() == "items")
      //                  {
      //                      gw.Controllers.TestController tc = new TestController();
      //                      var ok = await tc.MAINTAINANCE_Today_OrderItemSync();
      //                      System.Console.WriteLine("Today - Update Order Item Complete");

      //                  }
      //                  if (read.ToLower() == "pay")
      //                  {
      //                      gw.Controllers.TestController tc = new TestController();
      //                      var ok = await tc.MAINTAINANCE_UpdatePaymentsToday();
      //                      System.Console.WriteLine("Today - Update Payments Complete");

      //                  }
      //                  if (read.ToLower() == "push")
      //                  {
      //                      gw.Controllers.TestController tc = new TestController();
      //                      var ok = await tc.GECKOBOARD_PushAllDailyWidgets();
      //                      System.Console.WriteLine("Today - Daily Push Complete");
      //                  }
      //                  if (read.ToLower() == "exit")
      //                  {
      //                      x = 0;
      //                  }
      //                  if (read.ToLower() == "night")
      //                  {
      //                      gw.Controllers.TestController tc = new TestController();
      //                      var ok = await tc.RunCombinedOvernightWidgets();
      //                      System.Console.WriteLine("Overnight Push Complete");
      //                  }
      //                  if (read.ToLower() == "paylast")
      //                  {
      //                      gw.Controllers.TestController tc = new TestController();
      //                      var ok = await tc.GetPaymentsSinceLastPaymentInDbAndInsert();
      //                      System.Console.WriteLine("Payment Update Complete");
      //                  }
      //                  if (read.ToLower() == "yest")
      //                  {
      //                      gw.Controllers.TestController tc = new TestController();
      //                      var ok = await tc.MAINTAINANCE_UpdatePaymentsYesterday();
      //                      await tc.MAINTAINANCE_Yesterday_OrderSync();
      //                      await tc.MAINTAINANCE_Yesterday_OrderItemSync();
      //                      System.Console.WriteLine("Payment Update Complete");
      //                  }
      //                  if (read.ToLower() == "today")
      //                  {
      //                      gw.Controllers.TestController tc = new TestController();
      //                      var ok = await tc.MAINTAINANCE_UpdatePaymentsToday();
      //                      await tc.MAINTAINANCE_Today_CombinedOrderAndOrderItemSync();
      //                      await tc.GECKOBOARD_PushAllDailyWidgets();
      //                      System.Console.WriteLine("Payment Update Complete");
      //                  }
      //                  if (read.ToLower() == "card")
      //                  {
      //                      SyncingController sc = new SyncingController();
      //                      var ok = await sc.RecentCustomerAndCardSync();
      //                      System.Console.WriteLine("Card Update Complete");
      //                  }
      //                  if (read.ToLower() == "cardsovernight")
      //                  {
      //                      SyncingController sc = new SyncingController();
      //                      var ok = await sc.Run3amRoutineWrapper();
      //                      System.Console.WriteLine("Overnight Card Update Complete");
      //                  }
      //                  if (read.ToLower() == "resetred")
      //                  {
      //                      SyncingController sc = new SyncingController();
      //                      var ok = await sc.ResetAllRedCards();
      //                      System.Console.WriteLine("All Red Cards Reset!");
      //                  }




                //    } while (x != 0);

                //    System.Console.ReadLine();
                //    System.Console.WriteLine("Thank you, bye!");
                //    return 0;
                //}
    
        public void RunDailySync()
        {


        }
    }
}
