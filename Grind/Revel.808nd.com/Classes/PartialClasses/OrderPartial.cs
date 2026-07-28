using System;
using System.Collections.Generic;
using System.Linq;
using Revel._808nd.com.Models;

namespace Revel._808nd.com.Classes
{

    /// <summary>
    /// EF methods
    /// </summary>
    public partial class Order
    {
        public List<Order> GetOrdersByDate(Establishment establishment, DateTime StartDate, DateTime EndDate)
        {

            using (GrindContext _db = new GrindContext())
            {
                List<Order> items = (_db.Orders
                    .Where(x => x.establishment.Equals(establishment))
                    .Where(x => x.created_date >= StartDate)
                    .Where(x => x.created_date <= EndDate)
                    ).ToList();


                return items;
            }

        }


        public static DateTime? GetLastOrderDateTime()
        {
            using (GrindContext _db = new GrindContext())
            {

                return _db.Orders.Max(c => (DateTime)c.created_date);
            }


        }

        }



    }

