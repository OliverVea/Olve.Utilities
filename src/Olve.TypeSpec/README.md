# Olve.TypeSpec

[![NuGet](https://img.shields.io/nuget/v/Olve.TypeSpec?logo=nuget)](https://www.nuget.org/packages/Olve.TypeSpec)
[![Docs](https://img.shields.io/badge/docs-API%20Reference-blue)](https://olivervea.github.io/Olve.Utilities/api/Olve.TypeSpec.html)

The runtime behind [`@olivervea/typespec-csharp`](https://www.npmjs.com/package/@olivervea/typespec-csharp), the TypeSpec emitter that turns an HTTP contract into a C# minimal-API surface: records, one response union per operation, one handler interface per operation, the route map, request validators and the JSON source-gen context. The generated code is compiled into the service; this package holds the hand-written types it relies on.

---

## Installation

```bash
dotnet add package Olve.TypeSpec
npm install --save-dev @olivervea/typespec-csharp @typespec/compiler @typespec/http
```

---

## Overview

| Type | Description |
| --- | --- |
| `IOperationHandler<TRequest, TResponse>` | Implements one contract operation; returns a variant of the operation's generated response union, so an undeclared status can't compile. |
| `IOperationResponse` | A response variant: its status and how it writes itself. |
| `ApiOperation` | An operation's id and declared statuses; attached to each generated endpoint as metadata. |
| `IApiErrorWriter` | The app's answer to binding and validation failures, in its own error body. One registration per app. |
| `RequestValidation` | Runs the generated request-body validator before the handler. |
| `BindingFailureMiddleware` | Answers `BadHttpRequestException` (malformed JSON, `limit=abc`, a missing parameter) through the error writer, and a bare 400 the endpoint set without a body (how the request delegate generator reports a bad optional parameter). |
| `JsonResponse<T>` | A response variant's JSON body with its status, serialized through the app's `JsonOptions` (no reflection, so it suits Native AOT). |
| `TextResponse`, `StreamResponse` | A `text/*` body written as is; a binary body copied from a `Stream` as it is read, then disposed. |
| `ResponseHeaders` | Sets a variant's declared `@header`s (null = not sent) before its body. |
| `PathParameters` | `CatchAll`: a `{+path}` value, with the `%2F` generated clients send read as `/`; a `.` or `..` segment is a binding failure (400). |
| `IServerSentEvent`, `ServerSentEvent<T>`, `ServerSentEventsResult<T>` | SSE streams of an `@events` union: event name = the variant's `type`. |
| `TypeSpecApiExtensions` | `AddTypeSpecApi` / `UseTypeSpecApi`, called by the generated `Add…Api` / `Use…Api`. |

---

## Usage

The examples below are the conformance suite's fixture (`tests/Olve.TypeSpec.Conformance`), which the emitter's tests keep current. The contract is a `.tsp` file:

```ts
// ../typespec-csharp/test/fixtures/widgets.tsp#L28-L43

model Circle {
  kind: "circle";
  radius: float64;
}

model Square {
  kind: "square";
  side: float64;
}

/** A shape, discriminated on `kind`. */
@discriminated(#{ envelope: "none", discriminatorPropertyName: "kind" })
union Shape {
  circle: Circle,
  square: Square,
}
```

```ts
// ../typespec-csharp/test/fixtures/widgets.tsp#L137-L148

/** The area of a shape. */
model Area {
  value: float64;
}

@route("/shapes")
interface Shapes {
  @get get(@path id: string): Shape | NotFound;

  /** Takes a shape, answers its area: the handler switches on the variant. */
  @route("area") @post area(@body shape: Shape): Area;
}
```

`tsp compile` with the emitter writes `<prefix>Models.g.cs`, `<prefix>Api.g.cs` and `<prefix>JsonContext.g.cs` into a folder the service compiles (an MSBuild target can run the compile before the build). The prefix is the service namespace's name unless set in `tspconfig.yaml`:

```yaml
emit:
  - "@typespec/openapi3"
  - "@olivervea/typespec-csharp"
options:
  "@olivervea/typespec-csharp":
    emitter-output-dir: "{project-root}/artifacts/generated"
    namespace: "Fixture.Api"
    external-types:
      ErrorEnvelope: "Fixture.FixtureErrorEnvelope"   # a spec model the app already has a type for
```

A handler implements the generated interface and returns one of the declared responses. A success body converts implicitly; errors are named:

```cs
// ../../tests/Olve.TypeSpec.Conformance/FixtureHandlers.cs#L223-L236

/// <summary>A union in the response: the stored data decides the variant; it converts implicitly to <c>Ok</c>.</summary>
private sealed class GetShape : IShapesGetHandler
{
    private static readonly Dictionary<string, Shape> Shapes = new()
    {
        ["c-1"] = new Circle { Radius = 1.5 },
        ["s-1"] = new Square { Side = 2 },
    };

    public Task<ShapesGetResponse> HandleAsync(ShapesGetRequest request, CancellationToken cancellationToken) =>
        Task.FromResult<ShapesGetResponse>(Shapes.TryGetValue(request.Id, out var shape)
            ? shape
            : new ShapesGetResponse.NotFound(Error("SHAPE_NOT_FOUND", request.Id)));
}
```

A discriminated union in a request arrives as the abstract record; the handler switches on the variant records:

```cs
// ../../tests/Olve.TypeSpec.Conformance/FixtureHandlers.cs#L208-L221

/// <summary>A union in the request: the handler switches on the variant records.</summary>
private sealed class ShapeArea : IShapesAreaHandler
{
    public Task<ShapesAreaResponse> HandleAsync(ShapesAreaRequest request, CancellationToken cancellationToken)
    {
        var area = request.Body switch
        {
            Circle circle => Math.PI * circle.Radius * circle.Radius,
            Square square => square.Side * square.Side,
            _ => throw new UnreachableException(),
        };
        return Task.FromResult<ShapesAreaResponse>(new Area { Value = area });
    }
}
```

The error writer decides the body of a binding or validation failure. This one answers its envelope when the operation declares the status and the bare status otherwise:

```cs
// ../../tests/Olve.TypeSpec.Conformance/FixtureErrors.cs#L44-L62

/// <summary>
/// Answers binding and validation failures with the envelope when the operation declares the
/// status, else with the status alone: the first problem is the message, every problem goes in
/// <c>details.problems</c> when there are several.
/// </summary>
public sealed class FixtureErrorWriter : IApiErrorWriter
{
    public IResult InvalidRequest(IReadOnlyList<ResultProblem> problems, int status, ApiOperation? operation)
    {
        if (operation?.Declares(status) != true)
        {
            return TypedResults.StatusCode(status);
        }

        var entries = problems.Select(p => new FixtureErrorProblem(FixtureErrors.InvalidRequest, p.FormattedMessage)).ToList();
        var error = new FixtureError(FixtureErrors.InvalidRequest, entries[0].Message, new FixtureErrorDetails { Problems = entries.Count > 1 ? entries : null });
        return TypedResults.Json(new FixtureErrorEnvelope(error), FixtureErrorJsonContext.Default.FixtureErrorEnvelope, statusCode: status);
    }
}
```

Wiring: the generated `Add…Api` registers the JSON context and makes binding failures reach the middleware; the app registers its handlers and its `IApiErrorWriter`; the generated `Use…Api` checks every handler is registered (fail fast, not a runtime 500), adds the middleware and maps every operation. It returns the route builders, for app-side conventions such as authorization (`endpoints.ShapesGet.RequireAuthorization("read")`).

```cs
// ../../tests/Olve.TypeSpec.Conformance/FixtureApp.cs#L36-L48

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

### Server-sent events

An operation returning `SSEStream<Events>` of an `@events` union (every variant a JSON model whose `type` literal is its event name) gets a response union whose `Ok` carries `IAsyncEnumerable<ServerSentEvent<Events>>`:

```ts
// ../typespec-csharp/test/fixtures/widgets.tsp#L185-L200

/** The widget event stream. */
@TypeSpec.Events.events
union WidgetEvent {
  @TypeSpec.Events.contentType("application/json") ping: Ping,
  @TypeSpec.Events.contentType("application/json") `widget.changed`: WidgetChanged,
  @TypeSpec.Events.contentType("application/json") `widget.removed`: WidgetRemoved,
}

@route("/widget-events")
interface WidgetEvents {
  /** Streams widget events; `type` limits them to these event names. */
  @get stream(
    @query(#{ explode: false }) type?: string[],
    @header("Last-Event-ID") lastEventId?: string,
  ): SSEStream<WidgetEvent> | BadRequest;
}
```

The handler yields events; the runtime writes each one as a frame named by the variant's `type`, with the item's `Id` as the SSE id (`null` for heartbeats, so `Last-Event-ID` never points at one). Nothing is kept per connection: `Last-Event-ID` is an ordinary header parameter the handler reads to decide where to resume, reconnecting is the client's job, and the stream ends quietly when the client disconnects or the app stops.

```cs
// ../../tests/Olve.TypeSpec.Conformance/FixtureHandlers.cs#L161-L192

region

ion sse
<summary>
A finite stream: a ping (no id), then <c>widget.changed</c> (id 1) and <c>widget.removed</c>
(id 2). <c>type</c> keeps only the named events (an unknown name is a 400 before streaming);
<c>Last-Event-ID</c> skips ids up to it.
</summary>
ate sealed class StreamWidgetEvents : IWidgetEventsStreamHandler

public static readonly Guid WidgetId = Guid.Parse("0b6f7f0e-4a52-4d0e-9d7a-5b1f0f6c2a11");

public Task<WidgetEventsStreamResponse> HandleAsync(WidgetEventsStreamRequest request, CancellationToken cancellationToken)
{
    if (request.Type?.FirstOrDefault(t => !WidgetEvent.EventTypes.Contains(t)) is { } unknown)
    {
        return Task.FromResult<WidgetEventsStreamResponse>(
            new WidgetEventsStreamResponse.BadRequest(FixtureError.Create("UNKNOWN_EVENT", $"Unknown event type '{unknown}'.")));
    }

    var after = long.TryParse(request.LastEventId, out var id) ? id : 0;
    var events = Events()
        .Where(e => request.Type is null || request.Type.Contains(e.Data.EventType))
        .Where(e => e.Id is null || long.Parse(e.Id, System.Globalization.CultureInfo.InvariantCulture) > after);
    return Task.FromResult<WidgetEventsStreamResponse>(new WidgetEventsStreamResponse.Ok(events.ToAsyncEnumerable()));
}

private static IEnumerable<ServerSentEvent<WidgetEvent>> Events()
{
    yield return new(new Ping { At = DateTimeOffset.UnixEpoch });
    yield return new(new WidgetChanged { At = DateTimeOffset.UnixEpoch, WidgetId = WidgetId, Name = "sample", Shape = new Square { Side = 2 } }, "1");
    yield return new(new WidgetRemoved { At = DateTimeOffset.UnixEpoch, WidgetId = WidgetId }, "2");
```

### Performance and AOT

The package is `IsAotCompatible` and `IsTrimmable`, and builds with the AOT analyzers as errors: no reflection, the middleware is registered with `app.Use`, JSON goes through the generated source-gen context, and every error body is written with a `JsonTypeInfo`. Per request, the generated endpoint allocates the request record and the response variant; the validation filter is one `IValidator<T>` instance per endpoint and a scan of the handful of bound arguments.

### What the emitter checks

The emitter's conformance suite (in the Olve.Utilities repo) compiles the fixture contract above with this runtime and proves over HTTP that every operation is routed, every response validates against the contract's schema for its status, declared statuses are honoured, parameters bind and binding failures are answered as the contract says. Services only test their own behaviour.

Shapes the emitter does not support are reported as `unsupported-*` warnings, never silently mistyped: untagged unions, enveloped discriminated unions, cookie parameters, multipart bodies, status code ranges, array parameters other than `explode: false` string lists in the query.
