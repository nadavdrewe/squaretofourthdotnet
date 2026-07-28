using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.WebserviceReader;
using Revel._808nd.com.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace web.fourth.revel.com.Controllers
{
    public class OrderController : Controller
    {
        public async Task<IEnumerable<Order>> PullOrdersFromRevelForBrand(Brand brandToPullOrdersFor, RevelContextBase db, DateTime start, DateTime end, int limit = 0)
        {

            Establishment revOrg = new Establishment(1, "Grind",
             brandToPullOrdersFor.key_secret,
             new Uri(brandToPullOrdersFor.revel_base_url));


            RevelWebserviceDataReader webReader = new RevelWebserviceDataReader(revOrg);
            RevelDBReader dbReader = new RevelDBReader(revOrg);
            RevelDBWriter dbWriter = new RevelDBWriter(db);

            var newOrders = await webReader.GetOrdersStandard(start, end);

            newOrders.ForEach(x => x.db_brand_id = brandToPullOrdersFor.brand_id);

            //remove existing

            var existing = db.Orders.Where(x => x.created_date >= start && x.created_date <= end && x.db_brand_id == brandToPullOrdersFor.brand_id).ToList();
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