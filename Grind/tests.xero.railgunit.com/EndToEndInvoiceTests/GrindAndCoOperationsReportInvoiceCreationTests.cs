using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xero.railgunit.com.Grind;
using xero.railgunit.com.Grind.Extension;
using xero.railgunit.com.Taxes;
using Xero.Api.Core.Model;
using Shouldly;
using xero.railgunit.com.Grind.Utility;
using System.IO;
using xero.railgunit.com.Grind.Utility;
using Revel._808nd.com.OperationsReport.Models;
using Xero.Api.Core;

namespace tests.xero.railgunit.com.EndToEndInvoiceTests
{
    [TestFixture]
    public class GrindAndCoOperationsReportInvoiceCreationTests : BaseEndToEndGrindAndCoInvoiceTests
    {

        RevelClassTaxMappingService taxMappingService;
        RevelProductClassAccountMappingService accountMappingService;

        List<EstablishmentXeroMapping> grinds;
        string directoryPath = @"C:\ReveLCSVs\";
        string browserDownloadPath = @"C:\Users\n\Downloads\";


        [SetUp]
        public async Task Arrange()
        {

            taxMappingService = new RevelClassTaxMappingService();
            accountMappingService = new RevelProductClassAccountMappingService();
            grinds = OtherExtensions.GetAllGrindStoresForReportDownload().ToList();

            await base.Arrange();
        }


        public async Task Act()
        {


        }


