using NUnit.Framework;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.Email;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace tests.domain.pipelines.fourth.com.MailSinkSerilogTests
{
    [TestFixture]
    public class MailSinkTests
    {
        Logger log;

        [SetUp]
        public async Task Arrange()
        {
            //for TLS
            //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            //ServicePointManager.ServerCertificateValidationCallback = new
            //RemoteCertificateValidationCallback
            //(
            //   delegate { return true; }
            //);


            var fromEmail = Environment.GetEnvironmentVariable("FOURTH_PIPELINE_TEST_SMTP_FROM");
            var toEmail = Environment.GetEnvironmentVariable("FOURTH_PIPELINE_TEST_SMTP_TO");
            var mailServer = Environment.GetEnvironmentVariable("FOURTH_PIPELINE_TEST_SMTP_HOST");
            var password = Environment.GetEnvironmentVariable("FOURTH_PIPELINE_TEST_SMTP_PASSWORD");

            if (string.IsNullOrWhiteSpace(fromEmail) || string.IsNullOrWhiteSpace(toEmail) ||
                string.IsNullOrWhiteSpace(mailServer) || string.IsNullOrWhiteSpace(password))
            {
                Assert.Ignore("Set FOURTH_PIPELINE_TEST_SMTP_* environment variables to run the mail sink test.");
            }

            log = new LoggerConfiguration()
                .WriteTo.Email(
                    from: fromEmail,
                    to: toEmail,
                    host: mailServer,
                    port: 465,
                    credentials: new System.Net.NetworkCredential(fromEmail, password),
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information
                )
                .CreateLogger();
        }


        [Test]
        public async Task TestError()
        {
            try
            {
                var ex = new Exception("Where it going??");
                log.Error(ex, "This is A test man!!!");
                var ok = "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
