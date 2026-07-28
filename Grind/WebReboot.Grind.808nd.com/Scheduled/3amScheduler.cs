using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Quartz;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Models;
using Web.Grind._808nd.com.Controllers;

namespace Web.Grind._808nd.com.Scheduled
{
    public class _3amScheduler : IJob
    {
        public async void Execute(IJobExecutionContext context)
        {
            var log = new ScheduledTaskLog();
            try
            {
                var sync = new SyncController();
                var ok = await sync.Run3amRoutine();
                

                //log this job
                log = new ScheduledTaskLog()
                {
                    Detail = "",//context.JobDetail.Dump(),
                    FireTime = DateTime.Now.ToUniversalTime(),
                    Message =
                        "3am Sync - Reset Red Card, Sync Customers/Cards, Multiply Points and Create Card Timestamps",
                    Result = 1
                };

                using (var db = new GrindContext())
                {
                    db.ScheduledTaskLogs.Add(log);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                log = new ScheduledTaskLog()
                {
                    Detail = "",//context.JobDetail.Dump(),
                    FireTime = DateTime.Now.ToUniversalTime(),
                    Message =
                        "FAILED: 3am Sync - Reset Red Card, Sync Customers/Cards, Multiply Points and Create Card Timestamps",
                    Result = 0
                };              

            }
            finally
            {

                using (var db = new GrindContext())
                {
                    db.ScheduledTaskLogs.Add(log);
                    db.SaveChanges();
                }
            }
        }
    }
}