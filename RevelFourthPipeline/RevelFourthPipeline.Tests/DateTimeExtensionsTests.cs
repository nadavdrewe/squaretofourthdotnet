using RevelFourthPipeline.Domain.Common;

namespace RevelFourthPipeline.Tests;

public class DateTimeExtensionsTests
{
    [Fact]
    public void ToRevelDate_UsesLegacyFormat()
    {
        var value = new DateTime(2026, 6, 7, 4, 5, 6);

        Assert.Equal("2026-06-07T04:05:06", value.ToRevelDate());
    }
}
