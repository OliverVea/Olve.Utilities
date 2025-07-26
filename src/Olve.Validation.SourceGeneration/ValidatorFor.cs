using Olve.Results;

namespace Olve.Validation.SourceGeneration;

/// <summary>
/// Base class for validators targeting a specific type.
/// </summary>
/// <typeparam name="T">Type being validated.</typeparam>
public abstract class ValidatorFor<T> : IValidator<T>
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
    /// Configures property validators.
    /// </summary>
    /// <param name="descriptor">Descriptor to add rules to.</param>
    protected abstract void Configure(ValidationDescriptor<T> descriptor);

    /// <summary>
    /// Validates the specified instance.
    /// </summary>
    /// <param name="value">Instance to validate.</param>
    /// <returns>Validation result.</returns>
    public Result Validate(T value) => _descriptor.Validate(value);
}
