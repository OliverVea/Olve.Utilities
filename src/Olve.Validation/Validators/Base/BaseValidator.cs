using Olve.Results;

namespace Olve.Validation.Validators.Base;

/// <summary>
/// Provides a base implementation for creating validators with fluent API support.
/// </summary>
/// <typeparam name="TValue">Type of the value being validated.</typeparam>
/// <typeparam name="TValidator">Concrete validator type for fluent chaining.</typeparam>
public abstract class BaseValidator<TValue, TValidator> : IValidator<TValue>
    where TValidator : BaseValidator<TValue, TValidator>
{
    private record ValidationRule(
        Func<TValue, bool> Condition,
        Func<TValue, ResultProblem> ProblemFactory,
        bool ProblemOverwritten = false)
    {
        /// <summary>
        /// Executes the validation rule against the specified value.
        /// </summary>
        /// <param name="value">Value to validate.</param>
        /// <returns>
        /// A <see cref="ResultProblem"/> if <c>Condition</c> evaluates to true; otherwise, <see cref="Result.Success()"/>.
        /// </returns>
        public Result Execute(TValue value)
        {
            return Condition(value)
                ? ProblemFactory(value)
                : Result.Success();
        }
    }

    private readonly List<ValidationRule> _validationRules = new();

    /// <summary>
    /// Gets the current validator instance for chaining additional rules.
    /// </summary>
    protected abstract TValidator Validator { get; }

    /// <summary>
    /// Overrides the problem produced by the last added validation rule.
    /// </summary>
    /// <param name="problemFactory">
    /// A factory that creates a <see cref="ResultProblem"/> to replace the existing failure.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if no validation rules have been defined or if the last rule's problem has already been overwritten.
    /// </exception>
    /// <returns>The current validator instance.</returns>
    public TValidator WithProblem(Func<TValue, ResultProblem> problemFactory)
    {
        if (_validationRules.Count == 0)
        {
            throw new InvalidOperationException("No validation rules have been defined.");
        }

        var previousRule = _validationRules[^1];
        if (previousRule.ProblemOverwritten)
        {
            throw new InvalidOperationException("The previous rule's problem has already been overwritten.");
        }

        _validationRules[^1] = previousRule with { ProblemFactory = problemFactory, ProblemOverwritten = true };
        return Validator;
    }

    /// <summary>
    /// Overrides the problem produced by the last added validation rule.
    /// </summary>
    /// <param name="problem">The <see cref="ResultProblem"/> to use for the last validation failure.</param>
    /// <returns>The current validator instance.</returns>
    public TValidator WithProblem(ResultProblem problem) => WithProblem(_ => problem);

    /// <summary>
    /// Overrides the message of the problem produced by the last added validation rule.
    /// </summary>
    /// <param name="message">The message to use in the replacement <see cref="ResultProblem"/>.</param>
    /// <returns>The current validator instance.</returns>
    public TValidator WithMessage(string message) => WithProblem(_ => new ResultProblem(message));

    /// <summary>
    /// Validates the specified value against all configured rules.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <returns>
    /// A <see cref="ResultProblem"/> collection if any rule fails; otherwise, <see cref="Result.Success()"/>.
    /// </returns>
    public Result Validate(TValue value)
    {
        var results = _validationRules.Select(rule => rule.Execute(value));
        return results.TryPickProblems(out var problems)
            ? problems
            : Result.Success();
    }

    /// <summary>
    /// Adds a validation rule that fails when the specified condition is true.
    /// </summary>
    /// <param name="condition">
    /// A predicate that indicates a validation failure when it returns true for the provided value.
    /// </param>
    /// <param name="problemFactory">
    /// A factory that creates a <see cref="ResultProblem"/> when <paramref name="condition"/> is met.
    /// </param>
    /// <returns>The current validator instance.</returns>
    protected TValidator FailIf(
        Func<TValue, bool> condition,
        Func<TValue, ResultProblem> problemFactory)
    {
        _validationRules.Add(new ValidationRule(condition, problemFactory));
        return Validator;
    }
}
