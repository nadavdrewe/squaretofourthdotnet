using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Classes.Reporting
{

    public class OrderItemSummary
    {
        
        public OrderItemSummary()
        {
            StartDate = new DateTime(1901, 01, 01, 00, 00, 00);
            EndDate = new DateTime(1901, 01, 01, 00, 00, 00);
            TotalDiscount = 0.00M;
            TotalTax = 0.00M;
            DifferentProducts = 0;
        }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPureSales { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalDiscountTax { get; set; }
        public decimal TotalTax { get; set; }
        public int DifferentProducts { get; set; }
        public string StoreIdentifier { get; set; }
        public string StoreType { get; set; }

    }
}
