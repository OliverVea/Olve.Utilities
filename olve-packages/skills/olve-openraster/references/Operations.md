# Olve.OpenRaster -- Operations

API docs: [https://olivervea.github.io/Olve.Utilities/api/Olve.OpenRaster.html](https://olivervea.github.io/Olve.Utilities/api/Olve.OpenRaster.html)

Source: `src/Olve.OpenRaster/Operations/`

## ReadOpenRasterFile

Reads and validates an OpenRaster (`.ora`) file, extracting metadata, layers, and groups.

Implements `IOperation<ReadOpenRasterFile.Request, OpenRasterFile>`.

```csharp
public class ReadOpenRasterFile : IOperation<ReadOpenRasterFile.Request, OpenRasterFile>
{
    public record Request(string FilePath);

    public Result<OpenRasterFile> Execute(Request request);
}
```

### Usage

```csharp
var operation = new ReadOpenRasterFile();
var request = new ReadOpenRasterFile.Request(filePath);

var result = operation.Execute(request);
if (result.TryPickProblems(out var problems, out var file))
{
    return problems.Prepend("Failed to read .ora file");
}

// file.Version, file.Width, file.Height
// file.Layers -- flat list of all layers
// file.Groups -- flat list of all groups
```

### Behavior

- Validates the file path exists and is non-empty.
- Opens the `.ora` file as a ZIP archive.
- Parses `stack.xml` to build the full `OpenRasterFile` model.
- Establishes bidirectional links between layers and groups.
- Returns `ResultProblem` for invalid paths, missing files, or malformed XML.

## ReadLayerAs\<T\>

Loads a specific layer's image data from an `.ora` file and parses it into a custom type using an `ILayerParser<T>`.

Implements `IOperation<ReadLayerAs<T>.Request, T>`.

```csharp
public class ReadLayerAs<T> : IOperation<ReadLayerAs<T>.Request, T>
{
    public record Request(string FilePath, string LayerSource, ILayerParser<T> LayerParser);

    public Result<T> Execute(Request request);
}
```

### Usage

```csharp
var parser = new PngLayerParser();
var request = new ReadLayerAs<Png>.Request(filePath, "data/layer0.png", parser);

var result = new ReadLayerAs<Png>().Execute(request);
if (result.TryPickProblems(out var problems, out var png))
{
    return problems.Prepend("Failed to read layer image");
}

// png.Width, png.Height, etc.
```

### Behavior

- Validates the file path.
- Opens the `.ora` ZIP archive.
- Locates the entry matching `LayerSource` within the archive.
- Passes the entry stream to `ILayerParser<T>.ParseLayer`.
- Returns `ResultProblem` if the file is invalid, the layer entry is missing, or the parser fails.

### Request parameters

| Parameter | Type | Description |
| --- | --- | --- |
| `FilePath` | `string` | Path to the `.ora` file. |
| `LayerSource` | `string` | Layer source path within the ZIP (from `Layer.Source`, e.g. `"data/layer0.png"`). |
| `LayerParser` | `ILayerParser<T>` | Parser implementation to convert the stream into `T`. |

## ILayerParser\<T\>

Interface for converting a layer's byte stream into a custom type. The library does not bundle any image decoder -- implement this with your preferred image library.

```csharp
public interface ILayerParser<T>
{
    Result<T> ParseLayer(Stream stream);
}
```

### Parameters

- `stream` -- positioned at the beginning of the layer's image data from the ZIP entry.

### Example implementation (BigGustave)

```csharp
public class PngLayerParser : ILayerParser<Png>
{
    public Result<Png> ParseLayer(Stream stream)
    {
        var ms = new MemoryStream();
        stream.CopyTo(ms);
        ms.Position = 0;
        return Png.Open(ms);
    }
}
```

### Example implementation (byte array)

```csharp
public class ByteArrayLayerParser : ILayerParser<byte[]>
{
    public Result<byte[]> ParseLayer(Stream stream)
    {
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }
}
```
