// Code inspired by: https://github.com/TwentyFourMinutes/BidirectionalDict/tree/master

using System.Collections;

namespace Olve.Utilities.Collections;

/// <summary>
/// Represents a bidirectional collection of value pairs.
/// </summary>
/// <typeparam name="T1">The type of the first values in the dictionary.</typeparam>
/// <typeparam name="T2">The type of the second values in the dictionary.</typeparam>
public class BidirectionalDictionary<T1, T2> : IBidirectionalDictionary<T1, T2>
	 where T1 : notnull
	 where T2 : notnull
{
	/// <inheritdoc/>
	public bool IsSynced => Count == _secondToFirst.Count;

	/// <inheritdoc/>
	public int Count => _firstToSecond.Count;

	private readonly IDictionary<T1, T2> _firstToSecond;
	private readonly IDictionary<T2, T1> _secondToFirst;

	/// <summary>Initializes a new instance of the <see cref="BidirectionalDictionary{TFirst, TSecond}" /> class that is empty, has the default initial capacity, and uses the default equality comparer for the key type.</summary>
	public BidirectionalDictionary(IEnumerable<KeyValuePair<T1, T2>>? collection = null, IEqualityComparer<T1>? firstComparer = null, IEqualityComparer<T2>? secondComparer = null)
    {
        _firstToSecond = new Dictionary<T1, T2>(collection ?? [], firstComparer);
        _secondToFirst = new Dictionary<T2, T1>(_firstToSecond.Select(x => new KeyValuePair<T2, T1>(x.Value, x.Key)), secondComparer);
    }

	/// <inheritdoc/>
	public bool TryAdd(T1 first, T2 second)
	{
		return _firstToSecond.TryAdd(first, second) && _secondToFirst.TryAdd(second, first);
	}

	/// <inheritdoc/>
	public void Set(T1 first, T2 second)
	{
		if (Get(second).TryPickT0(out var existingFirst, out _))
		{
			_firstToSecond.Remove(existingFirst);
		}
		
		if (Get(first).TryPickT0(out var existingSecond, out _))
		{
			_secondToFirst.Remove(existingSecond);
		}
		
		_firstToSecond[first] = second;
		_secondToFirst[second] = first;
	}

	/// <inheritdoc/>
	public bool TryRemove(T1 first)
	{
		if (!_firstToSecond.Remove(first, out var second))
		{
			return false;
		}

		_secondToFirst.Remove(second);

		return true;
	}

	/// <inheritdoc/>
	public bool TryRemove(T2 second)
	{
		if (!_secondToFirst.Remove(second, out var first))
		{
			return false;
		}

		_firstToSecond.Remove(first);

		return true;
	}

	/// <inheritdoc/>
	public bool Contains(T1 first)
		=> _firstToSecond.ContainsKey(first);

	/// <inheritdoc/>
	public bool Contains(T2 second)
		=> _secondToFirst.ContainsKey(second);

	/// <inheritdoc/>
	public OneOf<T2, NotFound> Get(T1 first)
	{
		return _firstToSecond.TryGetValue(first, out var second)
            ? second
            : new NotFound();
	}

	/// <inheritdoc/>
	public OneOf<T1, NotFound> Get(T2 second)
	{
		return _secondToFirst.TryGetValue(second, out var first)
            ? first
            : new NotFound();
	}

	/// <inheritdoc/>
	/// <remarks>Instantiates an array with the values.</remarks>
	public IReadOnlyCollection<T1> FirstValues => _firstToSecond.Keys.ToArray();
	
	/// <inheritdoc/>
	/// <remarks>Instantiates an array with the values.</remarks>
	public IReadOnlyCollection<T2> SecondValues => _secondToFirst.Keys.ToArray();

	/// <inheritdoc/>
	public void Clear()
	{
		_secondToFirst.Clear();
		_firstToSecond.Clear();
	}

	/// <summary>
	/// Gets the <see cref="IEnumerator{T}"/> of the <see cref="BidirectionalDictionary{TFirst, TSecond}"/>.
	/// </summary>
	/// <returns>The <see cref="IEnumerator{T}"/> of the <see cref="BidirectionalDictionary{TFirst, TSecond}"/></returns>
	public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator() => _firstToSecond.GetEnumerator();

	/// <summary>
	/// Gets the <see cref="IEnumerator"/> of the <see cref="BidirectionalDictionary{TFirst, TSecond}"/>.
	/// </summary>
	/// <returns>The <see cref="IEnumerator"/> of the <see cref="BidirectionalDictionary{TFirst, TSecond}"/></returns>
	IEnumerator IEnumerable.GetEnumerator() => _firstToSecond.GetEnumerator();
}