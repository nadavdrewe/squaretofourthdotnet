using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revel._808nd.com.Interfaces;
using Revel._808nd.com.Models;

namespace Revel._808nd.com.Classes.Reporting.ReportingFactory
{
    public interface ICreateProductOrderItemReport
    {
        List<ProductOrderItemSummary> CreateProductOrderItemSummaryReport(ReportContext context, GrindContext dataSource, out List<OrderItem> orderItems, string UserURI );
    }

    public class ReportContext
    {
        public int IdOfStore { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NoOfDaysInEachReportingPeriod { get; set; }
    }
}
