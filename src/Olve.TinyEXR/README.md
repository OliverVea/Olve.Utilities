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

The package includes prebuilt native libraries for linux-x64 and win-x64. Cross-platform builds for osx-x64 and osx-arm64 are planned.

### Rebuilding the native libraries

The native shared libraries are built from [syoyo/tinyexr](https://github.com/syoyo/tinyexr) with [miniz](https://github.com/richgel999/miniz) for compression. Source files are not included in this repository — only the prebuilt artifacts under `runtimes/`.

To rebuild, download the following files from the upstream repositories into a working directory:

- `tinyexr.h` from [syoyo/tinyexr](https://github.com/syoyo/tinyexr)
- `miniz.h` and `miniz.c` from [richgel999/miniz](https://github.com/richgel999/miniz)

Create `tinyexr.cc`:

```cpp
#define TINYEXR_IMPLEMENTATION
#include "tinyexr.h"
```

Create `CMakeLists.txt`:

```cmake
cmake_minimum_required(VERSION 3.10)
project(tinyexr LANGUAGES C CXX)

add_library(tinyexr SHARED tinyexr.cc miniz.c)
target_include_directories(tinyexr PRIVATE ${CMAKE_CURRENT_SOURCE_DIR})
```

**Windows (MSVC):**

```bash
mkdir build && cd build
cmake -G "Visual Studio 17 2022" -A x64 -DCMAKE_WINDOWS_EXPORT_ALL_SYMBOLS=ON ..
cmake --build . --config Release
# Output: build/Release/tinyexr.dll → runtimes/win-x64/native/
```

**Linux (GCC/Clang):**

```bash
mkdir build && cd build
cmake -DCMAKE_BUILD_TYPE=Release ..
cmake --build .
# Output: build/libtinyexr.so → runtimes/linux-x64/native/
```

---

## Documentation

Full API reference: [https://olivervea.github.io/Olve.Utilities/api/Olve.TinyEXR.html](https://olivervea.github.io/Olve.Utilities/api/Olve.TinyEXR.html)

---

## License

MIT License © [OliverVea](https://github.com/OliverVea)
