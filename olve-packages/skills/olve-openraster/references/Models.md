# Olve.OpenRaster -- Models

API docs: [https://olivervea.github.io/Olve.Utilities/api/Olve.OpenRaster.html](https://olivervea.github.io/Olve.Utilities/api/Olve.OpenRaster.html)

Source: `src/Olve.OpenRaster/Models/`

## OpenRasterFile

Represents the content of an OpenRaster (`.ora`) file.

```csharp
public class OpenRasterFile
{
    public required string Version { get; set; }
    public required int Width { get; set; }
    public required int Height { get; set; }
    public int XResolution { get; set; } = 72;
    public int YResolution { get; set; } = 72;
    public IReadOnlyList<Layer> Layers { get; set; } = [];
    public IReadOnlyList<Group> Groups { get; set; } = [];
}
```

- `Version` -- OpenRaster format version (e.g. `"0.0.5"`).
- `Width`, `Height` -- canvas dimensions in pixels.
- `XResolution`, `YResolution` -- DPI, defaults to 72.
- `Layers` -- flat list of all layers in the file.
- `Groups` -- flat list of all groups (stacks) in the file.

## Layer

Represents a layer element in the layer stack.

```csharp
public class Layer
{
    public required string Name { get; set; }
    public required string Source { get; set; }
    public CompositeOperation CompositeOperation { get; set; } = CompositeOperation.SrcOver;
    public float Opacity { get; set; } = 1.0f;
    public bool Visible { get; set; } = true;
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;
    public IList<Group> Groups { get; set; } = [];
}
```

- `Name` -- layer name from the `name` attribute.
- `Source` -- path to the layer image within the ZIP (e.g. `"data/layer0.png"`).
- `CompositeOperation` -- blend mode, defaults to `SrcOver`.
- `Opacity` -- 0.0 (transparent) to 1.0 (opaque), defaults to 1.0.
- `Visible` -- visibility, defaults to `true`.
- `X`, `Y` -- position offset relative to the canvas.
- `Groups` -- parent groups this layer belongs to (bidirectional link).

## Group

A group of layers (corresponds to a `<stack>` element).

```csharp
public class Group
{
    public required string Name { get; set; }
    public CompositeOperation CompositeOperation { get; set; } = CompositeOperation.SrcOver;
    public float Opacity { get; set; } = 1.0f;
    public Visibility Visibility { get; set; } = Visibility.Visible;
    [Obsolete] public int X { get; set; }
    [Obsolete] public int Y { get; set; }
    public IList<Layer> Layers { get; set; } = [];
}
```

- `Name` -- group name.
- `Visibility` -- uses the `Visibility` enum (not a plain `bool` like `Layer`).
- `Layers` -- child layers in this group (bidirectional link).
- `X`, `Y` -- deprecated since OpenRaster 0.0.6.

## CompositeOperation

A record struct combining a blending function and compositing operator with its SVG key string.

```csharp
public readonly record struct CompositeOperation(
    string Key,
    BlendingFunction BlendingFunction,
    CompositingOperator CompositingOperator);
```

### Static presets

| Property | Key | BlendingFunction | CompositingOperator |
| --- | --- | --- | --- |
| `SrcOver` | `svg:src-over` | `Normal` | `SourceOver` |
| `Multiply` | `svg:multiply` | `Multiply` | `SourceOver` |
| `Screen` | `svg:screen` | `Screen` | `SourceOver` |
| `Overlay` | `svg:overlay` | `Overlay` | `SourceOver` |
| `Darken` | `svg:darken` | `Darken` | `SourceOver` |
| `Lighten` | `svg:lighten` | `Lighten` | `SourceOver` |
| `ColorDodge` | `svg:color-dodge` | `ColorDodge` | `SourceOver` |
| `ColorBurn` | `svg:color-burn` | `ColorBurn` | `SourceOver` |
| `HardLight` | `svg:hard-light` | `HardLight` | `SourceOver` |
| `SoftLight` | `svg:soft-light` | `SoftLight` | `SourceOver` |
| `Difference` | `svg:difference` | `Difference` | `SourceOver` |
| `Color` | `svg:color` | `Color` | `SourceOver` |
| `Luminosity` | `svg:luminosity` | `Luminosity` | `SourceOver` |
| `Hue` | `svg:hue` | `Hue` | `SourceOver` |
| `Saturation` | `svg:saturation` | `Saturation` | `SourceOver` |
| `Plus` | `svg:plus` | `Normal` | `Lighter` |
| `DestinationIn` | `svg:dst-in` | `Normal` | `DestinationIn` |
| `DestinationOut` | `svg:dst-out` | `Normal` | `DestinationOut` |
| `SourceAtop` | `svg:src-atop` | `Normal` | `SourceAtop` |
| `DestinationAtop` | `svg:dst-atop` | `Normal` | `DestinationAtop` |

### Methods

```csharp
public static Result<CompositeOperation> FromKey(string key);
```

Parses an SVG key string (e.g. `"svg:multiply"`) into the corresponding preset. Returns a `ResultProblem` for unknown keys.

## BlendingFunction

Enum defining how colors are blended between layers. Follows the [W3C Compositing and Blending](https://www.w3.org/TR/compositing-1/) specification.

```csharp
public enum BlendingFunction
{
    Normal,
    Multiply,
    Screen,
    Overlay,
    Darken,
    Lighten,
    ColorDodge,
    ColorBurn,
    HardLight,
    SoftLight,
    Difference,
    Color,
    Luminosity,
    Hue,
    Saturation
}
```

## CompositingOperator

Enum defining how alpha channels interact during compositing.

```csharp
public enum CompositingOperator
{
    SourceOver,
    Lighter,
    DestinationIn,
    DestinationOut,
    SourceAtop,
    DestinationAtop
}
```

## Visibility

Enum for layer/group visibility state.

```csharp
public enum Visibility
{
    Hidden = 0,
    Visible = 1
}
```
