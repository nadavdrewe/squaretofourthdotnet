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
    public class MenuTypesController : Controller
    {
        private GrindContext db = new GrindContext();

        // GET: MenuTypes
        public ActionResult Index()
        {
            return View(db.MenuTypes.ToList());
        }

        // GET: MenuTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuType menuType = db.MenuTypes.Find(id);
            if (menuType == null)
            {
                return HttpNotFound();
            }
            return View(menuType);
        }

        // GET: MenuTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MenuTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name")] MenuType menuType)
        {
            if (ModelState.IsValid)
            {
                db.MenuTypes.Add(menuType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(menuType);
        }

        // GET: MenuTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuType menuType = db.MenuTypes.Find(id);
            if (menuType == null)
            {
                return HttpNotFound();
            }
            return View(menuType);
        }

        // POST: MenuTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name")] MenuType menuType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(menuType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(menuType);
        }

        // GET: MenuTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuType menuType = db.MenuTypes.Find(id);
            if (menuType == null)
            {
                return HttpNotFound();
            }
            return View(menuType);
        }

        // POST: MenuTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MenuType menuType = db.MenuTypes.Find(id);
            db.MenuTypes.Remove(menuType);
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
