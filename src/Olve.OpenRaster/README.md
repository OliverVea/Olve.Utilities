# Olve.OpenRaster

[![NuGet](https://img.shields.io/nuget/v/Olve.OpenRaster?logo=nuget)](https://www.nuget.org/packages/Olve.OpenRaster)[![GitHub](https://img.shields.io/github/license/OliverVea/Olve.OpenRaster)](LICENSE)![LOC](https://img.shields.io/endpoint?url=https%3A%2F%2Fghloc.vercel.app%2Fapi%2FOliverVea%2FOlve.OpenRaster%2Fbadge)![NuGet Downloads](https://img.shields.io/nuget/dt/Olve.OpenRaster)

The purpose of this library is to provide simple read-only access to `.ora`, or [OpenRaster](https://www.openraster.org/), files, with a simple native C# library with minimal dependencies, in fact, [`Olve.Results`](https://github.com/OliverVea/Olve.Utilities/tree/master/src/Olve.Results), [`Olve.Operations`](https://github.com/OliverVea/Olve.Utilities/tree/master/src/Olve.Operations), and, transiently, `Microsoft.Extensions.DependencyInjection.Abstractions` are the only dependencies of this project.

## Installation

Simply run the following command to add a dependency for the nuget package to your project:

```bash
dotnet add package Olve.OpenRaster
```

## Usage

This package only contains two operations:

### ReadOpenRasterFile

[**`ReadOpenRasterFile`**](Olve.OpenRaster/Operations/ReadOpenRasterFile.cs) reads an `.ora` file and returns the metadata and layer data as an easily-consumable class hierarchy.

```csharp
using Olve.Results;

namespace Olve.OpenRaster.Test;

public static class ReadOpenRasterFile_Example
{
    public static void ReadOpenRasterFile()
    {
        ReadOpenRasterFile operation = new();
        ReadOpenRasterFile.Request request = new("map_1.ora");

        var result = operation.Execute(request);
        if (!result.TryPickValue(out var openRasterFile, out var problems))
        {
            problems.Prepend(new ResultProblem("could not read OpenRaster file '{0}'", request.FilePath));

            foreach (var problem in problems)
            {
                Console.WriteLine(problem.ToDebugString());
            }

            return;
        }

        Console.WriteLine($"Successfully read OpenRaster file '{request.FilePath}' with {openRasterFile.Layers.Count} layers and {openRasterFile.Groups.Count} groups");
    }
}
```

### ReadLayerAs

[**`ReadLayerAs<T>`**](Olve.OpenRaster/Operations/ReadLayerAs.cs) takes a layer source string and parses the layer image into the output type `T` with a provided [`ILayerParser<T>`](Olve.OpenRaster/ILayerParser.cs).

```csharp
using BigGustave;
using Olve.OpenRaster;
using Olve.Results;

public static class ReadLayerAs_Example
{
    public static void ReadLayerAsPng()
    {
        PngLayerParser pngLayerParser = new();
        
        ReadLayerAs<Png> readLayerAsPng = new();
        ReadLayerAs<Png>.Request request = new("map_1.ora", "data/002.png", pngLayerParser);
        
        var result = readLayerAsPng.Execute(request);
        if (!result.TryPickValue(out var png, out var problems))
        {
            problems.Prepend(new ResultProblem("could not read layer image '{0}' in file '{1}'", request.LayerSource, request.FilePath));
            
            foreach (var problem in problems)
            {
                Console.WriteLine(problem.ToDebugString());
            }

            return;
        }
        
        Console.WriteLine($"Successfully decoded {png.Width}x{png.Height} PNG image");
        return;
    }
}
```

> [!TIP]
> The `ReadLayerAs<T>` operation is a generic operation that can be used to read any layer type, as long as you provide an implementation of `ILayerParser<T>`.
>
> For example, an `ILayerParser<Mesh>` could be used to convert a heightmap layer into a 3D mesh, isolating the logic for this conversion from the rest of your code.

Currently, no default implementations of `ILayerParser` have been added to the library, you will have to convert the byte stream to an image file yourself, but the `.ora` structure is handily abstracted away, so you will most likely only need to parse a `.png` file yourself.

The [`BigGustave`](https://github.com/EliotJones/BigGustave) library is a good candidate for this, as it is a simple and lightweight PNG decoder. Here is an example of how you could implement this:

```csharp
using BigGustave;
using Olve.Results;

namespace Olve.OpenRaster.Test;

public class PngLayerParser : ILayerParser<Png>
{
    public Result<Png> ParseLayer(Stream stream)
    {
        return Png.Open(stream);
    }
}
```

And don't forget to add the `BigGustave` package to your project:

```bash
dotnet add package BigGustave
```

## Future

I want to expand this in the future with:

1. Reading all content. Currently, the following is not supported:
    - Thumbnail images
    - Merged image
1. Writing operations to both stack and all images.
