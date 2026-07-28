using Newtonsoft.Json;
using Quartz;
using Revel._808nd.com.CaternetData;
using Revel._808nd.com.CaternetData.Models;
using Revel._808nd.com.CaternetFTPClient;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.WebserviceReader;
using Revel._808nd.com.Models;
using Revel._808nd.com.ProductMix;
using Revel._808nd.com.ProductMix.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Web.Grind._808nd.com.Controllers;

namespace scheduledtasks.grind._808nd.com.ScheduledTasks
{
    public class PushToCaternetJob : IJob
    {
        private string RevelAPIKEY { get; } = ConfigurationManager.AppSettings["RevelAPIKEY"];
        private string RevelBaseURL { get; } = ConfigurationManager.AppSettings["RevelBaseURL"];


        static string path = @"C:\test\";
        private static string fullXmlPath = path + String.Format("latestCaternetXML_{0}", DateTime.Now.ToString("dd_MM_yyyy"));
        private static string xmlSuffix = ".xml";

        void CreateXmlPath(DateTime syncStart)
        {
            fullXmlPath = path + String.Format("latestCaternetXML_{0}", syncStart.ToString("dd_MM_yyyy"));
        }

        public async void Execute(IJobExecutionContext context)
        {
            //todo: MAKE SURE YOU HAVE REFRESHED ITEMS FOT THAT DAY COMPLETELY 
            var startMarker = DateTime.Now.AddDays(-1);
            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday && DateTime.Now >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 00, 00))
            {
                startMarker = DateTime.Now; //this is for the sunday evening run
            }

            var syncStart = new DateTime(startMarker.Year, startMarker.Month, startMarker.Day, 03, 00, 00);
            var syncEnd = syncStart.AddDays(1);

            //var syncStart = new DateTime(2019, 10, 28, 03, 00, 00);
            //var syncEnd = syncStart.AddDays(1);


            CreateXmlPath(syncStart);

