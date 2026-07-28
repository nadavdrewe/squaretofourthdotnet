using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xero.Api.Core;
using Xero.Api.Example.Applications.Private;
using Xero.Api.Infrastructure.OAuth;
using Xero.Api.Serialization;
using Shouldly;
using System.Security.Cryptography.X509Certificates;

namespace tests.xero.railgunit.com.LogonTests
{
    [TestFixture]
    public class XeroInitialisationTests : BaseXeroTest
    {


        //uses demo company
        [Test]
        public async Task Shoudld_Get_Some_Contacts_From_Xero()
        {
            var result = SUT.Contacts.Find().ToList();
            result.ShouldNotBeNull();
            result.ShouldNotBeEmpty();
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
            SUT.Accounts.Find(""); 
        }

        [Test]
        public async Task Shoudld_Get_Sepcific_Invoice_Accounts_From_Xero()
        {

        }




    }
}
