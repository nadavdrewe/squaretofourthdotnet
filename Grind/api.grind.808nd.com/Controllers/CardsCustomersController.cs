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
using Thinktecture.IdentityModel.WebApi;
using shared.services.grind.railgunit.com.DataShaping;

namespace api.grind._808nd.com.Controllers
{

    public class CardsCustomersController : ApiController
    {
        private GrindContext _db;
        private RewardCardServices _cardService;
        private CustomerService _customerService;
        private CardCustomerFactory _cardCustomerFactory;

        private const int maxPageSize = 100;

        public CardsCustomersController()
        {
            _db = new GrindContext();
            _cardService = new RewardCardServices(_db);
            _customerService = new CustomerService(_db);
            _cardCustomerFactory = new CardCustomerFactory();
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









        /*    [ResourceActionAuthorize("Read", "")]*/
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
                    return Ok(DataShaping.CreateDataShapedObject(_cardCustomerFactory.Create(card, customer), lstOfFields));
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
