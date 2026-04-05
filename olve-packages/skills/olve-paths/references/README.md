# Olve.Paths

Source: https://olivervea.github.io/Olve.Utilities/src/Olve.Paths/README.html

A .NET library for working with file and directory paths inspired by Python's `pathlib` module. Provides an object-oriented interface for manipulating paths instead of raw strings.

## Key Types

- **IPurePath** -- Immutable, pure path independent of the filesystem. Platform-aware but does not touch disk.
- **IPath** -- Extends IPurePath with environment-aware filesystem operations (existence, children, element type).
- **Path** -- Static factory class for creating IPurePath and IPath instances.
- **PathPlatform** -- Platform discriminator: `Unix` or `Windows`.
- **PathType** -- Path classification: `Absolute`, `Relative`, or `Stub`.
- **ElementType** -- Filesystem element type: `File`, `Directory`, or `None`.

## Primary Usage Patterns

**Path navigation:** Create paths with `Path.Create()` or `Path.CreatePure()`, navigate with `Parent`, `Name`, and the `/` operator for joining segments.

**Pure manipulation:** `Path.CreatePure()` with an explicit `PathPlatform` creates paths that work on any host OS without filesystem access.

**Filesystem queries:** `IPath` supports `Exists()`, `ElementType`, `TryGetChildren()`, `TryGetAbsolute()`, and `EnsurePathExists()`.

**Temporary paths:** `Path.GetTempDirectory()`, `Path.CreateTempDirectory()`, and `Path.CreateTempFile()` wrap .NET temp APIs and return `IPath`.

## Design Philosophy

Separation between pure path manipulation (no side effects, cross-platform) and environment-aware paths (filesystem access). The `/` operator provides intuitive path joining. All implementations are internal; consumers work through `IPurePath` and `IPath` interfaces. Platform detection is automatic when no `PathPlatform` is specified. Stub paths (bare names like `file.txt`) can be appended; relative paths (starting with `.` or `..`) are resolved during concatenation.
