using System;
using System.Collections.Generic;
using System.Configuration;
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
    [Authorize]
    public class RewardCardController : Controller
    {
        private GrindContext db = new GrindContext();

        private string RevelAPIKEY { get; } = ConfigurationManager.AppSettings["RevelAPIKEY"];
        private string RevelBaseURL { get; } = ConfigurationManager.AppSettings["RevelBaseURL"];

        public RewardCardController()
        {
            db = new GrindContext();
        }

        // GET: /RewardCard/
        public async Task<ActionResult> Index()
        {
            List<RewardsCardNew> cards = RewardsCardNew.GetRewardCardsNewAndCustomer();


            return View(cards.Where(x => x.is_vip_card != true).ToList());
        }

        [Authorize(Roles = "admin")]
        public async Task<ActionResult> IndexVIP()
        {
            List<RewardsCardNew> cards =  RewardsCardNew.GetRewardCardsNewAndCustomer();

            return View("Index", cards.Where(x => x.is_vip_card == true).ToList());
        }


        // GET: /RewardCard/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RewardsCardNew rewardscardnew = await ((DbSet<RewardsCardNew>)db.RewardsCardNew).FindAsync(id);
            if (rewardscardnew == null)
            {
                return HttpNotFound();
            }
            return View(rewardscardnew);
        }

        // GET: /RewardCard/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /RewardCard/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "DBKEY_rewardscardnew_id,ResourceUri,created_by,created_date,current_points,customer_revel,establishment,Revelid,number,payment_type,resource_uri,total_points,total_purchases,total_visits,updated_by,updated_date,customer_id,establishment_id,is_vip_card,theAddress,vip_points_refresh")] RewardsCardNew rewardscardnew)
        {
            var est = new Establishment(1, "Grind",
                                  RevelAPIKEY,
                                  new Uri(RevelBaseURL));

            //create in Revel
            var writer = new WebserviceDataWriter(est, db);

            var webCreate = await writer.CreateRewardCard(rewardscardnew);

            //need to get the IDs back from Revel

            if (webCreate.Equals(0))
            {
                db.RewardsCardNew.Add(rewardscardnew);
                int saveCount = await db.SaveChangesAsync();

                if (saveCount > 0)
                {
                    //successfull!
                    return RedirectToAction("Index");
                }
                else
                {
                    throw new Exception();
                }

            }






            return View(rewardscardnew);
        }



        public async Task<int> CreateCardRevelandDB([Bind(Include = "DBKEY_rewardscardnew_id,ResourceUri,created_by,created_date,current_points,customer_revel,establishment,Revelid,number,payment_type,resource_uri,total_points,total_purchases,total_visits,updated_by,updated_date,customer_id,establishment_id,is_vip_card,theAddress,vip_points_refresh")] RewardsCardNew rewardscardnew)
        {
            var est = new Establishment(1, "Grind",
           RevelAPIKEY,
            new Uri(RevelBaseURL));
            //create in Revel
            var writer = new WebserviceDataWriter(est, db);

            var webCreate = await writer.CreateRewardCard(rewardscardnew);

            //need to get the IDs back from Revel

            if (webCreate.Equals(0))
            {
                db.RewardsCardNew.Add(rewardscardnew);
                int saveCount = await db.SaveChangesAsync();

                if (saveCount > 0)
                {
                    return saveCount;
                }


            }


            return -1;
        }



        // GET: /RewardCard/Edit/5
        [
        Authorize(Roles = "admin")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RewardsCardNew rewardscardnew = await ((DbSet<RewardsCardNew>)db.RewardsCardNew).FindAsync(id);
            if (rewardscardnew == null)
            {
                return HttpNotFound();
            }

            Session["editedCard"] = rewardscardnew;
            return View(rewardscardnew);
        }

        // POST: /RewardCard/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult> Edit([Bind(Include = "DBKEY_rewardscardnew_id,ResourceUri,created_by,created_date,current_points,customer_revel,establishment,Revelid,number,payment_type,resource_uri,total_points,total_purchases,total_visits,updated_by,updated_date,customer_id,establishment_id,is_vip_card,theAddress,vip_points_refresh")] RewardsCardNew rewardscardnew)
        {
            if (ModelState.IsValid)
            {

                var est = new Establishment(1, "Grind",
           RevelAPIKEY,
            new Uri(RevelBaseURL));

                //if it's a red card, immediately set the points             
                var oldCard = (RewardsCardNew)Session["editedCard"];
                if (oldCard.is_vip_card != true)
                {
                    if (rewardscardnew.is_vip_card == true)
                    {
                        rewardscardnew.current_points = rewardscardnew.vip_points_refresh;
                        rewardscardnew.total_points += rewardscardnew.vip_points_refresh;

                    }
                }

                var writer = new WebserviceDataWriter(est, db);

                var webCreate = await writer.UpdateRewardCard(rewardscardnew);

                if (webCreate.Equals(0))
                {
                    db.Entry(rewardscardnew).State = EntityState.Modified;

                    var ok = await db.SaveChangesAsync();

                    if (ok > 0)
                    {
                        ViewBag.Message = "Card Update Successful!";
                    }
                    else
                    {
                        throw new Exception();
                        //log error

                    }

                }

                if (rewardscardnew.is_vip_card == true)
                {
                    List<RewardsCardNew> cards =  RewardsCardNew.GetRewardCardsNewAndCustomer();
                    return View("Index", cards.Where(x => x.is_vip_card == true).ToList());
                }
                else
                {
                    List<RewardsCardNew> cards =  RewardsCardNew.GetRewardCardsNewAndCustomer();
                    return View("Index", cards.Where(x => x.is_vip_card != true).ToList());
                }


            }
            return View(rewardscardnew);
        }

        // GET: /RewardCard/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RewardsCardNew rewardscardnew = await ((DbSet<RewardsCardNew>)db.RewardsCardNew).FindAsync(id);
            if (rewardscardnew == null)
            {
                return HttpNotFound();
            }
            return View(rewardscardnew);
        }

        // POST: /RewardCard/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            RewardsCardNew rewardscardnew = await ((DbSet<RewardsCardNew>)db.RewardsCardNew).FindAsync(id);
            db.RewardsCardNew.Remove(rewardscardnew);
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
