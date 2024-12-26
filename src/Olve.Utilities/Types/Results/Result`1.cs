using System.Diagnostics.CodeAnalysis;

namespace Olve.Utilities.Types.Results;

/// <summary>
/// Represents a result of an operation with a value, indicating success or failure.
/// </summary>
/// <typeparam name="T">The type of the result value.</typeparam>
public readonly struct Result<T> : IResult<T>
{
    private Result(T? result, IReadOnlyCollection<ResultProblem>? problems)
    {
        Succeded = problems is null;
        Value = result;
        Problems = problems;
    }

    /// <inheritdoc/>
    public bool Succeded { get; }

    /// <inheritdoc/>
    public IReadOnlyCollection<ResultProblem>? Problems { get; }

    /// <inheritdoc/>
    public T? Value { get; }

    /// <summary>
    /// Creates a result representing success with the specified value.
    /// </summary>
    /// <param name="value">The value associated with the success.</param>
    /// <returns>A success result.</returns>
    public static Result<T> Success(T value) => new(value, null);

    /// <summary>
    /// Creates a result representing failure with the specified problems.
    /// </summary>
    /// <param name="problems">The problems associated with the failure.</param>
    /// <returns>A failure result.</returns>
    public static Result<T> Failure(IReadOnlyCollection<ResultProblem> problems) => new(default, problems);

    /// <inheritdoc/>
    public bool TryPickProblems([NotNullWhen(true)] out IReadOnlyCollection<ResultProblem>? problems)
    {
        problems = Problems;
        return problems is not null;
    }

    /// <inheritdoc/>
    public bool TryPickValue([NotNullWhen(true)] out T? value, [NotNullWhen(false)] out IReadOnlyCollection<ResultProblem>? problems)
    {
        value = Value;
        problems = Problems;
        return value is not null;
    }

    /// <inheritdoc/>
    public bool TryPickProblems([NotNullWhen(true)] out IReadOnlyCollection<ResultProblem>? problems, [NotNullWhen(false)] out T? value)
    {
        problems = Problems;
        value = Value;
        return problems is not null;
    }
    
    /// <summary>
    /// Converts the specified value to a success result.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>A success result.</returns>
    public static implicit operator Result<T>(T value) => Success(value);
}