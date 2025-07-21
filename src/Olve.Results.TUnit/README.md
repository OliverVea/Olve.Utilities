# Olve.Results.TUnit
[![NuGet](https://img.shields.io/nuget/v/Olve.Results.TUnit?logo=nuget)](https://www.nuget.org/packages/Olve.Results.TUnit)

TUnit assertion extensions for verifying `Result` values in unit tests.

## Installation

```bash
dotnet add package Olve.Results.TUnit
```

## Usage

```csharp
await Assert.That(myResult).Succeeded();
await Assert.That(myResult).SucceededAndValue().IsNotNull();
await Assert.That(myResult).Failed();
await Assert.That(myResult).FailedAndProblemCollection().IsNotEmpty();
```

See the [API documentation](https://olivervea.github.io/Olve.Utilities/api/) for more details.
