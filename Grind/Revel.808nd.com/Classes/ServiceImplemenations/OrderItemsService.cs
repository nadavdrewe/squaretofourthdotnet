using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Revel._808nd.com.Classes.WebserviceReader;
using Revel._808nd.com.Extensions;
using Revel._808nd.com.Models;

namespace Revel._808nd.com.Classes.ServiceImplementaitons
{
    public class OrderItemsService : IOrderItemsService, IDisposable
    {
        public async Task<int> GetAllDailyOrderItemsAndInsertAnyMissingRecords(string RevelAPIKEY, string RevelBaseURL, GrindContext _db, DateTime startDate, DateTime endDate)
        {
            Establishment revOrg = new Establishment(1, "Grind",
                RevelAPIKEY,
                new Uri(RevelBaseURL));


            RevelWebserviceDataReader webReader = new RevelWebserviceDataReader(revOrg);
            RevelDBWriter writer = new RevelDBWriter(_db);
            RevelDBReader DBReader = new RevelDBReader(revOrg);


            bool ok = false;


            //get all orders from our DB for that day
            try
            {
                List<OrderItem> orderItemsFromDB = await DBReader.GetOrderItems(startDate, endDate);
                List<int> dbOrderItemIDs = (from orderItems in orderItemsFromDB
                                            select (int)orderItems.orderitem_id).ToList();

                List<OrderItem> webServiceOrders = await webReader.GetOrderItems(startDate, endDate);
                List<int> webServiceOrderItemIDs = (from orderItems in webServiceOrders
                                                        // where orders.closed == true
                                                    select (int)orderItems.orderitem_id).ToList();

                var db = new GrindContext();
                var allProdsForEst = db.Products.ToList();

                var errors = new List<OrderItem>();

                //need to change this
                var assignedItems = OrderItem.AssignProductASKUAndEstablishmentToOrderItems(allProdsForEst,
                    webServiceOrders, out errors);
                //

                var longStringOFErrors = "";

                foreach (var item in errors)
                {
                    longStringOFErrors += item.product_name_override + ", ";
                    var orderForErrorItems = db.Orders.FirstOrDefault(x => x.order_id == item.parent_order_id);
                    if (orderForErrorItems != null)
                    {
                        item.establishment = orderForErrorItems.establishment;

                        //try and convert the key
                        try
                        {
                            item.establishment_id =
                                                RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(orderForErrorItems.establishment);
                        }
                        catch (Exception)
                        {
                            try
                            {
                                item.establishment_id =
                                                   RevelHelper.ConvertEstablishmentWithHyphenToPrimaryKey(orderForErrorItems.establishment);
                            }
                            catch (Exception)
                            {


                            }
                        }

                    }


                }

                if (errors.Count() > 0)
                {
                    assignedItems.AddRange(errors);

                }
                //add the errors back into the assigned items, cos we still want them



                var orderItemIdsToInsert = webServiceOrderItemIDs.Except(dbOrderItemIDs);

                GrindContext grindContext = new GrindContext();

                List<OrderItem> orderItemsToInsert = new List<OrderItem>();

                foreach (var item in orderItemIdsToInsert)
                {
                    OrderItem OrderToInsert = assignedItems.Where(c => c.orderitem_id == item).FirstOrDefault();
                    orderItemsToInsert.Add(OrderToInsert);

                }


                //check any specific orders exist in new order set
                //missing orders


                if (orderItemsToInsert.Any())
                {
                    grindContext.OrderItems.AddRange(orderItemsToInsert);
                    grindContext.SaveChanges();
                }



                return orderItemsToInsert.Count();
            }
            catch (Exception)
            {

                throw;
            }
            //project new list of ints


            //get all orders from Revel for that day for corresponding period

            //project list of ints


            return 0;
        }


