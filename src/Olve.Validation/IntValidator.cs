using Olve.Results;

namespace Olve.Validation;

/// <summary>
/// Validator for <see cref="int"/> values.
/// </summary>
public class IntValidator : NumericStructValidator<int, IntValidator>
{
    /// <summary>
    /// Gets the validator instance for chaining.
    /// </summary>
    protected override IntValidator Validator => this;

    /// <summary>
    /// Fails when the value is not even.
    /// </summary>
    /// <returns>The current validator.</returns>
    public IntValidator IsEven() => FailIf(
        v => v % 2 != 0, 
        _ => new ResultProblem("Value must be even"));

    /// <summary>
    /// Fails when the value is not odd.
    /// </summary>
    /// <returns>The current validator.</returns>
    public IntValidator IsOdd() => FailIf(
        v => v % 2 == 0, 
        _ => new ResultProblem("Value must be odd"));
}
