using CsvHelper;
using MongoDB.Driver;
using Quartz;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Models;
using Revel._808nd.com.OperationsReport.Factory;
using Revel._808nd.com.OperationsReport.Models;
using Revel._808nd.com.OperationsReport.Mongo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xero.railgunit.com.Grind.Utility;
using mailservice.railgunit.com.LessSecureGmailClient;
using Aspose.Email.Mail;
using shared.services.grind.railgunit.com.OpsReporting;

namespace xeroservice.grind.railgunit.com.ScheduledTasks
{
    public class _GenerateXeroOpsReportV1 : BaseJob
    {

        List<DansOpsReportv1> reportData = new List<DansOpsReportv1>();


        public override async Task Execute(IJobExecutionContext context)
        {
            //fixed time for end of report
            try
            {
                Bootstrap();
                List<string> allParentCats = new List<string>();
                List<DansOpsReportv1> allDataForRerport = new List<DansOpsReportv1>();

                var now = DateTime.Now;
                var endTimeForReport = new DateTime(now.Year, now.Month, now.Day, 04, 00, 00);
                var startTime = endTimeForReport.AddDays(-98); //fixed at 29th April
                //var startTime = new DateTime(2018, 08, 28, 04, 00, 00);                

                //29 nov
                //no of hour to start of report               

                var howManyHoursBackFromEndtime = Convert.ToInt32(((endTimeForReport - startTime).TotalHours));
                //var howManyHoursBackFromEndtime = 2304; //768;

                db = new GrindContext();
                allEstablishments = db.Establishments.Where(x => x.establishment_id != 2).ToList();


                using (var client = new GmailClient("grindandco808@gmail.com", "teenpunks23"))
                {
                    client.Send(new List<string> { "emailnadz@gmail.com" }, "Main hourly ops report generating", "Working on it");
                }
                ///////////////////
                //CHECK RANGE EXISTS - IF NOT POPULATE
                peristenceDataWrappers = OpsReportHourlyWrapperFactory.Create(endTimeForReport, howManyHoursBackFromEndtime, allEstablishments.Where(y => y.establishment_id != 2).Select(x => x.establishment_id).ToList()).OrderBy(x => x.containerStart).ToList();
                foreach (var wrapper in peristenceDataWrappers)
                {
                    Console.WriteLine("Doing wrapper: " + wrapper.containerStart);
                    //test if record exists already - if not, pull it
                    var doesExist = collection.AsQueryable().FirstOrDefault(x => x.containerStart == wrapper.containerStart && x.establishmentId == wrapper.establishmentId);
                    if (doesExist == null)
                    {
                        //query and save
                        await PopulateOpsReportWrapperFromRevel(wrapper);
                        SaveOpsDataToMongo(wrapper);
                    }
                    else
                    {
                        //already exists - replace 
                        //collection.FindOneAndDelete(x => x._id == doesExist._id);
                        //await PopulateOpsReportWrapperFromRevel(wrapper);
                        //SaveOpsDataToMongo(wrapper);
                    }
                }


                ///////////////////
                //REPORT GENERATION
                ///////////////////
                var bucketOfData = collection.AsQueryable().Where(x => x.containerStart >= startTime
                      && x.containerEnd <= endTimeForReport).ToList();
                //generate the hours for entire range - pass last datetime and how many hours back              
                var allHourlyDates = CreateHourlyRanges(endTimeForReport, howManyHoursBackFromEndtime);
                //for each grind, generate the data for each hour

                var barIncSoftAndJuice = "Bar inc. Soft and Juice";
                var problemDates = new List<string>();
                Console.WriteLine("Now gen estbs");
                foreach (var est in allEstablishments)
                {
                    Console.WriteLine("Est:" + est.name);
                    OpsReportHourlyWrapper currentWrapperForError;
                    //create parent data holder
                    var thisGrindDataHolder = new DansOpsReportv1 { GrindName = est.name, GrindIds = est.establishment_id.ToString() };

                    //get all for this establishment in entire range
                    List<OpsReportHourlyWrapper> allWrappersInRange = new List<OpsReportHourlyWrapper>();

                    //populate all wrappers
                    foreach (var hourRangeOpsReport in allHourlyDates.OrderBy(x => x.Start))
                    {
                        //get the matching hourly range from the db
                        var rangeWereLookingFor = bucketOfData.FirstOrDefault(x => x.containerStart == hourRangeOpsReport.Start
                        && x.containerEnd == hourRangeOpsReport.End
                        && x.establishmentId == est.establishment_id
                        );

                        if (rangeWereLookingFor == null)
                        {
                            var issue = String.Format("There was a problem with dates {0} to {1} for est: {2}", hourRangeOpsReport.Start.ToString(), hourRangeOpsReport.End.ToString(), est.name);
                            problemDates.Add(issue);
                            allWrappersInRange.Add(new OpsReportHourlyWrapper { opsReport = new RootObject { sales_data = new SalesData(), product_mix_data = new List<ProductMixData>() }, establishmentId = est.establishment_id, containerStart = hourRangeOpsReport.Start, containerEnd = hourRangeOpsReport.End });
                            // throw new Exception(String.Format("We couldn't find range {0} to {1} for {2} generating ops report", hourRangeOpsReport.Start, hourRangeOpsReport.End, est.name));
                        }
                        else
                        {
                            allWrappersInRange.Add(rangeWereLookingFor);
                        }
                    }


                    //got all data, now run queries on the data for each grind
                    //generate report items and add to list - dynamic

                    //var currentParentCats = allWrappersInRange.SelectMany(x => x.opsReport.product_mix_data.Select(y => y.parent_pclass)).Where(z => z != null).ToList().Distinct();
                    //currentParentCats.ToList().ForEach(x =>
                    //{
                    //    var exist = allParentCats.FirstOrDefault(y => y == x);
                    //    if (exist == null)
                    //    {
                    //        allParentCats.Add(x);
                    //    }
                    //});

                    //generate static cats
                    allParentCats = new List<string>
                    {
                        "Bar inc. Soft and Juice",
                        "Bar",
                        "Coffee/Hot Drinks",
                        "Food",
                        "Juice",
                        "Retail",
                        "Soft Drinks",
                        "Unknown Class"
                    };
                    allParentCats = allParentCats.OrderBy(x => x).ToList();

                    //now cycle through all data and generate our data                   
                    foreach (var wrapper in allWrappersInRange)
                    {
                        currentWrapperForError = wrapper;
                        var day = wrapper.containerStart.DayOfWeek;
                        //define all variables we need for each hour in report
                        //first fixed ones
                        var orderDiscounts = wrapper.opsReport.sales_data.order_discounts_total;
                        var itemDiscounts = wrapper.opsReport.sales_data.item_discounts;

                        if (orderDiscounts == null)
                        { thisGrindDataHolder.Data.Add(new DansOpsReportV1DateNameValuePair { Name = "Order Discount", Value = "0.00", RangeStart = wrapper.containerStart, RangeEnd = wrapper.containerEnd, DateOfWork = wrapper.containerStart.Date.ToString("dd/MM/yyyy") }); }
                        else { thisGrindDataHolder.Data.Add(new DansOpsReportV1DateNameValuePair { Name = "Order Discount", Value = orderDiscounts.ToString(), RangeStart = wrapper.containerStart, RangeEnd = wrapper.containerEnd, DateOfWork = wrapper.containerStart.Date.ToString("dd/MM/yyyy") }); }

                        if (itemDiscounts == null)
                        { thisGrindDataHolder.Data.Add(new DansOpsReportV1DateNameValuePair { Name = "Item Discount", Value = "0.00", RangeStart = wrapper.containerStart, RangeEnd = wrapper.containerEnd, DateOfWork = wrapper.containerStart.Date.ToString("dd/MM/yyyy") }); }
                        else { thisGrindDataHolder.Data.Add(new DansOpsReportV1DateNameValuePair { Name = "Item Discount", Value = itemDiscounts.ToString(), RangeStart = wrapper.containerStart, RangeEnd = wrapper.containerEnd, DateOfWork = wrapper.containerStart.Date.ToString("dd/MM/yyyy") }); }

                        //end discounts

                        //begin trans
                        var noOfTrasactions = wrapper.opsReport.sales_data.total_orders;
                        var noOfCovers = wrapper.opsReport.sales_data.total_number_of_people;

                        if (noOfTrasactions == null)
                        {
                            thisGrindDataHolder.Data.Add(new DansOpsReportV1DateNameValuePair { Name = "Transactions", Value = "0", RangeStart = wrapper.containerStart, RangeEnd = wrapper.containerEnd, DateOfWork = wrapper.containerStart.Date.ToString("dd/MM/yyyy") });
                        }
                        else { thisGrindDataHolder.Data.Add(new DansOpsReportV1DateNameValuePair { Name = "Transactions", Value = noOfTrasactions.ToString(), RangeStart = wrapper.containerStart, RangeEnd = wrapper.containerEnd, DateOfWork = wrapper.containerStart.Date.ToString("dd/MM/yyyy") }); }


                        if (noOfCovers == null)
                        {
                            thisGrindDataHolder.Data.Add(new DansOpsReportV1DateNameValuePair { Name = "Covers", Value = "0", RangeStart = wrapper.containerStart, RangeEnd = wrapper.containerEnd, DateOfWork = wrapper.containerStart.Date.ToString("dd/MM/yyyy") });
                        }
                        else { thisGrindDataHolder.Data.Add(new DansOpsReportV1DateNameValuePair { Name = "Covers", Value = noOfCovers.ToString(), RangeStart = wrapper.containerStart, RangeEnd = wrapper.containerEnd, DateOfWork = wrapper.containerStart.Date.ToString("dd/MM/yyyy") }); }

                        ///end tran


                        //use reflection to get names of cats, then for each cat - get corresponding items and ££ sales. 
                        //If cat is null in dataset then put zero

                        //create a new item for each cat - jus
                        foreach (var cat in allParentCats)
                        {

                            if (cat == barIncSoftAndJuice
                                || cat == "Bar"
                                || cat == "Juice"
                                || cat == "Soft Drinks"
                                )
                            {
                                //does the main cat exist for this datetime / store - if not, create it
                                var existingBarIncSoftAndJuice = thisGrindDataHolder.Data
                                    .Where(x => x.RangeStart == wrapper.containerStart)
                                    .Where(x => x.RangeEnd == wrapper.containerEnd)
                                    .Where(x => x.Name == barIncSoftAndJuice).ToList(); //this pulls both out
                                if (existingBarIncSoftAndJuice.Count() == 0)
                                {
                                    //couldn't find anything, add zero values row
                                    thisGrindDataHolder.Data.Add(new DansOpsReportV1DateNameValuePair { Name = barIncSoftAndJuice, Value = "0.00", RangeStart = wrapper.containerStart, RangeEnd = wrapper.containerEnd, DateOfWork = wrapper.containerStart.Date.ToString("dd/MM/yyyy"), Type = "sales" });
                                    thisGrindDataHolder.Data.Add(new DansOpsReportV1DateNameValuePair { Name = barIncSoftAndJuice, Value = "0", RangeStart = wrapper.containerStart, RangeEnd = wrapper.containerEnd, DateOfWork = wrapper.containerStart.Date.ToString("dd/MM/yyyy"), Type = "items" });

                                    existingBarIncSoftAndJuice = thisGrindDataHolder.Data
                                  .Where(x => x.RangeStart == wrapper.containerStart)
                                  .Where(x => x.RangeEnd == wrapper.containerEnd)
                                  .Where(x => x.Name == barIncSoftAndJuice).ToList(); //this pulls both out
                                }

                                //now bundle all the values into this cat if possible to get those values
                                try
                                {
                                    var tryGetCorrectData = wrapper.opsReport.product_mix_data.FirstOrDefault(x => x.product_class == cat);
                                    if (tryGetCorrectData != null)
                                    {
                                        //we using what for sales here?
                                        var sales = tryGetCorrectData.price;
                                        var items = Convert.ToDecimal(tryGetCorrectData.n_items);

                                        //THIS IS A STRING!! NEED TO CONVERT BACK AND SUM THEN ASSIGN
                                        var convertedSales = Convert.ToDecimal(existingBarIncSoftAndJuice.Where(x => x.Type == "sales").First().Value);
                                        var convertedItems = Convert.ToDecimal(existingBarIncSoftAndJuice.Where(x => x.Type == "items").First().Value);

                                        convertedSales += sales;
                                        convertedItems += items;

                                        //reassign
                                        existingBarIncSoftAndJuice.Where(x => x.Type == "sales").First().Value = sales.ToString();
                                        existingBarIncSoftAndJuice.Where(x => x.Type == "items").First().Value = items.ToString();
                                    }
                                }
                                catch (Exception)
                                {
                                    var holder = existingBarIncSoftAndJuice;
                                    var whoopsTheresAProblemWith = cat;
                                }


                            }
                            else
                            {
                                var tryGetCorrectData = wrapper.opsReport.product_mix_data.FirstOrDefault(x => x.product_class == cat);
                                if (tryGetCorrectData == null)
                                {
                                    //couldn't find anything, add zero values row
                                    thisGrindDataHolder.Data.Add(new DansOpsReportV1DateNameValuePair { Name = cat, Value = "0.00", RangeStart = wrapper.containerStart, RangeEnd = wrapper.containerEnd, DateOfWork = wrapper.containerStart.Date.ToString("dd/MM/yyyy"), Type = "sales" });
                                    thisGrindDataHolder.Data.Add(new DansOpsReportV1DateNameValuePair { Name = cat, Value = "0", RangeStart = wrapper.containerStart, RangeEnd = wrapper.containerEnd, DateOfWork = wrapper.containerStart.Date.ToString("dd/MM/yyyy"), Type = "items" });
                                }
                                else
                                {
                                    //we using what for sales here?
                                    var sales = tryGetCorrectData.price;
                                    var items = tryGetCorrectData.n_items;

                                    thisGrindDataHolder.Data.Add(new DansOpsReportV1DateNameValuePair { Name = cat, Value = sales.ToString(), RangeStart = wrapper.containerStart, RangeEnd = wrapper.containerEnd, DateOfWork = wrapper.containerStart.Date.ToString("dd/MM/yyyy"), Type = "sales" });
                                    thisGrindDataHolder.Data.Add(new DansOpsReportV1DateNameValuePair { Name = cat, Value = items.ToString(), RangeStart = wrapper.containerStart, RangeEnd = wrapper.containerEnd, DateOfWork = wrapper.containerStart.Date.ToString("dd/MM/yyyy"), Type = "items" });
                                }

                            }
                        }
                    }

                    var testJuice = thisGrindDataHolder.Data.Where(x => x.Name == barIncSoftAndJuice);
                    allDataForRerport.Add(thisGrindDataHolder);

                }

                var juiceAndShiz = allDataForRerport.Select(x => x.Data.Where(y => y.Name == barIncSoftAndJuice)).ToList();
                allDataForRerport = allDataForRerport.OrderBy(x => x.GrindIds).ToList();

                ///////////////////////////
                //now generate actual report
                ///////////////////////////                 
                //remove unneccessary cats now          
                allParentCats.Remove("Bar");
                allParentCats.Remove("Juice");
                allParentCats.Remove("Soft Drinks");
                //remove unnesseacry data now 
                allDataForRerport.ForEach(grindData =>
                {
                    var removeOldCatsData = grindData.Data.Where(x => x.Name == "Bar" || x.Name == "Juice" || x.Name == "Soft Drinks");
                    foreach (var item in removeOldCatsData)
                    {
                        grindData.Data.Remove(item);
                    }
                });

                var reportName = string.Format("OpsReport_{0}", startTime.ToString("dd_MM_yyyy"));
                var fullFilePath = String.Format(@"c:\test\{0}.csv", reportName);
                using (var writer = new StreamWriter(fullFilePath))
                {
                    var csv = new CsvWriter(writer);
                    //write a header row first
                    WriteHeaderRow(csv, allParentCats);


                    foreach (var grindTopLevelContainer in allDataForRerport)
                    {
                        var currentGrindName = grindTopLevelContainer.GrindName;
                        var grouping = grindTopLevelContainer.Data.GroupBy(x => x.DateOfWork);


                        //TEST CODE
                        //var testCode20thMayShoreditch = grindTopLevelContainer.Data.Where(y => y.RangeStart == new DateTime(2018, 05, 20, 09, 00, 00)).ToList();
                        //check coffee etc numbers heres

                        //write a new row for each hour                 
                        foreach (var dayGroup in grouping)
                        {
                            WriteCSVRows(csv, currentGrindName, dayGroup, allParentCats);
                        }


                    }

                    csv.Flush();
                }

                Console.WriteLine("CSV writing completed");
                Console.WriteLine("Error dates:");
                foreach (var item in problemDates)
                {
                    Console.WriteLine(item);
                }

                using (var client = new GmailClient("grindandco808@gmail.com", "teenpunks23"))
                {

                    var listOfPeopleToEmail = new List<string> {
                    //"michaela@grind.co.uk",
                    //"dan@grind.co.uk",
                    //"glynn@grind.co.uk",
                    "emailnadz@gmail.com"
                };

                    var toAttach = new List<Attachment> { new Attachment(fullFilePath) };
                    client.Send(listOfPeopleToEmail, "Ops Report", "Please find attached. Regards", toAttach);

                }


            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                throw;
            }

        }

