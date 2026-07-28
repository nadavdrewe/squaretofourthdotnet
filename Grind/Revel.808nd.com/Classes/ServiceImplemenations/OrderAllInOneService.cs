using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revel._808nd.com.Models;
using System.Data.Entity;

namespace Revel._808nd.com.Classes.ServiceImplemenations
{
    public class OrderAllInOneService : BaseService
    {
        public OrderAllInOneService(string RevelAPIKEY, string RevelBaseURL, RevelContextBase db) : base(RevelAPIKEY, RevelBaseURL, db)
        {
        }

        public static class OrderAllInOneServiceQueries
        {
            public static string getOrderAllInOneForDateRange(DateTime startDate, DateTime endDate)
            {
                var query = "/resources/OrderAllInOne?format=json&created_date__gt={0}&created_date__lte={1}&limit=0";
                var startdateString = startDate.ToString("yyyy-MM-ddTHH:mm:ss");
                var endDateString = endDate.ToString("yyyy-MM-ddTHH:mm:ss");

                string webURL = String.Format(query,
                    startdateString,
                    endDateString);


                return webURL;
            }

            public static string getOrderAllInOneForDateRangeAndEstablishment(DateTime startDate, DateTime endDate, int establishmentId)
            {
                var query = getOrderAllInOneForDateRange(startDate, endDate);
                query += String.Format("&establishment={0}", establishmentId);
                return query;
            }
        }

        public async Task<IEnumerable<OrderAllInOne>> GetOrderAllInOneFromRevel(string query)
        {
            return await this._webReader.GetRevelWebserviceData<OrderAllInOne>(new OrderAllInOne(), query, _genericObjectCreatorFactory);
        }

        public async Task<IEnumerable<OrderAllInOne>> GetOrderAllInOneForDateRangeAndEstablishmentAndReplaceLocal(DateTime startDate, DateTime endDate, int establishmentId)
        {
            var query = OrderAllInOneServiceQueries.getOrderAllInOneForDateRangeAndEstablishment(startDate, endDate, establishmentId);
            var newItems = await GetOrderAllInOneFromRevel(query);

            if (newItems.Count() > 0)
            {
                var earliestDateInOrders = newItems.Min(x => x.created_date);
                var latestDateInOrders = newItems.Max(x => x.created_date);

                var existingItems = _db.OrdersAllInOne
                    .Where(x => x.establishment == "/enterprise/Establishment/" + establishmentId + "/")
                    .Where(x => x.created_date >= earliestDateInOrders && x.created_date <= latestDateInOrders)
                    .ToList();

                ((DbSet<OrderAllInOne>)_db.OrdersAllInOne).RemoveRange(existingItems);
                ((DbSet<OrderAllInOne>)_db.OrdersAllInOne).AddRange(newItems);
                _db.SaveChanges();
            }

            return newItems;
        }


    }
}
