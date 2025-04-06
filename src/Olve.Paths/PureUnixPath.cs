using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Olve.Paths;

public class PureUnixPath : PurePath
{
    internal IReadOnlyList<string> Segments { get; }
    internal bool IsFile { get; }

    internal PureUnixPath(string[] segments, bool isFile, PathType pathType)
    {
        Segments = segments;
        IsFile = isFile;
        Type = pathType;
    }
    
    internal PureUnixPath(string path)
    {
        Segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        IsFile = !path.EndsWith('/');
        
        Type = GetPathType(path);
    }

    public override PathPlatform Platform => PathPlatform.Unix;
    public override PathType Type { get; }

    public override string GetFullString()
    {
        StringBuilder sb = new();

        if (Type == PathType.Absolute)
        {
            sb.Append('/');
        }

        foreach (var segment in Segments.Take(Segments.Count - 1))
        {
            sb.Append(segment);
            sb.Append('/');
        }

        sb.Append(Segments[^1]);

        if (!IsFile)
        {
            sb.Append('/');
        }

        return sb.ToString();
    }

    public override bool TryGetParent([NotNullWhen(true)] out PurePath? parent)
    {
        if (Segments.Count <= 1)
        {
            parent = null;
            return false;
        }

        parent = new PureUnixPath(Segments.Take(Segments.Count - 1).ToArray(), false, Type);
        return true;
    }

    public override bool TryGetFileName([NotNullWhen(true)] out string? fileName)
    {
        if (!IsFile)
        {
            fileName = null;
            return false;
        }

        if (Segments.Count > 0)
        {
            fileName = Segments[^1];
            return true;
        }

        fileName = null;
        return false;
    }

    protected override PurePath Append(PurePath right)
    {
        if (IsFile)
        {
            throw new InvalidOperationException("Cannot append to a file path.");
        }

        if (right is PureUnixPath unixRight)
        {
            return Append(unixRight);
        }

        return Append(right.GetFullString());
    }

    private PurePath Append(PureUnixPath right)
    {
        if (right.Type == PathType.Stub)
        {
            var combinedSegments = Segments.Concat(right.Segments).ToArray();
            return new PureUnixPath(combinedSegments, right.IsFile, Type);
        }

        if (right.Type != PathType.Relative)
        {
            throw new NotSupportedException("Absolute paths cannot be on the right hand side of a path concatenation");
        }
        
        var segments = Segments.ToList();

        foreach (var segment in right.Segments)
        {
            if (segment.All(x => x == '.'))
            {
                var stepUpCount = segment.Length - 1;

                for (var i = 0; i < stepUpCount; i++)
                {
                    if (segments.Count == 0)
                    {
                        throw new ArgumentException("Relative path tried to step out of the path root");
                    }
                    
                    segments.RemoveAt(segments.Count - 1);
                }
            }

            else
            {
                segments.Add(segment);
            }
        }
        
        return new PureUnixPath(segments.ToArray(), right.IsFile, Type);
    }

    protected override PurePath Append(string right)
    {
        if (IsFile)
            throw new InvalidOperationException("Cannot append to a file path.");

        var rightPath = new PureUnixPath(right);

        if (rightPath.Type != PathType.Stub)
            throw new ArgumentException("Can only append stub strings (not absolute or relative paths).", nameof(right));

        var combinedSegments = Segments.Concat(rightPath.Segments).ToArray();
        return new PureUnixPath(combinedSegments, rightPath.IsFile, this.Type);
    }


    private const char AbsoluteStart = '/';
    private const char RelativeStart = '.';
    
    private static PathType GetPathType(string path)
    {
        if (path.Length == 0) return PathType.Stub;

        return path[0] switch
        {
            AbsoluteStart => PathType.Absolute,
            RelativeStart => PathType.Relative,
            _ => PathType.Stub
        };
    }
}