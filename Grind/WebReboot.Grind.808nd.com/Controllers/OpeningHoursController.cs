using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Models;
using Syncfusion.Linq;
using Web.Grind._808nd.com.Controllers;
using Revel._808nd.com.Classes.ServiceImplemenations;

namespace WebReboot.Grind._808nd.com.Controllers
{
    public class OpeningHoursController : Controller
    {
        private GrindContext db = new GrindContext();
        private string RevelAPIKEY { get; } = ConfigurationManager.AppSettings["RevelAPIKEY"];
        private string RevelBaseURL { get; } = ConfigurationManager.AppSettings["RevelBaseURL"];

        // GET: OpeningHours
        public async Task<ActionResult> Index()
        {
            return View(await db.OpeningHours.ToListAsync());
        }

        // GET: OpeningHours/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OpeningHours openingHours = await ((DbSet<OpeningHours>)db.OpeningHours).FindAsync(id);
            if (openingHours == null)
            {
                return HttpNotFound();
            }
            return View(openingHours);
        }

        // GET: OpeningHours/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OpeningHours/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "OpeningHoursID,Day,OpeningTime,ClosingTime")] OpeningHours openingHours)
        {
            if (ModelState.IsValid)
            {
                db.OpeningHours.Add(openingHours);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(openingHours);
        }

        // GET: OpeningHours/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OpeningHours openingHours = await ((DbSet<OpeningHours>)db.OpeningHours).FindAsync(id);
            if (openingHours == null)
            {
                return HttpNotFound();
            }
            return View(openingHours);
        }

        // POST: OpeningHours/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "OpeningHoursID,Day,OpeningTime,ClosingTime")] OpeningHours openingHours)
        {
            if (ModelState.IsValid)
            {
                db.Entry(openingHours).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(openingHours);
        }

        // GET: OpeningHours/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OpeningHours openingHours = await ((DbSet<OpeningHours>)db.OpeningHours).FindAsync(id);
            if (openingHours == null)
            {
                return HttpNotFound();
            }
            return View(openingHours);
        }

        // POST: OpeningHours/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            OpeningHours openingHours = await ((DbSet<OpeningHours>)db.OpeningHours).FindAsync(id);
            db.OpeningHours.Remove(openingHours);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



        public void CheckIfAllStoresHaveOpenedLate()
        {
            var today = DateTime.Now;
            var openingHOurs = db.OpeningHours.ToList();
            var factory = new ScheduledTaskFactory(db);
            var latehtmlBodyForEMail = "<h1 style='color:red;'>Stores today that opened late:</h1>";
            var notlatehtmlBodyForEMail = "</br/></br/><h1>Stores today that opened on time:</h1>";
            var storedOpenedLateFlag = false;

            var settings = db.MiscSettings.FirstOrDefault();
            var minutesLate = settings.LateOpeningStoreMinutesWindow;
            var weShouldRunTHeRoutine = settings.LateOpeningStoreNotifier;

            var shouldWeSendTheEmail = false;

            if (weShouldRunTHeRoutine)
            {
                foreach (var est in db.Establishments.Where(x => x.establishment_id != 2).ToList())
                {

                    //is there existing record, if so don't bother doing a check 
                    var existing = factory.Get(est.establishment_id, today);
                    if (existing == null)
                    {
                        var openingHour = openingHOurs.FirstOrDefault();

                        var datyIntValueForOpeningHour = Convert.ToInt32(openingHour.Day);
                        var dayIntValueForToday = Convert.ToInt32(today.DayOfWeek);

                        var comparisonPeriod = openingHOurs.Where(x => x.Day == today.DayOfWeek).FirstOrDefault();

                        if (comparisonPeriod != null) // we can do the check
                        {
                            var appended = "";
                            var lateFlag = false;
                            CheckIfStoreHasOpenedLate(est, minutesLate, Convert.ToDateTime(comparisonPeriod.OpeningTime), today,
                                out appended, out storedOpenedLateFlag);

                            if (storedOpenedLateFlag)
                            {
                                shouldWeSendTheEmail = true;
                                latehtmlBodyForEMail += "<h5>" + appended + "</h5>";
                                Console.WriteLine(latehtmlBodyForEMail);
                            }
                            else
                            {
                                notlatehtmlBodyForEMail += "<h5>" + appended + "</h5>";
                                Console.WriteLine(notlatehtmlBodyForEMail);
                            }
                        }
                        else
                        {
                            notlatehtmlBodyForEMail += "<h5>Apparently {0} Grind is not open today</h5>";
                            Console.WriteLine(notlatehtmlBodyForEMail);
                        }
                    }
                    else
                    {
                        using (var emailer = new EmailController())
                        {
                            emailer.SendMessageNadavIgnoreSendExeceptions(String.Format("THere was no period in the db for Establishment {0} on {1}", est.name, DateTime.Now.ToShortDateString()));
                        }
                    }

                }


                /* if (storedOpenedLateFlag)
                 {*/
                if (shouldWeSendTheEmail)
                {
                    using (var emailer = new EmailController())
                    {

                        emailer.SendMessage("david@grind.co.uk", "Grind - daily establishment late report - " + DateTime.Now.ToShortDateString(), latehtmlBodyForEMail + notlatehtmlBodyForEMail);
                        //  emailer.SendMessageNadav("Grind - daily establishment late report - " + DateTime.Now.Date, latehtmlBodyForEMail + notlatehtmlBodyForEMail);
                    }
                }


                /*}*/
            }

        }

