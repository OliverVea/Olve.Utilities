using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Olve.Results;

/// <summary>
///     Represents a problem encountered during an operation.
/// </summary>
[DebuggerDisplay("{ToString()}")]
public class ResultProblem
{
    /// <summary>
    ///     Gets or sets the default source of the problem, if any.
    /// </summary>
    public static string? DefaultSource = null;

    /// <summary>
    ///     Gets or sets the default tags categorizing the problem.
    /// </summary>
    public static string[] DefaultTags = [];

    /// <summary>
    ///     Gets or sets the default severity level of the problem.
    /// </summary>
    public static int DefaultSeverity = 0;

    /// <summary>
    ///    Gets or sets the default value indicating whether to print debug information.
    /// </summary>
    public static bool DefaultPrintDebug = false;

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

    /// <summary>
    ///     Initializes a new instance of the <see cref="ResultProblem" /> class for JSON deserialization.
    /// </summary>
    [JsonConstructor]
    public ResultProblem(string formattedMessage, string[]? tags, int severity, string? source)
    {
        Message = formattedMessage;
        Args = [];
        Tags = tags ?? [];
        Severity = severity;
        Source = source;
        Exception = null;
        OriginInformation = default;
    }

    internal ResultProblem(Exception? exception,
        string message,
        object[] args,
        StackFrame stackFrame)
    {
        Exception = exception;
        Message = message;
        Args = args;

        var path = Paths.Path.Create(stackFrame.GetFileName() ?? string.Empty);
        var lineNumber = stackFrame.GetFileLineNumber();

        OriginInformation = new ProblemOriginInformation(path, lineNumber);
    }


    /// <summary>
    ///     Gets the raw format string describing the problem.
    /// </summary>
    [JsonIgnore]
    public string Message { get; }

    /// <summary>
    ///     Gets the formatted message describing the problem, with arguments applied.
    /// </summary>
    [JsonPropertyName("message")]
    public string FormattedMessage => Args.Length > 0 ? string.Format(Message, Args) : Message;

    /// <summary>
    ///     Gets the optional tags categorizing the problem.
    ///     The default value can be set using <see cref="DefaultTags"/>.
    /// </summary>
    public string[] Tags { get; init; } = DefaultTags;

    /// <summary>
    ///     Gets the severity level of the problem, where higher values indicate more severe problems.
    ///     The default value can be set using <see cref="DefaultSeverity"/>.
    /// </summary>
    public int Severity { get; init; } = DefaultSeverity;

    /// <summary>
    ///     Gets the optional arguments providing additional details about the problem.
    /// </summary>
    [JsonIgnore]
    public object[] Args { get; }

    /// <summary>
    ///     Gets the source of the problem, if any.
    ///     The default value can be set using <see cref="DefaultSource"/>.
    /// </summary>
    public string? Source { get; init; } = DefaultSource;

    /// <summary>
    ///     Gets the exception that caused the problem, if any.
    /// </summary>
    [JsonIgnore]
    public Exception? Exception { get; }

    /// <summary>
    ///     Gets the origin information of the problem.
    /// </summary>
    [JsonIgnore]
    public ProblemOriginInformation OriginInformation { get; }


    /// <summary>
    ///     Formats the problem as a string.
    ///     If <see cref="DefaultPrintDebug"/> is <see langword="true" />, <see cref="ToDebugString"/> is used; otherwise, <see cref="ToBriefString"/> is used.
    /// </summary>
    /// <returns>The formatted string.</returns>
    public override string ToString() => DefaultPrintDebug ? ToDebugString() : ToBriefString();


    /// <summary>
    ///    Formats the problem as a string for brief display, omitting references to code locations.
    /// </summary>
    /// <returns>The formatted string.</returns>
    public string ToBriefString() => Exception != null
        ? $"{string.Format(Message, Args)} ({Exception.GetType().Name}: {Exception.Message})"
        : string.Format(Message, Args);

    /// <summary>
    ///    Formats the problem as a string for debugging purposes.
    /// </summary>
    /// <returns>The formatted string.</returns>
    public string ToDebugString()
    {
        var linkString = OriginInformation.LinkString;
        var message = ToBriefString();

        return $"[{linkString}] {message}";
    }
}