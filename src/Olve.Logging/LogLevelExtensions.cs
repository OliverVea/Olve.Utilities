namespace Olve.Logging;

/// <summary>
/// Extension helpers to convert between <see cref="LogLevel"/> and <see cref="Microsoft.Extensions.Logging.LogLevel"/>.
/// </summary>
public static class LogLevelExtensions
{
    /// <summary>
    /// Convert an <see cref="LogLevel"/> value to the equivalent <see cref="Microsoft.Extensions.Logging.LogLevel"/>.
    /// </summary>
    /// <param name="logLevel">The Olve logging level to convert.</param>
    /// <returns>The corresponding Microsoft.Extensions.Logging.LogLevel value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="logLevel"/> is an unrecognized value.</exception>
    public static Microsoft.Extensions.Logging.LogLevel ToMicrosoftLogLevel(this LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.None => Microsoft.Extensions.Logging.LogLevel.None,
            LogLevel.Trace => Microsoft.Extensions.Logging.LogLevel.Trace,
            LogLevel.Debug => Microsoft.Extensions.Logging.LogLevel.Debug,
            LogLevel.Info => Microsoft.Extensions.Logging.LogLevel.Information,
            LogLevel.Warning => Microsoft.Extensions.Logging.LogLevel.Warning,
            LogLevel.Error => Microsoft.Extensions.Logging.LogLevel.Error,
            LogLevel.Critical => Microsoft.Extensions.Logging.LogLevel.Critical,
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
        };
    }

    /// <summary>
    /// Convert a <see cref="Microsoft.Extensions.Logging.LogLevel"/> to the equivalent <see cref="LogLevel"/>.
    /// </summary>
    /// <param name="logLevel">The Microsoft.Extensions.Logging.LogLevel value to convert.</param>
    /// <returns>The corresponding <see cref="LogLevel"/> value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="logLevel"/> is an unrecognized value.</exception>
    public static LogLevel ToOlveLogLevel(this Microsoft.Extensions.Logging.LogLevel logLevel)
    {
        return logLevel switch
        {
            Microsoft.Extensions.Logging.LogLevel.Trace => LogLevel.Trace,
            Microsoft.Extensions.Logging.LogLevel.Debug => LogLevel.Debug,
            Microsoft.Extensions.Logging.LogLevel.Information => LogLevel.Info,
            Microsoft.Extensions.Logging.LogLevel.Warning => LogLevel.Warning,
            Microsoft.Extensions.Logging.LogLevel.Error => LogLevel.Error,
            Microsoft.Extensions.Logging.LogLevel.Critical => LogLevel.Critical,
            Microsoft.Extensions.Logging.LogLevel.None => LogLevel.None,
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
        };
    }
}
