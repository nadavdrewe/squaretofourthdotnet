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
using Revel._808nd.com.Classes.WebserviceReader;
using Revel._808nd.com.Extensions;

namespace web.fourth.revel.com.Controllers
{
    public class ProductsController : Controller
    {
        private RevelContext db = new RevelContext();

        // GET: Products
        public async Task<ActionResult> Index()
        {
            ViewBag.Brands = db.Brands.ToList();
            ViewBag.Establishments = db.Establishments.ToList();

            return View(await db.Products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<ActionResult> Details(int? id)
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

        // GET: Products/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [Bind(
                 Include =
                     "DBKEY_product_id,active,allow_price_override,attribute_type,barcode,brand,category,color_code,commission,cost,created_by,created_date,crv_enabled,deleted,description,dining_options,disable_modifier_popup,display_on_kiosk,display_online,ebt_no,establishment,export,happy_hour,product_id,is_cold,is_combo,is_drink,kitchen_print_name,lock_enable,max_price,name,preparation_time,price,price_embedded,product_weight_unit,productclass,resource_uri,rti_combo,sku,sold_by_weight,sorting,tare,tax,tax_class,tax_included,updated_by,updated_date,uuid,variable_pricing,variable_pricing_by,establishment_id,productclass_id,tax_id,brand_id,categoryID,theAddress"
             )] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<ActionResult> Edit(int? id)
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

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            [Bind(
                 Include =
                     "DBKEY_product_id,active,allow_price_override,attribute_type,barcode,brand,category,color_code,commission,cost,created_by,created_date,crv_enabled,deleted,description,dining_options,disable_modifier_popup,display_on_kiosk,display_online,ebt_no,establishment,export,happy_hour,product_id,is_cold,is_combo,is_drink,kitchen_print_name,lock_enable,max_price,name,preparation_time,price,price_embedded,product_weight_unit,productclass,resource_uri,rti_combo,sku,sold_by_weight,sorting,tare,tax,tax_class,tax_included,updated_by,updated_date,uuid,variable_pricing,variable_pricing_by,establishment_id,productclass_id,tax_id,brand_id,categoryID,theAddress"
             )] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<ActionResult> Delete(int? id)
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

        public async Task RefreshProductsByBrand()
        {



            var brand = db.Brands.FirstOrDefault();
            var ests = db.Establishments.Where(x => x.db_brand_id == brand.brand_id).ToList();

            var br = new Establishment(0, "", brand.key_secret, new Uri(brand.revel_base_url));
            var products = new List<Product>();
            using (var reader = new RevelWebserviceDataReader(br))
            {

                var query = "/resources/Product/?format=json&limit=800";

                var instanceProduct = new Product();

                try
                {
                    await Console.Out.WriteLineAsync("Executing....");
                    products = await reader.GetRevelWebserviceData<Product>(instanceProduct, query);
                }
                catch (Exception e)
                {

                    throw new Exception("Could not get data from Revel", e);
                }

            }

            var oldProds = new List<Product>();
            foreach (var est in ests)
            {
                oldProds.AddRange(db.Products.Where(x => x.db_establishment_id == est.DBKEY_establishment_id));
            }
                       

            try
            {
                //delete all old products and wholesale add in new products
                db.Products.RemoveRange(oldProds);
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw new Exception("Couldnt' delete old products");
            }

            try
            {
                foreach (var item in products)
                {
                    try
                    {
                        var whichEst = ests.FirstOrDefault(y => y.resource_uri == item.establishment);

                        item.brand_id = brand.brand_id;
                        item.db_brand_id = brand.brand_id;
                        item.db_establishment_id = whichEst.DBKEY_establishment_id;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("EXCEPTION: " + ex.Message);
                        //keep going 
                        //throw ex;
                    }

                }


                db.Products.AddRange(products);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                throw new Exception("Couldn't add new products");
            }

            //end


            /////////////////OLD CODE

            /* var productsToAdd = new List<Product>();
            if (products.Count > 0) //make sure there's some new ones before deleting the old ones.
            {               

                foreach (var prod in products)
                {
                    //there isn't that product already in existence - add it

                    if (prod.product_id == 2202)
                    {
                        
                    }

                    try
                    {
                        if (
                                     oldProds.Where(x => x.name == prod.name && x.resource_uri == prod.resource_uri)
                                         .ToList()
                                         .Count()
                                         .Equals(0))
                        {
                            prod.db_brand_id = brand.brand_id;
                            prod.db_establishment_id =
                                ests.First(x => x.resource_uri == prod.establishment).DBKEY_establishment_id;

                            productsToAdd.Add(prod);
                        }
                        else
                        {
                            var existingProd =
                                oldProds.Where(x => x.name == prod.name && x.resource_uri == prod.resource_uri)
                                    .FirstOrDefault();

                            if (existingProd != null)
                            {
                                if (existingProd.sku != prod.sku)
                                {
                                    existingProd.sku = prod.sku;
                                    db.Products.Attach(existingProd);
                                    var entry = db.Entry(existingProd);
                                    entry.Property(x => x.sku).IsModified = true;
                                    db.SaveChanges();
                                }
                            }
                            else
                            {
                                var weHaveaProblem = "";
                                throw new Exception();
                            }

                        }
                    }
                    catch (Exception ex)
                    {

                        throw new Exception("Couldn't refresh products", ex);
                    }
                }

                if (productsToAdd.Count > 0)
                {
                    db.Products.AddRange(productsToAdd);
                    await db.SaveChangesAsync();
                    return productsToAdd.Count();
                }

            }*/


        }


        public async Task RefreshProductForChosenBrandsByEstablishment()
        {
            var brands = db.Brands.ToList();

            foreach (var brand in brands.Where(x => x.brand_id != 6 && x.brand_id != 7 && x.brand_id != 11))
            {
                var ests = db.Establishments.Where(x => x.db_brand_id == brand.brand_id).ToList();


                foreach (var establishment in ests)
                {

                    var result = await RefreshProductsByEstablishment(establishment.establishment_id);


                }


            }
        }



        public async Task<int> RefreshProductsByEstablishment(int estId)
        {

            var est = db.Establishments.Find(estId);

            //delete previous
            //db.Products.RemoveRange(db.Products.Where(x => x.db_establishment_id == est.DBKEY_establishment_id).ToList());
            //db.SaveChanges();


            var brand = db.Brands.Where(x => x.resource_uri == est.brand).First();

            var br = new Establishment(0, "", brand.key_secret, new Uri(brand.revel_base_url));

            var products = new List<Product>();

            using (var reader = new RevelWebserviceDataReader(br))
            {
                var estIdSplit = est.resource_uri.Split('/')[3];
                var query = "/resources/Product/?format=json&limit=600&establishment=" + estIdSplit;

                var instanceProduct = new Product();
                products = await reader.GetRevelWebserviceData<Product>(instanceProduct, query);
            }


            var productsToAdd = new List<Product>();
            if (products.Count > 0)
            {
                var currentProds = db.Products.Where(x => x.establishment == est.resource_uri).ToList();

                foreach (var prod in products)
                {
                    //there isn't that brand already in existence
                    if (currentProds.Where(x => x.name == prod.name && x.resource_uri == prod.resource_uri).ToList().Count().Equals(0))
                    {
                        productsToAdd.Add(prod);
                    }
                }

                if (productsToAdd.Count > 0)
                {
                    db.Products.AddRange(productsToAdd);
                    await db.SaveChangesAsync();
                    return productsToAdd.Count();
                }

            }

            return 0;
        }


        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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
