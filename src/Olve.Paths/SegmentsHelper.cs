namespace Olve.Paths;

internal static class SegmentsHelper
{
    public static bool IsNavigation(string segment) => segment is "." or "..";

    public static IReadOnlyList<string> EvaluateAndConcatenateSegments(IReadOnlyList<string> left, IReadOnlyList<string> right)
    {
        var segments = left.ToList();

        foreach (var segment in right)
        {
            if (segment == ".")
            {
                continue;
            }

            if (segment == "..")
            {
                if (segments.Count == 0)
                {
                    throw new ArgumentException("Relative path tried to step out of the path root");
                }

                segments.RemoveAt(segments.Count - 1);
                continue;
            }

            segments.Add(segment);
        }

        return segments;
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
