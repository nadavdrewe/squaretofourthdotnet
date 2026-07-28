using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Grind._808nd.com.Controllers;

namespace scheduledtasks.grind._808nd.com
{

    public class FundingTask : IJob
    {
        public async void Execute(IJobExecutionContext context)
        {
            var tc = new Test1Controller();
            var ok = await tc.TestPullGrindTotal();
        }
    }
}
