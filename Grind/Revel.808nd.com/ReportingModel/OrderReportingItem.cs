using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.ReportingModel
{
    public class OrderReportingItem
    {
        public int Id { get; set; }
        public string ParentOrderId { get; set; }
        public int ProductId { get; set; }
        public decimal Amount { get; set; }
        public int Quantity { get; set; }
        public string DiscountReason { get; set; }
        public string CreatedBy { get; set; }
    }
}
