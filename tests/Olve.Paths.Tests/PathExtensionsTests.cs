namespace Olve.Paths.Tests;

public class PathExtensionsTests
{
    [Test]
    public async Task EnsurePathExists_WhenDirectoryDoesNotExist_CreatesAndReturnsTrue()
    {
        var tempDir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
        var path = Path.Create(tempDir);

        try
        {
            var created = path.EnsurePathExists();

            await Assert.That(created).IsTrue();
            await Assert.That(Directory.Exists(tempDir)).IsTrue();
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir);
            }
        }
    }

    [Test]
    public async Task EnsurePathExists_WhenDirectoryExists_ReturnsFalse()
    {
        var tempDir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var path = Path.Create(tempDir);

        try
        {
            var created = path.EnsurePathExists();

            await Assert.That(created).IsFalse();
        }
        finally
        {
            Directory.Delete(tempDir);
        }
    }
}
