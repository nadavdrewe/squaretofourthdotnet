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
    public class _445CalendarController : Controller
    {
        private GrindContext db = new GrindContext();

        // GET: _445Calendar
        public ActionResult Index()
        {
            return View(db._445Calendar.ToList());
        }

        // GET: _445Calendar/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            _445Calendar _445Calendar = db._445Calendar.Find(id);
            if (_445Calendar == null)
            {
                return HttpNotFound();
            }
            return View(_445Calendar);
        }

        // GET: _445Calendar/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: _445Calendar/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,StartDate")] _445Calendar _445Calendar)
        {
            if (ModelState.IsValid)
            {
                db._445Calendar.Add(_445Calendar);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(_445Calendar);
        }

        // GET: _445Calendar/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            _445Calendar _445Calendar = db._445Calendar.Find(id);
            if (_445Calendar == null)
            {
                return HttpNotFound();
            }
            return View(_445Calendar);
        }

        // POST: _445Calendar/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,StartDate")] _445Calendar _445Calendar)
        {
            if (ModelState.IsValid)
            {
                db.Entry(_445Calendar).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(_445Calendar);
        }

        // GET: _445Calendar/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            _445Calendar _445Calendar = db._445Calendar.Find(id);
            if (_445Calendar == null)
            {
                return HttpNotFound();
            }
            return View(_445Calendar);
        }

        // POST: _445Calendar/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _445Calendar _445Calendar = db._445Calendar.Find(id);
            db._445Calendar.Remove(_445Calendar);
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
