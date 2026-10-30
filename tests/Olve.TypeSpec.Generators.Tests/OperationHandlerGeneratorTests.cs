namespace Olve.TypeSpec.Generators.Tests;

public class OperationHandlerGeneratorTests
{
    private const string Handle = "public Task<{1}> HandleAsync({0} request, CancellationToken cancellationToken) => Task.FromResult(new {1}());";

    private static string Get(string declaration) => Handler(declaration, "GetRequest", "GetResponse");

    private static string List(string declaration) => Handler(declaration, "ListRequest", "ListResponse");

    private static string Handler(string declaration, string request, string response) =>
        declaration + " { " + string.Format(Handle, request, response) + " }\n";

    private static readonly string ListThings = List("[OperationHandler] internal sealed class ListThings : IThingsListHandler");

    private const string Wiring =
        "public static class Startup { public static void Wire(Microsoft.Extensions.DependencyInjection.IServiceCollection services) => services.AddOperationHandlers(); }\n";

    /// <summary>An app that wires its handlers with <c>AddOperationHandlers</c>.</summary>
    private static string App(params string[] members) => Manual(Wiring + string.Concat(members));

    /// <summary>An app that doesn't call <c>AddOperationHandlers</c>.</summary>
    private static string Manual(params string[] members) =>
        GeneratorRunner.Api + "namespace App { using Api; using Microsoft.Extensions.DependencyInjection; using Olve.TypeSpec; using System.Threading; using System.Threading.Tasks;\n" +
        string.Concat(members) + "}";

    private static IEnumerable<string> Ids(IEnumerable<Microsoft.CodeAnalysis.Diagnostic> diagnostics) => diagnostics.Select(d => d.Id);

    [Test]
    public async Task MarkedHandlers_AreRegisteredUnderTheirOperationInterface()
    {
        var (diagnostics, generated, errors) = GeneratorRunner.Run(App(
            Get("[OperationHandler] public sealed class GetThing : IThingsGetHandler"),
            ListThings));

        await Assert.That(diagnostics).IsEmpty();
        await Assert.That(errors).IsEmpty();
        await Assert.That(generated).Contains("AddTransient<global::Api.IThingsGetHandler, global::App.GetThing>(services)");
        await Assert.That(generated).Contains("AddTransient<global::Api.IThingsListHandler, global::App.ListThings>(services)");
    }

    [Test]
    public async Task UnmarkedImplementation_IsNotRegistered()
    {
        var (diagnostics, generated, errors) = GeneratorRunner.Run(App(
            Get("[OperationHandler] public sealed class GetThing : IThingsGetHandler"),
            Get("public sealed class LoggingGetThing : IThingsGetHandler"),
            ListThings));

        await Assert.That(diagnostics).IsEmpty();
        await Assert.That(errors).IsEmpty();
        await Assert.That(generated).DoesNotContain("LoggingGetThing");
    }

    [Test]
    public async Task OperationWithoutMarkedHandler_IsAnError()
    {
        var (diagnostics, generated, _) = GeneratorRunner.Run(App(
            Get("public sealed class GetThing : IThingsGetHandler"),
            ListThings));

        await Assert.That(Ids(diagnostics)).IsEquivalentTo(["OTSP005"]);
        await Assert.That(diagnostics[0].GetMessage()).Contains("Api.IThingsGetHandler");
        await Assert.That(generated).DoesNotContain("GetThing");
    }

    [Test]
    public async Task OperationWithoutMarkedHandler_IsNotReported_WhenTheAppDoesNotCallAddOperationHandlers()
    {
        var (diagnostics, generated, errors) = GeneratorRunner.Run(Manual(
            Get("public sealed class GetThing : IThingsGetHandler"),
            List("public sealed class ListThings : IThingsListHandler"),
            "public static class Startup { public static void Wire(Microsoft.Extensions.DependencyInjection.IServiceCollection services) =>\n" +
            "    Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddTransient<IThingsGetHandler, GetThing>(services); }\n"));

        await Assert.That(diagnostics).IsEmpty();
        await Assert.That(errors).IsEmpty();
        await Assert.That(generated).IsNull();
    }

    [Test]
    public async Task MarkedHandlers_AreGenerated_WhenTheAppDoesNotCallAddOperationHandlers()
    {
        var (diagnostics, generated, _) = GeneratorRunner.Run(Manual(
            Get("public sealed class GetThing : IThingsGetHandler"),
            ListThings));

        await Assert.That(diagnostics).IsEmpty();
        await Assert.That(generated).Contains("global::App.ListThings");
    }

