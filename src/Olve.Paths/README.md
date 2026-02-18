# Olve.Paths
[![NuGet](https://img.shields.io/nuget/v/Olve.Paths?logo=nuget)](https://www.nuget.org/packages/Olve.Paths)[![GitHub](https://img.shields.io/github/license/OliverVea/Olve.Utilities)](LICENSE)![LOC](https://img.shields.io/endpoint?url=https%3A%2F%2Fghloc.vercel.app%2Fapi%2FOliverVea%2FOlve.Paths%2Fbadge)![NuGet Downloads](https://img.shields.io/nuget/dt/Olve.Paths)

A .NET library for working with file and directory paths inspired by Python's `pathlib` module.

It provides a set of classes and methods to manipulate file paths in a more intuitive and object-oriented way.

## Features

- Object-oriented interface for file and directory paths.
- Easy navigation and manipulation of paths.
  - Supports the `/` operator for path joining.
- Support of different path formats.
  - Currently, only Unix-style paths are supported.

See [`Olve.Paths.Glob`](https://github.com/OliverVea/Olve.Utilities/tree/master/src/Olve.Paths.Glob) for more information about the globbing functionality.

## Installation

Simply install the package via NuGet:

```bash
dotnet add package Olve.Paths
```

## Usage

```cs
// ../../tests/Olve.Paths.Tests/ReadmeDemo.cs#L18-L24

var path = Path.Create("/home/user/documents"); // /home/user/documents

var parent = path.Parent; // /home/user
var folderName = path.Name; // documents

var newPath = path / "newfile.txt"; // /home/user/documents/newfile.txt
var exists = newPath.Exists(); // Check if the file exists
```

See the [API documentation](https://olivervea.github.io/Olve.Utilities/api/) for more details.
