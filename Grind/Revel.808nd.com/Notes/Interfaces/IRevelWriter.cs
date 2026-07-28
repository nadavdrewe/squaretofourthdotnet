using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revel._808nd.com.Classes;


namespace Revel._808nd.com.Interfaces
{
    public interface IRevelWriter
    {


        
        bool SaveProductCategories(List<ProductCategory> pc);
        bool SaveProducts(List<Product> p );
        bool SaveOrders(List<Order> o);
        bool SaveOrderItems(List<OrderItem> oi);

    }
}
