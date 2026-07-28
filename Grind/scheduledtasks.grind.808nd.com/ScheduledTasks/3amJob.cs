using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using GeckoboardTestWebApp.Controllers;
using Quartz;

namespace scheduledtasks.grind._808nd.com
{
    public class _3amJob : IJob
    {
        async void IJob.Execute(IJobExecutionContext context)
        {    //set up TLS
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback = new
            RemoteCertificateValidationCallback
            (
               delegate { return true; }
            );

            Console.WriteLine("Starting 3am Push");
            SyncingController sc = new SyncingController();
            var ok = await sc.Run3amRoutineWrapper();
        
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Application";
                eventLog.WriteEntry("3am push has finished", EventLogEntryType.Information, 1, 1);
            }

            Console.WriteLine("Finished 3am Push");
        }
    }
}
