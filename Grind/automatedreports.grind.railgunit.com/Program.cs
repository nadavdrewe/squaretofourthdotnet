using shared.services.grind.railgunit.com.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace automatedreports.grind.railgunit.com
{
    class Program
    {
        static void Main(string[] args)
        {
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Application";
                eventLog.WriteEntry("AutomatedReportsService Service Main Method - Service Host Being Created", EventLogEntryType.Information, 0, 1);
            }

            HostFactory.Run(x =>
            {
                x.Service<IService>(s =>
                {
                    s.ConstructUsing(name => new AutomatedReportsService());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                    s.WhenPaused(tc => tc.Pause());
                    s.WhenContinued(tc => tc.Continue());
                });

                x.RunAsLocalSystem();
                x.SetDescription("AutomatedReportsService hosted by TopShelf");
                x.SetDisplayName("AutomatedReportsService Service");
                x.SetServiceName("AutomatedReportsService");

            });
        }

    }
    
}

