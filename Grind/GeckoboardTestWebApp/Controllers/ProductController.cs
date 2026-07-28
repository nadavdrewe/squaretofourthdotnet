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
    public class ProductController : Controller
    {
        private GrindContext db = new GrindContext();

        // GET: /Product/
        public ActionResult Index()
        {
            return View(db.Products.ToList());
        }

        // GET: /Product/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: /Product/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Product/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="id,active,allow_price_override,attribute_type,barcode,brand,category,color_code,commission,cost,created_by,created_date,crv_enabled,deleted,description,dining_options,disable_modifier_popup,display_on_kiosk,display_online,ebt_no,establishment,export,happy_hour,is_cold,is_combo,is_drink,kitchen_print_name,lock_enable,max_price,name,preparation_time,price,price_embedded,product_weight_unit,productclass,resource_uri,rti_combo,sku,sold_by_weight,sorting,tare,tax,tax_class,tax_included,updated_by,updated_date,uuid,variable_pricing,variable_pricing_by,establishment_id,productclass_id,tax_id,brand_id")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: /Product/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: /Product/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="id,active,allow_price_override,attribute_type,barcode,brand,category,color_code,commission,cost,created_by,created_date,crv_enabled,deleted,description,dining_options,disable_modifier_popup,display_on_kiosk,display_online,ebt_no,establishment,export,happy_hour,is_cold,is_combo,is_drink,kitchen_print_name,lock_enable,max_price,name,preparation_time,price,price_embedded,product_weight_unit,productclass,resource_uri,rti_combo,sku,sold_by_weight,sorting,tare,tax,tax_class,tax_included,updated_by,updated_date,uuid,variable_pricing,variable_pricing_by,establishment_id,productclass_id,tax_id,brand_id")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: /Product/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: /Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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
