using Olve.Results;

namespace Olve.Validation;

/// <summary>
/// Provides validation methods for struct types.
/// </summary>
/// <typeparam name="TValue">The struct type being validated.</typeparam>
/// <typeparam name="TValidator">The type of the validator implementing this class.</typeparam>
public abstract class BaseStructValidator<TValue, TValidator> : BaseValidator<TValue, TValidator>
    where TValue : struct
    where TValidator : BaseStructValidator<TValue, TValidator>
{
    /// <summary>
    /// Fails validation if the value is the default for its type.
    /// </summary>
    protected TValidator IsNotDefault() => FailIf(
        value => value.Equals(default(TValue)),
        _ => new ResultProblem("Value was default"));
}
