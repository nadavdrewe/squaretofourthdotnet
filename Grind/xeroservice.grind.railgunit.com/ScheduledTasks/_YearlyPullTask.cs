using MongoDB.Bson.IO;
using MongoDB.Driver;
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
    public class _YearlyPullTask : BaseJob
    {
        public override async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var hoursBack = 24;//720 * 11;

                //WE HAVE FROM MID FEB
                //var hoursBack = 336;


                Bootstrap();
                //get latest record - if none exists, start from 1st may 2017
                allEstablishments = db.Establishments.Where(x => x.establishment_id != 2).ToList();
                DateTime myDate;
                //var latest = collection.AsQueryable().Where(x => x.containerStart < new DateTime(2018, 05, 01)).OrderByDescending(x => x.containerStart).FirstOrDefault();
                //if (latest != null)
                //{
                //    myDate = latest.containerStart;
                //}


                //else { myDate = new DateTime(2017, 05, 01, 00, 00, 00); }

                //TEST
                myDate = new DateTime(2018, 01, 01, 00, 00, 00);

                //we want to do 2 weeks AFTER last date - then all the hour in between
                //var theEndDate = myDate.AddDays(14);
                var theEndDate = myDate;
                Init(theEndDate, hoursBack);//leavea  bit of extra window               


                peristenceDataWrappers = OpsReportHourlyWrapperFactory.Create(finalDateToPullTo, howManyHoursBack, allEstablishments.Where(y => y.establishment_id != 2).Select(x => x.establishment_id).ToList()).OrderBy(x => x.containerStart).ToList();


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
                    else
                    {
                        //already exists - replace
                        collection.FindOneAndDelete(x => x._id == doesExist._id);
                        await PopulateOpsReportWrapperFromRevel(wrapper);
                        SaveOpsDataToMongo(wrapper);
                    }
                }

                Console.WriteLine("Service complete!!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception:" + ex.Message);
                throw;
            }

        }
    }
}
