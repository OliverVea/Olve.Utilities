using Olve.Results;

namespace Olve.Validation;

/// <summary>
/// Provides validation methods for reference types.
/// </summary>
/// <typeparam name="TValue">The reference type being validated.</typeparam>
/// <typeparam name="TValidator">The type of the validator implementing this class.</typeparam>
public abstract class BaseObjectValidator<TValue, TValidator> : BaseValidator<TValue?, TValidator>
    where TValue : class
    where TValidator : BaseObjectValidator<TValue, TValidator>
{
    /// <summary>
    /// Fails validation if the value is null.
    /// </summary>
    /// <returns>The current validator instance.</returns>
    public TValidator IsNotNull() => FailIf(
        value => value == null, 
        _ => new ResultProblem("Value was null"));

    /// <summary>
    /// Fails validation if the value is not one of the allowed values.
    /// </summary>
    /// <param name="values">The collection of allowed values.</param>
    /// <returns>The current validator instance.</returns>
    public TValidator IsNotOneOf(ICollection<TValue?> values) => FailIf(
        value => !values.Contains(value), 
        _ => new ResultProblem("Value was not one of the allowed values: [{0}]", string.Join(", ", values)));
}
