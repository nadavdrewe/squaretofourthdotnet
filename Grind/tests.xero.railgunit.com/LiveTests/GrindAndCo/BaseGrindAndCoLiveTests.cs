using NUnit.Framework;
using Revel._808nd.com.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xero.Api.Core;
using Xero.Api.Example.Applications.Private;
using Xero.Api.Infrastructure.OAuth;
using Xero.Api.Serialization;

namespace tests.xero.railgunit.com.LiveTests
{
    [TestFixture]
    public class BaseGrindAndCoLiveTests
    {
        protected string revelAPIKey = "";
        protected string revelBaseURL = "";
        protected string BaseUrl = "https://api.xero.com/api.xro/2.0/";
        protected string ConsumerKey = "9C219QS0VADCCFZT9T6NMNLBLHEAGA";
        protected string ConsumerSecret = "9TRZOJMAIAIPW28BMQXUEL6T6ROY5D";
        protected string pathToCert = @"C:\GIT2016\Grind\tests.xero.railgunit.com\Certificates\GrindAndCo\public_privatekey.pfx";
        protected XeroCoreApi SUT;
        protected GrindContext db = new GrindContext("GrindLiveContext");




        [SetUp]
        public async Task Arrange()
        {
            // Private Application Sample
            X509Certificate2 cert = new X509Certificate2(pathToCert, "1");
            SUT = new XeroCoreApi(BaseUrl, new PrivateAuthenticator(cert),
                new Consumer(ConsumerKey, ConsumerSecret), null,
                new DefaultMapper(), new DefaultMapper());

            var user = new ApiUser { Name = Environment.MachineName };
            await Act();
        }

        public async Task Act()
        {

        }
    }
}
