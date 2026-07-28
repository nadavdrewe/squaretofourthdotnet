using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revel._808nd.com.Classes;


namespace Revel._808nd.com.Interfaces
{
    public interface IRevelReader
    {
        
        List<ProductCategory> GetProductCategories();
        List<Product> GetProducts(int establishmentID);
        List<Order> GetOrders(DateTime StartDate, DateTime EndDate);
        List<OrderItem> GetOrderItems(DateTime StartDate, DateTime EndDate);

        List<Product> GetProductsNoEstablishment();

        List<ProductCategory> GetProductCategoriesNoEstablishment();
       

    }
}
