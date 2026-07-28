using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.Reporting;
using Revel._808nd.com.Classes.Reporting.ReportingFactory;
using Revel._808nd.com.Models;
using Revel._808nd.com.ReportingModel;

namespace WebReboot.Grind._808nd.com.Controllers
{

    public class BIController : Controller
    {
        GrindContext db = new GrindContext();
        private List<SelectListItem> allUsers = new List<SelectListItem>();

        public BIController()
        {

        }

        private List<SelectListItem> GetUsersSelectListFromOrderItems(IEnumerable<string> users)
        {
            allUsers.Add(new SelectListItem { Value = "", Text = "None", Selected = true });
            var moreUsers =
                db.Users
                    .Where(x => x.is_active)
                    .ToList()
                    .FindAll(x => users.Any(anId => anId == x.resource_uri))
                    .Select(
                        x =>
                            new SelectListItem
                            {
                                Value = x.resource_uri.ToString(),
                                Text = x.last_name + ", " + x.first_name
                            })
                    .OrderBy(x => x.Text)
                    .ToList();

            return allUsers.Concat(moreUsers).ToList();
        }

        // GET: BI
        public ActionResult Index()
        {
            /* var start = DateTime.Now.AddDays(-2);
             var end = start.AddDays(1);
             */
            ViewBag.EstablishmentsSelect =
                db.Establishments.ToList().Select(x => new SelectListItem { Value = x.id, Text = x.name }).ToList();
            ViewBag.UsersSelect = allUsers;

            ViewBag.DateRange = new List<DateTime> { DateTime.Now, DateTime.Now };
            ViewBag.HasFired = false;
            return View("Index");
        }


        public async Task<ActionResult> IndexAutoRefresh()
        {
            ViewBag.EstablishmentsSelect =
                db.Establishments.ToList().Select(x => new SelectListItem { Value = x.id, Text = x.name }).ToList();
            ViewBag.UsersSelect = allUsers;

            ViewBag.DateRange = new List<DateTime> { DateTime.Now, DateTime.Now };
            ViewBag.HasFired = false;
            return View();
        }

        public ActionResult ShowFullProductSalesBreakdown()
        {
            ViewBag.EstablishmentsSelect =
                db.Establishments.Where(x => x.establishment_id != 2).ToList().Select(x => new SelectListItem { Value = x.id, Text = x.name }).ToList();
            ViewBag.UsersSelect = allUsers;

            return View();
        }

        [HttpPost]
        public ActionResult ShowFullProductSalesBreakdown(DateTime start, int noOfWeeks, int EstablishmentId, string UserURI = "")
        {
            ViewBag.EstablishmentsSelect =
            db.Establishments.Where(x => x.establishment_id != 2).ToList().Select(x => new SelectListItem { Value = x.id, Text = x.name }).ToList();


            ViewBag.UsersSelect = allUsers;

            var factory = new OrderItemReportFactory();

            var est = db.Establishments.First(x => x.establishment_id == EstablishmentId);


            var end = start.AddDays(7 * noOfWeeks);

            var orderItemsOUtPut = new List<OrderItem>();
            var reportData = new List<ProductOrderItemSummary>();

            reportData = factory.CreateProductOrderItemSummaryReport(new ReportContext
            {
                StartDate = start,
                EndDate = end,
                NoOfDaysInEachReportingPeriod = 7,
                IdOfStore = est.establishment_id
            }, new GrindContext(), out orderItemsOUtPut, UserURI);

            ViewBag.UsersSelect = GetUsersSelectListFromOrderItems(GetsUserForEstablishmentFromOrderItems(orderItemsOUtPut.ToOrderItemReportingItems()));

            return View(reportData.OrderBy(x => x.Period).ThenBy(x => x.ProductIdentifier).ToList());
        }




        public ActionResult SalesBreakdownToday()
        {
            ViewBag.EstablishmentsSelect =
                db.Establishments.Where(x => x.establishment_id != 2).ToList().Select(x => new SelectListItem { Value = x.id, Text = x.name }).ToList();
            ViewBag.UsersSelect = allUsers;

            return View();
        }

