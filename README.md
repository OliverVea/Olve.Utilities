# Olve.Utilities

[![GitHub](https://img.shields.io/github/license/OliverVea/Olve.Utilities)](LICENSE)

A collection of small, focused .NET libraries for common tasks: result types, typed IDs, collections, path manipulation, validation, and more.

Full API documentation: [olivervea.github.io/Olve.Utilities](https://olivervea.github.io/Olve.Utilities/)

---

## Packages

| Package | NuGet | Description |
| --- | --- | --- |
| [Olve.Utilities](src/Olve.Utilities) | [![NuGet](https://img.shields.io/nuget/v/Olve.Utilities?logo=nuget)](https://www.nuget.org/packages/Olve.Utilities) | Typed IDs, specialized collections, directed graphs, pagination, datetime formatting, and more. Meta-package that includes Olve.Results, Olve.Paths, and Olve.Validation. |
| [Olve.Results](src/Olve.Results) | [![NuGet](https://img.shields.io/nuget/v/Olve.Results?logo=nuget)](https://www.nuget.org/packages/Olve.Results) | Lightweight functional result types for non-throwing error handling. |
| [Olve.Results.TUnit](src/Olve.Results.TUnit) | [![NuGet](https://img.shields.io/nuget/v/Olve.Results.TUnit?logo=nuget)](https://www.nuget.org/packages/Olve.Results.TUnit) | TUnit assertion extensions for `Result` types. |
| [Olve.Paths](src/Olve.Paths) | [![NuGet](https://img.shields.io/nuget/v/Olve.Paths?logo=nuget)](https://www.nuget.org/packages/Olve.Paths) | Cross-platform path manipulation for Unix and Windows. |
| [Olve.Paths.Glob](src/Olve.Paths.Glob) | [![NuGet](https://img.shields.io/nuget/v/Olve.Paths.Glob?logo=nuget)](https://www.nuget.org/packages/Olve.Paths.Glob) | Glob pattern matching for Olve.Paths. |
| [Olve.Validation](src/Olve.Validation) | [![NuGet](https://img.shields.io/nuget/v/Olve.Validation?logo=nuget)](https://www.nuget.org/packages/Olve.Validation) | Fluent validation helpers built on Olve.Results. |
| [Olve.MinimalApi](src/Olve.MinimalApi) | [![NuGet](https://img.shields.io/nuget/v/Olve.MinimalApi?logo=nuget)](https://www.nuget.org/packages/Olve.MinimalApi) | Conventions for ASP.NET Minimal API endpoints. |
| [Olve.OpenRaster](src/Olve.OpenRaster) | [![NuGet](https://img.shields.io/nuget/v/Olve.OpenRaster?logo=nuget)](https://www.nuget.org/packages/Olve.OpenRaster) | Read-only access to OpenRaster (`.ora`) image files. |

### Deprecated

| Package | Description |
| --- | --- |
| [Olve.Operations](src/Olve.Operations) | Operation abstractions — use plain interfaces or delegates instead. |
| [Olve.Logging](src/Olve.Logging) | In-memory logging — use `Microsoft.Extensions.Logging` instead. |

---

## License

MIT License © [OliverVea](https://github.com/OliverVea)
