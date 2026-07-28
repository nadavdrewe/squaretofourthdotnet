using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revel._808nd.com.Classes.WebserviceReader;
using Revel._808nd.com.Extensions;
using Revel._808nd.com.Models;

namespace Revel._808nd.com.Classes.ServiceImplemenations
{
    public class OrderService
    {
        private string RevelAPIKEY { get; set; }
        private string RevelBaseURL { get; set; }
        private RevelContextBase _db { get; set; }

        public OrderService(string RevelAPIKEY, string RevelBaseURL, RevelContextBase db)
        {
            this.RevelAPIKEY = RevelAPIKEY;
            this.RevelBaseURL = RevelBaseURL;
            this._db = db;
        }


        


        public async Task UpdateOrders(DateTime startDate, DateTime endDate)
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
                List<Order> ordersFromDB = await DBReader.GetOrdersSinglePull(startDate, endDate);
                List<int> dbOrderIDs = (from orders in ordersFromDB
                                        select (int)orders.order_id).ToList();

                List<Order> webServiceOrders = await webReader.GetOrdersSinglePull(startDate, endDate);
                List<int> webServiceOrderIDs = (from orders in webServiceOrders
                                                    // where orders.closed == true
                                                select (int)orders.order_id).ToList();



                var splitBills = webServiceOrders.Where(x => x.bill_parent != null).ToList();
                

                //WEBSERVICE VARS
                var web_closedORders = webServiceOrders; //.Where(x => x.closed = true);
                var web_closedAndUnpaidFalse = webServiceOrders
                    /*.Where(x => x.closed = true)
                    .Where(u => u.is_unpaid == "False")*/
                    .ToList();



                var orderIdsToInsert = webServiceOrderIDs.Except(dbOrderIDs);

                GrindContext grindContext = new GrindContext();

                List<Order> ordersToInsert = new List<Order>();

                foreach (var item in orderIdsToInsert)
                {
                    Order OrderToInsert = webServiceOrders.Where(c => c.order_id == item).FirstOrDefault();
                    ordersToInsert.Add(OrderToInsert);

                }

                if (ordersToInsert.Any())
                {
                    grindContext.Orders.AddRange(ordersToInsert);
                    grindContext.SaveChanges();
                }

                //end insert


                //orders to be replaced
                List<Order> OrdersToBeReplacedInDB = new List<Order>();
                List<Order> OrdersToBeDeletedFromDb = new List<Order>();

                //refresh DB after insert above
                ordersFromDB = await DBReader.GetOrdersSinglePull(startDate, endDate);

                //create new context with all records
                var InsertGrindContext = new GrindContext();

                //cycle through web closed Order
                foreach (Order webOrder in web_closedAndUnpaidFalse)
                {
                    //cycle through ALL DB orders!
                    foreach (Order dbOrder in ordersFromDB)
                    {
                        if (webOrder.order_id == dbOrder.order_id)
                        {

                            // OrdersToBeDeletedFromDb.Add(webOrder);
                            if (!((webOrder.closed).Equals(dbOrder.closed))
                                ||
                                !((webOrder.is_unpaid).Equals(dbOrder.is_unpaid))
                                ||
                                !((decimal.Round(webOrder.final_total, 2)).Equals((decimal.Round(dbOrder.final_total))))
                                || !((decimal.Round(webOrder.tax)).Equals(decimal.Round(dbOrder.tax)))
                                )
                            {
                                //check if it's already added

                                var exists =
                                    OrdersToBeDeletedFromDb.Where(e => e.DBKEY_order_id.Equals(dbOrder.DBKEY_order_id))
                                        .Count();

                                if (exists < 1)
                                {
                                    try
                                    {
                                        OrdersToBeDeletedFromDb.Add(dbOrder);
                                        OrdersToBeReplacedInDB.Add(webOrder);
                                    }
                                    catch (Exception ex)
                                    {

                                        throw ex;
                                    }
                                }

                            }



                        }
                    }
                }




                //delete old
                if (OrdersToBeDeletedFromDb.Any())
                {
                    foreach (var order in OrdersToBeDeletedFromDb)
                    {

                        //check it doesn't exist first - duplicates
                        try
                        {

                            InsertGrindContext.Orders.Remove(order);

                        }

                        catch (Exception EX)
                        {
                            //need to add it probs
                            try
                            {
                                InsertGrindContext.Orders.Attach(order);
                                InsertGrindContext.Entry(order).State = EntityState.Modified;

                                InsertGrindContext.Orders.Remove(order);

                            }
                            catch (Exception ex)
                            {
                                var omg = "";
                                throw ex;
                            }
                            finally
                            {
                                //          InsertGrindContext.SaveChanges();
                            }

                        }
                    }


                    try
                    {

                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }

                    InsertGrindContext.SaveChanges();
                }

                //add new
                if (OrdersToBeReplacedInDB.Any())
                {

                    //check it doesn't exist first - duplicates
                    InsertGrindContext.Orders.AddRange(OrdersToBeReplacedInDB);
                    InsertGrindContext.SaveChanges();

                }


            }
            catch (Exception ex)
            {

                throw ex;
            }



        }

    }
}
