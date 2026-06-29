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
/// <remarks>
///     The <c>await</c> case only fires when the awaited operand is itself a task-like wrapper
///     (<see cref="System.Threading.Tasks.Task{TResult}"/>, <see cref="System.Threading.Tasks.ValueTask{TResult}"/>,
///     or their <c>Configured*Awaitable</c> forms) around a marked type — i.e. an async method that
///     <em>returns</em> a result. Awaiting any other custom awaitable that merely <em>yields</em> a marked
///     type on completion is an observation, not a discard, and is intentionally ignored. The motivating
///     case is a fluent assertion such as <c>await Assert.That(result).Failed();</c>: TUnit assertion
///     builders surface the asserted value as the await result, so without this gate every result
///     assertion would be a false positive.
/// </remarks>
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

        // The set of task-like wrappers whose bare 'await' counts as discarding a returned result.
        // ConfiguredTaskAwaitable<T> / ConfiguredValueTaskAwaitable<T> cover 'await foo.ConfigureAwait(false);'.
        var taskLike = new[]
            {
                taskOfT,
                valueTaskOfT,
                context.Compilation.GetTypeByMetadataName("System.Runtime.CompilerServices.ConfiguredTaskAwaitable`1"),
                context.Compilation.GetTypeByMetadataName("System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable`1"),
            }
            .Where(t => t is not null)
            .ToImmutableArray();

        context.RegisterOperationAction(
            ctx => Analyze(ctx, marker, taskOfT, valueTaskOfT, taskLike!),
            OperationKind.ExpressionStatement);
    }

    private static void Analyze(
        OperationAnalysisContext context,
        INamedTypeSymbol marker,
        INamedTypeSymbol? taskOfT,
        INamedTypeSymbol? valueTaskOfT,
        ImmutableArray<INamedTypeSymbol> taskLike)
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

            // Only flag 'await' when the awaited operand is a task-like wrapper around a marked type
            // (an async method that returns a result). Awaiting any other awaitable that merely yields a
            // marked type — e.g. a TUnit assertion builder — is an observation, not a discard.
            case IAwaitOperation { Type: { } awaitedType } await
                when IsMarked(awaitedType, marker) && IsTaskLike(await.Operation.Type, taskLike):
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

    private static bool IsTaskLike(ITypeSymbol? operandType, ImmutableArray<INamedTypeSymbol> taskLike)
    {
        if (operandType is not INamedTypeSymbol { IsGenericType: true } named)
        {
            return false;
        }

        var definition = named.OriginalDefinition;
        return taskLike.Any(t => SymbolEqualityComparer.Default.Equals(definition, t));
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
