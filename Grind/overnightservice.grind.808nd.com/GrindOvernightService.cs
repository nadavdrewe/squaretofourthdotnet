using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using interfaces.service.grind._808nd.com;
using Quartz;
using Quartz.Impl;
using scheduledtasks.grind._808nd.com;
using scheduledtasks.grind._808nd.com.ScheduledTasks;

namespace overnightservice.grind._808nd.com
{
    class GrindOvernightService : IService
    {
        private ISchedulerFactory schedFact;
        private TimeZoneInfo timeZoneInfo;

        public GrindOvernightService()
        {

            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            schedFact = new StdSchedulerFactory();

            SetUpTLS();
        }

        void SetUpTLS()
        {
            //set up TLS
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback = new
            RemoteCertificateValidationCallback
             (
               delegate { return true; }
            );
        }

        public void Start()
        {
            var ok = RunScheduledTasks();
        }

        public void Stop()
        {

        }

        public void Continue()
        {

        }

        public void Pause()
        {

        }

        public bool RunScheduledTasks()
        {

            IScheduler sched = schedFact.GetScheduler();
            sched.Start();

            //OVERNIGHT
            IJobDetail overnightJob = JobBuilder.Create<OvernightJob>()
               .WithIdentity("overnightJob", "group3")
               .Build();

            ITrigger overnightJobTrigger = TriggerBuilder.Create()
              .WithIdentity("myTrigger", "group3")
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(04, 10))
                 .StartNow()
              .Build();
          //  sched.ScheduleJob(overnightJob, overnightJobTrigger);

            ////LATE REPORT
            //IJobDetail lateReportJob = JobBuilder.Create<LateReportJob>()
            //   .WithIdentity("lateReportJob", "group4")
            //   .Build();

            //ITrigger lateReportJobTrigger = TriggerBuilder.Create()
            //  .WithIdentity("lateReportTrigger", "group4")
            //      .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(10, 03))
            //      .StartNow()s
            //  .Build();
            //sched.ScheduleJob(lateReportJob, lateReportJobTrigger);

            //CATERNET JOB 
            var triggerSet = new Quartz.Collection.HashSet<ITrigger>();
            IJobDetail caternetJob = JobBuilder.Create<PushToCaternetJob>()
               .WithIdentity("caternetJob", "group5")
               .Build();

            ITrigger caternetJobTrigger = TriggerBuilder.Create()
              .WithIdentity("caternetJobTrigger", "group5")
             //      .WithCronSchedule("0 53 03 ? * TUE-SUN")
                  .StartNow()
              .Build();

            ITrigger caternetJobSundayTrigger = TriggerBuilder.Create()
              .WithIdentity("caternetJobSundayTrigger", "group5")
            .WithCronSchedule("0 0 23 ? * SUN")
                  .StartNow()
              .Build();

            triggerSet.Add(caternetJobTrigger);
            triggerSet.Add(caternetJobSundayTrigger);

            sched.ScheduleJob(caternetJob, triggerSet, true);


            return true;

        }
    }
}
