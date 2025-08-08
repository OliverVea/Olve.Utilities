using Olve.Utilities.StringFormatting;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace Olve.Utilities.Tests;

public class DateTimeFormatterTests
{

    [Test]
    [MethodDataSource(typeof(DataSources), nameof(DataSources.Data))]
    public async Task FormatTimeAgo_WhenDateTimeIsNow_ReturnsJustNow(DateTimeOffset now,
        DateTimeOffset then,
        string expected)
    {
        // Arrange

        // Act
        var result = DateTimeFormatter.FormatTimeAgo(now, then);

        // Assert
        await Assert
            .That(result)
            .IsEqualTo(expected);
    }

    public static class DataSources
    {
        public static IEnumerable<(DateTimeOffset Now, DateTimeOffset Then, string Expected)> Data()
        {
            // Years
            yield return (
                new DateTimeOffset(2025, 01, 06, 16, 02, 00, new TimeSpan(0)),
                new DateTimeOffset(2024, 01, 06, 16, 02, 00, new TimeSpan(0)), "1 year ago");

            yield return (
                new DateTimeOffset(2025, 01, 06, 16, 02, 00, new TimeSpan(0)),
                new DateTimeOffset(1991, 07, 19, 13, 37, 12, new TimeSpan(0)), "33 years ago");

            // Months
            yield return (
                new DateTimeOffset(2025, 03, 01, 00, 00, 00, new TimeSpan(0)),
                new DateTimeOffset(2025, 01, 01, 00, 00, 00, new TimeSpan(0)), "2 months ago");

            yield return (
                new DateTimeOffset(2025, 02, 28, 00, 00, 00, new TimeSpan(0)),
                new DateTimeOffset(2025, 01, 31, 00, 00, 00, new TimeSpan(0)), "1 month ago");

            // Days
            yield return (
                new DateTimeOffset(2025, 01, 06, 16, 02, 00, new TimeSpan(0)),
                new DateTimeOffset(2025, 01, 05, 16, 02, 00, new TimeSpan(0)), "1 day ago");

            yield return (
                new DateTimeOffset(2025, 01, 06, 16, 02, 00, new TimeSpan(0)),
                new DateTimeOffset(2025, 01, 01, 16, 02, 00, new TimeSpan(0)), "5 days ago");

            // Hours
            yield return (
                new DateTimeOffset(2025, 01, 06, 16, 02, 00, new TimeSpan(0)),
                new DateTimeOffset(2025, 01, 06, 15, 02, 00, new TimeSpan(0)), "1 hour ago");

            yield return (
                new DateTimeOffset(2025, 01, 06, 16, 02, 00, new TimeSpan(0)),
                new DateTimeOffset(2025, 01, 06, 12, 02, 00, new TimeSpan(0)), "4 hours ago");

            // Minutes
            yield return (
                new DateTimeOffset(2025, 01, 06, 16, 02, 00, new TimeSpan(0)),
                new DateTimeOffset(2025, 01, 06, 16, 01, 00, new TimeSpan(0)), "1 minute ago");

            yield return (
                new DateTimeOffset(2025, 01, 06, 16, 02, 00, new TimeSpan(0)),
                new DateTimeOffset(2025, 01, 06, 15, 55, 00, new TimeSpan(0)), "7 minutes ago");

            // Seconds
            yield return (
                new DateTimeOffset(2025, 01, 06, 16, 02, 00, new TimeSpan(0)),
                new DateTimeOffset(2025, 01, 06, 16, 01, 59, new TimeSpan(0)), "1 second ago");

            yield return (
                new DateTimeOffset(2025, 01, 06, 16, 02, 00, new TimeSpan(0)),
                new DateTimeOffset(2025, 01, 06, 16, 01, 50, new TimeSpan(0)), "10 seconds ago");

            // "Just now" cases
            yield return (
                new DateTimeOffset(2025, 01, 06, 16, 02, 00, new TimeSpan(0)),
                new DateTimeOffset(2025, 01, 06, 16, 02, 00, new TimeSpan(0)), "just now");

            // Future dates
            yield return (
                new DateTimeOffset(2025, 01, 06, 16, 02, 00, new TimeSpan(0)),
                new DateTimeOffset(2025, 01, 06, 16, 05, 00, new TimeSpan(0)), "just now"); // Future dates

            // Extreme edge cases
            yield return (DateTimeOffset.MaxValue, DateTimeOffset.MinValue,
                $"{DateTimeOffset.MaxValue.Year - DateTimeOffset.MinValue.Year} years ago"); // Max and Min
            yield return (DateTimeOffset.MaxValue, DateTimeOffset.MaxValue, "just now"); // Max value compared to itself
            yield return (DateTimeOffset.MinValue, DateTimeOffset.MinValue, "just now"); // Min value compared to itself

            // Large gaps in time
            yield return (
                new DateTimeOffset(3000, 01, 01, 00, 00, 00, TimeSpan.Zero),
                new DateTimeOffset(2000, 01, 01, 00, 00, 00, TimeSpan.Zero), "1000 years ago");

            yield return (
                new DateTimeOffset(2000, 01, 01, 00, 00, 00, TimeSpan.Zero),
                new DateTimeOffset(1000, 01, 01, 00, 00, 00, TimeSpan.Zero), "1000 years ago");
        }
    }
}