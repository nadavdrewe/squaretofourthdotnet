using Quartz.Unity;
using System.Diagnostics;
using Topshelf;
using Unity;

namespace xeroservice.grind.railgunit.com
{
    class Program
    {
        static IUnityContainer RegisterUnity()
        {


            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<QuartzUnityExtension>();

            container.RegisterType<XeroAutomatedService, XeroAutomatedService>();
            //container.RegisterType<xeroservice.grind.railgunit.com.IService, XeroAutomatedService>();

            return container;
        }




        static void Main(string[] args)
        {

            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Application";
                eventLog.WriteEntry("Xero Service Main Method - Service Host Being Created", EventLogEntryType.Information, 0, 1);
            }

            HostFactory.Run(x =>
            {
                using (var container = RegisterUnity())
                {
                    x.Service<IService>(s =>
                    {
                        s.ConstructUsing(name => new XeroAutomatedService());
                        s.WhenStarted(tc => tc.Start());
                        s.WhenStopped(tc => tc.Stop());
                        s.WhenPaused(tc => tc.Pause());
                        s.WhenContinued(tc => tc.Continue());
                    });

                    x.RunAsLocalSystem();
                    x.SetDescription("Xero Service  hosted by TopShelf");
                    x.SetDisplayName("Xero Service");
                    x.SetServiceName("Xero_Servoce");
                }

            });
        }
    }
}
