using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using WebReboot.Grind._808nd.com.Controllers;

namespace scheduledtasks.grind._808nd.com.ScheduledTasks
{
    public class LateReportJob : IJob
    {
        private string RevelBaseURL { get; } = ConfigurationManager.AppSettings["RevelBaseURL"];

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var service = new OpeningHoursController();
                Console.WriteLine("Late job has started");
                service.CheckIfAllStoresHaveOpenedLate();

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
