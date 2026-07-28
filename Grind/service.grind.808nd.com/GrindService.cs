using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using interfaces.service.grind._808nd.com;
using Quartz;
using Quartz.Impl;
using scheduledtasks.grind._808nd.com;


namespace service.grind._808nd.com
{
    public class GrindService : IService
    {
        private ISchedulerFactory schedFact;
        private TimeZoneInfo timeZoneInfo;


        public GrindService()
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
            try
            {
                var ok = RunScheduledTasks();
            }
            catch (Exception ex)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry("RunScheduledTasks has exceptioned" + ex.Message, EventLogEntryType.Information, 666, 1);
                }

            }
        }

        public void Stop()
        {

        }


        public void Pause()
        {

        }

        public void Continue()
        {


        }


        public bool RunScheduledTasks()
        {

            // get a scheduler
            IScheduler sched = schedFact.GetScheduler();
            sched.Start();


            // 7 MIN
            IJobDetail _7minPushJob = JobBuilder.Create<_7minPushJob>()
                .WithIdentity("_7minPushJob", "group1")
                .Build();
            ITrigger _7minPushJobTrigger = TriggerBuilder.Create()
              .StartNow()
                .WithIdentity("myTrigger", "group1")
                      .WithSimpleSchedule(x => x.WithIntervalInSeconds(1240)
                          .RepeatForever())
              .Build();
            sched.ScheduleJob(_7minPushJob, _7minPushJobTrigger);


            //////HOURLY
            IJobDetail _15minJob = JobBuilder.Create<_15minJob>()
                .WithIdentity("_HourlyJob", "group2")
                .Build();
            ITrigger _15minJobTrigger = TriggerBuilder.Create()
                .WithIdentity("myTrigger", "group2")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(22)
                    .RepeatForever())
                .Build();

            sched.ScheduleJob(_15minJob, _15minJobTrigger);



            sched.Start();


            return true;

        }



    }
}
