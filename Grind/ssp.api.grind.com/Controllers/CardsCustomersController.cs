using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Ajax.Utilities;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.ServiceImplementaitons;
using Revel._808nd.com.Classes.WebserviceReaderImplementations;
using Revel._808nd.com.Models;
using Revel._808nd.com.DTO.Factory;
using shared.services.grind.railgunit.com.DataShaping;
using ssp.api.grind.com.Requests;
using Revel._808nd.com.Classes.WebserviceReader;
using System.Configuration;
using Revel._808nd.com.ObjectCreationFactories;

namespace ssp.api.grind.com.Controllers
{
    [Authorize]
    public class CardsCustomersController : ApiController
    {
        private GrindContext _db;
        private RewardCardServices _cardService;
        private CustomerService _customerService;
        private CardCustomerFactory _cardCustomerFactory;

        WebserviceDataWriter _webWriter;
        RevelWebserviceDataReader _webReader;
        RevelDBWriter _writer;
        RevelDBReader _bBReader;

        private string RevelAPIKEY { get; } = ConfigurationManager.AppSettings["RevelAPIKEY"];
        private string RevelBaseURL { get; } = ConfigurationManager.AppSettings["RevelBaseURL"];
        private string RevelCardInsertUser { get; } = ConfigurationManager.AppSettings["RevelCardInsertUser"];


        private const int maxPageSize = 100;

        public CardsCustomersController()
        {
            _db = new GrindContext();
            _cardService = new RewardCardServices(_db);
            _customerService = new CustomerService(_db);
            _cardCustomerFactory = new CardCustomerFactory();

            var revOrg = new Establishment(1, "Grind",
               RevelAPIKEY,
                new Uri(RevelBaseURL));

            _webReader = new RevelWebserviceDataReader(revOrg);
            _webWriter = new WebserviceDataWriter(revOrg, _db);
            _writer = new RevelDBWriter(_db);
            _bBReader = new RevelDBReader(revOrg);

        }

        public CardsCustomersController(GrindContext db, RewardCardServices cardService = null, CustomerService customerService = null, CardCustomerFactory cardCustomerFactory = null)
        {
            _db = db;
            _cardService = cardService;

            if (_cardService == null)
            {
                _cardService = new RewardCardServices(db);
            }

            if (customerService == null)
            {
                customerService = new CustomerService(db);
            }

            if (_cardCustomerFactory == null)
            {
                cardCustomerFactory = new CardCustomerFactory();
            }
        }

        //Gets latest version of the card from the API
        async Task<RewardsCardNew> GetCardFromRevelApiAndUpdateIfDifferingBalance(RewardsCardNew card)
        {
            var url = String.Format("/resources/RewardsCardNew/{0}?format=json", card.Revelid); //card revelID
            var latestVersionOfCard = await _webReader.GetRevelWebserviceItem(new RewardsCardNew(), url, new GenericFactory());

            if (latestVersionOfCard.current_points != card.current_points)
            {
                //update
                card.current_points = latestVersionOfCard.current_points;
                card.total_points = latestVersionOfCard.total_points;

                var updated = await _writer.UpdateRevelType<RewardsCardNew>(new List<RewardsCardNew> { card });
            }

            return card;
        }

        [Route("api/incrementbalance")]
        public async Task<IHttpActionResult> Increment([FromBody] MutateBalanceRequest request)
        {
            try
            {
                if (!request.cardNumber.IsNullOrWhiteSpace())
                {
                    var numberStrippedLeadingZeros = request.cardNumber;
                    //project card number original and trimmed
                    //check every card against the trimmed number 
                    //if match                                       
                    var card = _cardService.GetByNumber(request.cardNumber);
                    if (card != null)
                    {
                        //DO AN UPDATE OF THE CARD!! This mutates the original variable if necessary!
                        card = await GetCardFromRevelApiAndUpdateIfDifferingBalance(card);

                        //do the increment
                        card.current_points += request.amount;
                        card.total_points += request.amount;
                        card.updated_by = RevelCardInsertUser;
                        card.updated_date = DateTime.Now;

                        if (await _webWriter.UpdateRewardCard(card) == 0)
                        {
                            //update in the DB
                            var updated = await _writer.UpdateRevelType<RewardsCardNew>(new List<RewardsCardNew> { card });
                            return Ok(card);
                        }
                        return InternalServerError();
                    }
                    return BadRequest("Couldn't find that card!!");
                    //get latest card from API
                }
                return BadRequest("You didn't provide a card number");
            }
            catch (Exception ex)
            {

                return InternalServerError();
            }
        }

