using System;

namespace Olve.Validation.SourceGeneration;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ValidatorForAttribute : Attribute
{
    public ValidatorForAttribute(Type targetType) => TargetType = targetType;

    public Type TargetType { get; }
}
