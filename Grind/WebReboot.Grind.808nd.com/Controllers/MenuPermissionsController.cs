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
using extension.railgunit.com.MVCHelper;
using Microsoft.Ajax.Utilities;
using Menu = System.Web.UI.WebControls.Menu;

namespace WebReboot.Grind._808nd.com.Controllers
{
    [Authorize]
    public class MenuPermissionsController : Controller
    {
        private GrindContext db = new GrindContext();

        // GET: MenuPermissions
        public ActionResult Index()
        {
            return View(db.MenuPermissions.Include(x => x.Establishment).Include(x => x.MenuType).ToList());
        }

        // GET: MenuPermissions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuPermissions menuPermissions = db.MenuPermissions.Find(id);
            if (menuPermissions == null)
            {
                return HttpNotFound();
            }
            return View(menuPermissions);
        }

        // GET: MenuPermissions/Create
        public ActionResult Create()
        {
            ViewBag.Ests =
                 DropDownListExtensions.AddPleaseSelectRow(db.Establishments.ToList().Select(x => new SelectListItem
                 {
                     Text = x.name,
                     Value = x.DBKEY_establishment_id.ToStringInvariant()

                 }).ToList());

            ViewBag.MenuTypes =
                 DropDownListExtensions.AddPleaseSelectRow(db.MenuTypes.ToList().Select(x => new SelectListItem
                 {
                     Text = x.name,
                     Value = x.id.ToStringInvariant()

                 }).ToList());


            return View();
        }

        // POST: MenuPermissions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MenuPermissions menuPermissions)
        {
            ViewBag.Ests =
                DropDownListExtensions.AddPleaseSelectRow(db.Establishments.ToList().Select(x => new SelectListItem
                {
                    Text = x.name,
                    Value = x.DBKEY_establishment_id.ToStringInvariant()

                }).ToList());

            ViewBag.MenuTypes =
                 DropDownListExtensions.AddPleaseSelectRow(db.MenuTypes.ToList().Select(x => new SelectListItem
                 {
                     Text = x.name,
                     Value = x.id.ToStringInvariant()

                 }).ToList());


            menuPermissions.Establishment =
                db.Establishments.First(
                    x => x.DBKEY_establishment_id == menuPermissions.Establishment.DBKEY_establishment_id);

            menuPermissions.MenuType = db.MenuTypes.First(x => x.id == menuPermissions.MenuType.id);

            if (ModelState.IsValid)
            {
                db.MenuPermissions.Add(menuPermissions);
                db.SaveChanges();



                return RedirectToAction("Index");
            }

            return View(menuPermissions);
        }

        // GET: MenuPermissions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuPermissions menuPermissions = db.MenuPermissions.Find(id);
            if (menuPermissions == null)
            {
                return HttpNotFound();
            }
            return View(menuPermissions);
        }

        // POST: MenuPermissions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id")] MenuPermissions menuPermissions)
        {
            if (ModelState.IsValid)
            {
                db.Entry(menuPermissions).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(menuPermissions);
        }

        // GET: MenuPermissions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuPermissions menuPermissions = db.MenuPermissions.Find(id);
            if (menuPermissions == null)
            {
                return HttpNotFound();
            }
            return View(menuPermissions);
        }

        // POST: MenuPermissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MenuPermissions menuPermissions = db.MenuPermissions.Find(id);
            db.MenuPermissions.Remove(menuPermissions);
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
