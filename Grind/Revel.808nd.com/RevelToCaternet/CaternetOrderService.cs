using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.WebserviceReader;
using Revel._808nd.com.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.RevelToCaternet
{
    public class CaternetOrderService
    {

        public async Task<IEnumerable<Order>> PullOrdersFromRevel(Establishment establishmentToPullOrdersFor, RevelContextBase db, DateTime start, DateTime end, int limit = 0)
        {
            RevelWebserviceDataReader webReader = new RevelWebserviceDataReader(establishmentToPullOrdersFor);
            RevelDBReader dbReader = new RevelDBReader(establishmentToPullOrdersFor);
            RevelDBWriter dbWriter = new RevelDBWriter(db);

            var newOrders = await webReader.GetOrdersSinglePull(start, end);
            //newOrders.ForEach(x => x.establishment_id = establishmentToPullOrdersFor.establishment_id);

            //remove existing

            var existing = db.Orders.Where(x => x.created_date >= start && x.created_date <= end && x.establishment_id == establishmentToPullOrdersFor.establishment_id).ToList();
            if (existing.Count() > 0)
            {
                ((DbSet<Order>)db.Orders).RemoveRange(existing);
                db.SaveChanges();
            }

          ((DbSet<Order>)db.Orders).AddRange(newOrders);
            db.SaveChanges();

            return newOrders;
        }


    }
}
