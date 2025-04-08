namespace Olve.Paths;

/// <summary>
/// Specifies the platform that a path is intended for.
/// </summary>
public enum PathPlatform
{
    /// <summary>
    /// Indicates no specific platform.
    /// </summary>
    None,

    /// <summary>
    /// Indicates a Windows-style path.
    /// </summary>
    Windows,

    /// <summary>
    /// Indicates a Unix-style path.
    /// </summary>
    Unix
}