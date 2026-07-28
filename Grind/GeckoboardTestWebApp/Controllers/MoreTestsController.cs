using Geckoboard._808nd.com;
using GeckoboardLibrary.Classes;
using GeckoboardLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GeckoboardTestWebApp.Controllers
{
    public class MoreTestsController : Controller
    {
        // GET: MoreTests
        public async Task RunRandomWidget()
        {
            GeckoboardOrganisation geckoOrg =
          new GeckoboardOrganisation("ab876212d31d37960e3154eb5e2bc0a0", "ShoreditchGrind");

            IGeckoboardObjectCreatorFactory factory = new GeckoboardObjectCreatorFactory(geckoOrg);
            List<GeckoboardObject> widgets = new List<GeckoboardObject>();

            var geckoFactory = new GeckoboardObjectCreatorFactory(geckoOrg);
            GeckoboardPushService pushService = new GeckoboardPushService();

            var widget = geckoFactory.CreateNumberSecondaryStat(19, "TodayWC",
"https://push.geckoboard.com/v1/send/51912-c8776700-f388-0134-dfc0-22000b048960",
"Today", 2, "Same Day Last Week",
1);


            await pushService.Push(widget);


        }
    }
}
