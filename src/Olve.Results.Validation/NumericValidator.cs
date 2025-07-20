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

    /// <summary>
    /// Fails when the value is less than <paramref name="limit"/>.
    /// </summary>
    /// <param name="limit">Minimum allowed value.</param>
    /// <returns>The current validator.</returns>
    public NumericValidator<T> IsGreaterThanOrEqualTo(T limit) =>
        FailIf(v => v < limit, () => new ResultProblem($"Value must be greater than or equal to {limit}"));

    /// <summary>
    /// Fails when the value is greater than <paramref name="limit"/>.
    /// </summary>
    /// <param name="limit">Maximum allowed value.</param>
    /// <returns>The current validator.</returns>
    public NumericValidator<T> IsLessThanOrEqualTo(T limit) =>
        FailIf(v => v > limit, () => new ResultProblem($"Value must be less than or equal to {limit}"));

    /// <summary>
    /// Fails when the value is not between <paramref name="min"/> and <paramref name="max"/> inclusive.
    /// </summary>
    /// <param name="min">Minimum allowed value.</param>
    /// <param name="max">Maximum allowed value.</param>
    /// <returns>The current validator.</returns>
    public NumericValidator<T> IsBetween(T min, T max) =>
        FailIf(v => v < min || v > max, () => new ResultProblem($"Value must be between {min} and {max}"));

    /// <summary>
    /// Fails when the value is not positive.
    /// </summary>
    /// <returns>The current validator.</returns>
    public NumericValidator<T> IsPositive() =>
        FailIf(v => v <= T.Zero, () => new ResultProblem("Value must be positive"));

    /// <summary>
    /// Fails when the value is not negative.
    /// </summary>
    /// <returns>The current validator.</returns>
    public NumericValidator<T> IsNegative() =>
        FailIf(v => v >= T.Zero, () => new ResultProblem("Value must be negative"));

    /// <summary>
    /// Fails when the value is not equal to zero.
    /// </summary>
    /// <returns>The current validator.</returns>
    public NumericValidator<T> IsZero() =>
        FailIf(v => v != T.Zero, () => new ResultProblem("Value must be zero"));
}
