using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using api.grind._808nd.com.Models;
using Microsoft.Ajax.Utilities;
using Revel._808nd.com;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.ServiceImplementaitons;
using Revel._808nd.com.Classes.WebserviceReaderImplementations;
using Revel._808nd.com.Models;
using Slack._808nd.com.Classes;
using Web.Grind._808nd.com.Controllers;
using Web.Grind._808nd.MailChimp;

namespace api.grind._808nd.com.Controllers
{
    public class BlackCardSignupsController : ApiController
    {
        private GrindContext db = new GrindContext();
        private RewardCardServices _cardService;
        private CustomerService _customerService;

        public BlackCardSignupsController()
        {
            _cardService = new RewardCardServices(db);
            _customerService = new CustomerService(db);
        }

        private string RevelAPIKEY { get; } = ConfigurationManager.AppSettings["RevelAPIKEY"];
        private string RevelBaseURL { get; } = ConfigurationManager.AppSettings["RevelBaseURL"];
        private string RevelCardInsertUser { get; } = ConfigurationManager.AppSettings["RevelCardInsertUser"];
        // GET: api/BlackCardSignups
        /* public IQueryable<BlackCardSignup> GetBlackCardSignups()
         {
             return db.BlackCardSignups;
         }*/

        // GET: api/BlackCardSignups/5


        // PUT: api/BlackCardSignups/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutBlackCardSignup(int id, BlackCardSignup blackCardSignup)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != blackCardSignup.Id)
            {
                return BadRequest();
            }

            db.Entry(blackCardSignup).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlackCardSignupExists(id))
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

