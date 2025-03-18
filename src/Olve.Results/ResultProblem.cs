using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Olve.Results;

/// <summary>
///     Represents a problem encountered during an operation.
/// </summary>
[DebuggerDisplay("{ToString()}")]
public class ResultProblem
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ResultProblem" /> class.
    /// </summary>
    /// <param name="message">The message describing the problem.</param>
    /// <param name="args">Optional arguments providing additional details about the problem.</param>
    [StackTraceHidden]
    public ResultProblem([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message,
        params object[] args) : this(null, message, args, new StackFrame(1, true))
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ResultProblem" /> class from an <see cref="System.Exception"/>.
    /// </summary>
    /// <param name="exception">The exception that caused the problem, if any.</param>
    /// <param name="message">The message describing the problem.</param>
    /// <param name="args">Optional arguments providing additional details about the problem.</param>
    [StackTraceHidden]
    public ResultProblem(Exception exception,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string message,
        params object[] args) : this(exception, message, args, new StackFrame(1, true))
    {
    }

    internal ResultProblem(Exception? exception,
        string message,
        object[] args,
        StackFrame stackFrame)
    {
        Exception = exception;
        Message = message;
        Args = args;

        OriginInformation = new ProblemOriginInformation(stackFrame.GetFileName() ?? string.Empty, stackFrame.GetFileLineNumber());
    }


    /// <summary>
    ///     Gets the message describing the problem.
    /// </summary>
    public string Message { get; }

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
    public object[] Args { get; }

    /// <summary>
    ///     Gets the source of the problem, if any.
    /// </summary>
    public string? Source { get; init; }

    /// <summary>
    ///     Gets the exception that caused the problem, if any.
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    ///     Gets the origin information of the problem.
    /// </summary>
    public ProblemOriginInformation OriginInformation { get; }


    /// <summary>
    ///     Formats the problem as a string.
    /// </summary>
    /// <returns>The formatted string.</returns>
    public override string ToString() => Exception != null
        ? $"{string.Format(Message, Args)} ({Exception.GetType().Name}: {Exception.Message})"
        : string.Format(Message, Args);


    /// <summary>
    ///    Formats the problem as a string for debugging purposes.
    /// </summary>
    /// <returns>The formatted string.</returns>
    public string ToDebugString()
    {
        StringBuilder sb = new();

        sb.Append("[");
        sb.Append(OriginInformation.FilePath);
        sb.Append(":l");
        sb.Append(OriginInformation.LineNumber);
        sb.Append("] ");

        sb.Append(string.Format(Message, Args));

        if (Exception != null)
        {
            sb.Append($" ({Exception.GetType().Name}: {Exception.Message})");
        }

        return sb.ToString();
    }
}