// Code inspired by: https://github.com/TwentyFourMinutes/BidirectionalDict/tree/master

namespace Olve.Utilities.Collections;

/// <summary>
///     Represents a bidirectional collection of value pairs.
/// </summary>
/// <typeparam name="T1">The type of the first values in the dictionary.</typeparam>
/// <typeparam name="T2">The type of the second values in the dictionary.</typeparam>
public interface IBidirectionalDictionary<T1, T2> : IReadOnlyBidirectionalDictionary<T1, T2>
    where T1 : notnull
    where T2 : notnull
{
    /// <summary>
    ///     Tries to add new value pair to the <see cref="IBidirectionalDictionary{TFirst, TSecond}" />.
    /// </summary>
    /// <param name="first">The first value of the pair</param>
    /// <param name="second">The second value of the pair</param>
    /// <returns>Returns <see langword="true" />, if the operation was successful, otherwise returns <see langword="false" />.</returns>
    bool TryAdd(T1 first, T2 second);

    /// <summary>
    ///     Sets the value pair in the <see cref="IBidirectionalDictionary{TFirst, TSecond}" />.
    /// </summary>
    /// <param name="first">The first value of the pair</param>
    /// <param name="second">The second value of the pair</param>
    void Set(T1 first, T2 second);

    /// <summary>
    ///     Tries to remove a value pair from the <see cref="IBidirectionalDictionary{TFirst, TSecond}" />, by the first value
    ///     of the pair.
    /// </summary>
    /// <param name="first">The first value of the pair</param>
    /// <returns>Returns <see langword="true" />, if the operation was successful, otherwise returns <see langword="false" />.</returns>
    bool TryRemove(T1 first);

    /// <summary>
    ///     Tries to remove a value pair from the <see cref="IBidirectionalDictionary{TFirst, TSecond}" />, by the second value
    ///     of the pair.
    /// </summary>
    /// <param name="second">The second value of the pair</param>
    /// <returns>Returns <see langword="true" />, if the operation was successful, otherwise returns <see langword="false" />.</returns>
    bool TryRemove(T2 second);

    /// <summary>
    ///     Clears all value pairs in the <see cref="IBidirectionalDictionary{TFirst, TSecond}" />.
    /// </summary>
    void Clear();
}