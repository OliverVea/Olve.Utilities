namespace Olve.Paths.Tests;

public class PurePathTests
{
    [Test]
    [Arguments("/home/user/file.txt", "file.txt")]
    [Arguments("./file.txt", "file.txt")]
    [Arguments("file.txt", "file.txt")]
    [Arguments("./file", "file")]
    [Arguments("/home/user", "user")]
    [Arguments("/dir/ ", "dir")]
    public async Task TryGetName_OnVariousPaths_ReturnsExpectedResult(string filePath, string? expectedFileName)
    {
        // Arrange
        var path = Path.CreatePure(filePath, PathPlatform.Unix);

        // Act
        var gotFileName = path.TryGetName(out var fileName);

        // Assert
        await Assert.That(gotFileName).IsEqualTo(expectedFileName != null);
        await Assert.That(fileName).IsEqualTo(expectedFileName);
    }

    [Test]
    [Arguments("/home/user/file.txt", "/home/user")]
    [Arguments("/home/user", "/home")]
    [Arguments("/home", null)]
    [Arguments("./user", ".")]
    [Arguments("/home/user", "/home")]
    public async Task TryGetParent_OnVariousPaths_ReturnsExpectedResults(string filePath, string? expectedParentPath)
    {
        // Arrange
        var path = Path.CreatePure(filePath, PathPlatform.Unix);

        // Act
        var gotParentPath = path.TryGetParentPure(out var parentPath);
        var parentPathString = parentPath?.Path;

        // Assert
        await Assert.That(gotParentPath).IsEqualTo(expectedParentPath != null);
        await Assert.That(parentPathString).IsEqualTo(expectedParentPath);
    }

    [Test]
    [Arguments("/home/user/", "file.txt", "/home/user/file.txt")]
    [Arguments("/home/user", "file.txt", "/home/user/file.txt")]
    [Arguments("/home/user", "docs", "/home/user/docs")]
    [Arguments("/home", "user", "/home/user")]
    public async Task DivisionOperator_OnVariousPaths_ReturnsExpectedResults(string left, string right, string expected)
    {
        // Arrange
        var leftPath = Path.CreatePure(left, PathPlatform.Unix);

        // Act
        var actualPath = leftPath / right;
        var actual = actualPath.Path;
        
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
    [Arguments(".aws", PathType.Stub)]
    [Arguments(".aws/s3/credentials.json", PathType.Stub)]
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
    public async Task GetExecutable_InTestEnvironment_ReturnsPathToExecutableFile()
    {
        // Act
        var canGetExecutable = Path.TryGetAssemblyExecutable(out var path);
        var executableString = path?.Path;
        
        // Assert
        await Assert.That(canGetExecutable).IsTrue();
        await Assert.That(File.Exists(executableString)).IsTrue();
        await Assert.That(path?.Type).IsEqualTo(PathType.Absolute);
    }

    [Test]
    [Arguments(new[] { "/home", "user", "file.txt" }, "/home/user/file.txt")]
    [Arguments(new[] { "/home", "user/file.txt" }, "/home/user/file.txt")]
    public async Task DivisionOperator_ChainedPath_EvaluatesCorrectly(string[] segments, string expected)
    {
        // Arrange
        var path = Path.CreatePure(segments[0], PathPlatform.Unix);

        // Act
        for (int i = 1; i < segments.Length; i++)
        {
            path /= segments[i];
        }

        var pathString = path.Path;

        // Assert
        await Assert.That(pathString).IsEqualTo(expected);
    }
}