        [HttpPost]
        public ActionResult SalesBreakdownToday(int EstablishmentId, string UserURI)
        {
            var today = DateTime.Now;

            if (today.Hour >= 0 && today.Hour <= 3)
                today.AddDays(-1);

            var start = new DateTime(today.Year, today.Month, today.Day, 02, 00, 00);
            var noOfWeeks = 1;




            var factory = new OrderItemReportFactory();

            var est = db.Establishments.First(x => x.establishment_id == EstablishmentId);


            var end = start.AddDays(7 * noOfWeeks);


            var orderItemsOUtPut = new List<OrderItem>();
            var reportData = new List<ProductOrderItemSummary>();
            reportData = factory.CreateProductOrderItemSummaryReport(new ReportContext
            {
                StartDate = start,
                EndDate = end,
                NoOfDaysInEachReportingPeriod = 7,
                IdOfStore = est.establishment_id
            }, new GrindContext(), out orderItemsOUtPut, UserURI);



            ViewBag.EstablishmentsSelect =
        db.Establishments.Where(x => x.establishment_id != 2).ToList().Select(x => new SelectListItem { Value = x.id, Text = x.name }).ToList();

            ViewBag.UsersSelect = GetUsersSelectListFromOrderItems(GetsUserForEstablishmentFromOrderItems(orderItemsOUtPut.
                Where(x => x.establishment_id == est.establishment_id).ToList()
                .ToOrderItemReportingItems()));


            return View(reportData.OrderBy(x => x.Period).ThenBy(x => x.ProductIdentifier).ToList());
        }



        [HttpPost]
        public async Task<ActionResult> IndexAutoRefresh(IEnumerable<int> EstablishmentId)
        {
            if (EstablishmentId == null)
            {
                EstablishmentId = new List<int>() { 2 };

            }

            var today = DateTime.Now;

            if (today.Hour >= 0 && today.Hour <= 3)
                today.AddDays(-1);

            await GetBIResultsAndPlaceInViewBag(today, today.AddDays(1), EstablishmentId);


            return View();
        }


        private IEnumerable<string> GetsUserForEstablishmentFromOrderItems(List<OrderItemReportingItem> items)
        {
            return items.Select(x => x.CreatedBy).Distinct().ToList();

        }

        [HttpPost]
        public async Task<ActionResult> Index(DateTime start, DateTime end, IEnumerable<int> EstablishmentId)
        {
            await GetBIResultsAndPlaceInViewBag(start, end, EstablishmentId);

            return View("Index");
        }

        private async Task GetBIResultsAndPlaceInViewBag(DateTime start, DateTime end, IEnumerable<int> EstablishmentId)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<int> parsedEstablihsmentIds = new List<int>();

            if (EstablishmentId == null)
            {
                EstablishmentId = new List<int>() { 2 };

            }


            if (EstablishmentId.Contains(2))
            {
                parsedEstablihsmentIds =
                    db.Establishments.Where(x => x.establishment_id != 2).Select(x => x.establishment_id).ToList();
            }
            else
            {
                parsedEstablihsmentIds = EstablishmentId as List<int>;
            }


            /*    start = DateTime.Now.AddDays(-2);
                end = start.AddDays(1);*/
            var revelTImeStart = new DateTime(start.Year, start.Month, start.Day, 02, 00, 00);
            var revelTImeEnd = new DateTime(end.Year, end.Month, end.Day, 02, 00, 00);

            var datesToCycle = extension.railgunit.com.DateTimeExtensions.DateRange(revelTImeStart, revelTImeEnd).ToList();

            var topSellers = new List<TopSeller>();
            var productWatch = new List<TopSeller>();
            var avgBasket = new List<TopSeller>();
            var hourAndSpends = new List<HourAndSpend>();


            //day cycles to prevent memory errors

            db = new GrindContext();

            db.Configuration.ProxyCreationEnabled = false;
            db.Configuration.LazyLoadingEnabled = false;


            //get establishments
            /*            var est =
                                db.Establishments.AsNoTracking()
                                    .Where(x => parsedEstablihsmentIds.Any(anId => anId == x.establishment_id))
                                    .ToList();*/

            //get users
            var users = db.Users.ToList();

