using System.Reflection;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Olve.Results.TUnit;

namespace Olve.Validation.SourceGeneration.Tests;

public class MissingValidatorMethodTests
{
    private static Compilation CreateCompilation(string source)
    {
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location));

        return CSharpCompilation.Create(
            "Test",
            [CSharpSyntaxTree.ParseText(source)],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    [Test]
    public async Task ReportsDiagnosticWhenMethodMissing()
    {
        const string src = """
using Olve.Validation;

public class Person { public string Name { get; set; } = string.Empty; public int Age { get; set; } }

[ValidatorFor(typeof(Person))]
public partial class PersonValidator { }
""";
        var compilation = CreateCompilation(src);
        var generatorPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory!, "..", "..", "..", "..", "..", "src", "Olve.Validation.SourceGeneration", "bin", "Debug", "netstandard2.0", "Olve.Validation.SourceGeneration.dll"));
        var generatorAssembly = System.Reflection.Assembly.LoadFrom(generatorPath);
        var generatorType = generatorAssembly.GetType("Olve.Validation.SourceGeneration.ValidatorForGenerator")!;
        var generator = (IIncrementalGenerator)Activator.CreateInstance(generatorType)!;
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var output, out var diagnostics);
        var allDiagnostics = diagnostics.Concat(output.GetDiagnostics());

        await Assert.That(allDiagnostics.Any(d => d.Id == "OVSG001")).IsTrue();
    }

    [Test]
    public async Task NoDiagnosticWhenAllMethodsExist()
    {
        const string src = """
using Olve.Validation;

public class Person { public string Name { get; set; } = string.Empty; public int Age { get; set; } }

[ValidatorFor(typeof(Person))]
public partial class PersonValidator
{
    private static IValidator<string> GetNameValidator() => new StringValidator();
    private static IValidator<int> GetAgeValidator() => new IntValidator();
}
""";
        var compilation = CreateCompilation(src);
        var generatorPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory!, "..", "..", "..", "..", "..", "src", "Olve.Validation.SourceGeneration", "bin", "Debug", "netstandard2.0", "Olve.Validation.SourceGeneration.dll"));
        var generatorAssembly = System.Reflection.Assembly.LoadFrom(generatorPath);
        var generatorType = generatorAssembly.GetType("Olve.Validation.SourceGeneration.ValidatorForGenerator")!;
        var generator = (IIncrementalGenerator)Activator.CreateInstance(generatorType)!;
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var output, out var diagnostics);
        var allDiagnostics = diagnostics.Concat(output.GetDiagnostics());

        await Assert.That(allDiagnostics.Any(d => d.Id == "OVSG001")).IsFalse();
    }
}
