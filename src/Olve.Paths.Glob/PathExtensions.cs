using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace Olve.Paths.Glob;

public static class PathExtensions
{
    public static bool TryGlob(this IPath path, string pattern, [NotNullWhen(true)] out IEnumerable<IPath>? matches, bool ignoreCase = false)
    {
        var stringComparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        Matcher matcher = new(stringComparison);

        matcher.AddInclude(pattern);
        matcher.AddExclude("**/.*");

        if (!path.TryGetAbsolute(out var absolutePath))
        {
            matches = null;
            return false;
        }

        var pathString = absolutePath.Path;

        DirectoryInfo directoryInfo = new(pathString);
        DirectoryInfoWrapper directoryInfoWrapper = new (directoryInfo);

        var result = matcher.Execute(directoryInfoWrapper);

        matches = MapToPaths(result, path);
        return true;
    }

    private static IEnumerable<IPath> MapToPaths(PatternMatchingResult result, IPath rootPath)
    {
        return result.Files.Select(x => MapToPath(x, rootPath));
    }

    private static IPath MapToPath(FilePatternMatch file, IPath rootPath)
    {
        return rootPath / file.Path;
    }
}