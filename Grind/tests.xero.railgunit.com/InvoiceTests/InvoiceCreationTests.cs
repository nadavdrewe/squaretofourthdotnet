using NUnit.Framework;
using Revel._808nd.com.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xero.Api.Core.Model;

namespace tests.xero.railgunit.com
{
    public class InvoiceCreationTests : BaseXeroTest
    {
        [Test]
        public void Should_Create_A_New_Invoice()
        {
            var db = new GrindContext("GrindLiveContext");
            var cat = db.ProductCategories.First();

            var randomContact = SUT.Contacts.Find().ToList().First();

            var invoice = new Invoice
            {
                Contact = randomContact,
                Type = Xero.Api.Core.Model.Types.InvoiceType.AccountsReceivable,
                Status = Xero.Api.Core.Model.Status.InvoiceStatus.Draft,
                LineAmountTypes = Xero.Api.Core.Model.Types.LineAmountType.Exclusive,
                Date = DateTime.Now,
                DueDate = DateTime.Now,
                Reference = "Revel Sales RE TEST " + DateTime.Now.ToString("ddmmyy")
            };

            invoice.LineItems = new List<LineItem> {
                new LineItem {Description = "Test Line 1", Quantity = 1,  LineAmount = 345.23M},
                new LineItem {Description = "Test Line 2", Quantity = 1, LineAmount = 13145.23M  },
                 new LineItem {Description = "Test Line 3", Quantity = 1, LineAmount = 13145.23M }
            };

            try
            {
                var result = SUT.Invoices.Create(invoice);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
