using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Revel._808nd.com.Classes.FourthModelMapping
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
