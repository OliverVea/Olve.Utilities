# Olve.Operations

## Purpose

The purpose of this library is to enforce a consistent patterne for operations in .NET.

Operations are to be used to encapsulate a single unit of work. This ensures the following properties:

1. Each operation should encapsulate a single unit of work, no more, no less.
2. The services injected into each operation is limited to just the services required to perform the operation.
3. Allows for a pattern where operations can be composed together to form more complex operations.

## Usage

This small library specifies four interfaces for operations to be injected using DI:

- `IOperation<in TRequest>` - An operation that takes a request and returns no response.
- `IOperation<in TRequest, out TResponse>` - An operation that takes a request and returns a response.
- `IAsyncOperation<in TRequest>` - An asynchronous operation that takes a request and returns no response.
- `IAsyncOperation<in TRequest, TResponse>` - An asynchronous operation that takes a request and returns a response.

Each operation has a single public method - `Execute` or `ExecuteAsync` for synchronous and asynchronous operations respectively - that takes a request and returns a response.

An operation is added to the DI container by registering it in the `IServiceCollection`:

```csharp
services.AddSingleton<MyOperation>();
```

The operation can then be injected into a controller or other service:

```csharp
public class MyController(MyOperation myOperation, MyAsyncOperation myAsyncOperation) : ControllerBase
{
    public async Task<IActionResult> MyAction(MyRequest request, MyAsyncRequest asyncRequest, CancellationToken cancellationToken = default)
    {
        var response = myOperation.Execute(request);
        var asyncResponse = await myAsyncOperation.ExecuteAsync(asyncRequest, cancellationToken);
        
        return Ok(new { response, asyncResponse });
    }
}
```
