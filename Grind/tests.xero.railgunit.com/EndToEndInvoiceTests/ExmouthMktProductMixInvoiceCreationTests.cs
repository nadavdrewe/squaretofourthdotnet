using Newtonsoft.Json;
using NUnit.Framework;
using Revel._808nd.com.ProductMix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using xero.railgunit.com.Grind;
using xero.railgunit.com.Grind.Extension;
using xero.railgunit.com.Taxes;
using Xero.Api.Core;
using Xero.Api.Core.Model;
using Xero.Api.Example.Applications.Private;
using Xero.Api.Infrastructure.OAuth;
using Xero.Api.Serialization;

namespace tests.xero.railgunit.com.EndToEndInvoiceTests
{
    public class ExmouthMktProductMixInvoiceCreationTests : BaseEndToEndExmouthMktTests
    {
        string JSON = "https://shoreditchgrind.revelup.com/brand/reports/product_mix/data/?sort_by=&sort_reverse=&combo_expand=&employee=&online_app=&online_app_type=&online_app_platform=&dining_option=&show_opened=1&show_unpaid=1&show_irregular=1&sort_view=0&show_product=1&show_sku=1&show_class=1&quantity_settings=3&taxable_not_taxable=1&item_discount=1&order_discount=1&tax_column=1&no-filter=0&range_from=16%2F03%2F2018+04%3A00&range_to=17%2F03%2F2018+04%3A00&format=json";

        RootObject productMixRoot;
        string JSONMixString = "";
        const string filePath = @"C:\test\json\mix.json";
            RevelProductClassAccountMappingService accountMappingService;

        public override async Task Arrange()
        {
            accountMappingService = new RevelProductClassAccountMappingService();
            JSONMixString = System.IO.File.ReadAllText(filePath);
            productMixRoot = JsonConvert.DeserializeObject<RootObject>(JSONMixString);
            await base.Arrange();
        }


        [Test]
        public void Should_Create_A_New_Invoice_For_Exmouth_Grind_Using_Product_Mix()
        {
            //setup contact
            List<Contact> Contacts = new List<Contact>();
            List<Contact> Contactp = new List<Contact>();
            int i = 1;
            do
            {
                Contactp = SUT.Contacts.Page(i).Find().ToList();
                Contacts.AddRange(Contactp);
                i++;
            } while (Contactp.Count() > 0);

            var exmounthGrind = Contacts.FirstOrDefault(x => x.Name == "Exmouth Market Grind Sales");
            //end


            //setup invoice
            var invoice = new Invoice
            {
                Contact = exmounthGrind,
                Type = Xero.Api.Core.Model.Types.InvoiceType.AccountsReceivable,
                Status = Xero.Api.Core.Model.Status.InvoiceStatus.Draft,
                LineAmountTypes = Xero.Api.Core.Model.Types.LineAmountType.Exclusive,
                Date = DateTime.Now,
                DueDate = DateTime.Now,
                Reference = "Revel Sales XERO EXMOUTH TEST " + DateTime.Now.ToString("ddmmyy")
            };

            invoice.LineItems = new List<LineItem>();

            //assign xero classes to each product - for tax
            var allTaxCodes = XeroTaxCodeHelper.GetTaxCodes();
            //
            //get correct data for each class

            //get top level groups
            var groupings = productMixRoot.GetParentCategoriesAndProductGroups();



            //create discount line item
            var discountItemSum = groupings.Sum(x => x.ProductMixes.Sum(y => Convert.ToDecimal(y.discount)));
            var discountOrderSum = groupings.Sum(x => x.ProductMixes.Sum(y => Convert.ToDecimal(y.order_discount)));
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
            groupings.ToList().ForEach(x =>
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


            //we're done
            try
            {
               // var result = SUT.Invoices.Create(invoice);
            }
            catch (Exception ex)
            {

                throw;
            }
        }


    }
}
