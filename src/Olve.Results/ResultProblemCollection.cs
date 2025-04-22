using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

namespace Olve.Results;

/// <summary>
///     Represents a collection of problems encountered during an operation.
/// </summary>
/// <param name="problems">The problems encountered during the operation.</param>
public class ResultProblemCollection(params IEnumerable<ResultProblem> problems)
    : IEnumerable<ResultProblem>
{
    /// <inheritdoc />
    public IEnumerator<ResultProblem> GetEnumerator() => problems.AsEnumerable().GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    ///     Appends the specified problems to the collection.
    /// </summary>
    /// <param name="resultProblems">The problems to append.</param>
    /// <returns>A new collection with the specified problems appended.</returns>
    public ResultProblemCollection Append(params IEnumerable<ResultProblem> resultProblems) =>
        new(problems.Concat(resultProblems));

    /// <summary>
    ///     Prepends the specified problems to the collection.
    /// </summary>
    /// <param name="resultProblems">The problems to prepend.</param>
    /// <returns>A new collection with the specified problems prepended.</returns>
    public ResultProblemCollection Prepend(params IEnumerable<ResultProblem> resultProblems) =>
        new(resultProblems.Concat(problems));

    /// <summary>
    ///     Prepends a new problem to the collection using a formatted message.
    /// </summary>
    /// <param name="message">The format string for the problem message.</param>
    /// <param name="args">The arguments to format the message.</param>
    /// <returns>A new collection with the formatted problem prepended.</returns>
    public ResultProblemCollection Prepend(
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message,
        params object[] args
    ) => Prepend(new ResultProblem(null, message, args: args, stackFrame: new StackFrame(1, true)));

    /// <summary>
    ///     Prepends a new problem from an exception to the collection using a formatted message.
    /// </summary>
    /// <param name="exception">The exception causing the problem.</param>
    /// <param name="message">The format string for the problem message.</param>
    /// <param name="args">The arguments to format the message.</param>
    /// <returns>A new collection with the formatted problem prepended.</returns>
    public ResultProblemCollection Prepend(
        Exception exception,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message,
        params object[] args
    ) =>
        Prepend(
            new ResultProblem(exception, message, args: args, stackFrame: new StackFrame(1, true))
        );

    /// <summary>
    ///     Merges multiple problem collections together into a single collection.
    /// </summary>
    /// <param name="problemCollections">The problem collections to merge.</param>
    /// <returns>A new collection containing all problems from the specified collections.</returns>
    public static ResultProblemCollection Merge(
        params IEnumerable<ResultProblemCollection> problemCollections
    )
    {
        var allProblems = problemCollections.SelectMany(x => x);
        return new ResultProblemCollection(allProblems);
    }
}
