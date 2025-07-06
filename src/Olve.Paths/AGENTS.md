# Guidelines for Olve.Paths

This file documents the design and usage of the **Olve.Paths** project. Keep it up to date when the API changes.

## Purpose

`Olve.Paths` offers an object oriented approach to working with file system paths inspired by Python's `pathlib`. It abstracts platform differences and allows easy manipulation and inspection of paths.

## Core Types

- **Path** – static helper for creating `IPurePath` and `IPath` instances. Chooses Unix or Windows implementations based on `OperatingSystem` or `PathPlatform`.
- **IPurePath** – immutable representation of a path independent of the file system. Supports appending segments (`/` operator) and inspection of name, parent and platform.
- **IPath** – extends `IPurePath` with access to the environment. Provides element type, absolute paths, children enumeration and existence checks.
- **IPathEnvironment** – abstraction for environment dependent operations such as retrieving the current directory or executable path. `DefaultUnixPathEnvironment.Shared` is provided.
- **WindowsPath/UnixPath** – platform specific implementations of `IPath`.
- **WindowsPurePath/UnixPurePath** – platform specific implementations of `IPurePath`.
- **PathType** – enum describing whether a path is `Stub`, `Relative` or `Absolute`.
- **PathPlatform** – enum describing the underlying platform (`Windows` or `Unix`).
- **ElementType** – enum describing whether a path refers to a `File`, `Directory` or `None`.

## Usage Notes

1. Use `Path.Create()` to obtain an `IPath` resolved to the current platform. `CreatePure()` returns an `IPurePath` without environment binding.
2. Paths can be combined using the `/` operator or the `Append`/`AppendPath` methods.
3. `TryGetParent`, `TryGetChildren`, `TryGetAbsolute` and related helpers return success flags rather than throwing on failure.
4. `GetLinkString()` builds a clickable URL string pointing to the path, optionally with line and column numbers. It is used by `Olve.Results` problem origins.
5. When implementing new path environments or platforms, ensure they implement `IPathEnvironment` and update this document.

## Maintenance

- When adding or changing path-related APIs, update this document.
- `workflow-triggers.txt` lists paths that trigger NuGet publishing when changed.
