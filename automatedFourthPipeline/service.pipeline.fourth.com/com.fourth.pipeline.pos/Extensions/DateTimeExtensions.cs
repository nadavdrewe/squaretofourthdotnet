using System;
using System.Collections.Generic;
using System.Text;

namespace com.fourth.pipeline.pos.Extensions
{
    public static class DateTimeOffsetExtensions
    {
        public static string ToFourthSalesCSVDateUTC(this DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.UtcDateTime.ToString("yyyy-MM-dd");
        }

        public static string ToFourthSalesCSVTimeUTC(this DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.UtcDateTime.ToString("HH:mm:ss");
        }

        public static string ToFourthSalesCSVTimeUTC(this DateTime dateTimeUTC)
        {
            return dateTimeUTC.ToString("HH:mm:ss");
        }
    }
}
