using System.Diagnostics.CodeAnalysis;

namespace Olve.Paths;

internal class UnsupportedPurePath : IPurePath
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
}