using Microsoft.CodeAnalysis;

namespace Olve.SG.CopyProperties.Models;

public class GeneratedTypeModel(
    string @namespace,
    string typeType,
    string typeName,
    Accessibility typeAccessibility,
    PropertyModel[] properties
)
{
    public string Namespace { get; } = @namespace;
    public string TypeType { get; } = typeType;
    public string TypeName { get; } = typeName;
    public Accessibility TypeAccessibility { get; } = typeAccessibility;
    public PropertyModel[] Properties { get; } = properties;
}
