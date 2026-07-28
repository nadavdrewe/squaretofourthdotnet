using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using WebReboot.Grind._808nd.com;
using WebReboot.Grind._808nd.com.Controllers;

namespace scheduledtasks.grind._808nd.com.ScheduledTasks
{
    public class DemoGeckoPushJob : IJob
    {
        
        public async void Execute(IJobExecutionContext context)
        {
            DemoController service = new DemoController();
            await service.SendDemoWidgets();
        }
    }
}
