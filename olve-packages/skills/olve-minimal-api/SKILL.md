---
name: olve-minimal-api
description: Reference for Olve.MinimalApi — ASP.NET Core Minimal API extensions for Result mapping to HTTP responses, endpoint validation filters, handler interfaces, and IPath JSON conversion. Use when writing or reading code that integrates Olve.Results with Minimal API endpoints.
user-invocable: false
---

# Olve.MinimalApi

ASP.NET Core Minimal API extensions for [Olve.Results](https://www.nuget.org/packages/Olve.Results). Source: `Olve.Utilities/src/Olve.MinimalApi/`.

Reference docs: [README](references/README.md) | [Handlers](references/Handlers.md) | [ResultMapping](references/ResultMapping.md) | [Validation](references/Validation.md) | [Services](references/Services.md)

## Result Mapping

Map `Result` / `Result<T>` return values to HTTP responses automatically:

```csharp
// Generic — 200 OK with body or 400 Bad Request
app.MapGet("/users/{id}", (int id, UserHandler handler, CancellationToken ct)
        => handler.HandleAsync(new GetUser(id), ct))
    .WithResultMapping<UserDto>();

// Non-generic — 200 OK (empty) or 400 Bad Request
app.MapDelete("/users/{id}", (int id, DeleteHandler handler, CancellationToken ct)
        => handler.RunAsync(new DeleteUser(id), ct))
    .WithResultMapping();
```

| Return value | HTTP response |
| --- | --- |
| `Result` success | 200 OK (empty body) |
| `Result<T>` success | 200 OK with `T` as body |
| Any failure | 400 Bad Request with `ResultProblem[]` as body |

## Validation

Add validation filters using `IValidator<T>` from Olve.Validation:

```csharp
app.MapPost("/users", (CreateUserRequest request, UserHandler handler, CancellationToken ct)
        => handler.HandleAsync(request, ct))
    .WithResultMapping<UserDto>()
    .WithValidation<CreateUserRequest, CreateUserValidator>();
```

## Handler Interfaces

```csharp
// No return value (delete, update)
public class MyHandler : IHandler<MyRequest>
{
    public Task<Result> RunAsync(MyRequest request, CancellationToken ct) { ... }
}

// With return value (get, create)
public class MyHandler : IHandler<MyRequest, MyResponse>
{
    public Task<Result<MyResponse>> HandleAsync(MyRequest request, CancellationToken ct) { ... }
}
```

## Path JSON Conversion

```csharp
builder.Services.WithPathJsonConversion();
```
