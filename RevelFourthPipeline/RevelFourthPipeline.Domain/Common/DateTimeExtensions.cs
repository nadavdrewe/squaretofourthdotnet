using System.Globalization;

namespace RevelFourthPipeline.Domain.Common;

public static class DateTimeExtensions
{
    public static string ToRevelDate(this DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
    }
}
