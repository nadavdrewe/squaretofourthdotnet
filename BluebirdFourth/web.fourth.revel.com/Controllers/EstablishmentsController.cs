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
    public class EstablishmentsController : Controller
    {
        private RevelContext db = new RevelContext();

        // GET: Establishments
        public async Task<ActionResult> Index()
        {
            return View(await db.Establishments.ToListAsync());
        }

        // GET: Establishments/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Establishment establishment =  db.Establishments.Find(id);
            if (establishment == null)
            {
                return HttpNotFound();
            }
            return View(establishment);
        }

        // GET: Establishments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Establishments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create([Bind(Include = "DBKEY_establishment_id,establishment_id,is_fourth_active,theAddress,address,brand,email,name,resource_uri,location_email,time_zone,effective_from,id,RevelOrganiationName")] Establishment establishment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Establishments.Add(establishment);
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }

        //    return View(establishment);
        //}

       /* [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int BrandId)
        {
            var brand = db.Brands.Find(BrandId);

            var br = new Establishment(0, "", brand.key_secret, new Uri(brand.revel_base_url));

            var establishments = new List<Establishment>();
            using (var reader = new RevelWebserviceDataReader(br))
            {
                var instanceEstablishment = new Establishment();
                establishments = await reader.GetRevelWebserviceData<Establishment>(instanceEstablishment, instanceEstablishment.theAddress);
            }


            var establishmentsToAdd = new List<Establishment>();
            if (establishments.Count > 0)
            {
                var currentEsts = db.Establishments.Where(x => x.brand == brand.revel_base_url).ToList();

                foreach (var est in establishments)
                {
                    //there isn't that brand already in existence
                    if (
                        currentEsts.Where(x => x.name == est.name && x.resource_uri == est.resource_uri)
                            .ToList()
                            .Count()
                            .Equals(0))

                        est.db_brand_id = brand.brand_id;
                    establishmentsToAdd.Add(est);
                }

                if (establishmentsToAdd.Count > 0)
                {
                    db.Establishments.AddRange(establishmentsToAdd);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }

            }
            return RedirectToAction("Index");
        }
*/

        // GET: Establishments/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Establishment establishment =  db.Establishments.Find(id);
            if (establishment == null)
            {
                return HttpNotFound();
            }
            return View(establishment);
        }

        // POST: Establishments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "DBKEY_establishment_id,establishment_id,is_fourth_active,theAddress,address,brand,email,name,resource_uri,location_email,time_zone,effective_from,id,RevelOrganiationName")] Establishment establishment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(establishment).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(establishment);
        }

        // GET: Establishments/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Establishment establishment =  db.Establishments.Find(id);
            if (establishment == null)
            {
                return HttpNotFound();
            }
            return View(establishment);
        }

        // POST: Establishments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Establishment establishment =  db.Establishments.Find(id);
            db.Establishments.Remove(establishment);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<int> RefreshEstablishments()
        {


            var brand = db.Brands.First();

            var existingEstablishments = db.Establishments.Where(x => x.db_brand_id == brand.brand_id).ToList();

           
            var br = new Establishment(0, "", brand.key_secret, new Uri(brand.revel_base_url));

            var establishments = new List<Establishment>();
            using (var reader = new RevelWebserviceDataReader(br))
            {
                var instanceEstablishment = new Establishment();
                establishments = await reader.GetRevelWebserviceData<Establishment>(instanceEstablishment, instanceEstablishment.theAddress);
            }


            var establishmentsToAdd = new List<Establishment>();
            if (establishments.Count > 0)
            {
                var currentEsts = existingEstablishments;

                foreach (var est in establishments)
                {
                    //there isn't that establishment already in existence
                    if (currentEsts.Where(x => x.establishment_id == est.establishment_id && x.resource_uri == est.resource_uri).ToList().Count().Equals(0))
                    {
                        est.db_brand_id = brand.brand_id;
                        est.brand = brand.revel_base_url; //this is needed to map an establishment to a brand
                        est.is_fourth_active = true;
                        establishmentsToAdd.Add(est);
                    }
                }

                if (establishmentsToAdd.Count > 0)
                {
                    db.Establishments.AddRange(establishmentsToAdd);
                    await db.SaveChangesAsync();
                    return establishmentsToAdd.Count();
                }


            }

            return 0;
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
