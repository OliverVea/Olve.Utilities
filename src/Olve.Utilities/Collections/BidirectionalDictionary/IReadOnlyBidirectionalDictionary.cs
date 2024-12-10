namespace Olve.Utilities.Collections;

/// <summary>
/// Represents a read-only version of a bidirectional collection.
/// </summary>
/// <typeparam name="T1">The type of the first values in the dictionary.</typeparam>
/// <typeparam name="T2">The type of the second values in the dictionary.</typeparam>
public interface IReadOnlyBidirectionalDictionary<T1, T2> : IEnumerable<KeyValuePair<T1, T2>>
    where T1 : notnull
    where T2 : notnull
{
    ///	<summary>Tells if the inner Dictionaries of the <see cref="IReadOnlyBidirectionalDictionary{TFirst, TSecond}" /> are still synced.</summary>
    /// <returns>The bool if the inner Dictionaries of the <see cref="IReadOnlyBidirectionalDictionary{TFirst, TSecond}" /> are still synced..</returns>
    bool IsSynced { get; }

    ///	<summary>Gets the number of value pairs contained in the <see cref="IReadOnlyBidirectionalDictionary{TFirst, TSecond}" />.</summary>
    /// <returns>The number of value pairs contained in the <see cref="IReadOnlyBidirectionalDictionary{TFirst, TSecond}" />.</returns>
    int Count { get; }

    /// <summary>
    /// Tells if the <see cref="IReadOnlyBidirectionalDictionary{TFirst, TSecond}"/> contains the first value of the value pair.
    /// </summary>
    /// <param name="first">The first value of the pair</param>
    /// <returns>Returns <see langword="true"/>, if the operation was successful, otherwise returns <see langword="false"/>.</returns>
    bool Contains(T1 first);

    /// <summary>
    /// Tells if the <see cref="IReadOnlyBidirectionalDictionary{TFirst, TSecond}"/> contains the second value of the value pair.
    /// </summary>
    /// <param name="second">The second value of the pair</param>
    /// <returns>Returns <see langword="true"/>, if the operation was successful, otherwise returns <see langword="false"/>.</returns>
    bool Contains(T2 second);

    /// <summary>
    /// Gets the value associated with the first value of the value pair.
    /// </summary>
    /// <param name="first">The first value of the pair</param>
    /// <returns>The value associated with the first value of the pair, or a <see cref="NotFound"/> object if the first value is not in the dictionary.</returns>
    OneOf<T2, NotFound> Get(T1 first);
    
    /// <summary>
    /// Gets the value associated with the second value of the value pair.
    /// </summary>
    /// <param name="second">The second value of the pair</param>
    /// <returns>The value associated with the second value of the pair, or a <see cref="NotFound"/> object if the second value is not in the dictionary.</returns>
    OneOf<T1, NotFound> Get(T2 second);
    
    /// <summary>
    /// Gets all the first values in the dictionary.
    /// </summary>
    IReadOnlyCollection<T1> FirstValues { get; }
    
    /// <summary>
    /// Gets all the second values in the dictionary.
    /// </summary>
    IReadOnlyCollection<T2> SecondValues { get; }
}