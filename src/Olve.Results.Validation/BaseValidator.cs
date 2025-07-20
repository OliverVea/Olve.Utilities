using System.Diagnostics;
using Olve.Results;

namespace Olve.Results.Validation;

/// <summary>
/// Provides common validation functionality.
/// </summary>
/// <typeparam name="TValue">Type of value being validated.</typeparam>
/// <typeparam name="TValidator">Concrete validator type.</typeparam>
public abstract class BaseValidator<TValue, TValidator>(TValue value)
    where TValidator : BaseValidator<TValue, TValidator>
{
    private readonly List<ResultProblem> _problems = [];
    private bool _lastFailed;

    /// <summary>
    /// Gets the validator instance for fluent chaining.
    /// </summary>
    protected abstract TValidator Validator { get; }

    private TValue Value => value;

    private void PutProblem(ResultProblem resultProblem) => _problems.Add(resultProblem);

    /// <summary>
    /// Replaces the last problem when the preceding validation failed.
    /// </summary>
    /// <param name="problemFormat">Message format.</param>
    /// <param name="args">Optional arguments.</param>
    /// <returns>The current validator.</returns>
    [StackTraceHidden]
    public TValidator WithProblem(string problemFormat, object[]? args = null)
    {
        if (_lastFailed)
        {
            _problems[^1] = new ResultProblem(problemFormat, args ?? []);
        }

        return Validator;
    }

    private Result Validate() => _problems.Count != 0 ? Result.Failure(_problems) : Result.Success();

    /// <summary>
    /// Executes <paramref name="condition" /> and adds a problem when it returns true.
    /// </summary>
    /// <param name="condition">Condition that signals failure.</param>
    /// <param name="problemFactory">Factory producing a <see cref="ResultProblem"/>.</param>
    /// <returns>The current validator.</returns>
    protected TValidator FailIf(Func<TValue, bool> condition, Func<ResultProblem> problemFactory)
    {
        _lastFailed = condition(Value);

        if (_lastFailed)
        {
            var resultProblem = problemFactory();
            PutProblem(resultProblem);
        }

        return Validator;
    }

    /// <summary>
    /// Implicitly converts the validator into a <see cref="Result"/>.
    /// </summary>
    /// <param name="baseValidator">The validator instance.</param>
    public static implicit operator Result(BaseValidator<TValue, TValidator> baseValidator) => baseValidator.Validate();
}
