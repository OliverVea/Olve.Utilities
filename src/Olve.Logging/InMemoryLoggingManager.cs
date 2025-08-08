using Microsoft.Extensions.Logging;
using Olve.Results;

namespace Olve.Logging;

/// <summary>
/// An in-memory <see cref="ILoggingManager"/> implementation intended for tests and short-lived applications.
/// </summary>
/// <param name="logger">Optional Microsoft logger to forward messages to.</param>
public class InMemoryLoggingManager(ILogger<InMemoryLoggingManager>? logger = null) : ILoggingManager
{
    private readonly List<LogMessage> _logs = [];

    /// <summary>
    /// Event that is invoked when the log collection changes.
    /// </summary>
    public LogsUpdatedEvent LogsUpdatedEvent { get; } = new();
    
    /// <summary>
    /// Add a pre-built <see cref="LogMessage"/> to the in-memory store and notify subscribers.
    /// </summary>
    /// <param name="log">The message to add.</param>
    public void Log(LogMessage log)
    {
        _logs.Add(log);
        LogsUpdatedEvent.Invoke();
        logger?.Log(log.Level.ToMicrosoftLogLevel(), log.Message);
    }

    /// <summary>
    /// Create and add a log entry with the provided level, message and optional tags.
    /// </summary>
    /// <param name="logLevel">Severity of the log.</param>
    /// <param name="message">Message text.</param>
    /// <param name="tags">Optional tags.</param>
    public void Log(LogLevel logLevel, string message, string[]? tags = null)
    {
        Log(new LogMessage(logLevel, message, null, null, DateTime.Now, tags));
    }

    /// <summary>
    /// Convert problems to <see cref="LogMessage"/> instances and add them to the log store.
    /// </summary>
    /// <param name="problems">Problems to log.</param>
    public void Log(params IEnumerable<ResultProblem> problems)
    {
        foreach (var problem in problems)
        {
            Log(problem.ToLogMessage());
        }
    }

    /// <summary>
    /// Retrieve logs filtered and paged according to <see cref="GetLogsRequest"/>.
    /// </summary>
    /// <param name="request">Request describing filtering and paging parameters.</param>
    /// <returns>A <see cref="Result{GetLogsResponse}"/> containing the matching logs.</returns>
    public Result<GetLogsResponse> GetLogs(GetLogsRequest request)
    {
        var logs = _logs.AsEnumerable();

        logs = logs.Where(log => log.Level >= request.LogLevel);

        if (request.Query is { Length: > 0 } query)
        {
            logs = logs.Where(x => x.SearchableText.Contains(query, StringComparison.InvariantCultureIgnoreCase));
        }

        logs = logs.Where(x => x.Time >= request.Since);
        
        logs = logs
            .Reverse()
            .Take(request.Count)
            .Reverse();

        return new GetLogsResponse(logs);
    }
}
