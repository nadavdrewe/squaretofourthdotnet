using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace scheduledtasks.grind._808nd.com
{
    public class WeeklySync : IJob
    {
        void IJob.Execute(IJobExecutionContext context)
        {
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Application";
                eventLog.WriteEntry("Weekly push has finished", EventLogEntryType.Information, 0, 1);
            }
        }
    }
}
