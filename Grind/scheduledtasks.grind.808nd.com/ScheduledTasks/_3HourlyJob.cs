    using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using GeckoboardTestWebApp.Controllers;
using Quartz;
using Revel._808nd.com.Classes;
using Web.Grind._808nd.com.Controllers;

namespace scheduledtasks.grind._808nd.com
{
    public class _3HourlyJob : IJob
    {
        async void IJob.Execute(IJobExecutionContext context)
        {
            //set up TLS
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback = new
            RemoteCertificateValidationCallback
            (
               delegate { return true; }
            );


            using (var emailer = new EmailController())
            {
                try
                {
                    emailer.SendMessageNadavIgnoreSendExeceptions("3 hr job - started in Card service");
                    if (RevelHelper.IsDateTimeCurrentlyWithinOpeningHours())
                    {
                        Console.WriteLine("Starting 3 hr job");
                        Debug.WriteLine("Starting card service");

                        SyncingController sc = new SyncingController();
                        var stopwatch = new Stopwatch();
                        stopwatch.Start();
                        emailer.SendMessageNadavIgnoreSendExeceptions("3 hr job - started full card customer sync in  update in Card service", null, "railgunit.maintenance@gmail.com");
                        var ok = await sc.FullCustomerAndCardSync();
                        stopwatch.Stop();
                        emailer.SendMessageNadavIgnoreSendExeceptions(
                            String.Format(
                                "3 hr job - finished full card customer sync in  update in Card service, took: {0}",
                                stopwatch.Elapsed), null, "railgunit.maintenance@gmail.com");

                        stopwatch.Restart();
                        emailer.SendMessageNadavIgnoreSendExeceptions("3 hr job - started SyncNewRewardLogs() in  update in Card service", null,
                            "railgunit.maintenance@gmail.com");
                        await sc.SyncNewRewardLogs();

                        await sc.FullGiftCardSync();
                        stopwatch.Stop();
                        emailer.SendMessageNadavIgnoreSendExeceptions(
                            String.Format("3 hr job - finished SyncNewRewardLogs() in  update in Card service, took: {0}",
                                stopwatch.Elapsed), null, "railgunit.maintenance@gmail.com");


                    }
                    else
                    {
                        emailer.SendMessageNadavIgnoreSendExeceptions("3 hr job - didn't run because wasn't in opening hours");

                    }
                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "Application";
                        eventLog.WriteEntry("Grind 3Hour Job finished", EventLogEntryType.Information, 0, 1);
                    }
                    Console.WriteLine("3 hr job complete");
                    Debug.WriteLine("3 hr job complete");
                }
                catch (Exception exception)
                {
                    emailer.SendMessageNadavIgnoreSendExeceptions("DANGER - 3 hr job failed! - Grind card service", "<p>" + exception.Message + "</p><br/><br/><h1>Inner exception</h1><p>" + exception.InnerException + "</p>");
                    throw;
                }
            }
        }
    }
}

