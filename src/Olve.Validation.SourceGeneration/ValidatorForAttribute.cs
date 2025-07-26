using System;

namespace Olve.Validation.SourceGeneration;

/// <summary>
/// Marks a validator class for the specified target type.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ValidatorForAttribute : Attribute
{
    /// <summary>
    /// Initializes the attribute.
    /// </summary>
    /// <param name="targetType">Type that the validator targets.</param>
    public ValidatorForAttribute(Type targetType) => TargetType = targetType;

    /// <summary>
    /// Gets the target type.
    /// </summary>
    public Type TargetType { get; }
}
