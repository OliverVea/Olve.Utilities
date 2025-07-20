namespace Olve.Results.Validation;

/// <summary>
/// Validator for <see cref="double"/> values.
/// </summary>
/// <param name="value">Value to validate.</param>
public class DoubleValidator(double value) : NumericValidator<double>(value)
{
    /// <summary>
    /// Gets the validator instance for chaining.
    /// </summary>
    protected override DoubleValidator Validator => this;
}
