# Repository guidelines

- Central package versions are defined in `Directory.Packages.props`.
- Projects using builders reference the `BuilderGenerator` package. Version `3.0.5` is listed centrally.
- `Microsoft.CodeAnalysis.CSharp` uses version `4.13.0` to match the SDK.
- Analyzer packages (`Microsoft.CodeAnalysis.*Analyzers`) use version `4.14.0`.
- The compiler version is locked by `Microsoft.Net.Compilers.Toolset` `4.13.0`.
- README files from `src/**/README.md` are included in the documentation via DocFX `build.content`.
- `Olve.Results.Validation` is part of the DocFX metadata `src` list. Add new projects there when needed.
- `Olve.Validation.SourceGeneration` is documented via DocFX to cover the generator.
- `Olve.Validation.SourceGeneration` includes `Olve.Validation.SourceGenerators` as a transitive analyzer so consumers get the generator automatically.
- Run `dotnet docfx metadata docs/docfx.json` when source projects change to keep YAML up to date.
- Keep this file updated as packages or build tooling change.
- Version bump commits on `master` are automatically pushed to `develop` via the workflow.
- To run tests: `dotnet test --verbosity normal --logger "console;verbosity=minimal"`

## Examples of XML Documentation in Source Code

Below are some representative examples of XML documentation comments found in the C# source, with links to their origin files.

### 1. Interface Documentation Example ([src/Olve.Paths/IPath.cs](src/Olve.Paths/IPath.cs))

```csharp
/// <summary>
/// Represents a path that includes contextual information about its environment, such as its element type and resolved state.
/// </summary>
public interface IPath : IPurePath
{
    /// <summary>
    /// Attempts to retrieve the type of the element represented by the path.
    /// </summary>
    /// <param name="type">When this method returns, contains the type of the element if available; otherwise, <c>ElementType.None</c>.</param>
    /// <returns><c>true</c> if the element type was successfully retrieved; otherwise, <c>false</c>.</returns>
    bool TryGetElementType(out ElementType type);

    /// <summary>
    /// Gets the type of the element represented by the path.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the element type cannot be determined.</exception>
    ElementType ElementType => TryGetElementType(out var type) ? type : throw new InvalidOperationException("Element does not have a type");
    // ... (other members omitted for brevity)
}
```

---

### 2. Method Documentation Example ([src/Olve.Results.Validation/StringValidator.cs](src/Olve.Results.Validation/StringValidator.cs))

```csharp
/// <summary>
/// Fails when the string length is shorter than <paramref name="minLength"/>.
/// </summary>
/// <param name="minLength">Minimum length allowed.</param>
/// <returns>The current validator.</returns>
public StringValidator MinLength(int minLength) =>
    FailIfValue(v => v?.Length < minLength, () => new ResultProblem("Value must be at least '{0}' characters", minLength));
```

---

### 3. Static Method with Exception Documentation ([src/Olve.Paths/Path.cs](src/Olve.Paths/Path.cs))

```csharp
/// <summary>
/// Creates a platform-specific pure path from the given string and platform.
/// </summary>
/// <param name="path">The string path to convert.</param>
/// <param name="platform">The platform type to use for path interpretation.</param>
/// <returns>An instance of <see cref="IPurePath"/>.</returns>
/// <exception cref="ArgumentNullException">Thrown when the platform is <see cref="PathPlatform.None"/>.</exception>
public static IPurePath CreatePure(string path, PathPlatform platform)
{
    path = path.Trim();

    return platform switch
    {
        PathPlatform.Unix => CreatePureUnixPath(path),
        PathPlatform.Windows => CreatePureWindowsPath(path),
        _ => new UnsupportedPurePath()
    };
}
```

---

These examples illustrate the level and style of XML documentation present in the codebase, including summaries, parameter descriptions, return values, and exception documentation.

---

## Testing Standards

To ensure high-quality, maintainable, and effective tests across the repository, adhere to the following testing standards:

1. **Clarity and Readability**  
   Write tests that are easy to read and understand. Use descriptive test names, clear assertions, and avoid unnecessary complexity. Anyone should be able to grasp what a test does at a glance.

2. **Maintainability**  
   Structure tests to be easy to update as the codebase evolves. Avoid duplication, use helper methods or fixtures where appropriate, and keep test logic DRY (Don't Repeat Yourself).

3. **Test One Thing**  
   Each test should verify a single behavior or concept. This makes failures easier to diagnose and helps keep tests focused and readable.

4. **Parameterization**  
   Use parameterized tests to cover multiple input scenarios efficiently, reducing code duplication and improving coverage.

5. **Minimalism**  
   Write as few tests as necessary to cover the intended behaviors. Avoid redundant or overlapping tests.

6. **Isolation**  
   Tests should be independent and not rely on the state or outcome of other tests. Use mocks, stubs, or test doubles judiciously to isolate units under test.

7. **Determinism**  
   Ensure tests are deterministic and produce the same result every run. Avoid dependencies on external systems, time, or random values unless explicitly controlled.

8. **Descriptive Failures**  
   Write assertions and error messages that make it clear why a test failed, aiding in quick diagnosis and resolution.

9. **Document Intent**  
   When a test's purpose is not obvious, add comments to explain the intent, especially for edge cases or non-trivial scenarios.

10. **Continuous Improvement**  
    Regularly review and refactor tests as the codebase changes. Strive to improve coverage, clarity, and maintainability over time.

By following these standards, we aim to keep our test suite robust, reliable, and easy to work with as the project grows.
