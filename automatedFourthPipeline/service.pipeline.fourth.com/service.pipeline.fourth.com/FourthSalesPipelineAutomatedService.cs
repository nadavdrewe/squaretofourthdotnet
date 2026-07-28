using Quartz;
using Quartz.Impl;
using service.pipeline.fourth.com.ScheduledTasks;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace service.pipeline.fourth.com
{
    class FourthSalesPipelineAutomatedService
    {
        private ISchedulerFactory schedFact;
        private TimeZoneInfo timeZoneInfo;

        public FourthSalesPipelineAutomatedService()
        {
            //init scheduler, usual stuff
            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            schedFact = new StdSchedulerFactory();
        }

        public async Task Start()
        {

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback = new
            RemoteCertificateValidationCallback
            (
               delegate { return true; }
            );

            Console.WriteLine("We are started!!");

            await ScheduleTasks();
        }

        public async Task ScheduleTasks()
        {
            //set up all tasks
            IScheduler sched = await schedFact.GetScheduler();
            await sched.Start();

            //refresh prods / cats and receipts
            IJobDetail _overnightJob = JobBuilder.Create<NightlySquareSalesToFourthPushTask>()
                .WithIdentity("_OvernightJob", "_OvernightJob")
                .Build();
            ITrigger _overnightJobTrigger = TriggerBuilder.Create()
                .WithIdentity("myTrigger", "_OvernightJob")
                //.WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(01, 30)) //when it runs
                .StartNow()
                .Build();
            await sched.ScheduleJob(_overnightJob, _overnightJobTrigger);

        }

        public void Stop()
        {

        }
    }
}
