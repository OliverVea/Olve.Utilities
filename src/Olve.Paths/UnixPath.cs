using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Olve.Paths;

public class UnixPath : Path
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

    public override bool TryGetAbsolute([NotNullWhen(true)] out Path? absolute)
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

        absolute = Create(cwd) / _pureUnixPath;
        
        return true;
    }

    public override PathPlatform Platform => _pureUnixPath.Platform;
    public override PathType Type => _pureUnixPath.Type;
    public override string GetFullString() => _pureUnixPath.GetFullString();

    public override bool TryGetParent([NotNullWhen(true)] out PurePath? parent)
        => _pureUnixPath.TryGetParent(out parent);

    public override bool TryGetFileName([NotNullWhen(true)] out string? fileName) 
        => _pureUnixPath.TryGetFileName(out fileName);

    protected override Path AppendPath(PurePath right)
    {
        if (_pureUnixPath / right is not PureUnixPath pureUnixPath)
        {
            throw new InvalidOperationException("Path segments could not be combined.");
        }

        return new UnixPath(pureUnixPath, _pathEnvironment);
    }

    protected override Path AppendPath(string right)
    {
        if (_pureUnixPath / right is not PureUnixPath pureUnixPath)
        {
            throw new InvalidOperationException("Path segments could not be combined.");
        }

        return new UnixPath(pureUnixPath, _pathEnvironment);
    }

    public override string GetLinkString(int? lineNumber = null, int? columnNumber = null)
    {
        StringBuilder sb = new();

        sb.Append(LinkStringPrefix);
        sb.Append(GetFullString());

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