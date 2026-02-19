// Code inspired by: https://github.com/TwentyFourMinutes/BidirectionalDict/tree/master

using System.Collections;
using System.Collections.ObjectModel;

namespace Olve.Utilities.Collections;

/// <summary>
///     Represents a read-only version of a bidirectional collection.
/// </summary>
/// <typeparam name="T1">The type of the first values in the dictionary.</typeparam>
/// <typeparam name="T2">The type of the second values in the dictionary.</typeparam>
public class ReadOnlyBidirectionalDictionary<T1, T2> : IReadOnlyBidirectionalDictionary<T1, T2>
    where T1 : notnull
    where T2 : notnull
{

    private readonly IReadOnlyDictionary<T1, T2> _firstToSecond;
    private readonly IReadOnlyDictionary<T2, T1> _secondToFirst;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ReadOnlyBidirectionalDictionary{TFirst, TSecond}" /> class that
    ///     contains elements copied from the specified <see cref="IEnumerable{T}" /> and uses the default equality comparer
    ///     for the key type.
    /// </summary>
    /// <param name="collection">
    ///     The <see cref="IEnumerable{T}" /> whose elements are copied to the new
    ///     <see cref="ReadOnlyBidirectionalDictionary{TFirst, TSecond}" />.
    /// </param>
    public ReadOnlyBidirectionalDictionary(IEnumerable<KeyValuePair<T1, T2>> collection)
    {
        _firstToSecond = new ReadOnlyDictionary<T1, T2>(collection.ToDictionary(k => k.Key, v => v.Value));
        _secondToFirst = new ReadOnlyDictionary<T2, T1>(_firstToSecond.ToDictionary(k => k.Value, v => v.Key));
    }

    /// <inheritdoc />
    public bool IsSynced => Count == _secondToFirst.Count;

    /// <inheritdoc />
    public int Count => _firstToSecond.Count;

    /// <inheritdoc />
    public bool Contains(T1 first)
        => _firstToSecond.ContainsKey(first);

    /// <inheritdoc />
    public bool Contains(T2 second)
        => _secondToFirst.ContainsKey(second);

    /// <inheritdoc />
    public bool TryGet(T1 first, [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out T2 second)
    {
        return _firstToSecond.TryGetValue(first, out second);
    }

    /// <inheritdoc />
    public bool TryGet(T2 second, [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out T1 first)
    {
        return _secondToFirst.TryGetValue(second, out first);
    }

    /// <inheritdoc />
    /// <remarks>Instantiates an array with the values.</remarks>
    public IReadOnlyCollection<T1> FirstValues => _firstToSecond.Keys.ToArray();

    /// <inheritdoc />
    /// <remarks>Instantiates an array with the values.</remarks>
    public IReadOnlyCollection<T2> SecondValues => _secondToFirst.Keys.ToArray();

    /// <summary>
    ///     Gets the <see cref="IEnumerator{T}" /> of the <see cref="ReadOnlyBidirectionalDictionary{TFirst, TSecond}" />.
    /// </summary>
    /// BiDictionary
    /// <returns>
    ///     The <see cref="IEnumerator{T}" /> of the <see cref="ReadOnlyBidirectionalDictionary{TFirst, TSecond}" />
    /// </returns>
    public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
        => _firstToSecond.GetEnumerator();

    /// <summary>
    ///     Gets the <see cref="IEnumerator" /> of the <see cref="ReadOnlyBidirectionalDictionary{TFirst, TSecond}" />.
    /// </summary>
    /// <returns>
    ///     The <see cref="IEnumerator" /> of the <see cref="ReadOnlyBidirectionalDictionary{TFirst, TSecond}" />
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}