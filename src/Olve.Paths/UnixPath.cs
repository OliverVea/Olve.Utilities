using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Olve.Paths;

[DebuggerDisplay("{Path}")]
public class UnixPath : IPath
{
    private const string LinkStringPrefix = "file://";
    
    private readonly PureUnixPath _pureUnixPath;
    private readonly IPathEnvironment _pathEnvironment;

    private UnixPath(PureUnixPath path, IPathEnvironment? pathEnvironment = null)
    {
        _pureUnixPath = path;
        _pathEnvironment = pathEnvironment ?? DefaultUnixPathEnvironment.Shared;
    }
    
    internal UnixPath(string path, IPathEnvironment? pathEnvironment = null)
    {
        _pureUnixPath = new PureUnixPath(path);
        _pathEnvironment = pathEnvironment ?? DefaultUnixPathEnvironment.Shared;
    }

    public string Path => _pureUnixPath.Path;

    public bool TryGetElementType(out ElementType type)
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

        absolute = Olve.Paths.Path.Create(cwd) / _pureUnixPath;
        
        return true;
    }

    public PathPlatform Platform => _pureUnixPath.Platform;
    public PathType Type => _pureUnixPath.Type;

    public bool TryGetParentPure([NotNullWhen(true)] out IPurePath? parent)
        => _pureUnixPath.TryGetParentPure(out parent);
    public bool TryGetParent([NotNullWhen(true)] out IPath? parent)
    {
        if (TryGetParentPure(out var parentPure))
        {
            parent = new UnixPath((PureUnixPath)parentPure, _pathEnvironment);
            return true;
        }

        parent = null;
        return false;
    }

    public bool TryGetName([NotNullWhen(true)] out string? fileName) 
        => _pureUnixPath.TryGetName(out fileName);

    public IPurePath Append(IPurePath right)
    {
        if (_pureUnixPath / right is not PureUnixPath pureUnixPath)
        {
            throw new InvalidOperationException("Path segments could not be combined.");
        }

        return new UnixPath(pureUnixPath, _pathEnvironment);
    }

    public IPurePath Append(string right)
    {
        if ((IPurePath)_pureUnixPath / right is not PureUnixPath pureUnixPath)
        {
            throw new InvalidOperationException("Path segments could not be combined.");
        }

        return new UnixPath(pureUnixPath, _pathEnvironment);
    }

    public IPath AppendPath(IPurePath right)
    {
        if (_pureUnixPath / right is not PureUnixPath pureUnixPath)
        {
            throw new InvalidOperationException("Path segments could not be combined.");
        }

        return new UnixPath(pureUnixPath, _pathEnvironment);
    }

    public IPath AppendPath(string right)
    {
        if ((IPurePath)_pureUnixPath / right is not PureUnixPath pureUnixPath)
        {
            throw new InvalidOperationException("Path segments could not be combined.");
        }

        return new UnixPath(pureUnixPath, _pathEnvironment);
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