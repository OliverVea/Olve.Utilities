namespace Olve.Utilities.CollectionExtensions;

/// <summary>
///     Provides extension methods for <see cref="Random" />.
/// </summary>
public static class RandomExtensions
{
    private static Random Random => Random.Shared;

    /// <summary>
    ///     Shuffles the elements of the enumerable.
    /// </summary>
    /// <param name="source">The enumerable to shuffle.</param>
    /// <typeparam name="T">The type of the items in the enumerable.</typeparam>
    /// <returns>The shuffled enumerable.</returns>
    /// <remarks>This method is lazy and will not shuffle the enumerable until it is enumerated.</remarks>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.OrderBy(x => Random.Next());
    }
}