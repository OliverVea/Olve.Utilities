using System.Numerics;
using Olve.Results;

namespace Olve.Results.Validation;

/// <summary>
/// Validator for numeric values implementing <see cref="INumber{TSelf}"/>.
/// </summary>
/// <typeparam name="T">Numeric type.</typeparam>
/// <param name="value">Value to validate.</param>
public abstract class NumericValidator<T>(T value) : BaseValidator<T, NumericValidator<T>>(value)
    where T : INumber<T>
{
    /// <summary>
    /// Gets the validator instance for chaining.
    /// </summary>
    protected override NumericValidator<T> Validator => this;

    /// <summary>
    /// Fails when the value is less than or equal to <paramref name="limit"/>.
    /// </summary>
    /// <param name="limit">Limit that value must exceed.</param>
    /// <returns>The current validator.</returns>
    public NumericValidator<T> IsGreaterThan(T limit) =>
        FailIf(v => v <= limit, () => new ResultProblem($"Value must be greater than {limit}"));

    /// <summary>
    /// Fails when the value is greater than or equal to <paramref name="limit"/>.
    /// </summary>
    /// <param name="limit">Limit that value must be below.</param>
    /// <returns>The current validator.</returns>
    public NumericValidator<T> IsLessThan(T limit) =>
        FailIf(v => v >= limit, () => new ResultProblem($"Value must be less than {limit}"));
}
