using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Olve.SG.CopyProperties.Helpers;

public static class TypeExtractionHelper
{
    public static string GetType(TypeDeclarationSyntax typeDeclaration)
    {
        return typeDeclaration switch
        {
            ClassDeclarationSyntax => "class",
            RecordDeclarationSyntax recordDeclarationSyntax => GetTypeType(recordDeclarationSyntax),
            StructDeclarationSyntax => "struct",
            InterfaceDeclarationSyntax => "interface",
            _ => throw new InvalidOperationException($"The destination type must be a class or a struct. Was {typeDeclaration.Kind()}.")
        };
    }
    
    private static string GetTypeType(RecordDeclarationSyntax recordDeclarationSyntax)
    {
        return recordDeclarationSyntax.ClassOrStructKeyword.IsKind(SyntaxKind.StructKeyword)
            ? "record struct"
            : "record";
    }
}