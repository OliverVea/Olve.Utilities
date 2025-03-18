﻿using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

namespace Olve.Results;

/// <summary>
///     Represents a result of an operation without a value, indicating success or failure.
/// </summary>
public readonly struct Result
{
    private Result(ResultProblemCollection? problems)
    {
        Succeeded = problems is null;
        Problems = problems;
    }

    /// <summary>
    ///     Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool Succeeded { get; private init; }

    /// <summary>
    ///     Gets the collection of problems associated with the result, if any.
    /// </summary>
    public ResultProblemCollection? Problems { get; }

    /// <summary>
    ///     Gets a result representing success.
    /// </summary>
    public static Result Success() => new(problems: null);

    /// <summary>
    ///     Creates a result representing success with the specified value.
    /// </summary>
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);

    /// <summary>
    ///     Creates a result representing failure with the specified problems.
    /// </summary>
    /// <param name="problems">The problems associated with the failure.</param>
    /// <returns>A failure result.</returns>
    public static Result Failure(params IEnumerable<ResultProblem> problems) => new(new ResultProblemCollection(problems));

    /// <summary>
    ///     Attempts to execute the specified action and returns a <see cref="Result"/>.
    ///     If an exception of type <typeparamref name="TException"/> is thrown, it is captured
    ///     as a problem in the result.
    /// </summary>
    /// <typeparam name="TException">The type of exception to catch.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <param name="message">
    ///     An optional message providing additional context if an exception is thrown.
    ///     Supports composite formatting.
    /// </param>
    /// <param name="args">Optional arguments for formatting the message.</param>
    /// <returns>
    ///     A successful result if no exception is thrown; otherwise, a failure result
    ///     containing details of the caught exception.
    /// </returns>
    [DebuggerHidden]
    public static Result Try<TException>(Action action, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string? message = null, params object[] args) where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException exception)
        {
            StackFrame stackFrame = new(1, true);
            return new ResultProblem(exception, message ?? string.Empty, args: args, stackFrame);
        }

        return Success();
    }

    /// <summary>
    ///     Attempts to execute the specified function and returns a <see cref="Result{TValue}"/>.
    ///     If an exception of type <typeparamref name="TException"/> is thrown, it is captured
    ///     as a problem in the result.
    /// </summary>
    /// <typeparam name="TValue">The type of value returned by the function.</typeparam>
    /// <typeparam name="TException">The type of exception to catch.</typeparam>
    /// <param name="action">The function to execute.</param>
    /// <param name="message">
    ///     An optional message providing additional context if an exception is thrown.
    ///     Supports composite formatting.
    /// </param>
    /// <param name="args">Optional arguments for formatting the message.</param>
    /// <returns>
    ///     A successful result containing the function's return value if no exception is thrown;
    ///     otherwise, a failure result containing details of the caught exception.
    /// </returns>
    [DebuggerHidden]
    public static Result<TValue> Try<TValue, TException>(Func<TValue> action, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string? message = null, params object[] args) where TException : Exception
    {
        try
        {
            var value = action();
            return value;
        }
        catch (TException exception)
        {
            StackFrame stackFrame = new(1, true);
            return new ResultProblem(exception, message ?? string.Empty, args: args, stackFrame);
        }
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
    ///     Converts the specified problem to a failure result.
    /// </summary>
    /// <param name="problem">The problem to convert.</param>
    /// <returns>A failure result.</returns>
    public static implicit operator Result(ResultProblem problem) => Failure(problem);

    /// <summary>
    ///     Converts the specified problems to a failure result.
    /// </summary>
    /// <param name="problems">The problems to convert.</param>
    /// <returns>A failure result.</returns>
    public static implicit operator Result(ResultProblemCollection problems) => Failure(problems);
}