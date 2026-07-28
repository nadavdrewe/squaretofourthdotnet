using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using NUnit.Framework;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.Logging;
using Revel._808nd.com.Classes.ServiceImplementaitons;
using Revel._808nd.com.Extensions;
using Revel._808nd.com.Models;
using web.fourth.revel.com.Models;

namespace web.fourth.revel.com.Controllers
{
    public class OrderItemController : Controller
    {
        private RevelContext db = new RevelContext();
        private OrderItemsService orderItemService = new OrderItemsService();
        List<SelectListItem> brandDDL = new List<SelectListItem>();
        List<SelectListItem> estDDL = new List<SelectListItem>();


        public OrderItemController()
        {
            //var brands = db.Brands.ToList();
            //var ests = db.Establishments.ToList();

            //brandDDL.Add(new SelectListItem { Text = "None", Value = "0", Selected = true });
            //foreach (var brand in brands)
            //{

            //    brandDDL.Add(new SelectListItem { Text = brand.name, Value = brand.brand_id.ToString(), Selected = true });
            //}

            //estDDL.Add(new SelectListItem { Text = "None", Value = "0", Selected = true });

            //var allEsts = db.Establishments.ToList();

            //if (allEsts.Count > 0)
            //{
            //    foreach (var est in db.Establishments.ToList())
            //    {

            //        var currentBrand = brands.Find(x => x.brand_id == est.db_brand_id);

            //        if (currentBrand != null)
            //        {
            //            estDDL.Add(new SelectListItem { Text = currentBrand.name + " - " + est.name, Value = est.DBKEY_establishment_id.ToString(), Selected = true });
            //        }
                    
            //    }
            //}
        }
        // GET: OrderItem

        public async Task<ActionResult> ProductIndex()
        {

            ViewBag.Brands = brandDDL;
            ViewBag.Establishments = estDDL;

            return View(new List<ProductOrderItemSummary>());
        }


