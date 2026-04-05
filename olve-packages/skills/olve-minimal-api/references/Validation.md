# ValidationApiExtensions

API docs: [https://olivervea.github.io/Olve.Utilities/api/Olve.MinimalApi.ValidationApiExtensions.html](https://olivervea.github.io/Olve.Utilities/api/Olve.MinimalApi.ValidationApiExtensions.html)

Namespace: `Olve.MinimalApi`

Static class providing extension methods to add validation filters for Minimal API endpoints using `IValidator<T>` from `Olve.Validation`.

## Public Members

| Member | Signature |
| --- | --- |
| `WithValidation<TRequest, TValidator>` | `static RouteHandlerBuilder WithValidation<TRequest, TValidator>(this RouteHandlerBuilder builder) where TValidator : IValidator<TRequest>, new()` |
| `WithValidation<TRequest, TValidator>` | `static RouteHandlerBuilder WithValidation<TRequest, TValidator>(this RouteHandlerBuilder builder, TValidator validator) where TValidator : IValidator<TRequest>` |

## WithValidation (parameterless)

Creates a new `TValidator` instance and adds a validation endpoint filter. The validator must have a parameterless constructor.

```csharp
app.MapPost("/users", (CreateUserRequest request, UserHandler handler, CancellationToken ct)
        => handler.HandleAsync(request, ct))
    .WithResultMapping<UserDto>()
    .WithValidation<CreateUserRequest, CreateUserValidator>();
```

## WithValidation (with instance)

Adds a validation endpoint filter using the provided validator instance.

```csharp
var validator = new CreateUserValidator();
app.MapPost("/users", (CreateUserRequest request, UserHandler handler, CancellationToken ct)
        => handler.HandleAsync(request, ct))
    .WithResultMapping<UserDto>()
    .WithValidation<CreateUserRequest, CreateUserValidator>(validator);
```

## Behavior

The filter extracts the first `TRequest` argument from the endpoint invocation context. If found, it runs `validator.Validate(request)`. On validation failure, it short-circuits and returns the validation result as an HTTP response via `ToHttpResult()`. If no `TRequest` argument is found, the filter passes through to the next handler.
