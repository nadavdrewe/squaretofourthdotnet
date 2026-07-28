using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace extension.railgunit.com.XML
{
    public static class XMLExtensions
    {
        public class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
        }

        public static XmlDocument ToXML(this Object oObject)
        {
            XmlDocument xmlDoc = new XmlDocument();

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            XmlSerializer xmlSerializer = new XmlSerializer(oObject.GetType());

            using (StringWriter writer = new Utf8StringWriter())
            {
                xmlSerializer.Serialize(writer, oObject, ns);
                var utf8 = writer.ToString();
                var XmlDoc = new XmlDocument();
                XmlDoc.LoadXml(utf8);

                return XmlDoc;
            }
        }

     

        public static XmlDocument ChangeXmlEncoding(this XmlDocument xmlDoc, string newEncoding)
        {
            if (xmlDoc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
            {
                XmlDeclaration xmlDeclaration = (XmlDeclaration)xmlDoc.FirstChild;
                xmlDeclaration.Encoding = newEncoding;
            }
            return xmlDoc;
        }

        public static XElement RemoveAllNamespaces(XElement xmlDocument)
        {
            if (!xmlDocument.HasElements)
            {
                XElement xElement = new XElement(xmlDocument.Name.LocalName);
                xElement.Value = xmlDocument.Value;

                foreach (XAttribute attribute in xmlDocument.Attributes())
                    xElement.Add(attribute);

                return xElement;
            }
            return new XElement(xmlDocument.Name.LocalName, xmlDocument.Elements().Select(el => RemoveAllNamespaces(el)));
        }

        public static XmlDocument RemoveXmlns(this XmlDocument doc)
        {
            XDocument d;
            using (var nodeReader = new XmlNodeReader(doc))
                d = XDocument.Load(nodeReader);

            d.Root.Descendants().Attributes().Where(x => x.IsNamespaceDeclaration).Remove();

            foreach (var elem in d.Descendants())
                elem.Name = elem.Name.LocalName;

            var xmlDocument = new XmlDocument();
            using (var xmlReader = d.CreateReader())
                xmlDocument.Load(xmlReader);

            return xmlDocument;
        }


        public static string ConvertXMLDocToString(this XmlDocument xmlDoc)
        {
            return xmlDoc.OuterXml;
        }



    }
}
