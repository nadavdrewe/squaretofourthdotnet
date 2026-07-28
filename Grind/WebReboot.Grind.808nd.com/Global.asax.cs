
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Quartz;
using Quartz.Impl;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Models;
using Web.Grind._808nd.com.Controllers;
using WebReboot.Grind._808nd.com.CacheHelper;
using System.Threading.Tasks;
using System.Net;
using System.Net.Security;

namespace WebReboot.Grind._808nd.com
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private async static void RefreshTheCache()
        {
            var db = new GrindContext();
            using (var mailer = new EmailController())
            {
                Task.Run(() =>
                {
                    //mailer.SendMessageNadavIgnoreSendExeceptions("In Global.asax - STARTED refreshing the Web.Grind cards cache at" + DateTime.Now);
                    var watch = new Stopwatch();
                    watch.Start();

                    var cardCollection = RewardsCardNew.GetRewardCardsNewAndCustomerAsNoTracking(db);
                    CacheHelpers.RefreshCacheCollection(cardCollection, "allCards");

                    watch.Stop();

                    //mailer.SendMessageNadavIgnoreSendExeceptions("In Global.asax - CARDS REFRESHED and it took" + watch.Elapsed);

                });

                Task.Run(() =>
                {
                    try
                    {
                        using (var db2 = new GrindContext())
                        {
                            //mailer.SendMessageNadavIgnoreSendExeceptions("In Global.asax - STARTED refreshing the Web.Grind gift cards at" + DateTime.Now);

                            var giftCards = db2.GiftCards.AsNoTracking().ToList();
                            CacheHelpers.RefreshCacheCollection(giftCards, "giftCards");

                            //  mailer.SendMessageNadavIgnoreSendExeceptions("In Global.asax - FINISHED refreshing the Web.Grind gift cards");

                        }

                    }
                    catch (Exception ex)
                    {
                        mailer.SendMessageNadavIgnoreSendExeceptions("In Global.asax - EXCEPTION: gift cards " + ex.Message);

                    }


                });


            }
        }

        [DisallowConcurrentExecution]
        public class RefreshCacheJob : IJob
        {
            async void IJob.Execute(IJobExecutionContext context)
            {
                RefreshTheCache();
            }
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //set up TLS
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback = new
            RemoteCertificateValidationCallback
            (
               delegate { return true; }
            );


#if (DEBUG)

            //get some test cards
            using (var db = new GrindContext())
            {
                var mostRecentCards = db.RewardsCardNew.Where(x => x.Active == true || x.LoyaltyCardType != null).Include(x => x.LoyaltyCardType).OrderByDescending(x => x.created_date).Take(15).ToList();
                CacheHelpers.RefreshCacheCollection(mostRecentCards, "allCards");

                var mostRecentGiftCards = db.GiftCards.OrderByDescending(x => x.created_date).Take(15).ToList();
                CacheHelpers.RefreshCacheCollection(mostRecentGiftCards, "giftCards");
            }


#else

            try
            {
                using (var db = new GrindContext())
                {
                    db.SystemLogs.Add(new SystemLog { Type = "APP_POOL", WhoTriggered = "Automated", Note = "App pool recycled, app started", WhenCreated = DateTime.Now });
                    db.SaveChanges();
                }

                /*Cache all cards*/

                //Refresh card cache
                IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
                scheduler.Start();

                IJobDetail refreshCacheJob = JobBuilder.Create<RefreshCacheJob>().Build();

                ITrigger refreshCacheTrigger = TriggerBuilder.Create()
                            .StartNow()
                            .WithSimpleSchedule(x => x.WithIntervalInSeconds(350).RepeatForever())
                            .Build();

                scheduler.ScheduleJob(refreshCacheJob, refreshCacheTrigger);
            }
            catch (Exception exception)
            {
                using (var mailer = new EmailController())
                {

                    mailer.SendMessageNadavIgnoreSendExeceptions("In Global.asax - EXCEPTION refreshing the Web.Grind cache at" +
                                            DateTime.Now + " ::: " + exception.Message);
                }
                throw;
            }

#endif



        }



        protected void Session_Start()
        {

            Session["uploads"] = new List<MenuFile>();

        }


    }
}
