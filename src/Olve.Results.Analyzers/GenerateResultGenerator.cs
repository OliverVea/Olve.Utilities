using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Olve.Results.Analyzers;

/// <summary>
///     Generates multi-state result boilerplate for partial structs marked
///     <c>[Olve.Results.GenerateResult]</c>. See issue #55.
/// </summary>
/// <remarks>
///     Emits a state discriminator, per-case <c>Is…</c> predicates, <c>Succeeded</c> (any success
///     case), <c>Failed</c> (any error case; grey cases are neither), the partial factory bodies, an
///     exhaustive <c>Match</c>, and <c>Problems</c>/<c>TryPickProblems</c>. Cases may carry a single
///     typed payload, surfaced typed through <c>Match</c>. When exactly one error case exists, implicit
///     conversions from <c>ResultProblem</c>/<c>ResultProblemCollection</c> are emitted, and the type is
///     marked <c>[MustBeUsedWhenReturned]</c>. <c>ToString</c>, equality and diagnostics are layered in
///     later slices.
/// </remarks>
[Generator(LanguageNames.CSharp)]
public sealed class GenerateResultGenerator : IIncrementalGenerator
{
    private const string GenerateResultAttribute = "Olve.Results.GenerateResultAttribute";
    private const string SuccessCaseAttribute = "Olve.Results.SuccessCaseAttribute";
    private const string ErrorCaseAttribute = "Olve.Results.ErrorCaseAttribute";
    private const string GreyCaseAttribute = "Olve.Results.GreyCaseAttribute";
    private const string ResultProblemCollectionType = "Olve.Results.ResultProblemCollection";
    private const string ResultProblemType = "Olve.Results.ResultProblem";

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var results = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                GenerateResultAttribute,
                predicate: static (node, _) => node is StructDeclarationSyntax,
                transform: static (ctx, ct) => Transform(ctx, ct))
            .Where(static x => x is not null);

        context.RegisterSourceOutput(results, static (spc, item) =>
        {
            var value = item!.Value;
            spc.AddSource(value.HintName, value.Source);
        });
    }

    private static (string HintName, string Source)? Transform(
        GeneratorAttributeSyntaxContext context,
        CancellationToken cancellationToken)
    {
        if (context.TargetSymbol is not INamedTypeSymbol type)
        {
            return null;
        }

        var cases = new List<CaseModel>();
        foreach (var method in type.GetMembers().OfType<IMethodSymbol>())
        {
            cancellationToken.ThrowIfCancellationRequested();

            var kind = GetCaseKind(method);
            if (kind is null || !method.IsStatic)
            {
                continue;
            }

            string? payloadType = null;
            string? payloadName = null;
            var isValueType = false;
            var errorPayloadKind = ErrorPayloadKind.None;

            if (method.Parameters.Length == 1)
            {
                var parameter = method.Parameters[0];
                payloadType = parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                payloadName = parameter.Name;
                isValueType = parameter.Type.IsValueType;

                if (kind == CaseKind.Error)
                {
                    if (IsResultProblemCollection(parameter.Type))
                    {
                        errorPayloadKind = ErrorPayloadKind.Collection;
                    }
                    else if (IsResultProblemSubtype(parameter.Type))
                    {
                        errorPayloadKind = ErrorPayloadKind.SingleProblem;
                    }
                }
            }
            else if (method.Parameters.Length > 1)
            {
                // Multi-parameter cases are deferred to a later slice (diagnostics will flag them).
                continue;
            }

            cases.Add(new CaseModel(
                method.Name,
                kind.Value,
                AccessibilityKeyword(method.DeclaredAccessibility),
                payloadType,
                payloadName,
                isValueType,
                errorPayloadKind));
        }

        if (cases.Count == 0)
        {
            return null;
        }

        var ns = type.ContainingNamespace.IsGlobalNamespace
            ? null
            : type.ContainingNamespace.ToDisplayString();

        var source = Emit(ns, type.Name, type.IsReadOnly, cases);
        var hintName = (ns is null ? string.Empty : ns + ".") + type.Name + ".GenerateResult.g.cs";
        return (hintName, source);
    }

    private static CaseKind? GetCaseKind(IMethodSymbol method)
    {
        foreach (var attribute in method.GetAttributes())
        {
            switch (attribute.AttributeClass?.ToDisplayString())
            {
                case SuccessCaseAttribute: return CaseKind.Success;
                case ErrorCaseAttribute: return CaseKind.Error;
                case GreyCaseAttribute: return CaseKind.Grey;
            }
        }

        return null;
    }

    private static bool IsResultProblemCollection(ITypeSymbol type) =>
        type.ToDisplayString() == ResultProblemCollectionType;

    private static bool IsResultProblemSubtype(ITypeSymbol type)
    {
        for (ITypeSymbol? current = type; current is not null; current = current.BaseType)
        {
            if (current.ToDisplayString() == ResultProblemType)
            {
                return true;
            }
        }

        return false;
    }

    private static string Emit(string? ns, string typeName, bool isReadOnly, List<CaseModel> cases)
    {
        const string state = "__state";
        const string stateEnum = "__CaseState";

        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();

        var indent = string.Empty;
        if (ns is not null)
        {
            sb.AppendLine($"namespace {ns}");
            sb.AppendLine("{");
            indent = "    ";
        }

        var member = indent + "    ";
        var body = member + "    ";
        var readonlyModifier = isReadOnly ? "readonly " : string.Empty;

        sb.AppendLine($"{indent}[global::Olve.Results.MustBeUsedWhenReturned]");
        sb.AppendLine($"{indent}{readonlyModifier}partial struct {typeName}");
        sb.AppendLine($"{indent}{{");

        // State discriminator.
        sb.AppendLine($"{member}private enum {stateEnum}");
        sb.AppendLine($"{member}{{");
        foreach (var c in cases)
        {
            sb.AppendLine($"{body}{c.Name},");
        }
        sb.AppendLine($"{member}}}");
        sb.AppendLine();

        sb.AppendLine($"{member}private readonly {stateEnum} {state};");
        sb.AppendLine();

        var payloadCases = cases.Where(c => c.HasPayload).ToList();

        // Payload backing fields. Reference-typed payloads are nullable (default when inactive);
        // value-typed payloads store the declared type directly — validity is gated by __state.
        foreach (var c in payloadCases)
        {
            sb.AppendLine($"{member}private readonly {c.FieldType} {c.FieldName};");
        }
        if (payloadCases.Count > 0)
        {
            sb.AppendLine();
        }

        // Constructor.
        if (payloadCases.Count == 0)
        {
            sb.AppendLine($"{member}private {typeName}({stateEnum} state) => {state} = state;");
        }
        else
        {
            sb.AppendLine($"{member}private {typeName}(");
            sb.AppendLine($"{body}{stateEnum} state,");
            for (var i = 0; i < payloadCases.Count; i++)
            {
                var c = payloadCases[i];
                var comma = i == payloadCases.Count - 1 ? ")" : ",";
                sb.AppendLine($"{body}{c.FieldType} {c.FieldName} = default{comma}");
            }
            sb.AppendLine($"{member}{{");
            sb.AppendLine($"{body}{state} = state;");
            foreach (var c in payloadCases)
            {
                sb.AppendLine($"{body}this.{c.FieldName} = {c.FieldName};");
            }
            sb.AppendLine($"{member}}}");
        }
        sb.AppendLine();

        // Factory bodies.
        foreach (var c in cases)
        {
            if (!c.HasPayload)
            {
                sb.AppendLine($"{member}{c.Accessibility} static partial {typeName} {c.Name}() => new {typeName}({stateEnum}.{c.Name});");
            }
            else
            {
                sb.AppendLine($"{member}{c.Accessibility} static partial {typeName} {c.Name}({c.PayloadType} {c.PayloadName}) => new {typeName}({stateEnum}.{c.Name}, {c.FieldName}: {c.PayloadName});");
            }
        }
        sb.AppendLine();

        // Per-case predicates.
        foreach (var c in cases)
        {
            sb.AppendLine($"{member}/// <summary>Whether this result is the <c>{c.Name}</c> state.</summary>");
            sb.AppendLine($"{member}public bool Is{c.Name} => {state} == {stateEnum}.{c.Name};");
        }
        sb.AppendLine();

        // Succeeded / Failed.
        sb.AppendLine($"{member}/// <summary>Whether this result is a success state.</summary>");
        sb.AppendLine($"{member}public bool Succeeded => {StateCondition(cases, CaseKind.Success, state, stateEnum)};");
        sb.AppendLine($"{member}/// <summary>Whether this result is an error state. Grey states are neither succeeded nor failed.</summary>");
        sb.AppendLine($"{member}public bool Failed => {StateCondition(cases, CaseKind.Error, state, stateEnum)};");
        sb.AppendLine();

        // Match.
        sb.AppendLine($"{member}/// <summary>Exhaustively matches over every case.</summary>");
        sb.AppendLine($"{member}public TResult Match<TResult>(");
        for (var i = 0; i < cases.Count; i++)
        {
            var c = cases[i];
            var comma = i == cases.Count - 1 ? ")" : ",";
            var func = c.HasPayload ? $"System.Func<{c.PayloadType}, TResult>" : "System.Func<TResult>";
            sb.AppendLine($"{body}{func} on{c.Name}{comma}");
        }
        sb.AppendLine($"{body}=> {state} switch");
        sb.AppendLine($"{body}{{");
        foreach (var c in cases)
        {
            var arg = c.HasPayload
                ? (c.IsValueType ? c.FieldName : c.FieldName + "!")
                : string.Empty;
            sb.AppendLine($"{body}    {stateEnum}.{c.Name} => on{c.Name}({arg}),");
        }
        sb.AppendLine($"{body}    _ => throw new System.InvalidOperationException(\"Unreachable result state.\"),");
        sb.AppendLine($"{body}}};");
        sb.AppendLine();

        // Problems / TryPickProblems.
        sb.AppendLine($"{member}/// <summary>Gets the problems for an error state, or <see langword=\"null\"/> otherwise.</summary>");
        sb.AppendLine($"{member}public global::Olve.Results.ResultProblemCollection? Problems => {state} switch");
        sb.AppendLine($"{member}{{");
        foreach (var c in cases.Where(c => c.Kind == CaseKind.Error && c.ErrorPayloadKind != ErrorPayloadKind.None))
        {
            var expr = c.ErrorPayloadKind == ErrorPayloadKind.Collection
                ? c.FieldName
                : $"new global::Olve.Results.ResultProblemCollection({c.FieldName}!)";
            sb.AppendLine($"{body}{stateEnum}.{c.Name} => {expr},");
        }
        sb.AppendLine($"{body}_ => null,");
        sb.AppendLine($"{member}}};");
        sb.AppendLine();

        sb.AppendLine($"{member}/// <summary>Attempts to retrieve the problems for an error state.</summary>");
        sb.AppendLine($"{member}public bool TryPickProblems([global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Olve.Results.ResultProblemCollection? problems)");
        sb.AppendLine($"{member}{{");
        sb.AppendLine($"{body}problems = Problems;");
        sb.AppendLine($"{body}return problems is not null;");
        sb.AppendLine($"{member}}}");

        // Implicit conversions from problems — only when exactly one error case exists (otherwise the
        // conversion target would be ambiguous). Skipped silently for zero or multiple error cases.
        var errorCases = cases.Where(c => c.Kind == CaseKind.Error).ToList();
        if (errorCases.Count == 1)
        {
            var e = errorCases[0];
            if (e.ErrorPayloadKind == ErrorPayloadKind.Collection)
            {
                sb.AppendLine();
                sb.AppendLine($"{member}/// <summary>Converts a problem collection into a <c>{e.Name}</c> result.</summary>");
                sb.AppendLine($"{member}public static implicit operator {typeName}(global::Olve.Results.ResultProblemCollection problems) => {typeName}.{e.Name}(problems);");
                sb.AppendLine($"{member}/// <summary>Converts a single problem into a <c>{e.Name}</c> result.</summary>");
                sb.AppendLine($"{member}public static implicit operator {typeName}(global::Olve.Results.ResultProblem problem) => {typeName}.{e.Name}(new global::Olve.Results.ResultProblemCollection(problem));");
            }
            else if (e.ErrorPayloadKind == ErrorPayloadKind.SingleProblem)
            {
                sb.AppendLine();
                sb.AppendLine($"{member}/// <summary>Converts a problem into a <c>{e.Name}</c> result.</summary>");
                sb.AppendLine($"{member}public static implicit operator {typeName}({e.PayloadType} {e.PayloadName}) => {typeName}.{e.Name}({e.PayloadName});");
            }
        }

        sb.AppendLine($"{indent}}}");

        if (ns is not null)
        {
            sb.AppendLine("}");
        }

        return sb.ToString();
    }

    private static string StateCondition(List<CaseModel> cases, CaseKind kind, string state, string stateEnum)
    {
        var matching = cases.Where(c => c.Kind == kind).ToList();
        if (matching.Count == 0)
        {
            return "false";
        }

        return string.Join(" || ", matching.Select(c => $"{state} == {stateEnum}.{c.Name}"));
    }

    private static string AccessibilityKeyword(Accessibility accessibility) => accessibility switch
    {
        Accessibility.Public => "public",
        Accessibility.Internal => "internal",
        Accessibility.Private => "private",
        Accessibility.Protected => "protected",
        Accessibility.ProtectedOrInternal => "protected internal",
        Accessibility.ProtectedAndInternal => "private protected",
        _ => "public",
    };

    private enum CaseKind
    {
        Success,
        Error,
        Grey,
    }

    private enum ErrorPayloadKind
    {
        None,
        Collection,
        SingleProblem,
    }

    private sealed class CaseModel(
        string name,
        CaseKind kind,
        string accessibility,
        string? payloadType,
        string? payloadName,
        bool isValueType,
        ErrorPayloadKind errorPayloadKind)
    {
        public string Name { get; } = name;
        public CaseKind Kind { get; } = kind;
        public string Accessibility { get; } = accessibility;
        public string? PayloadType { get; } = payloadType;
        public string? PayloadName { get; } = payloadName;
        public bool IsValueType { get; } = isValueType;
        public ErrorPayloadKind ErrorPayloadKind { get; } = errorPayloadKind;

        public bool HasPayload => PayloadType is not null;
        public string FieldName => "__payload_" + Name;
        public string FieldType => IsValueType ? PayloadType! : PayloadType + "?";
    }
}
