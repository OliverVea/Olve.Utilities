using Olve.Results;

namespace Olve.Logging;

/// <summary>
/// Manages in-memory logging operations.
/// </summary>
public interface ILoggingManager
{
    /// <summary>
    /// Event that is invoked when the log collection is updated.
    /// Subscribers are notified when new logs are added.
    /// </summary>
    LogsUpdatedEvent LogsUpdatedEvent { get; }
    
    /// <summary>
    /// Add a pre-built <see cref="LogMessage"/> to the log store.
    /// </summary>
    /// <param name="log">The log message to add.</param>
    void Log(LogMessage log);

    /// <summary>
    /// Create and add a log entry with the provided level, message and optional tags.
    /// </summary>
    /// <param name="logLevel">The severity level of the log.</param>
    /// <param name="message">The log message text.</param>
    /// <param name="tags">Optional tags associated with the log entry.</param>
    void Log(LogLevel logLevel, string message, string[]? tags = null);

    /// <summary>
    /// Convert a sequence of <see cref="ResultProblem"/> instances to <see cref="LogMessage"/> and add them to the log store.
    /// </summary>
    /// <param name="resultProblems">The problems to convert and log.</param>
    void Log(params IEnumerable<ResultProblem> resultProblems);

    /// <summary>
    /// Retrieve logs matching the specified request parameters.
    /// </summary>
    /// <param name="request">Filtering and paging parameters for the log query.</param>
    /// <returns>A <see cref="Result{GetLogsResponse}"/> containing the requested logs or a problem.</returns>
    Result<GetLogsResponse> GetLogs(GetLogsRequest request);
}
