using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Text.Json;

namespace Olve.TypeSpec.Conformance;

/// <summary>
/// The rest of the generated surface: the operation table matches the contract, parameters bind
/// from where the contract puts them, the response unions (variant statuses, the error envelope),
/// binding failures on operations without a declared 400, and the startup check for missing
/// handlers. The suite runs without reflection-based JSON (the project's
/// <c>JsonSerializerIsReflectionEnabledByDefault</c>), so bodies are read as documents.
/// </summary>
[ClassDataSource<FixtureApp>(Shared = SharedType.PerAssembly)]
public class GeneratedSurfaceTests(FixtureApp fixture)
{
    private static IReadOnlyList<ApiOperation> GeneratedOperations =>
        [.. typeof(FixtureOperations).GetFields().Select(f => (ApiOperation)f.GetValue(null)!)];

    private static async Task<JsonElement> Body(HttpResponseMessage response) =>
        JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;

    private static string Joined(IEnumerable<string> items) => string.Join("\n", items);

    [Test]
    public async Task OperationTable_MatchesContractStatuses()
    {
        var generated = GeneratedOperations
            .Select(o => $"{o.OperationId}: {string.Join(",", o.SuccessStatuses.Concat(o.ErrorStatuses).Order())}")
            .Order()
            .ToList();
        var spec = OpenApiContract.Operations
            .Select(o => $"{o.OperationId}: {string.Join(",", o.DeclaredStatuses.Order())}")
            .Order()
            .ToList();

        await Assert.That(generated).IsNotEmpty();
        await Assert.That(Joined(generated)).IsEqualTo(Joined(spec));
    }

    [Test]
    public async Task EveryHandlerType_IsRegisteredByTheFixture()
    {
        await Assert.That(FixtureApi.HandlerTypes.Count).IsEqualTo(GeneratedOperations.Count);
        await Assert.That(fixture.Services.MissingHandlers(FixtureApi.HandlerTypes)).IsEmpty();
    }

    [Test]
    public async Task UseFixtureApi_WithAMissingHandler_Throws()
    {
        var build = () => FixtureApp.Build(services =>
        {
            FixtureHandlers.Register(services);
            services.RemoveAll<IShapesGetHandler>();
        });

        await Assert.That(build).Throws<InvalidOperationException>().WithMessageContaining(nameof(IShapesGetHandler));
    }

    [Test]
    public async Task QueryAndHeaderParameters_Bind()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/widgets?color=green&limit=7");
        request.Headers.Add("x-request-id", "req-1");

        using var response = await fixture.CreateClient().SendAsync(request);
        var widgets = await Body(response);

