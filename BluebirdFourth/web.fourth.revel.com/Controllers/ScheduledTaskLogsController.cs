using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Models;
using System.Web.Caching;

namespace web.fourth.revel.com.Controllers
{
    public class ScheduledTaskLogsController : Controller
    {
        private RevelContext db = new RevelContext();

        // GET: ScheduledTaskLogs
        public async Task<ActionResult> RefreshCachedScheduledTasksAndRedirect()
        {
            var ok = RefreshCachedScheduledTasks();
            return Redirect(Request.UrlReferrer.ToString());
        }

        public string Download(int logId)
        {
            var file = db.ScheduledTaskLogs.Find(logId);


            Response.ContentType = "text/xml";
            return file.Notes;
        }

        public async Task<bool> RefreshCachedScheduledTasks()
        {
            using (var db = new RevelContext())
            {
                var last6Logs = new List<ScheduledTaskLog>();
                var yesterdaysLogs = new List<ScheduledTaskLog>();

                try
                {

                    last6Logs =
                        db.ScheduledTaskLogs.Where(x => x.LogType == "LOCAL")
                            .Take(5)
                            .OrderByDescending(x => x.FireTime)
                            .ToList();

                    HttpRuntime.Cache.Insert(
                        "logs",
                        last6Logs,
                        null,
                        /* absoluteExpiration */ Cache.NoAbsoluteExpiration,
                        /* slidingExpiration */  Cache.NoSlidingExpiration,
                        /* priority */           CacheItemPriority.NotRemovable,
                        /* onRemoveCallback */   null);

                    var lastLogDate = (DateTime)db.ScheduledTaskLogs.Max(x => x.ContainerStartDate);


                    Nullable<DateTime> lastlogTime = DateTime.Now.AddDays(-2);

                    var lastLogs2Days = db.ScheduledTaskLogs.Where(x => x.ContainerStartDate > lastlogTime).ToList();


                    foreach (var log in lastLogs2Days)
                    {

                        if (((DateTime)log.ContainerStartDate).ToString("dd/MM/yyyy") == lastLogDate.ToString("dd/MM/yyyy") && log.LogType == "LOCAL" && log.Result == 1)//success
                        {
                            yesterdaysLogs.Add(log);
                        }

                    }

                    HttpRuntime.Cache.Insert(
                        "yesterdayLogs",
                        yesterdaysLogs,
                        null,
                        /* absoluteExpiration */ Cache.NoAbsoluteExpiration,
                        /* slidingExpiration */  Cache.NoSlidingExpiration,
                        /* priority */           CacheItemPriority.NotRemovable,
                        /* onRemoveCallback */   null);
                }
                catch (Exception)
                {
                    return false;

                }
            }

            return true;
        }



        public async Task<ActionResult> Index()
        {

            var earliestsdATE = DateTime.Now.AddDays(-14);

            ViewBag.FourthLogs = await db.ScheduledTaskLogs
                .Where(x => x.FireTime >= earliestsdATE)
                .Where(x => x.LogType == "FOURTH")
                .OrderByDescending(x => x.FireTime)
                .ToListAsync();

            ViewBag.ErrorLogs = await db.ScheduledTaskLogs
                .Where(x => x.FireTime >= earliestsdATE)
               .Where(x=>x.Result == 0)
                .OrderByDescending(x => x.FireTime)
                .ToListAsync();

            return View("Index", new List<ScheduledTaskLog>());
        }

        public async Task<ActionResult> FourthIndex()
        {
            var theDate = DateTime.Now.AddDays(-60);
            return View("Index", await db.ScheduledTaskLogs.Where(x => x.ContainerStartDate >= theDate).Where(x => x.LogType == "FOURTH").OrderByDescending(x => x.ContainerStartDate).ToListAsync());
        }

        public async Task<ActionResult> LocalDbIndex()
        {
            var theDate = DateTime.Now.AddDays(-60);
            return View("Index", await db.ScheduledTaskLogs.Where(x => x.ContainerStartDate >= theDate).Where(x => x.LogType == "LOCAL").OrderByDescending(x => x.ContainerStartDate).ToListAsync());
        }

        public async Task<ActionResult> XMLIndex()
        {
            var theDate = DateTime.Now.AddDays(-60);
            return View("Index", await db.ScheduledTaskLogs.Where(x => x.ContainerStartDate >= theDate).Where(x => x.LogType == "XML").OrderByDescending(x => x.ContainerStartDate).ToListAsync());
        }
        // GET: ScheduledTaskLogs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduledTaskLog scheduledTaskLog = db.ScheduledTaskLogs.Find(id);
            if (scheduledTaskLog == null)
            {
                return HttpNotFound();
            }
            return View(scheduledTaskLog);
        }

        // GET: ScheduledTaskLogs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ScheduledTaskLogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Message,FireTime,Detail,Result,Brand,BrandName,Establishment,EstablishmentName,TotalPounds,TotalItemCount")] ScheduledTaskLog scheduledTaskLog)
        {
            if (ModelState.IsValid)
            {
                db.ScheduledTaskLogs.Add(scheduledTaskLog);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(scheduledTaskLog);
        }

        // GET: ScheduledTaskLogs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduledTaskLog scheduledTaskLog = db.ScheduledTaskLogs.Find(id);
            if (scheduledTaskLog == null)
            {
                return HttpNotFound();
            }
            return View(scheduledTaskLog);
        }

        // POST: ScheduledTaskLogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Message,FireTime,Detail,Result,Brand,BrandName,Establishment,EstablishmentName,TotalPounds,TotalItemCount")] ScheduledTaskLog scheduledTaskLog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(scheduledTaskLog).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(scheduledTaskLog);
        }

        // GET: ScheduledTaskLogs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduledTaskLog scheduledTaskLog = db.ScheduledTaskLogs.Find(id);
            if (scheduledTaskLog == null)
            {
                return HttpNotFound();
            }
            return View(scheduledTaskLog);
        }

        // POST: ScheduledTaskLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ScheduledTaskLog scheduledTaskLog = db.ScheduledTaskLogs.Find(id);
            db.ScheduledTaskLogs.Remove(scheduledTaskLog);
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
    }
}
