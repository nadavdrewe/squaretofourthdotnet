using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Models;
using Revel._808nd.com.Models.ViewModels;
using Microsoft.Ajax.Utilities;
using Revel._808nd.com.Classes;
using Slack._808nd.com.Classes;
using Revel._808nd.com.Models;
using Revel._808nd.com.Models.ViewModels;
using Web.Grind._808nd.MailChimp;

namespace Web.Grind._808nd.com.Controllers
{
    public class BlackCardFormController : Controller
    {
        private GrindContext db;
        private string RevelAPIKEY { get; } = ConfigurationManager.AppSettings["RevelAPIKEY"];
        private string RevelBaseURL { get; } = ConfigurationManager.AppSettings["RevelBaseURL"];
        private string RevelCardInsertUser { get; } = ConfigurationManager.AppSettings["RevelCardInsertUser"];

        public BlackCardFormController(GrindContext grind)
        {
            db = grind;

        }



        public BlackCardFormController()
        {
            db = new GrindContext();
        }
        //
        // GET: /BlackCardForm/

        public async Task<ActionResult> Create()
        {
            /*      ViewBag.ExistingCards = db.RewardsCardNew.ToList().Select(x => x.number).ToList();*/

            var test = 1;
            ViewBag.EstURI = "/enterprise/Establishment/1/";
            return View("Create");
        }

        public async Task<ActionResult> CreateLondon()
        {
            ViewBag.ExistingCards = db.RewardsCardNew.ToList().Select(x => x.number).ToList();
            ViewBag.EstURI = "/enterprise/Establishment/4/";
            return View("Create");
        }

        public async Task<ActionResult> CreateHoborn()
        {
            ViewBag.ExistingCards = db.RewardsCardNew.ToList().Select(x => x.number).ToList();
            ViewBag.EstURI = "/enterprise/Establishment/5/";
            return View("Create");
        }

        public async Task<ActionResult> CreateSoho()
        {
            ViewBag.ExistingCards = db.RewardsCardNew.ToList().Select(x => x.number).ToList();
            ViewBag.EstURI = "/enterprise/Establishment/3/";
            return View("Create");
        }


        [HttpPost]
        public async Task<ActionResult> Create(CustomerCardViewModel model)
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
                    establishment = model.establishment,
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


                };

                try
                {
                    customer.BirthDate = Convert.ToDateTime(model.dob);
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
                var ok = await slack.SendMessage("A new black card was created:" + card.number + " - " + customer.FirstName + " " + customer.LastName + " - " + customer.Email, "blackcard", "BlackCard");

                //mailchimp
                if (!customer.Email.ToLower().Contains("test")
                       &&
                       !customer.Email.ToLower().Contains("emailnadz")
                       )
                {
                    MailChimpGrind mailChimpGrind = new MailChimpGrind();
                    mailChimpGrind.PushCardSignUp(customer);
                }


                return View("Success");


                /*    var webCreate = await writer.CreateCustomer(customer);

                if (webCreate.Equals(0))
                {
                            var est = new Establishment(1, "Grind",
                   RevelAPIKEY,
                    new Uri(RevelBaseURL));

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

                                //push to slack


                               

                            }
                        }




                    }
                }*/



            }

            return View("Failure");
        }
    }
}