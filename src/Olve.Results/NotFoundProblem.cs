using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Olve.Results;

/// <summary>
///     A problem signalling that the thing an operation needed does not exist (an unknown id or key).
///     Transports can recover it by type, e.g. Olve.MinimalApi answers 404 Not Found.
/// </summary>
[DebuggerDisplay("{ToString()}")]
public sealed class NotFoundProblem : ResultProblem
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="NotFoundProblem" /> class.
    /// </summary>
    /// <param name="message">The message describing what was not found.</param>
    /// <param name="args">Optional arguments providing additional details about the problem.</param>
    [StackTraceHidden]
    public NotFoundProblem([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message,
        params object[] args) : base(null, message, args, new StackFrame(1, true))
    {
    }
}
