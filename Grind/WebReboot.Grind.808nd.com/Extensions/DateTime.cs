using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Grind._808nd.com.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }

     
            public static DateTime StartOfWeekMonday(this DateTime dt, DayOfWeek startOfWeek)
            {
                System.Globalization.CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
                DayOfWeek fdow = ci.DateTimeFormat.FirstDayOfWeek;
                return DateTime.Today.AddDays(-(DateTime.Today.DayOfWeek - fdow));
            }
        }

    
}