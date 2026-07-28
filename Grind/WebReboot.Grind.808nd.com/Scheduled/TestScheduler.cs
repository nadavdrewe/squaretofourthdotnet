using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using Quartz.Impl;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Models;
using Web.Grind._808nd.com.Controllers;

namespace Web.Grind._808nd.com.Scheduled
{
    public class TestScheduler : IJob
    {

        public async void Execute(IJobExecutionContext context)
        {
            var emailer = new EmailController();
            emailer.SendTestEmailToNadav();

          /*  var log = new ScheduledTaskLog()
            {
                Detail = context.JobDetail.Dump(),
                FireTime = DateTime.Now,                
                Message = "Time is now " + DateTime.Now.ToUniversalTime(),
                Result = context.Result.Dump()
            };

            using (var db = new GrindContext())
            {
                db.ScheduledTaskLogs.Add(log);
                await db.SaveChangesAsync();
            }*/

        }
       

    }
}