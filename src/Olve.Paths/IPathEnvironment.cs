using System.Diagnostics.CodeAnalysis;

namespace Olve.Paths;

public interface IPathEnvironment
{
    bool TryGetCurrentDirectory([NotNullWhen(true)]out string? path);
    bool TryGetAssemblyExecutable([NotNullWhen(true)] out string? path);
}