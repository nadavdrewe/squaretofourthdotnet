using Quartz;
using Revel._808nd.com.ProductMix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xero.Api.Core;
using xero.railgunit.com.Grind;
using Xero.Api.Example.Applications.Private;

namespace scheduledtasks.grind._808nd.com.ScheduledTasks
{

    /// <summary>
    /// Sends Revel Product Mix Accounts To Xero
    /// </summary>
    public class PushToXeroJob : IJob
    {
        RootObject productMixRoot;
        const string filePath = @"C:\test\json\mix.json";
        XeroCoreApi SUT;

        public async void Execute(IJobExecutionContext context)
        {
            //do date checks
            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {

                //setup containers
                List<XeroCompanyContainer> topLevelContainers = new List<XeroCompanyContainer>();
                XeroCompanyContainer grindContainer = new XeroCompanyContainer
                {//setup GrindAndCo container
                    ConsumerKey = "9C219QS0VADCCFZT9T6NMNLBLHEAGA",
                    ConsumerSecret = "9TRZOJMAIAIPW28BMQXUEL6T6ROY5D",
                    PathToCert = @"C:\GIT2016\Grind\tests.xero.railgunit.com\Certificates\GrindAndCo\public_privatekey.pfx",
                    EstablishmentMappings = new List<EstablishmentXeroMapping>
                {
                      new EstablishmentXeroMapping { EstablishmentId = "1", XeroContactName = "Shoreditch Grind Sales" },
                new EstablishmentXeroMapping { EstablishmentId = "3", XeroContactName = "Soho Grind Sales" },
                new EstablishmentXeroMapping { EstablishmentId = "4", XeroContactName = "London Grind Sales" },
                new EstablishmentXeroMapping { EstablishmentId = "5", XeroContactName = "Hatton Garden Grind Sales" },
                new EstablishmentXeroMapping { EstablishmentId = "6", XeroContactName = "Royal Exchange Sales" },
                new EstablishmentXeroMapping { EstablishmentId = "7", XeroContactName = "Covent Garden Grind Sales" },
                new EstablishmentXeroMapping { EstablishmentId = "8", XeroContactName = "Clerkenwell Grind Sales" },
                new EstablishmentXeroMapping { EstablishmentId = "9", XeroContactName = "Whitechapel Grind Sales" }
                }

                };

                XeroCompanyContainer exmouthMarketContainer = new XeroCompanyContainer
                {
                    ConsumerKey = "EBAL8VMLJODEF9LJF3VULOOXIKBUZ6",
                    ConsumerSecret = "GTL6TMZHOJW8E1FNVTTET8BY518DBU",
                    PathToCert = @"C:\GIT2016\Grind\tests.xero.railgunit.com\Certificates\Exmouth\public_privatekey.pfx",
                    EstablishmentMappings = new List<EstablishmentXeroMapping>
                {
                      new EstablishmentXeroMapping { EstablishmentId = "10", XeroContactName = "Exmouth Market Grind Sales" },
                }
                };




                //we're good to go
                // Private Application Sample



                //X509Certificate2 cert = new X509Certificate2(pathToCert, "");
                //SUT = new XeroCoreApi(BaseUrl, new PrivateAuthenticator(cert),
                //    new Consumer(ConsumerKey, ConsumerSecret), null,
                //    new DefaultMapper(), new DefaultMapper());

                //var user = new ApiUser { Name = Environment.MachineName };
                //await Act();



            }
        }
    }
}
