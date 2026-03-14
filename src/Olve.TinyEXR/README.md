# Olve.TinyEXR

[![NuGet](https://img.shields.io/nuget/v/Olve.TinyEXR?logo=nuget)](https://www.nuget.org/packages/Olve.TinyEXR)
[![Docs](https://img.shields.io/badge/docs-API%20Reference-blue)](https://olivervea.github.io/Olve.Utilities/api/Olve.TinyEXR.html)

.NET P/Invoke bindings for [tinyexr](https://github.com/syoyo/tinyexr), a small library for reading and writing OpenEXR images.

---

## Installation

```bash
dotnet add package Olve.TinyEXR
```

---

## Overview

| Type | Description |
| --- | --- |
| `TinyExr` | Safe managed wrapper with exception-based error handling for loading, saving, and validating EXR images. |
| `TinyExrException` | Exception thrown on native library errors, carrying the error code and message. |
| `EXRVersion` | Parsed EXR version header information (version number, tiled, multipart flags). |
| `NativeMethods` | Low-level P/Invoke declarations using `[LibraryImport]` source-generated interop. |

---

## Usage

### Saving and loading an EXR file

Create pixel data as RGBA floats, save to an EXR file, and load it back.

```cs
// ../../tests/Olve.TinyEXR.Tests/ReadmeDemo.cs#L11-L24

var pixels = new float[] { 1.0f, 0.5f, 0.0f, 1.0f }; // Single RGBA pixel
var tempFile = Path.GetTempFileName() + ".exr";

TinyExr.SaveEXR(pixels, 1, 1, 4, false, tempFile); // Save as 32-bit float EXR

var loaded = TinyExr.LoadEXR(tempFile, out var width, out var height, out var ptr);

var r = loaded[0]; // 1.0
var g = loaded[1]; // 0.5
var b = loaded[2]; // 0.0
var a = loaded[3]; // 1.0

TinyExr.FreeImageData(ptr);
File.Delete(tempFile);
```

### Loading from memory

EXR data can also be validated and loaded directly from a byte buffer.

```cs
// ../../tests/Olve.TinyEXR.Tests/ReadmeDemo.cs#L36-L41

var bytes = File.ReadAllBytes(tempFile);

var isExr = TinyExr.IsEXRFromMemory(bytes); // true
var loaded = TinyExr.LoadEXRFromMemory(bytes, out var width, out var height, out var ptr);

TinyExr.FreeImageData(ptr);
```

### Validating and inspecting EXR files

Check whether a file is a valid EXR and parse its version header.

```cs
// ../../tests/Olve.TinyEXR.Tests/ReadmeDemo.cs#L54-L55

var isExr = TinyExr.IsEXR(tempFile); // true
var version = TinyExr.ParseEXRVersionFromFile(tempFile); // version.version == 2
```

---

## Native library

The package includes a prebuilt `libtinyexr.so` for linux-x64. The native shared library is built from [syoyo/tinyexr](https://github.com/syoyo/tinyexr) with [miniz](https://github.com/richgel999/miniz) for compression. Cross-platform builds (win-x64, osx-x64, osx-arm64) are planned.

---

## Documentation

Full API reference: [https://olivervea.github.io/Olve.Utilities/api/Olve.TinyEXR.html](https://olivervea.github.io/Olve.Utilities/api/Olve.TinyEXR.html)

---

## License

MIT License © [OliverVea](https://github.com/OliverVea)
