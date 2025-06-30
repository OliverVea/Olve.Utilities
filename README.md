# Olve.Utilities

[![NuGet](https://img.shields.io/nuget/v/Olve.Utilities?logo=nuget)](https://www.nuget.org/packages/Olve.Utilities)
[![GitHub](https://img.shields.io/github/license/OliverVea/Olve.Utilities)](LICENSE)
![LOC](https://img.shields.io/endpoint?url=https%3A%2F%2Fghloc.vercel.app%2Fapi%2FOliverVea%2FOlve.Utilities%2Fbadge)
![NuGet Downloads](https://img.shields.io/nuget/dt/Olve.Utilities)

**Olve.Utilities** is a collection of small helpers and abstractions for .NET projects.
It bundles several packages that can be consumed separately via NuGet.

## Packages

- **Olve.Utilities** – general utilities such as assertions, path helpers, collections and date helpers.
- **Olve.Paths** – a pathlib inspired API for manipulating file and directory paths.
- **Olve.Paths.Glob** – globbing extensions for `Olve.Paths` supporting `*` and `**` patterns.
- **Olve.Results** – a lightweight result type for propagating successes and failures.
- **Olve.Results.TUnit** – assertion helpers for `Result` values when testing with TUnit.
- **Olve.Operations** – abstractions for defining synchronous/asynchronous operations with dependency injection support.
- **Olve.SG.CopyProperties** – a Roslyn source generator for creating types by copying properties from other types.

The [`docs`](https://olivervea.github.io/Olve.Utilities/) site contains API documentation and examples for these packages.

## Repository Layout

- `src/` contains all library projects and source generators.
- `tests/` holds unit tests for each package.
- `docs/` hosts the documentation site generated with DocFX.

## Contributing

This is a personal project and unsolicited contributions are discouraged.
If you have questions or encounter issues, please open an issue instead of sending a pull request.

## License

MIT – see the [LICENSE](LICENSE) file for details.
