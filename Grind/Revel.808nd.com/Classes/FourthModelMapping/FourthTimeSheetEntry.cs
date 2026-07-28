using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Revel._808nd.com.Classes.FourthModelMapping
{

    [XmlType("Record")]
    public class FourthTimeSheetEntry
    {
        public string EmpNo { get; set; }
        public string Location { get; set; }
        public int ClockStatus { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public string Notes { get; set; }
    }
}


