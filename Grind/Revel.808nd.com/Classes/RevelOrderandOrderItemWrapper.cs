using System;
using System.Collections.Generic;
using System.Linq;

namespace Revel._808nd.com.Classes
{

    /// <summary>
    /// A collection that takes a two dates and gets orders and ordersItems
    /// </summary>
    public class RevelOrderandOrderItemWrapper
    {
        public enum WrapperType { Order, OrderItem, Full, Order_Time, OrderItem_Time, Full_Time }

        public WrapperType Type
        { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Order> Orders { get; set; }
        public List<OrderItem> OrderItems { get; set; }


        public RevelOrderandOrderItemWrapper()
        {

        }

        public RevelOrderandOrderItemWrapper(DateTime startDate, DateTime endDate, WrapperType collectionType, Establishment est)
        {
            //create an order
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.Type = collectionType;

            this.Orders = (collectionType.Equals(WrapperType.Full) || collectionType.Equals(WrapperType.Order) || collectionType.Equals(WrapperType.Full_Time) || collectionType.Equals(WrapperType.Order_Time)
                ? new List<Order>() : null);

            this.OrderItems = (collectionType.Equals(WrapperType.Full) || collectionType.Equals(WrapperType.OrderItem) || collectionType.Equals(WrapperType.Full_Time) || collectionType.Equals(WrapperType.OrderItem_Time)
                ? new List<OrderItem>() : null);

        }


        public RevelOrderandOrderItemWrapper(DateTime startDate, DateTime endDate, WrapperType collectionType)
        {
            //create an order
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.Type = collectionType;

            this.Orders = (collectionType.Equals(WrapperType.Full) || collectionType.Equals(WrapperType.Order) || collectionType.Equals(WrapperType.Full_Time) || collectionType.Equals(WrapperType.Order_Time) 
                ? new List<Order>() : null);

            this.OrderItems = (collectionType.Equals(WrapperType.Full) || collectionType.Equals(WrapperType.OrderItem) || collectionType.Equals(WrapperType.Full_Time) || collectionType.Equals(WrapperType.OrderItem_Time)
                ? new List<OrderItem>() : null);

        }
        public decimal GetOrderTotalPoundsGross()
        {
            
            //CURRENTLY USES ONLY CLOSED/PAID ORDERS IN LINE WITH REVELUP

            var discount = GetOrderTotalPoundsTotalDiscount();
            

            return Decimal.Round(this.Orders
                .Where(x => x.closed = true)
                    .Where(u => u.is_unpaid == "False")
                .Sum(x => x.final_total), 2);

        }
        public decimal GetOrderTotalPoundsTotalDiscount()
        {
            //CURRENTLY USES ONLY CLOSED/PAID ORDERS IN LINE WITH REVELUP
            return Decimal.Round(this.Orders
                .Where(x => x.closed = true)
                    .Where(u => u.is_unpaid == "False").Sum(x => x.discount_amount), 2);
        }
        public decimal GetOrderTotalPoundsTax()
        {
            //CURRENTLY USES ONLY CLOSED/PAID ORDERS IN LINE WITH REVELUP
            return Decimal.Round(this.Orders
                .Where(x => x.closed = true)
                    .Where(u => u.is_unpaid == "False").Sum(x => x.tax), 2);
        }

        public decimal GetOrderTotalPoundsNet()
        {
            //CURRENTLY USES ONLY CLOSED/PAID ORDERS IN LINE WITH REVELUP
            var result = decimal.Round(this.Orders
                .Where(x => x.closed = true)
                    .Where(u => u.is_unpaid == "False")
                .Sum(x => x.final_total) 
                
                - 
                
                this.Orders
                .Where(x => x.closed = true)
                    .Where(u => u.is_unpaid == "False")
                    .Sum(x => x.tax), 2);
            return result;
        }

        public decimal GetOrderItemPoundsGross()
        {
            return Decimal.Round(this.OrderItems.Sum(x => x.price), 2);

        }

        public decimal GetOrderItemPoundsTax()
        {
            return  Decimal.Round(this.OrderItems.Sum(x => x.tax_amount));

        }

        public decimal GetOrderItemPoundsNet()
        {
            var result =  Decimal.Round((this.OrderItems.Sum(x=>x.price) - this.OrderItems.Sum(x => x.tax_amount)));
            return result;

        }

        public decimal GetAvgSpendNet()
        {
            if (this.Orders.Count > 0)
            {
                decimal avgspend = GetOrderTotalPoundsNet()/this.Orders.Where(x => x.closed = true)
                    .Where(u => u.is_unpaid == "False").Count();
                return Decimal.Round(avgspend, 2);
            }
            else
            {
                return 0.00M;
            }
        }

        public decimal GetAvgSpendGross()
        {
            if (this.Orders.Count > 0)
            {
                decimal avgspend = GetOrderTotalPoundsNet() / this.Orders.Where(x => x.closed = true)
                    .Where(u => u.is_unpaid == "False").Count();
                return Decimal.Round(avgspend, 2);
            }
            else
            {
                return 0.00M;
            }
        }


       



    }
}
