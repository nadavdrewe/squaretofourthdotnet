using CsvHelper;
using MongoDB.Driver;
using Quartz;
using Revel._808nd.com.Classes;
using Revel._808nd.com.OperationsReport.Factory;
using Revel._808nd.com.OperationsReport.Models;
using Revel._808nd.com.SelimaFTPClient;
using shared.services.grind.railgunit.com.OpsReporting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xeroservice.grind.railgunit.com.ScheduledTasks
{
    public class _SelimaCSVJob : BaseJob
    {
        public override async Task Execute(IJobExecutionContext context)
        {
            try
            {
                //FTP
                var ftpSite = "ftpro.selima.co.uk";
                var ftpPass = "Mu$k0g33#7";
                var user = "Grind";

                var selimaFTPClient = new SelimaFTPClient();

                Bootstrap();
                //get latest record - if none exists, start from 1st may 2017
                allEstablishments = db.Establishments.Where(x => x.establishment_id != 2).ToList();
                DateTime myDate = DateTime.Now;
                if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday && DateTime.Now >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 00, 00))
                {
                    myDate = DateTime.Now.AddDays(1); //this is for the sunday evening run
                }

                var howManYHours = 24;

                // var endDateRevelTime = new DateTime(2018, 10, 20, 03, 00, 00);
                var endDateRevelTime = new DateTime(myDate.Year, myDate.Month, myDate.Day, 03, 00, 00);
                var startDateRevelTime = endDateRevelTime.AddHours(-howManYHours);

                Init(endDateRevelTime, howManYHours);//leave a  bit of extra window
                peristenceDataWrappers = OpsReportHourlyWrapperFactory.Create(finalDateToPullTo, howManyHoursBack, allEstablishments.Where(y => y.establishment_id != 2).Select(x => x.establishment_id).ToList()).OrderBy(x => x.containerStart).ToList();

                //GET ANY MISSING DATA
                foreach (var wrapper in peristenceDataWrappers)
                {
                    //test if record exists already - if not, pull it
                    try
                    {
                        var doesExist = collection.Find(x => x.containerStart == wrapper.containerStart && x.establishmentId == wrapper.establishmentId).FirstOrDefault();
                        if (doesExist == null)
                        {
                            //query and save
                            await PopulateOpsReportWrapperFromRevel(wrapper);
                            SaveOpsDataToMongo(wrapper);
                        }
                    }
                    catch (Exception ex)
                    {
                        var thisError = wrapper;
                        throw;
                    }
                }


                var allHourlyDates = CreateHourlyRanges(endDateRevelTime, howManYHours);
                //GENERATE ONE CSV FOR EACH GRIND AND POST                

                List<OpsReportHourlyWrapper> allWrappersInRange = new List<OpsReportHourlyWrapper>();
                foreach (var hourRangeOpsReport in allHourlyDates.OrderBy(x => x.Start))
                {
                    //get the matching hourly range from the db
                    var rangeWereLookingFor = collection.Find(x => x.containerStart == hourRangeOpsReport.Start
                    && x.containerEnd == hourRangeOpsReport.End
                    ).ToList();


                    foreach (var est in allEstablishments)
                    {
                        var forEst = rangeWereLookingFor.FirstOrDefault(x => x.establishmentId == est.establishment_id);
                        if (forEst == null)
                        {
                            throw new Exception(String.Format("We couldn't find range {0} to {1} for {2} generating ops report", hourRangeOpsReport.Start, hourRangeOpsReport.End, est.name));
                        }
                    }

                    allWrappersInRange.AddRange(rangeWereLookingFor);
                }



                //we've got the ranges - now generate the CSV
                //var parentCats = allWrappersInRange.SelectMany(x => x.opsReport.product_mix_data.Select(y => y.parent_pclass)).Where(z => z != null).ToList().Distinct();
                var parentCats = new List<string>
                    {
                        "Bar inc. Soft and Juice",
                        "Coffee/Hot Drinks",
                        "Food",
                        "Retail",
                        "Unknown Class"
                    };
                parentCats = parentCats.OrderBy(x => x).ToList();

                foreach (var est in allEstablishments)
                {

                    var csvTitle = String.Format("cash_{0}_{1}", startDateRevelTime.ToString("dd-MM-yyyy"), est.name); //needs to include the world 'cash'
                    var recordsForThisBranch = allWrappersInRange.Where(x => x.establishmentId == est.establishment_id).OrderBy(x => x.containerStart).ToList();


                    var allRowsForThisEst = new List<SelimaCSVRow>();
                    //got records, now, divide up into necessary bundles for Selima categories
                    foreach (var hourContainerRow in recordsForThisBranch)
                    {
                        var drinksSum = Decimal.Round(Convert.ToDecimal(hourContainerRow.opsReport.product_mix_data.Where(x => x.product_class == "Bar"
                        || x.product_class == "Bar inc. Soft and Juice"
                        ).
                        Sum(x => x.price)), 2);

                        var foodSUm = Decimal.Round(Convert.ToDecimal(hourContainerRow.opsReport.product_mix_data.Where(x => x.product_class == "Food"
                        || x.product_class == "Unknown Class"
                        ).
                        Sum(x => x.price)), 2);

                        var hotDrinksSum = Decimal.Round(Convert.ToDecimal(hourContainerRow.opsReport.product_mix_data.Where(x =>
                        x.product_class == "Coffee/Hot Drinks"
                        || x.product_class == "Retail").
                        Sum(x => x.price)), 2);


                        var siteIDMappingCode = SelimaMappingService.Map(est.establishment_id);
                        var dateString = String.Format("{0}/{1}/{2}", hourContainerRow.containerStart.ToString("dd"), hourContainerRow.containerStart.ToString("MM"), hourContainerRow.containerStart.ToString("yy"));
                        //now generate a selima row for each
                        var hotDrinksRow = new SelimaCSVRow
                        {
                            SiteIdentifier = siteIDMappingCode.ToString(),
                            Date = dateString,
                            Hour = hourContainerRow.containerStart.Hour.ToString(),
                            RevenueKey = "hot drinks",
                            Value = hotDrinksSum.ToString()
                        };

                        var foodRow = new SelimaCSVRow
                        {
                            SiteIdentifier = siteIDMappingCode.ToString(),
                            Date = dateString,
                            Hour = hourContainerRow.containerStart.Hour.ToString(),
                            RevenueKey = "food",
                            Value = foodSUm.ToString()
                        };


                        var drinksRow = new SelimaCSVRow
                        {
                            SiteIdentifier = siteIDMappingCode.ToString(),
                            Date = dateString,
                            Hour = hourContainerRow.containerStart.Hour.ToString(),
                            RevenueKey = "drinks",
                            Value = drinksSum.ToString()
                        };


                        allRowsForThisEst.Add(hotDrinksRow);
                        allRowsForThisEst.Add(foodRow);
                        allRowsForThisEst.Add(drinksRow);
                    }


                    //output the CSV from the rows

                    //generate header row
                    var genFilenaem = String.Format(@"c:\test\{0}.csv", csvTitle);
                    var filesTOSend = new List<string>();
                    using (var writer = new StreamWriter(genFilenaem))
                    {
                        var csv = new CsvWriter(writer);
                        //write a header row first
                        WriteHeaderRow(csv);
                        foreach (var item in allRowsForThisEst)
                        {
                            WriteCSVRow(csv, item);
                        }

                        csv.Flush();

                        var log = new ScheduledTaskLog
                        {
                            Detail = "Selima service ran",
                            FireTime = DateTime.Now,
                            Result = 1,
                            Message = String.Format("For Branch {0} - OK running Selima overnight routine", est.name),
                            /*Brand = brand.brand_id,
                            BrandName = brand.name,*/
                            Establishment = 0,
                            EstablishmentName = est.name,
                            LogType = "SELIMA - OK",
                            //ContainerEndDate = star,
                            //ContainerStartDate = syncStart,
                            User = "Automated - Selima Task"

                        };
                        db.ScheduledTaskLogs.Add(log);
                        db.SaveChanges();
                    }


                    selimaFTPClient.Upload(ftpSite, user, ftpPass, genFilenaem, csvTitle + ".csv");
                    ///END FTP
                    //post CSVs to Selima

                    var message = String.Format("The service ran correctly for {0} at {1}", DateTime.Now, est.name);
                    Console.WriteLine(message);
                    SendEmailNotification("Selima Service Completed!", message);
                    //write log

                }

                Console.WriteLine("Job completed");

            }
            catch (Exception ex)
            {
                var log = new ScheduledTaskLog
                {
                    Detail = "The scheduler failed" + ex.Message + ex.InnerException,
                    FireTime = DateTime.Now,
                    Result = 0,
                    Message = "Error running Selima overnight routine {0}.",
                    /*Brand = brand.brand_id,
                    BrandName = brand.name,*/
                    Establishment = 0,
                    EstablishmentName = "",
                    LogType = "SELIMA - ERROR",
                    //ContainerEndDate = star,
                    //ContainerStartDate = syncStart,
                    User = "Automated - Selima Task"

                };

                db.ScheduledTaskLogs.Add(log);
                db.SaveChanges();


                Console.WriteLine("Exception:" + ex.Message);
                SendEmailNotification("Exception: Selima Service", ex.Message);
                throw;
            }

        }


        private void WriteHeaderRow(CsvWriter csv)
        {
            csv.WriteField("Site Identifier");
            csv.WriteField("Date");
            csv.WriteField("Revenue Key");

            csv.WriteField("Hour");
            csv.WriteField("Value");

            csv.NextRecord();
        }

        private void WriteCSVRow(CsvWriter csv, SelimaCSVRow rowPoco)
        {

            csv.WriteField(rowPoco.SiteIdentifier);
            csv.WriteField(rowPoco.Date);
            csv.WriteField(rowPoco.RevenueKey);

            csv.WriteField(rowPoco.Hour);
            csv.WriteField(rowPoco.Value);
            csv.NextRecord();

        }

    }
}
