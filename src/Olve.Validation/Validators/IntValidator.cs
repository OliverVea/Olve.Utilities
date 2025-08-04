using Olve.Results;
using Olve.Validation.Validators.Base;

namespace Olve.Validation.Validators;

/// <summary>
/// Validator for <see cref="int"/> values.
/// </summary>
public class IntValidator : BaseNumericStructValidator<int, IntValidator>
{
    /// <inheritdoc />
    protected override IntValidator Validator => this;

    /// <summary>
    /// Fails when the value is not even.
    /// </summary>
    /// <returns>The current validator.</returns>
    public IntValidator MustBeEven() => FailIf(
        v => v % 2 != 0, 
        _ => new ResultProblem("Value must be even"));

    /// <summary>
    /// Fails when the value is not odd.
    /// </summary>
    /// <returns>The current validator.</returns>
    public IntValidator MustBeOdd() => FailIf(
        v => v % 2 == 0, 
        _ => new ResultProblem("Value must be odd"));
}
