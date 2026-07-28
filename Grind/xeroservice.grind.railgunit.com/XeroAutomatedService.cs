using Quartz;
using Quartz.Impl;
using Quartz.Simpl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using xeroservice.grind.railgunit.com.ScheduledTasks;

namespace xeroservice.grind.railgunit.com
{
    public class ContainerJobFactory : PropertySettingJobFactory
    {
        private readonly IUnityContainer container;

        public ContainerJobFactory(IUnityContainer container)
        {
            this.container = container;
        }

        public override IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var job = container.Resolve(bundle.JobDetail.JobType);
            if (ReferenceEquals(job, null))
                return base.NewJob(bundle, scheduler);
            SetObjectProperties(job, bundle.JobDetail.JobDataMap);
            return (IJob)job;
        }
    }


    public class XeroAutomatedService : IService
    {
        private ISchedulerFactory schedFact;
        private TimeZoneInfo timeZoneInfo;


        public XeroAutomatedService()
        {
            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            schedFact = new StdSchedulerFactory();

        }

        public void Start()
        {
            try
            {
                ScheduleTasks();
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



        public async void ScheduleTasks()
        {
            // get a scheduler
            IScheduler sched = await schedFact.GetScheduler();
            await sched.Start();


            ////MONDAY MORNING XERO PUSH TASK - DISABLED AS OF 12/12/2018

            //IJobDetail _15minJob = JobBuilder.Create<_PushAccountsToXero>()
            //    .WithIdentity("_HourlyJob", "group2")
            //    .Build();
            //ITrigger _15minJobTrigger = TriggerBuilder.Create()
            //    .WithIdentity("myTrigger", "group2")
            //           .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(04, 11))
            //      .StartNow()
            //    .Build();
            //await sched.ScheduleJob(_15minJob, _15minJobTrigger);


            //HOURLY XERO OPS REPORT GATHER
            IJobDetail _1hrOpsJob = JobBuilder.Create<_HourlyOpsReportDownload>()
                .WithIdentity("_1hrOpsJob", "group3")
                .Build();
            ITrigger _1hrOpsJobTrigger = TriggerBuilder.Create()
                .WithIdentity("_1hrOpsJobTrigger", "group3")
                       .WithSimpleSchedule(x => x
                       .WithIntervalInHours(1)
                       .RepeatForever())
                  .StartNow()
                .Build();
            await sched.ScheduleJob(_1hrOpsJob, _1hrOpsJobTrigger);

            ////DAILY XERO OPS BACKUP JOB
            //IJobDetail _24hrOpsJob = JobBuilder.Create<_DailyOpsReportDownload>()
            //    .WithIdentity("_24hrOpsJob", "group4")
            //    .Build();
            //ITrigger _24hrOpsJobTrigger = TriggerBuilder.Create()
            //    .WithIdentity("_24hrOpsJobTrigger", "group4")
            //      .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(04, 20))
            //      .StartNow()
            //    .Build();
            //await sched.ScheduleJob(_24hrOpsJob, _24hrOpsJobTrigger);


            ////DAILY XERO OPS REPORT
            IJobDetail _GenerateOpsJobReportV1 = JobBuilder.Create<_GenerateXeroOpsReportV1>()
                .WithIdentity("_GenerateOpsJobReportV1Job", "group5")
                .Build();
            ITrigger _GenerateOpsJobReportV1Trigger = TriggerBuilder.Create()
                .WithIdentity("_GenerateOpsJobReportV1Trigger", "group5")
                        .WithCronSchedule("0 02 04 ? * MON")
                 .StartNow()
                .Build();
            await sched.ScheduleJob(_GenerateOpsJobReportV1, _GenerateOpsJobReportV1Trigger);



            ////SELIMA CSV JOB            
            IJobDetail _SelimaCSVJob = JobBuilder.Create<_SelimaCSVJob>()
                .WithIdentity("_SelimaDailyJob", "group7")
                .Build();
            ITrigger _SelimaCSVJobTrigger = TriggerBuilder.Create()
                .WithIdentity("__SelimaDailyJobTrigger", "group7")
                 .WithCronSchedule("0 25 04 ? * TUE-SUN")
                 .StartNow()
                .Build();
            await sched.ScheduleJob(_SelimaCSVJob, _SelimaCSVJobTrigger);


            //SUNDAY JOB
            IJobDetail _SelimaCSVSundayJob = JobBuilder.Create<_SelimaCSVJob>()
               .WithIdentity("_SelimaSundayJob", "group7")
               .Build();
            ITrigger _SelimaCSVSundayJobTrigger = TriggerBuilder.Create()
                .WithIdentity("__SelimaSundayJobTrigger", "group7")
                   .WithCronSchedule("0 25 20 ? * SUN")
                 .StartNow()
                .Build();
            await sched.ScheduleJob(_SelimaCSVSundayJob, _SelimaCSVSundayJobTrigger);



            await sched.Start();

        }
    }
}
