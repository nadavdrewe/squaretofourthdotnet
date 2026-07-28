using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revel._808nd.com.Classes;

namespace Revel._808nd.com.Interfaces
{
    public interface IRevelReaderAsync
    {
        Establishment Establishment { get; set; }


        Task<List<ProductCategory>> GetProductCategories();
        Task<List<Product>> GetProducts();
        Task<List<Order>> GetOrders(DateTime StartDate, DateTime EndDate);
        Task<List<OrderItem>> GetOrderItems(DateTime StartDate, DateTime EndDate);

        Task<List<Product>> GetProductsNoEstablishment();

        Task<List<ProductCategory>> GetProductCategoriesNoEstablishment();
       

    }
}
