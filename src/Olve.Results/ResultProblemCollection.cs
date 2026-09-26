using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Olve.Results;

/// <summary>
///     Represents a collection of problems encountered during an operation.
/// </summary>
/// <param name="problems">The problems encountered during the operation.</param>
public class ResultProblemCollection(params IEnumerable<ResultProblem> problems) : IEnumerable<ResultProblem>
{
    /// <inheritdoc />
    public IEnumerator<ResultProblem> GetEnumerator() => problems
        .AsEnumerable()
        .GetEnumerator();

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
    ///     Gets a value indicating whether the problems in this collection may be resolved by retrying:
    ///     <see langword="true" /> if the collection is non-empty and every problem is
    ///     <see cref="ResultProblem.IsRetryable">retryable</see>; otherwise <see langword="false" />.
    /// </summary>
    public bool IsRetryable => problems.Any() && problems.All(p => p.IsRetryable);

    /// <summary>
    ///     Prepends a new problem to the collection using a formatted message, adding context to the existing problems.
    ///     The new problem inherits <see cref="IsRetryable" /> from this collection.
    /// </summary>
    /// <param name="message">The format string for the problem message.</param>
    /// <param name="args">The arguments to format the message.</param>
    /// <returns>A new collection with the formatted problem prepended.</returns>
    public ResultProblemCollection Prepend([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message,
        params object[] args)
        => Prepend(new ResultProblem(null, message, args: args, stackFrame: new StackFrame(1, true))
        {
            IsRetryable = IsRetryable,
        });

    /// <summary>
    ///     Prepends a new problem to the collection using a formatted message, adding context to the existing problems.
    /// </summary>
    /// <param name="retryable">Whether the new problem is <see cref="ResultProblem.IsRetryable">retryable</see>.</param>
    /// <param name="message">The format string for the problem message.</param>
    /// <param name="args">The arguments to format the message.</param>
    /// <returns>A new collection with the formatted problem prepended.</returns>
    public ResultProblemCollection Prepend(bool retryable,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message,
        params object[] args)
        => Prepend(new ResultProblem(null, message, args: args, stackFrame: new StackFrame(1, true))
        {
            IsRetryable = retryable,
        });

    /// <summary>
    ///     Prepends a new problem from an exception to the collection using a formatted message, adding context to the
    ///     existing problems. The new problem inherits <see cref="IsRetryable" /> from this collection.
    /// </summary>
    /// <param name="exception">The exception causing the problem.</param>
    /// <param name="message">The format string for the problem message.</param>
    /// <param name="args">The arguments to format the message.</param>
    /// <returns>A new collection with the formatted problem prepended.</returns>
    public ResultProblemCollection Prepend(Exception exception, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message,
        params object[] args)
        => Prepend(new ResultProblem(exception, message, args: args, stackFrame: new StackFrame(1, true))
        {
            IsRetryable = IsRetryable,
        });

    /// <summary>
    ///     Prepends a new problem from an exception to the collection using a formatted message, adding context to the
    ///     existing problems.
    /// </summary>
    /// <param name="retryable">Whether the new problem is <see cref="ResultProblem.IsRetryable">retryable</see>.</param>
    /// <param name="exception">The exception causing the problem.</param>
    /// <param name="message">The format string for the problem message.</param>
    /// <param name="args">The arguments to format the message.</param>
    /// <returns>A new collection with the formatted problem prepended.</returns>
    public ResultProblemCollection Prepend(bool retryable, Exception exception,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message,
        params object[] args)
        => Prepend(new ResultProblem(exception, message, args: args, stackFrame: new StackFrame(1, true))
        {
            IsRetryable = retryable,
        });

    /// <summary>
    ///     Attempts to retrieve the first problem assignable to <typeparamref name="TProblem" />, in enumeration order.
    /// </summary>
    /// <typeparam name="TProblem">The problem type to look for, typically a subclass of <see cref="ResultProblem" />.</typeparam>
    /// <param name="problem">
    ///     When this method returns <see langword="true" />, contains the first matching problem. Otherwise, <see langword="null" />.
    /// </param>
    /// <returns><see langword="true" /> if a matching problem exists; otherwise, <see langword="false" />.</returns>
    public bool TryPickProblem<TProblem>([NotNullWhen(true)] out TProblem? problem)
        where TProblem : ResultProblem
    {
        problem = this.OfType<TProblem>().FirstOrDefault();
        return problem is not null;
    }

    /// <summary>
    ///     Gets all problems assignable to <typeparamref name="TProblem" />, in enumeration order.
    /// </summary>
    /// <typeparam name="TProblem">The problem type to look for, typically a subclass of <see cref="ResultProblem" />.</typeparam>
    /// <returns>The matching problems, or an empty sequence if there are none.</returns>
    public IEnumerable<TProblem> PickProblems<TProblem>()
        where TProblem : ResultProblem
        => this.OfType<TProblem>();

    /// <summary>
    ///     Merges multiple problem collections together into a single collection.
    /// </summary>
    /// <param name="problemCollections">The problem collections to merge.</param>
    /// <returns>A new collection containing all problems from the specified collections.</returns>
    public static ResultProblemCollection Merge(params IEnumerable<ResultProblemCollection> problemCollections)
    {
        var allProblems = problemCollections.SelectMany(x => x);
        return new ResultProblemCollection(allProblems);
    }
}
