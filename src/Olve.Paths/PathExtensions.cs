using System.Diagnostics.CodeAnalysis;

namespace Olve.Paths;

/// <summary>
///     Extension methods for <see cref="IPath"/> and <see cref="IPurePath"/>.
/// </summary>
public static class PathExtensions
{
    /// <summary>
    ///     Determines whether <paramref name="path"/> is <paramref name="root"/> or lies inside it, after evaluating
    ///     <c>.</c> and <c>..</c> on both. The check is lexical: symbolic links are not resolved. Paths on different
    ///     platforms, or an absolute and a non-absolute path, are never within each other.
    /// </summary>
    /// <param name="path">The path to check.</param>
    /// <param name="root">The directory it must stay within.</param>
    /// <returns><see langword="true"/> if <paramref name="path"/> is within <paramref name="root"/>.</returns>
    public static bool IsWithin(this IPurePath path, IPurePath root)
    {
        if (!TryGetNormalized(path, out var pathKey) || !TryGetNormalized(root, out var rootKey))
        {
            return false;
        }

        if (pathKey.Platform != rootKey.Platform
            || pathKey.IsAbsolute != rootKey.IsAbsolute
            || pathKey.Segments.Count < rootKey.Segments.Count)
        {
            return false;
        }

        var comparer = pathKey.Platform == PathPlatform.Windows ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

        return comparer.Equals(pathKey.Drive, rootKey.Drive)
               && rootKey.Segments.Select((segment, i) => comparer.Equals(segment, pathKey.Segments[i])).All(x => x);
    }

    /// <summary>
    ///     Ensures the directory at the given path exists, creating it if necessary.
    /// </summary>
    /// <param name="path">The path to ensure exists as a directory.</param>
    /// <returns><see langword="true"/> if the directory was created; <see langword="false"/> if it already existed.</returns>
    public static bool EnsurePathExists(this IPath path)
    {
        if (path.Exists())
        {
            return false;
        }

        Directory.CreateDirectory(path.Path);

        return true;
    }

    private sealed record NormalizedPath(PathPlatform Platform, string? Drive, bool IsAbsolute, IReadOnlyList<string> Segments);

    private static bool TryGetNormalized(IPurePath path, [NotNullWhen(true)] out NormalizedPath? normalized)
    {
        normalized = null;
        if (path is UnsupportedPurePath or UnsupportedPath)
        {
            return false;
        }

        var (drive, segments) = Path.CreatePure(path.Path, path.Platform) switch
        {
            UnixPurePath unix => (null, unix.Segments),
            WindowsPurePath windows => (windows.DriveLetter?.ToString(), windows.Segments),
            _ => ((string?)null, (IReadOnlyList<string>?)null),
        };
        if (segments is null)
        {
            return false;
        }

        var isAbsolute = path.Type == PathType.Absolute;
        var evaluated = SegmentsHelper.Normalize(segments, isAbsolute);
        if (evaluated is null)
        {
            return false;
        }

        normalized = new NormalizedPath(path.Platform, drive, isAbsolute, evaluated);
        return true;
    }
}
