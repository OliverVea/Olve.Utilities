namespace Olve.Paths;

internal static class SegmentsHelper
{
    public static IReadOnlyList<string> EvaluateAndConcatenateSegments(
        IReadOnlyList<string> left,
        IReadOnlyList<string> right
    )
    {
        var segments = left.ToList();

        foreach (var segment in right)
        {
            if (segment.All(x => x == '.'))
            {
                var stepUpCount = segment.Length - 1;

                for (var i = 0; i < stepUpCount; i++)
                {
                    if (segments.Count == 0)
                    {
                        throw new ArgumentException(
                            "Relative path tried to step out of the path root"
                        );
                    }

                    segments.RemoveAt(segments.Count - 1);
                }
            }
            else
            {
                segments.Add(segment);
            }
        }

        return segments;
    }
}