    [Test]
    public async Task NestedInternalHandler_IsRegistered()
    {
        var (diagnostics, generated, errors) = GeneratorRunner.Run(App(
            "public static class Handlers {\n",
            Get("[OperationHandler] internal sealed class GetThing : IThingsGetHandler"),
            "}\n",
            ListThings));

        await Assert.That(diagnostics).IsEmpty();
        await Assert.That(errors).IsEmpty();
        await Assert.That(generated).Contains("global::App.Handlers.GetThing");
    }

    [Test]
    public async Task RecordClassHandler_IsRegistered()
    {
        var (diagnostics, generated, errors) = GeneratorRunner.Run(App(
            Get("[OperationHandler] public sealed record GetThing : IThingsGetHandler"),
            ListThings));

        await Assert.That(diagnostics).IsEmpty();
        await Assert.That(errors).IsEmpty();
        await Assert.That(generated).Contains("global::App.GetThing");
    }

    [Test]
    public async Task HandlerOfTwoOperations_IsRegisteredUnderBoth()
    {
        var (diagnostics, generated, errors) = GeneratorRunner.Run(App(
            "[OperationHandler] public sealed class Things : IThingsGetHandler, IThingsListHandler {\n" +
            string.Format(Handle, "GetRequest", "GetResponse") + "\n" +
            string.Format(Handle, "ListRequest", "ListResponse") + "\n}\n"));

        await Assert.That(diagnostics).IsEmpty();
        await Assert.That(errors).IsEmpty();
        await Assert.That(generated).Contains("AddTransient<global::Api.IThingsGetHandler, global::App.Things>(services)");
        await Assert.That(generated).Contains("AddTransient<global::Api.IThingsListHandler, global::App.Things>(services)");
    }

    [Test]
    public async Task PartialHandler_IsRegisteredOnce()
    {
        var (diagnostics, generated, errors) = GeneratorRunner.Run(App(
            "[OperationHandler] public sealed partial class GetThing : IThingsGetHandler;\n",
            Get("public sealed partial class GetThing"),
            ListThings));

        await Assert.That(diagnostics).IsEmpty();
        await Assert.That(errors).IsEmpty();
        await Assert.That(generated!.Split("global::App.GetThing").Length - 1).IsEqualTo(1);
    }

    [Test]
    public async Task TwoHandlersForOneOperation_IsAnError()
    {
        var (diagnostics, _, _) = GeneratorRunner.Run(App(
            Get("[OperationHandler] public sealed class GetThing : IThingsGetHandler"),
            Get("[OperationHandler] public sealed class GetThingAgain : IThingsGetHandler"),
            ListThings));

        await Assert.That(Ids(diagnostics)).IsEquivalentTo(["OTSP001"]);
        await Assert.That(diagnostics[0].GetMessage()).Contains("Api.IThingsGetHandler");
    }

    [Test]
    public async Task MarkedSubclassOfAMarkedHandler_IsADuplicate()
    {
        var (diagnostics, _, _) = GeneratorRunner.Run(App(
            Get("[OperationHandler] public class GetThing : IThingsGetHandler"),
            "[OperationHandler] public sealed class GetOtherThing : GetThing;\n",
            ListThings));

        await Assert.That(Ids(diagnostics)).IsEquivalentTo(["OTSP001"]);
    }

    [Test]
    public async Task PrivateNestedHandler_IsAnError()
    {
        var (diagnostics, _, _) = GeneratorRunner.Run(App(
            "public static class Handlers {\n",
            Get("[OperationHandler] private sealed class GetThing : IThingsGetHandler"),
            "}\n",
            ListThings));

        await Assert.That(Ids(diagnostics)).IsEquivalentTo(["OTSP002", "OTSP005"]);
    }

    [Test]
    [Arguments("public abstract class GetThing : IThingsGetHandler")]
    [Arguments("public sealed class GetThing<T> : IThingsGetHandler")]
    public async Task UninstantiableHandler_IsAnError(string declaration)
    {
        var (diagnostics, _, _) = GeneratorRunner.Run(App(Get("[OperationHandler] " + declaration), ListThings));

        await Assert.That(Ids(diagnostics)).IsEquivalentTo(["OTSP004", "OTSP005"]);
    }

    [Test]
    public async Task HandlerWithoutOperation_IsAnError()
    {
        var (diagnostics, _, _) = GeneratorRunner.Run(App(
            Get("[OperationHandler] public sealed class GetThing : IThingsGetHandler"),
            ListThings,
            "[OperationHandler] public sealed class NotAHandler;\n"));

        await Assert.That(Ids(diagnostics)).IsEquivalentTo(["OTSP003"]);
    }

    [Test]
    public async Task NoOperationsAndNoHandlers_EmitsNothing()
    {
        var (diagnostics, generated, _) = GeneratorRunner.Run("namespace App { public sealed class NotAHandler { } }");

        await Assert.That(diagnostics).IsEmpty();
        await Assert.That(generated).IsNull();
    }
}
