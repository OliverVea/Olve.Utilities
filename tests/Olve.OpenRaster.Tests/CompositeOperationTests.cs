namespace Olve.OpenRaster.Tests;

public class CompositeOperationTests
{
    [Test]
    [Arguments("svg:src-over", BlendingFunction.Normal, CompositingOperator.SourceOver)]
    [Arguments("svg:multiply", BlendingFunction.Multiply, CompositingOperator.SourceOver)]
    [Arguments("svg:screen", BlendingFunction.Screen, CompositingOperator.SourceOver)]
    [Arguments("svg:overlay", BlendingFunction.Overlay, CompositingOperator.SourceOver)]
    [Arguments("svg:darken", BlendingFunction.Darken, CompositingOperator.SourceOver)]
    [Arguments("svg:lighten", BlendingFunction.Lighten, CompositingOperator.SourceOver)]
    [Arguments("svg:color-dodge", BlendingFunction.ColorDodge, CompositingOperator.SourceOver)]
    [Arguments("svg:color-burn", BlendingFunction.ColorBurn, CompositingOperator.SourceOver)]
    [Arguments("svg:hard-light", BlendingFunction.HardLight, CompositingOperator.SourceOver)]
    [Arguments("svg:soft-light", BlendingFunction.SoftLight, CompositingOperator.SourceOver)]
    [Arguments("svg:difference", BlendingFunction.Difference, CompositingOperator.SourceOver)]
    [Arguments("svg:color", BlendingFunction.Color, CompositingOperator.SourceOver)]
    [Arguments("svg:luminosity", BlendingFunction.Luminosity, CompositingOperator.SourceOver)]
    [Arguments("svg:hue", BlendingFunction.Hue, CompositingOperator.SourceOver)]
    [Arguments("svg:saturation", BlendingFunction.Saturation, CompositingOperator.SourceOver)]
    [Arguments("svg:plus", BlendingFunction.Normal, CompositingOperator.Lighter)]
    [Arguments("svg:dst-in", BlendingFunction.Normal, CompositingOperator.DestinationIn)]
    [Arguments("svg:dst-out", BlendingFunction.Normal, CompositingOperator.DestinationOut)]
    [Arguments("svg:src-atop", BlendingFunction.Normal, CompositingOperator.SourceAtop)]
    [Arguments("svg:dst-atop", BlendingFunction.Normal, CompositingOperator.DestinationAtop)]
    public async Task FromKey_WithValidKey_ReturnsExpectedOperation(
        string key,
        BlendingFunction expectedBlend,
        CompositingOperator expectedCompositing)
    {
        var result = CompositeOperation.FromKey(key);

        await Assert.That(result.TryPickValue(out var op, out _)).IsTrue();
        await Assert.That(op.Key).IsEqualTo(key);
        await Assert.That(op.BlendingFunction).IsEqualTo(expectedBlend);
        await Assert.That(op.CompositingOperator).IsEqualTo(expectedCompositing);
    }

    [Test]
    public async Task FromKey_WithUnknownKey_ReturnsProblems()
    {
        var result = CompositeOperation.FromKey("svg:unknown");

        await Assert.That(result.TryPickProblems(out _)).IsTrue();
    }

    [Test]
    public async Task StaticProperties_MatchFromKey()
    {
        await Assert.That(CompositeOperation.SrcOver).IsEqualTo(CompositeOperation.FromKey("svg:src-over").Value);
        await Assert.That(CompositeOperation.Multiply).IsEqualTo(CompositeOperation.FromKey("svg:multiply").Value);
        await Assert.That(CompositeOperation.Plus).IsEqualTo(CompositeOperation.FromKey("svg:plus").Value);
        await Assert.That(CompositeOperation.DestinationIn).IsEqualTo(CompositeOperation.FromKey("svg:dst-in").Value);
    }

    [Test]
    public async Task AllCompositeOperations_AreReadFromFile()
    {
        var path = new OraFileBuilder()
            .WithStackXml("""
                <?xml version="1.0" encoding="UTF-8"?>
                <image version="0.0.5" w="64" h="64">
                  <stack>
                    <layer name="multiply" src="data/l.png" composite-op="svg:multiply" />
                    <layer name="screen" src="data/l.png" composite-op="svg:screen" />
                    <layer name="plus" src="data/l.png" composite-op="svg:plus" />
                    <layer name="dst-in" src="data/l.png" composite-op="svg:dst-in" />
                  </stack>
                </image>
                """)
            .Build();

        var result = new ReadOpenRasterFile().Execute(new ReadOpenRasterFile.Request(path));

        await Assert.That(result.TryPickValue(out var file, out _)).IsTrue();
        await Assert.That(file!.Layers[0].CompositeOperation).IsEqualTo(CompositeOperation.Multiply);
        await Assert.That(file.Layers[1].CompositeOperation).IsEqualTo(CompositeOperation.Screen);
        await Assert.That(file.Layers[2].CompositeOperation).IsEqualTo(CompositeOperation.Plus);
        await Assert.That(file.Layers[3].CompositeOperation).IsEqualTo(CompositeOperation.DestinationIn);

        File.Delete(path);
    }
}
