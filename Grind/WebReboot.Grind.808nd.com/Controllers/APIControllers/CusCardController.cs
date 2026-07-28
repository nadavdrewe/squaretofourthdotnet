using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Ajax.Utilities;
using Revel._808nd.com.Classes;

using Revel._808nd.com.Models;
using Revel._808nd.com.Models.ViewModels;
using Revel._808nd.com.Classes.WebserviceReader;

namespace Web.Grind._808nd.com.Controllers.APIControllers
{
    public class CusCardController : ApiController
    {
        private GrindContext db = new GrindContext();
        private string RevelAPIKEY { get; } = ConfigurationManager.AppSettings["RevelAPIKEY"];
        private string RevelBaseURL { get; } = ConfigurationManager.AppSettings["RevelBaseURL"];
        private string RevelCardInsertUser { get; } = ConfigurationManager.AppSettings["RevelCardInsertUser"];



        // GET api/CusCard
        /*    public IQueryable<RewardsCardNew> GetRewardsCardNew()
            {
                return db.RewardsCardNew;
            }

            // GET api/CusCard/5
            [ResponseType(typeof(RewardsCardNew))]
            public async Task<IHttpActionResult> GetRewardsCardNew(int id)
            {
                RewardsCardNew rewardscardnew = await db.RewardsCardNew.FindAsync(id);
                if (rewardscardnew == null)
                {
                    return NotFound();
                }

                return Ok(rewardscardnew);
            }

            // PUT api/CusCard/5
            public async Task<IHttpActionResult> PutRewardsCardNew(int id, RewardsCardNew rewardscardnew)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != rewardscardnew.DBKEY_rewardscardnew_id)
                {
                    return BadRequest();
                }

                db.Entry(rewardscardnew).State = EntityState.Modified;

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RewardsCardNewExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return StatusCode(HttpStatusCode.NoContent);
            }*/

        // POST api/CusCard
        [ResponseType(typeof(string))]
        public async Task<HttpResponseMessage> PostRewardsCardNew(CustomerCardViewModel model)
        {
            var est = new Establishment(1, "Grind",
                       RevelAPIKEY,
                       new Uri(RevelBaseURL));

            //checking use input
            if (!model.number.IsNullOrWhiteSpace())
            {

                var writer = new WebserviceDataWriter(est, db);

                //create card
                var card = new RewardsCardNew
                {
                    number = model.number,
                    created_by = RevelCardInsertUser,
                    created_date = DateTime.Now,
                    current_points = 0,
                    total_points = 0,
                    total_purchases = 0,
                    total_visits = 0,
                    is_vip_card = false,
                    establishment = null,
                    updated_by = RevelCardInsertUser,
                    updated_date = DateTime.Now,
                    payment_type = 4,
                    vip_points_refresh = 0,

                };

                //create customer
                var customer = new Customer
                {
                    UpdatedBy = RevelCardInsertUser,
                    CreatedBy = RevelCardInsertUser,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    FirstName = model.firstname,
                    LastName = model.lastname,
                    Active = true,
                    Email = model.email,
                    LicNumber = card.number,
                    LoyaltyNumber = card.number,
                    RefNumber = card.number,
                    Address = "n/a",
                    BirthDate = null


                };



                //check what's what and create
                card.created_date = DateTime.Now;
                card.updated_date = DateTime.Now;
                card.created_by = RevelCardInsertUser;
                card.updated_by = RevelCardInsertUser;

                //create in Revel

                var webCreate = await writer.CreateCustomer(customer);

                if (webCreate.Equals(0))
                {

                    var webReader = new RevelWebserviceDataReader(est);

                    var createdCustomer = await webReader.GetRevelWebserviceItem(new Customer(), customer.ResourceUri);
                    customer.Uuid = createdCustomer.Uuid;

                    db.Customers.Add(customer);
                    var saveCount = db.SaveChanges();

                    if (saveCount > 0)
                    {
                        //create the address, assign customerid etc

                        var addCard = await writer.CreateRewardCard(card);

                        if (addCard == 0)
                        {

                            db.RewardsCardNew.Add(card);
                            saveCount = db.SaveChanges();
                            if (saveCount > 0)
                            {
                                card.Customer = customer;

                                return Request.CreateResponse(HttpStatusCode.OK, "Thank you for registering!");

                            }
                        }




                    }
                }



                return new HttpResponseMessage(HttpStatusCode.Created);


                //that's it
            }

            return new HttpResponseMessage(HttpStatusCode.BadRequest);


            /* if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            return CreatedAtRoute("DefaultApi", new { id = rewardscardnew.DBKEY_rewardscardnew_id }, rewardscardnew);*/

        }

        // DELETE api/CusCard/5
        /*  [ResponseType(typeof(RewardsCardNew))]
          public async Task<IHttpActionResult> DeleteRewardsCardNew(int id)
          {
              RewardsCardNew rewardscardnew = await db.RewardsCardNew.FindAsync(id);
              if (rewardscardnew == null)
              {
                  return NotFound();
              }

              db.RewardsCardNew.Remove(rewardscardnew);
              await db.SaveChangesAsync();

              return Ok(rewardscardnew);
          }*/

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RewardsCardNewExists(int id)
        {
            return db.RewardsCardNew.Count(e => e.DBKEY_rewardscardnew_id == id) > 0;
        }
    }
}