        void CheckIfStoreHasOpenedLate(Establishment est, int lateMinutesNumber, DateTime timeToCheckAgainsttimeToCheckAgainst, DateTime dateToCheckAgainst, out string message, out bool lateFlag)
        {
            var today = dateToCheckAgainst;
            var amalgamatedDate = new DateTime(today.Year, today.Month, today.Day, timeToCheckAgainsttimeToCheckAgainst.Hour, timeToCheckAgainsttimeToCheckAgainst.Minute, timeToCheckAgainsttimeToCheckAgainst.Second);
            message = null;
            lateFlag = false;



            try
            {
                var dayOfWeek = DateTime.Now.DayOfWeek;
                var openingHour = db.OpeningHours.Include(x => x.Establishment).Where(y => y.Day == dayOfWeek && y.Establishment.establishment_id == est.establishment_id).FirstOrDefault();



                using (var emailer = new EmailController())
                {

                    if (openingHour.OpeningTime != null)
                    {


                        var openingTimeCast = Convert.ToDateTime(openingHour.OpeningTime);
                        var windowEnd = openingTimeCast.AddMinutes(lateMinutesNumber);

                        var startOfOpeningWindow = new DateTime(today.Year,
                            today.Month,
                            today.Day,
                            openingTimeCast.Hour,
                            openingTimeCast.Minute,
                            openingTimeCast.Second
                            );

                        var endOfOpeningWindow = new DateTime(
                            today.Year,
                            today.Month,
                            today.Day,
                              windowEnd.Hour,
                              windowEnd.Minute,
                             windowEnd.Second
                            );


                        timeToCheckAgainsttimeToCheckAgainst.AddMinutes(lateMinutesNumber);
                        var anyOrder =
                            db.OrderItems.Where(
                                x =>
                                    x.created_date <= endOfOpeningWindow
                                    && x.created_date >= startOfOpeningWindow
                                    && x.establishment_id == est.establishment_id).FirstOrDefault();

                        var firstActualOrder = db.OrderItems.Where(
                                x =>
                                    x.created_date >= startOfOpeningWindow
                                    && x.establishment_id == est.establishment_id).OrderBy(x => x.created_date).FirstOrDefault();



                        if (anyOrder == null)
                        {

                            string firstOrderTime = "never";
                            string name = "nothing";
                            if (firstActualOrder != null)
                            {
                                firstOrderTime = ((DateTime)firstActualOrder.created_date).TimeOfDay.ToString();
                                name = firstActualOrder.product_name_override.ToString();
                            }

                            lateFlag = true;
                            message =
                                String.Format(
                                    "{0} did not have an order within the first {1} minutes of opening, the window was from {2} to {3} and the first actual order was at {4} and was a {5}",
                                    est.name, lateMinutesNumber, startOfOpeningWindow.TimeOfDay,
                                    endOfOpeningWindow.TimeOfDay, firstOrderTime, name);
                            //send an email

                        }
                        else
                        {
                            lateFlag = false;
                            message = String.Format("{0} was supposed to open at {3} had an order at today at {1} that was a {2}", est.name, ((DateTime)anyOrder.created_date).TimeOfDay, anyOrder.product_name_override, startOfOpeningWindow.TimeOfDay);


                        }


                    }


                }

            }
            catch (Exception ex)
            {
                //todo: log it in the logs shown in the system
                throw;
            }



        }


    }
}
