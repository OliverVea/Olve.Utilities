# Olve.Paths

[![NuGet](https://img.shields.io/nuget/v/Olve.Paths?logo=nuget)](https://www.nuget.org/packages/Olve.Paths)
[![Docs](https://img.shields.io/badge/docs-API%20Reference-blue)](https://olivervea.github.io/Olve.Utilities/api/Olve.Paths.html)

A .NET library for working with file and directory paths inspired by Python's `pathlib` module. It provides an object-oriented interface for manipulating paths in a more intuitive way than raw strings.

---

## Installation

```bash
dotnet add package Olve.Paths
```

---

## Overview

| Type | Description |
| --- | --- |
| `IPath` | Environment-aware path with filesystem operations (existence checks, children, element type). |
| `IPurePath` | Immutable, pure path independent of the filesystem. Useful for manipulation without side effects. |
| `PathPlatform` | Platform discriminator: `Unix` or `Windows`. |
| `ElementType` | Filesystem element type: `File`, `Directory`, or `None`. |
| `PathType` | Path classification: `Absolute`, `Relative`, or `Stub`. |

---

## Usage

### Path navigation

Create a path with `Path.Create()` and navigate using `Parent`, `Name`, and the `/` operator for joining.

```cs
// ../../tests/Olve.Paths.Tests/ReadmeDemo.cs#L8-L14

var path = Path.Create("/home/user/documents"); // /home/user/documents

var parent = path.Parent; // /home/user
var folderName = path.Name; // documents

var newPath = path / "newfile.txt"; // /home/user/documents/newfile.txt
var exists = newPath.Exists(); // Check if the file exists
```

On Windows the same API applies with backslash-separated paths:

```cs
// ../../tests/Olve.Paths.Tests/ReadmeDemo.cs#L28-L34

var path = Path.Create(@"C:\users\user\documents"); // C:\users\user\documents

var parent = path.Parent; // C:\users\user
var folderName = path.Name; // documents

var newPath = path / "newfile.txt"; // C:\users\user\documents\newfile.txt
var exists = newPath.Exists(); // Check if the file exists
```

### Pure paths

`Path.CreatePure()` creates immutable paths that work on any platform without touching the filesystem. Pass a `PathPlatform` to create paths for a specific platform regardless of the host OS.

```cs
// ../../tests/Olve.Paths.Tests/ReadmeDemo.cs#L48-L53

// Pure paths allow platform-independent path manipulation
var unixPath = Path.CreatePure("/home/user/docs", PathPlatform.Unix);
var windowsPath = Path.CreatePure(@"C:\users\user\docs", PathPlatform.Windows);

var unixPlatform = unixPath.Platform; // PathPlatform.Unix
var windowsPlatform = windowsPath.Platform; // PathPlatform.Windows
```

### Filesystem operations

`IPath` provides methods for querying and manipulating the filesystem: `Exists()`, `ElementType`, `TryGetChildren()`, and `EnsurePathExists()`.

```cs
// ../../tests/Olve.Paths.Tests/ReadmeDemo.cs#L66-L78

var tempDir = Path.Create(System.IO.Path.GetTempPath()) / "olve-paths-demo";
tempDir.EnsurePathExists(); // Creates the directory if it doesn't exist

var exists = tempDir.Exists(); // true
var elementType = tempDir.ElementType; // ElementType.Directory

if (tempDir.TryGetChildren(out var children))
{
    foreach (var child in children)
    {
        Console.WriteLine(child.Path);
    }
}
```

---

## Globbing

See [`Olve.Paths.Glob`](https://github.com/OliverVea/Olve.Utilities/tree/master/src/Olve.Paths.Glob) for wildcard pattern matching on paths.

---

## Documentation

Full API reference: [https://olivervea.github.io/Olve.Utilities/api/Olve.Paths.html](https://olivervea.github.io/Olve.Utilities/api/Olve.Paths.html)

---

## License

MIT License © [OliverVea](https://github.com/OliverVea)
