# TinyExr

API docs: [https://olivervea.github.io/Olve.Utilities/api/Olve.TinyEXR.TinyExr.html](https://olivervea.github.io/Olve.Utilities/api/Olve.TinyEXR.TinyExr.html)

Static class providing safe managed wrappers over the tinyexr native library. All methods require `unsafe` context.

**Namespace:** `Olve.TinyEXR`

## Loading

### LoadEXR

Loads an EXR image from a file. Returns pixel data as RGBA float values.

```csharp
public static ReadOnlySpan<float> LoadEXR(
    string filename,
    out int width,
    out int height,
    out float* nativePtr)
```

The returned span has length `width * height * 4`. Free `nativePtr` with `FreeImageData` when done.

### LoadEXRWithLayer

Loads an EXR image with a specific layer from a file.

```csharp
public static ReadOnlySpan<float> LoadEXRWithLayer(
    string filename,
    string layerName,
    out int width,
    out int height,
    out float* nativePtr)
```

Free `nativePtr` with `FreeImageData` when done.

### LoadEXRFromMemory

Loads an EXR image from a byte buffer in memory.

```csharp
public static ReadOnlySpan<float> LoadEXRFromMemory(
    ReadOnlySpan<byte> memory,
    out int width,
    out int height,
    out float* nativePtr)
```

Free `nativePtr` with `FreeImageData` when done.

## Saving

### SaveEXR

Saves RGBA float data to an EXR file.

```csharp
public static void SaveEXR(
    ReadOnlySpan<float> data,
    int width,
    int height,
    int components,
    bool saveAsFp16,
    string filename)
```

- `components`: number of channels (typically 4 for RGBA).
- `saveAsFp16`: `true` to save as half-precision floats, `false` for full 32-bit.

### SaveEXRToMemory

Saves RGBA float data to an EXR in memory.

```csharp
public static ReadOnlySpan<byte> SaveEXRToMemory(
    ReadOnlySpan<float> data,
    int width,
    int height,
    int components,
    bool saveAsFp16,
    out byte* nativePtr)
```

Returns the EXR file bytes as a span. Free `nativePtr` with `FreeMemoryBuffer` when done.

## Validation

### IsEXR

Checks if a file is a valid EXR image.

```csharp
public static bool IsEXR(string filename)
```

### IsEXRFromMemory

Checks if a byte buffer contains a valid EXR image.

```csharp
public static bool IsEXRFromMemory(ReadOnlySpan<byte> memory)
```

## Version parsing

### ParseEXRVersionFromFile

Parses the EXR version header from a file.

```csharp
public static EXRVersion ParseEXRVersionFromFile(string filename)
```

### ParseEXRVersionFromMemory

Parses the EXR version header from a byte buffer.

```csharp
public static EXRVersion ParseEXRVersionFromMemory(ReadOnlySpan<byte> memory)
```

## Memory management

### FreeImageData

Frees image data returned by `LoadEXR`, `LoadEXRWithLayer`, or `LoadEXRFromMemory`.

```csharp
public static void FreeImageData(float* data)
```

### FreeMemoryBuffer

Frees a buffer returned by `SaveEXRToMemory`.

```csharp
public static void FreeMemoryBuffer(byte* buffer)
```
