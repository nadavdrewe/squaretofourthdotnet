using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Classes.Reporting
{

    public class CategoryProductOrderItemSummary
    {
        public int ProductCategoryId { get; set; }
        public string CategoryName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalQuantityOrderItems { get; set; }
        public int TotalQuantityVoided { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalPureSales { get; set; }
        public List<ProductOrderItemSummary> productOrderSummaries { get; set; }



        public void SetAttributes()
        {
            StartDate = productOrderSummaries.Min(x => x.StartDate);
            EndDate = productOrderSummaries.Min(x => x.EndDate);
            this.TotalDiscount = productOrderSummaries.Sum(x => x.TotalDiscount);
            this.TotalTax = productOrderSummaries.Sum(x => x.TotalTax);
            this.TotalPureSales = productOrderSummaries.Sum(x => x.TotalPureSales);
            TotalQuantityVoided = productOrderSummaries.Sum(x => x.TotalVoided);
            TotalQuantityOrderItems = productOrderSummaries.Sum(x => x.TotalQuantity);
        }

    }
}
