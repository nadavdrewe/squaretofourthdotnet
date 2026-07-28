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
    public class CashupNotifiersController : Controller
    {
        private GrindContext db = new GrindContext();
        private IEnumerable<SelectListItem> Establishments;
        // GET: CashupNotifiers


        public CashupNotifiersController()
        {           
           Establishments = db.Establishments.ToList().Select(x=>new SelectListItem
            {
                Value = x.DBKEY_establishment_id.ToString(),
                Text = x.name
            });
        }
        public ActionResult Index()
        {
            var cashupNotifiers = db.CashupNotifiers.Include(c => c.Establishment);
            return View(cashupNotifiers.ToList());
        }

        // GET: CashupNotifiers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CashupNotifier cashupNotifier = db.CashupNotifiers.Find(id);
            if (cashupNotifier == null)
            {
                return HttpNotFound();
            }
            return View(cashupNotifier);
        }

        // GET: CashupNotifiers/Create
        public ActionResult Create()
        {
            ViewBag.DBKEY_establishment_id = new SelectList(db.Establishments, "DBKEY_establishment_id", "name");
            return View();
        }

        // POST: CashupNotifiers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,NotificationAddress,DBKEY_establishment_id,Enabled,UniversalContact")] CashupNotifier cashupNotifier)
        {
            if (ModelState.IsValid)
            {
                db.CashupNotifiers.Add(cashupNotifier);
                db.SaveChanges();
                ViewBag.Est = Establishments;

                return RedirectToAction("Index");
            }

            ViewBag.DBKEY_establishment_id = new SelectList(db.Establishments, "DBKEY_establishment_id", "name", cashupNotifier.DBKEY_establishment_id);
            return View(cashupNotifier);
        }

        // GET: CashupNotifiers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CashupNotifier cashupNotifier = db.CashupNotifiers.Find(id);
            if (cashupNotifier == null)
            {
                return HttpNotFound();
            }
            ViewBag.DBKEY_establishment_id = new SelectList(db.Establishments, "DBKEY_establishment_id", "name", cashupNotifier.DBKEY_establishment_id);
            ViewBag.Est = Establishments;
            return View(cashupNotifier);
        }

        // POST: CashupNotifiers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,NotificationAddress,DBKEY_establishment_id,Enabled,UniversalContact")] CashupNotifier cashupNotifier)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cashupNotifier).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DBKEY_establishment_id = new SelectList(db.Establishments, "DBKEY_establishment_id", "name", cashupNotifier.DBKEY_establishment_id);
            return View(cashupNotifier);
        }

        // GET: CashupNotifiers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CashupNotifier cashupNotifier = db.CashupNotifiers.Find(id);
            if (cashupNotifier == null)
            {
                return HttpNotFound();
            }
            return View(cashupNotifier);
        }

        // POST: CashupNotifiers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CashupNotifier cashupNotifier = db.CashupNotifiers.Find(id);
            db.CashupNotifiers.Remove(cashupNotifier);
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
