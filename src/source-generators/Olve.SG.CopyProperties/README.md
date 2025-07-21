# Olve.SG.CopyProperties
[![NuGet](https://img.shields.io/nuget/v/Olve.SG.CopyProperties?logo=nuget)](https://www.nuget.org/packages/Olve.SG.CopyProperties)

Source generator that copies properties from a source type to a target declaration using a `[CopyProperties]` attribute.

## Installation

```bash
dotnet add package Olve.SG.CopyProperties
```

## Usage

```csharp
[CopyProperties(typeof(Source))]
public partial class Target { }
```

See the [API documentation](https://olivervea.github.io/Olve.Utilities/api/) for more details.
