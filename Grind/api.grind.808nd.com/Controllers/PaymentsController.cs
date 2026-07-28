using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.PaymentSummaries;
using Revel._808nd.com.Models;

namespace api.grind._808nd.com.Controllers
{
    public class PaymentsController : ApiController
    {

        public PaymentsController()
        {
            _existingEstablishmentIds = db.Establishments.Select(x => x.establishment_id).ToList();
        }
        private GrindContext db = new GrindContext();
        private List<int> _existingEstablishmentIds;
        // GET: api/Payments

           
        public IQueryable<Payment> GetPayments()
        {
            return db.Payments;
        }

        // GET: api/Payments/5
        [ResponseType(typeof(Payment))]
        public async Task<IHttpActionResult> GetPayment(int id)
        {
            var dbSet = (DbSet<Payment>) db.Payments;
            Payment payment = await dbSet.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            return Ok(payment);
        }


        // GET: api/Payments/5        
        [ResponseType(typeof(PaymentSummaryGroup))]
        public async Task<IHttpActionResult> GetPayments(DateTime start, int howLongUnits, PaymentSummaryGroupRequestType requestType, List<int> requestedEstablishments = null)
        {
            var adjustedStart = new DateTime(start.Year, start.Month, start.Day, 03, 00, 00);
            DateTime adjustedEnd;
            
            switch (requestType)
            {

                case PaymentSummaryGroupRequestType.Hours:
                    adjustedEnd = adjustedStart.AddHours(howLongUnits);
                    break;
                case PaymentSummaryGroupRequestType.Days:
                    adjustedEnd = adjustedStart.AddDays(howLongUnits);
                    break;
                case PaymentSummaryGroupRequestType.Weeks:
                    adjustedEnd = adjustedStart.AddDays(howLongUnits * 7);
                    break;
                case PaymentSummaryGroupRequestType.Months:
                    adjustedEnd = adjustedStart.AddMonths(howLongUnits);
                    break;
                default:
                    return BadRequest("Incorrect unit type");

            }


            if (requestedEstablishments != null)
            {
                foreach (var existing in requestedEstablishments)
                {
                    foreach (var estId in _existingEstablishmentIds)
                    {
                        if (existing == estId)
                        {
                            break;

                        }
                        return BadRequest("That establishmentId doesn't exist");
                    }
                }
            }

            if (adjustedEnd <= adjustedStart)
            {
                return BadRequest("Your dates are wrong, end is after start");
            }

            

            //parse to 'Revel Time'
            

            try
            {

                PaymentSummaryGroupFactory factory = new PaymentSummaryGroupFactory(db);
                var paymentGroup = factory.Create(adjustedStart, adjustedEnd, requestType, requestedEstablishments);
                //turn into ViewModel


                return Ok(paymentGroup);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);                
            }
        }

        // PUT: api/Payments/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPayment(int id, Payment payment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != payment.DBKEY_payment_id)
            {
                return BadRequest();
            }

            db.Entry(payment).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentExists(id))
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

        // POST: api/Payments
        [ResponseType(typeof(Payment))]
        public async Task<IHttpActionResult> PostPayment(Payment payment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Payments.Add(payment);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = payment.DBKEY_payment_id }, payment);
        }

        // DELETE: api/Payments/5
        [ResponseType(typeof(Payment))]
        public async Task<IHttpActionResult> DeletePayment(int id)
        {
            var dbSet = (DbSet<Payment>)db.Payments;
            Payment payment = await dbSet.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            db.Payments.Remove(payment);
            await db.SaveChangesAsync();

            return Ok(payment);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PaymentExists(int id)
        {
            return db.Payments.Count(e => e.DBKEY_payment_id == id) > 0;
        }
    }
}