# Olve.OpenRaster

[![NuGet](https://img.shields.io/nuget/v/Olve.OpenRaster?logo=nuget)](https://www.nuget.org/packages/Olve.OpenRaster)
[![Docs](https://img.shields.io/badge/docs-API%20Reference-blue)](https://olivervea.github.io/Olve.Utilities/api/Olve.OpenRaster.html)

Lightweight, read-only access to [OpenRaster](https://www.openraster.org/) (`.ora`) files in native C#.
OpenRaster is a layered image format used by digital painting applications like GIMP and Krita.

---

## Installation

```bash
dotnet add package Olve.OpenRaster
```

---

## Overview

| Type | Description |
| --- | --- |
| `ReadOpenRasterFile` | Reads an `.ora` file and returns an `OpenRasterFile` model with all metadata, layers, and groups. |
| `ReadLayerAs<T>` | Loads a specific layer's image data and parses it into a custom type `T` via an `ILayerParser<T>`. |
| `ILayerParser<T>` | Interface for converting a layer's byte stream into `T`. Bring your own image library. |
| `OpenRasterFile` | Top-level model: format version, canvas dimensions, resolution, layers, and groups. |
| `Layer` | Layer metadata: name, source path, opacity, position, visibility, blend mode, and parent groups. |
| `Group` | Group (stack) metadata: name, visibility, opacity, blend mode, and contained layers. |
| `CompositeOperation` | Blending/compositing mode combining a `BlendingFunction` and `CompositingOperator`. |

---

## Usage

### Reading an OpenRaster file

`ReadOpenRasterFile` opens an `.ora` file, parses `stack.xml`, and returns the full layer hierarchy.

```cs
// ../../tests/Olve.OpenRaster.Tests/ReadmeDemo.cs#L13-L22

var operation = new ReadOpenRasterFile();
var request = new ReadOpenRasterFile.Request(filePath);

var result = operation.Execute(request);
if (result.TryPickValue(out var file, out var problems))
{
    Console.WriteLine($"Version: {file.Version}");
    Console.WriteLine($"Canvas: {file.Width}x{file.Height} @ {file.XResolution} DPI");
    Console.WriteLine($"Layers: {file.Layers.Count}, Groups: {file.Groups.Count}");
}
```

---

### Inspecting layers and groups

Each `Layer` carries its metadata from the `.ora` file. Layers track which `Group`s they belong to, and groups track their contained `Layer`s.

```cs
// ../../tests/Olve.OpenRaster.Tests/ReadmeDemo.cs#L39-L50

foreach (var layer in file!.Layers)
{
    Console.WriteLine($"  {layer.Name} ({layer.Source})");
    Console.WriteLine($"    opacity={layer.Opacity}, visible={layer.Visible}");
    Console.WriteLine($"    position=({layer.X}, {layer.Y})");
    Console.WriteLine($"    blend={layer.CompositeOperation.Key}");
}

foreach (var group in file.Groups)
{
    Console.WriteLine($"  Group: {group.Name} ({group.Layers.Count} layers)");
}
```

---

### Reading a layer image

The library does not bundle an image decoder. Instead, `ReadLayerAs<T>` delegates to your `ILayerParser<T>` implementation, so you can use whichever image library you prefer.

For example, using the [`BigGustave`](https://github.com/EliotJones/BigGustave) PNG library:

```cs
// ../../tests/Olve.OpenRaster.Tests/ReadmeDemo.cs#L142-L151

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

Then read a specific layer by its source path (found on `Layer.Source`):

```cs
// ../../tests/Olve.OpenRaster.Tests/ReadmeDemo.cs#L63-L70

var parser = new PngLayerParser();
var request = new ReadLayerAs<Png>.Request(filePath, "data/layer0.png", parser);

var result = new ReadLayerAs<Png>().Execute(request);
if (result.TryPickValue(out var png, out _))
{
    Console.WriteLine($"Layer image: {png.Width}x{png.Height}");
}
```

---

### Composite operations

Each layer and group carries a `CompositeOperation` describing how it blends with layers beneath it.
A `CompositeOperation` combines a `BlendingFunction` (how colors mix) with a `CompositingOperator` (how alpha channels interact), following the [W3C Compositing and Blending](https://www.w3.org/TR/compositing-1/) specification.

All standard OpenRaster blend modes are supported:

| Blend modes | Compositing modes |
| --- | --- |
| `SrcOver`, `Multiply`, `Screen`, `Overlay` | `Plus` |
| `Darken`, `Lighten`, `ColorDodge`, `ColorBurn` | `DestinationIn`, `DestinationOut` |
| `HardLight`, `SoftLight`, `Difference` | `SourceAtop`, `DestinationAtop` |
| `Color`, `Luminosity`, `Hue`, `Saturation` | |

You can also parse a composite operation from its SVG key string:

```cs
// ../../tests/Olve.OpenRaster.Tests/ReadmeDemo.cs#L81-L86

var result = CompositeOperation.FromKey("svg:multiply");
if (result.TryPickValue(out var op, out _))
{
    Console.WriteLine(op.BlendingFunction);    // Multiply
    Console.WriteLine(op.CompositingOperator); // SourceOver
}
```

---

## Error handling

All operations return `Result<T>` from [`Olve.Results`](https://www.nuget.org/packages/Olve.Results) instead of throwing exceptions. Use `TryPickValue` or `TryPickProblems` to inspect outcomes:

```cs
// ../../tests/Olve.OpenRaster.Tests/ReadmeDemo.cs#L95-L103

var operation = new ReadOpenRasterFile();
var request = new ReadOpenRasterFile.Request("/nonexistent/file.ora");

var result = operation.Execute(request);
if (result.TryPickProblems(out var problems))
{
    foreach (var problem in problems)
        Console.WriteLine(problem.Message);
}
```

---

## Documentation

Full API reference:
[https://olivervea.github.io/Olve.Utilities/api/Olve.OpenRaster.html](https://olivervea.github.io/Olve.Utilities/api/Olve.OpenRaster.html)

---

## License

MIT License © [OliverVea](https://github.com/OliverVea)
