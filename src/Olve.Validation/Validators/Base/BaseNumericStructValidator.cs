using System.Numerics;
using Olve.Results;

namespace Olve.Validation.Validators.Base;

/// <summary>
/// Validator for numeric values implementing <see cref="INumber{TSelf}"/>.
/// </summary>
/// <typeparam name="TValue">Numeric type.</typeparam>
/// <typeparam name="TValidator">Validator type.</typeparam>
public abstract class BaseNumericStructValidator<TValue, TValidator> : BaseStructValidator<TValue, TValidator>
    where TValue : struct, INumber<TValue>
    where TValidator : BaseNumericStructValidator<TValue, TValidator>
{
    /// <summary>
    /// Fails when the value is less than or equal to <paramref name="limit"/>.
    /// </summary>
    /// <param name="limit">Limit that value must exceed.</param>
    /// <returns>The current validator.</returns>
    public TValidator MustBeGreaterThan(TValue limit) =>
        FailIf(v => v <= limit, _ => new ResultProblem("Value must be greater than '{0}'", limit));

    /// <summary>
    /// Fails when the value is greater than or equal to <paramref name="limit"/>.
    /// </summary>
    /// <param name="limit">Limit that value must be below.</param>
    /// <returns>The current validator.</returns>
    public TValidator MustBeLessThan(TValue limit) =>
        FailIf(v => v >= limit, _ => new ResultProblem("Value must be less than '{0}'", limit));

    /// <summary>
    /// Fails when the value is less than <paramref name="limit"/>.
    /// </summary>
    /// <param name="limit">Minimum allowed value.</param>
    /// <returns>The current validator.</returns>
    public TValidator MustBeGreaterThanOrEqualTo(TValue limit) =>
        FailIf(v => v < limit, _ => new ResultProblem("Value must be greater than or equal to '{0}'", limit));

    /// <summary>
    /// Fails when the value is greater than <paramref name="limit"/>.
    /// </summary>
    /// <param name="limit">Maximum allowed value.</param>
    /// <returns>The current validator.</returns>
    public TValidator MustBeLessThanOrEqualTo(TValue limit) =>
        FailIf(v => v > limit, _ => new ResultProblem("Value must be less than or equal to '{0}'", limit));

    /// <summary>
    /// Fails when the value is not between <paramref name="min"/> and <paramref name="max"/> inclusive.
    /// </summary>
    /// <param name="min">Minimum allowed value.</param>
    /// <param name="max">Maximum allowed value.</param>
    /// <returns>The current validator.</returns>
    public TValidator MustBeBetween(TValue min, TValue max) =>
        FailIf(v => v < min || v > max, _ => new ResultProblem("Value must be between '{0}' and '{1}'", min, max));

    /// <summary>
    /// Fails when the value is not positive.
    /// </summary>
    /// <returns>The current validator.</returns>
    public TValidator MustBePositive() =>
        FailIf(v => v <= TValue.Zero, _ => new ResultProblem("Value must be positive"));

    /// <summary>
    /// Fails when the value is not negative.
    /// </summary>
    /// <returns>The current validator.</returns>
    public TValidator MustBeNegative() =>
        FailIf(v => v >= TValue.Zero, _ => new ResultProblem("Value must be negative"));

    /// <summary>
    /// Fails when the value is not equal to zero.
    /// </summary>
    /// <returns>The current validator.</returns>
    public TValidator MustBeZero() =>
        FailIf(v => v != TValue.Zero, _ => new ResultProblem("Value must be zero"));
}