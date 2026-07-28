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
    public class BrandsController : Controller
    {
        private RevelContext db = new RevelContext();

        // GET: Brands
        public async Task<ActionResult> Index()
        {
            return View(await db.Brands.ToListAsync());
        }

        // GET: Brands/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Brand brand = db.Brands.Find(id);
            if (brand == null)
            {
                return HttpNotFound();
            }
            return View(brand);
        }

        // GET: Brands/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Brands/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> Create(Brand brand)
        {
            //try and make a webservice call to the endpoint in question
            try
            {
                var message = "That worked, great!";
                var br = new Establishment(0, "", brand.key_secret, new Uri(brand.revel_base_url));
                var brands = new List<Brand>();

                using (var reader = new RevelWebserviceDataReader(br))
                {
                    var instanceBrand = new Brand();
                    brands = await reader.GetRevelWebserviceData<Brand>(instanceBrand, instanceBrand.theAddress);

                }

                var brandsToAdd = new List<Brand>();
                if (brands.Count > 0)
                {
                    var existingBrands = db.Brands.Where(x => x.revel_base_url == brand.revel_base_url).ToList();

                    foreach (var b in brands)
                    {
                        //there isn't that brand already in existence
                        if (existingBrands.Where(x => x.name == b.name && x.resource_uri == b.resource_uri).ToList().Count().Equals(0))
                        {
                            b.revel_base_url = brand.revel_base_url;
                            b.key_secret = brand.key_secret;
                            b.is_fourth_active = true;
                            b.fourth_locationID = "1";
                            b.fourth_RevenueCenter = "1";
                            b.fourth_guid = brand.fourth_guid;
                            b.fourth_password = brand.fourth_password;
                            b.fourth_username = brand.fourth_username;
                        
                            brandsToAdd.Add(b);
                        }
                        else
                        {

                            brands.First().revel_base_url = brand.revel_base_url;
                            brands.First().key_secret = brand.key_secret;
                            brands.First().is_fourth_active = true;
                            brands.First().fourth_locationID = "1";
                            brands.First().fourth_RevenueCenter = "1";
                            brands.First().fourth_guid = brand.fourth_guid;
                            brands.First().fourth_password = brand.fourth_password;
                            brands.First().fourth_username = brand.fourth_username;
           

                            brandsToAdd = brands;
                        }
                    }


                    if (brandsToAdd.Count > 0)
                    {
                        db.Brands.AddRange(brandsToAdd);
                        await db.SaveChangesAsync();
                        return message;
                    }

                }

                return "Unable to find anything in Revel - check your API key is correct!";
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }

        // GET: Brands/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Brand brand = db.Brands.Find(id);
            if (brand == null)
            {
                return HttpNotFound();
            }
            return View(brand);
        }

        // POST: Brands/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Brand brand)
        {
            var existingBrand = db.Brands.First(x => x.brand_id == brand.brand_id);
            existingBrand.key_secret = brand.key_secret;
            existingBrand.revel_base_url = brand.revel_base_url;
            existingBrand.is_fourth_active = brand.is_fourth_active;
            //fourth fields

            existingBrand.fourth_password = brand.fourth_password;
            existingBrand.fourth_username = brand.fourth_username;
            existingBrand.fourth_guid = brand.fourth_guid;
            existingBrand.fourth_locationID = brand.fourth_locationID;
            existingBrand.fourth_PushByEstablishment = brand.fourth_PushByEstablishment;
            existingBrand.fourth_RevenueCenter = brand.fourth_RevenueCenter;

            db.Entry(existingBrand).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");

            return View(brand);
        }

        // GET: Brands/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Brand brand = db.Brands.Find(id);
            if (brand == null)
            {
                return HttpNotFound();
            }
            return View(brand);
        }

        // POST: Brands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Brand brand = db.Brands.Find(id);
            db.Brands.Remove(brand);
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
