---
name: olve-tinyexr
description: Reference for Olve.TinyEXR — P/Invoke bindings for the tinyexr EXR image library. Covers loading, saving, validating, and inspecting OpenEXR images via the TinyExr static class. Use when writing or reading code that works with EXR files.
user-invocable: false
---

# Olve.TinyEXR

P/Invoke bindings for [tinyexr](https://github.com/syoyo/tinyexr). Source: `Olve.Utilities/src/Olve.TinyEXR/`.

Reference docs: [README](references/README.md) | [TinyExr](references/TinyExr.md) | [Types](references/Types.md)

**Golden rule:** All `TinyExr` methods require `unsafe` context. Always free native pointers with `FreeImageData` or `FreeMemoryBuffer` when done.

## Save and load an EXR file

```csharp
unsafe
{
    var pixels = new float[] { 1.0f, 0.5f, 0.0f, 1.0f }; // Single RGBA pixel

    TinyExr.SaveEXR(pixels, 1, 1, 4, false, "image.exr"); // Save as 32-bit float EXR

    var loaded = TinyExr.LoadEXR("image.exr", out var width, out var height, out var ptr);
    // loaded is ReadOnlySpan<float> of length width * height * 4

    var r = loaded[0]; // 1.0
    var g = loaded[1]; // 0.5

    TinyExr.FreeImageData(ptr); // MUST free native memory
}
```

## Load from memory

```csharp
unsafe
{
    var bytes = File.ReadAllBytes("image.exr");

    var isExr = TinyExr.IsEXRFromMemory(bytes); // true
    var loaded = TinyExr.LoadEXRFromMemory(bytes, out var width, out var height, out var ptr);

    TinyExr.FreeImageData(ptr);
}
```

## Save to memory

```csharp
unsafe
{
    var pixels = new float[] { 1.0f, 0.0f, 0.0f, 1.0f };
    var data = TinyExr.SaveEXRToMemory(pixels, 1, 1, 4, false, out var ptr);
    // data is ReadOnlySpan<byte>

    TinyExr.FreeMemoryBuffer(ptr); // MUST free native memory
}
```

## Validate and inspect

```csharp
var isExr = TinyExr.IsEXR("image.exr"); // true/false
var version = TinyExr.ParseEXRVersionFromFile("image.exr");
// version.version == 2, version.tiled, version.multipart, etc.
```

## Memory management

- `LoadEXR`, `LoadEXRWithLayer`, `LoadEXRFromMemory` return a `float*` via `out` parameter. Free with `TinyExr.FreeImageData(ptr)`.
- `SaveEXRToMemory` returns a `byte*` via `out` parameter. Free with `TinyExr.FreeMemoryBuffer(ptr)`.
- Failing to free causes native memory leaks.

## Error handling

All methods throw `TinyExrException` on failure. The exception carries the native error code (`Code`) and message.

```csharp
try
{
    unsafe { TinyExr.LoadEXR("missing.exr", out _, out _, out _); }
}
catch (TinyExrException ex)
{
    // ex.Code == -7 (TINYEXR_ERROR_CANT_OPEN_FILE)
    // ex.Message contains the native error description
}
```
