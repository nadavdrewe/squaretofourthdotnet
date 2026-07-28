using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using api.grind._808nd.com.Helper;
using Microsoft.Ajax.Utilities;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.ServiceImplementaitons;
using Revel._808nd.com.Classes.WebserviceReaderImplementations;
using Revel._808nd.com.Models;
using shared.services.grind.railgunit.com.DataShaping;
using WebGrease.Css.Extensions;


namespace api.grind._808nd.com.Controllers
{


    public class RewardCardsController : ApiController
    {
        private GrindContext _db;
        private RewardCardServices _cardService;

        private const int maxPageSize = 100;

        public RewardCardsController(GrindContext db, RewardCardServices cardService = null)
        {
            _db = db;
            _cardService = cardService;

            if (_cardService == null)
            {
                Debug.Assert(db != null, "db != null");
                _cardService = new RewardCardServices(db);
            }

        }

        public RewardCardsController()
        {
            _db = new GrindContext();
            _cardService = new RewardCardServices(_db);
        }



        // GET: api/RewardCards

        //[Route("api/rewardcards", Name = "RewardCardsList")]        
        //public IHttpActionResult GetAll(string sort = "DBKEY_rewardscardnew_id", int page = 1, int pageSize = 100)
        //{
        //    try
        //    {
        //        var allCards = _db.RewardsCardNew
        //            .ApplySort(sort)
        //            .ToSafeReadOnlyCollection();

        //        //limit max page size
        //        if (pageSize > maxPageSize)
        //        {
        //            pageSize = maxPageSize;
        //        }

        //        var totalCount = allCards.Count();
        //        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        //        var urlHelper = new UrlHelper(request: Request);
        //        var prevLink = page > 1
        //            ? urlHelper.Link("RewardCardsList",
        //                new
        //                {
        //                    page = page - 1,
        //                    pageSize = pageSize,
        //                    sort = sort
        //                }
        //                )
        //            : "";

        //        var nextLink = page < totalPages
        //            ? urlHelper.Link("RewardCardsList", new
        //            {
        //                page = page + 1,
        //                pageSize = pageSize,
        //                sort = sort
        //            })
        //            : "";

        //        var paginationHeader = new
        //        {
        //            currentPage = page,
        //            pageSize = pageSize,
        //            totalPages = totalPages,
        //            previousPageLink = prevLink,
        //            nextPageLink = nextLink
        //        };

        //        HttpContext.Current.Response.Headers.Add("X-Pagination",
        //            Newtonsoft.Json.JsonConvert.SerializeObject(paginationHeader)
        //            );


        //        var sortedAndPagedResults =
        //            allCards
        //                .Skip(pageSize * (page - 1))
        //                .Take(pageSize)
        //                .ToSafeReadOnlyCollection();

        //        return Ok(sortedAndPagedResults);
        //    }
        //    catch (Exception)
        //    {
        //        return InternalServerError();
        //    }

        //}

        // GET: api/RewardCards/5
        public IHttpActionResult Get(int id)
        {
            try
            {
                var card = _db.RewardsCardNew.FirstOrDefault(x => x.DBKEY_rewardscardnew_id == id);

                if (card != null)
                {
                    return Ok(card);
                }

                return NotFound();


            }
            catch (Exception)
            {

                return InternalServerError();
            }

        }


        
        public async Task<IHttpActionResult> Get([FromUri]string number = "", string email = null, string fields = null)
        {

            //data shaping
            var lstOfFields = new List<string>();

            if (fields != null)
            {
                lstOfFields = fields.ToLower().Split(',').ToList();
            }


            try
            {
                RewardsCardNew card = null;
                Customer customer = null;

                if (!number.IsNullOrWhiteSpace())
                {
                    var numberStrippedLeadingZeros = number;

                    card = _cardService.GetByNumber(number);
                   
                    
                }

                if (!email.IsNullOrWhiteSpace())
                {
                    //get the customer 
                    card = await _cardService.GetByCustomerEmail(email);

                }


                if (card != null)
                {
                    return Ok(DataShaping.CreateDataShapedObject(card, lstOfFields));
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                //log it
                return InternalServerError();

            }
        }

        // POST: api/RewardCards
     /*   public void Post([FromBody]string value)
        {
        }

        // PUT: api/RewardCards/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/RewardCards/5
        public void Delete(int id)
        {
        }*/
    }
}
