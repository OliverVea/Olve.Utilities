﻿using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Olve.Paths;


[DebuggerDisplay("{Path}")]
internal class PureUnixPath : IPurePath
{
    internal IReadOnlyList<string> Segments { get; }

    internal PureUnixPath(string[] segments, PathType pathType)
    {
        Segments = segments;
        Type = pathType;

        Path = pathType == PathType.Absolute
            ? $"/{string.Join('/', Segments)}"
            : string.Join("/", Segments);
    }

    internal PureUnixPath(string path) : this(path.Split('/', StringSplitOptions.RemoveEmptyEntries), GetPathType(path))
    {
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

        parent = new PureUnixPath(Segments.Take(Segments.Count - 1).ToArray(), Type);
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
        if (right is PureUnixPath unixRight)
        {
            return Append(unixRight);
        }

        return Append(right.Path);
    }

    private IPurePath Append(PureUnixPath right)
    {
        if (right.Type == PathType.Stub)
        {
            var combinedSegments = Segments.Concat(right.Segments).ToArray();
            return new PureUnixPath(combinedSegments, Type);
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
        
        return new PureUnixPath(segments.ToArray(), Type);
    }

    public IPurePath Append(string right)
    {
        var rightPath = new PureUnixPath(right);

        if (rightPath.Type != PathType.Stub)
            throw new ArgumentException("Can only append stub strings (not absolute or relative paths).", nameof(right));

        var combinedSegments = Segments.Concat(rightPath.Segments).ToArray();
        return new PureUnixPath(combinedSegments, Type);
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