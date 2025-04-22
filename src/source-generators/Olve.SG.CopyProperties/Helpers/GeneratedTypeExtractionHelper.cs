using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Olve.SG.CopyProperties.Models;

namespace Olve.SG.CopyProperties.Helpers;

public static class GeneratedTypeExtractionHelper
{
    public static GeneratedTypeModel ExtractGeneratedType(
        INamedTypeSymbol[] sourceSymbols,
        INamedTypeSymbol destinationSymbol,
        TypeDeclarationSyntax typeDeclaration
    )
    {
        var properties = sourceSymbols
            .SelectMany(PropertyEnumerationHelper.GetProperties)
            .ToArray();

        var typeName = TypeExtractionHelper.GetType(typeDeclaration);

        return new GeneratedTypeModel(
            destinationSymbol.ContainingNamespace.ToDisplayString(),
            typeName,
            destinationSymbol.Name,
            destinationSymbol.DeclaredAccessibility,
            properties
        );
    }

    /*


        var attribute = typeDeclaration
            .AttributeLists
            .SelectMany(al => al.Attributes)
            .FirstOrDefault(a => a.Name.ToString() == "CopyProperties");

        if (attribute?.ArgumentList?.Arguments.FirstOrDefault()?.Expression is not TypeOfExpressionSyntax typeOfExpression)
        {
            throw new InvalidOperationException("The CopyPropertiesAttribute must have a type argument.");
        }

        if (ModelExtensions.GetTypeInfo(semanticModel, typeOfExpression.Type).Type is not INamedTypeSymbol sourceType)
        {
            throw new InvalidOperationException("The type argument of the CopyPropertiesAttribute must be a valid type.");
        }

        var destNamespace = typeDeclaration.FirstAncestorOrSelf<BaseNamespaceDeclarationSyntax>()?.Name.ToString()
                            ?? throw new InvalidOperationException("The destination type must be in a namespace.");
        var destTypeType = GetTypeType(typeDeclaration);
        var destTypeName = typeDeclaration.Identifier.Text;

        var destIsInterface = typeDeclaration is InterfaceDeclarationSyntax;

        var publicProperties = sourceType

        return new GeneratedTypeModel(destNamespace, destTypeType, destTypeName, publicProperties);
     */
}
