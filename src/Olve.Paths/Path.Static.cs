using System.Diagnostics.CodeAnalysis;

namespace Olve.Paths;

public abstract partial class Path
{
    public static PurePath CreatePure(string path)
    {
        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        {
            return CreatePureUnixPath(path);
        }

        throw new PlatformNotSupportedException("Only Unix paths are currently supported.");
    }
    
    public static PurePath CreatePure(string path, PathPlatform platform)
    {
        return platform switch
        {
            PathPlatform.Unix => CreatePureUnixPath(path),
            PathPlatform.None => throw new ArgumentNullException(nameof(path)),
            PathPlatform.Windows => throw new PlatformNotSupportedException("Windows paths are currently not supported"),
            _ => throw new PlatformNotSupportedException("Only Unix paths are currently supported.")
        };
    }

    public static Path Create(string path, IPathEnvironment? pathEnvironment = null)
    {
        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        {
            return CreateUnixPath(path, pathEnvironment);
        }

        throw new PlatformNotSupportedException("Only Unix paths are currently supported.");
    }

    public static Path Create(string path, PathPlatform platform, IPathEnvironment? pathEnvironment = null)
    {
        return platform switch
        {
            PathPlatform.Unix => CreateUnixPath(path, pathEnvironment),
            PathPlatform.None => throw new ArgumentNullException(nameof(path)),
            PathPlatform.Windows => throw new PlatformNotSupportedException("Windows paths are currently not supported"),
            _ => throw new PlatformNotSupportedException("Only Unix paths are currently supported.")
        };
    }

    public static bool TryGetAssemblyExecutable([NotNullWhen(true)] out PurePath? purePath, IPathEnvironment? pathEnvironment = null)
    {
        pathEnvironment ??= DefaultUnixPathEnvironment.Shared;
        if (!pathEnvironment.TryGetAssemblyExecutable(out var executable))
        {
            purePath = null;
            return false;
        }

        purePath = Create(executable);
        return true;
    }

    public static PurePath GetCurrentDirectory()
    {
        var path = Directory.GetCurrentDirectory();
        return CreatePure(path);
    }

    public static PurePath GetHomeDirectory()
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