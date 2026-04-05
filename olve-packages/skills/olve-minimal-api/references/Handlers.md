# Handler Interfaces

API docs: [https://olivervea.github.io/Olve.Utilities/api/Olve.MinimalApi.html](https://olivervea.github.io/Olve.Utilities/api/Olve.MinimalApi.html)

Namespace: `Olve.MinimalApi`

## IHandler\<TRequest\>

Handler for operations that return no value (e.g. delete, update).

```csharp
public interface IHandler<in TRequest>
{
    Task<Result> RunAsync(TRequest request, CancellationToken cancellationToken);
}
```

### Members

| Member | Signature |
| --- | --- |
| `RunAsync` | `Task<Result> RunAsync(TRequest request, CancellationToken cancellationToken)` |

## IHandler\<TRequest, TResponse\>

Handler for operations that return a value (e.g. get, create).

```csharp
public interface IHandler<in TRequest, TResponse>
{
    Task<Result<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken);
}
```

### Members

| Member | Signature |
| --- | --- |
| `HandleAsync` | `Task<Result<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken)` |

## Usage

```csharp
// Register as a service
builder.Services.AddScoped<IHandler<GetUser, UserDto>, GetUserHandler>();

// Wire to endpoint
app.MapGet("/users/{id}", (int id, IHandler<GetUser, UserDto> handler, CancellationToken ct)
        => handler.HandleAsync(new GetUser(id), ct))
    .WithResultMapping<UserDto>();
```
