using System.Collections;
using System.Collections.Frozen;

namespace Olve.Utilities.Lookup;

/// <summary>
///     Represents a frozen lookup. A frozen lookup is a lookup that is immutable.
/// </summary>
/// <param name="items">The items to initialize the lookup with.</param>
/// <typeparam name="TKey">The type of the keys in the lookup.</typeparam>
/// <typeparam name="TValue">The type of the values in the lookup.</typeparam>
public class FrozenLookupBase<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> items)
    : IReadOnlyCollection<KeyValuePair<TKey, TValue>>
    where TKey : notnull
{
    private readonly FrozenDictionary<TKey, TValue> _dictionary = items.ToFrozenDictionary(
        x => x.Key,
        x => x.Value);

    /// <summary>
    ///     Gets the keys in the lookup.
    /// </summary>
    public IReadOnlyCollection<TKey> Keys => _dictionary.Keys;

    /// <summary>
    ///     Gets the values in the lookup.
    /// </summary>
    public IReadOnlyCollection<TValue> Values => _dictionary.Values;

    /// <summary>
    ///     Gets the keys and values in the lookup as key-value pairs.
    /// </summary>
    public IReadOnlyCollection<KeyValuePair<TKey, TValue>> Items => _dictionary.AsReadOnly();

    /// <inheritdoc />
    public int Count => _dictionary.Count;

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    ///     Checks if the lookup contains the specified key.
    /// </summary>
    /// <param name="key">The key to check for.</param>
    /// <returns>True if the key is in the lookup, false otherwise.</returns>
    /// <remarks>This is an O(1) operation.</remarks>
    public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

    /// <summary>
    ///     Checks if the lookup contains the specified value.
    /// </summary>
    /// <param name="value">The value to check for.</param>
    /// <returns>True if the value is in the lookup, false otherwise.</returns>
    /// <remarks>This is an O(n) operation.</remarks>
    public bool ContainsValue(TValue value) => _dictionary.Values.Contains(value);

    /// <summary>
    ///     Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key to get the value for.</param>
    /// <returns>The value associated with the key, or a <see cref="NotFound" /> object if the key is not in the lookup.</returns>
    public OneOf<TValue, NotFound> GetValue(TKey key) => _dictionary.TryGetValue(key, out var value)
        ? OneOf<TValue, NotFound>.FromT0(value)
        : new NotFound();
}