using System.Numerics;
using Olve.Validation.Validators.Base;

namespace Olve.Validation.Validators;

/// <summary>
/// Validator for numeric values of type <typeparamref name="TValue"/>.
/// </summary>
/// <typeparam name="TValue">Numeric type being validated.</typeparam>
public class DecimalValidator<TValue> : BaseNumericStructValidator<TValue, DecimalValidator<TValue>>
    where TValue : struct, INumber<TValue>
{
    /// <inheritdoc />
    protected override DecimalValidator<TValue> Validator => this;
}
