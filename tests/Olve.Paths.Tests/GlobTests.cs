using Olve.Paths.Glob;

namespace Olve.Paths.Tests;

public class GlobTests
{
    private const string EverythingPattern = "**/*";

    private static string GetPlatformString(string path)
        => OperatingSystem.IsWindows()
            ? path.Replace("/", "\\")
            : path;

    [Test]
    [Arguments("**/*", new[] {"text-file.txt", "text-file-2.txt", "video-file.avi", "image-file.png", "dir/nested-file.txt"})]
    [Arguments("**/*.txt", new[] {"text-file.txt", "text-file-2.txt", "dir/nested-file.txt"})]
    [Arguments("*.txt", new[] {"text-file.txt", "text-file-2.txt"})]
    [Arguments("*.png", new[] {"image-file.png"})]
    [Arguments("*.avi", new[] {"video-file.avi"})]
    public async Task TryGlob_WithVariousGlobPatterns_ReturnsExpectedResults(string pattern, string[] expectedFiles)
    {
        // Act
        var gotGlob = TryGlobTestData(pattern, out var matches);
        var matchPaths = matches?.Select(match => match.Path).ToList();

        // Assert
        await Assert.That(gotGlob).IsTrue();
        await Assert.That(matchPaths).IsNotNull();
        await Assert.That(matchPaths).HasCount().EqualTo(expectedFiles.Length);

        foreach (var (actual, expectedRaw) in matchPaths!.Order().Zip(expectedFiles.Order()))
        {
            var expected = GetPlatformString(expectedRaw);
            
            await Assert.That(actual).EndsWith(expected);
        }
    }

    [Test]
    public async Task TryGlob_OnEverythingPattern_CorrectNumberOfMatchesIsReturned()
    {
        // Arrange
        const int elementCount = 5;
        
        // Act
        var gotGlob = TryGlobTestData(EverythingPattern, out var matches);
        matches = matches?.ToList();

        // Assert
        await Assert.That(gotGlob).IsTrue();
        await Assert.That(matches).IsNotNull();
        await Assert.That(matches).HasCount().EqualTo(elementCount);
    }

    [Test]
    public async Task TryGlob_OnEverythingPattern_MatchesAreAbsolutePaths()
    {
        // Act
        TryGlobTestData(EverythingPattern, out var matches);

        // Assert
        foreach (var match in matches!)
        {
            await Assert.That(match.Type).IsEqualTo(PathType.Absolute);
        }
    }

    [Test]
    public async Task TryGlob_OnEverythingPattern_MatchesAreFiles()
    {
        // Act
        TryGlobTestData(EverythingPattern, out var matches);

        // Assert
        foreach (var match in matches!)
        {
            await Assert.That(match.ElementType).IsEqualTo(ElementType.File);
        }
    }

    [Test]
    public async Task TryGlob_OnEverythingPattern_MatchesExist()
    {
        // Act
        TryGlobTestData(EverythingPattern, out var matches);

        // Assert
        foreach (var match in matches!)
        {
            var exists = File.Exists(match.Path);
            await Assert.That(exists).IsTrue();
        }
    }

    [Test]
    [Arguments(true, 1)]
    [Arguments(false, 0)]
    public async Task TryGlob_WithIgnoreAndDoNotIgnoreCase_ReturnsExpectedResults(bool ignoreCase, int expected)
    {
        // Arrange
        const string patternWithIncorrectCasing = "**/Text-file.txt";

        // Act
        TryGlobTestData(patternWithIncorrectCasing, out var ignoreCaseMatches, ignoreCase);

        // Assert
        await Assert.That(ignoreCaseMatches).HasCount().EqualTo(expected);
    }

    private static bool TryGlobTestData(string pattern, out IEnumerable<IPath>? matches, bool ignoreCase = false)
    {
        if (!Path.TryGetAssemblyExecutable(out var assemblyPath))
        {
            Assert.Fail("Assembly executable path not found.");
            matches = null;
            return false;
        }

        var path = assemblyPath.Parent / "testdata";

        return path.TryGlob(pattern, out matches, ignoreCase);
    }
}