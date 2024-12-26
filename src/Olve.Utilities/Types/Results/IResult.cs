using System.Diagnostics.CodeAnalysis;

namespace Olve.Utilities.Types.Results;

/// <summary>
/// Represents the result of an operation, containing a success indicator and optional problems.
/// </summary>
public interface IResult
{
    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    bool Succeded { get; }

    /// <summary>
    /// Gets the collection of problems associated with the result, if any.
    /// </summary>
    IReadOnlyCollection<ResultProblem>? Problems { get; }

    /// <summary>
    /// Attempts to retrieve the problems associated with the result.
    /// </summary>
    /// <param name="problems">
    /// When this method returns <see langword="true"/>, contains the problems. Otherwise, <see langword="null"/>.
    /// </param>
    /// <returns><see langword="true"/> if problems exist; otherwise, <see langword="false"/>.</returns>
    bool TryGetProblems([NotNullWhen(true)] out IReadOnlyCollection<ResultProblem>? problems);
}