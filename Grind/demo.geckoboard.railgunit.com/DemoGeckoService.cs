using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using interfaces.service.grind._808nd.com;
using Quartz;
using Quartz.Impl;
using scheduledtasks.grind._808nd.com;
using scheduledtasks.grind._808nd.com.ScheduledTasks;

namespace demo.geckoboard.railgunit.com
{
    public class DemoGeckoService : IService
    {
        private ISchedulerFactory schedFact = new StdSchedulerFactory();
        private TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");


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
                    eventLog.WriteEntry("Demo Gecko Service has exceptioned" + ex.Message, EventLogEntryType.Information, 666, 1);
                }

            }
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


            // 7 MIN
            IJobDetail repeatPushJob = JobBuilder.Create<DemoGeckoPushJob>()
                .WithIdentity("DemoGeckoPushJob", "group1")
                .Build();
            ITrigger repeatPushJobTrigger = TriggerBuilder.Create()
              .StartNow()
                .WithIdentity("myTrigger", "group1")
                      .WithSimpleSchedule(x => x.WithIntervalInSeconds(52)
                          .RepeatForever())
              .Build();
            sched.ScheduleJob(repeatPushJob, repeatPushJobTrigger);

            sched.Start();

            return true;
        }
    }
}
