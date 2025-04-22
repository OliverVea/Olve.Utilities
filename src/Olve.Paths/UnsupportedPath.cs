using System.Diagnostics.CodeAnalysis;

namespace Olve.Paths;

internal class UnsupportedPath : IPath
{
    private static PlatformNotSupportedException PlatformNotSupportedException => new("Unsupported platform");
    
    public string Path => throw PlatformNotSupportedException;
    public PathPlatform Platform => throw PlatformNotSupportedException;
    public PathType Type => throw PlatformNotSupportedException;
    public bool TryGetParentPure([NotNullWhen(true)] out IPurePath? parent)
    {
        throw PlatformNotSupportedException;
    }

    public bool TryGetName([NotNullWhen(true)] out string? fileName)
    {
        throw PlatformNotSupportedException;
    }

    public IPurePath Append(IPurePath right)
    {
        throw PlatformNotSupportedException;
    }

    public IPurePath Append(string right)
    {
        throw PlatformNotSupportedException;
    }

    public bool TryGetElementType(out ElementType type)
    {
        throw PlatformNotSupportedException;
    }

    public bool TryGetAbsolute([NotNullWhen(true)] out IPath? absolute)
    {
        throw PlatformNotSupportedException;
    }

    public bool TryGetParent([NotNullWhen(true)] out IPath? parent)
    {
        throw PlatformNotSupportedException;
    }

    public bool TryGetChildren([NotNullWhen(true)] out IEnumerable<IPath>? children)
    {
        throw PlatformNotSupportedException;
    }

    public IPath AppendPath(IPurePath right)
    {
        throw PlatformNotSupportedException;
    }

    public IPath AppendPath(string right)
    {
        throw PlatformNotSupportedException;
    }

    public bool Exists()
    {
        throw PlatformNotSupportedException;
    }

    public string GetLinkString(int? lineNumber = null, int? columnNumber = null)
    {
        throw PlatformNotSupportedException;
    }
}