        [Test]
        public void Should_Create_A_New_Invoice_For_All_Relevant_Grinds_Using_Opertions_report()
        {
            //setup containers
            List<XeroCompanyContainer> topLevelContainers = new List<XeroCompanyContainer>();
            XeroCompanyContainer grindContainer = new XeroCompanyContainer
            {//setup GrindAndCo container
                ConsumerKey = "9C219QS0VADCCFZT9T6NMNLBLHEAGA",
                ConsumerSecret = "9TRZOJMAIAIPW28BMQXUEL6T6ROY5D",
                PathToCert = @"C:\GIT2016\Grind\tests.xero.railgunit.com\Certificates\GrindAndCo\public_privatekey.pfx",
                EstablishmentMappings = new List<EstablishmentXeroMapping>
                {
                      new EstablishmentXeroMapping { EstablishmentId = "1", XeroContactName = "Shoreditch Grind Sales" },
                new EstablishmentXeroMapping { EstablishmentId = "3", XeroContactName = "Soho Grind Sales" },
                new EstablishmentXeroMapping { EstablishmentId = "4", XeroContactName = "London Grind Sales" },
                new EstablishmentXeroMapping { EstablishmentId = "5", XeroContactName = "Holborn Grind Sales" },
                new EstablishmentXeroMapping { EstablishmentId = "6", XeroContactName = "Royal Exchange Sales" },
                new EstablishmentXeroMapping { EstablishmentId = "7", XeroContactName = "Covent Garden Grind Sales" },
                new EstablishmentXeroMapping { EstablishmentId = "8", XeroContactName = "Clerkenwell Grind Sales" },
                new EstablishmentXeroMapping { EstablishmentId = "9", XeroContactName = "Whitechapel Grind Sales" }
                }

            };

            XeroCompanyContainer exmouthMarketContainer = new XeroCompanyContainer
            {
                ConsumerKey = "EBAL8VMLJODEF9LJF3VULOOXIKBUZ6",
                ConsumerSecret = "GTL6TMZHOJW8E1FNVTTET8BY518DBU",
                PathToCert = @"C:\GIT2016\Grind\tests.xero.railgunit.com\Certificates\Exmouth\public_privatekey.pfx",
                EstablishmentMappings = new List<EstablishmentXeroMapping>
                {
                      new EstablishmentXeroMapping { EstablishmentId = "10", XeroContactName = "Exmouth Market Grind Sales" },
                }
            };

            topLevelContainers.Add(grindContainer);
            topLevelContainers.Add(exmouthMarketContainer);

            //GENERATE CONTACTS FOR BOTH COMPANIES
            //setup contact
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



            //GENERATE EXMOUTH CONTACTS
            List<Contact> ExContacts = new List<Contact>();
            List<Contact> ExContactp = new List<Contact>();
            int i1 = 1;
            do
            {
                ExContactp = ExSUT.Contacts.Page(i1).Find().ToList();
                ExContacts.AddRange(ExContactp);
                i1++;
            } while (ExContactp.Count() > 0);
            //END



            foreach (var company in topLevelContainers)
            {

                //BEGIN INVOICE CREATION
                foreach (var establishment in company.EstablishmentMappings)
                {

                    //FIND WHICH COMPANY SERVICE WE NEED TO USE
                    Contact currentGrind;
                    XeroCoreApi currentSUT;
                    if (establishment.EstablishmentId == "10") //EXMOUTH
                    {
                        currentSUT = ExSUT;
                        currentGrind = ExContacts.FirstOrDefault(x => x.Name == establishment.XeroContactName);

                    }
                    else
                    { //ALL OTHER GRINDS
                        currentSUT = GrindSUT;
                        currentGrind = Contacts.FirstOrDefault(x => x.Name == establishment.XeroContactName);
                    }

                    currentGrind.ShouldNotBeNull();
                    //setup invoice
                    var invoice = new Invoice
                    {
                        Contact = currentGrind,
                        Type = Xero.Api.Core.Model.Types.InvoiceType.AccountsReceivable,
                        Status = Xero.Api.Core.Model.Status.InvoiceStatus.Draft,
                        LineAmountTypes = Xero.Api.Core.Model.Types.LineAmountType.Exclusive,
                        Date = DateTime.Now,
                        DueDate = DateTime.Now,
                        Reference = "Revel Sales XERO TEST " + DateTime.Now.ToString("ddmmyy")
                    };

                    invoice.LineItems = new List<LineItem>();

                    //assign xero classes to each product - for tax
                    var allTaxCodes = XeroTaxCodeHelper.GetTaxCodes();


                    //DATA
                    //get correct data for each class                    
                    var fullGrindName = OtherExtensions.GetGrindName(grinds.First(x => x.EstablishmentId == establishment.EstablishmentId).XeroContactName);
                    var fullPathandFileName = Path.Combine(directoryPath, fullGrindName);
                    var fileString = System.IO.File.ReadAllText(fullPathandFileName);
                    var poco = JsonConvert.DeserializeObject<RootObject>(fileString);

                    //get top level groups
                    var container = poco.CreateOperationsReportGroup();


                    //create discount line item
                    var discountItemSum = container.XeroOperationsProducClassGroups.Sum(x => x.ProductMix.discount);
                    var discountOrderSum = container.XeroOperationsProducClassGroups.Sum(x => x.ProductMix.order_discount);
                    var totalDiscounts = discountItemSum + discountOrderSum;

                    invoice.LineItems.Add(new LineItem
                    {
                        Quantity = 1,
                        AccountCode = accountMappingService.GetRevelAccountCodeForCategory("discounts").AccountCode,
                        TaxType = allTaxCodes.Where(x => x.XeroValue == "INPUT2").First().XeroValue,
                        LineAmount = totalDiscounts,
                        Description = "Revel - Discounts",
                        UnitAmount = totalDiscounts

                    });
                    //end discounts

                    //do all items
                    container.XeroOperationsProducClassGroups.ToList().ForEach(x =>
                    {
                        var taxedSales = x.GetTotalTaxedSales();
                        var taxAmount = x.GetTotalTaxAmount();
                        var nonTaxedSales = x.GetTotalNonTaxedSales();

                        //ADD TAXED SALES
                        if (taxedSales > 0)
                        {
                            invoice.LineItems.Add(new LineItem
                            {
                                Quantity = 1,
                                AccountCode = accountMappingService.GetRevelAccountCodeForCategory(x.ParentCategoryName).AccountCode,
                                TaxType = allTaxCodes.Where(y => y.XeroValue == "OUTPUT2").First().XeroValue,
                                LineAmount = taxedSales,
                                Description = "Revel - " + x.ParentCategoryName,
                                UnitAmount = taxedSales
                            });
                        }
                        //ADD NON TAXED SALES
                        if (nonTaxedSales > 0)
                        {
                            invoice.LineItems.Add(new LineItem
                            {
                                Quantity = 1,
                                AccountCode = accountMappingService.GetRevelAccountCodeForCategory(x.ParentCategoryName).AccountCode,
                                TaxType = allTaxCodes.Where(y => y.XeroValue == "ZERORATEDOUTPUT").First().XeroValue,
                                LineAmount = nonTaxedSales,
                                Description = "Revel - " + x.ParentCategoryName,
                                UnitAmount = nonTaxedSales
                            });
                        }
                    });

                    //ADD TIPS AS LINE ITEM - UNTAXED
                    var tips = Convert.ToDecimal(fullGrindName.Contains("Soho") ? container.GetTips() : container.GetServiceFee());
                    if (tips > 0)
                    {
                        invoice.LineItems.Add(new LineItem
                        {
                            Quantity = 1,
                            AccountCode = accountMappingService.GetRevelAccountCodeForCategory("tips").AccountCode,
                            TaxType = allTaxCodes.Where(y => y.XeroValue == "ZERORATEDOUTPUT").First().XeroValue,
                            LineAmount = tips,
                            Description = "Revel - " + "Tips and Service Charge",
                            UnitAmount = tips
                        });

                    }
                    //ADD GIFT STORE SALES AS LINE ITEM
                    var storeCredit = Convert.ToDecimal(fullGrindName.Contains("Soho") ? container.GetGiftAndServicePayable() : container.GetGiftAndServicePayable());
                    if (storeCredit > 0)
                    {
                        invoice.LineItems.Add(new LineItem
                        {
                            Quantity = 1,
                            AccountCode = accountMappingService.GetRevelAccountCodeForCategory("gift").AccountCode,
                            TaxType = allTaxCodes.Where(y => y.XeroValue == "ZERORATEDOUTPUT").First().XeroValue,
                            LineAmount = storeCredit,
                            Description = "Revel - " + "Gift Card and Store Credit",
                            UnitAmount = storeCredit
                        });
                    }

                    //we're done
                    try
                    {
                        var result = currentSUT.Invoices.Create(invoice);
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }

            }

        }
    }
}


