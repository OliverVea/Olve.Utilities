namespace Olve.TypeSpec.Generators.Tests;

public class OperationHandlerGeneratorTests
{
    private const string Handle = "public Task<{1}> HandleAsync({0} request, CancellationToken cancellationToken) => Task.FromResult(new {1}());";

    private static string Handler(string declaration, string request, string response) =>
        declaration + " { " + string.Format(Handle, request, response) + " }\n";

    [Test]
    public async Task Handlers_AreRegisteredUnderTheirOperationInterface()
    {
        var (diagnostics, generated, errors) = GeneratorRunner.Run(
            "namespace App { using Api; using System.Threading; using System.Threading.Tasks;\n" +
            Handler("public sealed class GetThing : IThingsGetHandler", "GetRequest", "GetResponse") +
            Handler("internal sealed class ListThings : IThingsListHandler", "ListRequest", "ListResponse") +
            "}");

        await Assert.That(diagnostics).IsEmpty();
        await Assert.That(errors).IsEmpty();
        await Assert.That(generated).Contains("AddTransient<global::Api.IThingsGetHandler, global::App.GetThing>(services)");
        await Assert.That(generated).Contains("AddTransient<global::Api.IThingsListHandler, global::App.ListThings>(services)");
    }

    [Test]
    public async Task NestedInternalHandler_IsRegistered()
    {
        var (diagnostics, generated, errors) = GeneratorRunner.Run(
            "namespace App { using Api; using System.Threading; using System.Threading.Tasks;\n" +
            "public static class Handlers {\n" +
            Handler("internal sealed class GetThing : IThingsGetHandler", "GetRequest", "GetResponse") +
            "} }");

        await Assert.That(diagnostics).IsEmpty();
        await Assert.That(errors).IsEmpty();
        await Assert.That(generated).Contains("global::App.Handlers.GetThing");
    }

    [Test]
    public async Task AbstractAndGenericClasses_AreNotHandlers()
    {
        var (diagnostics, generated, _) = GeneratorRunner.Run(
            "namespace App { using Api; using System.Threading; using System.Threading.Tasks;\n" +
            Handler("public abstract class Base : IThingsGetHandler", "GetRequest", "GetResponse") +
            Handler("public class Generic<T> : IThingsListHandler", "ListRequest", "ListResponse") +
            "}");

        await Assert.That(diagnostics).IsEmpty();
        await Assert.That(generated).IsNull();
    }

    [Test]
    public async Task NoHandlers_EmitsNothing()
    {
        var (diagnostics, generated, _) = GeneratorRunner.Run("namespace App { public sealed class NotAHandler { } }");

        await Assert.That(diagnostics).IsEmpty();
        await Assert.That(generated).IsNull();
    }

    [Test]
    public async Task TwoHandlersForOneOperation_IsAnError()
    {
        var (diagnostics, _, _) = GeneratorRunner.Run(
            "namespace App { using Api; using System.Threading; using System.Threading.Tasks;\n" +
            Handler("public sealed class GetThing : IThingsGetHandler", "GetRequest", "GetResponse") +
            Handler("public sealed class GetThingAgain : IThingsGetHandler", "GetRequest", "GetResponse") +
            "}");

        await Assert.That(diagnostics.Select(d => d.Id)).IsEquivalentTo(["OTSP001"]);
        await Assert.That(diagnostics[0].GetMessage()).Contains("Api.IThingsGetHandler");
    }

    [Test]
    public async Task PrivateNestedHandler_IsReported()
    {
        var (diagnostics, generated, _) = GeneratorRunner.Run(
            "namespace App { using Api; using System.Threading; using System.Threading.Tasks;\n" +
            "public static class Handlers {\n" +
            Handler("private sealed class GetThing : IThingsGetHandler", "GetRequest", "GetResponse") +
            "} }");

        await Assert.That(diagnostics.Select(d => d.Id)).IsEquivalentTo(["OTSP002"]);
        await Assert.That(generated).IsNull();
    }
}
