# Olve.OpenRaster -- Overview

API docs: [https://olivervea.github.io/Olve.Utilities/api/Olve.OpenRaster.html](https://olivervea.github.io/Olve.Utilities/api/Olve.OpenRaster.html)

## What it does

Lightweight, read-only access to [OpenRaster](https://www.openraster.org/) (`.ora`) files in native C#. OpenRaster is a layered image format used by digital painting applications like GIMP and Krita.

## Design philosophy

- **Read-only**: The library parses `.ora` files but does not create or modify them.
- **No image decoding**: Layer image data is accessed through the `ILayerParser<T>` interface, letting consumers bring their own image library (e.g., BigGustave, SkiaSharp, ImageSharp).
- **Result-based error handling**: All operations return `Result<T>` from `Olve.Results` instead of throwing exceptions.
- **Follows the OpenRaster spec**: Models map directly to the [OpenRaster baseline layer stack specification](https://www.openraster.org/baseline/layer-stack-spec.html). Composite operations follow the [W3C Compositing and Blending](https://www.w3.org/TR/compositing-1/) specification.
- **Bidirectional layer/group relationships**: Layers know which groups they belong to and groups know which layers they contain.

## Package structure

| Namespace | Contents |
| --- | --- |
| `Olve.OpenRaster` | Models (`OpenRasterFile`, `Layer`, `Group`, `CompositeOperation`, enums), operations (`ReadOpenRasterFile`, `ReadLayerAs<T>`), and `ILayerParser<T>` interface. |
| `Olve.OpenRaster.Parsing` | Internal XML and ZIP parsing helpers (not part of the public API). |

## Installation

```bash
dotnet add package Olve.OpenRaster
```

## How .ora files work

An `.ora` file is a ZIP archive containing:

- `mimetype` -- must be `image/openraster`
- `stack.xml` -- XML describing the layer hierarchy, canvas dimensions, and metadata
- `data/*.png` (or other formats) -- the actual layer image files

The library reads `stack.xml` to build the `OpenRasterFile` model, and can open individual layer entries from the ZIP for custom parsing.
