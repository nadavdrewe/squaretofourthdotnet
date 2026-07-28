using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using api.grind._808nd.com.Controllers;
using NUnit.Framework;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Models;
using Should;
using SpecsFor;

namespace apitests.grind._808nd.com.IntegrationTests.Controller
{
    public class WiFiControllerReadSpecs :SpecsFor<WifiLoginsController>
    {

        private IHttpActionResult result ;
        GrindContext db = new GrindContext();
        private WifiLogin record;
        protected override void Given()
        {
            SUT = new WifiLoginsController();                 
        }

        protected override void When()
        {
            record = db.WifiLogins.Take(1).First();            
        }

        protected override void AfterSpec()
        {
            db.Dispose();
        }

        [Test]
        public async Task then_it_should_read_back_a_record()
        {
            var fromAPI = await SUT.GetWifiLogin(record.Id);
            fromAPI.ShouldNotBeNull();
        }


    }
}