        var labels = widgets[0].GetProperty("labels");
        await Assert.That(labels.GetProperty("color").GetString()).IsEqualTo(nameof(Color.Green));
        await Assert.That(labels.GetProperty("limit").GetString()).IsEqualTo("7");
        await Assert.That(labels.GetProperty("requestId").GetString()).IsEqualTo("req-1");
    }

    [Test]
    public async Task PathQueryAndBodyParameters_Bind()
    {
        var body = ResponseConformanceTests.WidgetBody(serial: "SN-9", name: "renamed", extra: """ "note":"hi", """);

        using var response = await fixture.CreateClient().PutAsync("/api/widgets/w42?dryRun=false", ResponseConformanceTests.Json(body));
        var widget = await Body(response);

        await Assert.That((int)response.StatusCode).IsEqualTo(200);
        var labels = widget.GetProperty("labels");
        await Assert.That(labels.GetProperty("id").GetString()).IsEqualTo("w42");
        await Assert.That(labels.GetProperty("dryRun").GetString()).IsEqualTo("false");
        await Assert.That(labels.GetProperty("note").GetString()).IsEqualTo("hi");
        await Assert.That(widget.GetProperty("serial").GetString()).IsEqualTo("SN-9");
        await Assert.That(widget.GetProperty("name").GetString()).IsEqualTo("renamed");
    }

    [Test]
    public async Task RequestBody_RoundTripsUnionsNullablesAndOptionals()
    {
        var body = ResponseConformanceTests.WidgetBody(
            shape: """{"side":3,"kind":"square"}""",
            extra: """ "color":"red","parent":null,"tags":["a","b"],"labels":{"k":"v"},"weight":0, """);

        using var response = await fixture.CreateClient().PostAsync("/api/widgets", ResponseConformanceTests.Json(body));
        var widget = await Body(response);

        await Assert.That((int)response.StatusCode).IsEqualTo(201);
        await Assert.That(widget.GetProperty("shape").GetProperty("kind").GetString()).IsEqualTo("square");
        await Assert.That(widget.GetProperty("shape").GetProperty("side").GetDouble()).IsEqualTo(3);
        await Assert.That(widget.GetProperty("color").GetString()).IsEqualTo("red");
        await Assert.That(widget.GetProperty("parent").ValueKind).IsEqualTo(JsonValueKind.Null);
        await Assert.That(widget.GetProperty("weight").GetInt32()).IsEqualTo(0);
        await Assert.That(widget.GetProperty("tags").GetArrayLength()).IsEqualTo(2);
    }

    [Test]
    public async Task EventStream_NamesEachEventByItsType_AndPassesIdsThrough()
    {
        using var response = await fixture.CreateClient().GetAsync("/api/widget-events");
        var events = OpenApiContract.ParseEvents(await response.Content.ReadAsStringAsync());

        await Assert.That(response.Content.Headers.ContentType?.MediaType).IsEqualTo("text/event-stream");
        await Assert.That(Joined(events.Select(e => $"{e.EventType}#{e.EventId}")))
            .IsEqualTo(Joined(["ping#", "widget.changed#1", "widget.removed#2"]));
        foreach (var e in events)
        {
            using var data = JsonDocument.Parse(e.Data);
            await Assert.That(data.RootElement.GetProperty("type").GetString()).IsEqualTo(e.EventType);
        }
    }

    [Test]
    public async Task EventStream_BindsExplodeFalseListsAndHeaders()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/widget-events?type=widget.removed, widget.changed,");
        request.Headers.Add("Last-Event-ID", "1");

        using var response = await fixture.CreateClient().SendAsync(request);
        var events = OpenApiContract.ParseEvents(await response.Content.ReadAsStringAsync());

        await Assert.That(Joined(events.Select(e => e.EventType))).IsEqualTo("widget.removed");
    }

    [Test]
    public async Task RequiredExplodeFalseList_SplitsTrimsAndDropsEmptyEntries()
    {
        using var response = await fixture.CreateClient().GetAsync("/api/tags?names=a, b,,c");
        var names = (await Body(response)).EnumerateArray().Select(e => e.GetString()!);

        await Assert.That(Joined(names)).IsEqualTo(Joined(["a", "b", "c"]));
    }

    [Test]
    public async Task ErrorResponse_IsTheEnvelopeWithTheHandlersCode()
    {
        using var response = await fixture.CreateClient().PutAsync(
            $"/api/widgets/{FixtureHandlers.Conflict}", ResponseConformanceTests.Json(ResponseConformanceTests.WidgetBody()));
        var error = (await Body(response)).GetProperty("error");

        await Assert.That((int)response.StatusCode).IsEqualTo(409);
        await Assert.That(error.GetProperty("code").GetString()).IsEqualTo("WIDGET_LOCKED");
        await Assert.That(error.GetProperty("message").GetString()).IsEqualTo("'conflict' failed with WIDGET_LOCKED.");
        await Assert.That(error.GetProperty("details").EnumerateObject().Count()).IsEqualTo(0);
    }

    [Test]
    public async Task ValidationFailures_ListEveryProblemInDetails()
    {
        var body = ResponseConformanceTests.WidgetBody(name: new string('x', 51), extra: """ "weight":101, """);

        using var response = await fixture.CreateClient().PostAsync("/api/widgets", ResponseConformanceTests.Json(body));
        var error = (await Body(response)).GetProperty("error");

        await Assert.That((int)response.StatusCode).IsEqualTo(400);
        await Assert.That(error.GetProperty("code").GetString()).IsEqualTo(FixtureErrors.InvalidRequest);
        var problems = error.GetProperty("details").GetProperty("problems");
        await Assert.That(Joined(problems.EnumerateArray().Select(p => p.GetProperty("message").GetString()!).Order()))
            .IsEqualTo(Joined(["'name' cannot exceed 50 characters.", "'weight' cannot exceed 100."]));
        await Assert.That(error.GetProperty("message").GetString()).IsEqualTo(problems[0].GetProperty("message").GetString());
    }

    [Test]
    public async Task ResponseVariants_KnowTheirStatus()
    {
        WidgetsCreateResponse created = new WidgetsCreateResponse.Created(FixtureHandlers.SampleWidget());
        WidgetsCreateResponse accepted = new WidgetsCreateResponse.Accepted(FixtureHandlers.SampleWidget());
        WidgetsUpdateResponse implicitOk = FixtureHandlers.SampleWidget();

        await Assert.That(created.Status).IsEqualTo(201);
        await Assert.That(accepted.Status).IsEqualTo(202);
        await Assert.That(implicitOk).IsTypeOf<WidgetsUpdateResponse.Ok>();
    }

    [Test]
    [Arguments("/api/widgets?limit=abc")]
    [Arguments("/api/widgets?color=blue")]
    [Arguments("/api/tags")]
    [Arguments("/api/files/docs%2F..%2F..%2Fsecret")]
    [Arguments("/api/files/%2E%2E%2Fsecret")]
    [Arguments("/api/files/docs%2F.%2Freadme.txt")]
    public async Task BindingFailure_OnOperationWithoutDeclared400_IsBodyless400(string url)
    {
        using var response = await fixture.CreateClient().GetAsync(url);

        await Assert.That((int)response.StatusCode).IsEqualTo(400);
        await Assert.That(await response.Content.ReadAsStringAsync()).IsEmpty();
    }

    [Test]
    public async Task BindingFailure_OnOperationWithDeclared400_ExplainsTheFailure()
    {
        using var response = await fixture.CreateClient().PostAsync("/api/widgets", ResponseConformanceTests.Json("{"));
        var error = (await Body(response)).GetProperty("error");

        await Assert.That((int)response.StatusCode).IsEqualTo(400);
        await Assert.That(error.GetProperty("code").GetString()).IsEqualTo(FixtureErrors.InvalidRequest);
        await Assert.That(error.GetProperty("message").GetString()).IsNotNull().And.IsNotEmpty();
    }
}