        [Route("api/decrementbalance")]
        public async Task<IHttpActionResult> Decrement([FromBody] MutateBalanceRequest request)
        {
            try
            {
                if (!request.cardNumber.IsNullOrWhiteSpace())
                {
                    var numberStrippedLeadingZeros = request.cardNumber;
                    //project card number original and trimmed
                    //check every card against the trimmed number 
                    //if match                                       
                    var card = _cardService.GetByNumber(request.cardNumber);
                    if (card != null)
                    {
                        //DO AN UPDATE OF THE CARD!! This mutates the original variable if necessary!
                        card = await GetCardFromRevelApiAndUpdateIfDifferingBalance(card);

                        //do the increment
                        //check balance
                        if (card.current_points < request.amount)
                        {
                            return BadRequest("The card doesn't have enough points to allow that");
                        }

                        card.current_points -= request.amount;
                        card.total_points -= request.amount;
                        card.updated_by = RevelCardInsertUser;
                        card.updated_date = DateTime.Now;

                        if (await _webWriter.UpdateRewardCard(card) == 0)
                        {
                            //update in the DB
                            var updated = await _writer.UpdateRevelType<RewardsCardNew>(new List<RewardsCardNew> { card });
                            return Ok(card);
                        }
                        return InternalServerError();
                    }
                    return BadRequest("Couldn't find that card!!");
                    //get latest card from API
                }
                return BadRequest("You didn't provide a card number");
            }
            catch (Exception ex)
            {

                return InternalServerError();
            }
        }

        [AllowAnonymous]
        [Route("api/cardscustomers")]
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
                    //project card number original and trimmed
                    //check every card against the trimmed number 
                    //if match                                       
                    card = _cardService.GetByNumber(number);
                    if (card != null)
                    {
                        //DO AN UPDATE OF THE CARD!! This mutates the original variable if necessary!
                        card = await GetCardFromRevelApiAndUpdateIfDifferingBalance(card);

                        //try match on both Revel keys
                        if (!card.customer_revel.IsNullOrWhiteSpace())
                        {

                            try
                            {
                                customer = await _customerService.GetFromRevelCustomerURL(card.customer_revel);
                            }
                            catch (Exception)
                            {


                            }
                        }
                        if (customer == null)
                        {
                            customer = _db.Customers.FirstOrDefault(x => x.LicNumber.Trim() == card.number.Trim());
                        }


                    }

                }
                else if (!email.IsNullOrWhiteSpace())
                {
                    //get the customer 
                    card = await _cardService.GetByCustomerEmail(email);
                    if (card != null)
                    {
                        customer = card.Customer;
                    }

                }


                if (card != null)
                {
                    // return Ok(DataShaping.CreateDataShapedObject(_cardCustomerFactory.Create(card, customer), lstOfFields));
                    return Ok(_cardCustomerFactory.Create(card, customer));
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                //log it
                return InternalServerError();

            }
        }
        /*
                // POST: api/CardCustomer
                public void Post([FromBody]string value)
                {
                }

                // PUT: api/CardCustomer/5
                public void Put(int id, [FromBody]string value)
                {
                }

                // DELETE: api/CardCustomer/5
                public void Delete(int id)
                {
                }*/
    }
}
