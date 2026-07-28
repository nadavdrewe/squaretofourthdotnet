using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Models;
using WebGrease.Css.Extensions;

namespace Web.Grind._808nd.com.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private GrindContext _db = new GrindContext();

        public HomeController()
        {
            _db = new GrindContext();
        }
        public ActionResult Index()
        {
            /*    var files =
                   Directory.GetFiles(HttpRuntime.AppDomainAppPath + @"SavedFiles\")
                       .OrderByDescending(d => new FileInfo(d).LastWriteTime).ToSafeReadOnlyCollection();


                List<string> justFileNames = new List<string>();

                foreach (var file in files)
                {
                
                    justFileNames.Add(Path.GetFileName(file));
                }


                return View(justFileNames);*/


            //var cards = _db.RewardsCardNew.AsNoTracking().OrderByDescending(x => x.current_points).Take(30).ToList();
            //List<RewardsCardNew> cardsTosend = new List<RewardsCardNew>();

            //foreach (var c in cards)
            //{
            //    if (c.number != null)
            //    {
            //        c.Customer = _db.Customers.Where(x => x.LicNumber.Trim() == c.number.Trim()).FirstOrDefault();
            //        cardsTosend.Add(c);
            //    }
            //}



            //ViewBag.Top10 = cardsTosend;
            ViewBag.Message = "Welcome to Grind back of house";

            return View();
        }



        [HttpPost]
        public void OpenXLSFile(string filename)
        {
            //put filename in session
            var path = filename;
            Session["activeFilename"] = path;
            /*  Session["currentCashupPage"] = currentPage;  */
            Response.Redirect("~/Pages/Cashup.aspx");

        }
    }
}