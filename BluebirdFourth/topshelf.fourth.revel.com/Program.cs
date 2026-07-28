using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;


namespace topshelf.fourth.revel.com
{
    class Program
    {
        static void Main(string[] args)
        {

            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Application";
                eventLog.WriteEntry("Fourth Revel Job Main Method - Service Host Being Created", EventLogEntryType.Information, 0, 1);
            } 

              HostFactory.Run(x =>                                 
        {
            x.Service<IService>(s =>                        
            {
                s.ConstructUsing(name => new FourthService());   
                s.WhenStarted(tc => tc.Start());             
                s.WhenStopped(tc => tc.Stop());
                s.WhenPaused(tc => tc.Pause());
                s.WhenContinued(tc => tc.Continue());     
            });
            x.RunAsLocalSystem();

            x.SetDescription("Fourth Revel Service hosted by TopShelf");
            x.SetDisplayName("Fourth Revel Service");
            x.SetServiceName("FourthRevel");                       
        });               

        }
    }
}
