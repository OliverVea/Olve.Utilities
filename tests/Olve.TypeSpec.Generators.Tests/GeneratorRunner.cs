using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Olve.TypeSpec.Generators.Tests;

/// <summary>
/// Runs <see cref="OperationHandlerGenerator"/> over a C# snippet in memory. Snippets are hermetic: the
/// prelude declares the handler interface and a minimal <c>IServiceCollection</c>, so the generated
/// source compiles without the runtime or DI assemblies.
/// </summary>
internal static class GeneratorRunner
{
    private const string Prelude = """
        using System.Threading;
        using System.Threading.Tasks;

        namespace Olve.TypeSpec
        {
            public interface IOperationResponse { }
            public interface IOperationHandler<in TRequest, TResponse> where TResponse : IOperationResponse
            {
                Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
            }
        }

        namespace Microsoft.Extensions.DependencyInjection
        {
            public interface IServiceCollection { }
            public static class ServiceCollectionServiceExtensions
            {
                public static IServiceCollection AddTransient<TService, TImplementation>(IServiceCollection services)
                    where TService : class where TImplementation : class, TService => services;
            }
        }

        namespace Api
        {
            public sealed record GetRequest;
            public sealed record GetResponse : Olve.TypeSpec.IOperationResponse;
            public interface IThingsGetHandler : Olve.TypeSpec.IOperationHandler<GetRequest, GetResponse>;
            public sealed record ListRequest;
            public sealed record ListResponse : Olve.TypeSpec.IOperationResponse;
            public interface IThingsListHandler : Olve.TypeSpec.IOperationHandler<ListRequest, ListResponse>;
        }

        """;

    private static readonly ImmutableArray<MetadataReference> References = AppContext
        .GetData("TRUSTED_PLATFORM_ASSEMBLIES") is string paths
        ? paths.Split(Path.PathSeparator)
            .Where(p => p.Length > 0)
            .Select(p => (MetadataReference)MetadataReference.CreateFromFile(p))
            .ToImmutableArray()
        : ImmutableArray<MetadataReference>.Empty;

    /// <summary>The generator's diagnostics, its generated source (null when it emits none) and the compiler's errors after generation.</summary>
    public static (ImmutableArray<Diagnostic> Diagnostics, string? Generated, ImmutableArray<Diagnostic> CompileErrors) Run(string body)
    {
        var compilation = CSharpCompilation.Create(
            assemblyName: "GeneratorSnippet",
            syntaxTrees: [CSharpSyntaxTree.ParseText(Prelude + body, new CSharpParseOptions(LanguageVersion.Latest))],
            references: References,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var driver = CSharpGeneratorDriver.Create(new OperationHandlerGenerator().AsSourceGenerator())
            .RunGeneratorsAndUpdateCompilation(compilation, out var output, out var diagnostics);
        var generated = driver.GetRunResult().GeneratedTrees.SingleOrDefault()?.ToString();
        var errors = output.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToImmutableArray();
        return (diagnostics, generated, errors);
    }
}
