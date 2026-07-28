using Microsoft.Extensions.Configuration;
using Quartz;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace service.pipeline.fourth.com.ScheduledTasks
{
    public class NightlySquareSalesToFourthPushTask : IJob
    {
        void ConfigureLogger(string connectionstring, string tableName)
        {

            Log.Logger = new LoggerConfiguration()
            .WriteTo.MSSqlServer(connectionstring, tableName, autoCreateSqlTable: true)
            .CreateLogger();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            var connectionstring = configuration.GetConnectionString("FourthSalesPipelineContext");
            var tablename = configuration.GetValue<string>("LoggingTableName");

            ConfigureLogger(connectionstring, tablename);
            Log.Logger.Information("this is a test log");
            //update 
        }
    }
}
