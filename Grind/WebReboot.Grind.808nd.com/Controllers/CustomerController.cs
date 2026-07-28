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
using Revel._808nd.com.Classes.WebserviceReader;
using StructureMap;


namespace Web.Grind._808nd.com.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private GrindContext db;
        private string RevelAPIKEY { get; } = ConfigurationManager.AppSettings["RevelAPIKEY"];
        private string RevelBaseURL { get; } = ConfigurationManager.AppSettings["RevelBaseURL"];

        public CustomerController()
        {
       
            db = new GrindContext();
        }

        public CustomerController(GrindContext grind)
        {
            db = grind;
        }

        // GET: /Customer/
        [Authorize]
        public async Task<ActionResult> Index()
        {
            return View(await db.Customers.ToListAsync());
        }

        // GET: /Customer/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await ((DbSet<Customer>)db.Customers).FindAsync(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: /Customer/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Customer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "DBKEY_customer_id,Active,Address,BirthDate,CcExp,CcFirstName,CcLast4Digits,CcLastName,CcNumber,City,CreatedBy,CreatedDate,Email,ExpDate,FirstName,RevelId,IsVisitor,LastName,LicNumber,LoyaltyNumber,LoyaltyRefId,Notes,PhoneNumber,Picture,RefNumber,ResourceUri,State,TotalPurchases,TotalVisits,UpdatedBy,UpdatedDate,Uuid,Zipcode,customer_id,establishment_id,theAddress")] Customer customer)
        {
            var est = new Establishment(1, "Grind",
                        RevelAPIKEY,
                        new Uri(RevelBaseURL));

            customer.LoyaltyNumber = customer.LicNumber;
            customer.RefNumber = customer.LicNumber;



            //create in Revel
            var writer = new WebserviceDataWriter(est, db);

            var webCreate = await writer.CreateCustomer(customer);

            if (webCreate.Equals(0))
            {


                var webReader = new RevelWebserviceDataReader(est);

                var createdCustomer = await webReader.GetRevelWebserviceItem(new Customer(), customer.ResourceUri);
                customer.Uuid = createdCustomer.Uuid;

                db.Customers.Add(customer);
                var saveCount = await db.SaveChangesAsync();

                if (saveCount > 0)
                {
                    //successfull!
                    ViewBag.Message = "Customer Create Successful!" + " Customer:" + customer.FirstName + " " + customer.LastName;
                    return RedirectToAction("Create");
                }
                else
                {
                    throw new Exception();
                }
            }



            return View(customer);
        }

        // GET: /Customer/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await ((DbSet<Customer>)db.Customers).FindAsync(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: /Customer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "DBKEY_customer_id,Active,Address,BirthDate,CcExp,CcFirstName,CcLast4Digits,CcLastName,CcNumber,City,CreatedBy,CreatedDate,Email,ExpDate,FirstName,RevelId,IsVisitor,LastName,LicNumber,LoyaltyNumber,LoyaltyRefId,Notes,PhoneNumber,Picture,RefNumber,ResourceUri,State,TotalPurchases,TotalVisits,UpdatedBy,UpdatedDate,Uuid,Zipcode,customer_id,establishment_id,theAddress")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                var est = new Establishment(1, "Grind",
                    RevelAPIKEY,
                    new Uri(RevelBaseURL));

                var writer = new WebserviceDataWriter(est, db);

                var webCreate = await writer.UpdateCustomer(customer);

                if (webCreate.Equals(0))
                {
                    db.Customers.Attach(customer);
                    db.Entry(customer).State = EntityState.Modified;
                    var ok = await db.SaveChangesAsync();

                    if (ok > 0)
                    {
                        ViewBag.Message = "Customer update successful!";
                        return View("Index", await db.Customers.ToListAsync());
                    }
                    else
                    {
                        throw new Exception();
                    }

                }

            }



            return View(customer);
        }

        // GET: /Customer/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await ((DbSet<Customer>)db.Customers).FindAsync(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: /Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Customer customer = await ((DbSet<Customer>)db.Customers).FindAsync(id);
            db.Customers.Remove(customer);
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
