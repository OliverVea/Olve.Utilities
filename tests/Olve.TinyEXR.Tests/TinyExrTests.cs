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

        TinyExr.SaveEXR(pixels, width, height, 4, false, filePath);

        int w, h;
        float[] loadedCopy;
        unsafe
        {
            var loaded = TinyExr.LoadEXR(filePath, out w, out h, out var ptr);
            loadedCopy = loaded.ToArray();
            TinyExr.FreeImageData(ptr);
        }

        await Assert.That(w).IsEqualTo(width);
        await Assert.That(h).IsEqualTo(height);
        await Assert.That(loadedCopy).HasCount().EqualTo(width * height * 4);
        await Assert.That(loadedCopy[0]).IsEqualTo(1.0f);
        await Assert.That(loadedCopy[1]).IsEqualTo(0.5f);
        await Assert.That(loadedCopy[2]).IsEqualTo(0.0f);
        await Assert.That(loadedCopy[3]).IsEqualTo(1.0f);
    }

    [Test]
    public async Task IsEXR_WithValidFile_ReturnsTrue()
    {
        var filePath = Path.Combine(_tempDir, "valid.exr");
        TinyExr.SaveEXR(CreateTestPixels(2, 2), 2, 2, 4, false, filePath);

        var result = TinyExr.IsEXR(filePath);

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task IsEXR_WithInvalidFile_ReturnsFalse()
    {
        var filePath = Path.Combine(_tempDir, "invalid.exr");
        await File.WriteAllTextAsync(filePath, "not an exr file");

        var result = TinyExr.IsEXR(filePath);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task IsEXRFromMemory_WithValidData_ReturnsTrue()
    {
        var filePath = Path.Combine(_tempDir, "valid.exr");
        TinyExr.SaveEXR(CreateTestPixels(2, 2), 2, 2, 4, false, filePath);
        var bytes = await File.ReadAllBytesAsync(filePath);

        var result = TinyExr.IsEXRFromMemory(bytes);

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task IsEXRFromMemory_WithInvalidData_ReturnsFalse()
    {
        var result = TinyExr.IsEXRFromMemory("not an exr"u8);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task ParseEXRVersionFromFile_WithValidFile_ReturnsVersion()
    {
        var filePath = Path.Combine(_tempDir, "version.exr");
        TinyExr.SaveEXR(CreateTestPixels(2, 2), 2, 2, 4, false, filePath);

        var version = TinyExr.ParseEXRVersionFromFile(filePath);

        await Assert.That(version.version).IsEqualTo(2);
    }

    [Test]
    public async Task ParseEXRVersionFromMemory_WithValidData_ReturnsVersion()
    {
        var filePath = Path.Combine(_tempDir, "version.exr");
        TinyExr.SaveEXR(CreateTestPixels(2, 2), 2, 2, 4, false, filePath);
        var bytes = await File.ReadAllBytesAsync(filePath);

        var version = TinyExr.ParseEXRVersionFromMemory(bytes);

        await Assert.That(version.version).IsEqualTo(2);
    }

    [Test]
    public async Task LoadEXRFromMemory_WithValidData_LoadsSuccessfully()
    {
        var filePath = Path.Combine(_tempDir, "memory.exr");
        const int width = 2;
        const int height = 2;
        TinyExr.SaveEXR(CreateTestPixels(width, height), width, height, 4, false, filePath);
        var bytes = await File.ReadAllBytesAsync(filePath);

        int w, h;
        int loadedLength;
        unsafe
        {
            var loaded = TinyExr.LoadEXRFromMemory(bytes, out w, out h, out var ptr);
            loadedLength = loaded.Length;
            TinyExr.FreeImageData(ptr);
        }

        await Assert.That(w).IsEqualTo(width);
        await Assert.That(h).IsEqualTo(height);
        await Assert.That(loadedLength).IsEqualTo(width * height * 4);
    }

    [Test]
    public unsafe Task SaveEXRToMemory_AndFreeBuffer_DoesNotThrow()
    {
        var pixels = CreateTestPixels(2, 2);

        var data = TinyExr.SaveEXRToMemory(pixels, 2, 2, 4, false, out var ptr);
        TinyExr.FreeMemoryBuffer(ptr);

        return Task.CompletedTask;
    }

    [Test]
    public async Task SaveEXR_WithFp16_RoundTripsSuccessfully()
    {
        const int width = 2;
        const int height = 2;
        var pixels = CreateTestPixels(width, height);
        var filePath = Path.Combine(_tempDir, "fp16.exr");

        TinyExr.SaveEXR(pixels, width, height, 4, true, filePath);

        int w, h;
        unsafe
        {
            TinyExr.LoadEXR(filePath, out w, out h, out var ptr);
            TinyExr.FreeImageData(ptr);
        }

        await Assert.That(w).IsEqualTo(width);
        await Assert.That(h).IsEqualTo(height);
    }

    [Test]
    public Task LoadEXR_WithNonexistentFile_ThrowsTinyExrException()
    {
        Assert.Throws<TinyExrException>(() =>
        {
            unsafe
            {
                TinyExr.LoadEXR("/nonexistent/file.exr", out _, out _, out _);
            }
        });

        return Task.CompletedTask;
    }
}
