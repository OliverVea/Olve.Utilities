using System.Diagnostics.CodeAnalysis;

namespace Olve.Utilities.Types.Results;

/// <summary>
/// Represents a problem encountered during an operation.
/// </summary>
/// <param name="Message">The message describing the problem.</param>
/// <param name="Tags">Optional tags categorizing the problem.</param>
/// <param name="Severity">The severity level of the problem, where higher values indicate more severe problems.</param>
/// <param name="Args">Optional arguments providing additional details about the problem.</param>
public record ResultProblem(
    [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Message,
    string[] Tags,
    int Severity,
    object[] Args)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResultProblem"/> class.
    /// </summary>
    /// <param name="message">The message describing the problem.</param>
    public ResultProblem(string message)
        : this(message, [], 0, []) { }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ResultProblem"/> class.
    /// </summary>
    /// <param name="message">The message describing the problem.</param>
    /// <param name="args">Optional arguments providing additional details about the problem.</param>
    public ResultProblem([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, params object[] args)
        : this(message, [], 0, args) { }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ResultProblem"/> class.
    /// </summary>
    /// <param name="message">The message describing the problem.</param>
    /// <param name="tags">Optional tags categorizing the problem.</param>
    public ResultProblem(string message, string[] tags)
        : this(message, tags, 0, []) { }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ResultProblem"/> class.
    /// </summary>
    /// <param name="message">The message describing the problem.</param>
    /// <param name="tags">Optional tags categorizing the problem.</param>
    /// <param name="args">Optional arguments providing additional details about the problem.</param>
    public ResultProblem([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, string[] tags, params object[] args)
        : this(message, tags, 0, args) { }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ResultProblem"/> class.
    /// </summary>
    /// <param name="message">The message describing the problem.</param>
    /// <param name="severity">The severity level of the problem, where higher values indicate more severe problems.</param>
    public ResultProblem(string message, int severity)
        : this(message, [], severity, []) { }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ResultProblem"/> class.
    /// </summary>
    /// <param name="message">The message describing the problem.</param>
    /// <param name="severity">The severity level of the problem, where higher values indicate more severe problems.</param>
    /// <param name="args">Optional arguments providing additional details about the problem.</param>
    public ResultProblem([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, int severity, params object[] args)
        : this(message, [], severity, args) { }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ResultProblem"/> class.
    /// </summary>
    /// <param name="message">The message describing the problem.</param>
    /// <param name="tags">Optional tags categorizing the problem.</param>
    /// <param name="severity">The severity level of the problem, where higher values indicate more severe problems.</param>
    public ResultProblem(string message, string[] tags, int severity)
        : this(message, tags, severity, []) { }
}