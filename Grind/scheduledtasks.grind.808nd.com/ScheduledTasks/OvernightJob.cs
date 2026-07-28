using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using GeckoboardTestWebApp.Controllers;
using Quartz;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.Reporting.ReportingFactory;
using Revel._808nd.com.Models;
using Web.Grind._808nd.com.Controllers;
using Web.Grind._808nd.com.Services;

namespace scheduledtasks.grind._808nd.com
{
    public class OvernightJob : IJob
    {

        private string RevelBaseURL { get; } = ConfigurationManager.AppSettings["RevelBaseURL"];

        async void IJob.Execute(IJobExecutionContext context)
        {
            try
            {
                using (var emailer = new EmailController())
                {
                    emailer.SendMessageNadavIgnoreSendExeceptions(String.Format("Grind overnight service update has started at {0}", RevelBaseURL));


                    SyncingController sc = new SyncingController();
                    var tc = new TestController();

                    Console.WriteLine("Starting Products");
                    await tc.UpdateDatabaseProductsAndCategories();
                    /*    await tc.GetDiscountsAndSaveToDB();*/
                    Console.WriteLine("Products updated");
                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "Application";
                        eventLog.WriteEntry("Grind products updated has finished", EventLogEntryType.Information, 1, 1);
                        emailer.SendMessageNadavIgnoreSendExeceptions(String.Format("Grind products update has finished at {0}", RevelBaseURL));
                    }

                    Console.WriteLine("Starting Yesterday Widgets");
                    await tc.RunYesterdayWidgets();
                    Console.WriteLine("Grind yesterday widgets updated");
                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "Application";
                        eventLog.WriteEntry("Grind yesterday widgets updated", EventLogEntryType.Information, 1, 1);
                        emailer.SendMessageNadavIgnoreSendExeceptions(String.Format("Grind yesterday widgets update has finished at {0}", RevelBaseURL));
                    }

                    await tc.RunCombinedOvernightWidgets();
                    Console.WriteLine("Overnight widgets updated");
                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "Application";
                        eventLog.WriteEntry("Grind Overnight widgets updated", EventLogEntryType.Information, 1, 1);
                        emailer.SendMessageNadavIgnoreSendExeceptions(String.Format("Grind overnight widgets update has finished at {0}", RevelBaseURL));
                    }


                    emailer.SendMessageNadavIgnoreSendExeceptions(String.Format("Overnight Grind Gecko succeded at {0}", RevelBaseURL));
                }

                using (var repo = new GrindContext())
                {
                    repo.ScheduledTaskLogs.Add(new ScheduledTaskLog
                    {
                        Detail = "Location " + RevelBaseURL,
                        Message = "Overnight Gecko Succeeded!",
                        Result = 1


                    });
                    repo.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                using (var mailer = new EmailController())
                {
                    mailer.SendMessageNadavIgnoreSendExeceptions("DANGER - overnight job exceptioned out at" + DateTime.Now, String.Format("<h3>Exception: " + ex.Message + "</h3><br/><br/><h3>Src" + ex.Source.ToString() + "</h3>"));
                }

                using (var repo = new GrindContext())
                {
                    repo.ScheduledTaskLogs.Add(new ScheduledTaskLog
                    {
                        Detail = "Exception: " + ex.Message,
                        Message = "Overnight Gecko Failed",
                        FireTime = DateTime.Now,

                    });
                    repo.SaveChanges();
                }
            }


            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Application";
                eventLog.WriteEntry("Overnight push has failed", EventLogEntryType.Error, 666, 1);
            }


        }

        private void RunWeeklyAccountingReport()
        {
            var db = new GrindContext();


            var end = DateTime.Now.AddDays(-1);
            /*var start = end.AddDays(-91);*/
            var start = new DateTime(2015, 12, 06);

            var listOfFiles = new List<string>();
            var establihsments = db.Establishments
                .Where(x => x.establishment_id != 2)
                .Where(x => x.establishment_id != 9).ToList();

            var factory = new OrderItemReportFactory();

            var currentEst = "";

            foreach (var est in establihsments)
            {

                try
                {
                    var orderItemsOUtPut = new List<OrderItem>();
                    var reportData = factory.CreateProductOrderItemSummaryReport(new ReportContext
                    {
                        StartDate = start,
                        EndDate = end,
                        NoOfDaysInEachReportingPeriod = 7,
                        IdOfStore = est.establishment_id
                    }, new GrindContext(), out orderItemsOUtPut, "");

                    var csvPath = String.Format("c:\\temp\\{0}_{1}.csv", est.name, DateTime.Now.ToLongDateString());

                    /*      ListToCSVExtensions.CreateCSVFromGenericList(reportData, csvPath);*/


                    using (var csv = new CsvWriter(new StreamWriter(csvPath)))
                    {
                        csv.Configuration.HasHeaderRecord = true;
                        csv.Configuration.Delimiter = ",";
                        csv.WriteRecords(reportData);
                    }

                    listOfFiles.Add(csvPath);
                }
                catch (Exception exception)
                {

                    var toerror = new List<string>();
                    /*  to.Add("dan@grindandco.com");*/
                    /*to.Add("michaela@grindandco.com");*/
                    toerror.Add("emailnadz@gmail.com");
                    MailService mailer = new MailService(toerror, "ERROR: Weekly Item Report in Establihsment " + est.name + "  - here's where we got up to", null, listOfFiles);
                    mailer.SendEmail();
                }
            }


            //mailout
            var to = new List<string>();
            to.Add("dan@grindandco.com");
            to.Add("michaela@grindandco.com");
            to.Add("emailnadz@gmail.com");
            MailService mail = new MailService(to, "Weekly Item Report", null, listOfFiles);
            mail.SendEmail();
        }
    }
}
