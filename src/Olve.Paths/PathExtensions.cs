namespace Olve.Paths;

/// <summary>
///     Extension methods for <see cref="IPath"/>.
/// </summary>
public static class PathExtensions
{
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
}
