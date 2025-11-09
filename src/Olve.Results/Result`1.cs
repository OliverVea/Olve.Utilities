using System.Diagnostics.CodeAnalysis;

namespace Olve.Results;

/// <summary>
///     Represents a result of an operation with a value, indicating success or failure.
/// </summary>
/// <typeparam name="T">The type of the result value.</typeparam>
public readonly struct Result<T>
{
    internal Result(T? result, ResultProblemCollection? problems)
    {
        Succeeded = problems is null;
        Value = result;
        Problems = problems;
    }

    /// <summary>
    ///     Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool Succeeded { get; }

    /// <summary>
    ///     Gets a value indicating whether the operation failed.
    /// </summary>
    public bool Failed => !Succeeded;

    /// <summary>
    ///     Gets the collection of problems associated with the result, if any.
    /// </summary>
    public ResultProblemCollection? Problems { get; }

    /// <summary>
    ///     Gets the value associated with the result, if any.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    ///     Creates a result representing success with the specified value.
    /// </summary>
    /// <param name="value">The value associated with the success.</param>
    /// <returns>A success result.</returns>
    public static Result<T> Success(T value) => new(value, problems: null);

    /// <summary>
    ///     Creates a result representing failure with the specified problems.
    /// </summary>
    /// <param name="problems">The problems associated with the failure.</param>
    /// <returns>A failure result.</returns>
    [Obsolete("Use Result.Failure<T> instead")]
    public static Result<T> Failure(params IEnumerable<ResultProblem> problems) =>
        new(default, new ResultProblemCollection(problems));

    /// <summary>
    ///     Attempts to retrieve the value of the result.
    /// </summary>
    /// <param name="value">
    ///     When this method returns <see langword="true" />, contains the value. Otherwise, <see langword="null" />.
    /// </param>
    /// <returns><see langword="true" /> if a value exists; otherwise, <see langword="false" />.</returns>
    public bool TryPickValue([NotNullWhen(true)] out T? value)
    {
        value = Value;
        return value is not null;
    }

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

    /// <summary>
    ///     Attempts to retrieve the value of the result.
    /// </summary>
    /// <param name="value">
    ///     When this method returns <see langword="true" />, contains the value. Otherwise, <see langword="null" />.
    /// </param>
    /// <param name="problems">
    ///     When this method returns <see langword="false" />, contains the problems. Otherwise, <see langword="null" />.
    /// </param>
    /// <returns><see langword="true" /> if a value exists; otherwise, <see langword="false" />.</returns>
    public bool TryPickValue([NotNullWhen(true)] out T? value, [NotNullWhen(false)] out ResultProblemCollection? problems)
    {
        value = Value;
        problems = Problems;
        return value is not null;
    }

    /// <summary>
    ///     Attempts to retrieve the problems associated with the result.
    /// </summary>
    /// <param name="problems">
    ///     When this method returns <see langword="true" />, contains the problems. Otherwise, <see langword="null" />.
    /// </param>
    /// <param name="value">
    ///     When this method returns <see langword="false" />, contains the value. Otherwise, <see langword="null" />.
    /// </param>
    /// <returns><see langword="true" /> if problems exist; otherwise, <see langword="false" />.</returns>
    public bool TryPickProblems([NotNullWhen(true)] out ResultProblemCollection? problems,
        [NotNullWhen(false)] out T? value)
    {
        problems = Problems;
        value = Value;
        return problems is not null;
    }

    /// <summary>
    ///     Gets the value of the result or throws an exception if the result is a failure.
    /// </summary>
    /// <param name="defaultValue">The default value to return if the result is a failure.</param>
    /// <returns>The value of the result or the default value.</returns>
    public T GetValueOrDefault(T defaultValue) => Value ?? defaultValue;

    /// <summary>
    ///     Attempts to retrieve the value of the result or a default value.
    /// </summary>
    /// <param name="value">The value of the result or the default value.</param>
    /// <param name="defaultValue">The default value to return if the result is a failure.</param>
    /// <returns><see langword="true" /> if a value exists; otherwise, <see langword="false" />.</returns>
    public bool TryGetValueOrDefault([NotNullWhen(true)] out T? value, T defaultValue)
    {
        value = Value;
        return value is not null || defaultValue is not null;
    }

    /// <summary>
    ///     Executes a sequence of functions that return a <see cref="Result{TValue}"/>.
    /// </summary>
    /// <param name="action">The function to execute.</param>
    /// <returns>The result of the function.</returns>
    public Result<T> IfProblem(Action<ResultProblemCollection> action)
    {
        if (Problems is not null)
        {
            action(Problems);
        }

        return this;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Succeeded ? $"Success({Value})" : "Failure";
    }

    /// <summary>
    ///     Converts the specified value to a success result.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>A success result.</returns>
    public static implicit operator Result<T>(T value) => Success(value);

    /// <summary>
    ///     Converts the specified problem to a failure result.
    /// </summary>
    /// <param name="problem">The problem to convert.</param>
    /// <returns>A failure result.</returns>
    public static implicit operator Result<T>(ResultProblem problem) => Result.Failure<T>(problem);

    /// <summary>
    ///     Converts the specified problems to a failure result.
    /// </summary>
    /// <param name="problems">The problems to convert.</param>
    /// <returns>A failure result.</returns>
    public static implicit operator Result<T>(ResultProblemCollection problems) => Result.Failure<T>(problems);
}