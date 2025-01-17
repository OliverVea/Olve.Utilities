using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Olve.Utilities.Types.Results;

/// <summary>
///     Represents a problem encountered during an operation.
/// </summary>
/// <param name="exception">The exception that caused the problem, if any.</param>
/// <param name="message">The message describing the problem.</param>
/// <param name="args">Optional arguments providing additional details about the problem.</param>
[DebuggerDisplay("{ToString()}")]
public class ResultProblem(
    Exception? exception,
    [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message,
    params object[] args)
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ResultProblem" /> class.
    /// </summary>
    /// <param name="message">The message describing the problem.</param>
    /// <param name="args">Optional arguments providing additional details about the problem.</param>
    public ResultProblem([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, params object[] args) : this(
        null,
        message,
        args)
    {
    }

    /// <summary>
    ///     Gets the message describing the problem.
    /// </summary>
    public string Message { get; } = message;

    /// <summary>
    ///     Gets the optional tags categorizing the problem.
    /// </summary>
    public string[] Tags { get; init; } = [];

    /// <summary>
    ///     Gets the severity level of the problem, where higher values indicate more severe problems.
    /// </summary>
    public int Severity { get; init; } = 0;

    /// <summary>
    ///     Gets the optional arguments providing additional details about the problem.
    /// </summary>
    public object[] Args { get; } = args;

    /// <summary>
    ///     Gets the source of the problem, if any.
    /// </summary>
    public string? Source { get; init; }

    /// <summary>
    ///     Gets the exception that caused the problem, if any.
    /// </summary>
    public Exception? Exception { get; } = exception;

    /// <summary>
    ///     Formats the problem as a string.
    /// </summary>
    /// <returns>The formatted string.</returns>
    public override string ToString() => Exception != null
        ? $"{string.Format(Message, Args)} ({Exception.GetType().Name}: {Exception.Message})"
        : string.Format(Message, Args);
}