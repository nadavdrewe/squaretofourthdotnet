using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revel._808nd.com.Classes;

namespace Revel._808nd.com.ReportingModel
{
    public class OrderItemTypeCategoryBreakdown
    {
        public IList<OrderItem> FoodItems { get; set; }
        public IList<OrderItem> HotDrinkItems { get; set; }
        public IList<OrderItem> SoftDrinkItems { get; set; }
        public IList<OrderItem> AlcoholItems { get; set; }
        public IList<OrderItem> OtherItems { get; set; }


        public OrderItemTypeCategoryBreakdown()
        {
            FoodItems = new List<OrderItem>();
            HotDrinkItems = new List<OrderItem>();
            SoftDrinkItems = new List<OrderItem>();
            AlcoholItems = new List<OrderItem>();
            OtherItems = new List<OrderItem>();
        }
    }
}
