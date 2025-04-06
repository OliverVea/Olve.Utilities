using System.Diagnostics.CodeAnalysis;

namespace Olve.Results;

/// <summary>
///     Represents the origin of a problem.
/// </summary>
/// <param name="FilePath">The path to the file where the problem originated.</param>
/// <param name="LineNumber">The line number in the file where the problem originated.</param>
public readonly record struct ProblemOriginInformation(string FilePath, int LineNumber) {
    /// <summary>
    ///     The prefix for file links.
    /// </summary>
    public const string FileLinkPrefix = "file://";
    
    /// <summary>
    /// Attempts to get a link string for the current platform.
    /// </summary>
    /// <example>file:///home/user/file.txt</example>
    /// <param name="link">The link string for the current platform.</param>
    /// <returns>True if the link string was successfully generated; otherwise, false.</returns>
    public bool TryGetLinkStringForPlatform([NotNullWhen(true)] out string? link) {
        if (OperatingSystem.IsWindows()) {
            link = $"file:///{FilePath.Replace('\\', '/')}";
            return true;
        }

        if (OperatingSystem.IsLinux()) {
            link = FileLinkPrefix + FilePath;
            return true;
        }

        if (OperatingSystem.IsMacOS()) {
            link = FileLinkPrefix + FilePath;
            return true;
        }

        link = null;
        return false;
    }
    
    /// <summary>
    ///     Gets a link string for the current platform or a default value if the platform is not supported.
    /// </summary>
    /// <param name="defaultLink">The default link string to return if the platform is not supported.</param>
    /// <returns>The link string for the current platform or the default value.</returns>
    public string? GetLinkStringForPlatformOrDefault(string? defaultLink = null)
        => TryGetLinkStringForPlatform(out var link) ? link : defaultLink;
    
    /// <summary>
    ///     Gets a link string for the current platform or throws an exception if the platform is not supported.
    /// </summary>
    /// <returns>The link string for the current platform.</returns>
    /// <exception cref="PlatformNotSupportedException">When the platform is not supported.</exception>
    public string GetLinkStringForPlatform()
        => TryGetLinkStringForPlatform(out var link) ? link : throw new PlatformNotSupportedException();
}
