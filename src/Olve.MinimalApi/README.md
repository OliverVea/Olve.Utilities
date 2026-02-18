# Olve.MinimalApi

[![NuGet](https://img.shields.io/nuget/v/Olve.MinimalApi?logo=nuget)](https://www.nuget.org/packages/Olve.MinimalApi)
[![Docs](https://img.shields.io/badge/docs-API%20Reference-blue)](https://olivervea.github.io/Olve.Utilities/api/Olve.MinimalApi.html)

ASP.NET Core Minimal API extensions for [Olve.Results](https://www.nuget.org/packages/Olve.Results). Maps `Result` and `Result<T>` to HTTP responses, adds endpoint validation filters, and provides JSON conversion for `IPath`.

---

## Installation

```bash
dotnet add package Olve.MinimalApi
```

---

## Overview

| Type | Description |
| --- | --- |
| `ResultMappingExtensions` | Maps `Result` / `Result<T>` to 200 OK or 400 Bad Request responses. |
| `ValidationApiExtensions` | Adds endpoint validation filters using `IValidator<T>`. |
| `IHandler<TRequest>` | Handler interface returning `Result`. |
| `IHandler<TRequest, TResponse>` | Handler interface returning `Result<TResponse>`. |
| `PathJsonConverter` | JSON converter for `IPath` round-trip serialization. |
| `ServiceExtensions` | Registers `PathJsonConverter` in JSON options. |

---

## Usage

### Result mapping

Use `WithResultMapping()` on a `RouteHandlerBuilder` to automatically convert `Result` and `Result<T>` return values into HTTP responses:

```csharp
app.MapGet("/users/{id}", (int id, UserHandler handler, CancellationToken ct)
        => handler.HandleAsync(new GetUser(id), ct))
    .WithResultMapping<UserDto>();
```

| Return value | HTTP response |
| --- | --- |
| `Result` success | 200 OK (empty body) |
| `Result<T>` success | 200 OK with `T` as body |
| Any failure | 400 Bad Request with `ResultProblem[]` as body |

The non-generic `WithResultMapping()` is used when the handler returns `Result` (no value):

```csharp
app.MapDelete("/users/{id}", (int id, DeleteHandler handler, CancellationToken ct)
        => handler.RunAsync(new DeleteUser(id), ct))
    .WithResultMapping();
```

You can also convert results manually with the `ToHttpResult()` extension:

```csharp
app.MapGet("/manual", () =>
{
    Result<string> result = Result.Success("hello");
    return result.ToHttpResult();
});
```

---

### Validation

Use `WithValidation<TRequest, TValidator>()` on a `RouteHandlerBuilder` to validate requests before the handler runs. If validation fails, a 400 Bad Request with the validation problems is returned immediately:

```csharp
app.MapPost("/users", (CreateUserRequest request, UserHandler handler, CancellationToken ct)
        => handler.HandleAsync(request, ct))
    .WithResultMapping<UserDto>()
    .WithValidation<CreateUserRequest, CreateUserValidator>();
```

The validator must implement `IValidator<TRequest>` from Olve.Validation. A new instance is created per endpoint by default, or you can pass an existing instance:

```csharp
var validator = new CreateUserValidator();
app.MapPost("/users", ...)
    .WithValidation<CreateUserRequest, CreateUserValidator>(validator);
```

---

### Handler interfaces

Two handler interfaces standardize the request/response pattern:

```csharp
// For operations that return no value (e.g. delete, update)
public interface IHandler<in TRequest>
{
    Task<Result> RunAsync(TRequest request, CancellationToken cancellationToken);
}

// For operations that return a value (e.g. get, create)
public interface IHandler<in TRequest, TResponse>
{
    Task<Result<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken);
}
```

---

### Path JSON conversion

Register `PathJsonConverter` to enable JSON round-trip serialization of `IPath` values:

```csharp
builder.Services.WithPathJsonConversion();
```

This adds the converter to the default `JsonSerializerOptions`, allowing `IPath` to be used in request/response models. Null JSON values deserialize to `null`.

---

## Documentation

Full API reference:
[https://olivervea.github.io/Olve.Utilities/api/Olve.MinimalApi.html](https://olivervea.github.io/Olve.Utilities/api/Olve.MinimalApi.html)

---

## License

MIT License © [OliverVea](https://github.com/OliverVea)
