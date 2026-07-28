using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebReboot.Grind._808nd.com.XMLConverters.Models
{
    public class CaternetTillSales
    {
        public string TradingDate { get; set; }
        public string TillUnitId { get; set; } = "9200";

        public string TillServiceId { get; set; } = "9200";

        public List<Entry> Sales { get; set; } = new List<Entry>();


    }
}