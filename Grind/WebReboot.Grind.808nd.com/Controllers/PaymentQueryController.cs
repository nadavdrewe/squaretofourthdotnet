using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.PaymentSummaries;
using Revel._808nd.com.Models;

namespace WebReboot.Grind._808nd.com.Controllers
{
    [Authorize]
    public class PaymentQueryController : Controller
    {
        GrindContext db = new GrindContext();
        // GET: PaymentQuery

        private List<int> _existingEstablishmentIds;


        public PaymentQueryController()
        {
            _existingEstablishmentIds = db.Establishments.Select(x => x.establishment_id).ToList();
        }
        public async Task<ActionResult> GetPayments()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GetPayments(DateTime start, int howLongUnits, PaymentSummaryGroupRequestType requestType, List<int> requestedEstablishments = null)
        {

            var adjustedStart = new DateTime(start.Year, start.Month, start.Day, 03, 00, 00);
            DateTime adjustedEnd;


            switch (requestType)
            {

                case PaymentSummaryGroupRequestType.Hours:
                    adjustedEnd = adjustedStart.AddHours(howLongUnits);
                    break;
                case PaymentSummaryGroupRequestType.Days:
                    adjustedEnd = adjustedStart.AddDays(howLongUnits);
                    break;
                case PaymentSummaryGroupRequestType.Weeks:
                    adjustedEnd = adjustedStart.AddDays(howLongUnits * 7);
                    break;
                case PaymentSummaryGroupRequestType.Months:
                    adjustedEnd = adjustedStart.AddMonths(howLongUnits);
                    break;
                default:
                    return new HttpStatusCodeResult(400, "Didn't recognise unit type");

            }

            var model = new PaymentSummaryGroup();

            if (requestedEstablishments.Contains(0))
            {
                requestedEstablishments = null;
            }

            if (requestedEstablishments != null)
            {
                foreach (var existing in requestedEstablishments)
                {
                    var existsFlag = false;
                    foreach (var estId in _existingEstablishmentIds)
                    {
                        if (existing == estId)
                        {
                            existsFlag = true;
                            break;

                        }
                       
                    }

                    if (existsFlag != true)
                    {
                        return new HttpNotFoundResult("That establishmentId doesn't exist");
                    }
                        
                }
            }


            try
            {

                PaymentSummaryGroupFactory factory = new PaymentSummaryGroupFactory(db);
                var paymentGroup = factory.Create(adjustedStart, adjustedEnd, requestType, requestedEstablishments);
                //turn into ViewModel


                return View(paymentGroup);

            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, "Sorry, something went wrong");
            }


        }

        // GET: PaymentQuery/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PaymentQuery/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PaymentQuery/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: PaymentQuery/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PaymentQuery/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: PaymentQuery/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PaymentQuery/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
