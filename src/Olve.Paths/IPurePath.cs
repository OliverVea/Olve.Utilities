using System.Diagnostics.CodeAnalysis;

namespace Olve.Paths;

public interface IPurePath
{
    string Path { get; }

    PathPlatform Platform { get; }
    PathType Type { get; }

    bool TryGetParentPure([NotNullWhen(true)] out IPurePath? parent);
    IPurePath ParentPure => TryGetParentPure(out var parent) ? parent : throw new InvalidOperationException("Element does not have a parent");

    bool TryGetName([NotNullWhen(true)] out string? fileName);
    string? Name => TryGetName(out var fileName) ? fileName : null;

    IPurePath Append(IPurePath right);
    IPurePath Append(string right);

    static IPurePath operator /(IPurePath left, IPurePath right) => left.Append(right);
    static IPurePath operator /(IPurePath left, string right) => left.Append(right);
}