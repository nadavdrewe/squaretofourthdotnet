using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Revel._808nd.com.Models;

namespace Revel._808nd.com.Classes.ServiceImplementaitons
{
    public interface IOrderItemsService
    {
        Task<IEnumerable<OrderItem>> GetOrderItems(Brand brand, RevelContextBase db, DateTime startdate, DateTime endDate);

        Task<IEnumerable<OrderItem>> GetOrderItemsForBrand(Brand brand, RevelContextBase db, DateTime startdate, DateTime endDate, int limit);

    }
}
