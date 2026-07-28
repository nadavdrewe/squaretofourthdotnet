using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.ReportingModel
{
    public interface IReportingItem
    {
        int Id { get; set; }
        int ParentOrderId { get; set; }
        int ProductId { get; set; }
        decimal Amount { get; set; }
        int Quantity { get; set; }
        string DiscountReason { get; set; }
        string CreatedBy { get; set; }
    }
}
