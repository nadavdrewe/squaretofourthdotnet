using System.Diagnostics;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Quartz.Impl;
using web.fourth.revel.com.ScheduledTasks;

namespace topshelf.fourth.revel.com
{
    class FourthService : IService
    {
        public FourthService()
        {
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Application";
                eventLog.WriteEntry("Fourth Revel Job Firing", EventLogEntryType.Information, 0, 1);
            }

            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");

            ISchedulerFactory schedFact = new StdSchedulerFactory();


            //ORDER ITEM
            // get a scheduler
            IScheduler sched = schedFact.GetScheduler();
            sched.Start();

            // define the job and tie it to our HelloJob class
            IJobDetail orderItemJob = JobBuilder.Create<PushToFourth3amJob>()
               .WithIdentity("orderItemJob", "group1")
               .Build();

            // Trigger the job to run now, and then every 40 seconds
            ITrigger trigger = TriggerBuilder.Create()
              .WithIdentity("myTrigger", "group1")
              .StartNow()
                // .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(03, 35))
              .Build();

              sched.ScheduleJob(orderItemJob, trigger);


            //PRODUCT
            IJobDetail productJob = JobBuilder.Create<UpdateProductsFromRevel>()
               .WithIdentity("productJob", "group2")
               .Build();

            // Trigger the job to run now, and then every 40 seconds
            ITrigger productJobtrigger = TriggerBuilder.Create()
              .WithIdentity("myTrigger", "group2")
               // .StartNow()
               /*     .WithSimpleSchedule(x => x
                       .WithIntervalInSeconds(380)
                        .WithRepeatCount(3)) // note that 10 repeats will give a total of 11 firings
                    .ForJob(job) // identify job with handle to its JobDetail itself      */

                .WithSchedule(CronScheduleBuilder.CronSchedule("0 0 5 ? * MON"))
              .Build();


            sched.ScheduleJob(productJob, productJobtrigger);
        }



        public void Start()
        {
            Console.WriteLine("Fourth Service Started");
        }

        public void Stop()
        {
            Console.WriteLine("Fourth Service Stopped");
        }


        public void Pause()
        {
            Console.WriteLine("Service PAUSED");
        }

        public void Continue()
        {
            Console.WriteLine("Service CONTINUED");
        }

    }
}
