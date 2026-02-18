# Olve.OpenRaster

[![NuGet](https://img.shields.io/nuget/v/Olve.OpenRaster?logo=nuget)](https://www.nuget.org/packages/Olve.OpenRaster)
[![Docs](https://img.shields.io/badge/docs-API%20Reference-blue)](https://olivervea.github.io/Olve.Utilities/api/Olve.OpenRaster.html)

Lightweight, read-only access to [OpenRaster](https://www.openraster.org/) (`.ora`) files in native C#.

---

## Installation

```bash
dotnet add package Olve.OpenRaster
```

---

## Overview

| Type | Description |
| --- | --- |
| `ReadOpenRasterFile` | Reads an `.ora` file and returns an `OpenRasterFile` model. |
| `ReadLayerAs<T>` | Loads a layer image and parses it into a custom type `T`. |
| `ILayerParser<T>` | Interface for converting a layer's byte stream into `T`. |
| `OpenRasterFile` | Top-level model: dimensions, resolution, layers, and groups. |
| `Layer` | Layer metadata: name, source path, opacity, position, composite operation. |
| `Group` | Group metadata: name, visibility, opacity, contained layers. |
| `CompositeOperation` | Blending/compositing mode (e.g. `SrcOver`, `Multiply`, `Screen`). |

---

## Usage

### Reading an OpenRaster file

```csharp
var operation = new ReadOpenRasterFile();
var request = new ReadOpenRasterFile.Request("map.ora");

var result = operation.Execute(request);
if (result.TryPickValue(out var file, out var problems))
{
    Console.WriteLine($"{file.Width}x{file.Height}, {file.Layers.Count} layers");
}
```

---

### Reading a layer image

`ReadLayerAs<T>` parses a layer's image data using a custom `ILayerParser<T>`. The library does not bundle a default parser — you provide one for your image format.

For example, using the [`BigGustave`](https://github.com/EliotJones/BigGustave) PNG library:

```csharp
public class PngLayerParser : ILayerParser<Png>
{
    public Result<Png> ParseLayer(Stream stream) => Png.Open(stream);
}
```

Then use it to read a layer:

```csharp
var parser = new PngLayerParser();
var request = new ReadLayerAs<Png>.Request("map.ora", "data/002.png", parser);

var result = new ReadLayerAs<Png>().Execute(request);
if (result.TryPickValue(out var png, out _))
{
    Console.WriteLine($"{png.Width}x{png.Height}");
}
```

---

### Composite operations

Each layer and group has a `CompositeOperation` describing how it blends. The library supports all standard OpenRaster blend modes:

`SrcOver`, `Multiply`, `Screen`, `Overlay`, `Darken`, `Lighten`, `ColorDodge`, `ColorBurn`, `HardLight`, `SoftLight`, `Difference`, `Color`, `Luminosity`, `Hue`, `Saturation`, `Plus`, `DestinationIn`, `DestinationOut`, `SourceAtop`, `DestinationAtop`

---

## Documentation

Full API reference:
[https://olivervea.github.io/Olve.Utilities/api/Olve.OpenRaster.html](https://olivervea.github.io/Olve.Utilities/api/Olve.OpenRaster.html)

---

## License

MIT License © [OliverVea](https://github.com/OliverVea)
