using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Revel._808nd.com.Classes;
using GeckoboardTestWebApp.Controllers;
using Web.Grind._808nd.com.Controllers;


namespace scheduledtasks.grind._808nd.com
{
    public class _7minPushJob : IJob
    {
        async void IJob.Execute(IJobExecutionContext context)
        {
            if (RevelHelper.IsDateTimeCurrentlyWithinOpeningHours())
            {

                using (SyncingController theController = new SyncingController())
                {
                    try
                    {
                        using (var mailer = new EmailController())
                        {

                            var ok = await theController.ordersUpdateAndGeckoPush();
                            mailer.SendMessageNadavIgnoreSendExeceptions("7 min push succeeded at" + DateTime.Now, null,
                                "railgunit.maintenance@gmail.com");
                        }

                    }
                    catch (Exception ex)
                    {
                        using (var mailer = new EmailController())
                        {
                           mailer.SendMessageNadavIgnoreSendExeceptions("DANGER - 7 min push Exceptioned at" + DateTime.Now, String.Format("<h3>Exception: " + ex.Message + "</h3><br/><br/><h3>Src" + ex.Source.ToString() + "</h3>"));

                            using (EventLog eventLog = new EventLog("Application"))
                            {
                                eventLog.Source = "Application";
                                eventLog.WriteEntry("7 min push has exceptioned" + ex.Message,
                                    EventLogEntryType.Information, 666, 1);
                            }
                        }

                    }


                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "Application";
                        eventLog.WriteEntry("7 min push has finished", EventLogEntryType.Information, 0, 1);
                    }

                }


            }
        }

    }
}
