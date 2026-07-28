using RevelFourthPipeline.Domain.Configuration;

namespace RevelFourthPipeline.Domain.Pipeline;

public readonly record struct BusinessDayRange(DateTime RangeStart, DateTime RangeEnd);

public static class BusinessDayRangeResolver
{
    public static IReadOnlyList<BusinessDayRange> Resolve(
        RevelFourthPipelineOptions options,
        DateTime now)
    {
        if (options.OverrideRangeStart.HasValue || options.OverrideRangeEnd.HasValue)
        {
            if (!options.OverrideRangeStart.HasValue || !options.OverrideRangeEnd.HasValue)
            {
                throw new InvalidOperationException("Both OverrideRangeStart and OverrideRangeEnd must be supplied.");
            }

            if (options.OverrideRangeStart.Value >= options.OverrideRangeEnd.Value)
            {
                throw new InvalidOperationException("OverrideRangeStart must be earlier than OverrideRangeEnd.");
            }

            if (!options.SplitOverrideRangeIntoDailyRuns)
            {
                return
                [
                    new BusinessDayRange(options.OverrideRangeStart.Value, options.OverrideRangeEnd.Value)
                ];
            }

            return SplitIntoDailyRanges(options.OverrideRangeStart.Value, options.OverrideRangeEnd.Value);
        }

        var todayStart = new DateTime(now.Year, now.Month, now.Day, options.BusinessDayStartHour, 0, 0);
        var rangeEnd = now >= todayStart ? todayStart : todayStart.AddDays(-1);
        return [new BusinessDayRange(rangeEnd.AddDays(-1), rangeEnd)];
    }

    private static IReadOnlyList<BusinessDayRange> SplitIntoDailyRanges(
        DateTime rangeStart,
        DateTime rangeEnd)
    {
        var ranges = new List<BusinessDayRange>();
        var cursor = rangeStart;

        while (cursor < rangeEnd)
        {
            var next = cursor.AddDays(1);
            if (next > rangeEnd)
            {
                next = rangeEnd;
            }

            ranges.Add(new BusinessDayRange(cursor, next));
            cursor = next;
        }

        return ranges;
    }
}