        public IEnumerable<dynamic> LogItemsWithNoSKU(IEnumerable<OrderItem> items, RevelContextBase db, Establishment est)
        {
            var itemsWithoutSKU = items
                .Where(x => x.sku == "" || x.sku == null || x.sku.Trim().ToUpper() == "UNIDENTIFIED")
                .GroupBy((x) => new { x.product_id, x.product_name_override, x.DBKEY_orderitem_id })
                .Select(t => new
                {
                    DBKEY_orderitem_id = t.Key.DBKEY_orderitem_id,
                    Name = t.Key.product_name_override,
                    ProductId = t.Key.product_id
                })
                .ToList();

            if (itemsWithoutSKU.Any())
            {

                foreach (var item in itemsWithoutSKU)
                {
                    db.SystemErrors.Add(new SystemError
                    {
                        ErrorDate = DateTime.Now,
                        Establishment = est.establishment_id,
                        Description = "SKU Error - ProductID: " + item.ProductId + " - " + item.Name
                    });
                }

                db.SaveChanges();


            }
            return itemsWithoutSKU;
        }


        public IEnumerable<dynamic> LogItemsWithNoSKU(IEnumerable<OrderItem> items, RevelContextBase db, Brand brand)
        {
            var itemsWithoutSKU = items
                .Where(x => String.IsNullOrWhiteSpace(x.sku))
                .Where(x => x.sku == "" || x.sku.Trim().ToUpper() == "UNIDENTIFIED")
                .GroupBy((x) => new { x.sku, x.product_id, x.product_name_override })
                .Select(t => new
                {
                    Sku = t.Key.sku,
                    Name = t.Key.product_name_override,
                    ProductId = t.Key.product_id
                })
                .ToList();

            if (itemsWithoutSKU.Any())
            {

                foreach (var item in itemsWithoutSKU)
                {
                    db.SystemErrors.Add(new SystemError
                    {
                        ErrorDate = DateTime.Now,
                        Brand = brand.brand_id,
                        Description = "SKU Error - ProductID: " + item.ProductId + " - " + item.Name
                                      + " SKU " + item.Sku
                    });
                }

                db.SaveChanges();


            }
            return itemsWithoutSKU;
        }


        public async Task<List<OrderItem>> GetOrderItems(Brand brand, Establishment est, RevelContextBase db, DateTime startdate, DateTime endDate)
        {
            var prods = db.Products.Where(x => x.establishment == est.resource_uri).ToList();

            Establishment TopLevelOrg = new Establishment(1, "ARevelOrg",
    brand.key_secret,
    new Uri(brand.revel_base_url));
            var webReader = new RevelWebserviceDataReader(TopLevelOrg);

            var periodOrderItems = new List<OrderItem>();
            var estIdSplit = est.resource_uri.Split('/')[3];

            var query = "/resources/OrderItem?format=json&created_date__gt={0}&created_date__lte={1}&limit=0&establishment=" + estIdSplit;
            var startdateString = startdate.ToString("yyyy-MM-ddTHH:mm:ss");
            var endDateString = endDate.ToString("yyyy-MM-ddTHH:mm:ss");

            string webURL = String.Format(query,
                startdateString,
                endDateString);

            var orderItemsAstype = new OrderItem();
            var orders = await webReader.GetRevelWebserviceData<OrderItem>(orderItemsAstype, webURL);

            foreach (var order in orders)
            {
                order.brand = brand.resource_uri;
                order.establishment = est.resource_uri;
                try
                {
                    var prod = prods.Where(x => x.resource_uri == order.product).FirstOrDefault();
                    order.sku = prod.sku;

                }
                catch (Exception ex)
                {
                    order.sku = "UNIDENTIFIED";

                }
                periodOrderItems.Add(order);
            }



            return periodOrderItems;

        }

        /// <summary>
        /// Pass brand - get all items for entire brand in one call
        /// </summary>
        /// <param name="brand"></param>
        /// <param name="db"></param>
        /// <param name="startdate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>




        public void Dispose()
        {

        }

        async Task<IEnumerable<OrderItem>> IOrderItemsService.GetOrderItems(Brand brand, RevelContextBase db, DateTime startdate, DateTime endDate)
        {
            var estsForBrand = db.Establishments.Where(x => x.brand == brand.resource_uri);
            Establishment TopLevelOrg = new Establishment(1, "ARevelOrg",
    brand.key_secret,
    new Uri(brand.revel_base_url));
            var webReader = new RevelWebserviceDataReader(TopLevelOrg);


            var periodOrderItems = new List<OrderItem>();

            foreach (var est in estsForBrand)
            {
                var estIdSplit = est.resource_uri.Split('/')[3];

                var query = "/resources/OrderItem?format=json&created_date__gt={0}&created_date__lte={1}&limit=0&establishment=" + estIdSplit;
                var startdateString = startdate.ToString("yyyy-MM-ddTHH:mm:ss");
                var endDateString = endDate.ToString("yyyy-MM-ddTHH:mm:ss");

                string webURL = String.Format(query,
                    startdateString,
                    endDateString);

                var orderItemsAstype = new OrderItem();
                var orders = await webReader.GetRevelWebserviceData<OrderItem>(orderItemsAstype, webURL);


                periodOrderItems.AddRange(orders);

            }

            return periodOrderItems;

        }


