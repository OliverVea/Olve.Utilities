# Olve.MinimalApi — Overview

API docs: [https://olivervea.github.io/Olve.Utilities/api/Olve.MinimalApi.html](https://olivervea.github.io/Olve.Utilities/api/Olve.MinimalApi.html)

NuGet: [Olve.MinimalApi](https://www.nuget.org/packages/Olve.MinimalApi)

## Summary

ASP.NET Core Minimal API extensions for Olve.Results. Maps `Result` and `Result<T>` to HTTP responses, adds endpoint validation filters using `IValidator<T>`, and provides JSON conversion for `IPath`.

## Types

| Type | Description |
| --- | --- |
| `ResultMappingExtensions` | Maps `Result` / `Result<T>` to 200 OK or 400 Bad Request responses. |
| `ValidationApiExtensions` | Adds endpoint validation filters using `IValidator<T>`. |
| `IHandler<TRequest>` | Handler interface for operations returning `Result` (no value). |
| `IHandler<TRequest, TResponse>` | Handler interface for operations returning `Result<TResponse>`. |
| `PathJsonConverter` | JSON converter for `IPath` round-trip serialization. |
| `ServiceExtensions` | Registers `PathJsonConverter` in JSON options. |

## Dependencies

- `Olve.Results` — Result types and `ResultProblem`
- `Olve.Validation` — `IValidator<T>` interface
- `Olve.Paths` — `IPath` interface
- `Microsoft.AspNetCore.App` — Minimal API framework
