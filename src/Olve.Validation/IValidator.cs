namespace Olve.Validation;

/// <summary>
/// Defines a validator for values of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Value type to validate.</typeparam>
public interface IValidator<in T>
{
    /// <summary>
    /// Validates the specified value.
    /// </summary>
    /// <param name="value">Value to validate.</param>
    /// <returns>A <see cref="Olve.Results.Result"/> describing the outcome.</returns>
    Olve.Results.Result Validate(T value);
}
