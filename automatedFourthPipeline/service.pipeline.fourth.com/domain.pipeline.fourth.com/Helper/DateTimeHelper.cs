using data.pipeline.fourth.com.Models;
using domain.pipeline.fourth.com.Exceptions;
using domain.pipeline.fourth.com.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace domain.pipeline.fourth.com.Helper
{
    public static class DateTimeHelper
    {

        /// <summary>
        /// Takes a local time and converts it to the UTC time needed for Square + other POS querying
        /// </summary>
        /// <param name="timeToConvert"></param>
        /// <param name="timeZone"></param>
        /// <returns></returns>
        public static DateTime AmalgamateBaseDateAndFireTime(DateTime timeToConvert,
            TimeZoneInfo timeZoneInfo)
        {
            try
            {
                var timeZoneResultConversion = TimeZoneInfo.ConvertTimeToUtc(timeToConvert, timeZoneInfo);
                return timeZoneResultConversion;
            }
            catch (Exception ex)
            {
                throw new Exception("DateTimeHelper couldn't determine which Timezone we're in", ex);
            }
        }


        /// <summary>
        /// This will give you the last full 4am - 4am day - 
        /// </summary>
        /// <param name="offsetHours"></param>
        /// <returns></returns>
        public static DateTimeRange GenerateStandardDayYesterdayToTodayStartTimeGivenUTCNow(int offsetHours)
        {
            var utcNow = DateTime.UtcNow.AddDays(-1);
            var startDatePristine = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 04, 00, 00);
            //now apply offset
            var startDate = startDatePristine.AddHours(offsetHours);
            var transactionDate = startDate;
            var endDate = startDate.AddDays(1);

            return new DateTimeRange
            {
                StartDate = startDate,
                EndDate = endDate
            };

        }


        /// <summary>
        /// Returns the 'hour' used for the standard day on day pattern - e.g. 07am - 07am, 04am to 04am etc - based on what's in the DB
        /// </summary>
        public static int ResolveBrandUTCStartTime(BrandIntegration brandIntegrationToResolve)
        {
            try
            {
                var startTime = brandIntegrationToResolve.StartBatchQueryTimeUTC;
                if (startTime == null)
                {
                    throw new BrandStartTimeException("We couldn't resolve brand start time - the FireTimeUTC value in the DB is null!!");
                }

                //var startTimePatterMatched = startTime <= 10 ? "0" + startTime : startTime.ToString();
                return Convert.ToInt16(startTime);
            }
            catch (Exception ex)
            {
                throw new BrandStartTimeException("We couldn't resolve brand start time", ex);
            }
        }
      
    }
}
