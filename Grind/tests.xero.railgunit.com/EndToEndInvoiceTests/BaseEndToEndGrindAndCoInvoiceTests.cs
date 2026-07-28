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

namespace tests.xero.railgunit.com.EndToEndInvoiceTests
{
    public class BaseEndToEndGrindAndCoInvoiceTests
    {

        protected string GrindAndCoBaseURL = "https://api.xero.com/api.xro/2.0/";
        protected string GrindAndCoConsumerKey = "9C219QS0VADCCFZT9T6NMNLBLHEAGA";
        protected string GrindAndCoConsumerSecret = "9TRZOJMAIAIPW28BMQXUEL6T6ROY5D";
        protected string GrindAndCoPathToCert = @"C:\GIT2016\Grind\tests.xero.railgunit.com\Certificates\GrindAndCo\public_privatekey.pfx";
        protected XeroCoreApi GrindSUT;
        protected GrindContext db = new GrindContext("GrindLiveContext");

        protected string ExBaseUrl = "https://api.xero.com/api.xro/2.0/";
        protected string ExConsumerKey = "EBAL8VMLJODEF9LJF3VULOOXIKBUZ6";
        protected string ExConsumerSecret = "GTL6TMZHOJW8E1FNVTTET8BY518DBU";
        protected string ExpathToCert = @"C:\GIT2016\Grind\tests.xero.railgunit.com\Certificates\Exmouth\public_privatekey.pfx";
        protected XeroCoreApi ExSUT;

        [SetUp]
        public virtual async Task Arrange()
        {
            // Private Application Sample
            X509Certificate2 GrindAndCocert = new X509Certificate2(GrindAndCoPathToCert, "1");
            GrindSUT = new XeroCoreApi(GrindAndCoBaseURL, new PrivateAuthenticator(GrindAndCocert),
                new Consumer(GrindAndCoConsumerKey, GrindAndCoConsumerSecret), null,
                new DefaultMapper(), new DefaultMapper());

            var user = new ApiUser { Name = Environment.MachineName };

            // Private Application Sample
            X509Certificate2 ExCert = new X509Certificate2(ExpathToCert, "");
            ExSUT = new XeroCoreApi(ExBaseUrl, new PrivateAuthenticator(ExCert),
                new Consumer(ExConsumerKey, ExConsumerSecret), null,
                new DefaultMapper(), new DefaultMapper());



            await Act();



        }

        public async Task Act()
        {

        }

    }
}
