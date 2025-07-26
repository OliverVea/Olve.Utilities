using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Olve.Validation.SourceGenerators;

[Generator]
public sealed class ValidatorForGenerator : IIncrementalGenerator
{
    private const string AttributeName = "Olve.Validation.SourceGeneration.ValidatorForAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var symbols = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                AttributeName,
                static (n, _) => true,
                static (ctx, _) => ctx.TargetSymbol is INamedTypeSymbol t ? t : null);

        var combined = symbols.Combine(context.CompilationProvider);

        context.RegisterSourceOutput(combined, static (spc, pair) =>
        {
            if (pair.Left is not null)
            {
                GenerateForClass(spc, pair.Left, pair.Right);
            }
        });
    }

    private static void GenerateForClass(SourceProductionContext context, INamedTypeSymbol symbol, Compilation compilation)
    {
        var attr = symbol.GetAttributes().First(a => a.AttributeClass?.ToDisplayString() == AttributeName);
        var targetType = attr.ConstructorArguments.FirstOrDefault().Value as INamedTypeSymbol;
        if (targetType is null)
            return;

        var validatorInterface = compilation.GetTypeByMetadataName("Olve.Validation.IValidator`1");
        if (validatorInterface is null)
            return;

        var props = targetType.GetMembers().OfType<IPropertySymbol>().ToDictionary(p => p.Name);
        var mappings = new List<(string Property, string Method)>();

        foreach (var method in symbol.GetMembers().OfType<IMethodSymbol>())
        {
            if (!method.IsStatic || method.Parameters.Length != 0)
                continue;
            if (method.ReturnType is not INamedTypeSymbol returnType)
                continue;
            if (!returnType.OriginalDefinition.Equals(validatorInterface, SymbolEqualityComparer.Default))
                continue;
            if (!method.Name.StartsWith("Get") || !method.Name.EndsWith("Validator"))
                continue;

            var propertyName = method.Name.Substring(3, method.Name.Length - 3 - "Validator".Length);
            if (!props.TryGetValue(propertyName, out var prop))
                continue;
            if (!prop.Type.Equals(returnType.TypeArguments[0], SymbolEqualityComparer.Default))
                continue;

            mappings.Add((propertyName, method.Name));
        }

        if (mappings.Count == 0)
            return;

        var ns = symbol.ContainingNamespace.IsGlobalNamespace ? null : symbol.ContainingNamespace.ToDisplayString();
        var sb = new StringBuilder();
        sb.AppendLine("using Olve.Validation;");
        if (ns is not null)
        {
            sb.Append("namespace ").Append(ns).AppendLine();
            sb.AppendLine("{");
        }

        var indent = ns is null ? string.Empty : "    ";
        sb.Append(indent).Append("partial class ").Append(symbol.Name).AppendLine();
        sb.Append(indent).AppendLine("{");
        sb.Append(indent).Append("    protected override void Configure(ValidationDescriptor<")
          .Append(targetType.ToDisplayString()).AppendLine("> descriptor)");
        sb.Append(indent).AppendLine("    {");
        foreach (var (prop, method) in mappings)
        {
            sb.Append(indent).Append("        descriptor.For(r => r.").Append(prop)
              .Append(", ").Append(method).Append("(), nameof(")
              .Append(targetType.ToDisplayString()).Append('.').Append(prop).AppendLine("));");
        }
        sb.Append(indent).AppendLine("    }");
        sb.Append(indent).AppendLine("}");
        if (ns is not null)
            sb.AppendLine("}");

        context.AddSource(symbol.Name + ".ValidatorFor.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }
}
