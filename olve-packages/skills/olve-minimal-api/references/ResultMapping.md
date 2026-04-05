# ResultMappingExtensions

API docs: [https://olivervea.github.io/Olve.Utilities/api/Olve.MinimalApi.ResultMappingExtensions.html](https://olivervea.github.io/Olve.Utilities/api/Olve.MinimalApi.ResultMappingExtensions.html)

Namespace: `Olve.MinimalApi`

Static class providing extension methods to map `Result` and `Result<T>` to Minimal API HTTP responses.

## Public Members

| Member | Signature |
| --- | --- |
| `WithResultMapping` | `static RouteHandlerBuilder WithResultMapping(this RouteHandlerBuilder builder)` |
| `WithResultMapping<TResult>` | `static RouteHandlerBuilder WithResultMapping<TResult>(this RouteHandlerBuilder builder)` |
| `ToHttpResult` | `static IResult ToHttpResult(this Result result)` |
| `ToHttpResult<T>` | `static IResult ToHttpResult<T>(this Result<T> result)` |

## WithResultMapping (non-generic)

Configures the endpoint to produce 200 OK (empty body) on success or 400 Bad Request with `ResultProblem[]` on failure. Used when the handler returns `Result`.

```csharp
app.MapDelete("/users/{id}", (int id, DeleteHandler handler, CancellationToken ct)
        => handler.RunAsync(new DeleteUser(id), ct))
    .WithResultMapping();
```

## WithResultMapping\<TResult\>

Configures the endpoint to produce 200 OK with `TResult` body on success or 400 Bad Request with `ResultProblem[]` on failure. Used when the handler returns `Result<TResult>`.

```csharp
app.MapGet("/users/{id}", (int id, UserHandler handler, CancellationToken ct)
        => handler.HandleAsync(new GetUser(id), ct))
    .WithResultMapping<UserDto>();
```

## ToHttpResult

Converts a `Result` directly into an `IResult` HTTP response. Returns `TypedResults.Ok()` on success or `TypedResults.BadRequest(problems.ToArray())` on failure.

```csharp
Result result = DoSomething();
return result.ToHttpResult();
```

## ToHttpResult\<T\>

Converts a `Result<T>` directly into an `IResult` HTTP response. Returns `TypedResults.Ok(value)` on success or `TypedResults.BadRequest(problems.ToArray())` on failure.

```csharp
Result<string> result = GetValue();
return result.ToHttpResult();
```

## Behavior

The `WithResultMapping` methods add an endpoint filter factory that inspects the handler return type at startup. At runtime the filter unwraps `Result` / `Result<T>` (including `Task<Result>` / `Task<Result<T>>`) and converts to the appropriate HTTP response.
