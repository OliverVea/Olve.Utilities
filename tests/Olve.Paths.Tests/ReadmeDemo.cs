namespace Olve.Paths.Tests;

public class ReadmeDemo
{
    [Test, LinuxOnly]
    public async Task Snippet1_Linux()
    {
        var path = Path.Create("/home/user/documents"); // /home/user/documents

        var parent = path.Parent; // /home/user
        var folderName = path.Name; // documents

        var newPath = path / "newfile.txt"; // /home/user/documents/newfile.txt
        var exists = newPath.Exists(); // Check if the file exists

        // Assert
        await Assert.That(parent.Path).IsEqualTo("/home/user");
        await Assert.That(folderName).IsEqualTo("documents");
        await Assert.That(newPath.Path).IsEqualTo("/home/user/documents/newfile.txt");

        var actuallyExists = File.Exists(newPath.Path);
        await Assert.That(exists).IsEqualTo(actuallyExists);
    }

    [Test, WindowsOnly]
    public async Task Snippet1_Windows()
    {
        var path = Path.Create(@"C:\users\user\documents"); // C:\users\user\documents

        var parent = path.Parent; // C:\users\user
        var folderName = path.Name; // documents

        var newPath = path / "newfile.txt"; // C:\users\user\documents\newfile.txt
        var exists = newPath.Exists(); // Check if the file exists

        // Assert
        await Assert.That(parent.Path).IsEqualTo(@"C:\users\user");
        await Assert.That(folderName).IsEqualTo("documents");
        await Assert.That(newPath.Path).IsEqualTo(@"C:\users\user\documents\newfile.txt");

        var actuallyExists = File.Exists(newPath.Path);
        await Assert.That(exists).IsEqualTo(actuallyExists);
    }

    [Test]
    public async Task Snippet2()
    {
        // Pure paths allow platform-independent path manipulation
        var unixPath = Path.CreatePure("/home/user/docs", PathPlatform.Unix);
        var windowsPath = Path.CreatePure(@"C:\users\user\docs", PathPlatform.Windows);

        var unixPlatform = unixPath.Platform; // PathPlatform.Unix
        var windowsPlatform = windowsPath.Platform; // PathPlatform.Windows

        // Assert
        await Assert.That(unixPlatform).IsEqualTo(PathPlatform.Unix);
        await Assert.That(windowsPlatform).IsEqualTo(PathPlatform.Windows);
        await Assert.That(unixPath.Path).IsEqualTo("/home/user/docs");
        await Assert.That(windowsPath.Path).IsEqualTo(@"C:\users\user\docs");
    }

    [Test]
    public async Task Snippet3()
    {
        // Filesystem operations
        var tempDir = Path.Create(System.IO.Path.GetTempPath()) / "olve-paths-demo";
        tempDir.EnsurePathExists(); // Creates the directory if it doesn't exist

        var exists = tempDir.Exists(); // true
        var elementType = tempDir.ElementType; // ElementType.Directory

        if (tempDir.TryGetChildren(out var children))
        {
            foreach (var child in children)
            {
                Console.WriteLine(child.Path);
            }
        }

        // Cleanup
        Directory.Delete(tempDir.Path, true);

        // Assert
        await Assert.That(exists).IsTrue();
        await Assert.That(elementType).IsEqualTo(ElementType.Directory);
    }
}
