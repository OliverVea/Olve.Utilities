# @olivervea/typespec-csharp

[![npm](https://img.shields.io/npm/v/@olivervea/typespec-csharp?logo=npm)](https://www.npmjs.com/package/@olivervea/typespec-csharp)

A [TypeSpec](https://typespec.io) emitter: an HTTP contract → its C# minimal-API surface, on the [Olve.TypeSpec](https://www.nuget.org/packages/Olve.TypeSpec) runtime. The service compiles the generated files and hand-writes one handler class per operation; the contract is the only place the API is defined.

`tsp compile` writes three files, named after the service namespace (`namespace Widgets;` below; the `prefix` option overrides it):

| File | Contents |
| --- | --- |
| `WidgetsModels.g.cs` | Records (request shapes split by `@visibility`), enums, `@discriminated` unions, `@events` unions (`IServerSentEvent`), and an `IValidator<T>` per request body with constraints (`@minLength`, `@maxValue`, …). |
| `WidgetsApi.g.cs` | Per operation: a request record, a response union with one variant per declared status, and `I…Handler : IOperationHandler<…>`. Plus `WidgetsOperations` (declared statuses), `WidgetsApi.HandlerTypes`, `AddWidgetsApi()`, `UseWidgetsApi()`, `MapWidgetsApi()`. |
| `WidgetsJsonContext.g.cs` | A System.Text.Json source-generation context for every DTO (Native AOT ready). |

## Install

```bash
npm install --save-dev @olivervea/typespec-csharp @typespec/compiler @typespec/http @typespec/openapi3
dotnet add package Olve.TypeSpec
```

## Configure

In `tspconfig.yaml`:

```yaml
emit:
  - "@typespec/openapi3"
  - "@olivervea/typespec-csharp"
options:
  "@olivervea/typespec-csharp":
    emitter-output-dir: "{project-root}/artifacts/generated"
    namespace: "MyService.Api"        # default: Generated.Api
    prefix: "MyService"               # default: the service namespace's name → MyServiceApi, AddMyServiceApi, …
    external-types:                   # spec models the app already has a C# type for
      ErrorEnvelope: "MyService.Api.ErrorEnvelope"
```

Generated files belong in a gitignored folder the service's project compiles; an MSBuild target can run `tsp compile` before the build (`Inputs` the `.tsp` files, `Outputs` the `.g.cs`).

## What the contract gives you

- **Response unions.** `op update(...): Widget | NotFound | Conflict` becomes `WidgetsUpdateResponse` with `Ok(Widget)`, `NotFound(ErrorEnvelope)` and `Conflict(ErrorEnvelope)` variants. A handler returns one; an undeclared status can't compile. A success body no other variant carries converts implicitly (`return widget;`).
- **Several success statuses.** `(CreatedResponse & { @body body: Widget }) | (AcceptedResponse & …)` is `Created` and `Accepted`, each writing its own status.
- **Visibility.** `@visibility(Lifecycle.Read)` properties leave the request shapes (`WidgetCreate`, `WidgetWritable`); the response keeps them.
- **Discriminated unions** (`@discriminated(#{ envelope: "none", discriminatorPropertyName: "kind" })`) become an abstract record with `[JsonPolymorphic]`/`[JsonDerivedType]`; the discriminator may appear anywhere in a request object.
- **Parameters** bind from path, query and header; string enums by wire value (`?color=red`); `@query(#{ explode: false })` string lists from one comma-separated value.
- **SSE.** `SSEStream<Events>` of an `@events` union streams `ServerSentEvent<Events>` items: the event name is the variant's `type` literal, and the emitter refuses a variant whose `type` disagrees with its name.
- **Validation.** Constraints on a request body become a validator that runs before the handler; failures and binding failures are answered by the app's `IApiErrorWriter`, in the contract's terms (the matched operation's declared statuses are at hand).

Unsupported shapes are reported as `@olivervea/typespec-csharp/unsupported-type` or `unsupported-operation` warnings and emitted as `JsonElement` or skipped, never silently mistyped: untagged unions, enveloped discriminated unions, cookie parameters, multipart bodies, status code ranges, array parameters other than `explode: false` string lists in the query. `test/fixtures/unsupported.tsp` holds one operation per shape and the snapshot test asserts each diagnostic; none of them is blocked by the design, they are simply not needed yet. Validation covers the top-level request body only.

## Conformance

The emitter is proven once, in this repository: `test/emitter.test.js` snapshots the output of `test/fixtures/widgets.tsp`, and `tests/Olve.TypeSpec.Conformance` (.NET) compiles that output with the runtime, hosts it with trivial handlers and checks every operation and response over HTTP against the fixture's OpenAPI document. Services built on it test only their own behaviour.

```bash
npm test                                   # snapshots; UPDATE_SNAPSHOTS=1 npm test to accept new output
dotnet test --project tests/Olve.TypeSpec.Conformance   # from the repo root
```
