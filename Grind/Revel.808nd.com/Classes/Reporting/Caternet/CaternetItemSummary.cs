using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Classes.Reporting.Caternet
{
    public class CaternetItemSummary
    {
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; }
        public decimal FinalSalesNet { get; set; }
        public decimal VAT { get; set; }
        public int QtySold { get; set; }
        public int VoidOrComp { get; set; }
        public decimal Discount { get; set; }
        public decimal DiscountVAT { get; set; }
        public int ProductId { get; set; }


    }
}
