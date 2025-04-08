using System.Diagnostics.CodeAnalysis;

namespace Olve.Paths.Tests;

public class PathTests
{
    [Test]
    [Arguments("file.txt", "/home/user/file.txt")]
    [Arguments("file", "/home/user/file")]
    [Arguments("dir/", "/home/user/dir")]
    [Arguments("../", "/home")]
    public async Task TryGetAbsolute_WithVariousPathsFromHomeDirectory_AreEvaluatedCorrectly(string target, string? expected)
    {
        // Arrange
        ConstantPathEnvironment pathEnvironment = new("/home/user/");
        var path = Path.Create(target, pathEnvironment);

        // Act
        var gotAbsolute = path.TryGetAbsolute(out var absolutePath);
        var absolute = absolutePath?.Path;

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
    [Arguments("doesnotexist", ElementType.None)]
    [Arguments("testdata", ElementType.Directory)]
    [Arguments("testdata/text-file.txt", ElementType.File)]
    public async Task TryGetElementType_OnVariousPaths_ReturnsExpectedResults(string pathString, ElementType expected)
    {
        // Arrange
        if (!Path.TryGetAssemblyExecutable(out var assemblyPath))
        {
            Assert.Fail("Assembly executable path not found.");
        }

        var path = assemblyPath.Parent / pathString;

        // Act
        var gotElementType = path.TryGetElementType(out var elementType);

        // Assert
        await Assert.That(gotElementType).IsEqualTo(expected != ElementType.None);
        await Assert.That(elementType).IsEqualTo(expected);
    }

    [Test]
    public async Task TryGetChildren_OnValidDirectory_ReturnsExpectedChildren()
    {
        // Arrange
        string[] expectedElements = ["text-file.txt", "text-file-2.txt", "video-file.avi", "image-file.png", "dir"];

        if (!Path.TryGetAssemblyExecutable(out var assemblyPath))
        {
            Assert.Fail("Assembly executable path not found.");
        }

        var path = assemblyPath.Parent / "testdata";

        // Act
        var gotChildren = path.TryGetChildren(out var children);
        var childPaths = children?.Select(child => child.Name).ToList();

        // Assert
        await Assert.That(gotChildren).IsTrue();
        await Assert.That(childPaths).IsNotNull();
        await Assert.That(childPaths).HasCount().EqualTo(expectedElements.Length);

        foreach (var (actual, expected) in childPaths!.Order().Zip(expectedElements.Order()))
        {
            await Assert.That(actual).EndsWith(expected);
        }
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