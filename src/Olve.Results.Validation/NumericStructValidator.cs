using System.Numerics;

namespace Olve.Results.Validation;

/// <summary>
/// Validator for numeric values implementing <see cref="INumber{TSelf}"/>.
/// </summary>
/// <typeparam name="TValue">Numeric type.</typeparam>
/// <typeparam name="TValidator">Validator type.</typeparam>
public abstract class NumericStructValidator<TValue, TValidator> : BaseStructValidator<TValue, TValidator>
    where TValue : struct, INumber<TValue>
    where TValidator : NumericStructValidator<TValue, TValidator>
{
    /// <summary>
    /// Fails when the value is less than or equal to <paramref name="limit"/>.
    /// </summary>
    /// <param name="limit">Limit that value must exceed.</param>
    /// <returns>The current validator.</returns>
    public TValidator IsGreaterThan(TValue limit) =>
        FailIf(v => v <= limit, _ => new ResultProblem("Value must be greater than '{0}'", limit));

    /// <summary>
    /// Fails when the value is greater than or equal to <paramref name="limit"/>.
    /// </summary>
    /// <param name="limit">Limit that value must be below.</param>
    /// <returns>The current validator.</returns>
    public TValidator IsLessThan(TValue limit) =>
        FailIf(v => v >= limit, _ => new ResultProblem("Value must be less than '{0}'", limit));

    /// <summary>
    /// Fails when the value is less than <paramref name="limit"/>.
    /// </summary>
    /// <param name="limit">Minimum allowed value.</param>
    /// <returns>The current validator.</returns>
    public TValidator IsGreaterThanOrEqualTo(TValue limit) =>
        FailIf(v => v < limit, _ => new ResultProblem("Value must be greater than or equal to '{0}'", limit));

    /// <summary>
    /// Fails when the value is greater than <paramref name="limit"/>.
    /// </summary>
    /// <param name="limit">Maximum allowed value.</param>
    /// <returns>The current validator.</returns>
    public TValidator IsLessThanOrEqualTo(TValue limit) =>
        FailIf(v => v > limit, _ => new ResultProblem("Value must be less than or equal to '{0}'", limit));

    /// <summary>
    /// Fails when the value is not between <paramref name="min"/> and <paramref name="max"/> inclusive.
    /// </summary>
    /// <param name="min">Minimum allowed value.</param>
    /// <param name="max">Maximum allowed value.</param>
    /// <returns>The current validator.</returns>
    public TValidator IsBetween(TValue min, TValue max) =>
        FailIf(v => v < min || v > max, _ => new ResultProblem("Value must be between '{0}' and '{1}'", min, max));

    /// <summary>
    /// Fails when the value is not positive.
    /// </summary>
    /// <returns>The current validator.</returns>
    public TValidator IsPositive() =>
        FailIf(v => v <= TValue.Zero, _ => new ResultProblem("Value must be positive"));

    /// <summary>
    /// Fails when the value is not negative.
    /// </summary>
    /// <returns>The current validator.</returns>
    public TValidator IsNegative() =>
        FailIf(v => v >= TValue.Zero, _ => new ResultProblem("Value must be negative"));

    /// <summary>
    /// Fails when the value is not equal to zero.
    /// </summary>
    /// <returns>The current validator.</returns>
    public TValidator IsZero() =>
        FailIf(v => v != TValue.Zero, _ => new ResultProblem("Value must be zero"));
}