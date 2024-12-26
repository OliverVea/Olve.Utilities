using System.Diagnostics.CodeAnalysis;

namespace Olve.Utilities.Types.Results;

/// <summary>
/// Represents the result of an operation with a value, containing a success indicator and optional problems.
/// </summary>
/// <typeparam name="TResult">The type of the result value.</typeparam>
public interface IResult<TResult> : IResult
{
    /// <summary>
    /// Gets the value associated with the result, if any.
    /// </summary>
    TResult? Value { get; }

    /// <summary>
    /// Attempts to retrieve the value of the result.
    /// </summary>
    /// <param name="value">
    /// When this method returns <see langword="true"/>, contains the value. Otherwise, <see langword="null"/>.
    /// </param>
    /// <param name="problems">
    /// When this method returns <see langword="false"/>, contains the problems. Otherwise, <see langword="null"/>.
    /// </param>
    /// <returns><see langword="true"/> if a value exists; otherwise, <see langword="false"/>.</returns>
    bool TryGetValue([NotNullWhen(true)] out TResult? value, [NotNullWhen(false)] out IReadOnlyCollection<ResultProblem>? problems);

    /// <summary>
    /// Attempts to retrieve the problems associated with the result.
    /// </summary>
    /// <param name="problems">
    /// When this method returns <see langword="true"/>, contains the problems. Otherwise, <see langword="null"/>.
    /// </param>
    /// <param name="value">
    /// When this method returns <see langword="false"/>, contains the value. Otherwise, <see langword="null"/>.
    /// </param>
    /// <returns><see langword="true"/> if problems exist; otherwise, <see langword="false"/>.</returns>
    bool TryGetProblems([NotNullWhen(true)] out IReadOnlyCollection<ResultProblem>? problems, [NotNullWhen(false)] out TResult? value);
}