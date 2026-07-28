using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Models;

namespace Web.Grind._808nd.com.Controllers
{
    public class CustomerAPIController : ApiController
    {
        private GrindContext db = new GrindContext();

        // GET api/CustomerAPI

        public CustomerAPIController()
        {
            db = new GrindContext();
        }
        public IQueryable<Customer> GetCustomers()
        {
            return db.Customers;
        }

        // GET api/CustomerAPI/5
        [ResponseType(typeof(Customer))]
        public async Task<IHttpActionResult> GetCustomer(int id)
        {
            Customer customer = await ((DbSet<Customer>)db.Customers).FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        // PUT api/CustomerAPI/5
        public async Task<IHttpActionResult> PutCustomer(int id, Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customer.DBKEY_customer_id)
            {
                return BadRequest();
            }

            db.Entry(customer).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST api/CustomerAPI
        [ResponseType(typeof(Customer))]
        public async Task<IHttpActionResult> PostCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Customers.Add(customer);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = customer.DBKEY_customer_id }, customer);
        }

        // DELETE api/CustomerAPI/5
        [ResponseType(typeof(Customer))]
        public async Task<IHttpActionResult> DeleteCustomer(int id)
        {
            Customer customer = await ((DbSet<Customer>)db.Customers).FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            db.Customers.Remove(customer);
            await db.SaveChangesAsync();

            return Ok(customer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CustomerExists(int id)
        {
            return db.Customers.Count(e => e.DBKEY_customer_id == id) > 0;
        }
    }
}