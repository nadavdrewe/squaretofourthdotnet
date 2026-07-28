using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using domain.pipeline.fourth.com.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Serilog;
using squareservice.pipeline.fourth.com.Services;
using squareservice.pipeline.fourth.com.Job;
using squareservice.pipeline.fourth.com.JobFactory;
using ILArmyLogistics.Worker;

namespace squareservice.pipeline.fourth.com
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
       .SetBasePath(Directory.GetCurrentDirectory())
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
       .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
       //.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
       .Build();

        public static void Main(string[] args)
        {
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                var msg = "Square sales Service died:" + ex.Message;
                Log.Fatal(msg);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(hostContext.Configuration)
                        .CreateLogger();

                    // Serilog.Debugging.SelfLog.Enable(msg =>
                    // {
                    //     Debug.Print(msg);
                    //     Debugger.Break();
                    // });

                    //DO CONFIG DI
                    //services.AddHostedService<Worker>();
                    services.AddHostedService<QuartzHostedService>();
                    services.AddTransient<PushNightlyDataToSquareUSTimeZoneJob>();
                    services.AddSingleton<IPipelineAlertService, PipelineAlertService>();
                    services.AddDbContext<FourthPipelineContext>(options =>
                        options.UseSqlServer(hostContext.Configuration.GetConnectionString("FourthSalesPipelineContext")));
                    // Add Quartz services
                    services.AddSingleton<IJobFactory, SingletonJobFactory>();
                    services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

                    // Add our job
                    services.AddSingleton<QuartzHostedService>();
                    var cronExpression = hostContext.Configuration.GetValue<string>(
                        "SquareToFourthSales:CronExpression",
                        "0 0 11 * * ?");

                    IServiceCollection serviceCollection = services.AddSingleton(new Schedule(
                        jobType: typeof(PushNightlyDataToSquareUSTimeZoneJob),
                        cronExpression: cronExpression));

                    Console.WriteLine("job queued");
                })
           .UseWindowsService()
           .UseSerilog();
        
    }
}
