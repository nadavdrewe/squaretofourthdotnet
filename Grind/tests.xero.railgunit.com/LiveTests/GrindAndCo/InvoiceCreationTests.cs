using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Xero.Api.Core.Model;
using Revel._808nd.com.Classes.ServiceImplemenations;
using System.Data.SqlClient;
using System.Data;
using Revel._808nd.com.Classes.Reporting.Caternet;
using xero.railgunit.com.Taxes;

namespace tests.xero.railgunit.com.LiveTests.GrindAndCo
{
    public class InvoiceCreationTests : BaseGrindAndCoLiveTests
    {

        ////DANGER
        ///DO NOT RUN THESE THEY CREATE INVOICE IN THE LIVE SYSTEM
        ///
        [Test]
        public void Should_Create_A_New_Invoice_For_London_Grind()
        {
            var start = new DateTime(2017, 11, 22, 03, 00, 00);
            var end = new DateTime(2017, 11, 23, 03, 00, 00);
            var est = db.Establishments.FirstOrDefault(x => x.establishment_id == 4);

            var productClassService = new ProductClassService("", "", base.db);
            List<Contact> Contacts = new List<Contact>();
            List<Contact> Contactp = new List<Contact>();
            int i = 1;
            do
            {
                Contactp = SUT.Contacts.Page(i).Find().ToList();
                Contacts.AddRange(Contactp);
                i++;
            } while (Contactp.Count() > 0);

            var londonGrind = Contacts.FirstOrDefault(x => x.Name == "London Grind Sales");

            var invoice = new Invoice
            {
                Contact = londonGrind,
                Type = Xero.Api.Core.Model.Types.InvoiceType.AccountsReceivable,
                Status = Xero.Api.Core.Model.Status.InvoiceStatus.Draft,
                LineAmountTypes = Xero.Api.Core.Model.Types.LineAmountType.Exclusive,
                Date = DateTime.Now,
                DueDate = DateTime.Now,
                Reference = "Revel Sales RE TEST " + DateTime.Now.ToString("ddmmyy")
            };

            //execute stored proc and get back items
            //get summed items as per RevelUp
            var startParam = new SqlParameter("@startDate", SqlDbType.DateTime);
            startParam.Value = start;
            var endParam = new SqlParameter("@endDate", SqlDbType.DateTime);
            endParam.Value = end;
            var estParam = new SqlParameter("@establishmentId", SqlDbType.Int);
            estParam.Value = est.establishment_id;


            //PROC FILTERS OUT VOIDS - WE NEED TO KEEP IN VOID / COMP AMOUNT BUT NOT PURESALES
            //DELETED ARE REMOVED
            var summedItemsIncCompsAndVoids = db.Database.SqlQuery<CaternetItemSummary>(
                "Revel_GenerateCaternetSummary @startDate, @endDate, @establishmentId", startParam, endParam, estParam).ToList();



            //GetParentRootClass

            //assign xero classes to each product - for tax
            XeroTaxCodeHelper.GetTaxCodes();


            //put items into relevant categories 
            ProductCategoryService productCategoryService;


            //assign all correct codes

            invoice.LineItems = new List<LineItem> {
                new LineItem {Description = "Test Line 1", Quantity = 1,  LineAmount = 345.23M, AccountCode = "200" },
                new LineItem {Description = "Test Line 2", Quantity = 1, LineAmount = 13145.23M, AccountCode = "200"},
                 new LineItem {Description = "Test Line 3", Quantity = 1, LineAmount = 13145.23M, AccountCode = "200" }
            };

            try
            {
                //var result = SUT.Invoices.Create(invoice);
            }
            catch (Exception ex)
            {



                throw;
            }
        }

    }
}
