using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Revel._808nd.com.Interfaces;

namespace Revel._808nd.com.Classes
{
    public class RevelFactoryAsyncLocalDb : IRevelFactoryAsync
    {
        private IRevelReaderAsync dbReader { get; set; } //this is wrapped by the methods in this factory
        private Establishment establishment { get; set; }

        public RevelFactoryAsyncLocalDb(IRevelReaderAsync revelReaderAsync, Establishment Establishment)
        {
            this.dbReader = revelReaderAsync;
            this.establishment = Establishment;

        }


        public async Task<List<Product>> CreateProducts(List<Product> products)
        {
            products = await dbReader.GetProducts();
            return products;
        }

        public async Task<List<ProductCategory>> CreateProductCategories(List<ProductCategory> productCategories)
        {
            productCategories = await dbReader.GetProductCategories();

            return productCategories;
        }

        public async Task<List<OrderItem>> CreateOrderItems_Time(DateTime startQueryDateTime, DateTime EndQueryDateTime,
            List<OrderItem> theOrderItems)
        {

            theOrderItems = await dbReader.GetOrderItems(startQueryDateTime, EndQueryDateTime);

            return theOrderItems;

        }

        public async Task<List<OrderItem>> CreateOrderItems(DateTime startQueryDateTime, DateTime EndQueryDateTime,
            List<OrderItem> theOrderItems)
        {

            //chop the dates up so start is 2am
            DateTime queryDateProper = new DateTime(startQueryDateTime.Year, startQueryDateTime.Month, startQueryDateTime.Day, 02,00,00);

            theOrderItems = await dbReader.GetOrderItems(queryDateProper, EndQueryDateTime);


            //filter only establishment we need

            return theOrderItems;
        }

        public async Task<List<Order>> CreateOrders(DateTime startQueryDateTime, DateTime EndQueryDateTime,
            List<Order> theOrders)
        {

      
            //chop the dates up so start is 2am
            DateTime queryDateProper = new DateTime(startQueryDateTime.Year, startQueryDateTime.Month, startQueryDateTime.Day, 02, 00, 00);

            theOrders = await dbReader.GetOrdersSinglePull(queryDateProper, EndQueryDateTime);

            //filter only establishment we need

            return theOrders;

        }

        public async Task<List<Order>> CreateOrders_Time(DateTime startQueryDateTime, DateTime EndQueryDateTime,
            List<Order> theOrders)
        {

            theOrders = await dbReader.GetOrdersSinglePull(startQueryDateTime, EndQueryDateTime);
            return theOrders;

        }

        public async Task<RevelProductAndCategoryWrapper> CreateProductsAndCategories(RevelProductAndCategoryWrapper wrapper)
        {
     
            wrapper.Products = await this.CreateProducts(wrapper.Products);
            wrapper.ProductCategories = await this.CreateProductCategories(wrapper.ProductCategories);

            wrapper.CreateProductCategoriesDictionary();

            return wrapper;
        }

        public async Task<RevelOrderandOrderItemWrapper> PopulateOrderAndItemWrapper(
            RevelOrderandOrderItemWrapper wrapper)
        {
            //check wrapper type and know what collection to initialise
            if (wrapper.Type.Equals(RevelOrderandOrderItemWrapper.WrapperType.Full) || wrapper.Type.Equals(RevelOrderandOrderItemWrapper.WrapperType.Order))
            {
                wrapper.Orders = await this.CreateOrders(wrapper.StartDate, wrapper.EndDate, wrapper.Orders);
            }
            if (wrapper.Type.Equals(RevelOrderandOrderItemWrapper.WrapperType.Full) ||
                wrapper.Type.Equals(RevelOrderandOrderItemWrapper.WrapperType.OrderItem))
            {
                wrapper.OrderItems = await this.CreateOrderItems(wrapper.StartDate, wrapper.EndDate, wrapper.OrderItems);
            }


            //time based params
            if (wrapper.Type.Equals(RevelOrderandOrderItemWrapper.WrapperType.Full_Time) || wrapper.Type.Equals(RevelOrderandOrderItemWrapper.WrapperType.Order_Time))
            {
                wrapper.Orders = await this.CreateOrders_Time(wrapper.StartDate, wrapper.EndDate, wrapper.Orders);
            }

            if (wrapper.Type.Equals(RevelOrderandOrderItemWrapper.WrapperType.Full_Time) ||
                wrapper.Type.Equals(RevelOrderandOrderItemWrapper.WrapperType.OrderItem_Time))
            {
                wrapper.OrderItems = await this.CreateOrderItems_Time(wrapper.StartDate, wrapper.EndDate, wrapper.OrderItems);
            }


            //MAP ONLY THE CORRECT ESTABLISHMENT FROM DB!!
            if (wrapper.Orders.Any())
            {
                wrapper.Orders =
                    wrapper.Orders.Where(x => x.establishment_id == this.establishment.establishment_id).ToList();
            }


            //map order items
            
            if (wrapper.OrderItems != null)
            {
                List<OrderItem> correspondingOrderITems = new List<OrderItem>();
                var listofIDs = wrapper.Orders.Select(c => c.order_id).ToList();

                foreach (var id in listofIDs)
                {
                    foreach (var item in wrapper.OrderItems)
                    {
                        if (id == item.parent_order_id)
                        {
                            correspondingOrderITems.Add(item);
                        }
                    }

                }

                wrapper.OrderItems = correspondingOrderITems;
            }

            return wrapper;
        }

    }
}
