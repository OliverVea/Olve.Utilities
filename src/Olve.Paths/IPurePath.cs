using System.Diagnostics.CodeAnalysis;

namespace Olve.Paths;

/// <summary>
/// Represents an immutable and pure file system path that is independent of the actual file system.
/// </summary>
public interface IPurePath
{
    /// <summary>
    /// Gets the string representation of the path.
    /// </summary>
    string Path { get; }

    /// <summary>
    /// Gets the platform of the path (e.g., Unix, Windows).
    /// </summary>
    PathPlatform Platform { get; }

    /// <summary>
    /// Gets the type of the path (e.g., file, directory).
    /// </summary>
    PathType Type { get; }

    /// <summary>
    /// Attempts to retrieve the parent path.
    /// </summary>
    /// <param name="parent">When this method returns, contains the parent path if available; otherwise, <c>null</c>.</param>
    /// <returns><c>true</c> if the parent path was successfully retrieved; otherwise, <c>false</c>.</returns>
    bool TryGetParentPure([NotNullWhen(true)] out IPurePath? parent);

    /// <summary>
    /// Gets the parent path.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the path has no parent.</exception>
    IPurePath ParentPure =>
        TryGetParentPure(out var parent)
            ? parent
            : throw new InvalidOperationException("Element does not have a parent");

    /// <summary>
    /// Attempts to retrieve the name of the path component (e.g., filename or directory name).
    /// </summary>
    /// <param name="fileName">When this method returns, contains the name of the path component if available; otherwise, <c>null</c>.</param>
    /// <returns><c>true</c> if the name was successfully retrieved; otherwise, <c>false</c>.</returns>
    bool TryGetName([NotNullWhen(true)] out string? fileName);

    /// <summary>
    /// Gets the name of the path component, or <c>null</c> if unavailable.
    /// </summary>
    string? Name => TryGetName(out var fileName) ? fileName : null;

    /// <summary>
    /// Appends another pure path to the current path.
    /// </summary>
    /// <param name="right">The path to append.</param>
    /// <returns>A new <see cref="IPurePath"/> instance representing the combined path.</returns>
    IPurePath Append(IPurePath right);

    /// <summary>
    /// Appends a string segment to the current path.
    /// </summary>
    /// <param name="right">The segment to append.</param>
    /// <returns>A new <see cref="IPurePath"/> instance representing the combined path.</returns>
    IPurePath Append(string right);

    /// <summary>
    /// Concatenates two pure paths using the division operator.
    /// </summary>
    public static IPurePath operator /(IPurePath left, IPurePath right) => left.Append(right);

    /// <summary>
    /// Concatenates a path with a string segment using the division operator.
    /// </summary>
    public static IPurePath operator /(IPurePath left, string right) => left.Append(right);
}
