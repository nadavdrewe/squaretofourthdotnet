using System;
using System.Collections.Generic;
using System.Linq;
using Revel._808nd.com.Models;

namespace Revel._808nd.com.Classes
{

    public partial class OrderItem
    {

        public List<OrderItem> GetOrderItemsByDate(DateTime StartDate, DateTime EndDate)
        {

            using (GrindContext _db = new GrindContext())
            {
                List<OrderItem> items = (_db.OrderItems
                    .Where(x => x.created_date >= StartDate)
                    .Where(x => x.created_date <= EndDate)
                    ).ToList();
                    

                return items;
            }

        }

        public List<OrderItem> GetOrderItemsByOrderID(int OrderID)
        {
            using (GrindContext _db = new GrindContext())
            {
                List<OrderItem> items = (_db.OrderItems
                    .Where(x => x.parent_order_id.Equals(OrderID))                    
                    ).ToList();


                return items;
            }


        }

        public static DateTime? GetLastOrderItemDateTime()
        {
            using (GrindContext _db = new GrindContext())
            {


                return _db.OrderItems.Max(c => (DateTime)c.created_date);
            }


        }
    }
}
