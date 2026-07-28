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
    public class TestRewardsCardsNewsController : Controller
    {
        private GrindContext db = new GrindContext();

        // GET: TestRewardsCardsNews
        public async Task<ActionResult> Index()
        {
            return View(await db.RewardsCardNew.ToListAsync());
        }

        // GET: TestRewardsCardsNews/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RewardsCardNew rewardsCardNew = db.RewardsCardNew.Find(id);
            if (rewardsCardNew == null)
            {
                return HttpNotFound();
            }
            return View(rewardsCardNew);
        }

        // GET: TestRewardsCardsNews/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TestRewardsCardsNews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "DBKEY_rewardscardnew_id,ResourceUri,created_by,created_date,current_points,customer_revel,establishment,Revelid,number,payment_type,resource_uri,total_points,total_purchases,total_visits,updated_by,updated_date,customer_id,establishment_id,is_vip_card,vip_points_refresh,vip_points_last_refreshed,notes,days_since_last_visit,yesterdaysTotalPoints,yesterdaysTotalPointsWhenCreated,pointsMultiplierLastRun,theAddress")] RewardsCardNew rewardsCardNew)
        {
            if (ModelState.IsValid)
            {
                db.RewardsCardNew.Add(rewardsCardNew);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(rewardsCardNew);
        }

        // GET: TestRewardsCardsNews/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RewardsCardNew rewardsCardNew =  db.RewardsCardNew.Find(id);
            if (rewardsCardNew == null)
            {
                return HttpNotFound();
            }
            return View(rewardsCardNew);
        }

        // POST: TestRewardsCardsNews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "DBKEY_rewardscardnew_id,ResourceUri,created_by,created_date,current_points,customer_revel,establishment,Revelid,number,payment_type,resource_uri,total_points,total_purchases,total_visits,updated_by,updated_date,customer_id,establishment_id,is_vip_card,vip_points_refresh,vip_points_last_refreshed,notes,days_since_last_visit,yesterdaysTotalPoints,yesterdaysTotalPointsWhenCreated,pointsMultiplierLastRun,theAddress")] RewardsCardNew rewardsCardNew)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rewardsCardNew).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(rewardsCardNew);
        }

        // GET: TestRewardsCardsNews/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RewardsCardNew rewardsCardNew =  db.RewardsCardNew.Find(id);
            if (rewardsCardNew == null)
            {
                return HttpNotFound();
            }
            return View(rewardsCardNew);
        }

        // POST: TestRewardsCardsNews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            RewardsCardNew rewardsCardNew =  db.RewardsCardNew.Find(id);
            db.RewardsCardNew.Remove(rewardsCardNew);
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
