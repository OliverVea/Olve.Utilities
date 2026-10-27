# Wiring

Generated from `src/typespec-csharp/test/fixtures/widgets.tsp` by the emitter's tests; do not edit.

`Add<Prefix>Api` registers the generated JSON context and makes binding failures reach the middleware; the app registers its handlers and one `IApiErrorWriter`; `Use<Prefix>Api` checks every handler is registered (fail fast, never a runtime 500), adds the middleware and maps every operation, returning the route builders for app-side conventions such as authorization.

## Generated

```csharp
public static class FixtureApi
{
    /// <summary>Every operation's handler interface; each must have a registered implementation.</summary>
    public static IReadOnlyList<Type> HandlerTypes { get; } =
    [
        typeof(IShapesAreaHandler),
        typeof(IShapesGetHandler),
        typeof(ITagsEchoHandler),
        typeof(IWidgetEventsStreamHandler),
        typeof(IWidgetsCreateHandler),
        typeof(IWidgetsDeleteHandler),
        typeof(IWidgetsListHandler),
        typeof(IWidgetsUpdateHandler),
    ];

    /// <summary>Registers the generated JSON context and the runtime's request handling (the app registers its <see cref="IApiErrorWriter"/> and handlers).</summary>
    public static IServiceCollection AddFixtureApi(this IServiceCollection services) => services.AddTypeSpecApi(FixtureJsonContext.Default);

    /// <summary>Checks every handler is registered, adds the runtime's middleware and maps every contract operation.</summary>
    public static FixtureEndpoints UseFixtureApi(this WebApplication app) => app.UseTypeSpecApi(HandlerTypes).MapFixtureApi();

    /// <summary>Maps every contract operation onto its handler interface.</summary>
    public static FixtureEndpoints MapFixtureApi(this IEndpointRouteBuilder app) => new()
    {
        ShapesArea = app.MapPost("/api/shapes/area", async (
                [FromServices] IShapesAreaHandler handler,
                [FromBody] Shape body,
                CancellationToken ct) =>
                (await handler.HandleAsync(new ShapesAreaRequest(body), ct)).ToHttpResult())
            .WithName("Shapes_area")
            .WithMetadata(FixtureOperations.ShapesArea)
            .Produces<Area>(200),
        ShapesGet = app.MapGet("/api/shapes/{id}", async (
                [FromServices] IShapesGetHandler handler,
                [FromRoute(Name = "id")] string id,
                CancellationToken ct) =>
                (await handler.HandleAsync(new ShapesGetRequest(id), ct)).ToHttpResult())
            .WithName("Shapes_get")
            .WithMetadata(FixtureOperations.ShapesGet)
            .Produces<Shape>(200)
            .Produces<FixtureErrorEnvelope>(404),
        TagsEcho = app.MapGet("/api/tags", async (
                [FromServices] ITagsEchoHandler handler,
                [FromQuery(Name = "names")] string names,
                CancellationToken ct) =>
                (await handler.HandleAsync(new TagsEchoRequest(QueryParameters.List(names)), ct)).ToHttpResult())
            .WithName("Tags_echo")
            .WithMetadata(FixtureOperations.TagsEcho)
            .Produces<IReadOnlyList<string>>(200),
        WidgetEventsStream = app.MapGet("/api/widget-events", async (
                [FromServices] IWidgetEventsStreamHandler handler,
                [FromQuery(Name = "type")] string? type,
                [FromHeader(Name = "Last-Event-ID")] string? lastEventId,
                CancellationToken ct) =>
                (await handler.HandleAsync(new WidgetEventsStreamRequest(QueryParameters.List(type), lastEventId), ct)).ToHttpResult())
            .WithName("WidgetEvents_stream")
            .WithMetadata(FixtureOperations.WidgetEventsStream)
            .Produces<WidgetEvent>(200, "text/event-stream")
            .Produces<FixtureErrorEnvelope>(400),
        WidgetsCreate = app.MapPost("/api/widgets", async (
                [FromServices] IWidgetsCreateHandler handler,
                [FromBody] WidgetCreate body,
                CancellationToken ct) =>
                (await handler.HandleAsync(new WidgetsCreateRequest(body), ct)).ToHttpResult())
            .WithName("Widgets_create")
            .WithMetadata(FixtureOperations.WidgetsCreate)
            .WithRequestValidation<WidgetCreate, WidgetCreateValidator>(FixtureOperations.WidgetsCreate)
            .Produces<Widget>(201)
            .Produces<Widget>(202)
            .Produces<FixtureErrorEnvelope>(400),
        WidgetsDelete = app.MapDelete("/api/widgets/{id}", async (
                [FromServices] IWidgetsDeleteHandler handler,
                [FromRoute(Name = "id")] string id,
                CancellationToken ct) =>
                (await handler.HandleAsync(new WidgetsDeleteRequest(id), ct)).ToHttpResult())
            .WithName("Widgets_delete")
            .WithMetadata(FixtureOperations.WidgetsDelete)
            .Produces(204)
            .Produces<FixtureErrorEnvelope>(404),
        WidgetsList = app.MapGet("/api/widgets", async (
                [FromServices] IWidgetsListHandler handler,
                [FromQuery(Name = "color")] string? color,
                [FromQuery(Name = "limit")] int? limit,
                [FromHeader(Name = "x-request-id")] string? requestId,
                CancellationToken ct) =>
                (await handler.HandleAsync(new WidgetsListRequest(color is null ? null : FixtureParameters.ParseColor(color, "color"), limit, requestId), ct)).ToHttpResult())
            .WithName("Widgets_list")
            .WithMetadata(FixtureOperations.WidgetsList)
            .Produces<IReadOnlyList<Widget>>(200),
        WidgetsUpdate = app.MapPut("/api/widgets/{id}", async (
                [FromServices] IWidgetsUpdateHandler handler,
                [FromRoute(Name = "id")] string id,
                [FromQuery(Name = "dryRun")] bool? dryRun,
                [FromBody] WidgetWritable body,
                CancellationToken ct) =>
                (await handler.HandleAsync(new WidgetsUpdateRequest(id, dryRun, body), ct)).ToHttpResult())
            .WithName("Widgets_update")
            .WithMetadata(FixtureOperations.WidgetsUpdate)
            .WithRequestValidation<WidgetWritable, WidgetWritableValidator>(FixtureOperations.WidgetsUpdate)
            .Produces<Widget>(200)
            .Produces<FixtureErrorEnvelope>(400)
            .Produces<FixtureErrorEnvelope>(404)
            .Produces<FixtureErrorEnvelope>(409),
    };
}
```

## Hand-written

```csharp
/// <summary>Builds (but doesn't start) the fixture app with the given handler registrations.</summary>
public static WebApplication Build(Action<IServiceCollection> registerHandlers)
{
    var builder = WebApplication.CreateSlimBuilder(new WebApplicationOptions { EnvironmentName = "Testing" });
    builder.WebHost.UseTestServer();
    builder.Services.AddFixtureApi();
    builder.Services.AddSingleton<IApiErrorWriter, FixtureErrorWriter>();
    registerHandlers(builder.Services);

    var app = builder.Build();
    app.UseFixtureApi();
    return app;
}
```