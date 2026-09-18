using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using web.pipeline.fourth.com.Models;

namespace web.pipeline.fourth.com.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PublicOnboardingOptions _onboarding;

        public HomeController(ILogger<HomeController> logger, IOptions<PublicOnboardingOptions> onboarding)
        {
            _logger = logger;
            _onboarding = onboarding.Value;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            if (Request.Host.Host.Equals("squaresap.store", StringComparison.OrdinalIgnoreCase) ||
                Request.Host.Host.Equals("www.squaresap.store", StringComparison.OrdinalIgnoreCase))
                return View("~/Views/Sap/Index.cshtml");

            var clientSetupUrl = Url.Action("Index", "ClientSetup") ?? "/ClientSetup";
            ViewData["ClientSetupUrl"] = clientSetupUrl;
            ViewData["AdminLoginUrl"] = Url.Action("Login", "Access", new { returnUrl = clientSetupUrl })
                ?? "/Access/Login?returnUrl=%2FClientSetup";
            ViewData["OnboardingEnabled"] = _onboarding.Enabled;
            return View();
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
