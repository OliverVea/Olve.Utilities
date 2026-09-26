using System.Collections.Concurrent;
using Olve.Utilities.Ids;
using Olve.Utilities.Lookup;

namespace Olve.Utilities.Stores;

/// <summary>
/// A secondary unique index over an <see cref="IEntityStore{T,TId}"/> mapping each key to a single id.
/// </summary>
/// <remarks>
/// Thread-safe, with lock-free reads: writes go through <see cref="_gate"/>, and each store event
/// re-reads the entity under it, so the index matches the store once concurrent writes to an id settle.
/// A reverse id→key map records where each id currently sits, since the store may no longer hold the
/// entity or hold it under a different key. Keys may change; updates that keep the key are skipped
/// without taking the lock (see <see cref="EntityStoreIndex{T,TId,TKey}"/>). The key selector runs on
/// every update, so keep it pure and cheap. The store does not keep the index alive: keep a reference
/// while you read it, or dispose it to stop tracking immediately.
/// </remarks>
/// <typeparam name="T">The entity type.</typeparam>
/// <typeparam name="TId">The identifier type.</typeparam>
/// <typeparam name="TKey">The key type.</typeparam>
public class EntityStoreUniqueIndex<T, TId, TKey> : IDisposable
    where T : IHasId<TId>
    where TId : notnull
    where TKey : notnull
{
    private readonly Lock _gate = new();
    private readonly ConcurrentDictionary<TKey, TId> _index = new();
    private readonly Dictionary<TId, TKey> _keyById = new();
    private readonly IEntityStore<T, TId> _store;
    private readonly Func<T, TKey> _keySelector;
    private readonly IDisposable[] _subscriptions;
    private int _disposed;

    internal EntityStoreUniqueIndex(IEntityStore<T, TId> store, Func<T, TKey> keySelector)
    {
        _store = store;
        _keySelector = keySelector;

        // Subscribe before populating so a write landing in between is not lost; reconciling an id
        // twice is harmless.
        _subscriptions =
        [
            store.OnAdded.SubscribeWeak(this, static (index, e) => index.Reconcile(e.Id)),
            store.OnUpdated.SubscribeWeak(this, static (index, e) => index.OnUpdated(e)),
            store.OnDeleted.SubscribeWeak(this, static (index, e) => index.Reconcile(e.Id)),
        ];

        foreach (var entity in store.List())
        {
            Reconcile(entity.Id);
        }
    }

    /// <summary>
    /// Unsubscribes from the store. The index stops tracking changes but stays readable, frozen at its
    /// last state. Safe to call more than once.
    /// </summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 1) return;

        foreach (var subscription in _subscriptions) subscription.Dispose();
    }

    // See EntityStoreIndex: an update that kept the key cannot move the id.
    private void OnUpdated(EntityUpdated<T, TId> update)
    {
        if (EqualityComparer<TKey>.Default.Equals(_keySelector(update.Before), _keySelector(update.After))) return;

        Reconcile(update.Id);
    }

    // Re-read the store under the lock instead of applying the payload; see EntityStoreIndex.
    private void Reconcile(TId id)
    {
        lock (_gate)
        {
            if (_store.TryGet(id, out var entity)) AddLocked(id, _keySelector(entity));
            else RemoveLocked(id);
        }
    }

    private void AddLocked(TId id, TKey key)
    {
        if (_keyById.TryGetValue(id, out var existingKey) && !EqualityComparer<TKey>.Default.Equals(existingKey, key))
            RemoveLocked(id);

        _index[key] = id;
        _keyById[id] = key;
    }

    private void RemoveLocked(TId id)
    {
        if (!_keyById.Remove(id, out var key)) return;

        // Only drop the forward entry if it still points at this id; a later add under the same
        // key may have rebound it to a different id.
        _index.TryRemove(new KeyValuePair<TKey, TId>(key, id));
    }

    /// <summary>Resolves <paramref name="key"/> to its id, returning <see langword="false"/> if absent.</summary>
    public bool TryGet(TKey key, out TId id) => _index.TryGetValue(key, out id!);

    /// <summary>Returns whether <paramref name="key"/> currently resolves to an id.</summary>
    public bool ContainsKey(TKey key) => _index.ContainsKey(key);
}

/// <summary>
/// An <see cref="EntityStoreUniqueIndex{T,TId,TKey}"/> over a store keyed by <see cref="Id{T}"/>.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
/// <typeparam name="TKey">The key type.</typeparam>
public sealed class EntityStoreUniqueIndex<T, TKey> : EntityStoreUniqueIndex<T, Id<T>, TKey>
    where T : IHasId<Id<T>>
    where TKey : notnull
{
    internal EntityStoreUniqueIndex(IEntityStore<T, Id<T>> store, Func<T, TKey> keySelector) : base(store, keySelector)
    {
    }
}
