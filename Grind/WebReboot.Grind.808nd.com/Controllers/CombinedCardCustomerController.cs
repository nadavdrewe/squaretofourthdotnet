using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq.Dynamic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.Reporting;
using Revel._808nd.com.Classes.ServiceImplementaitons;
using Revel._808nd.com.Models;
using Web.Grind._808nd.MailChimp;
using Revel._808nd.com.Classes.WebserviceReader;
using WebReboot.Grind._808nd.com;
using WebReboot.Grind._808nd.com.CacheHelper;
using WebReboot.Grind._808nd.com.Controllers;
using WebReboot.Grind._808nd.com.Models.ViewModels;
using WebReboot.Grind._808nd.com.ViewModels;
using WebReboot.Grind._808nd.com.LoyaltyCardUI.UICardAdaptor;
using System.Reflection;
using System.Data.Entity.Migrations;
using WebReboot.Grind._808nd.com.DatatablesAjax;

namespace Web.Grind._808nd.com.Controllers
{


    [System.Web.Mvc.Authorize]
    public class CombinedCardCustomerController : Controller
    {
        private GrindContext db;
        private const string _cacheCardCollectionName = "allCards";
        private const string _cacheGiftCardCollectionName = "giftCards";

        private List<RewardsCardNew> allCards = HttpRuntime.Cache.Get(_cacheCardCollectionName) as List<RewardsCardNew>;
        private List<GiftCard> allGiftCards = HttpRuntime.Cache.Get(_cacheGiftCardCollectionName) as List<GiftCard>;

        private string RevelAPIKEY { get; } = ConfigurationManager.AppSettings["RevelAPIKEY"].ToString().Trim();
        private string RevelBaseURL { get; } = ConfigurationManager.AppSettings["RevelBaseURL"].ToString().Trim();
        private string RevelCardInsertUser { get; } = ConfigurationManager.AppSettings["RevelCardInsertUser"];
        //
        // GET: /CombinedCardCustomer/

        List<SelectListItem> GenerateLoyaltyCardDropdown()
        {
            return new List<SelectListItem>
            {
               new SelectListItem { Text = "Off", Value = "0" },
               new SelectListItem { Text = "Daily", Value = "1" },
               new SelectListItem { Text = "Weekly", Value = "2" },
               new SelectListItem { Text = "Monthly", Value = "3" }
            };
        }


        void AddSomebodyEditedLog(string editType, string cardnumber)
        {
            db.SystemLogs.Add(new SystemLog
            {
                Type = "CARD EDIT",
                WhoTriggered = User?.Identity?.Name ?? "None",
                WhenCreated = DateTime.Now,
                Note = editType + " " + cardnumber
            });
            db.SaveChanges();
        }

        public ActionResult GetCardsForToday()
        {
            //var today24HoursAgo = DateTime.Now.AddDays(-1);

            //var cards = db.RewardsCardNew.Where(x => x.created_date >= today24HoursAgo).ToList();
            //var customers = db.Customers.Where(x => x.CreatedDate >= today24HoursAgo).ToList();

            //foreach (var card in cards)
            //{
            //    card.Customer = customers.FirstOrDefault(x => x.ResourceUri == card.customer_revel);
            //    if (card.Customer == null)
            //    {
            //        card.Customer = new Customer { Email = "No customer attached" };
            //    }

            //}

            //var orderedCards = cards.OrderByDescending(x => x.created_date).ToList();
            //return Json(new { latestCards = orderedCards }, JsonRequestBehavior.AllowGet);


            return Json(new { latestCards = "" }, JsonRequestBehavior.AllowGet);
        }


        [System.Web.Mvc.Authorize(Roles = "admin")]
        public async void WipeCustomerEmail()
        {

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://shoreditchgrind.revelup.com/");

            var customer = db.Customers.First(x => x.DBKEY_customer_id == 18241);
            customer.Email = "";
            string json = JsonConvert.SerializeObject(customer);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(customer.ResourceUri, content);

            string resultContent = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {


                //do we need to assign any IDs to localDB
                //log transaction success
            }
            else
            {
                //log transaction fail

            }

        }

