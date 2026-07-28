//using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.Ajax.Utilities;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Revel._808nd.com.Classes.Logging;
using Revel._808nd.com.Models;
using Revel._808nd.com.Classes;
using web.fourth.revel.com.Controllers;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading.Tasks;
using Revel._808nd.com.Classes.FourthModelMapping;
using Revel._808nd.com.Classes.ServiceImplementaitons;
using Revel._808nd.com.FourthClient;
using System.Data.Entity;
using System.Net;
using System.Net.Security;

namespace web.fourth.revel.com.ScheduledTasks
{
    public class PushToFourth3amJob : IJob
    {
        public async void Execute(IJobExecutionContext context)
        {
            //var syncStart = DateTime.Now.AddDays(-1);
            //var syncEnd = syncStart.AddDays(1);
            //set up TLS

            //set up TLS
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback = new
            RemoteCertificateValidationCallback
            (
               delegate { return true; }
            );


            //SETUP DATES
            var DatesToExclude = new List<DateTime>
            {
                //new DateTime(2020, 06, 18),
                //new DateTime(2020, 06, 23),
                //new DateTime(2020, 06, 26),
                //new DateTime(2020, 06, 27),
            };

            //DOESN@T INCLUDE FINAL DATE            
            var startCycle = new DateTime(2026, 01, 24, 04, 00, 00);
            var endCycle = new DateTime(2026, 02, 07, 04, 00, 00);

            //var startCycle = DateTime.Now.AddDays(-1);
            //var endCycle = startCycle.AddDays(1);

            var dateSet = new List<DateTime>();
            var currentDate = startCycle;
            while (currentDate < endCycle)
            {
                if (!DatesToExclude.Contains(currentDate))
                {
                    dateSet.Add(currentDate);

                }
                currentDate = currentDate.AddDays(1);
                //build the set
            }

            //use the set

            foreach (var aDateToPushDatafor in dateSet)
            {
                Console.WriteLine("Now running date: " + aDateToPushDatafor);
                var db = new RevelContext();

                var syncStart = new DateTime(aDateToPushDatafor.Year, aDateToPushDatafor.Month, aDateToPushDatafor.Day, 04, 00, 00);
                var syncEnd = syncStart.AddDays(1);


                var itemService = new OrderItemsService();

                try
                {
                    //SendMaintainenceEmail("Bluebird Service About To Run", "");
                    var brandsPushed = await BrandPushToFourth(db, itemService, syncStart, syncEnd);
                    Console.WriteLine("One day completed!!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Was exception - crashed: " + ex.Message);
                    var log = new ScheduledTaskLog
                    {
                        Detail = "The scheduler failed" + ex.Message + ex.InnerException,
                        FireTime = DateTime.Now,
                        Result = 0,
                        Message = "Error running overnight routine - please investigate.",
                        /*Brand = brand.brand_id,
                        BrandName = brand.name,*/
                        Establishment = 0,
                        EstablishmentName = "",
                        LogType = "ERROR",
                        ContainerEndDate = syncEnd,
                        ContainerStartDate = syncStart,
                        User = "Automated - 3am Task"
                    };

                    db.ScheduledTaskLogs.Add(log);
                    db.SaveChanges();
                }
            }
        }

        private static void SendMaintainenceEmail(string subject, string htmlBody)
        {
            try
            {
                var toStart = new List<string>();
                toStart.Add("emailnadz@gmail.com");
                MailService startMail = new MailService(toStart,
                    subject,
                    htmlBody);
                startMail.SendEmail();

                startMail = null;
            }
            catch (Exception)
            {
                //supress

            }
        }


        //for failures
        class BrandAndEmail
        {
            public int BrandID { get; set; }

            public string EmailAddress { get; set; }

        }


        public static string GetEmergencyEmail(int brandID)
        {
            List<BrandAndEmail> emergencyEmailMaps = new List<BrandAndEmail>{
                new BrandAndEmail {BrandID = 12, EmailAddress = "emailnadz@gmail.com" },
            };

            return emergencyEmailMaps.FirstOrDefault(x => x.BrandID.Equals(brandID)).EmailAddress;
        }

