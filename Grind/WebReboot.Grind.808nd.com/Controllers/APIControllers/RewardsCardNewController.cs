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
    public class RewardsCardNewController : ApiController
    {
        private GrindContext db = new GrindContext();

        // GET api/RewardsCardNew
        public IQueryable<RewardsCardNew> GetRewardsCardNew()
        {
            return db.RewardsCardNew;
        }

        // GET api/RewardsCardNew/5
        [ResponseType(typeof(RewardsCardNew))]
        public async Task<IHttpActionResult> GetRewardsCardNew(int id)
        {
            RewardsCardNew rewardscardnew = await ((DbSet<RewardsCardNew>) db.RewardsCardNew).FindAsync(id);
            if (rewardscardnew == null)
            {
                return NotFound();
            }

            return Ok(rewardscardnew);
        }

        // PUT api/RewardsCardNew/5
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
        }

        // POST api/RewardsCardNew
        [ResponseType(typeof(RewardsCardNew))]
        public async Task<IHttpActionResult> PostRewardsCardNew(RewardsCardNew rewardscardnew)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.RewardsCardNew.Add(rewardscardnew);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = rewardscardnew.DBKEY_rewardscardnew_id }, rewardscardnew);
        }

        // DELETE api/RewardsCardNew/5
        [ResponseType(typeof(RewardsCardNew))]
        public async Task<IHttpActionResult> DeleteRewardsCardNew(int id)
        {
            RewardsCardNew rewardscardnew = await ((DbSet<RewardsCardNew>)db.RewardsCardNew).FindAsync(id);
            if (rewardscardnew == null)
            {
                return NotFound();
            }

            db.RewardsCardNew.Remove(rewardscardnew);
            await db.SaveChangesAsync();

            return Ok(rewardscardnew);
        }

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