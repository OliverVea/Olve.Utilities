namespace Olve.Utilities.StringFormatting;

/// <summary>
///     Formats a <see cref="DateTimeOffset" /> into human-readable strings.
/// </summary>
public static class DateTimeFormatter
{
    /// <summary>
    ///     Formats the specified <see cref="DateTimeOffset" /> into a string representing the time elapsed since the specified
    ///     date.
    /// </summary>
    /// <param name="now">The current date and time.</param>
    /// <param name="then">The date and time to format.</param>
    /// <example>
    ///     2021-09-01 12:00:00 -> "1 year ago"
    ///     2024-01-01 12:00:00 -> "just now"
    /// </example>
    /// <returns>A string representing the time elapsed since the specified date.</returns>
    public static string FormatTimeAgo(DateTimeOffset now, DateTimeOffset then)
    {
        if (!TryGetNumberAndUnit(now, then, out var number, out var unit))
        {
            return "just now";
        }

        return $"{number} {unit}{(number > 1 ? "s" : "")} ago";
    }

    private static bool TryGetNumberAndUnit(
        DateTimeOffset now,
        DateTimeOffset then,
        out int number,
        out string unit
    )
    {
        // Handle years and months using calendar-based logic
        if (TryGetYearsAndMonths(now, then, out number, out unit))
        {
            return true;
        }

        // Handle days, hours, minutes, and seconds using TimeSpan
        var delta = now - then;

        if (delta.TotalDays >= 1)
        {
            number = (int)delta.TotalDays;
            unit = "day";
            return true;
        }

        if (delta.TotalHours >= 1)
        {
            number = (int)delta.TotalHours;
            unit = "hour";
            return true;
        }

        if (delta.TotalMinutes >= 1)
        {
            number = (int)delta.TotalMinutes;
            unit = "minute";
            return true;
        }

        if (delta.TotalSeconds >= 1)
        {
            number = (int)delta.TotalSeconds;
            unit = "second";
            return true;
        }

        number = 0;
        unit = string.Empty;
        return false;
    }

    private static bool TryGetYearsAndMonths(
        DateTimeOffset now,
        DateTimeOffset then,
        out int number,
        out string unit
    )
    {
        var years = now.Year - then.Year;
        var months = now.Month - then.Month;

        // Adjust for incomplete years
        if (months < 0)
        {
            years--;
            months += 12;
        }

        // Handle years
        if (years >= 1)
        {
            number = years;
            unit = "year";
            return true;
        }

        // Handle months
        if (months >= 1)
        {
            number = months;
            unit = "month";
            return true;
        }

        number = 0;
        unit = string.Empty;
        return false;
    }
}
