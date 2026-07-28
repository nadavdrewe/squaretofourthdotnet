using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GeckoboardTestWebApp.Controllers;
using Quartz;
using Revel._808nd.com.Classes;
using Web.Grind._808nd.com.Controllers;

namespace scheduledtasks.grind._808nd.com
{
    public class _15minJob : IJob
    {
        async void IJob.Execute(IJobExecutionContext context)
        {
            if (RevelHelper.IsDateTimeCurrentlyWithinOpeningHours())
            {
                try
                {
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("http://http://dashboard.hummingbirdbakery.com");

                    using (var service = new EmailController())
                    {

                        var response = await client.GetAsync("/Account/Login");
                        //service.SendMessageNadavIgnoreSendExeceptions(String.Format("Ping was sent at {0}", DateTime.Now.ToString()), null, "railgunit.maintenance@gmail.com") ;
                    }


                    //that's it
                    //send request to the URL
                }
                catch (Exception ex)
                {

                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "Application";
                        eventLog.WriteEntry("15 min push has failed" + ex.Message, EventLogEntryType.Information, 666, 1);
                    }
                }

                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry("15 min push has finished", EventLogEntryType.Information, 0, 1);
                }

            }
        }
    }
}
