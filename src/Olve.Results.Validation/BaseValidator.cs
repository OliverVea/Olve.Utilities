namespace Olve.Results.Validation;

/// <summary>
/// Provides common validation functionality.
/// </summary>
/// <typeparam name="TValidator">Concrete validator type.</typeparam>
/// <typeparam name="TValue"></typeparam>
public abstract class BaseValidator<TValue, TValidator>
    where TValidator : BaseValidator<TValue, TValidator>
{
    private record ValidationRule(Func<TValue, bool> Condition, Func<TValue, ResultProblem> ProblemFactory)
    {
        public Result Execute(TValue value)
        {
            if (Condition(value))
            {
                return ProblemFactory(value);
            }
            
            return Result.Success();
        }
    }
    
    private readonly List<ValidationRule> _validationRules = [];

    /// <summary>
    /// Gets the validator instance for fluent chaining.
    /// </summary>
    protected abstract TValidator Validator { get; }

    /// <summary>
    /// Replaces the last problem when the preceding validation failed.
    /// </summary>
    /// <returns>The current validator.</returns>
    public TValidator WithProblem(Func<TValue, ResultProblem> problemFactory)
    {
            _validationRules[^1] = _validationRules[^1] with { ProblemFactory = problemFactory };

            return Validator;
    }


    /// <summary>
    /// Runs the validation
    /// </summary>
    /// <returns>Result depending on the match between the value and the configured validation rules.</returns>
    public Result Validate(TValue value)
    {
        var results = _validationRules.Select(x => x.Execute(value));
        if (results.TryPickProblems(out var problems))
        {
            return problems;
        }
        
        return Result.Success();
    }

    /// <summary>
    /// Executes <paramref name="condition" /> and adds a problem when it returns true.
    /// </summary>
    /// <param name="condition">Condition that signals failure.</param>
    /// <param name="problemFactory">Factory producing a <see cref="ResultProblem"/>.</param>
    /// <returns>The current validator.</returns>
    protected TValidator FailIf(Func<TValue, bool> condition, Func<TValue, ResultProblem> problemFactory)
    {
        _validationRules.Add(new ValidationRule(condition, problemFactory));
        return Validator;
    }
}