using Olve.Results;
using Olve.Validation.Validators.Base;

namespace Olve.Validation.Validators;

/// <summary>
/// Validator for string values.
/// </summary>
public class StringValidator : BaseObjectValidator<string, StringValidator>
{
    /// <summary>
    /// Gets the validator instance for chaining.
    /// </summary>
    protected override StringValidator Validator => this;

    /// <summary>
    /// Fails when the string is null or empty.
    /// </summary>
    /// <returns>The current validator.</returns>
    public StringValidator IsNotNullOrEmpty() =>
        FailIf(string.IsNullOrEmpty, _ => new ResultProblem("Value is null or empty"));

    /// <summary>
    /// Fails when the string is null or white space.
    /// </summary>
    /// <returns>The current validator.</returns>
    public StringValidator IsNotNullOrWhiteSpace() =>
        FailIf(string.IsNullOrWhiteSpace, _ => new ResultProblem("Value is null or white space"));

    /// <summary>
    /// Fails when the string length is shorter than <paramref name="minLength"/>.
    /// </summary>
    /// <param name="minLength">Minimum length allowed.</param>
    /// <returns>The current validator.</returns>
    public StringValidator MinLength(int minLength)
    {
        if (minLength < 0) throw new ArgumentException("minLength must be non-negative", nameof(minLength));
        return FailIf(v => v?.Length < minLength, _ => new ResultProblem("Value must be at least '{0}' characters", minLength));
    }

    /// <summary>
    /// Fails when the string length is longer than <paramref name="maxLength"/>.
    /// </summary>
    /// <param name="maxLength">Maximum length allowed.</param>
    /// <returns>The current validator.</returns>
    public StringValidator MaxLength(int maxLength)
    {
        if (maxLength < 0) throw new ArgumentException("maxLength must be non-negative", nameof(maxLength));
        return FailIf(v => v?.Length > maxLength, _ => new ResultProblem("Value must be at most '{0}' characters", maxLength));
    }
}
