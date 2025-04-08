using System.Diagnostics.CodeAnalysis;

namespace Olve.Paths;

/// <summary>
/// Provides access to environment-dependent path information, such as the current working directory or executable path.
/// </summary>
public interface IPathEnvironment
{
    /// <summary>
    /// Attempts to retrieve the current working directory.
    /// </summary>
    /// <param name="path">When this method returns, contains the path if available; otherwise, <c>null</c>.</param>
    /// <returns><c>true</c> if the current directory was successfully retrieved; otherwise, <c>false</c>.</returns>
    bool TryGetCurrentDirectory([NotNullWhen(true)] out string? path);

    /// <summary>
    /// Attempts to retrieve the file path of the currently executing assembly.
    /// </summary>
    /// <param name="path">When this method returns, contains the path to the assembly if available; otherwise, <c>null</c>.</param>
    /// <returns><c>true</c> if the assembly executable path was successfully retrieved; otherwise, <c>false</c>.</returns>
    bool TryGetAssemblyExecutable([NotNullWhen(true)] out string? path);
}
