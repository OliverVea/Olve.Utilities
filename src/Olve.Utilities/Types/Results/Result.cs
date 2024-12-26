using System.Diagnostics.CodeAnalysis;

namespace Olve.Utilities.Types.Results;

/// <summary>
/// Represents a result of an operation without a value, indicating success or failure.
/// </summary>
public readonly struct Result : IResult
{
    private Result(IReadOnlyCollection<ResultProblem>? problems)
    {
        Succeded = problems is null;
        Problems = problems;
    }

    /// <inheritdoc/>
    public bool Succeded { get; private init; }

    /// <inheritdoc/>
    public IReadOnlyCollection<ResultProblem>? Problems { get; private init; }

    /// <summary>
    /// Gets a result representing success.
    /// </summary>
    public static Result Success => new(null);

    /// <summary>
    /// Creates a result representing failure with the specified problems.
    /// </summary>
    /// <param name="problems">The problems associated with the failure.</param>
    /// <returns>A failure result.</returns>
    public static Result Failure(IReadOnlyCollection<ResultProblem> problems) => new(problems);

    /// <inheritdoc/>
    public bool TryGetProblems([NotNullWhen(true)] out IReadOnlyCollection<ResultProblem>? problems)
    {
        problems = Problems;
        return problems is not null;
    }
}
