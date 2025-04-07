using System.Diagnostics.CodeAnalysis;

namespace Olve.Paths;

public static class Path
{
    public static IPurePath CreatePure(string path)
    {
        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        {
            return CreatePureUnixPath(path);
        }

        throw new PlatformNotSupportedException("Only Unix paths are currently supported.");
    }
    
    public static IPurePath CreatePure(string path, PathPlatform platform)
    {
        path = path.Trim();

        return platform switch
        {
            PathPlatform.Unix => CreatePureUnixPath(path),
            PathPlatform.None => throw new ArgumentNullException(nameof(path)),
            PathPlatform.Windows => throw new PlatformNotSupportedException("Windows paths are currently not supported"),
            _ => throw new PlatformNotSupportedException("Only Unix paths are currently supported.")
        };
    }

    public static IPath Create(string path, IPathEnvironment? pathEnvironment = null)
    {
        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        {
            return CreateUnixPath(path, pathEnvironment);
        }

        throw new PlatformNotSupportedException("Only Unix paths are currently supported.");
    }

    public static IPath Create(string path, PathPlatform platform, IPathEnvironment? pathEnvironment = null)
    {
        return platform switch
        {
            PathPlatform.Unix => CreateUnixPath(path, pathEnvironment),
            PathPlatform.None => throw new ArgumentNullException(nameof(path)),
            PathPlatform.Windows => throw new PlatformNotSupportedException("Windows paths are currently not supported"),
            _ => throw new PlatformNotSupportedException("Only Unix paths are currently supported.")
        };
    }

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

    public static IPurePath GetCurrentDirectory()
    {
        var path = Directory.GetCurrentDirectory();
        return CreatePure(path);
    }

    public static IPurePath GetHomeDirectory()
    {
        var home = Environment.GetEnvironmentVariable("HOME");
        if (string.IsNullOrEmpty(home))
        {
            throw new InvalidOperationException("Could not determine the HOME environment variable.");
        }

        return CreatePure(home);
    }

    private static UnixPath CreateUnixPath(string path, IPathEnvironment? pathEnvironment)
    {
        path = ProcessUnixPath(path);

        return new UnixPath(path, pathEnvironment);
    }

    private static PureUnixPath CreatePureUnixPath(string path)
    {
        path = ProcessUnixPath(path);

        return new PureUnixPath(path);
    }
    
    private static string ProcessUnixPath(string path)
    {
        if (IsDirectory(path) && !path.EndsWith('/'))
        {
            path += '/';
        }

        return path;
    }
    
    private static bool IsDirectory(string path)
    {
        return Directory.Exists(path);
    }
}