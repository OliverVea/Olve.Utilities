using System.Numerics;
using Olve.Validation.Validators.Base;

namespace Olve.Validation.Validators;

/// <summary>
/// Validator for <see cref="decimal"/> values.
/// </summary>
public class DecimalValidator<TValue> : BaseNumericStructValidator<TValue, DecimalValidator<TValue>>
    where TValue : struct, INumber<TValue>
{
    /// <summary>
    /// Gets the validator instance for chaining.
    /// </summary>
    protected override DecimalValidator<TValue> Validator => this;
}