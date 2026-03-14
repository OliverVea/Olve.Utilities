using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Olve.TinyEXR.Tests;

[SuppressMessage("Usage", "TUnit0033:Intentional")]
public unsafe class ReadmeDemo
{
    [Test]
    public Task SaveAndLoadExr()
    {
        var pixels = new float[] { 1.0f, 0.5f, 0.0f, 1.0f }; // Single RGBA pixel
        var tempFile = Path.GetTempFileName() + ".exr";

        NativeMethods.SaveEXR(pixels, 1, 1, 4, 0, tempFile, out _);

        NativeMethods.LoadEXR(out var rgba, out var width, out var height, tempFile, out _);

        var r = rgba[0]; // 1.0
        var g = rgba[1]; // 0.5
        var b = rgba[2]; // 0.0
        var a = rgba[3]; // 1.0

        NativeMemory.Free(rgba);
        File.Delete(tempFile);

        return Task.CompletedTask;
    }

    [Test]
    public Task LoadFromMemory()
    {
        var pixels = new float[] { 1.0f, 0.0f, 0.0f, 1.0f };
        var tempFile = Path.GetTempFileName() + ".exr";
        NativeMethods.SaveEXR(pixels, 1, 1, 4, 0, tempFile, out _);

        var bytes = File.ReadAllBytes(tempFile);

        var isExr = NativeMethods.IsEXRFromMemory(bytes); // TINYEXR_SUCCESS
        NativeMethods.LoadEXRFromMemory(out var rgba, out var width, out var height, bytes, out _);

        NativeMemory.Free(rgba);
        File.Delete(tempFile);

        return Task.CompletedTask;
    }

    [Test]
    public Task ValidateAndInspect()
    {
        var pixels = new float[] { 1.0f, 0.0f, 0.0f, 1.0f };
        var tempFile = Path.GetTempFileName() + ".exr";
        NativeMethods.SaveEXR(pixels, 1, 1, 4, 0, tempFile, out _);

        var isExr = NativeMethods.IsEXR(tempFile); // TINYEXR_SUCCESS
        NativeMethods.ParseEXRVersionFromFile(out var version, tempFile); // version.version == 2

        File.Delete(tempFile);

        return Task.CompletedTask;
    }

    [Test]
    public Task SaveToMemory()
    {
        var pixels = new float[] { 1.0f, 0.5f, 0.0f, 1.0f };

        var size = NativeMethods.SaveEXRToMemory(pixels, 1, 1, 4, 0, out var buffer, out _);
        var exrBytes = new ReadOnlySpan<byte>(buffer, (int)size);

        // use exrBytes...

        NativeMemory.Free(buffer);

        return Task.CompletedTask;
    }

    [Test]
    public Task ParseHeaderAndLoadImage()
    {
        var pixels = new float[] { 1.0f, 0.0f, 0.0f, 1.0f };
        var tempFile = Path.GetTempFileName() + ".exr";
        NativeMethods.SaveEXR(pixels, 1, 1, 4, 0, tempFile, out _);

        NativeMethods.ParseEXRVersionFromFile(out var version, tempFile);

        EXRHeader header;
        NativeMethods.InitEXRHeader(&header);
        NativeMethods.ParseEXRHeaderFromFile(&header, in version, tempFile, out _);

        var numChannels = header.num_channels; // 4 (RGBA)
        var compression = header.compression_type; // TINYEXR_COMPRESSIONTYPE_ZIP

        EXRImage image;
        NativeMethods.InitEXRImage(&image);
        NativeMethods.LoadEXRImageFromFile(&image, &header, tempFile, out _);

        // use image.images, image.width, image.height ...

        NativeMethods.FreeEXRImage(&image);
        NativeMethods.FreeEXRHeader(&header);
        File.Delete(tempFile);

        return Task.CompletedTask;
    }
}
