using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.ServiceImplementaitons;
using Revel._808nd.com.Classes.WebserviceReader;
using Revel._808nd.com.Extensions;
using Revel._808nd.com.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.RevelToCaternet
{
    public class CaternetOrderItemService
    {


        public async Task<IEnumerable<OrderItem>> GetOrderItemsForEstabllishment(Establishment est, RevelContext db, DateTime startdate, DateTime endDate, int limit)
        {
            var prods = db.Products.Where(x => x.establishment_id == est.establishment_id).ToList();
            var webReader = new RevelWebserviceDataReader(est);

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

                try
                {
                    var prod = prods.Where(x => x.resource_uri == order.product).FirstOrDefault();
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


        public async Task<IEnumerable<OrderItem>> PullOrderItemsFromRevelForEstablishment(OrderItemsService itemsService, Brand brandToPullOrdersFor, RevelContextBase db, DateTime start, DateTime end, int limit = 0)
        {

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
                    ContainerStartDate = end
                };

                db.ScheduledTaskLogs.Add(log);
                db.SaveChanges();

                throw new Exception(
                    "OrderItem Controller cannot complete transaction from Revel to local Db", ex);
            }


            /*}*/

            return new List<OrderItem>();
        }


    }
}
