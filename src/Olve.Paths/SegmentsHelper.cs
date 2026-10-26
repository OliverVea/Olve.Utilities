namespace Olve.Paths;

internal static class SegmentsHelper
{
    public static bool IsNavigation(string segment) => segment is "." or "..";

    /// <summary>
    /// Evaluates <c>.</c> and <c>..</c> in both sides. A relative left keeps its leading <c>./</c> or <c>..</c>
    /// segments; <c>..</c> at the root of an absolute left is dropped. Throws when the right side steps above
    /// the start of the left.
    /// </summary>
    public static IReadOnlyList<string> EvaluateAndConcatenateSegments(IReadOnlyList<string> left, bool leftIsAbsolute, IReadOnlyList<string> right)
    {
        var segments = new List<string>();

        foreach (var segment in left)
        {
            Apply(segment, fromLeft: true);
        }

        foreach (var segment in right)
        {
            Apply(segment, fromLeft: false);
        }

        return segments;

        void Apply(string segment, bool fromLeft)
        {
            if (segment == ".")
            {
                if (fromLeft && segments.Count == 0 && !leftIsAbsolute)
                {
                    segments.Add(segment);
                }

                return;
            }

            if (segment != "..")
            {
                segments.Add(segment);
                return;
            }

            var last = segments.Count > 0 ? segments[^1] : null;
            switch (last)
            {
                case null when fromLeft && leftIsAbsolute:
                    return;
                case null when fromLeft:
                case "..":
                    segments.Add(segment);
                    return;
                case ".":
                    segments[^1] = segment;
                    return;
                case null:
                    throw new ArgumentException("Relative path tried to step out of the path root");
                default:
                    segments.RemoveAt(segments.Count - 1);
                    return;
            }
        }
    }

    /// <summary>
    /// Evaluates <c>.</c> and <c>..</c> lexically. <c>..</c> at the root of an absolute path stays at the root;
    /// above the start of a non-absolute path the result is unknown (<see langword="null"/>).
    /// </summary>
    public static IReadOnlyList<string>? Normalize(IReadOnlyList<string> segments, bool isAbsolute)
    {
        var normalized = new List<string>();

        foreach (var segment in segments)
        {
            if (segment == ".")
            {
                continue;
            }

            if (segment == "..")
            {
                if (normalized.Count > 0)
                {
                    normalized.RemoveAt(normalized.Count - 1);
                }
                else if (!isAbsolute)
                {
                    return null;
                }

                continue;
            }

            normalized.Add(segment);
        }

        return normalized;
    }
}
