using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace data.pipeline.fourth.com.Models
{
    [XmlType("Root")]
    public class FourthTimeSheetRoot
    {
        //public string GUID { get; set; }
        //public DateTime DateTime { get; set; }

        [XmlElement("Record")]
        public List<FourthTimeSheetEntry> Records { get; set; }
    }
}
