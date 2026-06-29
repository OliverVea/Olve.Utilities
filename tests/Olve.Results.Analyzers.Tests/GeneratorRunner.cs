using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Olve.Results.Analyzers;

namespace Olve.Results.Analyzers.Tests;

/// <summary>
/// Drives <see cref="GenerateResultGenerator"/> over a C# snippet in memory and returns the diagnostics
/// it reports. Snippets are hermetic: they declare their own <c>Olve.Results</c> attribute and problem
/// types (the generator resolves everything by metadata-name string), so the tests do not depend on the
/// Olve.Results runtime assembly. Compilation errors in the snippet (e.g. the CS8795 left when a factory
/// body is intentionally not generated) are irrelevant — only the generator's own diagnostics are returned.
/// </summary>
internal static class GeneratorRunner
{
    // The surface the generator looks up by metadata name. Kept minimal: just enough to mark cases and
    // carry problem payloads.
    private const string Prelude = """
        using System;

        namespace Olve.Results
        {
            [AttributeUsage(AttributeTargets.Struct)]
            public sealed class GenerateResultAttribute : Attribute { }

            [AttributeUsage(AttributeTargets.Method)]
            public sealed class SuccessCaseAttribute : Attribute { }

            [AttributeUsage(AttributeTargets.Method)]
            public sealed class ErrorCaseAttribute : Attribute { }

            [AttributeUsage(AttributeTargets.Method)]
            public sealed class GreyCaseAttribute : Attribute { }

            [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
            public sealed class MustBeUsedWhenReturnedAttribute : Attribute { }

            public class ResultProblem { }

            public sealed class ResultProblemCollection
            {
                public ResultProblemCollection(params ResultProblem[] problems) { }
            }
        }

        """;

    private static readonly ImmutableArray<MetadataReference> References = AppContext
        .GetData("TRUSTED_PLATFORM_ASSEMBLIES") is string paths
        ? paths.Split(Path.PathSeparator)
            .Where(p => p.Length > 0)
            .Select(p => (MetadataReference)MetadataReference.CreateFromFile(p))
            .ToImmutableArray()
        : ImmutableArray<MetadataReference>.Empty;

    /// <summary>
    /// Compiles <paramref name="body"/> (appended to the prelude), runs the generator, and returns its
    /// diagnostics.
    /// </summary>
    public static ImmutableArray<Diagnostic> GetGeneratorDiagnostics(string body)
    {
        var source = Prelude + body;

        var compilation = CSharpCompilation.Create(
            assemblyName: "GeneratorSnippet",
            syntaxTrees: [CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Latest))],
            references: References,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var driver = CSharpGeneratorDriver.Create(new GenerateResultGenerator().AsSourceGenerator());
        driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out var diagnostics);
        return diagnostics;
    }
}
