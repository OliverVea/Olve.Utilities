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

    [Test]
    [Arguments("/srv/runs", "/srv/runs", true)]
    [Arguments("/srv/runs/a/b", "/srv/runs", true)]
    [Arguments("/srv/runs/a/../b", "/srv/runs", true)]
    [Arguments("/srv/runs/../x", "/srv/runs", false)]
    [Arguments("/srv/runs-evil", "/srv/runs", false)]
    [Arguments("/srv", "/srv/runs", false)]
    [Arguments("/srv/a/../runs/x", "/srv/runs", true)]
    [Arguments("/srv/runs/x", "/srv/a/../runs", true)]
    [Arguments("/../../srv/runs/x", "/srv/runs", true)]
    [Arguments("runs/x", "runs", true)]
    [Arguments("./runs/x", "runs", true)]
    [Arguments("../runs/x", "runs", false)]
    [Arguments("runs/x", "/runs", false)]
    public async Task IsWithin_Unix_ReturnsExpectedResult(string path, string root, bool expected)
    {
        // Arrange
        var purePath = Path.CreatePure(path, PathPlatform.Unix);
        var rootPath = Path.CreatePure(root, PathPlatform.Unix);

        // Act
        var actual = purePath.IsWithin(rootPath);

        // Assert
        await Assert.That(actual).IsEqualTo(expected);
    }

    [Test]
    [Arguments(@"C:\srv\runs\a", @"C:\srv\runs", true)]
    [Arguments(@"c:\SRV\Runs\a", @"C:\srv\runs", true)]
    [Arguments(@"D:\srv\runs\a", @"C:\srv\runs", false)]
    [Arguments(@"C:\srv\runs-evil", @"C:\srv\runs", false)]
    [Arguments(@"C:\srv\runs\..\x", @"C:\srv\runs", false)]
    public async Task IsWithin_Windows_ReturnsExpectedResult(string path, string root, bool expected)
    {
        // Arrange
        var purePath = Path.CreatePure(path, PathPlatform.Windows);
        var rootPath = Path.CreatePure(root, PathPlatform.Windows);

        // Act
        var actual = purePath.IsWithin(rootPath);

        // Assert
        await Assert.That(actual).IsEqualTo(expected);
    }

    [Test]
    public async Task IsWithin_AfterAppendingRelativePathThatLeavesRoot_ReturnsFalse()
    {
        // Arrange
        var root = Path.CreatePure("/srv/runs", PathPlatform.Unix);
        var joined = root / Path.CreatePure("../../etc", PathPlatform.Unix);

        // Act
        var actual = joined.IsWithin(root);

        // Assert
        await Assert.That(joined.Path).IsEqualTo("/etc");
        await Assert.That(actual).IsFalse();
    }

    [Test]
    public async Task IsWithin_DifferentPlatforms_ReturnsFalse()
    {
        // Arrange
        var unix = Path.CreatePure("/srv/runs", PathPlatform.Unix);
        var windows = Path.CreatePure(@"C:\srv\runs", PathPlatform.Windows);

        // Act
        var actual = unix.IsWithin(windows);

        // Assert
        await Assert.That(actual).IsFalse();
    }
}
