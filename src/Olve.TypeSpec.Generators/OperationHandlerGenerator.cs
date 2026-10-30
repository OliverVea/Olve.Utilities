using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Olve.TypeSpec.Generators;

/// <summary>
///     Generates <c>services.AddOperationHandlers()</c>: one transient registration per <c>[OperationHandler]</c>
///     class, under each generated per-operation handler interface (an interface deriving directly from
///     <c>Olve.TypeSpec.IOperationHandler&lt;,&gt;</c>) it implements. Every operation interface declared in the
///     assembly needs exactly one handler. Emits nothing when the assembly declares no operations and marks no
///     handlers, so assemblies that see another's internals (tests) don't get a second, ambiguous copy.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class OperationHandlerGenerator : IIncrementalGenerator
{
    private const string OperationHandlerInterface = "Olve.TypeSpec.IOperationHandler`2";
    private const string OperationHandlerAttribute = "Olve.TypeSpec.OperationHandlerAttribute";

    private static readonly DiagnosticDescriptor DuplicateHandlerRule = Rule(
        "OTSP001",
        "More than one handler implements an operation",
        "'{0}' and '{1}' both handle '{2}'; an operation has one handler");

    private static readonly DiagnosticDescriptor InaccessibleHandlerRule = Rule(
        "OTSP002",
        "A handler is not accessible to AddOperationHandlers",
        "Handler '{0}' is private or protected, so AddOperationHandlers can't register it; make it internal or public");

    private static readonly DiagnosticDescriptor NoOperationRule = Rule(
        "OTSP003",
        "A handler implements no operation",
        "'{0}' is marked [OperationHandler] but implements no generated operation handler interface");

    private static readonly DiagnosticDescriptor NotInstantiableRule = Rule(
        "OTSP004",
        "A handler can't be instantiated",
        "Handler '{0}' is abstract, static or generic, so AddOperationHandlers can't register it");

    private static readonly DiagnosticDescriptor MissingHandlerRule = Rule(
        "OTSP005",
        "An operation has no handler",
        "No [OperationHandler] class implements '{0}'");

    private static DiagnosticDescriptor Rule(string id, string title, string message) =>
        new(id, title, message, "Usage", DiagnosticSeverity.Error, isEnabledByDefault: true);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var handlers = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                OperationHandlerAttribute,
                static (node, _) => node is ClassDeclarationSyntax or RecordDeclarationSyntax,
                static (ctx, _) => DescribeHandler(ctx))
            .Collect();

        var operations = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is InterfaceDeclarationSyntax { BaseList: not null },
                static (ctx, ct) => DescribeOperation(ctx, ct))
            .Where(static operation => operation is not null)
            .Collect();

        context.RegisterSourceOutput(handlers.Combine(operations), static (spc, found) => Emit(spc, found.Left, found.Right!));
    }

    private static HandlerInfo DescribeHandler(GeneratorAttributeSyntaxContext ctx)
    {
        var type = (INamedTypeSymbol)ctx.TargetSymbol;
        var operations = type.AllInterfaces
            .Where(IsOperationInterface)
            .Select(i => i.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
            .OrderBy(name => name, System.StringComparer.Ordinal);

        return new HandlerInfo(
            type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            type.ToDisplayString(),
            string.Join(";", operations),
            IsAccessible(type),
            !(type.IsAbstract || type.IsStatic || type.IsGenericType),
            SourceLocation.Of(((TypeDeclarationSyntax)ctx.TargetNode).Identifier.GetLocation()));
    }

    private static OperationInfo? DescribeOperation(GeneratorSyntaxContext ctx, CancellationToken ct)
    {
        if (ctx.SemanticModel.GetDeclaredSymbol(ctx.Node, ct) is not INamedTypeSymbol type || !IsOperationInterface(type))
        {
            return null;
        }

        return new OperationInfo(
            type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            SourceLocation.Of(((TypeDeclarationSyntax)ctx.Node).Identifier.GetLocation()));
    }

    private static bool IsOperationInterface(INamedTypeSymbol candidate) =>
        candidate.Interfaces.Any(i => i.IsGenericType && FullMetadataName(i.OriginalDefinition) == OperationHandlerInterface);

    private static string FullMetadataName(INamedTypeSymbol type) =>
        type.ContainingNamespace is { IsGlobalNamespace: false } ns
            ? ns.ToDisplayString() + "." + type.MetadataName
            : type.MetadataName;

    private static bool IsAccessible(INamedTypeSymbol type)
    {
        for (var current = type; current is not null; current = current.ContainingType)
        {
            if (current.DeclaredAccessibility is not (Accessibility.Public or Accessibility.Internal or Accessibility.ProtectedOrInternal))
            {
                return false;
            }
        }

        return true;
    }

    private static void Emit(SourceProductionContext spc, ImmutableArray<HandlerInfo> found, ImmutableArray<OperationInfo> operations)
    {
        // A partial class is found once per marked declaration.
        var handlers = found
            .GroupBy(h => h.Implementation)
            .Select(g => g.First())
            .OrderBy(h => h.Implementation, System.StringComparer.Ordinal)
            .ToList();

        var registrations = new SortedDictionary<string, HandlerInfo>(System.StringComparer.Ordinal);
        foreach (var handler in handlers)
        {
            if (!handler.Accessible)
            {
                spc.ReportDiagnostic(Diagnostic.Create(InaccessibleHandlerRule, handler.Location.ToLocation(), handler.DisplayName));
                continue;
            }

            if (!handler.Instantiable)
            {
                spc.ReportDiagnostic(Diagnostic.Create(NotInstantiableRule, handler.Location.ToLocation(), handler.DisplayName));
                continue;
            }

            if (handler.Operations.Length == 0)
            {
                spc.ReportDiagnostic(Diagnostic.Create(NoOperationRule, handler.Location.ToLocation(), handler.DisplayName));
                continue;
            }

            foreach (var operation in handler.Operations.Split(';'))
            {
                if (registrations.TryGetValue(operation, out var first))
                {
                    spc.ReportDiagnostic(Diagnostic.Create(DuplicateHandlerRule, handler.Location.ToLocation(), first.DisplayName, handler.DisplayName, Display(operation)));
                    continue;
                }

                registrations[operation] = handler;
            }
        }

        foreach (var operation in operations.GroupBy(o => o.Name).Select(g => g.First()))
        {
            if (!registrations.ContainsKey(operation.Name))
            {
                spc.ReportDiagnostic(Diagnostic.Create(MissingHandlerRule, operation.Location.ToLocation(), Display(operation.Name)));
            }
        }

        if (registrations.Count == 0)
        {
            return;
        }

        var source = new StringBuilder();
        source.AppendLine("// <auto-generated/>");
        source.AppendLine("#nullable enable");
        source.AppendLine();
        source.AppendLine("namespace Microsoft.Extensions.DependencyInjection");
        source.AppendLine("{");
        source.AppendLine("    [global::System.CodeDom.Compiler.GeneratedCode(\"Olve.TypeSpec.Generators\", null)]");
        source.AppendLine("    internal static class OperationHandlerRegistration");
        source.AppendLine("    {");
        source.AppendLine("        /// <summary>Registers every [OperationHandler] class in this assembly, transient, under its operation's handler interface.</summary>");
        source.AppendLine("        public static global::Microsoft.Extensions.DependencyInjection.IServiceCollection AddOperationHandlers(this global::Microsoft.Extensions.DependencyInjection.IServiceCollection services)");
        source.AppendLine("        {");
        foreach (var registration in registrations)
        {
            source.AppendLine($"            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddTransient<{registration.Key}, {registration.Value.Implementation}>(services);");
        }

        source.AppendLine("            return services;");
        source.AppendLine("        }");
        source.AppendLine("    }");
        source.AppendLine("}");
        spc.AddSource("OperationHandlerRegistration.g.cs", SourceText.From(source.ToString(), Encoding.UTF8));
    }

    private static string Display(string fullyQualified) => fullyQualified.Replace("global::", "");

    private sealed record HandlerInfo(
        string Implementation,
        string DisplayName,
        string Operations,
        bool Accessible,
        bool Instantiable,
        SourceLocation Location);

    private sealed record OperationInfo(string Name, SourceLocation Location);

    /// <summary>A location without the syntax tree, so the incremental pipeline can cache by value.</summary>
    private sealed record SourceLocation(string FilePath, TextSpan Span, LinePositionSpan LineSpan)
    {
        public static SourceLocation Of(Location location) =>
            new(location.SourceTree?.FilePath ?? "", location.SourceSpan, location.GetLineSpan().Span);

        public Location ToLocation() => FilePath.Length == 0 ? Location.None : Location.Create(FilePath, Span, LineSpan);
    }
}