            //get products - can improve time here by restricting!!!!
            var products = db.Products.AsNoTracking()
                .Where(x => parsedEstablihsmentIds.Any(anId => anId == x.establishment_id)).ToList();
            var productIdsForEstablishment = products.Select(x => x.product_id).ToList();

            //GET YOUR ORDERS FOR EST AS ORDERREPORTITEMS
            var unfilteredOrders = db.Orders.AsNoTracking()
                .Where(x => x.created_date >= revelTImeStart)
                .Where(x => x.created_date <= revelTImeEnd)
                .ToList()
                .FindAll(x => parsedEstablihsmentIds.Any(anId => anId == x.establishment_id))
                .Select(x => new OrderReportingItem
                {
                    Id = (int)x.order_id,
                    ParentOrderId = x.bill_parent,
                    Amount = x.final_total,
                    DiscountReason = x.discount_reason,
                    CreatedBy = x.created_by
                })
                .ToList();

            //EXCLUDE SPLIT BILLS
            var orderIdsToExclude = FilterSplitBills(unfilteredOrders);

            var ordersWithoutSplitBills = unfilteredOrders.Where(x => !orderIdsToExclude.Any(anId => anId == x.Id)).ToList();
            //get order items + filter 

            var ordersItems =
                 ReportingExtensionMethods.FilterSplitBills(orderIdsToExclude, db.OrderItems
                    .AsNoTracking()
                    .Where(x => x.created_date >= revelTImeStart)
                    .Where(x => x.created_date <= revelTImeEnd)
                    .Where(x => x.deleted == false).ToList()
                    .ToOrderItemReportingItems());


            var normalItems = ordersItems
                .FindAll(x => productIdsForEstablishment.Any(anId => anId == x.ProductId)).ToList();

            var itemsWithNoProduct = ordersItems.Where(x => x.ProductId == 0).ToList();

            var comps = ordersItems.Where(x => x.ERVC_Type != "0").ToList();

            var allItems = normalItems.Concat(itemsWithNoProduct).ToList();


            var itemsExcludingCompsAndvoids = normalItems.Where(x => x.ERVC_Type == "0").ToList();
            //get orders


            var prodWatches = db.ProductWatches.AsNoTracking().ToList()
                .Where(x => productIdsForEstablishment.Any(anId => anId == x.Revel_Product_Id))
                .ToList();

            //run 
            topSellers = topSellers.Concat(await GetTopSellers(itemsExcludingCompsAndvoids, users)).ToList();
            productWatch = productWatch.Concat(await GetProductWatches(prodWatches, products, itemsExcludingCompsAndvoids)).ToList();
            avgBasket = avgBasket.Concat(await GetTopSellersByBucketSize(users, ordersWithoutSplitBills, itemsExcludingCompsAndvoids)).ToList();

            hourAndSpends =
                hourAndSpends.Concat(await OrderItemReportFactory.GetHourlySalesForOrderItems(revelTImeStart, revelTImeEnd, itemsExcludingCompsAndvoids)).ToList();


            /* ViewBag.HourAndSpends = new List<HourAndSpend>();*/
            ViewBag.HourAndSpends = hourAndSpends;
            ViewBag.AvgBasket = avgBasket;
            ViewBag.TopSellers = topSellers;
            ViewBag.ProductWatch = productWatch;

            ViewBag.DateRange = new List<DateTime> { revelTImeStart, revelTImeEnd };
            ViewBag.HasFired = true;

            ViewBag.EstablishmentsSelect =
                db.Establishments.ToList().Select(x => new SelectListItem { Value = x.id, Text = x.name }).ToList();

            ViewBag.UsersSelect = GetUsersSelectListFromOrderItems(GetsUserForEstablishmentFromOrderItems(normalItems));

            stopwatch.Stop();

            ViewBag.HowLong = stopwatch.Elapsed.TotalSeconds.ToString();
            ViewBag.Orders = ordersWithoutSplitBills.Count().ToString();
            ;
            ViewBag.Items = ordersItems.Sum(x => x.Quantity).ToString();
            ;
            ViewBag.Days = datesToCycle.Count().ToString();
            db.Dispose();
        }




