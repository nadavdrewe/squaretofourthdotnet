using NUnit.Framework;
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

namespace tests.xero.railgunit.com.EndToEndInvoiceTests
{
    [TestFixture]
    public class BaseEndToEndExmouthMktTests
    {
        protected string revelAPIKey = "";
        protected string revelBaseURL = "";
        protected string BaseUrl = "https://api.xero.com/api.xro/2.0/";
        protected string ConsumerKey = "EBAL8VMLJODEF9LJF3VULOOXIKBUZ6";
        protected string ConsumerSecret = "GTL6TMZHOJW8E1FNVTTET8BY518DBU";
        protected string pathToCert = @"C:\GIT2016\Grind\tests.xero.railgunit.com\Certificates\Exmouth\public_privatekey.pfx";
        protected XeroCoreApi SUT;

 
        [SetUp]
        public virtual async Task Arrange()
        {
            // Private Application Sample
            X509Certificate2 cert = new X509Certificate2(pathToCert, "");
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
