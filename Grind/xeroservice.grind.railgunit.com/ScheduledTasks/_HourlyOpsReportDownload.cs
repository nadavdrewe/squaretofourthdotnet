using MongoDB.Driver;
using Newtonsoft.Json;
using Quartz;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.Utility;
using Revel._808nd.com.Models;
using Revel._808nd.com.OperationsReport.Factory;
using Revel._808nd.com.OperationsReport.Models;
using Revel._808nd.com.OperationsReport.Mongo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace xeroservice.grind.railgunit.com.ScheduledTasks
{
    public class _HourlyOpsReportDownload : BaseJob
    {


        public override async Task Execute(IJobExecutionContext context)
        {

            //THIS JOB IS TRIGGERED EVERY HOUR
            //CHECKS RECORDS FOR (UP TO) THE THE LAST FOUR HOURS EXIST - IF NOT, WILL SYNC THEM

            //generate start dates
            var myDate = DateTime.Now; //get current time
            //var myDate = new DateTime(2018, 05, 21, 00, 00, 00);

            Init(myDate, 72);

            peristenceDataWrappers = OpsReportHourlyWrapperFactory.Create(finalDateToPullTo, howManyHoursBack, allEstablishments.Select(x => x.establishment_id).ToList()).OrderBy(x => x.containerStart).ToList();

            foreach (var wrapper in peristenceDataWrappers)
            {
                //test if record exists already - if not, pull it
                var doesExist = collection.Find(x => x.containerStart == wrapper.containerStart && x.establishmentId == wrapper.establishmentId).FirstOrDefault();
                if (doesExist == null)
                {
                    //query and save
                    await PopulateOpsReportWrapperFromRevel(wrapper);
                    SaveOpsDataToMongo(wrapper);
                }
            }



        }
    }
}
