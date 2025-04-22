using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace Olve.Paths.Glob;

/// <summary>
/// Provides extension methods for glob pattern matching on <see cref="IPath"/> instances.
/// </summary>
public static class PathExtensions
{
    /// <summary>
    /// Attempts to find files matching a glob pattern within the specified directory path.
    /// </summary>
    /// <param name="path">The root directory to search within.</param>
    /// <param name="pattern">The glob pattern to apply. Example: <c>**&#47;*.txt</c></param>
    /// <param name="matches">
    /// When this method returns <c>true</c>, contains the matched file paths relative to <paramref name="path"/>.
    /// If no matches were found or the path could not be resolved to an absolute path, this will be <c>null</c>.
    /// </param>
    /// <param name="ignoreCase">
    /// Determines whether pattern matching should ignore case. Defaults to <c>false</c>.
    /// </param>
    /// <returns><c>true</c> if the path was absolute and any matches were found; otherwise, <c>false</c>.</returns>
    public static bool TryGlob(
        this IPath path,
        string pattern,
        [NotNullWhen(true)] out IEnumerable<IPath>? matches,
        bool ignoreCase = false
    )
    {
        var stringComparison = ignoreCase
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;
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
        DirectoryInfoWrapper directoryInfoWrapper = new(directoryInfo);

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
