using System.Numerics;

namespace Olve.Results.Validation;

/// <summary>
/// Validator for <see cref="decimal"/> values.
/// </summary>
public class DecimalValidator<TValue> : NumericStructValidator<TValue, DecimalValidator<TValue>>
    where TValue : struct, INumber<TValue>
{
    /// <summary>
    /// Gets the validator instance for chaining.
    /// </summary>
    protected override DecimalValidator<TValue> Validator => this;
}