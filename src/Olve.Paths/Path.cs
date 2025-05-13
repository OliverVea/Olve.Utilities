using System.Diagnostics.CodeAnalysis;

namespace Olve.Paths;

/// <summary>
/// Represents a utility class for creating and managing platform-specific path abstractions.
/// </summary>
public static class Path
{
    /// <summary>
    /// Creates a platform-specific pure path from the given string.
    /// </summary>
    /// <param name="path">The string path to convert.</param>
    /// <returns>An instance of <see cref="IPurePath"/> representing the input path.</returns>
    public static IPurePath CreatePure(string path)
    {
        if (OperatingSystem.IsLinux())
        {
            return CreatePureUnixPath(path);
        }

        if (OperatingSystem.IsWindows())
        {
            return CreatePureWindowsPath(path);
        }
        
        return new UnsupportedPurePath();
    }

    /// <summary>
    /// Creates a platform-specific pure path from the given string and platform.
    /// </summary>
    /// <param name="path">The string path to convert.</param>
    /// <param name="platform">The platform type to use for path interpretation.</param>
    /// <returns>An instance of <see cref="IPurePath"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the platform is <see cref="PathPlatform.None"/>.</exception>
    public static IPurePath CreatePure(string path, PathPlatform platform)
    {
        path = path.Trim();

        return platform switch
        {
            PathPlatform.Unix => CreatePureUnixPath(path),
            PathPlatform.Windows => CreatePureWindowsPath(path),
            _ => new UnsupportedPurePath()
        };
    }

    /// <summary>
    /// Creates a platform-specific full path from the given string.
    /// </summary>
    /// <param name="path">The string path to convert.</param>
    /// <param name="pathEnvironment">Optional environment to use for path resolution.</param>
    /// <returns>An instance of <see cref="IPath"/>.</returns>
    public static IPath Create(string path, IPathEnvironment? pathEnvironment = null)
    {
        if (OperatingSystem.IsLinux())
        {
            return CreateUnixPath(path, pathEnvironment);
        }

        if (OperatingSystem.IsWindows())
        {
            return CreateWindowsPath(path, pathEnvironment);
        }
        
        return new UnsupportedPath();
    }

    /// <summary>
    /// Creates a platform-specific full path from the given string and platform.
    /// </summary>
    /// <param name="path">The string path to convert.</param>
    /// <param name="platform">The platform type to use for path interpretation.</param>
    /// <param name="pathEnvironment">Optional environment to use for path resolution.</param>
    /// <returns>An instance of <see cref="IPath"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the platform is <see cref="PathPlatform.None"/>.</exception>
    public static IPath Create(string path, PathPlatform platform, IPathEnvironment? pathEnvironment = null)
    {
        return platform switch
        {
            PathPlatform.Unix => CreateUnixPath(path, pathEnvironment),
            PathPlatform.Windows => CreateWindowsPath(path, pathEnvironment),
            _ => new UnsupportedPath()
        };
    }

    /// <summary>
    /// Attempts to retrieve the current assembly's executable as an <see cref="IPath"/>.
    /// </summary>
    /// <param name="path">When this method returns, contains the path to the executable, if successful.</param>
    /// <param name="pathEnvironment">Optional path environment for path resolution.</param>
    /// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
    public static bool TryGetAssemblyExecutable([NotNullWhen(true)] out IPath? path, IPathEnvironment? pathEnvironment = null)
    {
        pathEnvironment ??= DefaultUnixPathEnvironment.Shared;
        if (!pathEnvironment.TryGetAssemblyExecutable(out var executable))
        {
            path = null;
            return false;
        }

        path = Create(executable);
        return true;
    }

    /// <summary>
    /// Attempts to retrieve the current assembly's executable as a pure path.
    /// </summary>
    /// <param name="purePath">When this method returns, contains the pure path to the executable, if successful.</param>
    /// <param name="pathEnvironment">Optional path environment for path resolution.</param>
    /// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
    public static bool TryGetAssemblyExecutablePure([NotNullWhen(true)] out IPurePath? purePath, IPathEnvironment? pathEnvironment = null)
    {
        if (TryGetAssemblyExecutable(out var path, pathEnvironment))
        {
            purePath = path;
            return true;
        }

        purePath = null;
        return false;
    }

    /// <summary>
    /// Gets the current working directory as a pure path.
    /// </summary>
    /// <returns>An instance of <see cref="IPurePath"/> representing the current directory.</returns>
    public static IPurePath GetCurrentDirectoryPure()
    {
        var cwd = Directory.GetCurrentDirectory();
        return CreatePure(cwd);
    }

    /// <summary>
    /// Gets the current working directory as a pure path.
    /// </summary>
    /// <returns>An instance of <see cref="IPath"/> representing the current directory.</returns>
    public static IPath GetCurrentDirectory()
    {
        var cwd = Directory.GetCurrentDirectory();
        return Create(cwd);
    }

    /// <summary>
    /// Gets the current user's home directory as a pure path.
    /// </summary>
    /// <returns>An instance of <see cref="IPurePath"/> representing the home directory.</returns>
    public static IPurePath GetHomeDirectoryPure()
    {
        var home = Environment.GetEnvironmentVariable("HOME");
        if (string.IsNullOrEmpty(home))
        {
            throw new InvalidOperationException("Could not determine the HOME environment variable.");
        }

        return CreatePure(home);
    }

    /// <summary>
    /// Gets the current user's home directory as a pure path.
    /// </summary>
    /// <returns>An instance of <see cref="IPath"/> representing the home directory.</returns>
    public static IPath GetHomeDirectory()
    {
        var home = Environment.GetEnvironmentVariable("HOME");
        if (string.IsNullOrEmpty(home))
        {
            throw new InvalidOperationException("Could not determine the HOME environment variable.");
        }

        return Create(home);
    }

    private static UnixPath CreateUnixPath(string path, IPathEnvironment? pathEnvironment)
    {
        path = ProcessUnixPath(path);
        return new UnixPath(path, pathEnvironment);
    }

    private static UnixPurePath CreatePureUnixPath(string path)
    {
        path = ProcessUnixPath(path);
        return UnixPurePath.FromPath(path);
    }

    private static string ProcessUnixPath(string path)
    {
        if (IsDirectory(path) && !path.EndsWith('/'))
        {
            path += '/';
        }
        return path;
    }

    private static WindowsPath CreateWindowsPath(string path, IPathEnvironment? pathEnvironment)
    {
        path = ProcessWindowsPath(path);
        return new WindowsPath(path, pathEnvironment);
    }

    private static WindowsPurePath CreatePureWindowsPath(string path)
    {
        path = ProcessWindowsPath(path);
        return WindowsPurePath.FromPath(path);
    }

    private static string ProcessWindowsPath(string path)
    {
        path = path.Trim();

        path = path.Replace('/', '\\');

        if (IsDirectory(path) && !path.EndsWith("\\"))
        {
            path += "\\";
        }

        return path;
    }

    private static bool IsDirectory(string path)
    {
        return Directory.Exists(path);
    }
}