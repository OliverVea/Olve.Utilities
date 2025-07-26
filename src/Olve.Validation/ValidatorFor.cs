using Olve.Results;

namespace Olve.Validation;

/// <summary>
/// Base class for object validators.
/// </summary>
/// <typeparam name="T">Type being validated.</typeparam>
public abstract partial class ValidatorFor<T> : IValidator<T>
{
    private readonly ValidationDescriptor<T> _descriptor = new();

    /// <summary>
    /// Initializes a new instance of the validator.
    /// </summary>
    protected ValidatorFor()
    {
        Configure(_descriptor);
    }

    /// <summary>
    /// Configures the validation descriptor.
    /// </summary>
    /// <param name="descriptor">Descriptor to populate.</param>
    protected abstract void Configure(ValidationDescriptor<T> descriptor);

    /// <summary>
    /// Validates the given value using configured rules.
    /// </summary>
    /// <param name="value">Value to validate.</param>
    /// <returns>Aggregated validation result.</returns>
    public Result Validate(T value) => _descriptor.Validate(value);
}
