using Microsoft.CodeAnalysis;

namespace Olve.SG.CopyProperties.Models;

public class PropertyModel(
    string type,
    string name,
    PropertyAccessModifiersModel accessModifiers,
    Accessibility propertyAccessibility,
    string initializer,
    string? xmlComment,
    string[] attributes,
    string[] namespaces)
{
    public string Type { get; } = type;
    public string Name { get; } = name;
    public PropertyAccessModifiersModel AccessModifiers { get; } = accessModifiers;
    public Accessibility PropertyAccessibility { get; } = propertyAccessibility;
    public string Initializer { get; } = initializer;
    public string? XmlComment { get; } = xmlComment;
    public string[] Attributes { get; } = attributes;
    public string[] Namespaces { get; } = namespaces;
    
    // Deconstruct
    public void Deconstruct(out string type, out string name, out PropertyAccessModifiersModel accessModifiers, out string initializer, out string? xmlComment, out string[] attributes, out string[] namespaces)
    {
        type = Type;
        name = Name;
        accessModifiers = AccessModifiers;
        initializer = Initializer;
        xmlComment = XmlComment;
        attributes = Attributes;
        namespaces = Namespaces;
    }
}