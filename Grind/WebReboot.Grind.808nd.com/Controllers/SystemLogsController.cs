using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Models;

namespace WebReboot.Grind._808nd.com.Controllers
{
    public class SystemLogsController : Controller
    {
        private GrindContext db = new GrindContext();

        // GET: SystemLogs
        public ActionResult Index()
        {
            return View(db.SystemLogs.OrderByDescending(x => x.WhenCreated).Take(500).ToList());
        }

        // GET: SystemLogs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemLog systemLog = db.SystemLogs.Find(id);
            if (systemLog == null)
            {
                return HttpNotFound();
            }
            return View(systemLog);
        }

        // GET: SystemLogs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SystemLogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Type,Note,WhoTriggered,WhenCreated")] SystemLog systemLog)
        {
            if (ModelState.IsValid)
            {
                db.SystemLogs.Add(systemLog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(systemLog);
        }

        // GET: SystemLogs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemLog systemLog = db.SystemLogs.Find(id);
            if (systemLog == null)
            {
                return HttpNotFound();
            }
            return View(systemLog);
        }

        // POST: SystemLogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Type,Note,WhoTriggered,WhenCreated")] SystemLog systemLog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(systemLog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(systemLog);
        }

        // GET: SystemLogs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemLog systemLog = db.SystemLogs.Find(id);
            if (systemLog == null)
            {
                return HttpNotFound();
            }
            return View(systemLog);
        }

        // POST: SystemLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SystemLog systemLog = db.SystemLogs.Find(id);
            db.SystemLogs.Remove(systemLog);
            db.SaveChanges();
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
