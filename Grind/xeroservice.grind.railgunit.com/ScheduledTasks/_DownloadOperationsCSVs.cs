using AutoIt;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Quartz;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.Utility;
using Revel._808nd.com.Models;
using Revel._808nd.com.OperationsReport.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using xero.railgunit.com.Grind;
using xero.railgunit.com.Grind.Utility;
using xero.railgunit.com.Taxes;
using Xero.Api.Core;
using Xero.Api.Core.Model;
using Xero.Api.Example.Applications.Private;
using Xero.Api.Infrastructure.OAuth;
using Xero.Api.Serialization;

namespace xeroservice.grind.railgunit.com.ScheduledTasks
{
    public class _PushAccountsToXero : BaseJob
    {
        DateTime start;
        DateTime end;
        HttpClient client;
        RevelClassTaxMappingService taxMappingService;
        RevelProductClassAccountMappingService accountMappingService;

        List<EstablishmentXeroMapping> grinds;

        protected string GrindAndCoBaseURL = "https://api.xero.com/api.xro/2.0/";
        protected string GrindAndCoConsumerKey = "9C219QS0VADCCFZT9T6NMNLBLHEAGA";
        protected string GrindAndCoConsumerSecret = "9TRZOJMAIAIPW28BMQXUEL6T6ROY5D";
        protected string GrindAndCoPathToCert = "";

        protected XeroCoreApi GrindSUT;
        protected GrindContext db = new GrindContext("GrindLiveContext");

        //protected string ExBaseUrl = "https://api.xero.com/api.xro/2.0/";
        //protected string ExConsumerKey = "EBAL8VMLJODEF9LJF3VULOOXIKBUZ6";
        //protected string ExConsumerSecret = "GTL6TMZHOJW8E1FNVTTET8BY518DBU";
        //protected string ExpathToCert = "";
        //protected XeroCoreApi ExSUT;

