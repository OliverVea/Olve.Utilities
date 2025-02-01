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
        return source.OrderBy(_ => Random.Next());
    }
    
    /// <summary>
    /// Picks a random element from the enumerable.
    /// </summary>
    /// <param name="source">The enumerable to pick from.</param>
    /// <typeparam name="T">The type of the items in the enumerable.</typeparam>
    /// <returns>A random element from the enumerable.</returns>
    public static T PickRandom<T>(this IEnumerable<T> source)
    {
        return source.Shuffle().First();
    }
    
    /// <summary>
    /// Picks a random element from the enumerable or returns the default value if the enumerable is empty.
    /// </summary>
    /// <param name="source">The enumerable to pick from.</param>
    /// <typeparam name="T">The type of the items in the enumerable.</typeparam>
    /// <returns>A random element from the enumerable or the default value if the enumerable is empty.</returns>
    public static T? PickRandomOrDefault<T>(this IEnumerable<T> source)
    {
        return source.Shuffle().FirstOrDefault();
    }
    
    /// <summary>
    /// Picks a random element from the enumerable or returns the specified default value if the enumerable is empty.
    /// </summary>
    /// <param name="source">The enumerable to pick from.</param>
    /// <param name="count">The number of elements to pick.</param>
    /// <typeparam name="T">The type of the items in the enumerable.</typeparam>
    /// <returns>A random element from the enumerable or the default value if the enumerable is empty.</returns>
    public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
    {
        return source.Shuffle().Take(count);
    }
}