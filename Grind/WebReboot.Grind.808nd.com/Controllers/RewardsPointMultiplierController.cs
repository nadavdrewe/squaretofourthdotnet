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

namespace Web.Grind._808nd.com.Controllers
{
    [Authorize(Roles = "admin")]
    public class RewardsPointMultiplierController : Controller
    {
        private GrindContext db = new GrindContext();

        public RewardsPointMultiplierController()
        {
            db = new GrindContext();
        }

        // GET: /RewardsPointMultiplier/
        public async Task<ActionResult> Index()
        {
            return View(await db.RewardsPointsMultiplier.ToListAsync());
        }

        // GET: /RewardsPointMultiplier/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RewardsPointsMultiplier rewardspointsmultiplier = await ((DbSet<RewardsPointsMultiplier>)db.RewardsPointsMultiplier).FindAsync(id);
            if (rewardspointsmultiplier == null)
            {
                return HttpNotFound();
            }
            return View(rewardspointsmultiplier);
        }

        // GET: /RewardsPointMultiplier/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /RewardsPointMultiplier/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RewardsPointsMultiplier rewardspointsmultiplier)
        {
            if (ModelState.IsValid)
            {
                db.RewardsPointsMultiplier.Add(rewardspointsmultiplier);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(rewardspointsmultiplier);
        }

        // GET: /RewardsPointMultiplier/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RewardsPointsMultiplier rewardspointsmultiplier = await ((DbSet<RewardsPointsMultiplier>)db.RewardsPointsMultiplier).FindAsync(id);
            if (rewardspointsmultiplier == null)
            {
                return HttpNotFound();
            }
            return View(rewardspointsmultiplier);
        }

        // POST: /RewardsPointMultiplier/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(RewardsPointsMultiplier rewardspointsmultiplier)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rewardspointsmultiplier).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(rewardspointsmultiplier);
        }

        // GET: /RewardsPointMultiplier/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RewardsPointsMultiplier rewardspointsmultiplier = await ((DbSet<RewardsPointsMultiplier>)db.RewardsPointsMultiplier).FindAsync(id);
            if (rewardspointsmultiplier == null)
            {
                return HttpNotFound();
            }
            return View(rewardspointsmultiplier);
        }

        // POST: /RewardsPointMultiplier/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            RewardsPointsMultiplier rewardspointsmultiplier = await ((DbSet<RewardsPointsMultiplier>)db.RewardsPointsMultiplier).FindAsync(id);
            db.RewardsPointsMultiplier.Remove(rewardspointsmultiplier);
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