        public override async Task Execute(IJobExecutionContext context)
        {

#if DEBUG
            if (true)
#else
            if (DateTime.Now.DayOfWeek == DayOfWeek.Monday) //ROCK AND ROLL ON MONDAY
#endif
            {

                var currentWorkingDirectory = System.AppDomain.CurrentDomain.BaseDirectory;


                GrindAndCoPathToCert = @"C:\XeroServiceCerts\GrindAndCo\public_privatekey.pfx";
                //ExpathToCert = @"C:\XeroServiceCerts\Exmouth\public_privatekey.pfx";


                //setup
                // Private Application Sample
                X509Certificate2 GrindAndCocert = new X509Certificate2(GrindAndCoPathToCert, "1");
                GrindSUT = new XeroCoreApi(GrindAndCoBaseURL, new PrivateAuthenticator(GrindAndCocert),
                            new Consumer(GrindAndCoConsumerKey, GrindAndCoConsumerSecret), null,
                            new DefaultMapper(), new DefaultMapper());

                var user = new ApiUser { Name = Environment.MachineName };

                // Private Application Sample
                //X509Certificate2 ExCert = new X509Certificate2(ExpathToCert, "");
                //ExSUT = new XeroCoreApi(ExBaseUrl, new PrivateAuthenticator(ExCert),
                //            new Consumer(ExConsumerKey, ExConsumerSecret), null,
                //            new DefaultMapper(), new DefaultMapper());



                ////init
                DateTime dt = DateTime.Now.AddDays(-2); //go back to Sat
                                                        //get monday
                dt = dt.StartOfWeek(DayOfWeek.Monday);

                start = new DateTime(dt.Year, dt.Month, dt.Day, 04, 00, 00);
                end = start.AddDays(7);
                var dueDate = DateTime.Now.AddDays(-1); //this should be Sunday - runs on Monday


                //test for Exmouth
                //var dt = DateTime.Now;
                //start = new DateTime(dt.Year, dt.Month, dt.Day, 04, 00, 00);
                ////var start = new DateTime(2018, 08, 13, 03, 00, 00);
                //var end = start.AddDays(7);

                //var dueDate = end.AddDays(-1); //this should be Sunday - runs on 
                //end test


                var RevelAPIKEY = ConfigurationManager.AppSettings["RevelAPIKEY"];
                var RevelBaseURL = ConfigurationManager.AppSettings["RevelBaseURL"];

                Establishment revOrg = new Establishment(1, "Grind",
                   RevelAPIKEY,
                   new Uri(RevelBaseURL));


                RevelFactory helperFactory = new RevelFactory(revOrg);
                client = helperFactory.CreateShoreditchGrindHttpClient(RevelBaseURL, RevelAPIKEY);

                taxMappingService = new RevelClassTaxMappingService();
                accountMappingService = new RevelProductClassAccountMappingService();
                grinds = OtherExtensions.GetAllGrindStoresForReportDownload().ToList();


                try
                {

                    //setup containers
                    List<XeroCompanyContainer> topLevelContainers = new List<XeroCompanyContainer>();
                    XeroCompanyContainer grindContainer = new XeroCompanyContainer
                    {//setup GrindAndCo container
                        ConsumerKey = GrindAndCoConsumerKey,
                        ConsumerSecret = GrindAndCoConsumerSecret,
                        PathToCert = GrindAndCoPathToCert,
                        EstablishmentMappings = new List<EstablishmentXeroMapping>
                {
                new EstablishmentXeroMapping { EstablishmentId = "1", XeroContactName = "Shoreditch Grind Sales" },
                new EstablishmentXeroMapping { EstablishmentId = "3", XeroContactName = "Soho Grind Sales" },
                new EstablishmentXeroMapping { EstablishmentId = "4", XeroContactName = "London Grind Sales" },
                new EstablishmentXeroMapping { EstablishmentId = "5", XeroContactName = "Hatton Garden Grind Sales" },
                new EstablishmentXeroMapping { EstablishmentId = "6", XeroContactName = "Royal Exchange Sales" },
                new EstablishmentXeroMapping { EstablishmentId = "7", XeroContactName = "Covent Garden Grind Sales" },
                new EstablishmentXeroMapping { EstablishmentId = "8", XeroContactName = "Clerkenwell Grind Sales" },
                new EstablishmentXeroMapping { EstablishmentId = "9", XeroContactName = "Whitechapel Grind Sales" },
                 new EstablishmentXeroMapping { EstablishmentId = "10", XeroContactName = "Exmouth Market Grind Sales" },
                      new EstablishmentXeroMapping { EstablishmentId = "11", XeroContactName = "Facebook Grind Sales" },
                }

                    };

                    //    XeroCompanyContainer exmouthMarketContainer = new XeroCompanyContainer
                    //    {
                    //        ConsumerKey = ExConsumerKey,
                    //        ConsumerSecret = ExConsumerSecret,
                    //        PathToCert = ExpathToCert,
                    //        EstablishmentMappings = new List<EstablishmentXeroMapping>
                    //{
                    //      new EstablishmentXeroMapping { EstablishmentId = "10", XeroContactName = "Exmouth Market Grind Sales" },
                    //}
                    //    };

                    topLevelContainers.Add(grindContainer);
                    //topLevelContainers.Add(exmouthMarketContainer);


                    //tracking
                    Console.WriteLine("Start all item tracking from Xero");
                    var storeLocationTrackings = GrindSUT.TrackingCategories.Find().ToList().First();

                    //  var = storeLocationTrackings.Option = 
                    //end tracking


                    //GENERATE CONTACTS FOR BOTH COMPANIES
                    //setup contact
                    Console.WriteLine("start all contact from Xero");
                    List<Contact> Contacts = new List<Contact>();
                    List<Contact> Contactp = new List<Contact>();
                    int i = 1;
                    do
                    {
                        Contactp = GrindSUT.Contacts.Page(i).Find().ToList();
                        Contacts.AddRange(Contactp);
                        i++;
                    } while (Contactp.Count() > 0);
                    //end contacts



                    Console.WriteLine("Got all contact from Xero");



                    InvoiceLineItemLocationMapping locationMappingService = new InvoiceLineItemLocationMapping(storeLocationTrackings);

                    foreach (var company in topLevelContainers)
                    {

                        //BEGIN INVOICE CREATION
                        foreach (var establishment in company.EstablishmentMappings.ToList())
                        {
                            Console.WriteLine("now processing " + establishment.XeroContactName);
                            //FIND WHICH COMPANY SERVICE WE NEED TO USE
                            Contact currentGrind;
                            XeroCoreApi currentSUT;


                            //ALL  GRINDS
                            currentSUT = GrindSUT;
                            currentGrind = Contacts.FirstOrDefault(x => x.Name == establishment.XeroContactName);



                            //setup invoice
                            var invoice = new Invoice
                            {
                                Contact = currentGrind,
                                Type = Xero.Api.Core.Model.Types.InvoiceType.AccountsReceivable,
                                Status = Xero.Api.Core.Model.Status.InvoiceStatus.Draft,
                                LineAmountTypes = Xero.Api.Core.Model.Types.LineAmountType.Exclusive,
                                Date = dueDate, //THIS NEEDS TO BE THE SUNDAY
                                DueDate = dueDate,
                                Reference = "Revel Sales WE " + dueDate.ToString("dd.MM.yyyy")
                            };

                            invoice.LineItems = new List<LineItem>();

                            //assign xero classes to each product - for tax
                            var allTaxCodes = XeroTaxCodeHelper.GetTaxCodes();

                            //get data from Revel API

                            //var query = String.Format("https://shoreditchgrind.revelup.com/reports/operations/json/?employee=&online_app=&online_app_type=&online_app_platform=&show_opened=1&show_unpaid=1&show_irregular=1&range_from={0}%2F{1}%2F{2}+04%3A00&range_to={3}%2F{4}%2F{5}+04%3A00&establishment={6}", start.Day.ToString("00"), start.Month.ToString("00"), start.Year, end.Day.ToString("00"), end.Month.ToString("00"), end.Year, establishment.EstablishmentId);

                            var query = String.Format("https://shoreditchgrind.revelup.com/reports/operations/json/?employee=&online_app=&online_app_type=&online_app_platform=&show_opened=1&show_unpaid=1&show_irregular=1&range_from={0}&range_to={1}&establishment={2}", start.ToRevelDate(), end.ToRevelDate(), establishment.EstablishmentId);

                            var response = await client.GetAsync(query);
                            var content = await response.Content.ReadAsStringAsync();
                            var poco = JsonConvert.DeserializeObject<RootObject>(content);



                            //get top level groups
                            var container = poco.CreateOperationsReportGroup();


                            //get container amoutns

                            //sales
                            var taxableSales = container.GetTotalTaxableSales();
                            var untaxableSales = container.GetTotalUnTaxableSales();
                            var tax = container.GetTotalUnTaxableSales();

                            //discounts
                            var getTotalItemDiscountSales = container.GetTotalItemDiscountSales();
                            var getTotalOrderiscountSales = container.GetTotalOrderDiscountSales();

                            //
                            var voids = container.GetTotalVoidSales();



                            //create discount line item
                            var discountItemSum = container.XeroOperationsProducClassGroups.Sum(x => x.ProductMix.discount);
                            var discountOrderSum = container.XeroOperationsProducClassGroups.Sum(x => x.ProductMix.order_discount);
                            var totalDiscounts = (discountItemSum + discountOrderSum) * -1; //NEEDS TO BE NEGATIVE NUMBER

                            var totalSalesMinusDiscounts = container.GetTotalGrossSales() + totalDiscounts;

                            Console.WriteLine("Discounts: " + totalDiscounts);

                            invoice.LineItems.Add(new LineItem
                            {
                                Quantity = 1,
                                AccountCode = accountMappingService.GetRevelAccountCodeForCategory("discounts").AccountCode,
                                TaxType = allTaxCodes.Where(x => x.XeroValue == "INPUT2").First().XeroValue,
                                LineAmount = totalDiscounts,
                                Description = establishment.XeroContactName + " - Revel - Discounts",
                                UnitAmount = totalDiscounts

                            });
                            //end discounts

                            //BUNDLE JUICE WITH BAR
                            //var juiceCategory = container.XeroOperationsProducClassGroups.FirstOrDefault(x => x.ParentCategoryName.ToLower() == "juice");

                            container.XeroOperationsProducClassGroups.ToList().ForEach(x => //.Where(x => x.ParentCategoryName.ToLower() != "juice").ToList().ForEach(x =>
                            {
                                var taxedSales = x.GetTotalTaxedSales();
                                var taxAmount = x.GetTotalTaxAmount();
                                var nonTaxedSales = x.GetTotalNonTaxedSales();


                                var totalGrossSales = x.GetTotalGrossSales();
                                var orderDisc = x.GetOrderDiscounts();
                                var itemDisc = x.GetItemDiscounts();
                                var applyDisc = totalGrossSales - orderDisc - itemDisc;

                                //if (x.ParentCategoryName.ToLower().Equals("bar"))
                                //{
                                //    taxedSales += juiceCategory.GetTotalTaxedSales();
                                //    taxAmount += juiceCategory.GetTotalTaxAmount();
                                //    nonTaxedSales += juiceCategory.GetTotalNonTaxedSales();
                                //    //add juice to bar category

                                //}


                                if (taxedSales > 0)
                                {
                                    var taxedSalesLine = new LineItem
                                    {
                                        Quantity = 1,
                                        AccountCode = accountMappingService.GetRevelAccountCodeForCategory(x.ParentCategoryName).AccountCode,
                                        TaxType = allTaxCodes.Where(y => y.XeroValue == "OUTPUT2").First().XeroValue,
                                        LineAmount = taxedSales,
                                        Description = establishment.XeroContactName + " - Revel - " + x.ParentCategoryName,
                                        UnitAmount = taxedSales
                                    };
                                    taxedSalesLine.Tracking = new ItemTracking();
                                    taxedSalesLine.Tracking.Add(locationMappingService.GetRevelLocationCodeForCategory(establishment.EstablishmentId));
                                    invoice.LineItems.Add(taxedSalesLine);
                                }
                                Console.WriteLine("Taxed sales: " + taxedSales);

                                //ADD NON TAXED SALES
                                if (nonTaxedSales > 0)
                                {
                                    var nonTaxedSalesLine = new LineItem
                                    {
                                        Quantity = 1,
                                        AccountCode = accountMappingService.GetRevelAccountCodeForCategory(x.ParentCategoryName).AccountCode,
                                        TaxType = allTaxCodes.Where(y => y.XeroValue == "ZERORATEDOUTPUT").First().XeroValue,
                                        LineAmount = nonTaxedSales,
                                        Description = establishment.XeroContactName + " - Revel - " + x.ParentCategoryName,
                                        UnitAmount = nonTaxedSales
                                    };
                                    nonTaxedSalesLine.Tracking = new ItemTracking();
                                    nonTaxedSalesLine.Tracking.Add(locationMappingService.GetRevelLocationCodeForCategory(establishment.EstablishmentId));
                                    invoice.LineItems.Add(nonTaxedSalesLine);
                                }
                                Console.WriteLine("UnTaxed sales: " + nonTaxedSales);
                            });

                            //ADD TIPS AS LINE ITEM - UNTAXED
                            var tips = Convert.ToDecimal(establishment.XeroContactName.Contains("Soho") ? container.GetTips() : container.GetServiceFee());
                            if (tips > 0)
                            {
                                var discountLineItem = new LineItem
                                {
                                    Quantity = 1,
                                    AccountCode = accountMappingService.GetRevelAccountCodeForCategory("tips").AccountCode,
                                    TaxType = allTaxCodes.Where(y => y.XeroValue == "NONE").First().XeroValue,
                                    LineAmount = tips,
                                    Description = establishment.XeroContactName + " - Revel - " + "Tips and Service Charge",
                                    UnitAmount = tips
                                };

                                discountLineItem.Tracking = new ItemTracking();
                                discountLineItem.Tracking.Add(locationMappingService.GetRevelLocationCodeForCategory(establishment.EstablishmentId));
                                invoice.LineItems.Add(discountLineItem);

                            }
                            Console.WriteLine("Tips: " + tips);

                            //ADD GIFT STORE SALES AS LINE ITEM
                            var storeCredit = Convert.ToDecimal(establishment.XeroContactName.Contains("Soho") ? container.GetGiftAndServicePayable() : container.GetGiftAndServicePayable());
                            if (storeCredit > 0)
                            {
                                var storeCreditLineItem = new LineItem
                                {
                                    Quantity = 1,
                                    AccountCode = accountMappingService.GetRevelAccountCodeForCategory("gift").AccountCode,
                                    TaxType = allTaxCodes.Where(y => y.XeroValue == "ZERORATEDOUTPUT").First().XeroValue,
                                    LineAmount = storeCredit,
                                    Description = establishment.XeroContactName + " - Revel - " + "Gift Card and Store Credit",
                                    UnitAmount = storeCredit
                                };


                                storeCreditLineItem.Tracking = new ItemTracking();
                                storeCreditLineItem.Tracking.Add(locationMappingService.GetRevelLocationCodeForCategory(establishment.EstablishmentId));
                                invoice.LineItems.Add(storeCreditLineItem);
                            }
                            Console.WriteLine("Store cRedit: " + storeCredit);
                            //we're done
                            try
                            {

                               var result = currentSUT.Invoices.Create(invoice);
                                Console.WriteLine("Created in Xero!");

                                base.SendEmailNotification("GRIND XERO", "invoice for " + establishment.XeroContactName + " create correctly!");
                            }
                            catch (Exception ex)
                            {

                                throw;
                            }
                        }

                    }



                }
                catch (Exception ex)
                {
                    base.SendEmailNotification("DANGER! - GRIND EXCEPTION", ex.Message);
                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "Application";
                        eventLog.WriteEntry("Xero Service has exceptioned" + ex.Message, EventLogEntryType.Information, 666, 1);
                    }

                }


            }
        }
    }
}
