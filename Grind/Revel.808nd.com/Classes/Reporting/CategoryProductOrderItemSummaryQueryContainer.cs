using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Classes.Reporting
{

    public class CategoryProductOrderItemSummaryQueryContainer
    {

        public string PeriodType { get; set; }
        public int Period { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalQuantityOrderItems { get; set; }
        public int TotalQuantityVoided { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalPureSales { get; set; }
        public List<CategoryProductOrderItemSummary> CategoryProductOrderItemSummaries { get; set; }

        public void SetAttributes()
        {
            StartDate = CategoryProductOrderItemSummaries.Min(x => x.StartDate);
            EndDate = CategoryProductOrderItemSummaries.Min(x => x.EndDate);
            this.TotalDiscount = CategoryProductOrderItemSummaries.Sum(x => x.TotalDiscount);
            this.TotalTax = CategoryProductOrderItemSummaries.Sum(x => x.TotalTax);
            this.TotalPureSales = CategoryProductOrderItemSummaries.Sum(x => x.TotalPureSales);
            TotalQuantityVoided = CategoryProductOrderItemSummaries.Sum(x => x.TotalQuantityVoided);
            TotalQuantityOrderItems = CategoryProductOrderItemSummaries.Sum(x => x.TotalQuantityOrderItems);
        }

        public void SetNullAttributes()
        {
            
            this.TotalDiscount = 0;
            this.TotalTax = 0;
            this.TotalPureSales = 0;
            TotalQuantityVoided = 0;
            TotalQuantityOrderItems = 0;
            
        }

    }
}