        // POST: api/BlackCardSignups
        [ResponseType(typeof(BlackCardSignup))]
        public async Task<IHttpActionResult> PostBlackCardSignup(BlackCardSignupViewModel vm)
        {
            try
            {
                if (vm == null)
                {
                    return BadRequest("It looks like there’s a mistake in what you’ve entered. Please check your email and date of birth are correct, and try again");
                }



                if (ValidateSignup(vm) && ModelState.IsValid)
                {
                    DateTime parsedDob = new DateTime(1901, 01, 01);
                    if (vm.dob != null)
                    {

                        DateTime.TryParse(vm.dob, out parsedDob);
                    }


                    var signUp = new BlackCardSignup
                    {
                        cardNumber = vm.cardNumber,
                        dob = parsedDob,
                        email = vm.email,
                        firstname = vm.firstname,
                        lastname = vm.lastname,
                        WhenCreated = DateTime.Now,
                        created = false,
                        valid = true
                    };



                    var existingNumbersCardsAny =
                        await
                            db.RewardsCardNew
                                .Where(x => x.number != null)
                                .Where(x => x.number.Trim().ToLower() == vm.cardNumber.Trim().ToLower())
                                .Select(x => x.number)
                                .ToListAsync();



                    //EITHER THERE IS CARDS ALREADY OR NOT
                    if (existingNumbersCardsAny.Count > 0)
                    {
                        //there is card already

                        var existingNumbers = existingNumbersCardsAny.Select(x => x.Trim().ToLower()).ToList();
                        var number = existingNumbers.FirstOrDefault();

                        //check if there is a card or customer already - if so, use the existing ones, if not update in the form 
                        //check it has not  been registered

                        var card = _cardService.GetByNumber(number);
                        var customer = new Customer();
                        //get existing card/customer and update

                        if (card != null)
                        {
                            //try match on both Revel keys to get customer
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
                                customer = db.Customers.FirstOrDefault(x => x.LicNumber.Trim() == card.number.Trim());
                            }

                            if (customer != null && !customer.Email.IsNullOrWhiteSpace())
                            {
                                return
                                   BadRequest("It seems this card is already registered to you, you can start using it right away!");
                            }

                            //if we have a customer now, check the email is blank or it has already been registered
                            if (customer != null && customer.Email.IsNullOrWhiteSpace())
                            {
                                var existingCustomers =
                           await
                               db.Customers.Where(x => x.Email.ToLower().Trim().Equals(vm.email.ToLower().Trim())).Select(x => x.Email)
                               .FirstOrDefaultAsync();

                                if (existingCustomers != null)
                                {
                                    return BadRequest("It seems this email is already registered to another card  please use another one, or drop us a quick email at hello@grind.co.uk");
                                }

                                //save the sign up
                                db.BlackCardSignups.Add(signUp);
                                await db.SaveChangesAsync();

                                //ok, update and save existing customer
                                customer.Email = signUp.email;
                                customer.BirthDate = parsedDob;
                                customer.FirstName = signUp.firstname;
                                customer.LastName = signUp.lastname;

                                //update customer
                                var est = new Establishment(1, "Grind",
                                    RevelAPIKEY,
                                    new Uri(RevelBaseURL));

                                var writer = new WebserviceDataWriter(est, db);
                                //else there's no customer, don't try and update one
                                if (await writer.UpdateCustomer(customer) == 0)
                                {
                                    //done
                                    return CreatedAtRoute("GrindApi", new { id = signUp.Id }, signUp);
                                }

                            }
                            else
                            {
                                return
                                    BadRequest("It seems this card is already registered to you, you can start using it right away!");
                            }


                        }

                    }
                    //there is no exsting
                    ////new logic branch of IF
                    else //there is no card already
                    {
                        var existingCustomers =
                            await
                                db.Customers.Where(x => x.Email.ToLower().Trim().Equals(vm.email.ToLower().Trim()))
                                    .FirstOrDefaultAsync();

                        if (existingCustomers != null)
                        {
                            return BadRequest("It seems this email is already registered to another card  please use another one, or drop us a quick email at hello@grind.co.uk");
                        }


                        db.BlackCardSignups.Add(signUp);
                        await db.SaveChangesAsync();

                        //create new card and customer
                        var newCard = new RewardsCardNew
                        {
                            number = signUp.cardNumber,
                            created_by = RevelCardInsertUser,
                            created_date = DateTime.Now,
                            current_points = 0,
                            total_points = 0,
                            total_purchases = 0,
                            total_visits = 0,
                            is_vip_card = false,
                            establishment = "/enterprise/Establishment/1/",
                            updated_by = RevelCardInsertUser,
                            updated_date = DateTime.Now,
                            payment_type = 4,
                            vip_points_refresh = 0,

                        };

                        //create customer
                        var newCustomer = new Customer
                        {
                            UpdatedBy = RevelCardInsertUser,
                            CreatedBy = RevelCardInsertUser,
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now,
                            FirstName = signUp.firstname,
                            LastName = signUp.lastname,
                            Active = true,
                            Email = signUp.email,
                            LicNumber = newCard.number,
                            LoyaltyNumber = newCard.number,
                            RefNumber = newCard.number,
                            Address = "n/a",


                        };


                        //check what's what and create
                        newCard.created_date = DateTime.Now;
                        newCard.updated_date = DateTime.Now;
                        newCard.created_by = RevelCardInsertUser;
                        newCard.updated_by = RevelCardInsertUser;

                        //create in Revel

                        var service = new CombinedCardCustomerController(db);
                        var successOrFail = await service.Create(newCustomer, newCard, "0");


                        //slack
                        var slack = new SlackMessenger();
                        var ok =
                            await
                                slack.SendMessage(
                                    "A new black card was created:" + newCard.number + " - " + newCustomer.FirstName + " " +
                                    newCustomer.LastName + " - " + newCustomer.Email, "blackcard", "BlackCard");

                        //mailchimp
                        if (!newCustomer.Email.ToLower().Contains("test")
                            &&
                            !newCustomer.Email.ToLower().Contains("emailnadz")
                        )
                        {
                            MailChimpGrind mailChimpGrind = new MailChimpGrind();
                            mailChimpGrind.PushCardSignUp(newCustomer);
                        }

                        //done
                        return CreatedAtRoute("GrindApi", new { id = signUp.Id }, signUp);

                    }
                    //didn't hit any logic - shouldn't be ever able to hit this BadRequest
                    return
                        BadRequest("Looks like something has gone wrong with your registration. Drop us a quick email at: hello@grind.co.uk.");
                }
                //card didn't validate
                return BadRequest("It looks like there’s a mistake in what you’ve entered. Please check your email and date of birth are correct, and try again");


            }
            catch (Exception ex)
            {

                //return InternalServerError(new Exception("Looks like something has gone wrong with your registration. Drop us a quick email at: hello@grind.co.uk."));
                var exe = new Exception("Test" + ex.Message, ex);
                //todo: logging
                return InternalServerError(exe);

            }
        }

        private async Task<bool> createRewardsCard(BlackCardSignup signUp)
        {
            var est = new Establishment(1, "Grind",
                   RevelAPIKEY,
                   new Uri(RevelBaseURL));
            //checking use input

            if (!String.IsNullOrWhiteSpace(signUp.cardNumber))
            {

                var writer = new WebserviceDataWriter(est, db);


                //create card
                var card = new RewardsCardNew
                {
                    number = signUp.cardNumber,
                    created_by = RevelCardInsertUser,
                    created_date = DateTime.Now,
                    current_points = 0,
                    total_points = 0,
                    total_purchases = 0,
                    total_visits = 0,
                    is_vip_card = false,
                    establishment = "/enterprise/Establishment/1/",
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
                    FirstName = signUp.firstname,
                    LastName = signUp.lastname,
                    Active = true,
                    Email = signUp.email,
                    LicNumber = card.number,
                    LoyaltyNumber = card.number,
                    RefNumber = card.number,
                    Address = "n/a",


                };

                try
                {
                    customer.BirthDate = Convert.ToDateTime(signUp.dob);
                }
                catch (Exception)
                {

                }

                //check what's what and create
                card.created_date = DateTime.Now;
                card.updated_date = DateTime.Now;
                card.created_by = RevelCardInsertUser;
                card.updated_by = RevelCardInsertUser;

                //create in Revel

                var service = new CombinedCardCustomerController(db);

                var successOrFail = await service.Create(customer, card, "0");


                //slack
                var slack = new SlackMessenger();
                var ok =
                    await
                        slack.SendMessage(
                            "A new black card was created:" + card.number + " - " + customer.FirstName + " " +
                            customer.LastName + " - " + customer.Email, "blackcard", "BlackCard");

                //mailchimp
                if (!customer.Email.ToLower().Contains("test")
                    &&
                    !customer.Email.ToLower().Contains("emailnadz")
                    )
                {
                    MailChimpGrind mailChimpGrind = new MailChimpGrind();
                    mailChimpGrind.PushCardSignUp(customer);
                }

            }

            return true;

        }

        private bool ValidateSignup(BlackCardSignupViewModel vm)
        {
            bool isEmail = Regex.IsMatch(vm.email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

            var number = 0;
            DateTime parseDate;

            if (vm.firstname != ""
                && vm.lastname != ""
                && isEmail
                && Int32.TryParse(vm.cardNumber, out number)
                && vm.cardNumber == vm.cardNumberConfirm
                && DateTime.TryParse(vm.dob, out parseDate))
            {
                return true;
            }
            return false;
        }

        // DELETE: api/BlackCardSignups/5
        [ResponseType(typeof(BlackCardSignup))]
        public async Task<IHttpActionResult> DeleteBlackCardSignup(int id)
        {
            var dbSet = db.BlackCardSignups as DbSet<BlackCardSignup>;
            BlackCardSignup blackCardSignup = await dbSet.FindAsync(id);
            if (blackCardSignup == null)
            {
                return NotFound();
            }

            db.BlackCardSignups.Remove(blackCardSignup);
            await db.SaveChangesAsync();

            return Ok(blackCardSignup);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BlackCardSignupExists(int id)
        {
            return db.BlackCardSignups.Count(e => e.Id == id) > 0;
        }
    }
}