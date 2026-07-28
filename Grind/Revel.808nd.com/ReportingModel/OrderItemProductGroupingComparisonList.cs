using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.Reporting;

namespace Revel._808nd.com.ReportingModel
{
    public class OrderItemProductGroupingComparisonList
    {
       
        public List<ProductOrderItemSummary> Items { get; set; }
        public List<ProductOrderItemSummary> ComparisonItems { get; set; }

        public OrderItemProductGroupingComparisonList()
        {
            Items = new List<ProductOrderItemSummary>();
            ComparisonItems = new List<ProductOrderItemSummary>();
        }
    }

    public class OrderItemProductGroupingComparisonDictionary
    {
        public string ItemsIdentifier { get; set; }

        public string ComparisonItemsIdentifier { get; set; }
        public Dictionary<string, decimal> Items { get; set; }
        public Dictionary<string, decimal> ComparisonItems { get; set; }

        public OrderItemProductGroupingComparisonDictionary()
        {
            Items = new Dictionary<string, decimal>();
            ComparisonItems = new Dictionary<string, decimal>();
        }
    }
}
