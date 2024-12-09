using System.Collections.Frozen;

namespace Olve.Utilities.Lookup;

public class FrozenLookupBase<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> items) where TKey : notnull
{
    private readonly FrozenDictionary<TKey, TValue> _dictionary = items.ToFrozenDictionary(
        x => x.Key,
        x => x.Value);
    
    public int Count => _dictionary.Count;
    public IReadOnlyCollection<TKey> Keys => _dictionary.Keys;
    public IReadOnlyCollection<TValue> Values => _dictionary.Values;
    public IReadOnlyCollection<KeyValuePair<TKey, TValue>> Items => _dictionary.AsReadOnly();
    
    public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);
    public bool ContainsValue(TValue value) => _dictionary.Values.Contains(value);
    
    public OneOf<TValue, NotFound> GetValue(TKey key) => _dictionary.TryGetValue(key, out var value)
        ? OneOf<TValue, NotFound>.FromT0(value)
        : new NotFound();
}