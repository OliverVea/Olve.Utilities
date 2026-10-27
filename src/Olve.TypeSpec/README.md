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
| `BindingFailureMiddleware` | Answers `BadHttpRequestException` (malformed JSON, `limit=abc`, a missing parameter) through the error writer. |
| `IServerSentEvent`, `ServerSentEvent<T>`, `ServerSentEventsResult<T>` | SSE streams of an `@events` union: event name = the variant's `type`. |
| `TypeSpecApiExtensions` | `AddTypeSpecApi` / `UseTypeSpecApi`, called by the generated `Add…Api` / `Use…Api`. |

---

## Usage

The examples below are the conformance suite's fixture (`tests/Olve.TypeSpec.Conformance`), which the emitter's tests keep current. The contract is a `.tsp` file:

```ts
// ../typespec-csharp/test/fixtures/widgets.tsp#L25-L40

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
// ../typespec-csharp/test/fixtures/widgets.tsp#L126-L129

@route("/shapes")
interface Shapes {
  @get get(@path id: string): Shape | NotFound;
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
// ../../tests/Olve.TypeSpec.Conformance/FixtureHandlers.cs#L186-L195

private sealed class GetShape : IShapesGetHandler
{
    public Task<ShapesGetResponse> HandleAsync(ShapesGetRequest request, CancellationToken cancellationToken) =>
        Task.FromResult<ShapesGetResponse>(request.Id switch
        {
            "circle" => new Circle { Radius = 1.5 },
            "square" => new Square { Side = 2 },
            _ => new ShapesGetResponse.NotFound(Error("SHAPE_NOT_FOUND", request.Id)),
        });
}
```

The error writer decides the body of a binding or validation failure. This one answers its envelope when the operation declares the status and the bare status otherwise:

```cs
// ../../tests/Olve.TypeSpec.Conformance/FixtureErrors.cs#L48-L61

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
// ../../tests/Olve.TypeSpec.Conformance/FixtureApp.cs#L36-L47

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

An operation returning `SSEStream<Events>` of an `@events` union (every variant a JSON model whose `type` literal is its event name) gets a response union whose `Ok` carries `IAsyncEnumerable<ServerSentEvent<Events>>`. Each item's `Id` becomes the SSE id (`null` for heartbeats, so `Last-Event-ID` never points at one). The stream ends quietly on disconnect or shutdown.

### Performance and AOT

The package is `IsAotCompatible` and `IsTrimmable`, and builds with the AOT analyzers as errors: no reflection, the middleware is registered with `app.Use`, JSON goes through the generated source-gen context, and every error body is written with a `JsonTypeInfo`. Per request, the generated endpoint allocates the request record and the response variant; the validation filter is one `IValidator<T>` instance per endpoint and a scan of the handful of bound arguments.

### What the emitter checks

The emitter's conformance suite (in the Olve.Utilities repo) compiles the fixture contract above with this runtime and proves over HTTP that every operation is routed, every response validates against the contract's schema for its status, declared statuses are honoured, parameters bind and binding failures are answered as the contract says. Services only test their own behaviour.

Shapes the emitter does not support are reported as `unsupported-*` warnings, never silently mistyped: untagged unions, enveloped discriminated unions, cookie parameters, multipart bodies, status code ranges, array parameters other than `explode: false` string lists in the query.
