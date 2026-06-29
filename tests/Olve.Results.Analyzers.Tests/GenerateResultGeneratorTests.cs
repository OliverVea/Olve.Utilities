namespace Olve.Results.Analyzers.Tests;

/// <summary>
/// Spec for the diagnostics reported by <c>GenerateResultGenerator</c>. Currently covers ORES002: a case
/// factory may declare at most one payload parameter. Multi-parameter factories are skipped (no body is
/// generated), so without this diagnostic the only signal would be a cryptic CS8795.
/// </summary>
public class GenerateResultGeneratorTests
{
    [Test]
    public async Task MultiParameterFactory_ReportsOres002()
    {
        var diagnostics = GeneratorRunner.GetGeneratorDiagnostics("""
            namespace Sample
            {
                using Olve.Results;

                [GenerateResult]
                public readonly partial struct MultiResult
                {
                    [SuccessCase] public static partial MultiResult Ok(int a, int b);
                }
            }
            """);

        await Assert.That(diagnostics.Length).IsEqualTo(1);
        await Assert.That(diagnostics[0].Id).IsEqualTo("ORES002");
        await Assert.That(diagnostics[0].GetMessage()).Contains("Ok");
        await Assert.That(diagnostics[0].GetMessage()).Contains("2 parameters");
    }

    [Test]
    public async Task SingleParameterFactory_ReportsNoOres002()
    {
        var diagnostics = GeneratorRunner.GetGeneratorDiagnostics("""
            namespace Sample
            {
                using Olve.Results;

                [GenerateResult]
                public readonly partial struct OneResult
                {
                    [SuccessCase] public static partial OneResult Ok(int value);
                }
            }
            """);

        await Assert.That(diagnostics.Where(d => d.Id == "ORES002")).IsEmpty();
    }

    [Test]
    public async Task ParameterlessFactory_ReportsNoOres002()
    {
        var diagnostics = GeneratorRunner.GetGeneratorDiagnostics("""
            namespace Sample
            {
                using Olve.Results;

                [GenerateResult]
                public readonly partial struct UnitResult
                {
                    [SuccessCase] public static partial UnitResult Ok();
                }
            }
            """);

        await Assert.That(diagnostics.Where(d => d.Id == "ORES002")).IsEmpty();
    }

    [Test]
    public async Task EachMultiParameterFactory_ReportsItsOwnOres002()
    {
        var diagnostics = GeneratorRunner.GetGeneratorDiagnostics("""
            namespace Sample
            {
                using Olve.Results;

                [GenerateResult]
                public readonly partial struct MultiResult
                {
                    [SuccessCase] public static partial MultiResult Ok(int a, int b);
                    [GreyCase] public static partial MultiResult Maybe(int a, int b, int c);
                }
            }
            """);

        await Assert.That(diagnostics.Where(d => d.Id == "ORES002").Count()).IsEqualTo(2);
    }
}
