namespace Olve.Paths;

/// <summary>
/// Describes the type of a path string.
/// </summary>
public enum PathType
{
    /// <summary>
    /// A stub path, not rooted.
    /// </summary>
    Stub,

    /// <summary>
    /// A relative path.
    /// </summary>
    Relative,

    /// <summary>
    /// An absolute path.
    /// </summary>
    Absolute
}