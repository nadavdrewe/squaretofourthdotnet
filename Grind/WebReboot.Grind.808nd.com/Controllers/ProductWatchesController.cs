using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using Geckoboard._808nd.com;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Models;
using Revel._808nd.com.ReportingModel;
using WebReboot.Grind._808nd.com.Models.ViewModels;

namespace WebReboot.Grind._808nd.com.Controllers
{
    [System.Web.Mvc.Authorize]
    public class ProductWatchesController : Controller
    {
        private GrindContext db = new GrindContext();

        // GET: ProductWatches

        public async Task<ActionResult> GetProductsForEstablishment(int EstablishmentID)
        {

            db.Establishments.ToList();

            var cats = db.ProductCategories.ToList();
            var prods = new List<Product>();
            var ests = db.Establishments.ToList();

            ViewBag.Establishments = ests;

            if (EstablishmentID == 2)
            {
                prods = db.Products.ToList();

            }
            else
            {
                prods = db.Products.Where(x => x.establishment_id.Equals(EstablishmentID)).ToList();

            }

            var listToReturn = new List<ProductCategoryAndEstablishmentViewModel>();

            foreach (var product in prods.Where(x => x.active == "True"))
            {
                var thisCat = cats.FirstOrDefault(x => x.productcategory_id == product.categoryID);
                if (thisCat == null)
                {
                    thisCat = new ProductCategory
                    {
                        name = "No category identified"
                    };

                }

                var thisEst = ests.FirstOrDefault(x => x.establishment_id == product.establishment_id);
                if (thisEst == null)
                {
                    thisEst = new Establishment
                    {
                        name = "No category identified"
                    };

                }


                listToReturn.Add(new ProductCategoryAndEstablishmentViewModel
                {
                    Product = product,
                    CategoryName = thisCat.name
                    ,
                    EstablishmentName = thisEst.name
                });
            }


            return PartialView("ListOfProductsForPotentialWatch", listToReturn.OrderBy(x => x.EstablishmentName).ThenBy(x => x.CategoryName).ThenBy(x => x.Product.name));
        }

        public ActionResult Index()
        {
            ViewBag.Establishments =
                db.Establishments.ToList();

            ViewBag.Products = db.Products.OrderBy(x => x.establishment_id).ThenBy(x => x.name).ToList();


            return View(db.ProductWatches.ToList());

        }

        // GET: ProductWatches/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductWatch productWatch = db.ProductWatches.Find(id);
            if (productWatch == null)
            {
                return HttpNotFound();
            }
            return View(productWatch);
        }

        // GET: ProductWatches/Create
        [System.Web.Mvc.HttpGet]
        public ActionResult Create()
        {
            ViewBag.EstablishmentsSelect =
           db.Establishments
           .Where(x => x.establishment_id != 2)
           .Where(x => x.establishment_id != 9)
           .ToList().Select(x => new SelectListItem { Value = x.id, Text = x.name }).ToList();
            return View();
        }

        // POST: ProductWatches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> Create(int ProductID)
        {
            ViewBag.EstablishmentsSelect =
          db.Establishments
          .Where(x => x.establishment_id != 2)
          .Where(x => x.establishment_id != 9)
          .ToList().Select(x => new SelectListItem { Value = x.id, Text = x.name }).ToList();

            var newWatch = new ProductWatch
            {
                Revel_Product_Id = ProductID,
            };

            if (ModelState.IsValid)
            {
                db.ProductWatches.Add(newWatch);
                db.SaveChanges();
                return new HttpStatusCodeResult(HttpStatusCode.Created);
            }

            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
        }

        // GET: ProductWatches/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductWatch productWatch = db.ProductWatches.Find(id);
            if (productWatch == null)
            {
                return HttpNotFound();
            }
            return View(productWatch);
        }

        // POST: ProductWatches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Revel_Product_Id")] ProductWatch productWatch)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productWatch).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(productWatch);
        }

        // GET: ProductWatches/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductWatch productWatch = db.ProductWatches.Find(id);
            if (productWatch == null)
            {
                return HttpNotFound();
            }
            return View(productWatch);
        }

        // POST: ProductWatches/Delete/5
        [System.Web.Mvc.HttpPost, System.Web.Mvc.ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductWatch productWatch = db.ProductWatches.Find(id);
            db.ProductWatches.Remove(productWatch);
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
