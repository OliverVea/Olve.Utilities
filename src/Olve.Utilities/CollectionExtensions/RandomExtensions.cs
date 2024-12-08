namespace Olve.Utilities.CollectionExtensions;

public static class RandomExtensions
{
    private static Random Random => Random.Shared;
    
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.OrderBy(x => Random.Next());
    }
}