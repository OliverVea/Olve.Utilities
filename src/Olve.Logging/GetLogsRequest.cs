namespace Olve.Logging;

/// <summary>
/// Request parameters used to query logs from <see cref="ILoggingManager"/>.
/// </summary>
/// <param name="Query">Optional search query to match against message text, source path and tags.</param>
/// <param name="Count">Maximum number of log entries to return.</param>
/// <param name="LogLevel">Minimum <see cref="LogLevel"/> to include.</param>
/// <param name="Since">Only include logs with a time greater than or equal to this value. If default, no lower bound is applied.</param>
public readonly record struct GetLogsRequest(string Query = "", int Count = 100, LogLevel LogLevel = LogLevel.Info, DateTime Since = default);
