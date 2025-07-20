namespace Olve.Results.Validation;

/// <summary>
/// Validator for <see cref="decimal"/> values.
/// </summary>
/// <param name="value">Value to validate.</param>
public class DecimalValidator(decimal value) : NumericValidator<decimal>(value)
{
    /// <summary>
    /// Gets the validator instance for chaining.
    /// </summary>
    protected override DecimalValidator Validator => this;
}
