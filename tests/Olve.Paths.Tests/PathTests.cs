using System.Diagnostics.CodeAnalysis;

namespace Olve.Paths.Tests;

public class PathTests
{
    [Test]
    [Arguments("/home/user/file.txt", "file.txt")]
    [Arguments("./file.txt", "file.txt")]
    [Arguments("file.txt", "file.txt")]
    [Arguments("./file", "file")]
    [Arguments("/home/user/", null)]
    public async Task TryGetFileName_OnVariousPaths_ReturnsExpectedResult(string filePath, string? expectedFileName)
    {
        // Arrange
        var path = Path.CreatePure(filePath, PathPlatform.Unix);

        // Act
        var gotFileName = path.TryGetFileName(out var fileName);

        // Assert
        await Assert.That(gotFileName).IsEqualTo(expectedFileName != null);
        await Assert.That(fileName).IsEqualTo(expectedFileName);
    }

    [Test]
    [Arguments("/home/user/file.txt", "/home/user/")]
    [Arguments("/home/user/", "/home/")]
    [Arguments("/home/", null)]
    [Arguments("./user/", "./")]
    [Arguments("/home/user", "/home/")]
    public async Task TryGetParent_OnVariousPaths_ReturnsExpectedResults(string filePath, string? expectedParentPath)
    {
        // Arrange
        var path = Path.CreatePure(filePath, PathPlatform.Unix);

        // Act
        var gotParentPath = path.TryGetParent(out var parentPath);
        var parentPathString = parentPath?.ToString();

        // Assert
        await Assert.That(gotParentPath).IsEqualTo(expectedParentPath != null);
        await Assert.That(parentPathString).IsEqualTo(expectedParentPath);
    }

    [Test]
    [Arguments("/home/user/", "file.txt", "/home/user/file.txt")]
    [Arguments("/home/", "user/", "/home/user/")]
    [Arguments("/home/user/", "docs/", "/home/user/docs/")]
    public async Task DivisionOperator_OnVariousPaths_ReturnsExpectedResults(string left, string right, string expected)
    {
        // Arrange
        var leftPath = Path.CreatePure(left, PathPlatform.Unix);

        // Act
        var actualPath = leftPath / right;
        var actual = actualPath.ToString();
        
        // Assert
        await Assert.That(actual).IsEqualTo(expected);
    }
    
    [Test]
    [Arguments("/home/user/file.txt", PathType.Absolute)]
    [Arguments("/", PathType.Absolute)]
    [Arguments("./file.txt", PathType.Relative)]
    [Arguments("../docs/", PathType.Relative)]
    [Arguments("file.txt", PathType.Stub)]
    [Arguments("folder/file.txt", PathType.Stub)]
    [Arguments("", PathType.Stub)]
    public async Task PathType_IsEvaluatedCorrectly(string input, PathType expectedType)
    {
        // Arrange
        var path = Path.CreatePure(input, PathPlatform.Unix);

        // Act
        var actualType = path.Type;

        // Assert
        await Assert.That(actualType).IsEqualTo(expectedType);
    }

    [Test]
    [Arguments("file.txt", "/home/user/file.txt")]
    [Arguments("file", "/home/user/file")]
    [Arguments("dir/", "/home/user/dir/")]
    [Arguments("../", "/home/")]
    public async Task TryGetAbsolute_WithVariousPathsFromHomeDirectory_AreEvaluatedCorrectly(string target, string? expected)
    {
        // Arrange
        ConstantPathEnvironment pathEnvironment = new("/home/user/");
        var path = Path.Create(target, pathEnvironment);
        
        // Act
        var gotAbsolute = path.TryGetAbsolute(out var absolutePath);
        var absolute = absolutePath?.GetFullString();
        
        // Assert
        await Assert.That(gotAbsolute).IsEqualTo(expected != null);
        await Assert.That(absolute).IsEqualTo(expected);
    }

    [Test]
    [Arguments("file.txt")]
    [Arguments("file")]
    [Arguments("dir/")]
    [Arguments("../")]
    public void TryGetAbsolute_WithVariousPathsFromHomeDirectoryAndDefaultPathEnvironment_DoesNotThrow(string target)
    {
        // Arrange
        var path = Path.Create(target);
        
        // Act
        path.TryGetAbsolute(out _);
        
        // Assert
    }

    [Test]
    public async Task GetLinkString_OnValidPath_ReturnsCorrectLinkString()
    {
        // Arrange
        const string pathString = "/home/oliver/file.txt";
        const int line = 17;
        const int column = 6;
        const string expected = "file:///home/oliver/file.txt:17:6";
        var path = Path.Create(pathString);
        
        // Act
        var linkString = path.GetLinkString(line, column);
        
        // Assert
        await Assert.That(linkString).IsEqualTo(expected);
    }

    [Test]
    public async Task GetExecutable_InTestEnvironment_ReturnsPathToExecutableFile()
    {
        // Act
        var canGetExecutable = Path.TryGetAssemblyExecutable(out var path);
        var executableString = path?.GetFullString();
        
        // Assert
        await Assert.That(canGetExecutable).IsTrue();
        await Assert.That(File.Exists(executableString)).IsTrue();
        await Assert.That(path?.Type).IsEqualTo(PathType.Absolute);
    }

    private class ConstantPathEnvironment(string? cwd = null, string? executable = null) : IPathEnvironment
    {
        public bool TryGetCurrentDirectory([NotNullWhen(true)] out string? path)
        {
            path = cwd;
            return cwd != null;
        }

        public bool TryGetAssemblyExecutable([NotNullWhen(true)] out string? path)
        {
            path = executable;
            return executable != null;
        }
    }
}
