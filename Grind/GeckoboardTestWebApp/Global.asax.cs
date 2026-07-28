using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using GeckoboardTestWebApp.Controllers;
using System.Threading.Tasks;
using Revel._808nd.com.Classes;
using System.Net;
using System.Net.Security;

namespace GeckoboardTestWebApp
{
    public class Global : HttpApplication
    {
        //global variables
        bool doneOvernightlyUpdate = false;
        bool doneWeeklySundayUpdate = false;

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //set up TLS
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback = new
            RemoteCertificateValidationCallback
            (
               delegate { return true; }
            );

            /*
                        //900000 timer - every 10 minutes
                        Timer ordersUpdateAndGeckoPush = new Timer(320000);
                        //Timer ordersUpdateAndGeckoPush = new Timer(120000);
                        ordersUpdateAndGeckoPush.Elapsed += DaliesTimer_Elapsed;
                        ordersUpdateAndGeckoPush.Enabled = true;
                        GC.KeepAlive(ordersUpdateAndGeckoPush);


                        //7.2e+6 timer - every 64 minutes
                        Timer recoverMissingOrdersAndItemsForToday = new Timer(3.84e+6);
                        recoverMissingOrdersAndItemsForToday.Elapsed += GetMissingOrdersTimer_Elapsed;
                        recoverMissingOrdersAndItemsForToday.Enabled = true;
                        GC.KeepAlive(recoverMissingOrdersAndItemsForToday);


                        ////a second timer to run longer methods - dailies - only runs outside openin hours
                        Timer longerTimer = new Timer(3.84e+6);
                        longerTimer.Elapsed += LongerTimer_Elapsed;
                        longerTimer.Enabled = true;
                        GC.KeepAlive(longerTimer);


                        ////Sunday night timer
                        Timer NotInOpeningHoursTimer = new Timer(600000);
                        NotInOpeningHoursTimer.Elapsed += NotInOpeningHoursTimer_Elapsed;
                        NotInOpeningHoursTimer.Enabled = true;
                        GC.KeepAlive(NotInOpeningHoursTimer);*/


        }




        /*     private void DaliesTimer_Elapsed(object sender, ElapsedEventArgs e)
             {


                 if (RevelHelper.IsDateTimeCurrentlyWithinOpeningHours())
                 {

                     SyncingController theController = new SyncingController();

                     var ok = theController.ordersUpdateAndGeckoPush().Result;

                     doneOvernightlyUpdate = false; //set this to false for longer updates
                     doneWeeklySundayUpdate = false;
                 }


             }

             private void LongerTimer_Elapsed(object sender, ElapsedEventArgs e)
             {
                 //we run all this if it's not in opening hours, and if it hasn't been done already
                 if (!RevelHelper.IsDateTimeCurrentlyWithinOpeningHours() && DateTime.Now.DayOfWeek != DayOfWeek.Sunday)
                 {

                     if (doneOvernightlyUpdate == false)
                     {
                         // exec all 
                         SyncingController theController = new SyncingController();

                         theController.longerTimer();

                         doneOvernightlyUpdate = true; //so it won't run again
                     }
                 }
             }*/

        private void GetMissingOrdersTimer_Elapsed(object sender, ElapsedEventArgs e)
        {

            if (RevelHelper.IsDateTimeCurrentlyWithinOpeningHours())
            {
                SyncingController theController = new SyncingController();
            
                var ok = theController.recoverMissingOrdersAndItemsForToday().Result;
              
            }

        }

        private void NotInOpeningHoursTimer_Elapsed(object sender, ElapsedEventArgs e)
        {            
            //if it's sunday night we do all updates, once only, not in opening hours
            if(DateTime.Now.DayOfWeek == DayOfWeek.Sunday &&  !(RevelHelper.IsDateTimeCurrentlyWithinOpeningHours() ) && doneWeeklySundayUpdate == false)
            {

                SyncingController theController = new SyncingController();

                //might time out so just exec
                var ok = theController.NotInOpeningHoursTimer();

                doneWeeklySundayUpdate = true;
            }

        }

    }

}