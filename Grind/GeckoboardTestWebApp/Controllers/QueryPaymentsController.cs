using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Models;

namespace GeckoboardTestWebApp.Controllers
{
    public class QueryPaymentsController : Controller
    {
        //

        // GET: /QueryPayments/

        public ActionResult ShowPayments()
        {


            return View();
        }


        [HttpPost]
        public ActionResult ShowPayments(TimeFrame timeFrame)
        {
           
            var Grind = new GrindContext();

            var payments = Grind.Database.SqlQuery<Payment>("GetPayments @StartDateTime @EndDateTime", new SqlParameter("@StartDateTime", timeFrame.Start.ToString()), new SqlParameter("@EndDateTime", timeFrame.End.ToString()));

            
            return View(payments);
        }

        public class TimeFrame
        {
            public DateTime Start { get; set; }
            public DateTime End  { get; set; }


        }
    }
}