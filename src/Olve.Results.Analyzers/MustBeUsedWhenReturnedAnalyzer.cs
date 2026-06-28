using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Olve.Results.Analyzers;

/// <summary>
///     Reports invocations whose result is a type marked with
///     <c>Olve.Results.MustBeUsedWhenReturnedAttribute</c> but is not observed (the call is used as a
///     statement). Task/ValueTask wrappers are unwrapped, so both <c>result;</c> and
///     <c>await resultAsync;</c> are covered. Discards (<c>_ = ...</c>) and methods that consume the
///     value (e.g. <c>DiscardResult()</c>) are not reported.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MustBeUsedWhenReturnedAnalyzer : DiagnosticAnalyzer
{
    /// <summary>The diagnostic id reported by this analyzer.</summary>
    public const string DiagnosticId = "ORES001";

    private const string MarkerAttributeMetadataName = "Olve.Results.MustBeUsedWhenReturnedAttribute";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: "Return value must be observed",
        messageFormat: "The result of type '{0}' is marked [MustBeUsedWhenReturned] and must be observed",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Values of types marked with [MustBeUsedWhenReturned] must be observed. " +
                     "Use a discard ('_ = expression;') or 'DiscardResult()' to ignore one intentionally.");

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Rule);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(OnCompilationStart);
    }

    private static void OnCompilationStart(CompilationStartAnalysisContext context)
    {
        var marker = context.Compilation.GetTypeByMetadataName(MarkerAttributeMetadataName);
        if (marker is null)
        {
            // The marker attribute (and therefore Olve.Results) is not referenced. Nothing to enforce.
            return;
        }

        var taskOfT = context.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1");
        var valueTaskOfT = context.Compilation.GetTypeByMetadataName("System.Threading.Tasks.ValueTask`1");

        context.RegisterOperationAction(
            ctx => Analyze(ctx, marker, taskOfT, valueTaskOfT),
            OperationKind.ExpressionStatement);
    }

    private static void Analyze(
        OperationAnalysisContext context,
        INamedTypeSymbol marker,
        INamedTypeSymbol? taskOfT,
        INamedTypeSymbol? valueTaskOfT)
    {
        var statement = (IExpressionStatementOperation)context.Operation;

        // Only a bare statement discards the value. 'var x = Foo();' and '_ = Foo();' wrap the
        // invocation in a declaration/assignment, so they are not ExpressionStatement -> Invocation.
        switch (statement.Operation)
        {
            case IInvocationOperation invocation:
                var returnType = Unwrap(invocation.TargetMethod.ReturnType, taskOfT, valueTaskOfT);
                if (IsMarked(returnType, marker))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, invocation.Syntax.GetLocation(), returnType.Name));
                }

                break;

            case IAwaitOperation { Type: { } awaitedType } await when IsMarked(awaitedType, marker):
                context.ReportDiagnostic(Diagnostic.Create(Rule, await.Syntax.GetLocation(), awaitedType.Name));
                break;
        }
    }

    private static ITypeSymbol Unwrap(ITypeSymbol type, INamedTypeSymbol? taskOfT, INamedTypeSymbol? valueTaskOfT)
    {
        if (type is INamedTypeSymbol { IsGenericType: true } named)
        {
            var definition = named.OriginalDefinition;
            if ((taskOfT is not null && SymbolEqualityComparer.Default.Equals(definition, taskOfT)) ||
                (valueTaskOfT is not null && SymbolEqualityComparer.Default.Equals(definition, valueTaskOfT)))
            {
                return named.TypeArguments[0];
            }
        }

        return type;
    }

    private static bool IsMarked(ITypeSymbol? type, INamedTypeSymbol marker)
    {
        for (var current = type; current is not null; current = current.BaseType)
        {
            if (HasMarker(current, marker) || HasMarker(current.OriginalDefinition, marker))
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasMarker(ISymbol symbol, INamedTypeSymbol marker) =>
        symbol.GetAttributes().Any(attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, marker));
}
