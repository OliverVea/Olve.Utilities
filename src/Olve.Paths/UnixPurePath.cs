﻿using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Olve.Paths;


[DebuggerDisplay("{Path}")]
internal class UnixPurePath : IPurePath
{
    private const char Divider = '/';
    
    private IReadOnlyList<string> Segments { get; }

    private UnixPurePath(IReadOnlyList<string> segments, PathType pathType)
    {
        Segments = segments;
        Type = pathType;

        Path = PathJoin(Segments, pathType == PathType.Absolute);
    }

    public static UnixPurePath FromPath(string path)
    {
        var segments = path.Split(Divider, StringSplitOptions.RemoveEmptyEntries);
        var pathType = GetPathType(path);

        return new UnixPurePath(segments, pathType);
    }

    public string Path { get; }
    public PathPlatform Platform => PathPlatform.Unix;
    public PathType Type { get; }

    public bool TryGetParentPure([NotNullWhen(true)] out IPurePath? parent)
    {
        if (Segments.Count <= 1)
        {
            parent = null;
            return false;
        }

        parent = new UnixPurePath(Segments.Take(Segments.Count - 1).ToArray(), Type);
        return true;
    }

    public bool TryGetName([NotNullWhen(true)] out string? fileName)
    {
        if (Segments.Count > 0)
        {
            fileName = Segments[^1];
            return true;
        }

        fileName = null;
        return false;
    }

    public IPurePath Append(IPurePath right)
    {
        if (right is UnixPurePath unixRight)
        {
            return Append(unixRight);
        }

        return Append(right.Path);
    }

    private IPurePath Append(UnixPurePath right)
    {
        if (right.Type == PathType.Stub)
        {
            var combinedSegments = Segments.Concat(right.Segments).ToArray();
            return new UnixPurePath(combinedSegments, Type);
        }

        if (right.Type != PathType.Relative)
        {
            throw new NotSupportedException("Absolute paths cannot be on the right hand side of a path concatenation");
        }

        var segments = SegmentsHelper.EvaluateAndConcatenateSegments(Segments, right.Segments);
        
        return new UnixPurePath(segments.ToArray(), Type);
    }

    public IPurePath Append(string right)
    {
        var rightPath = FromPath(right);

        if (rightPath.Type != PathType.Stub)
        {
            throw new ArgumentException("Can only append stub strings (not absolute or relative paths).", nameof(right));
        }

        var combinedSegments = Segments.Concat(rightPath.Segments).ToArray();
        return new UnixPurePath(combinedSegments, Type);
    }


    private const char AbsoluteStart = Divider;
    private const char RelativeStart = '.';
    
    private static PathType GetPathType(string path)
    {
        if (path.Length == 0) return PathType.Stub;

        var firstDivider = path.IndexOf(Divider);
        if (firstDivider == 0) return PathType.Absolute;
        
        if (firstDivider < 0) firstDivider = path.Length;

        var pathIsRelative = path.Take(firstDivider).All(ch => ch == '.');
        if (pathIsRelative) return PathType.Relative;

        return PathType.Stub;
    }

    private static string PathJoin(IEnumerable<string> pathSegments, bool startWithDivider = false)
    {
        var joinedPath = string.Join(Divider, pathSegments);
        return startWithDivider ? Divider + joinedPath : joinedPath;
    }
}