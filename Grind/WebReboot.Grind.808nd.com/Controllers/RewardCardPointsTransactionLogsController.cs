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
    public class RewardCardPointsTransactionLogsController : Controller
    {
        private GrindContext db = new GrindContext();

        // GET: RewardCardPointsTransactionLogs
        public ActionResult Index()
        {
            var start = new DateTime(2017, 09, 06);
            return View(db.RewardCardPointsTransactionLogs.Where(x => x.WhenCreated >= start).OrderBy(x=>x.id).ToList());
        }

        // GET: RewardCardPointsTransactionLogs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RewardCardPointsTransactionLog rewardCardPointsTransactionLog = db.RewardCardPointsTransactionLogs.Find(id);
            if (rewardCardPointsTransactionLog == null)
            {
                return HttpNotFound();
            }
            return View(rewardCardPointsTransactionLog);
        }

        // GET: RewardCardPointsTransactionLogs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RewardCardPointsTransactionLogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,orginal_points_total,orginal_points_current,new_points_total,new_points_current,pointsAdded,pointSetToRefreshInBucket,multiplier,card_number,WhenCreated")] RewardCardPointsTransactionLog rewardCardPointsTransactionLog)
        {
            if (ModelState.IsValid)
            {
                db.RewardCardPointsTransactionLogs.Add(rewardCardPointsTransactionLog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(rewardCardPointsTransactionLog);
        }

        // GET: RewardCardPointsTransactionLogs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RewardCardPointsTransactionLog rewardCardPointsTransactionLog = db.RewardCardPointsTransactionLogs.Find(id);
            if (rewardCardPointsTransactionLog == null)
            {
                return HttpNotFound();
            }
            return View(rewardCardPointsTransactionLog);
        }

        // POST: RewardCardPointsTransactionLogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,orginal_points_total,orginal_points_current,new_points_total,new_points_current,pointsAdded,pointSetToRefreshInBucket,multiplier,card_number,WhenCreated")] RewardCardPointsTransactionLog rewardCardPointsTransactionLog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rewardCardPointsTransactionLog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rewardCardPointsTransactionLog);
        }

        // GET: RewardCardPointsTransactionLogs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RewardCardPointsTransactionLog rewardCardPointsTransactionLog = db.RewardCardPointsTransactionLogs.Find(id);
            if (rewardCardPointsTransactionLog == null)
            {
                return HttpNotFound();
            }
            return View(rewardCardPointsTransactionLog);
        }

        // POST: RewardCardPointsTransactionLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RewardCardPointsTransactionLog rewardCardPointsTransactionLog = db.RewardCardPointsTransactionLogs.Find(id);
            db.RewardCardPointsTransactionLogs.Remove(rewardCardPointsTransactionLog);
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
