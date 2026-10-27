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

The contract is a `.tsp` file; `tsp compile` with the emitter writes `<prefix>Models.g.cs`, `<prefix>Api.g.cs` and `<prefix>JsonContext.g.cs` into a folder the service compiles (an MSBuild target can run the compile before the build).

```yaml
# tspconfig.yaml
emit:
  - "@typespec/openapi3"
  - "@olivervea/typespec-csharp"
options:
  "@olivervea/typespec-csharp":
    emitter-output-dir: "{project-root}/artifacts/generated"
    namespace: "MyService.Api"
    prefix: "MyService"
    external-types:
      ErrorEnvelope: "MyService.Api.ErrorEnvelope"   # a spec model the app already has a type for
```

A handler implements the generated interface and returns one of the declared responses. A success body converts implicitly; errors are named:

```csharp
public sealed class GetWidget(WidgetStore store) : IWidgetsGetHandler
{
    public Task<WidgetsGetResponse> HandleAsync(WidgetsGetRequest request, CancellationToken ct) =>
        Task.FromResult<WidgetsGetResponse>(store.Find(request.Id) is { } widget
            ? widget
            : new WidgetsGetResponse.NotFound(new ErrorEnvelope("WIDGET_NOT_FOUND", $"No widget '{request.Id}'.")));
}
```

Wiring: the generated `Add…Api` registers the JSON context and makes binding failures reach the middleware; the app registers its handlers and its `IApiErrorWriter`; the generated `Use…Api` checks every handler is registered (fail fast, not a runtime 500), adds the middleware and maps every operation. It returns the route builders, for app-side conventions such as authorization.

```csharp
builder.Services.AddMyServiceApi();
builder.Services.AddSingleton<IApiErrorWriter, MyErrorWriter>();
builder.Services.AddSingleton<IWidgetsGetHandler, GetWidget>();

var app = builder.Build();
var endpoints = app.UseMyServiceApi();
endpoints.WidgetsGet.RequireAuthorization("read");
```

The error writer decides the body of a binding or validation failure. A typical one answers its envelope when the operation declares the status and the bare status otherwise:

```csharp
public sealed class MyErrorWriter : IApiErrorWriter
{
    public IResult InvalidRequest(IReadOnlyList<ResultProblem> problems, int status, ApiOperation? operation) =>
        operation?.Declares(status) == true
            ? TypedResults.Json(new ErrorEnvelope("INVALID_REQUEST", problems[0].FormattedMessage), statusCode: status)
            : TypedResults.StatusCode(status);
}
```

### Server-sent events

An operation returning `SSEStream<Events>` of an `@events` union (every variant a JSON model whose `type` literal is its event name) gets a response union whose `Ok` carries `IAsyncEnumerable<ServerSentEvent<Events>>`. Each item's `Id` becomes the SSE id (`null` for heartbeats, so `Last-Event-ID` never points at one). The stream ends quietly on disconnect or shutdown.

### What the emitter checks

The emitter's conformance suite (in the Olve.Utilities repo) compiles a fixture contract with this runtime and proves over HTTP that every operation is routed, every response validates against the contract's schema for its status, declared statuses are honoured, parameters bind and binding failures are answered as the contract says. Services only test their own behaviour.

Shapes the emitter does not support are reported as `unsupported-*` warnings, never silently mistyped: untagged unions, enveloped discriminated unions, cookie and multipart parameters, status ranges and default responses, array parameters other than `explode: false` string lists in the query.
