using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.ReportingModel
{
    public class TopSeller
    {
        public string Name { get; set; }
        public decimal ValuePounds {get;set;}
        public string NumberOfItems { get; set; }
        public OrderItemTypeCategoryBreakdown Breakdown { get; set; }
    }
}
