using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using interfaces.service.grind._808nd.com;
using Topshelf;

namespace demo.geckoboard.railgunit.com
{
    class Program
    {
        static void Main(string[] args)
        {
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Application";
                eventLog.WriteEntry("Grind Service Main Method - Service Host Being Created", EventLogEntryType.Information, 0, 1);
            }

            HostFactory.Run(x =>
            {
                x.Service<IService>(s =>
                {
                    s.ConstructUsing(name => new DemoGeckoService());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                    s.WhenPaused(tc => tc.Pause());
                    s.WhenContinued(tc => tc.Continue());
                });

                x.RunAsLocalSystem();
                x.SetDescription("Railgun IT Demo Gecko Service hosted by TopShelf");
                x.SetDisplayName("Railgun IT Demo Gecko Service");
                x.SetServiceName("DemoGeckoService");

            });

        }
    }
}
