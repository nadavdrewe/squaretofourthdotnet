using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Revel._808nd.com.FourthClient
{
    public static class FourthXmlExtension
    {

        public static string ConvertXMLDocToString(this XmlDocument xmlDoc)
        {
            return xmlDoc.OuterXml;
        }

    }
}