        public ActionResult AjaxHandler(jQueryDataTableParamModel param, string sSearch)
        {

            List<RewardsCardNew> cards = allCards;
            List<GiftCard> giftCards = allGiftCards;

            foreach (var card in cards)
            {



                if (card.Customer == null)
                {
                    card.Customer = new Customer
                    {
                        FirstName = "",
                        LastName = "",
                        Email = "",
                        Active = false,
                    };
                }

                if (card.Customer.FirstName == null)
                {
                    card.Customer.FirstName = "";
                }
                if (card.Customer.LastName == null)
                {
                    card.Customer.LastName = "";
                }
                if (card.Customer.Email == null)
                {
                    card.Customer.Email = "";
                }

                if (card.number.IsNullOrWhiteSpace())
                {
                    card.number = "";
                }
            }

            var cardsNoVIP = cards
                .ToList();

            var cardsFiltered = new List<RewardsCardNew>();

            if (!String.IsNullOrEmpty(param.sSearch))
            {
                cardsFiltered = cardsNoVIP
                     .Where(x => x.number.ToLower().Contains(param.sSearch.ToLower())
                      || x.Customer.FirstName.ToLower().Contains(param.sSearch.ToLower())
                      || x.Customer.LastName.ToLower().Contains(param.sSearch.ToLower())
                      || x.Customer.Email.ToLower().Contains(param.sSearch.ToLower())
                       || x.created_date.ToString().Contains(param.sSearch.ToLower())
                       || x.Revelid.ToString().Contains(param.sSearch)
                    ).ToList();
            }
            else
            {
                cardsFiltered = cardsNoVIP;
            }


            //DO SORT
            var colNumberToSortBy = Convert.ToInt16(param.iSortCol_0);
            var whichColIsThat = param.sColumns.Split(',').ToList().ElementAt(colNumberToSortBy);
            var whichDirection = param.sSortDir_0.ToUpper();

            var orderByQuery = String.Format("{0} {1}", whichColIsThat, whichDirection);
            cardsFiltered = cardsFiltered.AsQueryable().OrderBy(orderByQuery).ToList();
            //END SORT


            try
            {
                var cardBeforeTransform = cardsFiltered.Skip(param.iDisplayStart)
             .Take(param.iDisplayLength);


                //get gift balances for above cards
                Dictionary<string, decimal> cardGiftBalances = new Dictionary<string, decimal>();
                cardBeforeTransform.ToList().ForEach(x =>
                {
                    var giftCard = giftCards.Where(y => y.number.Trim() == x.number.Trim()).FirstOrDefault();
                    var giftBalance = giftCard == null ? 0.00M : giftCard.remaining_balance;
                    var existing = cardGiftBalances.Where(y => y.Key == x.number);
                    if (!existing.Any())
                        cardGiftBalances.Add(x.number, giftBalance);
                });
                //end gift balances

                var cardsToReturn = cardBeforeTransform.Select(card =>
        new[]
     {
                card.number.ToString() ?? String.Empty,
                   card.Customer.Email,
                ((int?)card.Revelid).ToString(),
               card.Customer.FirstName.ToString(),
                  card.Customer.LastName,
                    ((DateTime?)card.created_date).ToString() ?? String.Empty,
                        ((int?)card.current_points).ToString() ?? String.Empty,
                        ((int?)card.total_points).ToString() ?? String.Empty,
                       cardGiftBalances.First(x=>x.Key == card.number).Value.ToString(),
                        ((decimal?) card.total_purchases).ToString() ?? String.Empty,
                       ((int?)card.total_visits).ToString(),
                         ((int?)card.days_since_last_visit).ToString(),
                       LoyaltyCardAdaptorUIService.MapUICardTypeToOptionText(LoyaltyCardAdaptorUIService.GetUICardType(card)),
                         ((DateTime?)card.ExpiryDate).ToString(),
                        ((DateTime?)card.vip_points_last_refreshed).ToString(),
                        card.notes,
                     "<a href='Edit/"+ card.DBKEY_rewardscardnew_id + "'>Edit</a>"


        }).ToArray();




                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = cardsNoVIP.Count(),
                    iTotalDisplayRecords = cardsFiltered.Count(),
                    aaData = cardsToReturn
                },
            JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }





