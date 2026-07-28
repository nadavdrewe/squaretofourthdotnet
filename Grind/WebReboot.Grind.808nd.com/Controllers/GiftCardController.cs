using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Models;
using WebReboot.Grind._808nd.com.Controllers;
using WebReboot.Grind._808nd.com.DatatablesAjax;

namespace Web.Grind._808nd.com.Controllers
{
    [Authorize]
    public class GiftCardController : Controller
    {
        private GrindContext db = new GrindContext();
        private const string _cacheCollectionName = "giftCards";

        private List<GiftCard> allCards = HttpRuntime.Cache.Get(_cacheCollectionName) as List<GiftCard>;
        private string RevelAPIKEY { get; } = ConfigurationManager.AppSettings["RevelAPIKEY"];
        private string RevelBaseURL { get; } = ConfigurationManager.AppSettings["RevelBaseURL"];
        private string RevelCardInsertUser { get; } = ConfigurationManager.AppSettings["RevelCardInsertUser"];

        public GiftCardController()
        {
            db = new GrindContext();
        }


        public ActionResult AjaxHandler(jQueryDataTableParamModel param, string sSearch)
        {

            List<GiftCard> cards = allCards;



            var cardsFiltered = cards;

            if (!String.IsNullOrEmpty(param.sSearch))
            {
                cardsFiltered = cards
                    .Where(x => x.number.Contains(param.sSearch)

                                || x.created_date.ToString().Contains(param.sSearch)
                                || x.id.ToString().Contains(param.sSearch)
                    ).ToList();
            }



            try
            {


                var cardsToReturn = cardsFiltered.OrderByDescending(x => x.created_date).Select(card => new[]
                  {
                     card.number.ToString() ?? String.Empty,
                    card.created_by.ToString() ?? String.Empty,
                    ((DateTime?) card.created_date).ToString() ?? String.Empty,
                    card.customer?.ToString() ?? String.Empty,
               /*     card.establishment?.ToString() ?? String.Empty,*/
                    card.id.ToString() ?? String.Empty,
                    card.initial_value.ToString() ?? String.Empty,
                    card.remaining_balance.ToString() ?? String.Empty,
                    card.resource_uri.ToString() ?? String.Empty,
                    card.updated_by.ToString() ?? String.Empty,
                    card.updated_date.ToString() ?? String.Empty,
                    card.LinkingRevelCustomerID.ToString() ?? String.Empty,
                    card.LinkingRevelRewardsCardNewID.ToString() ?? String.Empty,
                    "<a href='Edit/" + card.giftcard_id + "'>Edit</a>"
                }).Skip(param.iDisplayStart)
                    .Take(param.iDisplayLength).ToArray();

                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = cards.Count(),
                    iTotalDisplayRecords = cardsFiltered.Count(),
                    aaData = cardsToReturn
                },
                    JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {

                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = cards.Count(),
                    iTotalDisplayRecords = cardsFiltered.Count(),
                    aaData = new List<GiftCard>()
                },
                      JsonRequestBehavior.AllowGet);
            }
        }





        // GET: /GiftCard/
        public async Task<ActionResult> Index()
        {
            return View(allCards);
        }

        // GET: /GiftCard/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GiftCard giftcard = await ((DbSet<GiftCard>)db.GiftCards).FindAsync(id);
            if (giftcard == null)
            {
                return HttpNotFound();
            }
            return View(giftcard);
        }

        // GET: /GiftCard/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /GiftCard/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "giftcard_id,address,created_by,created_date,customer,establishment,id,initial_value,number,payment_type,remaining_balance,resource_uri,updated_by,updated_date")] GiftCard giftcard)
        {
            var est = new Establishment(1, "Grind",
                         RevelAPIKEY,
                         new Uri(RevelBaseURL));

            //assign params
            giftcard.remaining_balance = giftcard.initial_value;
            giftcard.payment_type = 4;
            giftcard.created_date = DateTime.Now;
            giftcard.updated_date = DateTime.Now;
            giftcard.created_by = RevelCardInsertUser;
            giftcard.updated_by = RevelCardInsertUser;


            //create in Revel
            var writer = new WebserviceDataWriter(est, db);

            var webCreate = await writer.CreateGiftCard(giftcard);

            //need to get the IDs back from Revel

            if (webCreate.Equals(0))
            {
                db.GiftCards.Add(giftcard);
                int saveCount = await db.SaveChangesAsync();

                if (saveCount > 0)
                {

                    ViewBag.Message = "Card Create Successful!";

                    //successfull!
                    return RedirectToAction("Create");
                }
                else
                {
                    throw new Exception();
                }

            }

            return View(giftcard);
        }

        // GET: /GiftCard/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GiftCard giftcard = await ((DbSet<GiftCard>)db.GiftCards).FindAsync(id);
            if (giftcard == null)
            {
                return HttpNotFound();
            }
            return View(giftcard);
        }

        // POST: /GiftCard/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "giftcard_id,address,created_by,created_date,customer,establishment,id,initial_value,number,payment_type,remaining_balance,resource_uri,updated_by,updated_date")] GiftCard giftcard)
        {
            var est = new Establishment(1, "Grind",
                        RevelAPIKEY,
                        new Uri(RevelBaseURL));

            var writer = new WebserviceDataWriter(est, db);

            var webCreate = await writer.UpdateGiftCard(giftcard);

            if (webCreate.Equals(0))
            {
                db.Entry(giftcard).State = EntityState.Modified;

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
            return View(giftcard);
        }

        // GET: /GiftCard/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GiftCard giftcard = await ((DbSet<GiftCard>)db.GiftCards).FindAsync(id);
            if (giftcard == null)
            {
                return HttpNotFound();
            }
            return View(giftcard);
        }

        // POST: /GiftCard/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            GiftCard giftcard = await ((DbSet<GiftCard>)db.GiftCards).FindAsync(id);
            db.GiftCards.Remove(giftcard);
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
