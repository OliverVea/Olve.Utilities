using System.Diagnostics.CodeAnalysis;

namespace Olve.TinyEXR.Tests;

[SuppressMessage("Usage", "TUnit0033:Intentional")]
public class ReadmeDemo
{
    [Test]
    public unsafe Task SaveAndLoadExr()
    {
        var pixels = new float[] { 1.0f, 0.5f, 0.0f, 1.0f }; // Single RGBA pixel
        var tempFile = Path.GetTempFileName() + ".exr";

        TinyExr.SaveEXR(pixels, 1, 1, 4, false, tempFile); // Save as 32-bit float EXR

        var loaded = TinyExr.LoadEXR(tempFile, out var width, out var height, out var ptr);

        var r = loaded[0]; // 1.0
        var g = loaded[1]; // 0.5
        var b = loaded[2]; // 0.0
        var a = loaded[3]; // 1.0

        TinyExr.FreeImageData(ptr);
        File.Delete(tempFile);

        return Task.CompletedTask;
    }

    [Test]
    public unsafe Task LoadFromMemory()
    {
        var pixels = new float[] { 1.0f, 0.0f, 0.0f, 1.0f };
        var tempFile = Path.GetTempFileName() + ".exr";
        TinyExr.SaveEXR(pixels, 1, 1, 4, false, tempFile);

        var bytes = File.ReadAllBytes(tempFile);

        var isExr = TinyExr.IsEXRFromMemory(bytes); // true
        var loaded = TinyExr.LoadEXRFromMemory(bytes, out var width, out var height, out var ptr);

        TinyExr.FreeImageData(ptr);
        File.Delete(tempFile);

        return Task.CompletedTask;
    }

    [Test]
    public Task ValidateExrFile()
    {
        var pixels = new float[] { 1.0f, 0.0f, 0.0f, 1.0f };
        var tempFile = Path.GetTempFileName() + ".exr";
        TinyExr.SaveEXR(pixels, 1, 1, 4, false, tempFile);

        var isExr = TinyExr.IsEXR(tempFile); // true
        var version = TinyExr.ParseEXRVersionFromFile(tempFile); // version.version == 2

        File.Delete(tempFile);

        return Task.CompletedTask;
    }
}
