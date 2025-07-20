namespace Olve.Results.Validation;

/// <summary>
/// Validator for <see cref="float"/> values.
/// </summary>
/// <param name="value">Value to validate.</param>
public class FloatValidator(float value) : NumericValidator<float>(value)
{
    /// <summary>
    /// Gets the validator instance for chaining.
    /// </summary>
    protected override FloatValidator Validator => this;
}
