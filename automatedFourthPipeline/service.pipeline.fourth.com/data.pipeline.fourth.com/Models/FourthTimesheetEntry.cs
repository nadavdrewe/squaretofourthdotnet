using com.fourth.pipeline.pos.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace data.pipeline.fourth.com.Models
{
    [XmlType("Record")]
    public class FourthTimeSheetEntry
    {
        public string EmpNo { get; set; }
        public string Location { get; set; }
        public TimesheetClockStatus ClockStatus { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public string Notes { get; set; }
    }
}
