using System.Collections.Immutable;
using Olve.Utilities.Ids;
using Olve.Utilities.Lookup;

namespace Olve.Utilities.Stores;

/// <summary>
/// A secondary index over an <see cref="EntityStore{T}"/> that groups entity ids by a key.
/// </summary>
/// <remarks>
/// <para>
/// Thread-safety: each key is backed by an <see cref="ImmutableHashSet{T}"/>, and every write
/// funnels through a locked read-modify-write under <see cref="_gate"/>. Each store event re-reads the
/// entity under that lock, so the index matches the store once concurrent writes to an id settle, even
/// when events arrive out of order. The key selector runs under the lock, so keep it pure and cheap. <see cref="GetForKey"/>
/// returns the immutable set reference directly (no copy), so callers can enumerate it lock-free
/// and it will never mutate underneath them — even while a concurrent write adds or removes ids
/// for the same key. The caller decides whether to snapshot for stability across an
/// <c>await</c>; reads stay zero-copy.
/// </para>
/// <para>
/// The index keys on a value that never changes for a given entity (e.g. a parent id), so it
/// deliberately does not subscribe to <see cref="IEntityStore{T,TId}.OnUpdated"/>. If the key could
/// change on update, this would be incorrect.
/// </para>
/// <para>
/// Lifetime: the index subscribes to the store's events, so the store keeps it alive. Dispose it to
/// unsubscribe, or keep it for the store's lifetime.
/// </para>
/// </remarks>
public sealed class EntityStoreIndex<T, TKey> : IDisposable
    where T : IHasId<Id<T>>
    where TKey : notnull
{
    private readonly Lock _gate = new();
    private readonly Dictionary<TKey, ImmutableHashSet<Id<T>>> _index = new();

    // Reverse map id -> key. The entity is already removed from the store by the time OnDeleted
    // fires, so the key cannot be recomputed from the entity at removal time; it is resolved here.
    private readonly Dictionary<Id<T>, TKey> _keyById = new();

    private readonly EntityStore<T> _store;
    private readonly Func<T, TKey> _keySelector;
    private int _disposed;

    internal EntityStoreIndex(EntityStore<T> store, Func<T, TKey> keySelector)
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

    // Events fire after the store write and outside any lock, so they can arrive out of order across
    // threads. Rather than trusting which event fired, re-read the store under the lock: the reconcile
    // for the last write to an id runs after that write, so the index converges on the store's state.
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
        if (_keyById.TryGetValue(id, out var existingKey))
        {
            if (EqualityComparer<TKey>.Default.Equals(existingKey, key)) return; // already indexed
            RemoveLocked(id);
        }

        var current = _index.TryGetValue(key, out var ids) ? ids : ImmutableHashSet<Id<T>>.Empty;
        _index[key] = current.Add(id);
        _keyById[id] = key;
    }

    private void RemoveLocked(Id<T> id)
    {
        if (!_keyById.Remove(id, out var key)) return;
        if (!_index.TryGetValue(key, out var current)) return;

        var next = current.Remove(id);
        if (next.IsEmpty) _index.Remove(key);
        else _index[key] = next;
    }

    /// <summary>
    /// Returns the ids currently indexed under <paramref name="key"/>. The result is an immutable
    /// snapshot reference: it never mutates, so it is safe to enumerate without further locking.
    /// </summary>
    public IReadOnlyCollection<Id<T>> GetForKey(TKey key)
    {
        lock (_gate)
        {
            return _index.TryGetValue(key, out var ids) ? ids : ImmutableHashSet<Id<T>>.Empty;
        }
    }

    /// <summary>Returns whether any ids are currently indexed under <paramref name="key"/>.</summary>
    public bool ContainsKey(TKey key)
    {
        lock (_gate)
        {
            return _index.ContainsKey(key);
        }
    }
}
