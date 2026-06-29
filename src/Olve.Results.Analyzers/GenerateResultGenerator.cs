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
///     marked <c>[MustBeUsedWhenReturned]</c>. <c>ToString</c> renders the case name plus any payload, and
///     value equality (<c>IEquatable&lt;T&gt;</c>, <c>==</c>/<c>!=</c>) is emitted. A <c>MapToResult()</c>
///     collapses onto the binary success/failure <c>Result</c>, taking an optional <c>allow{Case}</c> flag
///     per grey case to choose whether that grey state maps to success or a problem.
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

    /// <summary>
    ///     A case factory may declare at most one payload parameter. Multi-parameter factories are
    ///     skipped (no body is generated), which on its own surfaces only a cryptic CS8795; this rule
    ///     explains why.
    /// </summary>
    private static readonly DiagnosticDescriptor MultipleParametersRule = new(
        id: "ORES002",
        title: "Result case factory may declare at most one parameter",
        messageFormat: "Result case factory '{0}' declares {1} parameters; a [GenerateResult] case factory may declare at most one payload parameter",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "A [GenerateResult] case factory carries at most one payload parameter, surfaced typed through Match. " +
                     "Declare a single payload parameter, or wrap multiple values in one type.");

    /// <summary>
    ///     A <c>[GenerateResult]</c> type with no case factories generates nothing, which is almost always
    ///     an oversight (a forgotten case attribute, or a misspelled one). Flag the empty type so the cause
    ///     is visible rather than silent.
    /// </summary>
    private static readonly DiagnosticDescriptor NoCasesRule = new(
        id: "ORES003",
        title: "[GenerateResult] type declares no case factories",
        messageFormat: "[GenerateResult] type '{0}' declares no case factories; mark at least one static partial factory with [SuccessCase], [ErrorCase], or [GreyCase]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "A [GenerateResult] type with no case factories generates no result members. " +
                     "Declare at least one static partial factory marked [SuccessCase], [ErrorCase], or [GreyCase].");

    /// <summary>
    ///     A case factory's body is emitted as a partial method implementation, so the declaration must be
    ///     <c>static partial</c>. A factory that is not (e.g. missing <c>partial</c>, or an instance method)
    ///     is silently ignored; this rule explains why nothing was generated for it.
    /// </summary>
    private static readonly DiagnosticDescriptor NotStaticPartialRule = new(
        id: "ORES004",
        title: "Result case factory must be declared 'static partial'",
        messageFormat: "Result case factory '{0}' must be declared 'static partial'; it is otherwise ignored and no body is generated",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "A [GenerateResult] case factory's body is generated as a partial method implementation, " +
                     "so the declaration must be 'static partial'.");

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
            var value = item!;
            foreach (var diagnostic in value.Diagnostics)
            {
                spc.ReportDiagnostic(diagnostic.ToDiagnostic());
            }

            if (value.Source is not null)
            {
                spc.AddSource(value.HintName!, value.Source);
            }
        });
    }

    private static GenerationResult? Transform(
        GeneratorAttributeSyntaxContext context,
        CancellationToken cancellationToken)
    {
        if (context.TargetSymbol is not INamedTypeSymbol type)
        {
            return null;
        }

        var cases = new List<CaseModel>();
        var diagnostics = new List<DiagnosticInfo>();
        var sawCaseFactory = false;
        foreach (var method in type.GetMembers().OfType<IMethodSymbol>())
        {
            cancellationToken.ThrowIfCancellationRequested();

            var kind = GetCaseKind(method);
            if (kind is null)
            {
                continue;
            }

            sawCaseFactory = true;

            if (!method.IsStatic || !method.IsPartialDefinition)
            {
                // The body is emitted as a partial implementation, so the factory must be static partial.
                // Anything else is ignored (no body generated); flag why.
                diagnostics.Add(new DiagnosticInfo(
                    NotStaticPartialRule,
                    LocationInfo.CreateFrom(method.Locations.FirstOrDefault()),
                    new EquatableArray<string>(new[] { method.Name })));
                continue;
            }

            if (method.Parameters.Length > 1)
            {
                // A case factory carries at most one payload. Skip it (no body emitted) and flag why.
                diagnostics.Add(new DiagnosticInfo(
                    MultipleParametersRule,
                    LocationInfo.CreateFrom(method.Locations.FirstOrDefault()),
                    new EquatableArray<string>(new[]
                    {
                        method.Name,
                        method.Parameters.Length.ToString(),
                    })));
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

            cases.Add(new CaseModel(
                method.Name,
                kind.Value,
                AccessibilityKeyword(method.DeclaredAccessibility),
                payloadType,
                payloadName,
                isValueType,
                errorPayloadKind));
        }

        if (!sawCaseFactory)
        {
            // Marked [GenerateResult] but no method carries a case attribute — nothing to generate.
            diagnostics.Add(new DiagnosticInfo(
                NoCasesRule,
                LocationInfo.CreateFrom(type.Locations.FirstOrDefault()),
                new EquatableArray<string>(new[] { type.Name })));
        }

        if (cases.Count == 0 && diagnostics.Count == 0)
        {
            return null;
        }

        var diagnosticArray = new EquatableArray<DiagnosticInfo>(diagnostics.ToArray());
        if (cases.Count == 0)
        {
            return new GenerationResult(null, null, diagnosticArray);
        }

        var ns = type.ContainingNamespace.IsGlobalNamespace
            ? null
            : type.ContainingNamespace.ToDisplayString();

        var source = Emit(ns, type.Name, type.IsReadOnly, cases);
        var hintName = (ns is null ? string.Empty : ns + ".") + type.Name + ".GenerateResult.g.cs";
        return new GenerationResult(hintName, source, diagnosticArray);
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
        sb.AppendLine($"{indent}{readonlyModifier}partial struct {typeName} : global::System.IEquatable<{typeName}>");
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

        // ToString — the case name, with the payload appended when present.
        sb.AppendLine($"{member}/// <summary>Returns the case name, with the payload appended when present.</summary>");
        sb.AppendLine($"{member}public override string ToString() => {state} switch");
        sb.AppendLine($"{member}{{");
        foreach (var c in cases)
        {
            if (!c.HasPayload)
            {
                sb.AppendLine($"{body}    {stateEnum}.{c.Name} => \"{c.Name}\",");
            }
            else
            {
                var payload = c.IsValueType ? c.FieldName : c.FieldName + "!";
                sb.AppendLine($"{body}    {stateEnum}.{c.Name} => $\"{c.Name}({{{payload}}})\",");
            }
        }
        sb.AppendLine($"{body}    _ => throw new System.InvalidOperationException(\"Unreachable result state.\"),");
        sb.AppendLine($"{body}}};");
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
        sb.AppendLine();

        // Value equality — same state, and equal payloads on the active case. Inactive payload fields
        // are default for both operands, so comparing every payload field is correct (and cheap).
        sb.AppendLine($"{member}/// <summary>Determines whether this result equals another of the same type.</summary>");
        sb.AppendLine($"{member}public bool Equals({typeName} other)");
        sb.AppendLine($"{member}{{");
        sb.AppendLine($"{body}if ({state} != other.{state})");
        sb.AppendLine($"{body}{{");
        sb.AppendLine($"{body}    return false;");
        sb.AppendLine($"{body}}}");
        sb.AppendLine();
        if (payloadCases.Count == 0)
        {
            sb.AppendLine($"{body}return true;");
        }
        else
        {
            var checks = payloadCases
                .Select(c => $"global::System.Collections.Generic.EqualityComparer<{c.FieldType}>.Default.Equals(this.{c.FieldName}, other.{c.FieldName})")
                .ToList();
            sb.AppendLine($"{body}return {string.Join(" && ", checks)};");
        }
        sb.AppendLine($"{member}}}");
        sb.AppendLine();

        sb.AppendLine($"{member}/// <summary>Determines whether this result equals another object.</summary>");
        sb.AppendLine($"{member}public override bool Equals(object? obj) => obj is {typeName} other && Equals(other);");
        sb.AppendLine();

        sb.AppendLine($"{member}/// <summary>Returns a hash code incorporating the state and active payload.</summary>");
        sb.AppendLine($"{member}public override int GetHashCode()");
        sb.AppendLine($"{member}{{");
        sb.AppendLine($"{body}var __hash = new global::System.HashCode();");
        sb.AppendLine($"{body}__hash.Add({state});");
        foreach (var c in payloadCases)
        {
            sb.AppendLine($"{body}__hash.Add(this.{c.FieldName});");
        }
        sb.AppendLine($"{body}return __hash.ToHashCode();");
        sb.AppendLine($"{member}}}");
        sb.AppendLine();

        sb.AppendLine($"{member}/// <summary>Determines whether two results are equal.</summary>");
        sb.AppendLine($"{member}public static bool operator ==({typeName} left, {typeName} right) => left.Equals(right);");
        sb.AppendLine($"{member}/// <summary>Determines whether two results are unequal.</summary>");
        sb.AppendLine($"{member}public static bool operator !=({typeName} left, {typeName} right) => !left.Equals(right);");

        // MapToResult — collapses onto the binary success/failure Result. A grey state maps onto neither,
        // so each grey case gets an optional 'allow{Case}' flag (default true = treat as success); pass
        // false to map that grey state to a problem instead.
        var greyCases = cases.Where(c => c.Kind == CaseKind.Grey).ToList();
        sb.AppendLine();
        if (greyCases.Count == 0)
        {
            sb.AppendLine($"{member}/// <summary>Converts this result to a <c>Result</c>: success when succeeded, otherwise its problems.</summary>");
            sb.AppendLine($"{member}public global::Olve.Results.Result MapToResult()");
        }
        else
        {
            sb.AppendLine($"{member}/// <summary>Converts this result to a <c>Result</c>: success when succeeded, problems when failed, and each grey state per its flag.</summary>");
            foreach (var g in greyCases)
            {
                sb.AppendLine($"{member}/// <param name=\"allow{g.Name}\">When <see langword=\"true\"/> (default), the <c>{g.Name}</c> state maps to success; otherwise it maps to a problem.</param>");
            }
            sb.AppendLine($"{member}public global::Olve.Results.Result MapToResult(");
            for (var i = 0; i < greyCases.Count; i++)
            {
                var comma = i == greyCases.Count - 1 ? ")" : ",";
                sb.AppendLine($"{body}bool allow{greyCases[i].Name} = true{comma}");
            }
        }
        sb.AppendLine($"{member}{{");
        sb.AppendLine($"{body}if (Succeeded)");
        sb.AppendLine($"{body}{{");
        sb.AppendLine($"{body}    return global::Olve.Results.Result.Success();");
        sb.AppendLine($"{body}}}");
        foreach (var g in greyCases)
        {
            sb.AppendLine();
            sb.AppendLine($"{body}if (Is{g.Name})");
            sb.AppendLine($"{body}{{");
            sb.AppendLine($"{body}    return allow{g.Name} ? global::Olve.Results.Result.Success() : new global::Olve.Results.ResultProblem(\"{typeName} was {g.Name}.\");");
            sb.AppendLine($"{body}}}");
        }
        sb.AppendLine();
        sb.AppendLine($"{body}if (Problems is {{ }} __problems)");
        sb.AppendLine($"{body}{{");
        sb.AppendLine($"{body}    return __problems;");
        sb.AppendLine($"{body}}}");
        sb.AppendLine();
        sb.AppendLine($"{body}return new global::Olve.Results.ResultProblem(\"{typeName} was in an error state without problem details.\");");
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

    /// <summary>
    ///     The pipeline output for one <c>[GenerateResult]</c> type: the source to add (when any cases
    ///     were found) plus any diagnostics to report. Value-equatable so it caches incrementally.
    /// </summary>
    private sealed record GenerationResult(
        string? HintName,
        string? Source,
        EquatableArray<DiagnosticInfo> Diagnostics);

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
