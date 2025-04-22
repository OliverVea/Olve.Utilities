namespace Olve.Paths.Tests;

public class ReadmeDemo
{
    [Test]
    public Task Snippet1()
    {
        if (OperatingSystem.IsWindows()) return WindowsSnippet1();
        if (OperatingSystem.IsLinux()) return UnixSnippet1();
        
        Assert.Fail("Platform not supported");
        return Task.CompletedTask;
    }

    private async Task UnixSnippet1()
    {
        // Snippet 1
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

    private async Task WindowsSnippet1()
    {
        var path = Path.Create(@"C:\users\user\documents");

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
}