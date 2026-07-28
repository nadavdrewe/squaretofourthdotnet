using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using WebReboot.Grind._808nd.com.XMLConverters.Models;
using WebReboot.Grind._808nd.com.XMLConverters.XMLMappers;

namespace WebReboot.Grind._808nd.com.XMLConverters.Factory
{
    public class CaternetXMLFactory
    {

        public void CreateXML(string filepath, DateTime salesDate, string tillServiceId, string tillUnitId, IEnumerable<CaternetCsvRow> data)
        {

            var topLevelSales = new CaternetTillSales();
            var service = new RevelCsVToCaternetTillSalesMapper();
            CaternetTillSales caternetTillSalesForXMlTransform = service.Map(salesDate, tillServiceId, tillUnitId, data);

            XmlSerializer serializer = new XmlSerializer(typeof(CaternetTillSales));
            using (TextWriter writer = new StreamWriter(filepath))
            {
                serializer.Serialize(writer, caternetTillSalesForXMlTransform);
            }

            /* string xml = Xml.Net.XmlConvert.SerializeObject(caternetTillSalesForXMlTransform);
             return xml;*/
        }

    }
}