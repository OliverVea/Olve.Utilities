# Olve.Paths.Glob
[![NuGet](https://img.shields.io/nuget/v/Olve.Paths.Glob?logo=nuget)](https://www.nuget.org/packages/Olve.Paths.Glob)[![GitHub](https://img.shields.io/github/license/OliverVea/Olve.Utilities)](LICENSE)![LOC](https://img.shields.io/endpoint?url=https%3A%2F%2Fghloc.vercel.app%2Fapi%2FOliverVea%2FOlve.Paths.Glob%2Fbadge)![NuGet Downloads](https://img.shields.io/nuget/dt/Olve.Paths.Glob)

Globbing extension for `Olve.Paths` library.

## Features

- Support for Unix-style globbing patterns, including wildcards `*` and `**`.

## Installation

Simply install the package via NuGet:

```bash
dotnet add package Olve.Paths.Glob
```

## Usage

```csharp
using Olve.Paths;

var path = Path.Create("/home/user/documents");
var glob = path.Glob("*.txt"); // Matches all .txt files in the directory
var allFiles = path.Glob("**/*"); // Matches all files in the directory and subdirectories
```

See the [API documentation](https://olivervea.github.io/Olve.Utilities/api/) for more details.
