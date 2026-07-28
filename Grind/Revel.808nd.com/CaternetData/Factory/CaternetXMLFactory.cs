using Revel._808nd.com.CaternetData.Models;
using Revel._808nd.com.CaternetData.XMLMappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Revel._808nd.com.CaternetData
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