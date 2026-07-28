using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Revel._808nd.com.Classes;

namespace Revel._808nd.com.Interfaces
{
    public interface IRevelFactoryAsync
    {

        Task<List<Product>> CreateProducts(List<Product> products);

        Task<List<ProductCategory>> CreateProductCategories(List<ProductCategory> productCategories);

        Task<List<OrderItem>> CreateOrderItems_Time(DateTime startQueryDateTime, DateTime EndQueryDateTime,
            List<OrderItem> theOrderItems);

        Task<List<OrderItem>> CreateOrderItems(DateTime startQueryDateTime, DateTime EndQueryDateTime,
            List<OrderItem> theOrderItems);


        Task<List<Order>> CreateOrders(DateTime startQueryDateTime, DateTime EndQueryDateTime, List<Order> theOrders);


        Task<List<Order>> CreateOrders_Time(DateTime startQueryDateTime, DateTime EndQueryDateTime,
            List<Order> theOrders);

        Task<RevelProductAndCategoryWrapper> CreateProductsAndCategories(RevelProductAndCategoryWrapper wrapper);

        Task<RevelOrderandOrderItemWrapper> PopulateOrderAndItemWrapper(RevelOrderandOrderItemWrapper wrapper);






    }

}
