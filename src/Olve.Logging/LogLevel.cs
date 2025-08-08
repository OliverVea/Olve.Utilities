using System.Text.Json.Serialization;

namespace Olve.Logging;

/// <summary>
/// Represents the severity level of a log message.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LogLevel
{
    /// <summary>
    /// No logging.
    /// </summary>
    None = 0,

    /// <summary>
    /// Trace-level messages, very detailed.
    /// </summary>
    Trace,

    /// <summary>
    /// Debug-level messages, useful for debugging.
    /// </summary>
    Debug,

    /// <summary>
    /// Informational messages.
    /// </summary>
    Info,

    /// <summary>
    /// Warning messages indicating a potential issue.
    /// </summary>
    Warning,

    /// <summary>
    /// Error messages indicating a failure.
    /// </summary>
    Error,

    /// <summary>
    /// Critical errors that require immediate attention.
    /// </summary>
    Critical
}
