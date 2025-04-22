using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Olve.Paths;

internal class WindowsPath : IPath
{
    private const string LinkStringPrefix = "file://";
    
    private readonly WindowsPurePath _windowsPurePath;
    private readonly IPathEnvironment _pathEnvironment;

    private WindowsPath(WindowsPurePath path, IPathEnvironment? pathEnvironment = null)
    {
        _windowsPurePath = path;
        _pathEnvironment = pathEnvironment ?? DefaultUnixPathEnvironment.Shared;
    }
    
    internal WindowsPath(string path, IPathEnvironment? pathEnvironment = null)
    {
        _windowsPurePath = WindowsPurePath.FromPath(path);
        _pathEnvironment = pathEnvironment ?? DefaultUnixPathEnvironment.Shared;
    }

    public string Path => _windowsPurePath.Path;
    public PathPlatform Platform => PathPlatform.Windows;
    public PathType Type => _windowsPurePath.Type;
    
    public bool TryGetParentPure([NotNullWhen(true)] out IPurePath? parent)
        => _windowsPurePath.TryGetParentPure(out parent);

    public bool TryGetName([NotNullWhen(true)] out string? fileName)
        => _windowsPurePath.TryGetName(out fileName);

    public IPurePath Append(IPurePath right)
    {
        if (_windowsPurePath / right is not WindowsPurePath pureWindowsPath)
        {
            throw new InvalidOperationException("Path segments could not be combined.");
        }

        return new WindowsPath(pureWindowsPath, _pathEnvironment);
    }

    public IPurePath Append(string right)
    {
        if ((IPurePath)_windowsPurePath / right is not WindowsPurePath pureWindowsPath)
        {
            throw new InvalidOperationException("Path segments could not be combined.");
        }
        
        return new WindowsPath(pureWindowsPath, _pathEnvironment);
    }

    public bool TryGetElementType(out ElementType type)
    {
        if (File.Exists(_windowsPurePath.Path))
        {
            type = ElementType.File;
            return true;
        }

        if (Directory.Exists(_windowsPurePath.Path))
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

        absolute = Olve.Paths.Path.Create(cwd) / _windowsPurePath;
        
        return true;
    }

    public bool TryGetParent([NotNullWhen(true)] out IPath? parent)
    {
        if (TryGetParentPure(out var parentPure))
        {
            parent = new WindowsPath((WindowsPurePath)parentPure, _pathEnvironment);
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
    
    private IEnumerable<WindowsPath> GetChildren(string path)
    {
        var directories = Directory.EnumerateDirectories(path);
        var files = Directory.EnumerateFiles(path);

        foreach (var directory in directories)
        {
            yield return new WindowsPath(directory, _pathEnvironment);
        }

        foreach (var file in files)
        {
            yield return new WindowsPath(file, _pathEnvironment);
        }
    }

    public IPath AppendPath(IPurePath right)
    {
        if (_windowsPurePath / right is not WindowsPurePath pureWindowsPath)
        {
            throw new InvalidOperationException("Path segments could not be combined.");
        }

        return new WindowsPath(pureWindowsPath, _pathEnvironment);
    }

    public IPath AppendPath(string right)
    {
        if ((IPurePath)_windowsPurePath / right is not WindowsPurePath pureWindowsPath)
        {
            throw new InvalidOperationException("Path segments could not be combined.");
        }

        return new WindowsPath(pureWindowsPath, _pathEnvironment);
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