        private void WriteCSVRows(CsvWriter csv,
            string currentGrindName,
            IGrouping<string, DansOpsReportV1DateNameValuePair> dayGroup,
            List<string> allParentCats)
        {

            //OBVIOUSLY THEY'RE NOT BEING ADDED PROPERLY!!!!!!
            var areThereANyItemsBarANdJuice = dayGroup.Where(x => x.Name == "Bar inc.Soft and Juice");
            var allDistinctDatesHoursInTheGroup = dayGroup.Select(x => x.RangeStart).Distinct().ToList().OrderBy(x => x);

            foreach (var dateAndHour in allDistinctDatesHoursInTheGroup) //should 
            {
                try
                {
                    var itemsWeWant = dayGroup.Where(x => x.RangeStart == dateAndHour).ToList(); //these are all items for that particular hour

                    csv.WriteField(currentGrindName);
                    csv.WriteField(itemsWeWant.First().DateOfWork);
                    csv.WriteField(itemsWeWant.First().RangeStart.Hour);

                    foreach (var cat in allParentCats.OrderBy(x => x).ToList())
                    {
                        //try get correct record from group
                        try
                        {
                            var recordSales = itemsWeWant.FirstOrDefault(x => x.Name == cat && x.Type == "sales").Value;
                            var recordItems = itemsWeWant.FirstOrDefault(x => x.Name == cat && x.Type == "items").Value;

                            csv.WriteField(recordSales);
                            csv.WriteField(recordItems);
                        }
                        catch (Exception ex)
                        {
                            var catThatThrew = cat;
                            var grindThatThrew = currentGrindName;
                            throw;
                        }
                    }

                    //write discounts
                    var orderDisc = itemsWeWant.FirstOrDefault(x => x.Name == "Order Discount").Value;
                    var itemDisc = itemsWeWant.FirstOrDefault(x => x.Name == "Item Discount").Value;
                    var trans = itemsWeWant.FirstOrDefault(x => x.Name == "Transactions").Value;
                    var covers = itemsWeWant.FirstOrDefault(x => x.Name == "Covers").Value;

                    //write tran and covers
                    csv.WriteField(orderDisc);
                    csv.WriteField(itemDisc);

                    csv.WriteField(trans);
                    csv.WriteField(covers);

                    csv.NextRecord();
                }
                catch (Exception ex)
                {
                    var wihchGrind = currentGrindName;
                    throw;
                }

            }

        }

        private void WriteHeaderRow(CsvWriter csv, List<string> allParentCats)
        {
            csv.WriteField("Site");
            csv.WriteField("Date");
            csv.WriteField("Hour");


            foreach (var cat in allParentCats.OrderBy(x => x).ToList())
            {
                csv.WriteField(cat + " sales");
                csv.WriteField(cat + " items");
            }


            csv.WriteField("Order Discount");
            csv.WriteField("Item Discount");

            csv.WriteField("Transactions");
            csv.WriteField("Covers");
            csv.NextRecord();

        }


    }
}
