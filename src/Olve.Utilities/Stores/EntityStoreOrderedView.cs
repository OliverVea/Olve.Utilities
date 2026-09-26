using System.Collections;
using Olve.Utilities.Lookup;

namespace Olve.Utilities.Stores;

/// <summary>
/// A read-only view of an <see cref="IEntityStore{T,TId}"/>'s entities, ordered by an
/// <see cref="IComparer{T}"/>. The sorted array is cached until the store changes; the next read
/// after an add, update or delete re-sorts.
/// </summary>
/// <remarks>
/// <para>
/// Thread-safety: lock-free. Each change bumps a version, and the cached array is stamped with the
/// version read before it was built, so a change that races a rebuild is never served stale: the
/// next read sees the newer version and rebuilds. Each read returns an immutable array snapshot, so
/// <c>foreach</c> walks one consistent snapshot. Separate <see cref="Count"/> and indexer calls may
/// see different snapshots if the store changes in between.
/// </para>
/// <para>
/// Lifetime: the store does not keep the view alive. Keep a reference for as long as you read it;
/// once it is unreachable it is collected and stops tracking the store. Dispose it to stop tracking
/// immediately.
/// </para>
/// </remarks>
/// <typeparam name="T">The entity type.</typeparam>
/// <typeparam name="TId">The identifier type.</typeparam>
public sealed class EntityStoreOrderedView<T, TId> : IReadOnlyList<T>, IDisposable
    where T : IHasId<TId>
    where TId : notnull
{
    private sealed record Snapshot(T[] Values, long Version);

    private readonly IEntityStore<T, TId> _store;
    private readonly IComparer<T> _comparer;
    private readonly IDisposable[] _subscriptions;
    private long _version;
    private Snapshot? _snapshot;
    private int _disposed;

    internal EntityStoreOrderedView(IEntityStore<T, TId> store, IComparer<T> comparer)
    {
        _store = store;
        _comparer = comparer;

        _subscriptions =
        [
            store.OnAdded.SubscribeWeak(this, static (view, _) => view.Invalidate()),
            store.OnUpdated.SubscribeWeak(this, static (view, _) => view.Invalidate()),
            store.OnDeleted.SubscribeWeak(this, static (view, _) => view.Invalidate()),
        ];
    }

    /// <summary>
    /// Unsubscribes from the store. The view stops tracking changes but stays readable, frozen at
    /// its last state. Safe to call more than once.
    /// </summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 1) return;

        foreach (var subscription in _subscriptions) subscription.Dispose();
    }

    /// <summary>Gets the number of entities in the current ordered snapshot.</summary>
    public int Count => GetOrderedValues().Length;

    /// <summary>Gets the entity at <paramref name="index"/> in the current ordered snapshot.</summary>
    public T this[int index] => GetOrderedValues()[index];

    /// <summary>Enumerates the current ordered snapshot.</summary>
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)GetOrderedValues()).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void Invalidate() => Interlocked.Increment(ref _version);

    private T[] GetOrderedValues()
    {
        // Read the version before listing: the store fires its events after writing, so any change
        // this build misses bumps the version past the one stamped on the snapshot.
        var version = Interlocked.Read(ref _version);
        var snapshot = Volatile.Read(ref _snapshot);
        if (snapshot is not null && snapshot.Version == version) return snapshot.Values;

        var values = _store.List().Order(_comparer).ToArray();
        Volatile.Write(ref _snapshot, new Snapshot(values, version));
        return values;
    }
}
