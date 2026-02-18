namespace Olve.OpenRaster.Tests;

public class ReadOpenRasterFileTests
{
    private readonly ReadOpenRasterFile _sut = new();

    [Test]
    public async Task Execute_WithValidFile_ReturnsOpenRasterFile()
    {
        var path = new OraFileBuilder()
            .WithStackXml("""
                <?xml version="1.0" encoding="UTF-8"?>
                <image version="0.0.5" w="256" h="128">
                  <stack>
                    <layer name="Background" src="data/layer0.png" />
                  </stack>
                </image>
                """)
            .Build();

        var result = _sut.Execute(new ReadOpenRasterFile.Request(path));

        await Assert.That(result.TryPickValue(out var file, out _)).IsTrue();
        await Assert.That(file!.Version).IsEqualTo("0.0.5");
        await Assert.That(file.Width).IsEqualTo(256);
        await Assert.That(file.Height).IsEqualTo(128);
        await Assert.That(file.Layers).HasCount().EqualTo(1);

        File.Delete(path);
    }

    [Test]
    public async Task Execute_ReadsResolutionDefaults()
    {
        var path = new OraFileBuilder()
            .WithStackXml("""
                <?xml version="1.0" encoding="UTF-8"?>
                <image version="0.0.5" w="100" h="100">
                  <stack>
                    <layer name="L" src="data/l.png" />
                  </stack>
                </image>
                """)
            .Build();

        var result = _sut.Execute(new ReadOpenRasterFile.Request(path));

        await Assert.That(result.TryPickValue(out var file, out _)).IsTrue();
        await Assert.That(file!.XResolution).IsEqualTo(72);
        await Assert.That(file.YResolution).IsEqualTo(72);

        File.Delete(path);
    }

    [Test]
    public async Task Execute_ReadsCustomResolution()
    {
        var path = new OraFileBuilder()
            .WithStackXml("""
                <?xml version="1.0" encoding="UTF-8"?>
                <image version="0.0.5" w="100" h="100" xres="300" yres="150">
                  <stack>
                    <layer name="L" src="data/l.png" />
                  </stack>
                </image>
                """)
            .Build();

        var result = _sut.Execute(new ReadOpenRasterFile.Request(path));

        await Assert.That(result.TryPickValue(out var file, out _)).IsTrue();
        await Assert.That(file!.XResolution).IsEqualTo(300);
        await Assert.That(file.YResolution).IsEqualTo(150);

        File.Delete(path);
    }

    [Test]
    public async Task Execute_ReadsLayerMetadata()
    {
        var path = new OraFileBuilder()
            .WithStackXml("""
                <?xml version="1.0" encoding="UTF-8"?>
                <image version="0.0.5" w="64" h="64">
                  <stack>
                    <layer name="Top" src="data/top.png" composite-op="svg:multiply" opacity="0.5" visibility="hidden" x="10" y="20" />
                    <layer name="Bottom" src="data/bottom.png" />
                  </stack>
                </image>
                """)
            .Build();

        var result = _sut.Execute(new ReadOpenRasterFile.Request(path));

        await Assert.That(result.TryPickValue(out var file, out _)).IsTrue();
        await Assert.That(file!.Layers).HasCount().EqualTo(2);

        var top = file.Layers[0];
        await Assert.That(top.Name).IsEqualTo("Top");
        await Assert.That(top.Source).IsEqualTo("data/top.png");
        await Assert.That(top.CompositeOperation).IsEqualTo(CompositeOperation.Multiply);
        await Assert.That(top.Opacity).IsEqualTo(0.5f);
        await Assert.That(top.Visible).IsFalse();
        await Assert.That(top.X).IsEqualTo(10);
        await Assert.That(top.Y).IsEqualTo(20);

        var bottom = file.Layers[1];
        await Assert.That(bottom.Name).IsEqualTo("Bottom");
        await Assert.That(bottom.CompositeOperation).IsEqualTo(CompositeOperation.SrcOver);
        await Assert.That(bottom.Opacity).IsEqualTo(1.0f);
        await Assert.That(bottom.Visible).IsTrue();
        await Assert.That(bottom.X).IsEqualTo(0);
        await Assert.That(bottom.Y).IsEqualTo(0);

        File.Delete(path);
    }

    [Test]
    public async Task Execute_ReadsGroups()
    {
        var path = new OraFileBuilder()
            .WithStackXml("""
                <?xml version="1.0" encoding="UTF-8"?>
                <image version="0.0.5" w="64" h="64">
                  <stack>
                    <stack name="Group1" composite-op="svg:screen" opacity="0.8" visibility="hidden">
                      <layer name="InGroup" src="data/ingroup.png" />
                    </stack>
                    <layer name="Outside" src="data/outside.png" />
                  </stack>
                </image>
                """)
            .Build();

        var result = _sut.Execute(new ReadOpenRasterFile.Request(path));

        await Assert.That(result.TryPickValue(out var file, out _)).IsTrue();
        await Assert.That(file!.Groups).HasCount().EqualTo(1);
        await Assert.That(file.Layers).HasCount().EqualTo(2);

        var group = file.Groups[0];
        await Assert.That(group.Name).IsEqualTo("Group1");
        await Assert.That(group.CompositeOperation).IsEqualTo(CompositeOperation.Screen);
        await Assert.That(group.Opacity).IsEqualTo(0.8f);
        await Assert.That(group.Visibility).IsEqualTo(Visibility.Hidden);
        await Assert.That(group.Layers).HasCount().EqualTo(1);
        await Assert.That(group.Layers[0].Name).IsEqualTo("InGroup");

        // Verify bidirectional link
        var inGroup = file.Layers[0];
        await Assert.That(inGroup.Groups).HasCount().EqualTo(1);
        await Assert.That(inGroup.Groups[0].Name).IsEqualTo("Group1");

        File.Delete(path);
    }

    [Test]
    public async Task Execute_ReadsNestedGroups()
    {
        var path = new OraFileBuilder()
            .WithStackXml("""
                <?xml version="1.0" encoding="UTF-8"?>
                <image version="0.0.5" w="64" h="64">
                  <stack>
                    <stack name="Outer">
                      <stack name="Inner">
                        <layer name="Deep" src="data/deep.png" />
                      </stack>
                    </stack>
                  </stack>
                </image>
                """)
            .Build();

        var result = _sut.Execute(new ReadOpenRasterFile.Request(path));

        await Assert.That(result.TryPickValue(out var file, out _)).IsTrue();
        await Assert.That(file!.Groups).HasCount().EqualTo(2);
        await Assert.That(file.Layers).HasCount().EqualTo(1);

        var deep = file.Layers[0];
        await Assert.That(deep.Name).IsEqualTo("Deep");
        await Assert.That(deep.Groups).HasCount().EqualTo(2);

        File.Delete(path);
    }

    [Test]
    public async Task Execute_WithNonexistentFile_ReturnsProblems()
    {
        var result = _sut.Execute(new ReadOpenRasterFile.Request("/nonexistent/file.ora"));

        await Assert.That(result.TryPickProblems(out _)).IsTrue();
    }

    [Test]
    public async Task Execute_WithEmptyPath_ReturnsProblems()
    {
        var result = _sut.Execute(new ReadOpenRasterFile.Request(""));

        await Assert.That(result.TryPickProblems(out _)).IsTrue();
    }
}
