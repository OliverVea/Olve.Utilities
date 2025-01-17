using Microsoft.CodeAnalysis;
using Olve.SG.CopyProperties.Models;

namespace Olve.SG.CopyProperties.Helpers;

public static class PropertyEnumerationHelper
{
    public static IEnumerable<PropertyModel> GetProperties(INamedTypeSymbol namedTypeSymbol) =>
        namedTypeSymbol
            .GetMembers()
            .OfType<IPropertySymbol>()
            //.Where(ShouldBeCopied)
            .Select(PropertyExtractionHelper.ExtractProperty);

    private static bool ShouldBeCopied(IPropertySymbol propertySymbol) =>
        propertySymbol is { DeclaredAccessibility: Accessibility.Public, IsStatic: false };
}