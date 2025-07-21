# Olve.Operations
[![NuGet](https://img.shields.io/nuget/v/Olve.Operations?logo=nuget)](https://www.nuget.org/packages/Olve.Operations)

Abstractions for implementing operations that return `Result` objects. Provides factories
for creating operations through dependency injection.

## Installation

```bash
dotnet add package Olve.Operations
```

## Usage

```csharp
public class BuyItemOperation : IAsyncOperation<BuyItemOperation.Request>
{
    public record Request(string SkuId, int Quantity, float Price);

    public async ValueTask<Result> ExecuteAsync(Request request, CancellationToken ct = default)
    {
        // perform work
        return Result.Success();
    }
}

var op = new BuyItemOperation();
var result = await op.ExecuteAsync(new BuyItemOperation.Request("ABC123", 2, 9.99f));
if (result.Succeeded)
{
    // handle success
}
```

`IOperation<TRequest>` is available for synchronous implementations.

See the [API documentation](https://olivervea.github.io/Olve.Utilities/api/) for more details.
