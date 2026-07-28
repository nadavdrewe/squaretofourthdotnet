using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shared.services.grind.railgunit.com.OpsReporting
{
    public class DansOpsReportv1
    {
        public string GrindIds { get; set; } //which grind       
        public string GrindName { get; set; } //which grind             
        public List<DansOpsReportV1DateNameValuePair> Data { get; set; } = new List<DansOpsReportV1DateNameValuePair>();
    }


    public class DansOpsReportV1DateNameValuePair
    {
        public DateTime RangeStart { get; set; } //hour and day
        public DateTime RangeEnd { get; set; } //hour and day
        public string DateOfWork { get; set; }
        public string Name { get; set; } //generate a new
        public string Value { get; set; }
        public string Type { get; set; }

    }
}
