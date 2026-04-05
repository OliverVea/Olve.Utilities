# Olve.TinyEXR — Overview

API docs: [https://olivervea.github.io/Olve.Utilities/api/Olve.TinyEXR.html](https://olivervea.github.io/Olve.Utilities/api/Olve.TinyEXR.html)

.NET P/Invoke bindings for [tinyexr](https://github.com/syoyo/tinyexr), a small library for reading and writing OpenEXR images.

## Installation

```bash
dotnet add package Olve.TinyEXR
```

## Public API surface

| Type | Description |
| --- | --- |
| `TinyExr` | Static class with safe managed wrappers for loading, saving, and validating EXR images. |
| `TinyExrException` | Exception thrown on native library errors, carrying the error code and message. |
| `EXRVersion` | Parsed EXR version header struct (version number, tiled, multipart flags). |

## Platform support

Prebuilt native libraries are included for:

- **win-x64** — `runtimes/win-x64/native/tinyexr.dll`
- **linux-x64** — `runtimes/linux-x64/native/libtinyexr.so`

Cross-platform builds for osx-x64 and osx-arm64 are planned.

## Unsafe context requirement

All methods on `TinyExr` require an `unsafe` context because they work with native pointers (`float*`, `byte*`). The project or calling code must enable `<AllowUnsafeBlocks>true</AllowUnsafeBlocks>`.

## Native library resolution

The `NativeMethods` class includes a custom `DllImportResolver` that looks for the native library in the `runtimes/{rid}/native/` directory next to the assembly. If not found there, it falls back to default OS resolution.
