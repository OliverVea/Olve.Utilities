using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Olve.Results.Analyzers;

namespace Olve.Results.Analyzers.Tests;

/// <summary>
/// Compiles a C# snippet in memory and runs <see cref="MustBeUsedWhenReturnedAnalyzer"/> against it,
/// returning only the ORES001 diagnostics. Snippets are hermetic: they declare their own
/// <c>Olve.Results.MustBeUsedWhenReturnedAttribute</c> + <c>Result</c> types, so the tests do not depend
/// on the Olve.Results runtime assembly and stay focused on the analyzer's flow logic.
/// </summary>
internal static class AnalyzerRunner
{
    // Shared scaffolding every snippet builds on: the marker, the marked result types, a non-task-like
    // custom awaitable that yields a Result on completion (models a TUnit fluent assertion builder), and a
    // small API surface returning marked / unmarked / task-wrapped values.
    private const string Prelude = """
        using System;
        using System.Threading.Tasks;
        using System.Runtime.CompilerServices;

        namespace Olve.Results
        {
            [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
            public sealed class MustBeUsedWhenReturnedAttribute : Attribute { }

            [MustBeUsedWhenReturned] public readonly struct Result { }
            [MustBeUsedWhenReturned] public readonly struct Result<T> { }
        }

        namespace Sample
        {
            using Olve.Results;

            // Awaitable that yields a Result on completion but is NOT a Task/ValueTask wrapper.
            // This is the shape of a fluent assertion (e.g. `await Assert.That(result).Failed()`):
            // the Result is the asserted input echoed back, not a returned value to observe.
            public readonly struct AssertionResult
            {
                public Awaiter GetAwaiter() => default;

                public readonly struct Awaiter : INotifyCompletion
                {
                    public bool IsCompleted => true;
                    public Result GetResult() => default;
                    public void OnCompleted(Action continuation) { }
                }
            }

            public static class Api
            {
                public static Result Sync() => default;
                public static Result<int> SyncGeneric() => default;
                public static int Plain() => 0;
                public static Task<Result> AsyncTask() => Task.FromResult(default(Result));
                public static ValueTask<Result> AsyncValueTask() => new(default(Result));
                public static Task PlainTask() => Task.CompletedTask;
                public static AssertionResult Assertion() => default;
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
    /// Wraps <paramref name="body"/> in an async method, compiles it, and returns the ORES001 diagnostics.
    /// </summary>
    public static async Task<ImmutableArray<Diagnostic>> GetOres001Async(string body)
    {
        var source = Prelude + $$"""

            namespace Sample
            {
                using System.Threading.Tasks;
                using Olve.Results;

                public class Subject
                {
                    public async Task Run()
                    {
                        {{body}}
                        await Task.CompletedTask;
                    }
                }
            }
            """;

        var compilation = CSharpCompilation.Create(
            assemblyName: "AnalyzerSnippet",
            syntaxTrees: [CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Latest))],
            references: References,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        // Fail loudly if a snippet itself doesn't compile — otherwise a typo silently yields zero
        // diagnostics and the test would pass for the wrong reason.
        var compileErrors = compilation.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ToImmutableArray();
        if (compileErrors.Length > 0)
        {
            throw new InvalidOperationException(
                "Test snippet failed to compile:\n" + string.Join("\n", compileErrors));
        }

        var withAnalyzers = compilation.WithAnalyzers(
            ImmutableArray.Create<DiagnosticAnalyzer>(new MustBeUsedWhenReturnedAnalyzer()));

        var diagnostics = await withAnalyzers.GetAnalyzerDiagnosticsAsync();
        return diagnostics
            .Where(d => d.Id == MustBeUsedWhenReturnedAnalyzer.DiagnosticId)
            .ToImmutableArray();
    }
}
