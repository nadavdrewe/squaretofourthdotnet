using automatedreports.grind.railgunit.com.ScheduledTasks;
using Quartz;
using Quartz.Impl;
using shared.services.grind.railgunit.com.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace automatedreports.grind.railgunit.com
{
    class AutomatedReportsService : IService
    {
        private ISchedulerFactory schedFact;
        private TimeZoneInfo timeZoneInfo;


        public AutomatedReportsService()
        {
            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            schedFact = new StdSchedulerFactory();

        }

        public void Continue()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
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
                    eventLog.WriteEntry("Automated report service RunScheduledTasks has exceptioned" + ex.Message, EventLogEntryType.Information, 666, 1);
                }

            }
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public async void ScheduleTasks()
        {
            // get a scheduler
            IScheduler sched = await schedFact.GetScheduler();
            await sched.Start();

            //LATEST DAN REPORT V2 JOB
            IJobDetail __SecondOpsReportJournalJob = JobBuilder.Create<_SecondOpsReportJournalJob>()
               .WithIdentity("_SecondOpsReportJournalJob", "group9")
               .Build();
            ITrigger __SecondOpsReportJournalJobTrigger = TriggerBuilder.Create()
                .WithIdentity("__SecondOpsReportJournalJobTrigger", "group9")
               //       .WithCronSchedule("0 03 16 ? * *")
                       .WithCronSchedule("0 30 03 ? * WED")
                 .StartNow()
                .Build();
            await sched.ScheduleJob(__SecondOpsReportJournalJob, __SecondOpsReportJournalJobTrigger); //THIS IS ON

            //LATEST DAN REPORT V2 JOB
            //IJobDetail __VATRATEReportJournalJob = JobBuilder.Create<_VATRate_Cashup_RateReportJob>()
            //   .WithIdentity("__VATRATEReportJournalJob", "group101")
            //   .Build();
            //ITrigger ____VATRATEReportJournalJobTrigger = TriggerBuilder.Create()
            //    .WithIdentity("__VATRATEReportJournalJobTrigger", "group101")
            //          .WithCronSchedule("0 30 03 ? * WED")
            //     .StartNow()
            //    .Build();
            //         await sched.ScheduleJob(__VATRATEReportJournalJob, ____VATRATEReportJournalJobTrigger);


            //LATEST REPORT V3 JOB - EAT IN TAKE AWAY
            //IJobDetail _Salesv3ReportOpsReportJob = JobBuilder.Create<_SalesReportOpsReportJob>()
            //   .WithIdentity("_SalesV3ReportOpsReportJob", "group10")
            //   .Build();
            //ITrigger _SalesReportOpsReportJobTrigger = TriggerBuilder.Create()
            //  .WithIdentity("_SalesV3ReportOpsReportJobTrigger", "group10")
            //       .WithCronSchedule("0 50 04 ? * MON")
            //     .StartNow()
            //    .Build();
            //       await sched.ScheduleJob(_Salesv3ReportOpsReportJob, _SalesReportOpsReportJobTrigger);

            //NEW REPORT 
            IJobDetail _GeckoboardV2AutomatedJob = JobBuilder.Create<_SecondCashupJob>()
               .WithIdentity("_GeckoboardV2AutomatedJob", "group11")
               .Build();
            ITrigger _GeckoboardV2AutomatedJobTrigger = TriggerBuilder.Create()
              .WithIdentity("_GeckoboardV2AutomatedJobTrigger", "group11")
               // .WithSimpleSchedule(x => x.WithIntervalInSeconds(300).RepeatForever())           
                .WithCronSchedule("0 40 03 ? * *")
                 .StartNow()
                .Build();
            await sched.ScheduleJob(_GeckoboardV2AutomatedJob, _GeckoboardV2AutomatedJobTrigger);



            await sched.Start();
            Console.WriteLine("Scheduler started - jobs queued");
        }
    }
}
