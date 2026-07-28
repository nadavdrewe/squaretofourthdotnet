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
    public class ProjectionTypesController : Controller
    {
        private GrindContext db = new GrindContext();

        // GET: ProjectionTypes
        public ActionResult Index()
        {
            return View(db.ProjectionTypes.ToList());
        }

        // GET: ProjectionTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectionType projectionType = db.ProjectionTypes.Find(id);
            if (projectionType == null)
            {
                return HttpNotFound();
            }
            return View(projectionType);
        }

        // GET: ProjectionTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProjectionTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] ProjectionType projectionType)
        {
            if (ModelState.IsValid)
            {
                db.ProjectionTypes.Add(projectionType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(projectionType);
        }

        // GET: ProjectionTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectionType projectionType = db.ProjectionTypes.Find(id);
            if (projectionType == null)
            {
                return HttpNotFound();
            }
            return View(projectionType);
        }

        // POST: ProjectionTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] ProjectionType projectionType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(projectionType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(projectionType);
        }

        // GET: ProjectionTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectionType projectionType = db.ProjectionTypes.Find(id);
            if (projectionType == null)
            {
                return HttpNotFound();
            }
            return View(projectionType);
        }

        // POST: ProjectionTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProjectionType projectionType = db.ProjectionTypes.Find(id);
            db.ProjectionTypes.Remove(projectionType);
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
