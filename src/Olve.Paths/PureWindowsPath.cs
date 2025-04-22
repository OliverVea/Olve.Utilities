using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Olve.Paths;

[DebuggerDisplay("{Path}")]
internal class PureWindowsPath : IPurePath
{
    public char? DriveLetter { get; }
    internal IReadOnlyList<string> Segments { get; }

    public string Path { get; }

    public PathPlatform Platform { get; }
    public PathType Type { get; }

    internal PureWindowsPath(char? driveLetter, IReadOnlyList<string> segments, PathType pathType)
    {
        // Todo: prettify this logic
        var driveShouldBeNull = pathType != PathType.Absolute;
        var driveIsNull = driveLetter == null;
        if (driveIsNull && !driveShouldBeNull)
        {
            throw new ArgumentException($"{nameof(driveLetter)} must be defined in an absolute path.");
        }

        if (!driveIsNull && driveShouldBeNull)
        {
            throw new ArgumentException($"{nameof(driveLetter)} can only be defined in an absolute path.");
        }
        
        DriveLetter = driveLetter;
        Segments = segments;
        Type = pathType;
        Platform = PathPlatform.Windows;
        
        Path = pathType == PathType.Absolute
            ? $"{driveLetter}{DriveSeparator}{PathSeparator}{string.Join(PathSeparator, segments)}"
            : string.Join(PathSeparator, segments);
    }

    public static PureWindowsPath FromPath(string path)
    {
        ParsePath(path, out var drive, out var pathType, out var segments);
        return new PureWindowsPath(drive, segments, pathType);
    }

    public bool TryGetParentPure([NotNullWhen(true)] out IPurePath? parent)
    {
        if (Segments.Count == 0)
        {
            parent = null;
            return false;
        }

        parent = new PureWindowsPath(DriveLetter, Segments.Take(Segments.Count - 1).ToArray(), Type);
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
        if (right is PureWindowsPath unixRight)
        {
            return Append(unixRight);
        }

        return Append(right.Path);
    }

    private IPurePath Append(PureWindowsPath right)
    {
        if (right.Type == PathType.Stub)
        {
            var combinedSegments = Segments.Concat(right.Segments).ToArray();
            return new PureWindowsPath(DriveLetter, combinedSegments, Type);
        }

        if (right.Type != PathType.Relative)
        {
            throw new NotSupportedException("Absolute paths cannot be on the right hand side of a path concatenation");
        }

        var segments = SegmentsHelper.EvaluateAndConcatenateSegments(Segments, right.Segments);

        return new PureWindowsPath(DriveLetter, segments, Type);
    }

    public IPurePath Append(string right)
    {
        var rightPath = FromPath(right);

        if (rightPath.Type != PathType.Stub)
        {
            throw new ArgumentException("Can only append stub strings (not absolute or relative paths).", nameof(right));
        }

        var segments = SegmentsHelper.EvaluateAndConcatenateSegments(Segments, rightPath.Segments);
        
        return new PureWindowsPath(DriveLetter, segments, Type);
    }

    private const char PathSeparator = '\\';
    private const char AltPathSeparator = '/';
    private const char DriveSeparator = ':';
    private const char RelativeStart = '.';
    private static readonly char[] PathSegmentSeparators = [PathSeparator, AltPathSeparator];
    
    private static void ParsePath(string path, out char? driveLetter, out PathType pathType, out IReadOnlyList<string> segments)
    {       
        if (path.Length == 0 || (segments = GetSegments(path)).Count == 0)
        {
            driveLetter = null;
            pathType = PathType.Relative;
            segments = [];
            return;
        }
        
        var firstSegment = segments[0];

        if (firstSegment.Length == 2
            && char.IsLetter(firstSegment[0])
            && firstSegment[1] == DriveSeparator)
        {
            driveLetter = path[0];
            pathType = PathType.Absolute;
            segments = segments.Skip(1).ToArray();
            return;
        }

        var isRelative = firstSegment.All(c => c == RelativeStart);

        driveLetter = null;
        pathType = isRelative ? PathType.Relative : PathType.Stub;
    }

    
    private const StringSplitOptions SegmentSplitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
    private static string[] GetSegments(string path) => path.Split(PathSegmentSeparators, SegmentSplitOptions);
}