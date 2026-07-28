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

namespace WebReboot.Grind._808nd.com.Controllers
{
    public class MiscSettingsController : Controller
    {
        private GrindContext db = new GrindContext();

        // GET: MiscSettings
        public async Task<ActionResult> Index()
        {
            return View(await db.MiscSettings.ToListAsync());
        }

        // GET: MiscSettings/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MiscSettings miscSettings = await ((DbSet<MiscSettings>)db.MiscSettings).FindAsync(id);
            if (miscSettings == null)
            {
                return HttpNotFound();
            }
            return View(miscSettings);
        }

        // GET: MiscSettings/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MiscSettings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,LateOpeningStoreNotifier,LateOpeningStoreMinutesWindow")] MiscSettings miscSettings)
        {
            if (ModelState.IsValid)
            {
                db.MiscSettings.Add(miscSettings);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(miscSettings);
        }

        // GET: MiscSettings/Edit/5
        public async Task<ActionResult> Edit()
        {
          /*  if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }*/
            MiscSettings miscSettings = await ((DbSet<MiscSettings>)db.MiscSettings).FirstOrDefaultAsync() ?? new MiscSettings { LateOpeningStoreNotifier = false, LateOpeningStoreMinutesWindow = 0};
            if (miscSettings == null)
            {
                return HttpNotFound();
            }
            return View(miscSettings);
        }

        // POST: MiscSettings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,LateOpeningStoreNotifier,LateOpeningStoreMinutesWindow")] MiscSettings miscSettings)
        {
            if (ModelState.IsValid)
            {
                db.Entry(miscSettings).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Edit");
            }
            return View(miscSettings);
        }

        // GET: MiscSettings/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MiscSettings miscSettings = await ((DbSet<MiscSettings>)db.MiscSettings).FindAsync(id);
            if (miscSettings == null)
            {
                return HttpNotFound();
            }
            return View(miscSettings);
        }

        // POST: MiscSettings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            MiscSettings miscSettings = await ((DbSet<MiscSettings>)db.MiscSettings).FindAsync(id);
            db.MiscSettings.Remove(miscSettings);
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
