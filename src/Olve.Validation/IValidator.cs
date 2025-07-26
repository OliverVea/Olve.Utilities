using Olve.Results;

namespace Olve.Validation;

/// <summary>
/// Represents a validator for values of type <typeparamref name="TValue"/>.
/// </summary>
/// <typeparam name="TValue">Type of value being validated.</typeparam>
public interface IValidator<in TValue>
{
    /// <summary>
    /// Validates the specified value.
    /// </summary>
    /// <param name="value">Value to validate.</param>
    /// <returns>Validation result.</returns>
    Result Validate(TValue value);
}
