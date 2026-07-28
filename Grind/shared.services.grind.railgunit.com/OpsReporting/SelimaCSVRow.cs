using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shared.services.grind.railgunit.com.OpsReporting
{
    public class SelimaCSVRow
    {
        public string SiteIdentifier { get; set; } //mapping
        public string Date { get; set; } //just date
        public string RevenueKey { get; set; }
        public string Hour { get; set; } //2 digit 24 hour
        public string Value { get; set; } 

    }
}
