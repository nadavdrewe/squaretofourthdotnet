using System.Collections.Generic;

namespace Revel._808nd.com.CaternetData.Models
{
    public class CaternetTillSales
    {
        public string TradingDate { get; set; }
        public string TillUnitId { get; set; } = "9200";

        public string TillServiceId { get; set; } = "9200";

        public List<Entry> Sales { get; set; } = new List<Entry>();


    }
}