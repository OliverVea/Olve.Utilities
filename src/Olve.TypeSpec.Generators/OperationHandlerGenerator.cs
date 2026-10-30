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
///     Generates <c>services.AddOperationHandlers()</c>: one transient registration per class in the
///     assembly that implements a generated per-operation handler interface (an interface deriving
///     directly from <c>Olve.TypeSpec.IOperationHandler&lt;,&gt;</c>). Emits nothing when the assembly has
///     no handlers, so assemblies that see another's internals (tests) don't get a second, ambiguous copy.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class OperationHandlerGenerator : IIncrementalGenerator
{
    private const string OperationHandlerInterface = "Olve.TypeSpec.IOperationHandler`2";

    private static readonly DiagnosticDescriptor DuplicateHandlerRule = new(
        id: "OTSP001",
        title: "More than one handler implements an operation",
        messageFormat: "'{0}' and '{1}' both implement '{2}'; an operation has one handler",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor InaccessibleHandlerRule = new(
        id: "OTSP002",
        title: "A handler is not accessible to AddOperationHandlers",
        messageFormat: "Handler '{0}' is private or protected, so AddOperationHandlers can't register it; make it internal or public",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var handlers = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax { BaseList: not null },
                static (ctx, ct) => Describe(ctx, ct))
            .Where(static handler => handler is not null)
            .Collect();

        context.RegisterSourceOutput(handlers, static (spc, found) => Emit(spc, found!));
    }

    private static HandlerInfo? Describe(GeneratorSyntaxContext ctx, CancellationToken ct)
    {
        if (ctx.SemanticModel.GetDeclaredSymbol(ctx.Node, ct) is not INamedTypeSymbol type
            || type.IsAbstract || type.IsStatic || type.IsGenericType)
        {
            return null;
        }

        var operations = type.AllInterfaces
            .Where(IsOperationInterface)
            .Select(i => i.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
            .OrderBy(name => name, System.StringComparer.Ordinal)
            .ToArray();
        if (operations.Length == 0)
        {
            return null;
        }

        var location = ctx.Node is ClassDeclarationSyntax declaration ? declaration.Identifier.GetLocation() : ctx.Node.GetLocation();
        return new HandlerInfo(
            type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            type.ToDisplayString(),
            string.Join(";", operations),
            IsAccessible(type),
            location.SourceTree?.FilePath ?? "",
            location.SourceSpan,
            location.GetLineSpan().Span);
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

    private static void Emit(SourceProductionContext spc, ImmutableArray<HandlerInfo> found)
    {
        // A partial class is found once per declaration.
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
                spc.ReportDiagnostic(Diagnostic.Create(InaccessibleHandlerRule, handler.ToLocation(), handler.DisplayName));
                continue;
            }

            foreach (var operation in handler.Operations.Split(';'))
            {
                if (registrations.TryGetValue(operation, out var first))
                {
                    spc.ReportDiagnostic(Diagnostic.Create(DuplicateHandlerRule, handler.ToLocation(), first.DisplayName, handler.DisplayName, operation.Replace("global::", "")));
                    continue;
                }

                registrations[operation] = handler;
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
        source.AppendLine("        /// <summary>Registers every operation handler in this assembly, transient, under its operation's handler interface.</summary>");
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

    private sealed record HandlerInfo(
        string Implementation,
        string DisplayName,
        string Operations,
        bool Accessible,
        string FilePath,
        TextSpan Span,
        LinePositionSpan LineSpan)
    {
        public Location ToLocation() => FilePath.Length == 0 ? Location.None : Location.Create(FilePath, Span, LineSpan);
    }
}
