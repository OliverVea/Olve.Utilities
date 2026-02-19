// Code inspired by: https://github.com/TwentyFourMinutes/BidirectionalDict/tree/master

using System.Diagnostics.CodeAnalysis;

namespace Olve.Utilities.Collections;

/// <summary>
///     Represents a read-only version of a bidirectional collection.
/// </summary>
/// <typeparam name="T1">The type of the first values in the dictionary.</typeparam>
/// <typeparam name="T2">The type of the second values in the dictionary.</typeparam>
public interface IReadOnlyBidirectionalDictionary<T1, T2> : IEnumerable<KeyValuePair<T1, T2>>
    where T1 : notnull
    where T2 : notnull
{
    /// <summary>
    ///     Tells if the inner Dictionaries of the <see cref="IReadOnlyBidirectionalDictionary{TFirst, TSecond}" /> are
    ///     still synced.
    /// </summary>
    /// <returns>
    ///     The bool if the inner Dictionaries of the <see cref="IReadOnlyBidirectionalDictionary{TFirst, TSecond}" /> are
    ///     still synced..
    /// </returns>
    bool IsSynced { get; }

    /// <summary>
    ///     Gets the number of value pairs contained in the
    ///     <see cref="IReadOnlyBidirectionalDictionary{TFirst, TSecond}" />.
    /// </summary>
    /// <returns>The number of value pairs contained in the <see cref="IReadOnlyBidirectionalDictionary{TFirst, TSecond}" />.</returns>
    int Count { get; }

    /// <summary>
    ///     Gets all the first values in the dictionary.
    /// </summary>
    IReadOnlyCollection<T1> FirstValues { get; }

    /// <summary>
    ///     Gets all the second values in the dictionary.
    /// </summary>
    IReadOnlyCollection<T2> SecondValues { get; }

    /// <summary>
    ///     Tells if the <see cref="IReadOnlyBidirectionalDictionary{TFirst, TSecond}" /> contains the first value of the value
    ///     pair.
    /// </summary>
    /// <param name="first">The first value of the pair</param>
    /// <returns>Returns <see langword="true" />, if the operation was successful, otherwise returns <see langword="false" />.</returns>
    bool Contains(T1 first);

    /// <summary>
    ///     Tells if the <see cref="IReadOnlyBidirectionalDictionary{TFirst, TSecond}" /> contains the second value of the
    ///     value pair.
    /// </summary>
    /// <param name="second">The second value of the pair</param>
    /// <returns>Returns <see langword="true" />, if the operation was successful, otherwise returns <see langword="false" />.</returns>
    bool Contains(T2 second);

    /// <summary>
    ///     Tries to get the value associated with the first value of the value pair.
    /// </summary>
    /// <param name="first">The first value of the pair.</param>
    /// <param name="second">When this method returns, contains the associated second value if found; otherwise, the default value.</param>
    /// <returns><see langword="true" /> if the first value was found; otherwise, <see langword="false" />.</returns>
    bool TryGet(T1 first, [MaybeNullWhen(false)] out T2 second);

    /// <summary>
    ///     Tries to get the value associated with the second value of the value pair.
    /// </summary>
    /// <param name="second">The second value of the pair.</param>
    /// <param name="first">When this method returns, contains the associated first value if found; otherwise, the default value.</param>
    /// <returns><see langword="true" /> if the second value was found; otherwise, <see langword="false" />.</returns>
    bool TryGet(T2 second, [MaybeNullWhen(false)] out T1 first);
}