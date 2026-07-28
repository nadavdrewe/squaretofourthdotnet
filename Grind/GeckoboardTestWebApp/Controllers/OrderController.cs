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

namespace GeckoboardTestWebApp.Controllers
{
    public class OrderController : Controller
    {
        private GrindContext db = new GrindContext();

        // GET: /Order/
        public ActionResult Index()
        {
            return View(db.Orders.ToList());
        }

        // GET: /Order/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: /Order/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Order/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="id,asap,auto_grat_pct,bill_number,bills_info,bills_type,call_name,closed,created_at,created_by,created_date,crv_taxed,crv_value,dining_option,discount,discount_amount,discount_reason,discount_tax_amount,establishment,exchange_discount,exchanged,final_total,gift_reward_data,gratuity,gratuity_type,has_delivery_info,local_id,notes,notification_email_sent,notification_text_sent,number_of_people,points_added,points_redeemed,pos_mode,prevailing_surcharge,prevailing_tax,printed,remaining_due,resource_uri,rounding_delta,service_charge,subtotal,surcharge,tax,tax_country,tax_rebate,updated_by,updated_date,uuid,web_order,establishment_id")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(order);
        }

        // GET: /Order/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: /Order/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="id,asap,auto_grat_pct,bill_number,bills_info,bills_type,call_name,closed,created_at,created_by,created_date,crv_taxed,crv_value,dining_option,discount,discount_amount,discount_reason,discount_tax_amount,establishment,exchange_discount,exchanged,final_total,gift_reward_data,gratuity,gratuity_type,has_delivery_info,local_id,notes,notification_email_sent,notification_text_sent,number_of_people,points_added,points_redeemed,pos_mode,prevailing_surcharge,prevailing_tax,printed,remaining_due,resource_uri,rounding_delta,service_charge,subtotal,surcharge,tax,tax_country,tax_rebate,updated_by,updated_date,uuid,web_order,establishment_id")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(order);
        }

        // GET: /Order/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: /Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
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
