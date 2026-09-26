using Olve.Utilities.Ids;
using Olve.Utilities.Lookup;

namespace Olve.Utilities.Stores;

/// <summary>
/// Creates indexes and views over any <see cref="IEntityStore{T,TId}"/>.
/// </summary>
/// <remarks>
/// The store does not keep what these return alive. Hold the result for as long as you read it, typically
/// in a field next to the store; once it is unreachable it is collected and stops tracking the store.
/// Creating one per request is therefore safe but wasteful: each re-reads the whole store on creation.
/// </remarks>
public static class EntityStoreExtensions
{
    /// <summary>Creates a secondary index grouping entity ids by <paramref name="keySelector"/>.</summary>
    public static EntityStoreIndex<T, TKey> CreateIndex<T, TKey>(
        this IEntityStore<T, Id<T>> store, Func<T, TKey> keySelector)
        where T : IHasId<Id<T>>
        where TKey : notnull
        => new(store, keySelector);

    /// <summary>Creates a secondary index grouping entity ids by <paramref name="keySelector"/>.</summary>
    public static EntityStoreIndex<T, TId, TKey> CreateIndex<T, TId, TKey>(
        this IEntityStore<T, TId> store, Func<T, TKey> keySelector)
        where T : IHasId<TId>
        where TId : notnull
        where TKey : notnull
        => new(store, keySelector);

    /// <summary>Creates a secondary unique index mapping each key to a single id.</summary>
    public static EntityStoreUniqueIndex<T, TKey> CreateUniqueIndex<T, TKey>(
        this IEntityStore<T, Id<T>> store, Func<T, TKey> keySelector)
        where T : IHasId<Id<T>>
        where TKey : notnull
        => new(store, keySelector);

    /// <summary>Creates a secondary unique index mapping each key to a single id.</summary>
    public static EntityStoreUniqueIndex<T, TId, TKey> CreateUniqueIndex<T, TId, TKey>(
        this IEntityStore<T, TId> store, Func<T, TKey> keySelector)
        where T : IHasId<TId>
        where TId : notnull
        where TKey : notnull
        => new(store, keySelector);

    /// <summary>Creates a view of the entities ordered by <paramref name="comparer"/>, re-sorted after each change.</summary>
    public static EntityStoreOrderedView<T, TId> CreateOrderedView<T, TId>(
        this IEntityStore<T, TId> store, IComparer<T> comparer)
        where T : IHasId<TId>
        where TId : notnull
        => new(store, comparer);

    /// <summary>Creates dense per-entity columns that follow the store's membership; see <see cref="EntityStoreColumns{T,TId}"/>.</summary>
    public static EntityStoreColumns<T, TId> CreateColumns<T, TId>(this IEntityStore<T, TId> store)
        where T : IHasId<TId>
        where TId : notnull
        => new(store);
}
