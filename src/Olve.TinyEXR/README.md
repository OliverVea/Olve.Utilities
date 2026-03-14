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

This package provides thin, source-generated `[LibraryImport]` bindings to the tinyexr C API. All functions are on `NativeMethods` and follow the upstream signatures closely — return codes, `out` error pointers, and caller-managed memory.

| Type | Description |
| --- | --- |
| `NativeMethods` | P/Invoke bindings to the tinyexr C API. |
| `TinyExrConstants` | Return codes, pixel types, compression types, and other constants. |
| `EXRVersion` | Parsed EXR version header (version number, tiled, multipart flags). |
| `EXRHeader` | EXR header with channel info, compression, tiling, and custom attributes. |
| `EXRImage` | EXR image data with per-channel image buffers. |
| `DeepImage` | Deep image data with per-pixel sample counts. |

---

## Usage

### Saving and loading an EXR file

```cs
// ../../tests/Olve.TinyEXR.Tests/ReadmeDemo.cs#L12-L25

var pixels = new float[] { 1.0f, 0.5f, 0.0f, 1.0f }; // Single RGBA pixel
var tempFile = Path.GetTempFileName() + ".exr";

NativeMethods.SaveEXR(pixels, 1, 1, 4, 0, tempFile, out _);

NativeMethods.LoadEXR(out var rgba, out var width, out var height, tempFile, out _);

var r = rgba[0]; // 1.0
var g = rgba[1]; // 0.5
var b = rgba[2]; // 0.0
var a = rgba[3]; // 1.0

NativeMemory.Free(rgba);
File.Delete(tempFile);
```

### Loading from memory

```cs
// ../../tests/Olve.TinyEXR.Tests/ReadmeDemo.cs#L37-L43

var bytes = File.ReadAllBytes(tempFile);

var isExr = NativeMethods.IsEXRFromMemory(bytes); // TINYEXR_SUCCESS
NativeMethods.LoadEXRFromMemory(out var rgba, out var width, out var height, bytes, out _);

NativeMemory.Free(rgba);
File.Delete(tempFile);
```

### Saving to memory

```cs
// ../../tests/Olve.TinyEXR.Tests/ReadmeDemo.cs#L66-L73

var pixels = new float[] { 1.0f, 0.5f, 0.0f, 1.0f };

var size = NativeMethods.SaveEXRToMemory(pixels, 1, 1, 4, 0, out var buffer, out _);
var exrBytes = new ReadOnlySpan<byte>(buffer, (int)size);

// use exrBytes...

NativeMemory.Free(buffer);
```

### Validating and inspecting EXR files

```cs
// ../../tests/Olve.TinyEXR.Tests/ReadmeDemo.cs#L55-L56

var isExr = NativeMethods.IsEXR(tempFile); // TINYEXR_SUCCESS
NativeMethods.ParseEXRVersionFromFile(out var version, tempFile); // version.version == 2
```

### Parsing header and loading image data

For full control over channels, compression, and pixel types, use the advanced multi-step workflow.

```cs
// ../../tests/Olve.TinyEXR.Tests/ReadmeDemo.cs#L85-L101

NativeMethods.ParseEXRVersionFromFile(out var version, tempFile);

EXRHeader header;
NativeMethods.InitEXRHeader(&header);
NativeMethods.ParseEXRHeaderFromFile(&header, in version, tempFile, out _);

var numChannels = header.num_channels; // 4 (RGBA)
var compression = header.compression_type; // TINYEXR_COMPRESSIONTYPE_ZIP

EXRImage image;
NativeMethods.InitEXRImage(&image);
NativeMethods.LoadEXRImageFromFile(&image, &header, tempFile, out _);

// use image.images, image.width, image.height ...

NativeMethods.FreeEXRImage(&image);
NativeMethods.FreeEXRHeader(&header);
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
