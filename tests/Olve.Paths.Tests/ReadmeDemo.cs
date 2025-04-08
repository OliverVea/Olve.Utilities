namespace Olve.Paths.Tests;

public class ReadmeDemo
{
    [Test]
    public async Task Snippet1()
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
}