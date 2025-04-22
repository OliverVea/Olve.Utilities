using Microsoft.CodeAnalysis;
using Olve.SG.CopyProperties.Models;

namespace Olve.SG.CopyProperties.Helpers;

public static class PropertyExtractionHelper
{
    public static PropertyModel ExtractProperty(IPropertySymbol propertySymbol)
    {
        var type = propertySymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var name = propertySymbol.Name;
        var accessModifiers = PropertyAccessModifiersExtractionHelper.GetAccessModifiers(
            propertySymbol
        );
        var accessibility = propertySymbol.DeclaredAccessibility;

        var initializer = "";

        var xmlComment = propertySymbol.GetDocumentationCommentXml();

        var attributes = propertySymbol
            .GetAttributes()
            .Select(attr =>
            {
                var attributeName =
                    attr.AttributeClass?.ToDisplayString(
                        SymbolDisplayFormat.MinimallyQualifiedFormat
                    ) ?? "";
                if (attributeName.EndsWith("Attribute"))
                {
                    attributeName = attributeName.Substring(0, attributeName.Length - 9);
                }

                return $"[{attributeName}]";
            })
            .ToArray();

        var namespaces = propertySymbol
            .GetAttributes()
            .Select(attr => attr.AttributeClass?.ContainingNamespace.ToDisplayString())
            .Concat([propertySymbol.Type.ContainingNamespace.ToDisplayString()])
            .Where(ns => !string.IsNullOrEmpty(ns))
            .OfType<string>()
            .Distinct()
            .ToArray();

        return new PropertyModel(
            type,
            name,
            accessModifiers,
            accessibility,
            initializer,
            xmlComment,
            attributes,
            namespaces
        );
    }
}