        [HttpPost]
        public ActionResult ProductIndex(string Brands, string Establishments, DateTime start, DateTime end, int voids = 0)
        {

            ViewBag.Brands = brandDDL;
            ViewBag.Establishments = estDDL;
            var productOrderItemSummary = new ProductOrderItemSummary();

            if (Brands != "0" || Establishments != "0")
            {
                var brandName = "";
                var EstName = "";

                var bId = Convert.ToInt32(Brands);

                var stDt = new DateTime(start.Year, start.Month, start.Day, 02, 00, 00);
                var endDt = new DateTime(end.Year, end.Month, end.Day, 02, 00, 00);

                var items = new List<OrderItem>();
                if (Establishments != "0")
                {
                    var estid = Convert.ToInt32(Establishments);
                    var est = db.Establishments.FirstOrDefault(x => x.DBKEY_establishment_id == estid);
                    EstName = est.name;

                    items = db.OrderItems
                        .Where(x => x.db_establishment_id == est.DBKEY_establishment_id)
                        .Where(x => x.created_date >= stDt)
                        .Where(x => x.created_date <= endDt)
                        .ToList();

                    productOrderItemSummary.StoreType = "Establishment";
                    productOrderItemSummary.StoreIdentifier = est.name;
                }
                else
                {
                    var brand = db.Brands.First(x => x.brand_id.Equals(bId));
                    brandName = brand.name;

                    items = db.OrderItems.Where(x => x.db_brand_id == brand.brand_id)
                        .Where(x => x.created_date >= stDt)
                        .Where(x => x.created_date <= endDt)
                        .ToList();

                    //test items

                    productOrderItemSummary.StoreType = "Brand";
                    productOrderItemSummary.StoreIdentifier = brand.name;
                }



                switch (voids)
                {
                    case 0:
                        break;
                    case 1:
                        items = items.Where(v => v.voided_date == null).ToList();
                        break;
                    case 2:
                        items = items.Where(v => v.voided_date != null).ToList();
                        break;
                }

                var item = items.Where(x => x.sku == "963753").ToList();
                var groupedProducts = items.GroupBy(x => x.sku).ToList();


                var testGroup = groupedProducts.Select(x => new
                {

                }); ;

                var backtoproducts = groupedProducts.ToList();



                var testDiscounts = from @group in groupedProducts
                                    select new
                                    {
                                        Discounts = @group.Where(x => x.sku == "963753" && x.discount_amount > 0).Sum(x => x.discount_amount)
                                    };


                var testQuantity = from @group in groupedProducts
                                   select new
                                   {
                                       SUM = @group.Sum(x => x.quantity)
                                   };

                var testquantity = from @group in groupedProducts
                                   select new
                                   {
                                       sum = @group.Sum(x => x.quantity)
                                   };

                var espresso = items.Where(x => x.db_establishment_id == 90 && x.sku == "11181").ToList();
                var test = espresso;



                var groupsToReturn = groupedProducts.Select(x => new ProductOrderItemSummary
                {
                    StartDate = stDt,
                    EndDate = endDt,
                    //TotalDiscountTax = x.Where(i => i.discount_taxed == true).Sum(i => i.tax_amount),
                    TotalDiscount = x.Sum(i => i.discount_amount),
                    TotalPureSales = x.Sum(i => i.pure_sales),
                    TotalTax = x.Sum(i => i.tax_amount),

                    DifferentProducts = x.Count(),
                    ProductIdentifier = x.First().product,
                    ProductName = x.First().product_name_override,
                    SKU = x.First().sku,
                    StoreIdentifier = productOrderItemSummary.StoreIdentifier,
                    StoreType = productOrderItemSummary.StoreType,
                    TotalQuantity = x.Sum(i => i.quantity),
                    TotalVoided = x.Where(i => i.voided_date != null).Count()
                }); ;
                //select new ProductOrderItemSummary
                //{
                //    StartDate = stDt,
                //    EndDate = endDt,
                //    TotalDiscountTax = @group.Where(x => x.discount_taxed == true).Sum(x => x.tax_amount),

                //    TotalDiscount = @group.Sum(x => x.discount_amount), //minus TotalDiscountTax above

                //    TotalPureSales = @group.Where(x => x.voided_date == null).Sum(x => x.pure_sales),
                //    TotalTax = @group.Where(x => x.voided_date == null).Sum(x => x.tax_amount),

                //    DifferentProducts = @group.Count(),
                //    ProductIdentifier = @group.First().product,
                //    ProductName = @group.First().product_name_override,
                //    SKU = @group.First().sku,
                //    StoreIdentifier = productOrderItemSummary.StoreIdentifier,
                //    StoreType = productOrderItemSummary.StoreType,
                //    TotalQuantity = @group.Where(x => x.voided_reason == "").Sum(x => x.quantity),
                //    TotalVoided = @group.Count(x => x.voided_reason != "")

                //}).ToList();

                ViewBag.OrderSummary = productOrderItemSummary;

                ViewBag.Message = "Showing Brand:" + brandName + "||Establishment:" + EstName + "||From:" + stDt +
                                 "||To: " + endDt + "||Item Count: " + items.Count() + "||Total Quantity: "
                                 + items.Sum(c => c.quantity) + "||Total Pure Sales (inc voids): " + items.Sum(x => x.pure_sales)
                                 + "||Total Pure Sales (exc voids): " + items.Where(x => x.voided_date == null).Sum(x => x.pure_sales)
                                 + "||Voided Items: " + items.Where(x => x.voided_date != null).Count() + "||Voided Total Pure Sales: " + items.Where(x => x.voided_date != null).Sum(x => x.pure_sales) + "||Discounted Items: " + items.Where(x => x.discount_amount > 0).Count()
                                 + "|| Items Total Discount Amount: " + items.Where(x => x.voided_date == null).Sum(x => x.discount_amount);

                var test11 = groupsToReturn.Where(x => x.SKU == "963753");
                //void filter

                return View(groupsToReturn);


            }
            return View(new List<ProductOrderItemSummary>());

        }




        public async Task<ActionResult> Index()
        {


            ViewBag.Brands = brandDDL;
            ViewBag.Establishments = estDDL;

            return View(await db.OrderItems.ToListAsync());
        }

