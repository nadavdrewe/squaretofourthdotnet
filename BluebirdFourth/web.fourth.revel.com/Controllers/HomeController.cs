using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace web.fourth.revel.com.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        public ActionResult Lock()
        {

            return View();
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", "ScheduledTaskLogs");
             
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}