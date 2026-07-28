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
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Http.Filters;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Models;

namespace api.grind._808nd.com.Controllers
{
    [GrindApiAuthorise]
    public class WifiLoginsController : ApiController
    {
        private GrindContext db = new GrindContext();
        public IEnumerable<ApiAuthentication> authentications ;
        // GET: api/WifiLogins
        public IQueryable<WifiLogin> GetWifiLogins()
        {
            return db.WifiLogins.OrderByDescending(x=>x.LoginDate).Take(100);
        }

        public WifiLoginsController()
        {
            authentications = db.ApiAuthentications.ToList();
        }

      
        // GET: api/WifiLogins/5
        [ResponseType(typeof(WifiLogin))]
        public async Task<IHttpActionResult> GetWifiLogin(int id)
        {
            var dbSet = db.WifiLogins as DbSet<WifiLogin>;
            WifiLogin wifiLogin = await dbSet.FindAsync(id);
            if (wifiLogin == null)
            {
                return NotFound();
            }

            return Ok(wifiLogin);
        }

        // PUT: api/WifiLogins/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutWifiLogin(int id, WifiLogin wifiLogin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != wifiLogin.Id)
            {
                return BadRequest();
            }

            db.Entry(wifiLogin).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WifiLoginExists(id))
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

        // POST: api/WifiLogins

        [ResponseType(typeof(WifiLogin))]
        public async Task<IHttpActionResult> PostWifiLogin(WifiLogin wifiLogin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.WifiLogins.Add(wifiLogin);
            await db.SaveChangesAsync();

            return CreatedAtRoute("GrindApi", new { id = wifiLogin.Id }, wifiLogin);
        }

        // DELETE: api/WifiLogins/5
        [ResponseType(typeof(WifiLogin))]
        public async Task<IHttpActionResult> DeleteWifiLogin(int id)
        {

            var dbSet = db.WifiLogins as DbSet<WifiLogin>;
            WifiLogin wifiLogin = await dbSet.FindAsync(id);
            if (wifiLogin == null)
            {
                return NotFound();
            }

            db.WifiLogins.Remove(wifiLogin);
            await db.SaveChangesAsync();

            return Ok(wifiLogin);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool WifiLoginExists(int id)
        {
            return db.WifiLogins.Count(e => e.Id == id) > 0;
        }
    }

    public class GrindApiAuthoriseAttribute : AuthorizeAttribute
    {
     
        public override async Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            var controller = actionContext.ControllerContext.Controller as WifiLoginsController;
            var _authentications = controller.authentications;
            IEnumerable<string> authToken = new List<string>();
            actionContext.Request.Headers.TryGetValues("API-AUTH", out authToken);          
            if (authToken != null)
            {
                foreach (var auth in _authentications)
                {
                    if (authToken.FirstOrDefault() == auth.ApiKey)
                    {
                        return;
                    }
                }
            }
            
            HandleUnauthorizedRequest(actionContext);
        }

        protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var challengeMessage = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            challengeMessage.Headers.Add("WWW-Authenticate", "API-AUTH");
            throw new HttpResponseException(challengeMessage);

        }

    }
}