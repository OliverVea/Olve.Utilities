using System.Collections.Concurrent;
using System.Collections.Immutable;
using Olve.Utilities.Ids;
using Olve.Utilities.Lookup;

namespace Olve.Utilities.Stores;

/// <summary>
/// A secondary index over an <see cref="IEntityStore{T,TId}"/> that groups entity ids by a key.
/// </summary>
/// <remarks>
/// <para>
/// Thread-safety: reads are lock-free. Each key maps to an <see cref="ImmutableHashSet{T}"/>, and every
/// write swaps in a new set under <see cref="_gate"/>. Each store event re-reads the entity under that
/// lock, so the index matches the store once concurrent writes to an id settle, even when events arrive
/// out of order. <see cref="GetForKey"/> returns the immutable set reference directly (no copy), so
/// callers can enumerate it lock-free and it will never mutate underneath them — even while a concurrent
/// write adds or removes ids for the same key. The caller decides whether to snapshot for stability
/// across an <c>await</c>; reads stay zero-copy.
/// </para>
/// <para>
/// Keys may change: an update whose <see cref="EntityUpdated{T,TId}.Before"/> and
/// <see cref="EntityUpdated{T,TId}.After"/> share a key is skipped without taking the lock; any other
/// update moves the id. The key selector runs on every update, so keep it pure and cheap.
/// </para>
/// <para>
/// Lifetime: the store does not keep the index alive. Keep a reference for as long as you read it;
/// once it is unreachable it is collected and stops tracking the store. Dispose it to stop tracking
/// immediately.
/// </para>
/// </remarks>
/// <typeparam name="T">The entity type.</typeparam>
/// <typeparam name="TId">The identifier type.</typeparam>
/// <typeparam name="TKey">The key type.</typeparam>
public class EntityStoreIndex<T, TId, TKey> : IDisposable
    where T : IHasId<TId>
    where TId : notnull
    where TKey : notnull
{
    private readonly Lock _gate = new();
    private readonly ConcurrentDictionary<TKey, ImmutableHashSet<TId>> _index = new();

    // Reverse map id -> key, written under the lock. Reconcile needs to know where an id currently
    // sits: the store may no longer hold the entity, or hold it under a different key.
    private readonly Dictionary<TId, TKey> _keyById = new();

    private readonly IEntityStore<T, TId> _store;
    private readonly Func<T, TKey> _keySelector;
    private readonly IDisposable[] _subscriptions;
    private int _disposed;

    internal EntityStoreIndex(IEntityStore<T, TId> store, Func<T, TKey> keySelector)
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

    // A write that kept the key cannot move the id, and any write that does move it fires its own event,
    // so the filter is safe without the lock.
    private void OnUpdated(EntityUpdated<T, TId> update)
    {
        if (EqualityComparer<TKey>.Default.Equals(_keySelector(update.Before), _keySelector(update.After))) return;

        Reconcile(update.Id);
    }

    // Events fire after the store write and outside any lock, so they can arrive out of order across
    // threads. Rather than applying the payload, re-read the store under the lock: the reconcile for
    // the last write to an id runs after that write, so the index converges on the store's state.
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
        if (_keyById.TryGetValue(id, out var existingKey))
        {
            if (EqualityComparer<TKey>.Default.Equals(existingKey, key)) return; // already indexed
            RemoveLocked(id);
        }

        var current = _index.TryGetValue(key, out var ids) ? ids : ImmutableHashSet<TId>.Empty;
        _index[key] = current.Add(id);
        _keyById[id] = key;
    }

    private void RemoveLocked(TId id)
    {
        if (!_keyById.Remove(id, out var key)) return;
        if (!_index.TryGetValue(key, out var current)) return;

        var next = current.Remove(id);
        if (next.IsEmpty) _index.TryRemove(key, out _);
        else _index[key] = next;
    }

    /// <summary>
    /// Returns the ids currently indexed under <paramref name="key"/>. The result is an immutable
    /// snapshot reference: it never mutates, so it is safe to enumerate without further locking.
    /// </summary>
    public IReadOnlyCollection<TId> GetForKey(TKey key) =>
        _index.TryGetValue(key, out var ids) ? ids : ImmutableHashSet<TId>.Empty;

    /// <summary>Returns whether any ids are currently indexed under <paramref name="key"/>.</summary>
    public bool ContainsKey(TKey key) => _index.ContainsKey(key);
}

/// <summary>
/// An <see cref="EntityStoreIndex{T,TId,TKey}"/> over a store keyed by <see cref="Id{T}"/>.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
/// <typeparam name="TKey">The key type.</typeparam>
public sealed class EntityStoreIndex<T, TKey> : EntityStoreIndex<T, Id<T>, TKey>
    where T : IHasId<Id<T>>
    where TKey : notnull
{
    internal EntityStoreIndex(IEntityStore<T, Id<T>> store, Func<T, TKey> keySelector) : base(store, keySelector)
    {
    }
}
