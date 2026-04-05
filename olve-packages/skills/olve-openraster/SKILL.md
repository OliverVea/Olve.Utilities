---
name: olve-openraster
description: Reference for Olve.OpenRaster — read-only access to OpenRaster (.ora) layered image files, including layer/group metadata, composite operations, and custom layer parsing via ILayerParser<T>. Use when writing or reading code that works with .ora files.
user-invocable: false
---

# Olve.OpenRaster

Read-only access to [OpenRaster](https://www.openraster.org/) (`.ora`) layered image files. Source: `Olve.Utilities/src/Olve.OpenRaster/`.

Reference docs: [README](references/README.md) | [Models](references/Models.md) | [Operations](references/Operations.md)

API docs: [https://olivervea.github.io/Olve.Utilities/api/Olve.OpenRaster.html](https://olivervea.github.io/Olve.Utilities/api/Olve.OpenRaster.html)

## Quick usage

### Read an .ora file

```csharp
var operation = new ReadOpenRasterFile();
var request = new ReadOpenRasterFile.Request(filePath);

var result = operation.Execute(request);
if (result.TryPickProblems(out var problems, out var file))
{
    return problems.Prepend("Failed to read .ora file");
}

// file.Version, file.Width, file.Height, file.Layers, file.Groups
```

### Inspect layers

```csharp
foreach (var layer in file.Layers)
{
    // layer.Name, layer.Source, layer.Opacity, layer.Visible
    // layer.X, layer.Y, layer.CompositeOperation, layer.Groups
}
```

### Read a layer image with a custom parser

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

var parser = new PngLayerParser();
var request = new ReadLayerAs<Png>.Request(filePath, "data/layer0.png", parser);
var result = new ReadLayerAs<Png>().Execute(request);
```

### Composite operations

```csharp
var result = CompositeOperation.FromKey("svg:multiply");
if (result.TryPickValue(out var op, out _))
{
    // op.BlendingFunction == BlendingFunction.Multiply
    // op.CompositingOperator == CompositingOperator.SourceOver
}
```

## Key points

- All operations return `Result<T>` from `Olve.Results` -- never throws.
- The library does NOT bundle an image decoder. Implement `ILayerParser<T>` with your preferred image library.
- `.ora` files are ZIP archives containing `stack.xml` (metadata) and layer image files.
- Layers track their parent `Group`s and groups track their child `Layer`s (bidirectional).
