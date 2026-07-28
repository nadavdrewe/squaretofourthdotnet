using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Classes.Reporting
{
    public class ProductOrderItemSummary : OrderItemSummary
    {
     

        public ProductOrderItemSummary() : base()
        {
            ProductIdentifier = "Null Object";
            TotalQuantity = 0;
        }

        public int PeriodNumber { get; set; }
        public string Period { get; set; }
        public int ProductCategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ProductId { get; set; }
        public string ProductIdentifier { get; set; }
        public string SKU { get; set; }
        public int TotalQuantity { get; set; }
        public int TotalComps { get; set; }
        public int TotalVoided { get; set; }


    }
}
