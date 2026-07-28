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

namespace web.fourth.revel.com.Controllers
{
    public class SystemErrorsController : Controller
    {
        private RevelContext db = new RevelContext();

        // GET: SystemErrors
        public ActionResult Index()
        {
            return View(db.SystemErrors.ToList());
        }

        // GET: SystemErrors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemError systemError = db.SystemErrors.Find(id);
            if (systemError == null)
            {
                return HttpNotFound();
            }
            return View(systemError);
        }

        // GET: SystemErrors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SystemErrors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,Establishment,Brand,ErrorCode,Description,Notes")] SystemError systemError)
        {
            if (ModelState.IsValid)
            {
                db.SystemErrors.Add(systemError);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(systemError);
        }

        // GET: SystemErrors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemError systemError = db.SystemErrors.Find(id);
            if (systemError == null)
            {
                return HttpNotFound();
            }
            return View(systemError);
        }

        // POST: SystemErrors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,Establishment,Brand,ErrorCode,Description,Notes")] SystemError systemError)
        {
            if (ModelState.IsValid)
            {
                db.Entry(systemError).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(systemError);
        }

        // GET: SystemErrors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemError systemError = db.SystemErrors.Find(id);
            if (systemError == null)
            {
                return HttpNotFound();
            }
            return View(systemError);
        }

        // POST: SystemErrors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SystemError systemError = db.SystemErrors.Find(id);
            db.SystemErrors.Remove(systemError);
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