        [HttpPost]
        public async Task<ActionResult> Index(string Brands, string Establishments, DateTime start, DateTime end)
        {
            ViewBag.Brands = brandDDL;
            ViewBag.Establishments = estDDL;
            var orderItemSummary = new OrderItemSummary();


            if (Brands != "0" || Establishments != "0")
            {
                var brandName = "";
                var EstName = "";


                var bId = Convert.ToInt32(Brands);



                var stDt = new DateTime(start.Year, start.Month, start.Day, 03, 00, 00);
                var endDt = new DateTime(end.Year, end.Month, end.Day, 03, 00, 00);

                var items = new List<OrderItem>();
                if (Establishments != "0")
                {
                    var estid = Convert.ToInt32(Establishments);
                    var est = db.Establishments.FirstOrDefault(x => x.DBKEY_establishment_id == estid);

                    items = db.OrderItems
                        .Where(x => x.establishment == est.resource_uri)
                        .Where(x => x.created_date >= stDt)
                        .Where(x => x.created_date <= endDt)
                        .ToList();

                    orderItemSummary.StoreType = "Establishment";
                    orderItemSummary.StoreIdentifier = est.name;
                }
                else
                {
                    var brand = db.Brands.First(x => x.brand_id.Equals(bId));

                    items = db.OrderItems.Where(x => x.brand == brand.resource_uri)
                        .Where(x => x.created_date >= stDt)
                        .Where(x => x.created_date <= endDt)
                        .ToList();

                    orderItemSummary.StoreType = "Brand";
                    orderItemSummary.StoreIdentifier = brand.name;
                }

                ViewBag.Message = "Showing Brand:" + brandName + "||Establishment:" + EstName + "||From:" + stDt +
                                  "||To:" + endDt;

                orderItemSummary.StartDate = stDt;
                orderItemSummary.EndDate = endDt;
                orderItemSummary.TotalDiscount = items.Sum(x => x.discount_amount);
                orderItemSummary.TotalPureSales = items.Sum(x => x.pure_sales);
                orderItemSummary.TotalTax = items.Sum(x => x.tax_amount);
                orderItemSummary.DifferentProducts = items.Select(x => x.product).Distinct().Count();

                ViewBag.OrderSummary = orderItemSummary;


                return View(items);
            }


            return View(new List<OrderItem>());

        }

        public ActionResult PullOrderItems()
        {

            ViewBag.Brands = brandDDL;
            ViewBag.Establishments = estDDL;

            return View(new List<OrderItem>());
        }

        [HttpPost]
        public async Task<ActionResult> PullOrderItems(string Brands, DateTime start, DateTime end)
        {
            var bId = Convert.ToInt32(Brands);
            var brand = db.Brands.FirstOrDefault(x => x.brand_id == bId);

            if (Brands != "0")
            {
                var syncStart = new DateTime(start.Year, start.Month, start.Day, 03, 00, 00);
                var syncEnd = new DateTime(end.Year, end.Month, end.Day, 03, 00, 00);

                var orderItems = await PullOrderItemsFromRevelForBrand(orderItemService, brand, syncStart, syncEnd);

                ViewBag.Brands = brandDDL;
                ViewBag.Establishments = estDDL;



                return PartialView("~/Views/Partial/OrderItemTable.cshtml", orderItems);
            }

            return View();
        }



        public async Task<IEnumerable<OrderItem>> PullOrderItemsFromRevelForBrand(OrderItemsService itemsService, Brand brandToPullOrdersFor, DateTime start, DateTime end, int limit = 0)
        {
            //For logging
            var user = "Automated";
            if (User != null)
            {
                user = User.Identity.Name;
            }

            //Main

            try
            {
                var orderItems = new List<OrderItem>();

                orderItems = (List<OrderItem>)await itemsService.GetOrderItemsForBrand(brandToPullOrdersFor, db, start, end, limit);

                if (orderItems.Any())
                {
                    //TRANSACTION

                    //CLEAR THE ONES FOR THE SAME RANGE
                    var itemsToRemove =
                        db.OrderItems
                        .Where(x => x.created_date >= start && x.created_date <= end)
                        .Where(x => x.db_brand_id == brandToPullOrdersFor.brand_id)
                            .ToList();

                    if (itemsToRemove.Count > 0)
                    {
                        db.OrderItems.RemoveRange(itemsToRemove);

                        db.SaveChanges();
                    }
                    //ADD NEW
                    var addOK = db.OrderItems.AddRange(orderItems);
                    var saveOk = db.SaveChanges();


                    //LOG

                    var voided = new List<OrderItem>();
                    var discounted = new List<OrderItem>();

                    voided = itemsService.GetVoidedItems(orderItems) as List<OrderItem>;
                    discounted = itemsService.GetDiscountedItems(orderItems) as List<OrderItem>;

                    var discountAmount = discounted.Sum(x => x.discount_amount);

                    var discountTax = 0.00M;
                    discountTax = discounted.Where(x => x.discount_taxed == true).Sum(x => x.tax_amount);


                    //END LOG

                    /*transaction.Commit();*/




                    return orderItems;
                }
                else
                {
                    var log = new ScheduledTaskLog
                    {

                        Detail = "Brand:" + brandToPullOrdersFor.name + " " + "No of items:" + orderItems.Count(),
                        FireTime = DateTime.Now,
                        Result = 1,
                        Message = "OrderItems count was zero for this brand",
                        Brand = brandToPullOrdersFor.brand_id,
                        BrandName = brandToPullOrdersFor.name,
                        Establishment = 0,
                        EstablishmentName = "",
                        TotalItemCount = orderItems.Count(),
                        TotalPounds = orderItems.Sum(x => x.pure_sales),
                        LogType = "LOCAL",
                        ContainerEndDate = end,
                        ContainerStartDate = start,
                        User = user,
                        TotalItemQuantity = orderItems.Sum(x => x.quantity),
                        TotalVAT = orderItems.Sum(x => x.tax_amount)


                    };

                    db.ScheduledTaskLogs.Add(log);
                    db.SaveChanges();
                    //END LOG
                }
            }
            catch (Exception ex)
            {
                /* transaction.Rollback();*/

                var log = new ScheduledTaskLog
                {
                    Detail = "The scheduler failed",
                    FireTime = DateTime.Now,
                    Result = 0,
                    Message = "OrderItem Controller cannot complete transaction from Revel to local Db ",
                    Brand = brandToPullOrdersFor.brand_id,
                    BrandName = brandToPullOrdersFor.name,
                    Establishment = 0,
                    EstablishmentName = "",
                    LogType = "LOCAL",
                    ContainerEndDate = start,
                    ContainerStartDate = end,
                    User = user

                };

                db.ScheduledTaskLogs.Add(log);
                db.SaveChanges();

                throw new Exception(
                    "OrderItem Controller cannot complete transaction from Revel to local Db", ex);
            }


            /*}*/

            return new List<OrderItem>();
        }



