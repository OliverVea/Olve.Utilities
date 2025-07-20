# Repository guidelines

- Central package versions are defined in `Directory.Packages.props`.
- Projects using builders reference the `BuilderGenerator` package version `3.1.0`.
- Each project exposes `workflow-triggers.txt` to control NuGet publishing paths.
- The solution includes `Olve.Results.Validation` for validation helpers.
