using System.Diagnostics.CodeAnalysis;

namespace Olve.Paths;

public interface IPath : IPurePath
{
    bool TryGetElementType(out ElementType type);

    bool TryGetAbsolute([NotNullWhen(true)] out IPath? absolute);

    bool TryGetParent([NotNullWhen(true)] out IPath? parent);
    IPath Parent => TryGetParent(out var parent) ? parent : throw new InvalidOperationException("Element does not have a parent");

    IPath AppendPath(IPurePath right);
    static IPath operator /(IPath left, IPurePath right) => left.AppendPath(right);

    IPath AppendPath(string right);
    static IPath operator /(IPath left, string right) => left.AppendPath(right);

    string GetLinkString(int? lineNumber = null, int? columnNumber = null);
}

public enum ElementType
{
    None,
    Directory,
    File,
}