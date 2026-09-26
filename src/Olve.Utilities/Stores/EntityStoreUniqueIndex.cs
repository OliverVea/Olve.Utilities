using Olve.Utilities.Ids;
using Olve.Utilities.Lookup;

namespace Olve.Utilities.Stores;

/// <summary>
/// A secondary unique index over an <see cref="EntityStore{T}"/> mapping each key to a single id.
/// </summary>
/// <remarks>
/// Thread-safe: all reads and writes are guarded by <see cref="_gate"/>, and each store event re-reads
/// the entity under it, so the index matches the store once concurrent writes to an id settle. The key
/// selector runs under the lock, so keep it pure and cheap. A reverse id→key map is
/// kept so deletes can resolve the key (the entity is already gone from the store when
/// <see cref="IEntityStore{T,TId}.OnDeleted"/> fires). The index keys on a value that never changes for
/// a given entity, so it does not subscribe to <see cref="IEntityStore{T,TId}.OnUpdated"/>.
/// The store keeps the index alive through its subscriptions; dispose it to unsubscribe, or keep it
/// for the store's lifetime.
/// </remarks>
public sealed class EntityStoreUniqueIndex<T, TKey> : IDisposable
    where T : IHasId<Id<T>>
    where TKey : notnull
{
    private readonly Lock _gate = new();
    private readonly Dictionary<TKey, Id<T>> _index = new();
    private readonly Dictionary<Id<T>, TKey> _keyById = new();
    private readonly EntityStore<T> _store;
    private readonly Func<T, TKey> _keySelector;
    private int _disposed;

    internal EntityStoreUniqueIndex(EntityStore<T> store, Func<T, TKey> keySelector)
    {
        _store = store;
        _keySelector = keySelector;

        // Subscribe before populating so a write landing in between is not lost; reconciling an id
        // twice is harmless.
        store.OnAdded.Subscribe(Reconcile);
        store.OnDeleted.Subscribe(Reconcile);

        foreach (var entity in store.List())
        {
            Reconcile(entity.Id);
        }
    }

    /// <summary>
    /// Unsubscribes from the store. The index stops tracking adds and deletes but stays readable,
    /// frozen at its last state. Safe to call more than once.
    /// </summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 1) return;

        _store.OnAdded.Unsubscribe(Reconcile);
        _store.OnDeleted.Unsubscribe(Reconcile);
    }

    // Re-read the store under the lock instead of trusting which event fired; see EntityStoreIndex.
    private void Reconcile(Id<T> id)
    {
        lock (_gate)
        {
            if (_store.TryGet(id, out var entity)) AddLocked(id, _keySelector(entity));
            else RemoveLocked(id);
        }
    }

    private void AddLocked(Id<T> id, TKey key)
    {
        if (_keyById.TryGetValue(id, out var existingKey) && !EqualityComparer<TKey>.Default.Equals(existingKey, key))
            RemoveLocked(id);

        _index[key] = id;
        _keyById[id] = key;
    }

    private void RemoveLocked(Id<T> id)
    {
        if (!_keyById.Remove(id, out var key)) return;

        // Only drop the forward entry if it still points at this id; a later add under the same
        // key may have rebound it to a different id.
        if (_index.TryGetValue(key, out var current) && current.Equals(id))
            _index.Remove(key);
    }

    /// <summary>Resolves <paramref name="key"/> to its id, returning <see langword="false"/> if absent.</summary>
    public bool TryGet(TKey key, out Id<T> id)
    {
        lock (_gate)
        {
            return _index.TryGetValue(key, out id);
        }
    }

    /// <summary>Returns whether <paramref name="key"/> currently resolves to an id.</summary>
    public bool ContainsKey(TKey key)
    {
        lock (_gate)
        {
            return _index.ContainsKey(key);
        }
    }
}