        public CombinedCardCustomerController(GrindContext grind)
        {
            db = grind;
        }

        public CombinedCardCustomerController()
        {
            db = new GrindContext();
        }

        public ActionResult Create()
        {
            ViewBag.DropdownCardType = GenerateLoyaltyCardDropdown();
            ViewBag.RevelCardInsertUser = RevelCardInsertUser;
            return View();
        }

        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> Create(Customer customer, RewardsCardNew rewardscardnew, string RefreshableCardType, GiftCard giftcard = null)
        {
            ViewBag.DropdownCardType = GenerateLoyaltyCardDropdown();
            if (rewardscardnew.LoyaltyCardType != null)
            {
                if (rewardscardnew.LoyaltyCardType.id == 0)
                {
                    rewardscardnew.LoyaltyCardType = null;
                }
                else
                {
                    var typeid = rewardscardnew.LoyaltyCardType.id;
                    var cardType = db.LoyaltyCardTypes.First(x => x.id == typeid);
                    rewardscardnew.LoyaltyCardType = cardType;
                }

            }

            //SET 
            var optionSelected = Convert.ToInt32(RefreshableCardType);
            SetRefreshableCardTypeBasedOnOptionSelected(rewardscardnew, optionSelected, db.LoyaltyCardTypes.ToList());



            var est = new Establishment(1, "Grind",
                           RevelAPIKEY,
                           new Uri(RevelBaseURL));
            //create the customer, assign loyalty card number, any other atts

            rewardscardnew.number = rewardscardnew.number.Trim();

            rewardscardnew.days_since_last_visit = 0;
            rewardscardnew.created_date = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
            rewardscardnew.updated_date = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));

            customer.LicNumber = rewardscardnew.number;
            customer.LoyaltyNumber = rewardscardnew.number;
            customer.RefNumber = rewardscardnew.number;




            //create in Revel
            var writer = new WebserviceDataWriter(est, db);

            //check number doesn't exist already

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

                    //it's worked, great
                    rewardscardnew.customer_revel = customer.ResourceUri;

                    //create the card, assign customerid etc
                    var addCard = await writer.CreateRewardCard(rewardscardnew);

