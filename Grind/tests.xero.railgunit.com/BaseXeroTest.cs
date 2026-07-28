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

namespace tests.xero.railgunit.com
{
    public abstract class BaseXeroTest
    {
        string BaseUrl = "https://api.xero.com/api.xro/2.0/";
        string ConsumerKey = "2XENAKYOM5NEEZAMB1FDHXNL2JI8RN";
        string ConsumerSecret = "MTYCRWWF4HK7TZ2V7JMFMNJYG0QCUT";
        string pathToCert = @"C:\GIT2016\Grind\tests.xero.railgunit.com\Certificates\Demo\public_privatekey.pfx";
        protected XeroCoreApi SUT;

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