            var db = new GrindContext();
            try
            {
                await EstablishmentPushToCaternet(db, syncStart, syncEnd);

            }
            catch (Exception ex)
            {
                var log = new ScheduledTaskLog
                {
                    Detail = "The scheduler failed" + ex.Message + ex.InnerException,
                    FireTime = DateTime.Now,
                    Result = 0,
                    Message = "Error running Caternet overnight routine - please investigate.",
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

        private async Task EstablishmentPushToCaternet(GrindContext db, DateTime syncStart, DateTime syncEnd)
        {
            var VATRATE = 0.2M;

            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(600);

            var baseaddressSTring = @"https://shoreditchgrind.revelup.com/";
            client.BaseAddress = new Uri(baseaddressSTring);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Add("API-AUTHENTICATION", "408d6c05f2864ece90c037333d64f333:9ae943831e7f443b9edf3a6203e66598290fc7d2f3244ca9b69dd67404aa39f2");
            client.DefaultRequestHeaders.Add("Referer", baseaddressSTring);
            //get the orders for same periods - so we can filter         
            var emailer = new EmailController();

            var allEsts = db.Establishments.Where(x => x.establishment_id != 2)
                //.Where(x => x.establishment_id == 3) //GREENWICH
                .OrderBy(x => x.establishment_id).ToList();
            //allEsts = db.Establishments.Where(x => x.establishment_id != 1 && x.establishment_id != 2).ToList();

            RevelWebserviceDataReader dataReader = new RevelWebserviceDataReader(allEsts.First());

            try
            {

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            foreach (var est in allEsts)
            {
                try
                {
                    Thread.Sleep(60000);
                    est.BaseUri = new Uri(RevelBaseURL);
                    est.api_key = RevelAPIKEY;

                    Console.WriteLine("Now starting Establishment: " + est.name.ToString());
                    // emailer.SendMessageNadavIgnoreSendExeceptions(String.Format("Push to Caternet Job - now starting Establishment: " + est.name.ToString()));
                    //pull orderItems
                    var caternetXMLGenerationSErvice = "";


                    var query = ProductMixQueryFactory.GenerateDateRangeQueryWOwnTime(syncStart, syncEnd, est.establishment_id);
                    var result = await client.GetAsync(query);
                    var content = await result.Content.ReadAsStringAsync();

                    var res = JsonConvert.DeserializeObject<ProductMixRootObject>(content);


                    //NOW GENERATE THE XML - THESE ITEM QUANTITIES INCLUDE VOIDS AND COMPS
                    var allCaternetRowsIncCompsAndVoids = new List<CaternetCsvRow>();
                    var prodMix = res.productmix.Where(x => x.row_type == "Product" || x.row_type == "Modifier" || (x.row_type == "Parent_Product" && !String.IsNullOrWhiteSpace(x.product_sku))).ToList();

                    var prodMixWithModifiers = new List<ProductParentWithModifiers>();

                    //get modifiers
                    var modifiersQuery = @"/resources/Modifier/?format=json&active=1&limit=0";
                    dataReader.helperFactory = new RevelFactory(est);
                    var modContent = await dataReader.GetRevelWebserviceData<Modifier>(new Modifier(), modifiersQuery);
                    var mods = modContent;

                    //get modifierClasses
                    var modifierClassQuery = @"/resources/ModifierClass/?format=json&active=1&limit=0";
                    var modClasses = await dataReader.GetRevelWebserviceData<ModifierClass>(new ModifierClass(), modifierClassQuery);

                    ////get modifierItems
                    //var modifierItemsQuery = @"/resources/ModifierItem/?format=json&active=1&limit=0";
                    //var modItems = await dataReader.GetRevelWebserviceData<ModifierClass>(new ModifierClass(), modifierClassQuery);

                    //test code
                    //get almond milks from both

                    var milkMix = prodMix.Where(x => x.product_sku == "91005").ToList();
                    var milkMod = mods.Where(x => x.sku == "91005").ToList();





                    List<ProductSkuAndName> skuAndPrice = new List<ProductSkuAndName>();
                    prodMix.ForEach(x =>
                    {
                        //test 
                        if (x.product_sku == "91005")
                        {
                            var stop = "";
                        }


                        if (x.row_type != "Modifier")
                        {

                            var prodInDb = db.Products.FirstOrDefault(X => X.sku.Trim() == x.product_sku.Trim());
                            if (prodInDb != null)
                                skuAndPrice.Add(new ProductSkuAndName
                                {
                                    Name = prodInDb.name,
                                    Price = prodInDb.price,
                                    Sku = prodInDb.sku
                                });
                        }
                        else
                        {
                            var items = mods?.Where(X => x.product_sku == X.sku.Trim() && X.establishment == est.resource_uri).ToList();
                            var item = items.FirstOrDefault();

                            if (item != null)
                                skuAndPrice.Add(new ProductSkuAndName
                                {
                                    Name = item.name,
                                    Price = item.price,
                                    Sku = item.sku
                                });
                        }

                        //ProductParentWithModifiers mostRecent = new ProductParentWithModifiers();
                        //if (x.row_type != "Modifier")
                        //{
                        //    //add standard prod
                        //    var newToAdd = new ProductParentWithModifiers { MainProduct = x };
                        //    mostRecent = newToAdd;
                        //    mostRecent.MainProductName = x.product_name;
                        //    prodMixWithModifiers.Add(newToAdd);
                        //}
                        //else
                        //{
                        //    mostRecent.Modifiers.Add(x);
                        //}

                    });

                    prodMix.ForEach(x =>
                    {
                        try
                        {
                            //test
                            if (x.product_name == "Corona*")
                            {
                                var stopnow = "";
                            }


                            var prodInDb = skuAndPrice.FirstOrDefault(y => y.Sku.Trim() == x.product_sku.Trim());

                            var comps = Convert.ToInt32(Convert.ToDecimal(x.n_comps));
                            var voids = Convert.ToInt32(Convert.ToDecimal(x.n_voids));
                            var qty = Convert.ToInt32(Convert.ToDecimal(x.n_items));
                            var totalQty = qty + comps - voids;

                            var netSales = 0.00M;
                            var grossSales = 0.00M;

                            var totalInDisc = Convert.ToDecimal(x.gm.CoalesceDecimal());
                            var tax = Convert.ToDecimal(x.tax.CoalesceDecimal());

                            var name = x.product_name;
                            var pClass = x.product_class;
                            var sku = x.product_sku;
                            var price = prodInDb?.Price ?? 0;

                            //modifier specific 
                            if (x.row_type == "Modifier")
                            {
                                var modDisc = String.IsNullOrWhiteSpace(x.discount) ? 0.00M : Convert.ToDecimal(x.discount);
                                var modDisc2 = String.IsNullOrWhiteSpace(x.order_discount) ? 0.00M : Convert.ToDecimal(x.order_discount);

                                //net sales
                                netSales = Convert.ToDecimal(x.taxable_sales) + Convert.ToDecimal(x.untaxable_sales) - modDisc - modDisc2;

                                //gross
                                tax = Convert.ToDecimal(x.taxable_sales) * VATRATE;
                                grossSales = Decimal.Round(netSales + tax, 2);
                            }
                            else
                            {
                                netSales = totalInDisc;
                                grossSales = netSales + tax;
                            }

                            var row = new CaternetCsvRow
                            {
                                Quantity = totalQty,
                                GrossSalesPrice = price,
                                SKU = sku,
                                VAT = Convert.ToDecimal(tax),
                                Name = name,
                                NetSales = netSales,
                                GrossSales = Convert.ToDecimal(grossSales),
                                SalesTypeRef = 0
                            };

                            allCaternetRowsIncCompsAndVoids.Add(row);
                        }
                        catch (Exception ex)
                        {
                            var msg = "product: " + x.product_name + " failed";
                            throw;
                        }

                    });
                    //END

                    //SUM ANY ROWS WITH THE SAME SKU
                    var summedFinalRows = new List<CaternetCsvRow>();
                    var skuAlreadyProcessed = new List<string>();

                    var rowsWithoutSku = allCaternetRowsIncCompsAndVoids.Where(x => String.IsNullOrWhiteSpace(x.SKU)).ToList();

                    var rowsAllHaveSkus = allCaternetRowsIncCompsAndVoids.Except(rowsWithoutSku).ToList();
                    rowsAllHaveSkus.ForEach(x =>
                    {
                        if (!skuAlreadyProcessed.Exists(y => String.Equals(y, x.SKU)))
                        {
                            var sameSku = rowsAllHaveSkus.Where(y => y.SKU == x.SKU).ToList();
                            if (sameSku.Count() > 1)
                            {
                                var combinedRow = new CaternetCsvRow
                                {
                                    GrossSales = sameSku.Sum(y => y.GrossSales),
                                    NetSales = sameSku.Sum(y => y.NetSales),
                                    Quantity = sameSku.Sum(y => y.Quantity),
                                    SalesTypeRef = sameSku.First().SalesTypeRef,
                                    Name = sameSku.First().Name,
                                    SKU = sameSku.First().SKU,
                                    VAT = sameSku.Sum(y => y.VAT),
                                    GrossSalesPrice = sameSku.First().GrossSalesPrice,
                                    NetSalesPrice = sameSku.First().NetSalesPrice
                                };
                                summedFinalRows.Add(combinedRow);
                            }
                            else
                            {
                                summedFinalRows.Add(x);
                            }
                            skuAlreadyProcessed.Add(x.SKU);

                        }


                    });
                    //END SUM


                    //GENERATE XML
                    var factory = new CaternetXMLFactory();

                    if (summedFinalRows.Count > 0)
                    {
                        var unqiteXMLpath = fullXmlPath + "_" + est.establishment_id.ToString() + xmlSuffix;
                        factory.CreateXML(unqiteXMLpath, syncStart, est.id, est.id, summedFinalRows);

                        //ftp and upload to directory
                        var ftpClient = new CaternetFTPClient();
                        ftpClient.Upload("ftp.zupacaternet.com", "caternetexport@grind.co.uk", "JX7p5d0rhS0i", unqiteXMLpath, est.establishment_id.ToString());
                        Console.WriteLine("Completed");

                        //EMAIL ERRORS
                        if (rowsWithoutSku.Count() > 0)
                        {

                            var errorString = "";
                            rowsWithoutSku.ForEach(x =>
                            {
                                errorString += String.Format("{0}<br/>", x.SKU);
                            });

                            emailer.SendMessage("marcus@grind.co.uk", "CATERNET - Items without SKUs - for establishment: " + est.name, errorString);
                        }



                        //FINISHED - LOG
                        //LOG
                        var fileString = File.ReadAllText(unqiteXMLpath);
                        var successlog = new ScheduledTaskLog
                        {
                            Detail = fileString,
                            FireTime = DateTime.Now,
                            Result = 1,
                            Message = "CATERNET SUCCESS - Items send successfully for: " + est.name,
                            Brand = est.establishment_id,
                            BrandName = est.name,
                            Establishment = est.establishment_id,
                            EstablishmentName = "",
                            TotalItemCount = allCaternetRowsIncCompsAndVoids.Count(),
                            TotalPounds = Decimal.Round(allCaternetRowsIncCompsAndVoids.Sum(x => x.GrossSales), 2),
                            LogType = "LOCAL",
                            ContainerEndDate = syncStart,
                            ContainerStartDate = syncEnd,
                            User = "Automated",
                            TotalItemQuantity = allCaternetRowsIncCompsAndVoids.Sum(x => x.Quantity),
                            TotalVAT = allCaternetRowsIncCompsAndVoids.Sum(x => x.VAT),
                            //TotalItemVoidedCount = allCaternetRowsIncCompsAndVoids.Where(x => x > 0).Count(),
                            //TotalItemVoidedAmount = allCaternetRowsIncCompsAndVoids.Sum(x => x.VoidOrComp),
                            //TotalItemDiscountCount = discounted.Count(),
                            //TotalItemDiscountAmount = discounted.Sum(x => x.discount_amount),
                            //TotalItemDiscountTax = discounted.Sum(x => x.tax_amount),
                        };

                        db.ScheduledTaskLogs.Add(successlog);
                        db.SaveChanges();
                        emailer.SendMessage("marcus@grind.co.uk", String.Format("CATERNET SUCCESS - for establishment: " + est.name + " " + successlog.TotalItemCount + " items and " + successlog.TotalPounds + " pounds on Date " + syncStart.ToShortDateString()));
                        //emailer.SendMessageNadavIgnoreSendExeceptions(String.Format("CATERNET SUUCESS - for establishment: " + currentEst.name + " " + successlog.TotalItemCount + " items and " + successlog.TotalPounds + " pounds on Date" + syncStart.ToShortDateString()));
                    }



                }
                catch (Exception ex)
                {
                    emailer.SendMessageNadavIgnoreSendExeceptions(String.Format("CATERNET ERROR - Exception occurred" + ex.Message + " " + ex.InnerException));
                    Console.WriteLine("Exception: " + ex.Message);
                }
            }
        }
    }
}
