using System.Diagnostics.CodeAnalysis;

namespace Olve.Paths;

/// <summary>
/// Represents a path that includes contextual information about its environment, such as its element type and resolved state.
/// </summary>
public interface IPath : IPurePath
{
    /// <summary>
    /// Attempts to retrieve the type of the element represented by the path.
    /// </summary>
    /// <param name="type">When this method returns, contains the type of the element if available; otherwise, <c>ElementType.None</c>.</param>
    /// <returns><c>true</c> if the element type was successfully retrieved; otherwise, <c>false</c>.</returns>
    bool TryGetElementType(out ElementType type);

    /// <summary>
    /// Gets the type of the element represented by the path.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the element type cannot be determined.</exception>
    ElementType ElementType =>
        TryGetElementType(out var type)
            ? type
            : throw new InvalidOperationException("Element does not have a type");

    /// <summary>
    /// Attempts to retrieve the absolute version of the current path.
    /// </summary>
    /// <param name="absolute">When this method returns, contains the absolute path if available; otherwise, <c>null</c>.</param>
    /// <returns><c>true</c> if the absolute path was successfully retrieved; otherwise, <c>false</c>.</returns>
    bool TryGetAbsolute([NotNullWhen(true)] out IPath? absolute);

    /// <summary>
    /// Gets the absolute version of the current path.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the absolute path is not available.</exception>
    IPath Absolute =>
        TryGetAbsolute(out var absolute)
            ? absolute
            : throw new InvalidOperationException("Element does not have an absolute path");

    /// <summary>
    /// Attempts to retrieve the parent path.
    /// </summary>
    /// <param name="parent">When this method returns, contains the parent path if available; otherwise, <c>null</c>.</param>
    /// <returns><c>true</c> if the parent path was successfully retrieved; otherwise, <c>false</c>.</returns>
    bool TryGetParent([NotNullWhen(true)] out IPath? parent);

    /// <summary>
    /// Gets the parent path.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the path has no parent.</exception>
    IPath Parent =>
        TryGetParent(out var parent)
            ? parent
            : throw new InvalidOperationException("Element does not have a parent");

    /// <summary>
    /// Attempts to retrieve the child elements of the current path.
    /// </summary>
    /// <param name="children">When this method returns, contains the child paths if available; otherwise, <c>null</c>.</param>
    /// <returns><c>true</c> if children were successfully retrieved; otherwise, <c>false</c>.</returns>
    bool TryGetChildren([NotNullWhen(true)] out IEnumerable<IPath>? children);

    /// <summary>
    /// Gets the child elements of the current path.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the path has no children.</exception>
    IEnumerable<IPath> Children =>
        TryGetChildren(out var children)
            ? children
            : throw new InvalidOperationException("Element does not have children");

    /// <summary>
    /// Appends the specified path to the current path.
    /// </summary>
    /// <param name="right">The path to append.</param>
    /// <returns>A new <see cref="IPath"/> representing the combined path.</returns>
    IPath AppendPath(IPurePath right);

    /// <summary>
    /// Concatenates two paths using the division operator.
    /// </summary>
    public static IPath operator /(IPath left, IPurePath right) => left.AppendPath(right);

    /// <summary>
    /// Appends the specified string segment to the current path.
    /// </summary>
    /// <param name="right">The string segment to append.</param>
    /// <returns>A new <see cref="IPath"/> representing the combined path.</returns>
    IPath AppendPath(string right);

    /// <summary>
    /// Concatenates the current path with the specified string segment using the division operator.
    /// </summary>
    public static IPath operator /(IPath left, string right) => left.AppendPath(right);

    /// <summary>
    /// Determines whether the path exists in the file system.
    /// </summary>
    /// <returns><c>true</c> if the path exists; otherwise, <c>false</c>.</returns>
    bool Exists();

    /// <summary>
    /// Returns a string representation of the path as a clickable link, optionally including line and column information.
    /// </summary>
    /// <param name="lineNumber">Optional line number to include in the link.</param>
    /// <param name="columnNumber">Optional column number to include in the link.</param>
    /// <returns>A formatted link string.</returns>
    string GetLinkString(int? lineNumber = null, int? columnNumber = null);
}
