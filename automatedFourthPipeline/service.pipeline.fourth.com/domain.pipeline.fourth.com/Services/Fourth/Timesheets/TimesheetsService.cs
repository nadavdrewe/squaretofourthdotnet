using data.pipeline.fourth.com.Models;
using extension.railgunit.com.XML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace domain.pipeline.fourth.com.Services.Fourth.Timesheets
{
    /// <summary>
    /// Converts to Fourth T and A format
    /// </summary>
    public static class TimesheetsService
    {
        /// <summary>
        /// LS Resto To Fourth Timesheet XML
        /// </summary>
        public static XmlDocument ConvertToTimesheetXML(IEnumerable<FourthTimeSheetEntry> fourthTimesheets,
            DateTime timesheetDateTime,
            string groupGuid)
        {
            var objectToSerialsie = new FourthTimeSheetRoot
            { };
            objectToSerialsie.Records = fourthTimesheets.ToList();
            var xml = objectToSerialsie.ToXML();

            xml.DocumentElement.SetAttribute("GroupGUID", groupGuid);
            xml.DocumentElement.SetAttribute("DateTime", timesheetDateTime.ToString("yyyy-MM-ddTHH:mm:ss"));

            foreach (var item in xml.DocumentElement.ChildNodes)
            {
                var element = item as XmlElement;

                foreach (var childItem in element.ChildNodes)
                {
                    var childElement = childItem as XmlElement;
                    childElement.RemoveAllAttributes();
                }

            }
            new XmlStripper().RemoveAllNamespaces(xml.DocumentElement);

            return xml;
        }
    }
}
