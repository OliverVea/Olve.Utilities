using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Olve.Validation.SourceGenerators;

[Generator(LanguageNames.CSharp)]
public sealed class ValidatorForGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var validators = context.SyntaxProvider.ForAttributeWithMetadataName(
                "Olve.Validation.SourceGeneration.ValidatorForAttribute",
                static (n, _) => true,
                static (ctx, _) => (INamedTypeSymbol)ctx.TargetSymbol)
            .Where(static s => s != null);

        context.RegisterSourceOutput(validators, static (spc, symbol) =>
        {
            var attr = symbol.GetAttributes().First(a =>
                a.AttributeClass?.ToDisplayString() == "Olve.Validation.SourceGeneration.ValidatorForAttribute");
            GenerateForClass(spc, symbol, attr);
        });
    }

    private static void GenerateForClass(SourceProductionContext context, INamedTypeSymbol validatorClass, AttributeData attribute)
    {
        if (attribute.ConstructorArguments.Length == 0 || attribute.ConstructorArguments[0].Value is not INamedTypeSymbol targetType)
            return;

        var methods = validatorClass.GetMembers().OfType<IMethodSymbol>()
            .Where(m => m.IsStatic && m.Name.StartsWith("Get") && m.Name.EndsWith("Validator"))
            .ToImmutableArray();

        var properties = targetType.GetMembers().OfType<IPropertySymbol>()
            .Where(p => p.DeclaredAccessibility == Accessibility.Public)
            .ToImmutableArray();

        var lines = new List<string>();
        foreach (var property in properties)
        {
            var methodName = $"Get{property.Name}Validator";
            var method = methods.FirstOrDefault(m => m.Name == methodName);
            if (method is null)
                continue;
            lines.Add($"descriptor.For(x => x.{property.Name}, {methodName}(), nameof({targetType.Name}.{property.Name}));");
        }

        var ns = validatorClass.ContainingNamespace.IsGlobalNamespace ? null : validatorClass.ContainingNamespace.ToDisplayString();
        var body = string.Join("\n", lines.Select(l => "        " + l));
        var source = $"namespace {ns}\n{{\n    partial class {validatorClass.Name}\n    {{\n        protected override void Configure(Olve.Validation.ValidationDescriptor<{targetType.ToDisplayString()}> descriptor)\n        {{\n{body}\n        }}\n    }}\n}}";
        context.AddSource($"{validatorClass.Name}.ValidatorFor.g.cs", source);
    }
}
