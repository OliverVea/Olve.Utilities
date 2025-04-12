using System.Diagnostics.CodeAnalysis;

namespace Olve.Results;

/// <summary>
///     Represents the result of a deletion operation, which can succeed, fail due to not being found, or fail due to an error.
/// </summary>
public readonly struct DeletionResult
{
    private DeletionResult(bool found, ResultProblemCollection? problems)
    {
        Problems = problems;

        Succeeded = found && problems is null;
        WasNotFound = !found && problems is null;
    }

    /// <summary>
    ///     Gets a value indicating whether the deletion succeeded.
    /// </summary>
    public bool Succeeded { get; }

    /// <summary>
    ///     Gets a value indicating whether the deletion failed due to the entity not being found.
    /// </summary>
    public bool WasNotFound { get; }

    /// <summary>
    ///     Gets the collection of problems associated with the result, if any.
    /// </summary>
    public ResultProblemCollection? Problems { get; }

    /// <summary>
    ///     Creates a deletion result representing success.
    /// </summary>
    public static DeletionResult Success() => new(true, null);

    /// <summary>
    ///     Creates a deletion result representing failure due to the entity not being found.
    /// </summary>
    public static DeletionResult NotFound() => new(false, null);

    /// <summary>
    ///     Creates a deletion result representing failure due to an error.
    /// </summary>
    /// <param name="problems">The problems associated with the failure.</param>
    /// <returns>A failure result.</returns>
    public static DeletionResult Error(params IEnumerable<ResultProblem> problems) =>
        new(false, new ResultProblemCollection(problems));

    /// <summary>
    ///     Attempts to retrieve the problems associated with the result.
    /// </summary>
    /// <param name="problems">
    ///     When this method returns <see langword="true" />, contains the problems. Otherwise, <see langword="null" />.
    /// </param>
    /// <returns><see langword="true" /> if problems exist; otherwise, <see langword="false" />.</returns>
    public bool TryPickProblems([NotNullWhen(true)] out ResultProblemCollection? problems)
    {
        problems = Problems;
        return problems is not null;
    }
}
