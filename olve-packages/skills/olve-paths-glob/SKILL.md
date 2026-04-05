---
name: olve-paths-glob
description: Glob pattern matching for Olve.Paths — find files matching wildcard patterns within an IPath directory using the TryGlob extension method.
user-invocable: false
---

# Olve.Paths.Glob

Glob pattern matching for `IPath` directories. Source: `Olve.Utilities/src/Olve.Paths.Glob/`.

## API

Single extension method in `Olve.Paths.Glob.PathExtensions`:

```csharp
public static bool TryGlob(
    this IPath path,
    string pattern,
    [NotNullWhen(true)] out IEnumerable<IPath>? matches,
    bool ignoreCase = false)
```

### Parameters

| Parameter | Type | Description |
|---|---|---|
| `path` | `IPath` | Root directory to search within. **Must be absolute** — returns `false` if the path is not absolute. |
| `pattern` | `string` | Glob pattern to match files against. |
| `matches` | `out IEnumerable<IPath>?` | Matched file paths, each relative to `path` (combined via `path / match`). `null` when the method returns `false`. |
| `ignoreCase` | `bool` | Case-insensitive matching when `true`. Default: `false`. |

### Returns

`true` if `path` was absolute and matching succeeded (even if zero files matched). `false` if `path` could not be resolved to an absolute path.

## Pattern Syntax

Uses `Microsoft.Extensions.FileSystemGlobbing.Matcher` under the hood.

| Pattern | Matches |
|---|---|
| `*.txt` | All `.txt` files in the root directory |
| `**/*.txt` | All `.txt` files in any subdirectory (recursive) |
| `src/**/*.cs` | All `.cs` files under `src/` at any depth |
| `docs/*.md` | All `.md` files directly in `docs/` |

**Hidden file exclusion:** The matcher automatically excludes hidden files and directories (anything matching `**/.*`). This is always applied and cannot be overridden.

## Absolute Path Requirement

The `path` must resolve to an absolute path via `IPath.TryGetAbsolute`. If the path is relative, `TryGlob` returns `false` and `matches` is `null`. Always use absolute `IPath` instances as the root.

## Usage

```csharp
using Olve.Paths.Glob;

IPath directory = ...; // must be absolute

if (directory.TryGlob("**/*.cs", out var matches))
{
    foreach (var match in matches)
    {
        // match is an IPath: directory / relative match path
    }
}

// Case-insensitive matching
directory.TryGlob("**/*.TXT", out var files, ignoreCase: true);
```
