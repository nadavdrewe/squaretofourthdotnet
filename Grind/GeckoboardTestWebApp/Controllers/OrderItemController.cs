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
    public class OrderItemController : Controller
    {
        private GrindContext db = new GrindContext();

        // GET: /OrderItem/
        public ActionResult Index()
        {
            return View(db.OrderItems.ToList());
        }

        // GET: /OrderItem/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderItem orderitem = db.OrderItems.Find(id);
            if (orderitem == null)
            {
                return HttpNotFound();
            }
            return View(orderitem);
        }

        // GET: /OrderItem/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /OrderItem/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="id,catering_complete,commission,cost,course_number,created_by,created_date,crv_value,cup_qty,cup_weight,deleted,dining_option,discount,discount_amount,discount_reason,discount_rule_amount,discount_taxed,exchange_discount,exchanged,initial_price,is_cold,is_coupon,is_gift,modifier_amount,modifier_cost,modifieritems,on_hold,order,order_local_id,price,printed,product,product_name_override,quantity,resource_uri,shared,special_request,split_parts,split_type,split_with_seat,station,tax_amount,tax_rate,tax_rebate,taxed_flag,temp_sort,updated_by,updated_date,uuid,voided_reason,weight,total_price_after_tax,total_price_after_discount,parent_order_id,product_id")] OrderItem orderitem)
        {
            if (ModelState.IsValid)
            {
                db.OrderItems.Add(orderitem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(orderitem);
        }

        // GET: /OrderItem/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderItem orderitem = db.OrderItems.Find(id);
            if (orderitem == null)
            {
                return HttpNotFound();
            }
            return View(orderitem);
        }

        // POST: /OrderItem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="id,catering_complete,commission,cost,course_number,created_by,created_date,crv_value,cup_qty,cup_weight,deleted,dining_option,discount,discount_amount,discount_reason,discount_rule_amount,discount_taxed,exchange_discount,exchanged,initial_price,is_cold,is_coupon,is_gift,modifier_amount,modifier_cost,modifieritems,on_hold,order,order_local_id,price,printed,product,product_name_override,quantity,resource_uri,shared,special_request,split_parts,split_type,split_with_seat,station,tax_amount,tax_rate,tax_rebate,taxed_flag,temp_sort,updated_by,updated_date,uuid,voided_reason,weight,total_price_after_tax,total_price_after_discount,parent_order_id,product_id")] OrderItem orderitem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderitem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(orderitem);
        }

        // GET: /OrderItem/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderItem orderitem = db.OrderItems.Find(id);
            if (orderitem == null)
            {
                return HttpNotFound();
            }
            return View(orderitem);
        }

        // POST: /OrderItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OrderItem orderitem = db.OrderItems.Find(id);
            db.OrderItems.Remove(orderitem);
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
