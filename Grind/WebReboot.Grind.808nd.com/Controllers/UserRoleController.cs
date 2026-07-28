using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Web.Grind._808nd.com.Models;

namespace WebReboot.Grind._808nd.com.Controllers
{
    public class UserRoleController : Controller
    {
        private GrindAuthContext db = new GrindAuthContext();

        public UserRoleController()
        {
            db = new GrindAuthContext();
        }

        // GET: /UserRole/
        public async Task<ActionResult> Index()
        {
            var usersAndRoles = await db.AspNetRoles.Include(x => x.AspNetUsers).ToListAsync();
            return View(usersAndRoles);
        }

        // GET: /UserRole/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetRole aspnetrole = await db.AspNetRoles.FindAsync(id);
            if (aspnetrole == null)
            {
                return HttpNotFound();
            }
            return View(aspnetrole);
        }

        // GET: /UserRole/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /UserRole/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name")] AspNetRole aspnetrole)
        {
            if (ModelState.IsValid)
            {
                db.AspNetRoles.Add(aspnetrole);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(aspnetrole);
        }

        // GET: /UserRole/Edit/5
        public async Task<ActionResult> Edit(string id)
        {


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var vm = db.AspNetUsers.Include(x => x.AspNetRoles).Where(x => x.Id == id).Select(x => new UserRoleViewModel
            {
                UserName = x.UserName,
                Id = x.Id,
                Role = x.AspNetRoles.FirstOrDefault()
            });

            if (vm == null)
            {
                return HttpNotFound();
            }
            return View(vm);
        }

        // POST: /UserRole/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name")] AspNetRole aspnetrole)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspnetrole).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(aspnetrole);
        }

        // GET: /UserRole/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetRole aspnetrole = await db.AspNetRoles.FindAsync(id);
            if (aspnetrole == null)
            {
                return HttpNotFound();
            }
            return View(aspnetrole);
        }

        // POST: /UserRole/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            AspNetRole aspnetrole = await db.AspNetRoles.FindAsync(id);
            db.AspNetRoles.Remove(aspnetrole);
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
