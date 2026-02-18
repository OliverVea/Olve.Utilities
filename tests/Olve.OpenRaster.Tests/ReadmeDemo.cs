using BigGustave;
using Olve.Results;

namespace Olve.OpenRaster.Tests;

public class ReadmeDemo
{
    [Test]
    public async Task ReadingAnOpenRasterFile()
    {
        var filePath = CreateTestOra();

        var operation = new ReadOpenRasterFile();
        var request = new ReadOpenRasterFile.Request(filePath);

        var result = operation.Execute(request);
        if (result.TryPickValue(out var file, out var problems))
        {
            Console.WriteLine($"Version: {file.Version}");
            Console.WriteLine($"Canvas: {file.Width}x{file.Height} @ {file.XResolution} DPI");
            Console.WriteLine($"Layers: {file.Layers.Count}, Groups: {file.Groups.Count}");
        }

        await Assert.That(file!.Width).IsEqualTo(4);
        await Assert.That(file.Layers).HasCount().EqualTo(2);

        File.Delete(filePath);
    }

    [Test]
    public async Task InspectingLayersAndGroups()
    {
        var filePath = CreateTestOra();
        var result = new ReadOpenRasterFile()
            .Execute(new ReadOpenRasterFile.Request(filePath));
        result.TryPickValue(out var file, out _);
        await Assert.That(file).IsNotNull();

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

        await Assert.That(file.Layers).HasCount().EqualTo(2);
        await Assert.That(file.Groups).HasCount().EqualTo(1);

        File.Delete(filePath);
    }

    [Test]
    public async Task ReadingALayerImage()
    {
        var filePath = CreateTestOra();

        var parser = new PngLayerParser();
        var request = new ReadLayerAs<Png>.Request(filePath, "data/layer0.png", parser);

        var result = new ReadLayerAs<Png>().Execute(request);
        if (result.TryPickValue(out var png, out _))
        {
            Console.WriteLine($"Layer image: {png.Width}x{png.Height}");
        }

        await Assert.That(png!.Width).IsEqualTo(4);
        await Assert.That(png.Height).IsEqualTo(4);

        File.Delete(filePath);
    }

    [Test]
    public async Task CompositeOperations()
    {
        var result = CompositeOperation.FromKey("svg:multiply");
        if (result.TryPickValue(out var op, out _))
        {
            Console.WriteLine(op.BlendingFunction);    // Multiply
            Console.WriteLine(op.CompositingOperator); // SourceOver
        }

        await Assert.That(op.BlendingFunction).IsEqualTo(BlendingFunction.Multiply);
        await Assert.That(op.CompositingOperator).IsEqualTo(CompositingOperator.SourceOver);
    }

    [Test]
    public async Task ErrorHandling()
    {
        var operation = new ReadOpenRasterFile();
        var request = new ReadOpenRasterFile.Request("/nonexistent/file.ora");

        var result = operation.Execute(request);
        if (result.TryPickProblems(out var problems))
        {
            foreach (var problem in problems)
                Console.WriteLine(problem.Message);
        }

        await Assert.That(result.TryPickProblems(out _)).IsTrue();
    }

    private static string CreateTestOra()
    {
        var pngBytes = CreateMinimalPng(4, 4);

        return new OraFileBuilder()
            .WithStackXml("""
                <?xml version="1.0" encoding="UTF-8"?>
                <image version="0.0.5" w="4" h="4" xres="72" yres="72">
                  <stack>
                    <stack name="Foreground" composite-op="svg:screen" opacity="0.8">
                      <layer name="Top" src="data/layer1.png" composite-op="svg:multiply" opacity="0.5" visibility="hidden" x="1" y="2" />
                    </stack>
                    <layer name="Background" src="data/layer0.png" />
                  </stack>
                </image>
                """)
            .WithEntry("data/layer0.png", pngBytes)
            .WithEntry("data/layer1.png", pngBytes)
            .Build();
    }

    private static byte[] CreateMinimalPng(int width, int height)
    {
        var builder = PngBuilder.Create(width, height, false);
        for (var y = 0; y < height; y++)
        for (var x = 0; x < width; x++)
            builder.SetPixel(255, 255, 255, x, y);

        using var ms = new MemoryStream();
        builder.Save(ms);
        return ms.ToArray();
    }
}

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
