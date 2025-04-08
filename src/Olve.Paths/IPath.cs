using System.Diagnostics.CodeAnalysis;

namespace Olve.Paths;

public interface IPath : IPurePath
{
    bool TryGetElementType(out ElementType type);
    ElementType ElementType => TryGetElementType(out var type) ? type : throw new InvalidOperationException("Element does not have a type");

    bool TryGetAbsolute([NotNullWhen(true)] out IPath? absolute);
    IPath Absolute => TryGetAbsolute(out var absolute) ? absolute : throw new InvalidOperationException("Element does not have an absolute path");

    bool TryGetParent([NotNullWhen(true)] out IPath? parent);
    IPath Parent => TryGetParent(out var parent) ? parent : throw new InvalidOperationException("Element does not have a parent");

    bool TryGetChildren([NotNullWhen(true)] out IEnumerable<IPath>? children);
    IEnumerable<IPath> Children => TryGetChildren(out var children) ? children : throw new InvalidOperationException("Element does not have children");

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