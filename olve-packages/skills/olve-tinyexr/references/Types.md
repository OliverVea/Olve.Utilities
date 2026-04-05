# Types

API docs: [https://olivervea.github.io/Olve.Utilities/api/Olve.TinyEXR.html](https://olivervea.github.io/Olve.Utilities/api/Olve.TinyEXR.html)

## TinyExrException

Exception thrown when a tinyexr native call fails.

**Namespace:** `Olve.TinyEXR`

```csharp
public sealed class TinyExrException : Exception
```

### Constructor

```csharp
public TinyExrException(int code, string? message)
```

- `code`: the native tinyexr error code.
- `message`: the native error message, or a default message if null.

### Properties

| Property | Type | Description |
| --- | --- | --- |
| `Code` | `int` | The native tinyexr error code. |
| `Message` | `string` | Inherited from `Exception`. Contains the native error description or `"TinyEXR error {code}"`. |

### Error codes

| Code | Constant |
| --- | --- |
| 0 | `TINYEXR_SUCCESS` |
| -1 | `TINYEXR_ERROR_INVALID_MAGIC_NUMBER` |
| -2 | `TINYEXR_ERROR_INVALID_EXR_VERSION` |
| -3 | `TINYEXR_ERROR_INVALID_ARGUMENT` |
| -4 | `TINYEXR_ERROR_INVALID_DATA` |
| -5 | `TINYEXR_ERROR_INVALID_FILE` |
| -6 | `TINYEXR_ERROR_INVALID_PARAMETER` |
| -7 | `TINYEXR_ERROR_CANT_OPEN_FILE` |
| -8 | `TINYEXR_ERROR_UNSUPPORTED_FORMAT` |
| -9 | `TINYEXR_ERROR_INVALID_HEADER` |
| -10 | `TINYEXR_ERROR_UNSUPPORTED_FEATURE` |
| -11 | `TINYEXR_ERROR_CANT_WRITE_FILE` |
| -12 | `TINYEXR_ERROR_SERIALIZATION_FAILED` |
| -13 | `TINYEXR_ERROR_LAYER_NOT_FOUND` |
| -14 | `TINYEXR_ERROR_DATA_TOO_LARGE` |

## EXRVersion

Parsed EXR version header information.

**Namespace:** `Olve.TinyEXR`

```csharp
[StructLayout(LayoutKind.Sequential)]
public struct EXRVersion
```

### Fields

| Field | Type | Description |
| --- | --- | --- |
| `version` | `int` | EXR format version number (typically 2). |
| `tiled` | `int` | Non-zero if the image uses tiled storage. |
| `long_name` | `int` | Non-zero if the image uses long channel/attribute names. |
| `non_image` | `int` | Non-zero if the file contains non-image data (deep data). |
| `multipart` | `int` | Non-zero if the file is a multipart EXR. |