        public async Task<IEnumerable<OrderItem>> GetOrderItemsForBrand(Brand brand, RevelContextBase db, DateTime startdate, DateTime endDate, int limit)
        {
            var prods = db.Products.Where(x => x.brand_id == brand.brand_id).ToList();
            var ests = db.Establishments.Where(x => x.db_brand_id == brand.brand_id).ToList();

            Establishment TopLevelOrg = new Establishment(1, "ARevelOrg",
    brand.key_secret,
    new Uri(brand.revel_base_url)); 
            var webReader = new RevelWebserviceDataReader(TopLevelOrg);


            var periodOrderItems = new List<OrderItem>();


            var query = "/resources/OrderItem?format=json&created_date__gt={0}&created_date__lte={1}&limit=" + limit.ToString();

            var startdateString = startdate.ToString("yyyy-MM-ddTHH:mm:ss");
            var endDateString = endDate.ToString("yyyy-MM-ddTHH:mm:ss");

            string webURL = String.Format(query,
                startdateString,
                endDateString);

            var orderItemsAstype = new OrderItem();
            var orders = await webReader.GetRevelWebserviceData<OrderItem>(orderItemsAstype, webURL);

            foreach (var order in orders)
            {
                order.brand = brand.resource_uri;

                try
                {

                    var prod = prods.Where(x => x.resource_uri == order.product).FirstOrDefault();
                    if (prod == null)
                    {
                        var singleProdQuery = String.Format("/resources/Product/?format=json&limit=600&id={0}", order.product_id);
                        var instanceProduct = new Product();
                        var singleproduct = await webReader.GetRevelWebserviceData<Product>(instanceProduct, singleProdQuery);
                        prod = singleproduct.FirstOrDefault();
                        prod.db_brand_id = brand.brand_id;
                        prod.establishment_id = RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(prod.establishment);
                        prods.Add(prod); //so we don't hav to get it again next time!
                    }
                    if (prod.sku == "")
                    {
                        var sttop = "";
                    }
                    order.db_establishment_id = prod.db_establishment_id;
                    order.db_brand_id = prod.db_brand_id;
                    order.sku = prod.sku;
                    order.establishment = prod.establishment;

                }
                catch (Exception ex)
                {
                    order.sku = "UNIDENTIFIED";

                }
                periodOrderItems.Add(order);
            }


            return periodOrderItems;
        }



        public IEnumerable<OrderItem> FilterVoidItems(IEnumerable<OrderItem> orderItems, out List<OrderItem> voided)
        {
            voided = GetVoidedItems(orderItems).ToList();
            var items = orderItems.Except(voided).ToList();

            return items;
        }

        public IEnumerable<OrderItem> FilterDiscountedItems(IEnumerable<OrderItem> orderItems, out List<OrderItem> discounted)
        {
            discounted = GetDiscountedItems(orderItems) as List<OrderItem>;
            var items = orderItems.Except(discounted).ToList();

            return items;
        }

        public IEnumerable<OrderItem> GetDiscountedItems(IEnumerable<OrderItem> orderItems)
        {

            var discounted = new List<OrderItem>();
            var itemsWithDiscount = orderItems.Where(x => x.discount_amount > 0).ToList();

            if (itemsWithDiscount != null)
            {
                if (itemsWithDiscount.Count > 0)
                {
                    discounted = itemsWithDiscount;
                }
            }

            return discounted;
        }

        public IEnumerable<OrderItem> GetVoidedItems(IEnumerable<OrderItem> orderItems)
        {
            var voided = orderItems.Where(x => x.voided_reason != "").ToList();

            return voided;
        }

    }

}
