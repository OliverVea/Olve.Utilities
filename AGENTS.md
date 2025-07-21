# Repository guidelines

- Central package versions are defined in `Directory.Packages.props`.
- Projects using builders reference the `BuilderGenerator` package. Version `3.1.0` is listed centrally.
- `Microsoft.CodeAnalysis.CSharp` uses version `4.13.0` to match the SDK.
- Analyzer packages (`Microsoft.CodeAnalysis.*Analyzers`) use version `4.14.0`.
- The compiler version is locked by `Microsoft.Net.Compilers.Toolset` `4.13.0`.
- Keep this file updated as packages or build tooling change.
