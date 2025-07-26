namespace Olve.Validation;

/// <summary>
/// Attribute applied to validators to indicate the target type.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ValidatorForAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidatorForAttribute"/> class.
    /// </summary>
    /// <param name="targetType">Type that the validator handles.</param>
    public ValidatorForAttribute(Type targetType) => TargetType = targetType;

    /// <summary>
    /// Gets the type that the validator targets.
    /// </summary>
    public Type TargetType { get; }
}
