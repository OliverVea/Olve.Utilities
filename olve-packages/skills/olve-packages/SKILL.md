---
name: olve-packages
description: >-
  Overview of Olve.* shared NuGet packages — what packages exist, their purpose, and how they relate to each other.
  Use when the user asks about available Olve packages or needs help choosing which package to use.
user-invocable: false
---

# Olve Packages

Shared .NET NuGet packages published from [Olve.Utilities](https://github.com/OliverVea/Olve.Utilities). These are building-block libraries used across multiple Olve.* applications (e.g. Olve.Trains, Olve.Pipelines). This is not a complete list of all Olve.* packages — other repositories publish their own.

API docs: https://olivervea.github.io/Olve.Utilities/

## Available Packages

| Package | Purpose |
|---------|---------|
| **Olve.Results** | Functional `Result`/`Result<T>` types for non-throwing error handling |
| **Olve.Results.TUnit** | TUnit assertion extensions for Result types |
| **Olve.Paths** | Cross-platform path manipulation inspired by Python's pathlib |
| **Olve.Paths.Glob** | Glob pattern matching extension for Olve.Paths |
| **Olve.Validation** | Fluent validation framework returning `Result` |
| **Olve.Utilities** | Meta-package: typed IDs, collections, graphs, pagination, datetime formatting |
| **Olve.MinimalApi** | ASP.NET Minimal API extensions for Result mapping and validation filters |
| **Olve.OpenRaster** | Read-only access to OpenRaster (.ora) layered image files |
| **Olve.TinyEXR** | P/Invoke bindings for tinyexr OpenEXR library |

## Dependency Graph

```
Olve.Utilities (meta-package)
├── Olve.Results
├── Olve.Paths
│   └── Olve.Paths.Glob (extension)
└── Olve.Validation (uses Olve.Results)

Olve.MinimalApi
├── Olve.Results
├── Olve.Validation
└── Olve.Paths

Olve.Results.TUnit
└── Olve.Results

Olve.OpenRaster
└── Olve.Results

Olve.TinyEXR (standalone, native interop)
```

## Installation

All packages are available on NuGet:

```bash
dotnet add package Olve.Results
dotnet add package Olve.Utilities   # includes Results, Paths, Validation
```

## Per-Package Reference

Each package has its own skill with full API reference. Use the skill name to access detailed documentation:

- `olve-results` — Result types, Chain, Concat, Map, Bind, error propagation
- `olve-results-tunit` — TUnit assertions for Result types
- `olve-paths` — IPath, IPurePath, filesystem operations
- `olve-paths-glob` — TryGlob extension method
- `olve-validation` — Fluent validators (string, int, decimal, enumerable)
- `olve-utilities` — Id\<T\>, collections, graphs, pagination
- `olve-minimal-api` — Result-to-HTTP mapping, validation filters
- `olve-openraster` — OpenRaster file reading
- `olve-tinyexr` — EXR image loading/saving
