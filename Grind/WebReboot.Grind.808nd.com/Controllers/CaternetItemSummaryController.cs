using Revel._808nd.com.Classes.Reporting.Caternet;
using Revel._808nd.com.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebReboot.Grind._808nd.com.Controllers
{
    public class CaternetItemSummaryController : Controller
    {
        GrindContext db = new GrindContext();
        // GET: CaternetItemSummary
        public ActionResult ViewSummary()
        {

            ViewBag.Ests = db.Establishments.Where(x => x.establishment_id != 2).ToList().Select(x => new SelectListItem
            {
                Text = x.name,
                Value = Convert.ToString(x.establishment_id)

            }).ToList();
            return View();
        }

        [HttpPost]
        public ActionResult ViewSummary(DateTime start, int establishmentId)
        {

            var revelStart = new DateTime(start.Year, start.Month, start.Day, 03, 00, 00);
            var revelEnd = revelStart.AddDays(1);
            ///run proc return resultss
            //get summed items as per RevelUp
            var startParam = new SqlParameter("@startDate", SqlDbType.DateTime);
            startParam.Value = revelStart;
            var endParam = new SqlParameter("@endDate", SqlDbType.DateTime);
            endParam.Value = revelEnd;
            var estParam = new SqlParameter("@establishmentId", SqlDbType.Int);
            estParam.Value = establishmentId;


            //PROC FILTERS OUT VOIDS - WE NEED TO KEEP IN VOID / COMP AMOUNT BUT NOT PURESALES
            //DELETED ARE REMOVED
            var summedItemsIncCompsAndVoids = db.Database.SqlQuery<CaternetItemSummary>(
              "Revel_GenerateCaternetSummary @startDate, @endDate, @establishmentId", startParam, endParam, estParam).ToList();

            ViewBag.Ests = db.Establishments.Where(x => x.establishment_id != 2).ToList().Select(x => new SelectListItem
            {
                Text = x.name,
                Value = Convert.ToString(x.establishment_id)

            }).ToList();
            return View(summedItemsIncCompsAndVoids);
        }
    }
}