using RevelFourthPipeline.Domain.Configuration;
using RevelFourthPipeline.Domain.Pipeline;

namespace RevelFourthPipeline.Tests;

public class BusinessDayRangeResolverTests
{
    [Fact]
    public void Resolve_WithoutOverrides_UsesPreviousFourAmBusinessDay()
    {
        var ranges = BusinessDayRangeResolver.Resolve(
            new RevelFourthPipelineOptions
            {
                BusinessDayStartHour = 4
            },
            new DateTime(2026, 6, 8, 10, 30, 0));

        var range = Assert.Single(ranges);
        Assert.Equal(new DateTime(2026, 6, 7, 4, 0, 0), range.RangeStart);
        Assert.Equal(new DateTime(2026, 6, 8, 4, 0, 0), range.RangeEnd);
    }

    [Fact]
    public void Resolve_SingleDayOverride_ReturnsOneBusinessDayRange()
    {
        var ranges = BusinessDayRangeResolver.Resolve(
            new RevelFourthPipelineOptions
            {
                OverrideRangeStart = new DateTime(2026, 5, 1, 4, 0, 0),
                OverrideRangeEnd = new DateTime(2026, 5, 2, 4, 0, 0)
            },
            new DateTime(2026, 6, 8, 10, 30, 0));

        var range = Assert.Single(ranges);
        Assert.Equal(new DateTime(2026, 5, 1, 4, 0, 0), range.RangeStart);
        Assert.Equal(new DateTime(2026, 5, 2, 4, 0, 0), range.RangeEnd);
    }

    [Fact]
    public void Resolve_MonthOverride_SplitsIntoDailyBusinessRanges()
    {
        var ranges = BusinessDayRangeResolver.Resolve(
            new RevelFourthPipelineOptions
            {
                OverrideRangeStart = new DateTime(2026, 5, 1, 4, 0, 0),
                OverrideRangeEnd = new DateTime(2026, 6, 1, 4, 0, 0)
            },
            new DateTime(2026, 6, 8, 10, 30, 0));

        Assert.Equal(31, ranges.Count);
        Assert.Equal(new DateTime(2026, 5, 1, 4, 0, 0), ranges[0].RangeStart);
        Assert.Equal(new DateTime(2026, 5, 2, 4, 0, 0), ranges[0].RangeEnd);
        Assert.Equal(new DateTime(2026, 5, 31, 4, 0, 0), ranges[^1].RangeStart);
        Assert.Equal(new DateTime(2026, 6, 1, 4, 0, 0), ranges[^1].RangeEnd);
    }

    [Fact]
    public void Resolve_WhenSplitDisabled_ReturnsOneOverrideRange()
    {
        var ranges = BusinessDayRangeResolver.Resolve(
            new RevelFourthPipelineOptions
            {
                SplitOverrideRangeIntoDailyRuns = false,
                OverrideRangeStart = new DateTime(2026, 5, 1, 4, 0, 0),
                OverrideRangeEnd = new DateTime(2026, 6, 1, 4, 0, 0)
            },
            new DateTime(2026, 6, 8, 10, 30, 0));

        var range = Assert.Single(ranges);
        Assert.Equal(new DateTime(2026, 5, 1, 4, 0, 0), range.RangeStart);
        Assert.Equal(new DateTime(2026, 6, 1, 4, 0, 0), range.RangeEnd);
    }
}