        public async Task<int> BrandPushToFourth(RevelContext db, OrderItemsService itemService, DateTime syncStart, DateTime syncEnd)
        {
            var currentEst = new Establishment(); //for logging

            var brandToPullOrdersFor = new List<Brand>();
            ////REMOVE



            brandToPullOrdersFor = db.Brands
                //.Where(x=>x.brand_id == 27)                
                .Where(x => x.is_fourth_active).ToList();

            if (brandToPullOrdersFor.Count() > 0)
            {
                foreach (var brand in brandToPullOrdersFor)
                {

                    //ExceptionLogAndEmail(db, syncStart, syncEnd, brand, new Exception("This is a test exception"));
                    var clock = Stopwatch.StartNew();
                    Console.WriteLine("Now starting Brand: " + brand.name.ToString());

                    try
                    {
                        //SendMaintainenceEmail("Brand started: " + brand.name, "");
                        var productsService = new ProductsController();

                        //pull orderItems
                        var fourthClient = new FourthClient();
                        var orderItemservice = new OrderItemsService();

                        var orderService = new OrderController();
                        var systemService = new OrderItemController();
                        var estService = new EstablishmentsController();

                        //var estSuccesful = await estService.RefreshEstablishments(brand.brand_id);
                        try
                        {
                            //await productsService.RefreshProductsByBrand(brand.brand_id);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("COULD NOT UPDATE PRODS");
                            //SendMaintainenceEmail("FOURTH - DANGER - COULDN'T UPDATED PRODUCTS: " + brand.name, "");
                        }


                        Console.WriteLine("Puling orders");
                        //get the orders for same periods - so we can filter
                        var orders = await orderService.PullOrdersFromRevelForBrand(brand, db, syncStart, syncEnd);
                        var orderItems =
                            await
                                systemService.PullOrderItemsFromRevelForBrand(itemService, brand,
                                    syncStart,
                                    syncEnd, 4000);

                        Console.WriteLine("Finished items");
                        //avoid null errors
                        if (orderItems == null)
                        {
                            orderItems = new List<OrderItem>();
                        }


                        //filter discounts and voids
                        var voided = new List<OrderItem>();
                        var discounted = new List<OrderItem>();

                        var orderItemsThatAreCompsOrVoids = orderItems.Where(i => i.ervc_type == "7" || i.ervc_type == "8" || i.ervc_type == "9"
                                     || i.ervc_type == "5"
                                     || i.ervc_type == "6").ToList();


                        orderItems = orderItemservice.FilterVoidItems(orderItems, out voided) as List<OrderItem>;
                        discounted = orderItemservice.GetDiscountedItems(orderItems) as List<OrderItem>;

                        //Filter split bills and other                                                
                        //get orders that are split bills
                        var ordersIDSWithSplitBills = orders.Where(x => !String.IsNullOrWhiteSpace(x.bill_parent)).Select(x => x.order_id).ToList();
                        var ordersWithoutSplitBills = orders.Where(x => !ordersIDSWithSplitBills.Any(anId => anId == x.order_id)).ToList();
                        var ordersWithDiscount = orders.Where(x => x.discount_amount > 0).ToList();
                        var totalDiscountAmount = ordersWithDiscount.Sum(x => x.discount_amount);
                        //THIS IS THE ONE WHERE WE FILTER OUT THE ORDER ITEMS
                        var orderItemsWeNeedToDelete = new List<OrderItem>();

                        foreach (var splitBillId in ordersIDSWithSplitBills)
                        {
                            foreach (var item in orderItems)
                            {
                                if ((int)splitBillId == item.parent_order_id)
                                {
                                    db.Entry(item).State = EntityState.Deleted;
                                    orderItemsWeNeedToDelete.Add(item); //just to have a list
                                }
                            }
                        }

                        if (orderItemsWeNeedToDelete.Count() > 0)
                        {
                            db.SaveChanges();
                        }

                        //var orderItemsWeNeedToDelete = orderItems.Where(x => ordersIDSWithSplitBills.All(anId => anId == (int?)x.parent_order_id)).ToList();

                        //THEN DELETE THESE ITEMS FROM THE DB

                        //END

                        //get summed items
                        var startParam = new SqlParameter("@startDate", SqlDbType.DateTime);
                        startParam.Value = syncStart;
                        var endParam = new SqlParameter("@endDate", SqlDbType.DateTime);
                        endParam.Value = syncEnd;
                        var brandParam = new SqlParameter("@brandId", SqlDbType.Int);
                        brandParam.Value = brand.brand_id;


                        //PROC FILTERS OUT VOIDS
                        var summedOrders = db.Database.SqlQuery<RevelSummedOrderItems>(
                            "Revel_Fourth_OrderItems @startDate, @endDate, @brandId", startParam, endParam, brandParam).ToList();

                        if (summedOrders == null)
                        {
                            summedOrders = new List<RevelSummedOrderItems>();
                        }

                        //LOG
                        var pullFromRevelLog = new ScheduledTaskLog
                        {

                            Detail = "Brand:" + brand.name + " " + "No of items:" + orderItems.Count(),
                            FireTime = DateTime.Now,
                            Result = 1,
                            Message = "OrderItems downloaded from Revel successfully!",
                            Brand = brand.brand_id,
                            BrandName = brand.name,
                            Establishment = 0,
                            EstablishmentName = "",
                            TotalItemCount = summedOrders.Count(),
                            TotalPounds = summedOrders.Sum(x => x.GROSS_W_MODIFIERS),
                            LogType = "LOCAL",
                            ContainerEndDate = summedOrders.Max(x => x.CREATED_DATE),
                            ContainerStartDate = summedOrders.Min(x => x.CREATED_DATE),
                            User = "Automated",
                            TotalItemQuantity = summedOrders.Sum(x => x.QUANTITY),
                            TotalVAT = summedOrders.Sum(x => x.TAX),
                            TotalItemVoidedCount = voided.Count(),
                            TotalItemVoidedAmount = voided.Sum(x => x.discount_amount),
                            TotalItemDiscountCount = discounted.Count(),
                            TotalItemDiscountAmount = discounted.Sum(x => x.discount_amount),
                            TotalItemDiscountTax = discounted.Sum(x => x.tax_amount),
                        };

                        db.ScheduledTaskLogs.Add(pullFromRevelLog);
                        db.SaveChanges();


                        //log any sku errors
                        try
                        {
                            var itemsWithNoSkU = orderItemservice.LogItemsWithNoSKU(orderItems, db, brand);
                            //email out 
                            string concatItems = "";
                            foreach (var item in itemsWithNoSkU)
                            {
                                concatItems += item.GetType().GetProperty("Name").GetValue(item, null) + "<br/>";
                            }

                            if (itemsWithNoSkU.Any())
                            {
                                try
                                {
                                    var to = new List<string>();
                                    to.Add("emailnadz@gmail.com");
                                    to.Add("support@bluebird-global.com");
                                    //MailService mail = new MailService(to,
                                    //    "Error in Bluebird / Fourth Overnight Task - Items with no SKU",
                                    //    "<h3>There were a number of items with no SKU in brand: " + brand.name +
                                    //    ". They are:<br/><br/>" + concatItems + "</h3>");
                                    //mail.SendEmail();
                                }
                                catch (Exception ex)
                                {
                                    // throw new Exception("Couldn't email out", ex);
                                    //suprress
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                        try
                        {
                            //push out to fourth and log
                            //n.b depends on if it's per brand or per establishment!!!
                            if (summedOrders.Any() && !brand.fourth_username.IsNullOrWhiteSpace()
                                && !brand.fourth_password.IsNullOrWhiteSpace())
                            {
                                fourthClient.Login(brand.fourth_username, brand.fourth_password);
                                var xml = "";

                                //submit all except voids and NOSKU                              
                                var fourthCode = 0;



                                if (false)
                                {
                                    //brand based

                                    LogFourthXML(db, summedOrders, brand, voided, discounted);
                                    fourthCode = (int)fourthClient.SubmitSalesRequestToFourth(summedOrders, brand, out xml);

                                    //SendMaintainenceEmail("Brand completed Successfully: " + brand.name, "");
                                    var log = new ScheduledTaskLog
                                    {
                                        Detail = "Brand:" + brand.name + " " + "No of items:" + summedOrders.Count(),
                                        FireTime = DateTime.Now,
                                        Result = 1,
                                        Message = "XML submitted to Fourth successfully! Code returned: " + fourthCode,
                                        Brand = brand.brand_id,
                                        BrandName = brand.name,
                                        Establishment = 0,
                                        EstablishmentName = "",
                                        TotalItemCount = summedOrders.Count(),
                                        TotalPounds = summedOrders.Sum(x => x.PURE_SALES_PLUS_TAX),
                                        LogType = "FOURTH",
                                        ContainerEndDate = summedOrders.Max(x => x.CREATED_DATE),
                                        ContainerStartDate = summedOrders.Min(x => x.CREATED_DATE),
                                        User = "Automated",
                                        TotalItemQuantity = summedOrders.Sum(x => x.QUANTITY),
                                        TotalVAT = summedOrders.Sum(x => x.TAX),
                                        TotalItemVoidedCount = voided.Count(),
                                        TotalItemVoidedAmount = voided.Sum(x => x.discount_amount),
                                        TotalItemDiscountCount = discounted.Count(),
                                        TotalItemDiscountAmount = discounted.Sum(x => x.discount_amount),
                                        TotalItemDiscountTax = discounted.Sum(x => x.tax_amount),
                                        Notes = xml
                                    };
                                    db.ScheduledTaskLogs.Add(log);
                                }
                                else
                                {
                                    //establishment based
                                    var ests = db.Establishments
                                        .Where(x => x.is_fourth_active)
                                        // .Where(x => x.establishment_id == 2)
                                        .Where(x => x.db_brand_id == brand.brand_id).ToList();

                                    foreach (var est in ests.ToList())
                                    {
                                        //get                                       

                                        Console.WriteLine("Now pushing sales for " + est.name);
                                        var thisEstOrderItemsOnly =
                                            summedOrders.Where(x => x.ESTABLISHMENT_ID == est.DBKEY_establishment_id)
                                                .ToList();
                                        var thisEstVoided = voided.Where(x => x.db_establishment_id == est.DBKEY_establishment_id)
                                                .ToList();
                                        var thisEstDiscount = discounted.Where(x => x.db_establishment_id == est.DBKEY_establishment_id)
                                                .ToList();

                                        var withoutSKU = thisEstOrderItemsOnly.Where(x => x.SKU == "").ToList();
                                        var withSKU = thisEstOrderItemsOnly.Except(withoutSKU).ToList();


                                        Console.WriteLine("There were {0} items without SKU", withoutSKU.Count());

                                        if (thisEstOrderItemsOnly.Count() > 0)
                                        {

                                            LogFourthXML(db, summedOrders, brand, voided, discounted, est);
                                            fourthCode = (int)fourthClient.SubmitSalesRequestToFourth(thisEstOrderItemsOnly, brand, out xml, est);
                                            var log = new ScheduledTaskLog
                                            {
                                                Detail = "Establishent:" + est.name + " in brand" + brand.name + " No of items:" + summedOrders.Count(),
                                                FireTime = DateTime.Now,
                                                Result = 1,
                                                Message = "XML submitted to Fourth successfully! Code returned: " + fourthCode,
                                                Brand = brand.brand_id,
                                                BrandName = brand.name,
                                                Establishment = est.DBKEY_establishment_id,
                                                EstablishmentName = est.name,
                                                TotalItemCount = thisEstOrderItemsOnly.Count(),
                                                TotalPounds = thisEstOrderItemsOnly.Sum(x => x.GROSS_W_MODIFIERS),
                                                LogType = "FOURTH",
                                                ContainerEndDate = thisEstOrderItemsOnly.Max(x => x.CREATED_DATE),
                                                ContainerStartDate = thisEstOrderItemsOnly.Min(x => x.CREATED_DATE),
                                                User = "Automated",
                                                TotalItemQuantity = thisEstOrderItemsOnly.Sum(x => x.QUANTITY),
                                                TotalVAT = thisEstOrderItemsOnly.Sum(x => x.TAX),
                                                TotalItemVoidedCount = thisEstVoided.Count(),
                                                TotalItemVoidedAmount = thisEstVoided.Sum(x => x.discount_amount),
                                                TotalItemDiscountCount = thisEstDiscount.Count(),
                                                TotalItemDiscountAmount = thisEstDiscount.Sum(x => x.discount_amount),
                                                TotalItemDiscountTax = thisEstDiscount.Sum(x => x.tax_amount),
                                                Notes = xml
                                            };
                                            db.ScheduledTaskLogs.Add(log);

                                        }
                                        else
                                        {
                                            var log = new ScheduledTaskLog
                                            {
                                                Detail = "Establishent:" + est.name + " in brand" + brand.name + " No of items:" + summedOrders.Count(),
                                                FireTime = DateTime.Now,
                                                Result = 1,
                                                Message = "XML not submitted to Fourth as there were no items.",
                                                Brand = brand.brand_id,
                                                BrandName = brand.name,
                                                ContainerEndDate = syncStart,
                                                ContainerStartDate = syncEnd,
                                                Establishment = est.DBKEY_establishment_id,
                                                EstablishmentName = est.name,
                                                TotalItemCount = thisEstOrderItemsOnly.Count(),
                                                LogType = "FOURTH",
                                                User = "Automated",
                                                TotalItemQuantity = 0,
                                                TotalVAT = 0,
                                                TotalItemVoidedCount = 0,
                                                TotalItemVoidedAmount = 0,
                                                TotalItemDiscountCount = 0,
                                                TotalItemDiscountAmount = 0,
                                                TotalItemDiscountTax = 0,
                                                Notes = xml
                                            };
                                            db.ScheduledTaskLogs.Add(log);
                                        }



                                    }
                                }

                                if (fourthCode == 0)
                                {
                                    Console.WriteLine("FOURTHCODE WAS 0");
                                    var to = new List<string>();
                                    to.Add("emailnadz@gmail.com");
                                    to.Add("support@bluebird-global.com");
                                    to.Add("c@bluebird-global.com");
                                    Console.WriteLine("DANGER - Error in Bluebird / Fourth Overnight Task - IT message - was 0");

                                    //MailService mail = new MailService(to,
                                    //    "DANGER -  Error in Bluebird / Fourth Overnight Task - IT message",
                                    //    "<h3>The Bluebird/Fourth service returned 0. This is an IT message only, please do something");
                                    //mail.SendEmail();

                                }


                                clock.Stop();

                                db.SaveChanges();


                            }
                            else //no order items or couldn't log in
                            {
                                try
                                {
                                    var to = new List<string>();

                                    to.Add("support@bluebird-global.com");
                                    to.Add("emailnadz@gmail.com");
                                    to.Add("c@bluebird-global.com");
                                    //MailService mail = new MailService(to, "Error in Bluebird / Fourth Overnight Task",
                                    //    "<h3>The Bluebird/Fourth service did not send to Fourth for Brand: " +
                                    //    brand.name + ". " + "There were " + orderItems.Count() + " items retrieved from Revel.<br/> " +
                                    //    "If the number of items is 0, you may ignore this error, else you need to check the username / password credentials for Fourth are correct, or contact a sys admin.</h3>");
                                    //mail.SendEmail();


                                    //send email to client
                                    //var clientEmail = emergencyEmailMaps.FirstOrDefault(x => x.BrandID == brand.brand_id);


                                }
                                catch (Exception ex)
                                {


                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(String.Format("EXCEPTION: {0} - {1}", ex.Message, ex.InnerException));
                            var log = new ScheduledTaskLog
                            {
                                Detail = "The scheduler failed on establishment" + ex.Message + ex.InnerException + "but will try and continue for the next establishment",
                                FireTime = DateTime.Now,
                                Result = 0,
                                Message =
                                "Invidual Brand Error - see detail",
                                /*Brand = brand.brand_id,
                                BrandName = brand.name,*/
                                Establishment = 0,
                                EstablishmentName = "",
                                LogType = "ERROR",
                                ContainerEndDate = syncEnd,
                                ContainerStartDate = syncStart,
                                User = "Automated - 3am Task"

                            };

                            db.ScheduledTaskLogs.Add(log);
                            db.SaveChanges();

                            var to = new List<string>();
                            to.Add("emailnadz@gmail.com");
                            to.Add("c@bluebird-global.com");
                            to.Add("support@bluebird-global.com");
                            //MailService mail = new MailService(to,
                            //    "Error in Bluebird / Fourth Overnight Task - IT message",
                            //    "<h3>The Bluebird/Fourth service did not send to Fourth. This is an IT message only, pleas check the logs");
                            //mail.SendEmail();



                        }

                        Console.WriteLine("Finishing Brand:" + brand.name.ToString() + " took " + clock.Elapsed.TotalSeconds);
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogAndEmail(db, syncStart, syncEnd, brand, ex);
                    }
                }

            }
            Console.WriteLine("All Brands complete.");
            return brandToPullOrdersFor.Count;
        }

        private static void ExceptionLogAndEmail(RevelContext db, DateTime syncStart, DateTime syncEnd, Brand brand, Exception ex)
        {
            var log = new ScheduledTaskLog
            {

                Detail = "The scheduler failed " + ex.Message + ex.InnerException,
                FireTime = DateTime.Now,
                Result = 0,
                Message =
                                            "Invidual Brand Error - see detail",
                Brand = brand.brand_id,
                /*Brand = brand.brand_id,
                BrandName = brand.name,*/
                BrandName = brand.name,
                Establishment = 0,
                EstablishmentName = "",
                LogType = "ERROR",
                ContainerEndDate = syncEnd,
                ContainerStartDate = syncStart,
                User = "Automated - 3am Task"

            };

            db.ScheduledTaskLogs.Add(log);
            db.SaveChanges();

            var to = new List<string>();
            to.Add("emailnadz@gmail.com");
            to.Add("support@bluebird-global.com");
            to.Add("c@bluebird-global.com");
            //MailService mail = new MailService(to,
            //   "WARNING - Error in Bluebird / Fourth Overnight Task - Failure To Send To Fourth",
            //     String.Format("<h3>The Bluebird/Fourth service did not send to Fourth. This is an IT message only, pleas check the logs, because the scheduler failed for Brand {0}</h3><br/><h5>Techincal details: " + ex.Message + " " + ex.InnerException, brand.name));
            //mail.SendEmail();



        }

        private static void LogFourthXML(RevelContext db, List<RevelSummedOrderItems> summedOrders, Brand brand,
            List<OrderItem> voided, List<OrderItem> discounted, Establishment est = null)
        {
            var establishmentForLog = "";
            establishmentForLog = est?.name;

            var fourthHeader = FourthClient.GenerateFourthHeaderForSales(summedOrders, brand);
            var XmlDoc = FourthClient.ConvertToXMLDoc(fourthHeader).ConvertXMLDocToString();


            var Xmllog = new ScheduledTaskLog
            {
                Detail = "Brand:" + brand.name + " " + "No of items:" + summedOrders.Count(),
                FireTime = DateTime.Now,
                Result = 1,
                Message = "XML generated",
                Brand = brand.brand_id,
                BrandName = brand.name,
                Establishment = 0,
                EstablishmentName = establishmentForLog,
                TotalItemCount = summedOrders.Count(),
                TotalPounds = summedOrders.Sum(x => x.PURE_SALES_PLUS_TAX),
                LogType = "XML",
                ContainerEndDate = summedOrders.Max(x => x.CREATED_DATE),
                ContainerStartDate = summedOrders.Min(x => x.CREATED_DATE),
                User = "Automated",
                TotalItemQuantity = summedOrders.Sum(x => x.QUANTITY),
                TotalVAT = summedOrders.Sum(x => x.TAX),
                TotalItemVoidedCount = voided.Count(),
                TotalItemVoidedAmount = voided.Sum(x => x.discount_amount),
                TotalItemDiscountCount = discounted.Count(),
                TotalItemDiscountAmount = discounted.Sum(x => x.discount_amount),
                TotalItemDiscountTax = discounted.Sum(x => x.tax_amount),
                Notes = XmlDoc
            };
            db.ScheduledTaskLogs.Add(Xmllog);
            db.SaveChanges();
        }
    }




}
