using System;
using System.Configuration;
using System.Diagnostics;
using interfaces.service.grind._808nd.com;
using scheduledtasks.grind._808nd.com;
using Quartz;
using Quartz.Impl;

namespace cardservice.grind._808nd.com
{
    class GrindCardService : IService
    {
        private string RevelBaseURL { get; } = ConfigurationManager.AppSettings["RevelBaseURL"];
        private ISchedulerFactory schedFact;
        private TimeZoneInfo timeZoneInfo;

        public GrindCardService()
        {
            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            schedFact = new StdSchedulerFactory();
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
            // get a scheduler
            try
            {
                IScheduler sched = schedFact.GetScheduler();
                sched.Start();

                IJobDetail _3HourlyJob = JobBuilder.Create<_3HourlyJob>()
                  .WithIdentity("_4HourlyJob", "_4HourlyJobGroup")
                  .Build();
                ITrigger _3HourlyJobTrigger = TriggerBuilder.Create()
                    .WithIdentity("myTrigger", "_4HourlyJobGroup")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInHours(2)
                      .RepeatForever()
                      )
                    .Build();
                sched.ScheduleJob(_3HourlyJob, _3HourlyJobTrigger);


                //3AM
                IJobDetail _3amJob = JobBuilder.Create<_3amJob>()
                   .WithIdentity("overnightJob", "group4")
                   .Build();
                ITrigger _3amJobtrigger = TriggerBuilder.Create()
                  .WithIdentity("myTrigger", "group4")
                  .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(02, 50))
                  .StartNow()
                  .Build();
                sched.ScheduleJob(_3amJob, _3amJobtrigger);

                sched.Start();

                return true;
            }
            catch (Exception ex)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry("Exception:" + ex.Message, EventLogEntryType.Information, 666, 1);
                }

                throw new Exception("Grind Card Service cannot run scheduled tasks", ex);
            }

        }
    }
}
