using System;
using System.Collections.Generic;
using Revel._808nd.com.Classes;

namespace Revel._808nd.com.Interfaces
{
    /// <summary>
    /// Revel factory populates collections of types
    /// Product
    /// Order
    /// OrderItem etc
    /// 
    /// For async methods use IRevelFactoryAsync
    /// </summary>


    interface IRevelFactory
    {


          RevelProductAndCategoryWrapper CreateProductsAndCategories(RevelProductAndCategoryWrapper wrapper);
          List<Product> CreateProducts(List<Product> products);


          List<Order> CreateOrders(DateTime startQueryDateTime, DateTime EndQueryDateTime, List<Order> theOrders);
          List<Order> CreateOrders_Time(DateTime startQueryDateTime, DateTime EndQueryDateTime, List<OrderItem> theOrderItems);


          List<OrderItem> CreateOrderItem(DateTime startQueryDateTime, DateTime EndQueryDateTime, List<OrderItem> theOrderItems);
          List<OrderItem> CreateOrderItem_Time(DateTime startQueryDateTime, DateTime EndQueryDateTime, List<OrderItem> theOrderItems);

          RevelOrderandOrderItemWrapper PopulateOrderAndItemWrapper(RevelOrderandOrderItemWrapper wrapper);

    }

}