        public async Task<List<TopSeller>> GetProductWatches(List<ProductWatch> prodWatches, List<Product> products, List<OrderItemReportingItem> orderItems)
        {

            var topSellerBuckets = new List<TopSeller>();
            foreach (var watch in prodWatches)
            {
                var prod = products.FirstOrDefault(x => x.product_id == watch.Revel_Product_Id);
                var orderItemsForThisProduct = orderItems.Where(x => x.ProductId == prod.product_id).ToList();

                if (prod != null)
                {
                    if (orderItemsForThisProduct.Count > 0)
                    {
                        topSellerBuckets.Add(new TopSeller
                        {
                            Name = prod.name,
                            NumberOfItems = orderItemsForThisProduct.Sum(x => x.Quantity).ToString(),
                            ValuePounds = orderItemsForThisProduct.Sum(x => x.Amount)

                        });
                    }
                }

            }

            return topSellerBuckets;
        }




        public async Task<List<TopSeller>> GetTopSellersByBucketSize(List<User> users, List<OrderReportingItem> orders, List<OrderItemReportingItem> orderItems)
        {

            //get order items + filter          
            var listOfOrderIdsWeWant = orderItems.Select(x => x.ParentOrderId).Distinct().ToList();
            var ordersFilteredToOUrs = orders.FindAll(x => listOfOrderIdsWeWant.Any(anId => anId == x.Id));
            var topSellerBuckets = new List<TopSeller>();

            foreach (var user in users)            {
                //So..this only works if the user opened the order
                var ordersForThisUser = ordersFilteredToOUrs.Where(x => x.CreatedBy == user.resource_uri).ToList();

                if (ordersForThisUser.Count() > 0)
                {

                    var listOfOrderIds = ordersForThisUser.Select(x => x.Id).ToList();
                    var itemsForTHeORders = orderItems.Where(x => listOfOrderIds.Any(anId => anId == x.ParentOrderId));


                    var ordersForthisUserDecimal = (decimal)ordersForThisUser.Count();
                    var itemsForTHeORdersDecimal = (decimal)itemsForTHeORders.Count();
                    var pureItemSalesDecimal = (decimal)itemsForTHeORders.Sum(x => x.Amount);

                    var avgBasketSize = (itemsForTHeORdersDecimal / ordersForthisUserDecimal);
                    var avgSpend = pureItemSalesDecimal / ordersForthisUserDecimal;

                    topSellerBuckets.Add(new TopSeller
                    {
                        Name = user.first_name + " " + user.last_name,
                        NumberOfItems = avgBasketSize.ToString(),
                        ValuePounds = avgSpend
                    });


                }

            }
            //cycle orders, assign each one to the user who created it

            return topSellerBuckets.OrderByDescending(x => x.ValuePounds).ToList();
        }



        public async Task<List<TopSeller>> GetTopSellers(List<OrderItemReportingItem> ordersItems, IList<User> users)
        {


            var topSellerBuckets = new List<TopSeller>();

            foreach (var user in users)
            {
                var orderItemsForThisUser = ordersItems.Where(x => x.CreatedBy == user.resource_uri).ToList();

                if (orderItemsForThisUser.Count > 0)
                {
                    topSellerBuckets.Add(new TopSeller
                    {
                        Name = user.first_name + " " + user.last_name,
                        NumberOfItems = orderItemsForThisUser.Sum(x => x.Quantity).ToString(),
                        ValuePounds = orderItemsForThisUser.Sum(x => x.Amount)
                    });
                }

            }
            //cycle orders, assign each one to the user who created it

            return topSellerBuckets.OrderByDescending(x => x.ValuePounds).ToList();

        }


        private List<int> FilterSplitBills(
            List<OrderReportingItem> unfilteredORders)
        {

            var ordersSplitBills = unfilteredORders
                .Where(x => x.ParentOrderId != null).ToList();

            var splitBillIds = ordersSplitBills.Select(x => (int)x.Id).ToList();

            //LEAVE IN DISCOUNTS FOR NOW
            /*     var ordersFullDiscount =
                     unfilteredORders
                         .Where(x => !String.IsNullOrEmpty(x.DiscountReason)).ToList();

                 var discountOrderIDs = ordersFullDiscount.Select(x => (int)x.Id);*/

            /*            var orderIdsToExclude = splitBillIds.Concat(discountOrderIDs).ToList();*/

            return splitBillIds;

        }


    }
}

