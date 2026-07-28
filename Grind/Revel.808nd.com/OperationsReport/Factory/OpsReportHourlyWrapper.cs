using Revel._808nd.com.OperationsReport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.OperationsReport.Factory
{

    /// <summary>
    /// Generates 
    /// </summary>
    public static class OpsReportHourlyWrapperFactory
    {


        public static IList<OpsReportHourlyWrapper> Create(DateTime baseDate, int hoursBackFromBaseDate, IEnumerable<int> revelEstablishmentIds)
        {
            IList<OpsReportHourlyWrapper> peristenceDataWrappers = new List<OpsReportHourlyWrapper>();

            for (int i = hoursBackFromBaseDate; i > 0; i--)
            {
                var someHoursAgo = baseDate.AddHours(-i); //go back four hours
                var currentFixedHourStart = new DateTime(someHoursAgo.Year, someHoursAgo.Month, someHoursAgo.Day, someHoursAgo.Hour, 00, 00); // fix to the start of the previous hour
                var currentFixedHoursEnd = currentFixedHourStart.AddHours(1);

                //loop through each establishment and crete a container using these start and end dates
                foreach (var est in revelEstablishmentIds)
                {
                    var newContainer = new OpsReportHourlyWrapper { containerStart = currentFixedHourStart, containerEnd = currentFixedHoursEnd, establishmentId = est };
                    peristenceDataWrappers.Add(newContainer);
                }
            }


            return peristenceDataWrappers;

        }
    }
}
