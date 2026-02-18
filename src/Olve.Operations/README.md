# Olve.Operations

[![NuGet](https://img.shields.io/nuget/v/Olve.Operations?logo=nuget)](https://www.nuget.org/packages/Olve.Operations)
[![Docs](https://img.shields.io/badge/docs-API%20Reference-blue)](https://olivervea.github.io/Olve.Utilities/api/Olve.Operations.html)

> **Deprecated:** This package is no longer maintained. The abstractions it provides add little value over plain method signatures or `Func<TRequest, Result>`. Consider using simple interfaces or delegates instead.

Abstractions for implementing operations that return `Result` objects. Provides sync and async variants, with optional return values and DI-based factories.

---

## Installation

```bash
dotnet add package Olve.Operations
```

---

## Overview

| Type | Description |
| --- | --- |
| `IOperation<TRequest>` | Synchronous operation returning `Result`. |
| `IOperation<TRequest, TResult>` | Synchronous operation returning `Result<TResult>`. |
| `IAsyncOperation<TRequest>` | Async operation returning `Task<Result>`. |
| `IAsyncOperation<TRequest, TResult>` | Async operation returning `Task<Result<TResult>>`. |
| `OperationFactory<TOperation, TRequest>` | DI factory for sync operations. |
| `AsyncOperationFactory<TOperation, TRequest>` | DI factory for async operations. |

---

## Usage

### Synchronous operation

Implement `IOperation<TRequest>` for operations that don't return a value.

```cs
// ../../tests/Olve.Operations.Tests/ReadmeDemo.cs#L67-L70

private class GreetOperation : IOperation<string>
{
    public Result Execute(string request) => Result.Success();
}
```

```cs
// ../../tests/Olve.Operations.Tests/ReadmeDemo.cs#L11-L15

var op = new GreetOperation();

var result = op.Execute("Alice");

await Assert.That(result.Succeeded).IsTrue();
```

---

### Synchronous operation with return value

Implement `IOperation<TRequest, TResult>` when the operation produces a typed result.

```cs
// ../../tests/Olve.Operations.Tests/ReadmeDemo.cs#L72-L78

private class DoubleOperation : IOperation<int, int>
{
    public Result<int> Execute(int request) => request * 2;

    public class Factory(IServiceProvider sp)
        : OperationFactory<DoubleOperation, int, int>(sp);
}
```

```cs
// ../../tests/Olve.Operations.Tests/ReadmeDemo.cs#L21-L26

var op = new DoubleOperation();

var result = op.Execute(21);

await Assert.That(result.Succeeded).IsTrue();
await Assert.That(result.Value).IsEqualTo(42);
```

---

### Async operation

Implement `IAsyncOperation<TRequest>` for async work. All async variants accept an optional `CancellationToken`.

```cs
// ../../tests/Olve.Operations.Tests/ReadmeDemo.cs#L80-L87

private class SlowGreetOperation : IAsyncOperation<string>
{
    public async Task<Result> ExecuteAsync(string request, CancellationToken ct = default)
    {
        await Task.Delay(1, ct);
        return Result.Success();
    }
}
```

```cs
// ../../tests/Olve.Operations.Tests/ReadmeDemo.cs#L32-L36

var op = new SlowGreetOperation();

var result = await op.ExecuteAsync("Bob");

await Assert.That(result.Succeeded).IsTrue();
```

---

### Async operation with return value

`IAsyncOperation<TRequest, TResult>` combines async execution with a typed result.

```cs
// ../../tests/Olve.Operations.Tests/ReadmeDemo.cs#L89-L96

private class AsyncDoubleOperation : IAsyncOperation<int, int>
{
    public async Task<Result<int>> ExecuteAsync(int request, CancellationToken ct = default)
    {
        await Task.Delay(1, ct);
        return request * 2;
    }
}
```

```cs
// ../../tests/Olve.Operations.Tests/ReadmeDemo.cs#L42-L47

var op = new AsyncDoubleOperation();

var result = await op.ExecuteAsync(21);

await Assert.That(result.Succeeded).IsTrue();
await Assert.That(result.Value).IsEqualTo(42);
```

---

### Factory pattern with DI

Define a nested `Factory` class inheriting from `OperationFactory` (or `AsyncOperationFactory`). Register both the operation and factory in your DI container, then call `Build()` to resolve instances.

```cs
// ../../tests/Olve.Operations.Tests/ReadmeDemo.cs#L53-L62

var services = new ServiceCollection();
services.AddTransient<DoubleOperation>();
services.AddTransient<DoubleOperation.Factory>();
using var sp = services.BuildServiceProvider();

var factory = sp.GetRequiredService<DoubleOperation.Factory>();
var op = factory.Build();
var result = op.Execute(21);

await Assert.That(result.Value).IsEqualTo(42);
```

---

## Documentation

Full API reference:
[https://olivervea.github.io/Olve.Utilities/api/Olve.Operations.html](https://olivervea.github.io/Olve.Utilities/api/Olve.Operations.html)

---

## License

MIT License © [OliverVea](https://github.com/OliverVea)
