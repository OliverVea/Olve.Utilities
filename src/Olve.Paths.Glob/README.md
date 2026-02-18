# Olve.Paths.Glob

[![NuGet](https://img.shields.io/nuget/v/Olve.Paths.Glob?logo=nuget)](https://www.nuget.org/packages/Olve.Paths.Glob)
[![Docs](https://img.shields.io/badge/docs-API%20Reference-blue)](https://olivervea.github.io/Olve.Utilities/api/Olve.Paths.Glob.html)

Glob pattern matching extension for [Olve.Paths](https://www.nuget.org/packages/Olve.Paths). Adds a `TryGlob` extension method to `IPath` for finding files using Unix-style wildcard patterns.

---

## Installation

```bash
dotnet add package Olve.Paths.Glob
```

---

## Usage

`TryGlob` follows the Try-pattern — it returns `true` when the path is absolute and matches are found:

```cs
// ../../tests/Olve.Paths.Tests/GlobReadmeDemo.cs#L18-L22

if (path.TryGlob("*.txt", out var matches))
{
    foreach (var match in matches)
        Console.WriteLine(match.Path);
}
```

### Recursive patterns

Use `**` to match across directory boundaries:

```cs
// ../../tests/Olve.Paths.Tests/GlobReadmeDemo.cs#L33-L34

// All .txt files in any subdirectory
path.TryGlob("**/*.txt", out var sourceFiles);
```

### Case-insensitive matching

Pass `ignoreCase: true` for case-insensitive pattern matching:

```cs
// ../../tests/Olve.Paths.Tests/GlobReadmeDemo.cs#L45-L45

path.TryGlob("*.TXT", out var matches, ignoreCase: true);
```

### Hidden file exclusion

Files and directories starting with `.` are automatically excluded from all glob results. This is not configurable.

---

## API Reference

### `PathExtensions.TryGlob`

```csharp
public static bool TryGlob(
    this IPath path,
    string pattern,
    out IEnumerable<IPath>? matches,
    bool ignoreCase = false)
```

| Parameter | Description |
| --- | --- |
| `path` | The root directory to search within. Must be absolute. |
| `pattern` | Glob pattern (`*` matches within a directory, `**` matches across directories). |
| `matches` | Matched file paths, or `null` if the path is not absolute. |
| `ignoreCase` | When `true`, uses case-insensitive matching. Default: `false`. |

Returns `true` when the path is absolute and matches were found.

---

## Documentation

Full API reference:
[https://olivervea.github.io/Olve.Utilities/api/Olve.Paths.Glob.html](https://olivervea.github.io/Olve.Utilities/api/Olve.Paths.Glob.html)

---

## License

MIT License © [OliverVea](https://github.com/OliverVea)