        // GET: OrderItem/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderItem orderItem = db.OrderItems.Find(id);
            if (orderItem == null)
            {
                return HttpNotFound();
            }
            return View(orderItem);
        }

        // GET: OrderItem/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OrderItem/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "DBKEY_orderitem_id,catering_complete,commission,cost,course_number,created_by,created_date,crv_value,cup_qty,cup_weight,deleted,dining_option,discount,discount_amount,discount_reason,discount_rule_amount,discount_taxed,exchange_discount,exchanged,expedited,orderitem_id,initial_price,is_cold,is_coupon,is_gift,kitchen_completed,modifier_amount,modifier_cost,modifieritems,on_hold,order,order_local_id,price,printed,product,product_name_override,quantity,resource_uri,shared,special_request,split_parts,split_type,split_with_seat,station,tax_amount,tax_rate,tax_rebate,taxed_flag,temp_sort,updated_by,updated_date,uuid,voided_by,voided_date,voided_reason,weight,total_price_after_tax,total_price_after_discount,parent_order_id,product_id,discount_id,pure_sales,establishment,brand,sku,start_time,theAddress")] OrderItem orderItem)
        {
            if (ModelState.IsValid)
            {
                db.OrderItems.Add(orderItem);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(orderItem);
        }

        // GET: OrderItem/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderItem orderItem = db.OrderItems.Find(id);
            if (orderItem == null)
            {
                return HttpNotFound();
            }
            return View(orderItem);
        }

        // POST: OrderItem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "DBKEY_orderitem_id,catering_complete,commission,cost,course_number,created_by,created_date,crv_value,cup_qty,cup_weight,deleted,dining_option,discount,discount_amount,discount_reason,discount_rule_amount,discount_taxed,exchange_discount,exchanged,expedited,orderitem_id,initial_price,is_cold,is_coupon,is_gift,kitchen_completed,modifier_amount,modifier_cost,modifieritems,on_hold,order,order_local_id,price,printed,product,product_name_override,quantity,resource_uri,shared,special_request,split_parts,split_type,split_with_seat,station,tax_amount,tax_rate,tax_rebate,taxed_flag,temp_sort,updated_by,updated_date,uuid,voided_by,voided_date,voided_reason,weight,total_price_after_tax,total_price_after_discount,parent_order_id,product_id,discount_id,pure_sales,establishment,brand,sku,start_time,theAddress")] OrderItem orderItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderItem).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(orderItem);
        }

        // GET: OrderItem/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderItem orderItem = db.OrderItems.Find(id);
            if (orderItem == null)
            {
                return HttpNotFound();
            }
            return View(orderItem);
        }

        // POST: OrderItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            OrderItem orderItem = db.OrderItems.Find(id);
            db.OrderItems.Remove(orderItem);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