                    if (addCard == 0)
                    {

                        db.RewardsCardNew.Add(rewardscardnew);
                        saveCount = db.SaveChanges();
                        if (saveCount > 0)
                        {
                            var giftBalance = 15.00M;
                            if (giftcard != null)
                            {
                                giftBalance = giftcard.remaining_balance;
                            }
                            //create gift card
                            giftcard = new GiftCard
                            {
                                remaining_balance = giftBalance,
                                payment_type = 4,
                                created_date = DateTime.Now,
                                created_by = RevelCardInsertUser,
                                updated_by = RevelCardInsertUser,
                                updated_date = DateTime.Now,
                                LinkingRevelCustomerID = customer.RevelId,
                                LinkingRevelRewardsCardNewID = rewardscardnew.Revelid,
                                initial_value = (int)giftBalance,
                                establishment = rewardscardnew.establishment,
                                number = rewardscardnew.number,
                                customer = customer.ResourceUri,

                            };

                            giftcard.theCustomer = customer;
                            giftcard.RewardsCardNew = rewardscardnew;

                            webCreate = await writer.CreateGiftCard(giftcard);

                            if (webCreate.Equals(0))
                            {
                                db.GiftCards.Add(giftcard);
                                saveCount = await db.SaveChangesAsync();

                                if (saveCount > 0)
                                {
                                    //gift card was created
                                }
                            }

                            //log the attempt to edit
                            AddSomebodyEditedLog("Create", rewardscardnew.number);

                            rewardscardnew.Customer = customer;
                            ViewBag.RevelCardInsertUser = RevelCardInsertUser;
                            ViewBag.Message = "Card Update Successful! Card:" + rewardscardnew.Revelid + " Customer:" + customer.FirstName + " " + customer.LastName;
                            return View("Create");

                        }
                    }
                    else
                    {
                        rewardscardnew.Customer = customer;
                        ViewBag.RevelCardInsertUser = RevelCardInsertUser;
                        ViewBag.Message = "Card Update FAILED! Please try later!";

                        CacheHelpers.AddCardCache(rewardscardnew, _cacheGiftCardCollectionName);

                        return View("Create");
                    }



                }
            }


            //create the card


            throw new Exception("The save or one of the updates has failed. Please try again later");
        }



        public async Task<PartialViewResult> GetCardLogs(int id)
        {
            var rewardscardnew = await db.RewardsCardNew.FirstOrDefaultAsync(x => x.DBKEY_rewardscardnew_id.Equals(id));

            var logs = await db.RewardCardLogs.OrderByDescending(x => x.created_date)
                    .Where(x => x.reward_card_id == rewardscardnew.Revelid).Distinct()
                    .Select(x => new LogViewModel
                    {
                        type_of_change = x.type_of_change,
                        created_date = x.created_date,
                        establishment = x.establishment,
                        point = x.point,
                        order_id = x.order_id

                    })
                    .ToListAsync();

            var orderIds = logs.Select(x => (int?)x.order_id).Distinct().ToList();
            var orderItems = await db.OrderItems.Where(o => orderIds.Contains(o.parent_order_id))
                .Select(oi => new OrderItemViewModel
                {
                    product_id = oi.product_id,
                    order_item_id = oi.orderitem_id,
                    parent_order_id = oi.parent_order_id
                })
               .ToListAsync();


            var prodIds = orderItems.Select(x => x.product_id);
            var prods =
                await db.Products.Where(p => prodIds.Contains(p.product_id)).Select(pr => new ProductViewModel
                {
                    product_id = pr.product_id,
                    name = pr.name
                }).ToListAsync();



            var model = new CardLogPartialModel
            {
                Establishments = db.Establishments.Select(x => new EstablishmentViewModel
                {
                    name = x.name,
                    resource_uri = x.resource_uri
                }).ToList(),
                Logs = logs,
                Products = prods,
                OrderItems = orderItems
            };
            return PartialView("~/Views/Charts/Partial/CardLogsPartial.cshtml", model);

        }


        [System.Web.Mvc.HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RewardsCardNew rewardscardnew = ((DbSet<RewardsCardNew>)db.RewardsCardNew).Include(x => x.LoyaltyCardType).First(x => x.DBKEY_rewardscardnew_id == id);



            rewardscardnew.Customer = RewardsCardNew.FindCustomerForCard(rewardscardnew, db.Customers.ToList());

            ViewBag.GiftCard = db.GiftCards.Where(x => x.number.Trim() == rewardscardnew.number.Trim()).FirstOrDefault();


            if (rewardscardnew == null)
            {
                return HttpNotFound();
            }
            /*
                        var logs = db.RewardCardLogs.OrderByDescending(x => x.created_date)
                                .Where(x => x.reward_card_id == rewardscardnew.Revelid).Distinct().Take(50)
                                .Select(x=>new LogViewModel
                                {
                                    type_of_change = x.type_of_change,
                                    created_date = x.created_date,
                                    establishment = x.establishment,
                                    point = x.point,
                                    order_id = x.order_id

                                })
                                .ToList();*/

            /*   var orderIds = logs.Select(x => (int?)x.order_id).Distinct().ToList();
               var orderItems = db.OrderItems.Where(o => orderIds.Contains(o.parent_order_id))
                   .Select(oi=>new OrderItemViewModel
                   {
                       product_id = oi.product_id,
                       order_item_id = oi.orderitem_id,
                        parent_order_id = oi.parent_order_id
                   })
                   .ToList();


               var prodIds = orderItems.Select(x => x.product_id);
               var prods =
                   db.Products.Where(p => prodIds.Contains(p.product_id)).Select(pr=> new ProductViewModel
                   {
                       product_id = pr.product_id,
                       name = pr.name
                   }).ToList();

               ViewBag.OrderItems = orderItems;
               ViewBag.Products = prods;
               ViewBag.Establishments = db.Establishments.Select(x=>new EstablishmentViewModel
               {
                   name = x.name,
                   resource_uri = x.resource_uri
               }).ToList();*/

            /*  ViewBag.CardLogs = logs;*/




            ViewBag.DropdownCardType = GenerateLoyaltyCardDropdown();
            ViewBag.RevelCardInsertUser = RevelCardInsertUser;
            Session["editedCard"] = rewardscardnew;
            return View(rewardscardnew);
        }

        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> Edit(Customer customer, RewardsCardNew rewardscardnew, GiftCard giftCard, string RefreshableCardType)
        {
            var local = db.RewardsCardNew.First(x => x.DBKEY_rewardscardnew_id == rewardscardnew.DBKEY_rewardscardnew_id);

            //log the attempt to edit
            AddSomebodyEditedLog("Edit", local.number);

            //get card from DB and set all props
            Type type = typeof(RewardsCardNew);
            PropertyInfo[] properties = type.GetProperties();
            var currentProp = "";
            foreach (PropertyInfo property in properties)
            {

                currentProp = property.Name;
                if (property.Name != "theAddress" && property.Name != "Identifier" && property.Name != "PrimaryKey")
                {
                    //existing value in submitted card
                    var existingValue = property.GetValue(rewardscardnew, null);
                    //set on new card
                    currentProp = property.Name;

                    try
                    {
                        property.SetValue(local, existingValue, null);
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }

                }
            }
            rewardscardnew = local;
            //end
            //////


            rewardscardnew.created_by = RevelCardInsertUser;
            rewardscardnew.updated_by = RevelCardInsertUser;

            if (rewardscardnew.LoyaltyCardType != null)
            {
                if (rewardscardnew.LoyaltyCardType.id == 0)
                {
                    rewardscardnew.LoyaltyCardType = null;

                }
                else
                {
                    var typeid = rewardscardnew.LoyaltyCardType.id;
                    var cardType = db.LoyaltyCardTypes.First(x => x.id == typeid);
                    rewardscardnew.LoyaltyCardType = cardType;
                }

            }

            //SET 
            var optionSelected = Convert.ToInt32(RefreshableCardType);
            SetRefreshableCardTypeBasedOnOptionSelected(rewardscardnew, optionSelected, db.LoyaltyCardTypes.ToList());



            var est = new Establishment(1, "Grind",
                   RevelAPIKEY,
                    new Uri(RevelBaseURL));

            var writer = new WebserviceDataWriter(est, db);

            //get original card and update the points
            var points = rewardscardnew.current_points;




            rewardscardnew.updated_by = RevelCardInsertUser;
            rewardscardnew.updated_date = DateTime.Now;

            if (await writer.UpdateRewardCard(rewardscardnew) != 0)
            {

                ViewBag.RevelCardInsertUser = RevelCardInsertUser;
                ViewBag.Message = "Card Update FAILED! Please try later!";
            }
            else
            {
                ViewBag.Message = "Card Update Successful! Card:" + rewardscardnew.Revelid + " Customer:" + customer.FirstName + " " + customer.LastName;

            }


            if (customer.DBKEY_customer_id != 0)
            {
                //else there's no customer, don't try and update one
                if (await writer.UpdateCustomer(customer) == 0)
                {

                }
            }
            ////////CUSTOMER
            //there's no customer, create a new one if there are fields correct
            else
            {
                if (customer.FirstName != "" || customer.LastName != "" || customer.Email != "")
                {
                    customer.CreatedDate = DateTime.Now;
                    customer.UpdatedDate = DateTime.Now;

                    customer.CreatedBy = RevelCardInsertUser;
                    customer.UpdatedBy = RevelCardInsertUser;

                    customer.LicNumber = rewardscardnew.number;
                    customer.LoyaltyNumber = rewardscardnew.number;
                    customer.RefNumber = rewardscardnew.number;

                    //create in Revel

                    var webCreate = await writer.CreateCustomer(customer);

                    if (webCreate.Equals(0))
                    {

                        var webReader = new RevelWebserviceDataReader(est);

                        var createdCustomer =
                            await webReader.GetRevelWebserviceItem(new Customer(), customer.ResourceUri);
                        customer.Uuid = createdCustomer.Uuid;

                        db.Customers.Add(customer);
                        var saveCount = await db.SaveChangesAsync();

                        if (saveCount > 0)
                        {

                        }
                    }
                    else
                    {
                        //it hasn't worked, do something
                    }
                }
            }
            /////
            //GIFT CARD - create if not exist
            /////
            if (giftCard.giftcard_id != 0)
            {
                var cardFromDbDifferentBalance = db.GiftCards.Find(giftCard.giftcard_id);

                //change the balance and save the card
                cardFromDbDifferentBalance.remaining_balance = giftCard.remaining_balance;

                writer = new WebserviceDataWriter(est, db);

                if (await writer.UpdateGiftCard(card: cardFromDbDifferentBalance) == 0)
                {
                    /*  db.GiftCards.Attach(cardFromDbDifferentBalance);
                      db.Entry(cardFromDbDifferentBalance).State = EntityState.Modified;
                      var ok = db.SaveChanges();*/

                    giftCard = cardFromDbDifferentBalance;
                }
            }
            //there's no associated giftcard, create a new one for this customer, linking to reward card
            else
            {
                var giftcard = new GiftCard
                {

                    payment_type = 4,
                    created_date = DateTime.Now,
                    created_by = RevelCardInsertUser,
                    updated_by = RevelCardInsertUser,
                    updated_date = DateTime.Now,
                    LinkingRevelCustomerID = customer.RevelId,
                    LinkingRevelRewardsCardNewID = rewardscardnew.Revelid,
                    initial_value = 0,
                    establishment = rewardscardnew.establishment,
                    number = rewardscardnew.number,
                    customer = customer.ResourceUri,
                    remaining_balance = giftCard.remaining_balance

                };

                giftcard.theCustomer = new Customer
                {
                    DBKEY_customer_id = customer.DBKEY_customer_id
                };
                giftcard.RewardsCardNew = new RewardsCardNew
                {
                    DBKEY_rewardscardnew_id = rewardscardnew.DBKEY_rewardscardnew_id
                };

                var webCreate = await writer.CreateGiftCard(giftcard);

                if (webCreate.Equals(0))
                {
                    db.GiftCards.Add(giftcard);
                    var saveCount = await db.SaveChangesAsync();

                    if (saveCount > 0)
                    {
                        //gift card was created
                    }
                }

            }

            ViewBag.GiftCard = giftCard;
            rewardscardnew.Customer = customer;
            ViewBag.RevelCardInsertUser = RevelCardInsertUser;
            ViewBag.CardLogs =
                db.RewardCardLogs.OrderByDescending(x => x.created_date)
                    .Where(x => x.reward_card_id == rewardscardnew.Revelid).Take(50)
                    .ToList();

            /*Replace the object in the cache*/
            CacheHelpers.UpdateCardCache(rewardscardnew, _cacheCardCollectionName);
            CacheHelpers.UpdateCardCache(giftCard, _cacheGiftCardCollectionName);


            ViewBag.DropdownCardType = GenerateLoyaltyCardDropdown();
            return View("Edit", rewardscardnew);
        }

        private void SetRefreshableCardTypeBasedOnOptionSelected(RewardsCardNew card, int optionSelected, IEnumerable<LoyaltyCardType> allLoyaltyCardTypes)
        {
            LoyaltyCardAdaptorUIService.SetUICardType(optionSelected, card, allLoyaltyCardTypes);
        }

        public async Task<ActionResult> Index()
        {
            ViewBag.Message = "Please wait while we load the cards...";
            return View();
        }

        [System.Web.Mvc.Authorize(Roles = "admin")]
        public async Task<ActionResult> IndexVIP()
        {

            return View("AltIndex", allCards.Where(x => x.is_vip_card == true).ToList());
        }

        [System.Web.Mvc.Authorize(Roles = "admin")]
        public async Task<ActionResult> IndexInvestor()
        {


            return View("AltIndex", allCards.Where(x => x.LoyaltyCardType != null).ToList());
        }


    }
}