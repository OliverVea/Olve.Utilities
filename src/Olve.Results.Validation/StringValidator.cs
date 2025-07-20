using Olve.Results;

namespace Olve.Results.Validation;

/// <summary>
/// Validator for string values.
/// </summary>
/// <param name="value">String to validate.</param>
public class StringValidator(string value) : BaseValidator<string, StringValidator>(value)
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
        FailIf(string.IsNullOrEmpty, () => new ResultProblem("Value is null or empty"));

    /// <summary>
    /// Fails when the string is null or white space.
    /// </summary>
    /// <returns>The current validator.</returns>
    public StringValidator IsNotNullOrWhiteSpace() =>
        FailIf(string.IsNullOrWhiteSpace, () => new ResultProblem("Value is null or white space"));

    /// <summary>
    /// Fails when the string length is shorter than <paramref name="minLength"/>.
    /// </summary>
    /// <param name="minLength">Minimum length allowed.</param>
    /// <returns>The current validator.</returns>
    public StringValidator MinLength(int minLength) =>
        FailIf(v => v.Length < minLength, () => new ResultProblem($"Value must be at least {minLength} characters"));

    /// <summary>
    /// Fails when the string length is longer than <paramref name="maxLength"/>.
    /// </summary>
    /// <param name="maxLength">Maximum length allowed.</param>
    /// <returns>The current validator.</returns>
    public StringValidator MaxLength(int maxLength) =>
        FailIf(v => v.Length > maxLength, () => new ResultProblem($"Value must be at most {maxLength} characters"));
}
