# Olve.Utilities
[![NuGet](https://img.shields.io/nuget/v/Olve.Utilities?logo=nuget)](https://www.nuget.org/packages/Olve.Utilities)

Miscellaneous helpers such as collection extensions, ID generators and startup utilities.

## Installation

```bash
dotnet add package Olve.Utilities
```

## Usage

```csharp
var generator = new ThreadSafeUintGenerator();
var id = generator.Next();
Console.WriteLine(id);
```

See the [documentation](https://olivervea.github.io/Olve.Utilities/) for more details.
