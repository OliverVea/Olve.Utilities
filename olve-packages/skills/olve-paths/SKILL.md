---
name: olve-paths
description: Reference for Olve.Paths — cross-platform path manipulation with IPurePath, IPath, Path factory, and filesystem operations. Use when writing or reading code that uses Olve.Paths types.
user-invocable: false
---

# Olve.Paths

Cross-platform path manipulation inspired by Python's `pathlib`. Source: `Olve.Utilities/src/Olve.Paths/`.

Reference docs: [README](references/README.md) | [IPurePath](references/IPurePath.md) | [IPath](references/IPath.md) | [Path](references/Path.md) | [Enums](references/Enums.md) | [Extensions](references/Extensions.md)

**Key principle:** Use `IPurePath` for platform-independent path manipulation without filesystem access. Use `IPath` when you need filesystem operations (existence, children, element type).

## Path Creation

```csharp
// Environment-aware path (uses current OS)
var path = Path.Create("/home/user/documents");

// Pure path (no filesystem, works on any OS)
var pure = Path.CreatePure("/home/user/docs", PathPlatform.Unix);

// Temporary paths
var tempDir = Path.CreateTempDirectory("my-app-");
var tempFile = Path.CreateTempFile();
```

## Path Navigation

```csharp
var path = Path.Create("/home/user/documents");

var parent = path.Parent;           // /home/user
var name = path.Name;               // documents
var child = path / "newfile.txt";   // /home/user/documents/newfile.txt
```

## Filesystem Operations (IPath only)

```csharp
var exists = path.Exists();
var elementType = path.ElementType;  // File, Directory, or None

if (path.TryGetChildren(out var children))
{
    foreach (var child in children) { /* ... */ }
}

path.EnsurePathExists();  // creates directory if missing
```

## Pure Path Platform Targeting

```csharp
var unix = Path.CreatePure("/home/user", PathPlatform.Unix);
var win = Path.CreatePure(@"C:\users\user", PathPlatform.Windows);

unix.Platform;  // PathPlatform.Unix
win.Platform;   // PathPlatform.Windows
```
