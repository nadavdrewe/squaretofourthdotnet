using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Xero.Api.Core.Model;

namespace tests.xero.railgunit.com.LiveTests.GrindAndCo
{
    public class DataRetrievelTests : BaseGrindAndCoLiveTests
    {
        //uses demo company
        [Test]
        public async Task Shoudld_Get_All_Contacts_From_Xero()
        {
            List<Contact> Contacts = new List<Contact>();
            List<Contact> Contactp = new List<Contact>();
            int i = 1;
            do
            {
                Contactp = SUT.Contacts.Page(i).Find().ToList();
                Contacts.AddRange(Contactp);
                i++;
            } while (Contactp.Count() > 0);

            var result = Contacts;
            result.ShouldNotBeNull();
            result.ShouldNotBeEmpty();
        }

        [Test]
        public async Task Shoudld_Get_Shroeditch_Contact_From_Xero()
        {
            List<Contact> Contacts = new List<Contact>();
            List<Contact> Contactp = new List<Contact>();
            int i = 1;
            do
            {
                Contactp = SUT.Contacts.Page(i).Find().ToList();
                Contacts.AddRange(Contactp);
                i++;
            } while (Contactp.Count() > 0);


            var result = Contacts.FirstOrDefault(x => x.Name == "London Grind Sales");
            var result2 = Contacts.FirstOrDefault(x => x.Name == "Covent Garden Grind Sales");

            result.ShouldNotBeNull();
            result2.ShouldNotBeNull();


        }




        //uses demo company
        [Test]
        public async Task Shoudld_Get_The_OrgName_From_Xero()
        {
            var result = SUT.Contacts.Find().ToList();
            result.ShouldNotBeNull();
            result.ShouldNotBeEmpty();
        }

        [Test]
        public async Task Shoudld_Get_The_Invoice_Accounts_From_Xero()
        {

        }

        [Test]
        public async Task Shoudld_Get_Sepcific_Invoice_Accounts_From_Xero()
        {

        }


    }
}
