namespace Olve.Logging;

/// <summary>
/// Response returned from <see cref="ILoggingManager.GetLogs(GetLogsRequest)"/> containing the matching messages.
/// </summary>
/// <param name="Messages">The sequence of <see cref="LogMessage"/> entries matching the request.</param>
public readonly record struct GetLogsResponse(IEnumerable<LogMessage> Messages);
