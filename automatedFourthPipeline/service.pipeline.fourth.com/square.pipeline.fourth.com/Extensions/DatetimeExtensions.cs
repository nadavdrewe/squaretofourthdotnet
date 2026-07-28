using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace square.pipeline.fourth.com.Extensions
{
    public static class DatetimeExtensions
    {
        public static string ToSquareDateTime(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz", DateTimeFormatInfo.InvariantInfo);
        }

        public static string ToRFC339UTCDateTime(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd'T'HH:mm:sszzz", DateTimeFormatInfo.InvariantInfo);
        }

    }
}
