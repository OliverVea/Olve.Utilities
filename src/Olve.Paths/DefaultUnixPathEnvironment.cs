using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Olve.Paths;

internal class DefaultUnixPathEnvironment : IPathEnvironment
{
    public static DefaultUnixPathEnvironment Shared { get; } = new();
    
    public bool TryGetCurrentDirectory([NotNullWhen(true)] out string? path)
    {
        path = Directory.GetCurrentDirectory();
        return true;
    }

    public bool TryGetAssemblyExecutable([NotNullWhen(true)] out string? path)
    {
        path = Assembly.GetExecutingAssembly().Location;
        return true;
    }
}