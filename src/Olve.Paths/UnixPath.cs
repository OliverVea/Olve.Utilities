using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Olve.Paths;

[DebuggerDisplay("{Path}")]
internal class UnixPath : IPath
{
    private const string LinkStringPrefix = "file://";
    
    private readonly UnixPurePath _unixPurePath;
    private readonly IPathEnvironment _pathEnvironment;

    private UnixPath(UnixPurePath path, IPathEnvironment? pathEnvironment = null)
    {
        _unixPurePath = path;
        _pathEnvironment = pathEnvironment ?? DefaultUnixPathEnvironment.Shared;
    }
    
    internal UnixPath(string path, IPathEnvironment? pathEnvironment = null)
    {
        _unixPurePath = UnixPurePath.FromPath(path);
        _pathEnvironment = pathEnvironment ?? DefaultUnixPathEnvironment.Shared;
    }

    public string Path => _unixPurePath.Path;
    public PathPlatform Platform => _unixPurePath.Platform;
    public PathType Type => _unixPurePath.Type;

    public bool TryGetParentPure([NotNullWhen(true)] out IPurePath? parent)
        => _unixPurePath.TryGetParentPure(out parent);

    public bool TryGetElementType([NotNullWhen(true)] out ElementType type)
    {
        if (File.Exists(Path))
        {
            type = ElementType.File;
            return true;
        }

        if (Directory.Exists(Path))
        {
            type = ElementType.Directory;
            return true;
        }

        type = ElementType.None;
        return false;
    }

    public bool TryGetAbsolute([NotNullWhen(true)] out IPath? absolute)
    {
        if (Type == PathType.Absolute)
        {
            absolute = this;
            return true;
        }

        if (Type is not PathType.Relative and not PathType.Stub
            || !_pathEnvironment.TryGetCurrentDirectory(out var cwd))
        {
            absolute = null;
            return false;
        }

        absolute = Olve.Paths.Path.Create(cwd) / _unixPurePath;
        
        return true;
    }
    
    public bool TryGetParent([NotNullWhen(true)] out IPath? parent)
    {
        if (TryGetParentPure(out var parentPure))
        {
            parent = new UnixPath((UnixPurePath)parentPure, _pathEnvironment);
            return true;
        }

        parent = null;
        return false;
    }

    public bool TryGetChildren([NotNullWhen(true)] out IEnumerable<IPath>? children)
    {
        var canGetChildren = TryGetElementType(out var type)
            && type == ElementType.Directory
            && Directory.Exists(Path);

        if (!canGetChildren)
        {
            children = null;
            return false;
        }

        children = GetChildren(Path);
        return true;
    }

    // Todo: document exceptions
    private IEnumerable<UnixPath> GetChildren(string path)
    {
        var directories = Directory.EnumerateDirectories(path);
        var files = Directory.EnumerateFiles(path);

        foreach (var directory in directories)
        {
            yield return new UnixPath(directory, _pathEnvironment);
        }

        foreach (var file in files)
        {
            yield return new UnixPath(file, _pathEnvironment);
        }
    }

    public bool TryGetName([NotNullWhen(true)] out string? fileName) 
        => _unixPurePath.TryGetName(out fileName);

    public IPurePath Append(IPurePath right)
    {
        if (_unixPurePath / right is not UnixPurePath pureUnixPath)
        {
            throw new InvalidOperationException("Path segments could not be combined.");
        }

        return new UnixPath(pureUnixPath, _pathEnvironment);
    }

    public IPurePath Append(string right)
    {
        if ((IPurePath)_unixPurePath / right is not UnixPurePath pureUnixPath)
        {
            throw new InvalidOperationException("Path segments could not be combined.");
        }

        return new UnixPath(pureUnixPath, _pathEnvironment);
    }

    public IPath AppendPath(IPurePath right)
    {
        if (_unixPurePath / right is not UnixPurePath pureUnixPath)
        {
            throw new InvalidOperationException("Path segments could not be combined.");
        }

        return new UnixPath(pureUnixPath, _pathEnvironment);
    }

    public IPath AppendPath(string right)
    {
        if ((IPurePath)_unixPurePath / right is not UnixPurePath pureUnixPath)
        {
            throw new InvalidOperationException("Path segments could not be combined.");
        }

        return new UnixPath(pureUnixPath, _pathEnvironment);
    }

    public bool Exists()
    {
        if (TryGetElementType(out var type))
        {
            if (type == ElementType.File)
            {
                return File.Exists(Path);
            }

            if (type == ElementType.Directory)
            {
                return Directory.Exists(Path);
            }
        }

        return false;
    }

    public string GetLinkString(int? lineNumber = null, int? columnNumber = null)
    {
        StringBuilder sb = new();

        sb.Append(LinkStringPrefix);
        sb.Append(Path);

        if (lineNumber is { } l)
        {
            sb.Append(':');
            sb.Append(l);

            if (columnNumber is { } c)
            {
                sb.Append(':');
                sb.Append(c);
            }
        }
        
        var linkText = sb.ToString();

        return linkText;
    }
}