using System.Runtime.InteropServices;

namespace Olve.TinyEXR.Tests;

public class TinyExrTests
{
    private string _tempDir = null!;

    [Before(Test)]
    public void Setup()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"tinyexr-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    [After(Test)]
    public void Cleanup()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    private static float[] CreateTestPixels(int width, int height)
    {
        var pixels = new float[width * height * 4];
        for (var i = 0; i < width * height; i++)
        {
            pixels[i * 4 + 0] = 1.0f; // R
            pixels[i * 4 + 1] = 0.5f; // G
            pixels[i * 4 + 2] = 0.0f; // B
            pixels[i * 4 + 3] = 1.0f; // A
        }

        return pixels;
    }

    [Test]
    public async Task SaveEXR_AndLoadEXR_RoundTripsSuccessfully()
    {
        const int width = 4;
        const int height = 4;
        var pixels = CreateTestPixels(width, height);
        var filePath = Path.Combine(_tempDir, "test.exr");

        int w, h;
        float r, g, b, a;
        unsafe
        {
            NativeMethods.SaveEXR(pixels, width, height, 4, 0, filePath, out _);
            NativeMethods.LoadEXR(out var rgba, out w, out h, filePath, out _);
            r = rgba[0];
            g = rgba[1];
            b = rgba[2];
            a = rgba[3];
            NativeMemory.Free(rgba);
        }

        await Assert.That(w).IsEqualTo(width);
        await Assert.That(h).IsEqualTo(height);
        await Assert.That(r).IsEqualTo(1.0f);
        await Assert.That(g).IsEqualTo(0.5f);
        await Assert.That(b).IsEqualTo(0.0f);
        await Assert.That(a).IsEqualTo(1.0f);
    }

    [Test]
    public async Task IsEXR_WithValidFile_ReturnsSuccess()
    {
        var filePath = Path.Combine(_tempDir, "valid.exr");
        unsafe { NativeMethods.SaveEXR(CreateTestPixels(2, 2), 2, 2, 4, 0, filePath, out _); }

        var result = NativeMethods.IsEXR(filePath);

        await Assert.That(result).IsEqualTo(TinyExrConstants.TINYEXR_SUCCESS);
    }

    [Test]
    public async Task IsEXR_WithInvalidFile_ReturnsNonSuccess()
    {
        var filePath = Path.Combine(_tempDir, "invalid.exr");
        await File.WriteAllTextAsync(filePath, "not an exr file");

        var result = NativeMethods.IsEXR(filePath);

        await Assert.That(result).IsNotEqualTo(TinyExrConstants.TINYEXR_SUCCESS);
    }

    [Test]
    public async Task IsEXRFromMemory_WithValidData_ReturnsSuccess()
    {
        var filePath = Path.Combine(_tempDir, "valid.exr");
        unsafe { NativeMethods.SaveEXR(CreateTestPixels(2, 2), 2, 2, 4, 0, filePath, out _); }
        var bytes = await File.ReadAllBytesAsync(filePath);

        var result = NativeMethods.IsEXRFromMemory(bytes);

        await Assert.That(result).IsEqualTo(TinyExrConstants.TINYEXR_SUCCESS);
    }

    [Test]
    public async Task IsEXRFromMemory_WithInvalidData_ReturnsNonSuccess()
    {
        var result = NativeMethods.IsEXRFromMemory("not an exr"u8);

        await Assert.That(result).IsNotEqualTo(TinyExrConstants.TINYEXR_SUCCESS);
    }

    [Test]
    public async Task ParseEXRVersionFromFile_WithValidFile_ReturnsVersion()
    {
        var filePath = Path.Combine(_tempDir, "version.exr");
        unsafe { NativeMethods.SaveEXR(CreateTestPixels(2, 2), 2, 2, 4, 0, filePath, out _); }

        var ret = NativeMethods.ParseEXRVersionFromFile(out var version, filePath);

        await Assert.That(ret).IsEqualTo(TinyExrConstants.TINYEXR_SUCCESS);
        await Assert.That(version.version).IsEqualTo(2);
    }

    [Test]
    public async Task ParseEXRVersionFromMemory_WithValidData_ReturnsVersion()
    {
        var filePath = Path.Combine(_tempDir, "version.exr");
        unsafe { NativeMethods.SaveEXR(CreateTestPixels(2, 2), 2, 2, 4, 0, filePath, out _); }
        var bytes = await File.ReadAllBytesAsync(filePath);

        var ret = NativeMethods.ParseEXRVersionFromMemory(out var version, bytes);

        await Assert.That(ret).IsEqualTo(TinyExrConstants.TINYEXR_SUCCESS);
        await Assert.That(version.version).IsEqualTo(2);
    }

    [Test]
    public async Task LoadEXRFromMemory_WithValidData_LoadsSuccessfully()
    {
        var filePath = Path.Combine(_tempDir, "memory.exr");
        const int width = 2;
        const int height = 2;
        unsafe { NativeMethods.SaveEXR(CreateTestPixels(width, height), width, height, 4, 0, filePath, out _); }
        var bytes = await File.ReadAllBytesAsync(filePath);

        int w, h;
        unsafe
        {
            NativeMethods.LoadEXRFromMemory(out var rgba, out w, out h, bytes, out _);
            NativeMemory.Free(rgba);
        }

        await Assert.That(w).IsEqualTo(width);
        await Assert.That(h).IsEqualTo(height);
    }

    [Test]
    public async Task SaveEXRToMemory_ReturnsPositiveSize()
    {
        var pixels = CreateTestPixels(2, 2);
        int size;
        unsafe
        {
            size = NativeMethods.SaveEXRToMemory(pixels, 2, 2, 4, 0, out var buffer, out _);
            NativeMemory.Free(buffer);
        }

        await Assert.That(size).IsGreaterThan(0);
    }

    [Test]
    public async Task SaveEXR_WithFp16_RoundTripsSuccessfully()
    {
        const int width = 2;
        const int height = 2;
        var pixels = CreateTestPixels(width, height);
        var filePath = Path.Combine(_tempDir, "fp16.exr");

        int w, h;
        unsafe
        {
            NativeMethods.SaveEXR(pixels, width, height, 4, 1, filePath, out _);
            NativeMethods.LoadEXR(out var rgba, out w, out h, filePath, out _);
            NativeMemory.Free(rgba);
        }

        await Assert.That(w).IsEqualTo(width);
        await Assert.That(h).IsEqualTo(height);
    }

    [Test]
    public async Task LoadEXR_WithNonexistentFile_ReturnsError()
    {
        int ret;
        unsafe
        {
            ret = NativeMethods.LoadEXR(out _, out _, out _, "/nonexistent/file.exr", out var err);
            NativeMethods.FreeEXRErrorMessage(err);
        }

        await Assert.That(ret).IsNotEqualTo(TinyExrConstants.TINYEXR_SUCCESS);
    }
}
