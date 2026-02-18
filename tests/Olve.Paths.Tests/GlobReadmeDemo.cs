using Olve.Paths.Glob;

namespace Olve.Paths.Tests;

public class GlobReadmeDemo
{
    private static IPath GetTestDataPath()
    {
        Path.TryGetAssemblyExecutable(out var assemblyPath);
        return assemblyPath!.Parent / "testdata";
    }

    [Test]
    public async Task BasicGlob()
    {
        var path = GetTestDataPath();

        if (path.TryGlob("*.txt", out var matches))
        {
            foreach (var match in matches)
                Console.WriteLine(match.Path);
        }

        // assert
        await Assert.That(matches).IsNotNull();
    }

    [Test]
    public async Task RecursiveGlob()
    {
        var path = GetTestDataPath();

        // All .txt files in any subdirectory
        path.TryGlob("**/*.txt", out var sourceFiles);

        // assert
        await Assert.That(sourceFiles).IsNotNull();
    }

    [Test]
    public async Task CaseInsensitiveGlob()
    {
        var path = GetTestDataPath();

        path.TryGlob("*.TXT", out var matches, ignoreCase: true);

        // assert
        await Assert.That(matches).IsNotNull();
    }
}
