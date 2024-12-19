using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Olve.SG.CopyProperties;

public static class SourceGenerationHelper
{
    public const string Attribute = @"
namespace Olve.SG.CopyProperties;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public class CopyPropertiesAttribute : Attribute
{
    public CopyPropertiesAttribute(Type source)
    {
        
    }
}";
}


[Generator]
public class CopyPropertiesGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "CopyPropertiesAttribute.g.cs", 
            SourceText.From(SourceGenerationHelper.Attribute, Encoding.UTF8)));

        var toGenerate = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "Olve.SG.CopyProperties.CopyPropertiesAttribute",
                predicate: static (_, _) => true,
                transform: static (ctx, _) => GetClassOrStructToGenerate(ctx.SemanticModel, ctx.TargetNode))
            .Where(static m => m is not null);

        
        context.RegisterSourceOutput(toGenerate,
            static (spc, source) => Execute(source, spc));
    }

    static bool IsSyntaxTargetForGeneration(SyntaxNode node)
        => node is TypeDeclarationSyntax { AttributeLists.Count: > 0 };
    

    private static ClassOrStructToGenerate GetClassOrStructToGenerate(SemanticModel semanticModel, SyntaxNode syntaxNode)
    {
        if (syntaxNode is not TypeDeclarationSyntax typeDeclaration)
        {
            throw new InvalidOperationException("This generator can only be used on a type declaration.");
        }

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
            .GetMembers()
            .OfType<IPropertySymbol>() // Filter properties
            .Where(p => p.DeclaredAccessibility == Accessibility.Public && !p.IsStatic) // Only public and non-static
            .Select(p => GetProperty(p, includeInit: !destIsInterface))
            .ToArray();

        return new ClassOrStructToGenerate(destNamespace, destTypeType, destTypeName, publicProperties);
    }
    
    private static Property GetProperty(IPropertySymbol property, bool includeInit = true)
    {
        var type = property.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var name = property.Name;
        var accessModifiers = GetAccessModifiers(property);
        var initializer = property.SetMethod is null || !includeInit ? "" : $"= {property.Name}!;";
    
        var xmlComment = property.GetDocumentationCommentXml();

        var attributes = property.GetAttributes()
            .Select(attr =>
            {
                var attributeName = attr.AttributeClass?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) ?? "";
                if (attributeName.EndsWith("Attribute"))
                {
                    attributeName = attributeName.Substring(0, attributeName.Length - 9); // Remove "Attribute"
                }
                return $"[{attributeName}]";
            })
            .ToArray();

        var namespaces = property.GetAttributes()
            .Select(attr => attr.AttributeClass?.ContainingNamespace.ToDisplayString())
            .Concat([property.Type.ContainingNamespace.ToDisplayString()])
            .Where(ns => !string.IsNullOrEmpty(ns))
            .OfType<string>()
            .Distinct()
            .ToArray();

        return new Property(type, name, accessModifiers, initializer, xmlComment, attributes, namespaces);
    }
    
    private static string GetAccessModifiers(IPropertySymbol property)
    {
        var sb = new StringBuilder();
        if (property.GetMethod is {} getMethod)
        {
            var getAccessModifiers = GetGetAccessModifiers(getMethod);
            sb.Append(getAccessModifiers);
        }
        if (property.SetMethod is {} setMethod)
        {
            var setAccessModifiers = GetSetAccessModifiers(setMethod);
            sb.Append(setAccessModifiers);
        }
        return sb.ToString();
    }

    private static string GetGetAccessModifiers(IMethodSymbol methodSymbol)
    {
        var verb = "get";
        
        return GetAccessModifiers(methodSymbol, verb);
    }
    
    private static string GetSetAccessModifiers(IMethodSymbol methodSymbol)
    {
        var isInit = methodSymbol.Name.Contains("init");
        var verb = isInit ? "init" : "set";
        
        return GetAccessModifiers(methodSymbol, verb);
    }

    private static string GetAccessModifiers(IMethodSymbol methodSymbol, string verb)
    {
        var sb = new StringBuilder();
        
        if (methodSymbol.DeclaredAccessibility == Accessibility.Public)
        {
            sb.Append("public ");
        }

        if (methodSymbol.DeclaredAccessibility == Accessibility.Protected)
        {
            sb.Append("protected ");
        }

        if (methodSymbol.DeclaredAccessibility == Accessibility.Internal)
        {
            sb.Append("internal ");
        }

        if (methodSymbol.DeclaredAccessibility == Accessibility.ProtectedOrInternal)
        {
            sb.Append("protected internal ");
        }

        if (methodSymbol.DeclaredAccessibility == Accessibility.Private)
        {
            sb.Append("private ");
        }

        sb.Append(verb);
        return sb.ToString();
    }

    private static string GetTypeType(TypeDeclarationSyntax typeDeclaration)
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

    private static void Execute(ClassOrStructToGenerate source, SourceProductionContext spc)
    {
        var sourceCode = GenerateSource(source);
        
        spc.AddSource($"{source.TypeName}.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
        
        spc.ReportDiagnostic(Diagnostic.Create(
            new DiagnosticDescriptor(
                "CP001",
                "CopyProperties",
                $"Generated {source.TypeType} {source.TypeName}.",
                "CopyProperties",
                DiagnosticSeverity.Info,
                true),
            Location.None));
    }
    
    private static string GenerateSource(ClassOrStructToGenerate source)
    {
        var sb = new StringBuilder();

        // Collect all unique namespaces
        var namespaces = source.Properties
            .SelectMany(p => p.Namespaces)
            .Distinct()
            .OrderBy(ns => ns);

        // Add using statements
        foreach (var ns in namespaces)
        {
            sb.AppendLine($"using {ns};");
        }

        sb.AppendLine();
        sb.AppendLine($"namespace {source.Namespace};");
        sb.AppendLine();
        sb.AppendLine($"partial {source.TypeType} {source.TypeName}");
        sb.AppendLine("{");

        foreach (var (type, identifier, accessModifiers, _, xmlComment, attributes, _) in source.Properties)
        {
            // Format XML comments into triple-slash style
            if (!string.IsNullOrWhiteSpace(xmlComment))
            {
                var formattedComment = FormatXmlComment(xmlComment!);
                sb.Append(formattedComment);
            }
            
            // Add attributes
            foreach (var attribute in attributes)
            {
                sb.AppendLine($"    {attribute}");
            }

            // Generate property
            sb.AppendLine($"    public {type} {identifier} {{{accessModifiers}}}");
        }

        sb.AppendLine("}");

        return sb.ToString();
    }


    
    private static string FormatXmlComment(string rawXml)
    {
        var sb = new StringBuilder();
    
        // Split the raw XML comment into lines and trim each line
        var lines = rawXml.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            // Exclude the outer <member> tags
            if (line.Contains("<member") || line.Contains("</member>"))
                continue;

            // Add the triple-slash and indent correctly
            sb.AppendLine($"    /// {line.Trim()}");
        }

        return sb.ToString();
    }

}

/*
namespace NAMESPACE;

partial class CLASSNAME
{
    ...
}
*/

public class Property(
    string type,
    string name,
    string accessModifiers,
    string initializer,
    string? xmlComment,
    string[] attributes,
    string[] namespaces)
{
    public string Type { get; } = type;
    public string Name { get; } = name;
    public string AccessModifiers { get; } = accessModifiers;
    public string Initializer { get; } = initializer;
    public string? XmlComment { get; } = xmlComment;
    public string[] Attributes { get; } = attributes;
    public string[] Namespaces { get; } = namespaces;
    
    // Deconstruct
    public void Deconstruct(out string type, out string name, out string accessModifiers, out string initializer, out string? xmlComment, out string[] attributes, out string[] namespaces)
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

public class ClassOrStructToGenerate(string @namespace, string typeType, string typeName, Property[] properties)
{
    public string Namespace { get; } = @namespace;
    public string TypeType { get; } = typeType;
    public string TypeName { get; } = typeName;
    public Property[] Properties { get; } = properties;
}
