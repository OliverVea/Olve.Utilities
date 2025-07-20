namespace Olve.Results.Validation;

/// <summary>
/// Validator for <see cref="int"/> values.
/// </summary>
/// <param name="value">Value to validate.</param>
public class IntValidator(int value) : NumericValidator<int>(value)
{
    /// <summary>
    /// Gets the validator instance for chaining.
    /// </summary>
    protected override IntValidator Validator => this;
}
