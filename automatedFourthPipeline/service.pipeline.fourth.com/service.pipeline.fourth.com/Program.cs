using Microsoft.Extensions.DependencyInjection;
using System;
using Topshelf;

namespace service.pipeline.fourth.com
{
    class Program
    {
        private static IServiceProvider _serviceProvider;

        private static void DisposeServices()
        {
            if (_serviceProvider == null)
            {
                return;
            }
            if (_serviceProvider is IDisposable)
            {
                ((IDisposable)_serviceProvider).Dispose();
            }
        }

        private static void RegisterServices()
        {
            var collection = new ServiceCollection();

            //collection.AddScoped<IDemoService, DemoService>();
            // ...
            // Add other services
            // ...
            _serviceProvider = collection.BuildServiceProvider();
        }

        static void Main(string[] args)
        {
            RegisterServices();

            Console.WriteLine("Hello World! About to Start FourthSalesPipelineAutomatedService Service");
            HostFactory.Run(windowsService =>
            {
                windowsService.Service<FourthSalesPipelineAutomatedService>(s =>
                {
                    s.ConstructUsing(service => new FourthSalesPipelineAutomatedService());
                    s.WhenStarted(service => service.Start().GetAwaiter().GetResult());
                    s.WhenStopped(service => Stop());
                });

                windowsService.RunAsLocalSystem();
                windowsService.StartAutomatically();

                windowsService.SetDescription("FourthSalesPipelineAutomatedService");
                windowsService.SetDisplayName("FourthSalesPipelineAutomatedService");
                windowsService.SetServiceName("FourthSalesPipelineAutomatedService");
            });

        }


        static void Stop() => DisposeServices();
    }
}
