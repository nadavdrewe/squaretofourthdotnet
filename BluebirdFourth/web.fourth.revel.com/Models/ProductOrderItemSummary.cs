using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace web.fourth.revel.com.Models
{

    public class ProductOrderItemSummary : OrderItemSummary
    {
        public ProductOrderItemSummary() : base()
        {
            ProductIdentifier = "Null Object";
            ProductName = "Null Object";
            TotalQuantity = 0;
        }



        public string ProductIdentifier { get; set; }
        public string ProductName { get; set; }
        public string SKU { get; set; }
        public int TotalQuantity { get; set; }
        public int TotalVoided { get; set; }

    }
}