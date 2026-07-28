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
    public class MenuFilesController : Controller
    {
        private GrindContext db = new GrindContext();


        // GET: MenuFiles
        public ActionResult Index()
        {
 
            return View(db.MenuFiles.ToList());
        }

        // GET: MenuFiles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuFile menuFile = db.MenuFiles.Find(id);
            if (menuFile == null)
            {
                return HttpNotFound();
            }
            return View(menuFile);
        }

        // GET: MenuFiles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MenuFiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,bytes,filename,extension,url")] MenuFile menuFile)
        {

            if (ModelState.IsValid)
            {
                db.MenuFiles.Add(menuFile);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(menuFile);
        }

        // GET: MenuFiles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuFile menuFile = db.MenuFiles.Find(id);
            if (menuFile == null)
            {
                return HttpNotFound();
            }
            return View(menuFile);
        }

        // POST: MenuFiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,bytes,filename,extension,url")] MenuFile menuFile)
        {
            if (ModelState.IsValid)
            {
                db.Entry(menuFile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(menuFile);
        }

        // GET: MenuFiles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuFile menuFile = db.MenuFiles.Find(id);
            if (menuFile == null)
            {
                return HttpNotFound();
            }
            return View(menuFile);
        }

        // POST: MenuFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MenuFile menuFile = db.MenuFiles.Find(id);
            db.MenuFiles.Remove(menuFile);
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
