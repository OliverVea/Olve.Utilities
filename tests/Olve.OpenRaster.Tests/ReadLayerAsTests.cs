using Olve.Results;

namespace Olve.OpenRaster.Tests;

public class ReadLayerAsTests
{
    [Test]
    public async Task Execute_WithValidLayer_ReturnsParserResult()
    {
        var layerData = "hello-layer-data"u8.ToArray();
        var path = new OraFileBuilder()
            .WithStackXml("""
                <?xml version="1.0" encoding="UTF-8"?>
                <image version="0.0.5" w="64" h="64">
                  <stack>
                    <layer name="L" src="data/layer.bin" />
                  </stack>
                </image>
                """)
            .WithEntry("data/layer.bin", layerData)
            .Build();

        var parser = new ByteArrayLayerParser();
        var sut = new ReadLayerAs<byte[]>();
        var result = sut.Execute(new ReadLayerAs<byte[]>.Request(path, "data/layer.bin", parser));

        await Assert.That(result.TryPickValue(out var bytes, out _)).IsTrue();
        await Assert.That(bytes!.SequenceEqual(layerData)).IsTrue();

        File.Delete(path);
    }

    [Test]
    public async Task Execute_WithMissingLayer_ReturnsProblems()
    {
        var path = new OraFileBuilder()
            .WithStackXml("""
                <?xml version="1.0" encoding="UTF-8"?>
                <image version="0.0.5" w="64" h="64">
                  <stack>
                    <layer name="L" src="data/layer.bin" />
                  </stack>
                </image>
                """)
            .Build();

        var parser = new ByteArrayLayerParser();
        var sut = new ReadLayerAs<byte[]>();
        var result = sut.Execute(new ReadLayerAs<byte[]>.Request(path, "data/nonexistent.bin", parser));

        await Assert.That(result.TryPickProblems(out _)).IsTrue();

        File.Delete(path);
    }

    [Test]
    public async Task Execute_WithNonexistentFile_ReturnsProblems()
    {
        var parser = new ByteArrayLayerParser();
        var sut = new ReadLayerAs<byte[]>();
        var result = sut.Execute(new ReadLayerAs<byte[]>.Request("/nonexistent.ora", "data/layer.bin", parser));

        await Assert.That(result.TryPickProblems(out _)).IsTrue();
    }

    [Test]
    public async Task Execute_WithFailingParser_ReturnsProblems()
    {
        var layerData = "some-data"u8.ToArray();
        var path = new OraFileBuilder()
            .WithStackXml("""
                <?xml version="1.0" encoding="UTF-8"?>
                <image version="0.0.5" w="64" h="64">
                  <stack>
                    <layer name="L" src="data/layer.bin" />
                  </stack>
                </image>
                """)
            .WithEntry("data/layer.bin", layerData)
            .Build();

        var parser = new FailingLayerParser();
        var sut = new ReadLayerAs<byte[]>();
        var result = sut.Execute(new ReadLayerAs<byte[]>.Request(path, "data/layer.bin", parser));

        await Assert.That(result.TryPickProblems(out _)).IsTrue();

        File.Delete(path);
    }

    private class ByteArrayLayerParser : ILayerParser<byte[]>
    {
        public Result<byte[]> ParseLayer(Stream stream)
        {
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }
    }

    private class FailingLayerParser : ILayerParser<byte[]>
    {
        public Result<byte[]> ParseLayer(Stream stream)
        {
            return new ResultProblem("parse failed");
        }
    }
}
