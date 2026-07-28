using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using interfaces.service.grind._808nd.com;
using Topshelf;



namespace service.grind._808nd.com
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
                    s.ConstructUsing(name => new GrindService());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                    s.WhenPaused(tc => tc.Pause());
                    s.WhenContinued(tc => tc.Continue());
                });

                x.RunAsLocalSystem();
                x.SetDescription("Grind Daily Service hosted by TopShelf");
                x.SetDisplayName("Grind Daily Service");
                x.SetServiceName("GrindService");

            });
        }
    }
}
