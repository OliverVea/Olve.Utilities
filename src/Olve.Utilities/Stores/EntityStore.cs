using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Olve.Results;
using Olve.Utilities.Ids;
using Olve.Utilities.Lookup;

namespace Olve.Utilities.Stores;

/// <summary>
/// A mutable, observable in-memory store of entities keyed by <see cref="Id{T}"/>. It is the
/// mutable, eventful sibling of <see cref="IdFrozenLookup{T,TId}"/>: reads and writes are concurrent
/// (a <see cref="ConcurrentDictionary{TKey,TValue}"/> backs it, and <see cref="Mutate"/> uses
/// compare-and-swap), and every change fires a synchronous <see cref="Event{T}"/> so secondary
/// indexes stay consistent with the store.
/// </summary>
/// <typeparam name="T">The entity type, which must expose a <typeparamref name="TId"/>.</typeparam>
/// <typeparam name="TId">The identifier type.</typeparam>
[CollectionBuilder(typeof(EntityStoreBuilder), nameof(EntityStoreBuilder.Create))]
public class EntityStore<T, TId> : IEntityStore<T, TId>, IEnumerable<T>
    where T : IHasId<TId>
    where TId : notnull
{
    private readonly ConcurrentDictionary<TId, T> _entities;

    /// <summary>Creates an empty store.</summary>
    public EntityStore() : this([])
    {
    }

    /// <summary>Creates a store seeded with <paramref name="initialEntities"/>.</summary>
    public EntityStore(IEnumerable<T> initialEntities)
    {
        _entities = new(initialEntities.Select(e => new KeyValuePair<TId, T>(e.Id, e)));
    }

    /// <summary>Fires after an entity not previously present is added via <see cref="Set"/>.</summary>
    public Event<TId> OnAdded { get; } = new();

    /// <summary>Fires after an existing entity changes via <see cref="Set"/> or <see cref="Mutate"/>.</summary>
    public Event<TId> OnUpdated { get; } = new();

    /// <summary>Fires after an entity is removed via <see cref="Delete"/>.</summary>
    public Event<TId> OnDeleted { get; } = new();

    /// <summary>
    /// Inserts or replaces <paramref name="entity"/>, firing <see cref="OnAdded"/> when it is new or
    /// <see cref="OnUpdated"/> when it replaces an existing entity.
    /// </summary>
    public void Set(T entity)
    {
        var isUpdate = _entities.ContainsKey(entity.Id);
        _entities[entity.Id] = entity;

        if (isUpdate)
            OnUpdated.Invoke(entity.Id);
        else
            OnAdded.Invoke(entity.Id);
    }

    /// <summary>
    /// Max compare-and-swap attempts <see cref="Mutate"/> makes before giving up. Real same-key
    /// contention here is a handful of writers, so this is generous headroom; exhausting it signals
    /// pathological contention (or a non-pure <c>mutate</c>), not an expected outcome.
    /// </summary>
    private const int MaxMutateAttempts = 10;

    /// <summary>
    /// Atomically read-modify-write the entity with <paramref name="id"/>. <paramref name="mutate"/>
    /// may run more than once if it loses a compare-and-swap race, so it must be a pure function of
    /// its input (use a <c>with</c> expression; no side effects, no logging). Fires
    /// <see cref="OnUpdated"/> exactly once on a real change, never on a no-op or a missing entity.
    /// Fails if the entity does not exist, or if the CAS could not commit within
    /// <see cref="MaxMutateAttempts"/> attempts (the caller may retry the latter).
    ///
    /// MUST NOT change a value any index keys on — indexes track only <see cref="OnAdded"/>/
    /// <see cref="OnDeleted"/> by design. For a key change, use <see cref="Delete"/>+<see cref="Set"/>.
    /// </summary>
    public Result Mutate(TId id, Func<T, T> mutate)
    {
        for (var attempt = 0; attempt < MaxMutateAttempts; attempt++)
        {
            if (!_entities.TryGetValue(id, out var current))
                return new ResultProblem("Entity with id '{0}' not found.", id);

            var updated = mutate(current);
            if (EqualityComparer<T>.Default.Equals(updated, current))
                return Result.Success(); // present but unchanged: do not fire

            if (_entities.TryUpdate(id, updated, current))
            {
                OnUpdated.Invoke(id);
                return Result.Success();
            }
            // lost the CAS race; another writer moved it — re-read and retry
        }

        return new ResultProblem(
            "Could not commit mutation of entity '{0}' after {1} attempts under contention.",
            id, MaxMutateAttempts);
    }

    /// <summary>Gets the entity with <paramref name="id"/>, returning <see langword="false"/> if absent.</summary>
    public bool TryGet(TId id, [NotNullWhen(true)] out T? entity) => _entities.TryGetValue(id, out entity);

    /// <summary>Gets the number of entities currently in the store.</summary>
    public int Count => _entities.Count;

    /// <summary>Returns a snapshot of all entities currently in the store.</summary>
    public IReadOnlyList<T> List() => _entities.Values.ToList();

    /// <summary>Removes the entity with <paramref name="id"/>, firing <see cref="OnDeleted"/> on success.</summary>
    public DeletionResult Delete(TId id)
    {
        if (!_entities.TryRemove(id, out _))
            return DeletionResult.NotFound();

        OnDeleted.Invoke(id);
        return DeletionResult.Success();
    }

    /// <summary>Returns whether an entity with <paramref name="id"/> is present.</summary>
    public bool Contains(TId id) => _entities.ContainsKey(id);

    /// <summary>
    /// Enumerates the entities as a live view: no copy and no locks, safe while other threads write, but
    /// not a moment-in-time snapshot — entities added or removed during enumeration may or may not appear.
    /// Use <see cref="List"/> for a snapshot.
    /// </summary>
    public IEnumerator<T> GetEnumerator() => _entities.Select(entry => entry.Value).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>
/// An <see cref="EntityStore{T,TId}"/> keyed by <see cref="Id{T}"/> — the default for durable,
/// globally-identified entities. Secondary indexes hang off this shape.
/// </summary>
/// <typeparam name="T">The entity type, which must expose an <see cref="Id{T}"/>.</typeparam>
[CollectionBuilder(typeof(EntityStoreBuilder), nameof(EntityStoreBuilder.Create))]
public class EntityStore<T>(IEnumerable<T> initialEntities) : EntityStore<T, Id<T>>(initialEntities)
    where T : IHasId<Id<T>>
{
    /// <summary>Creates an empty store.</summary>
    public EntityStore() : this([])
    {
    }

    /// <summary>Creates a secondary index grouping entity ids by <paramref name="keySelector"/>.</summary>
    /// <remarks>
    /// The index subscribes to this store's events, so the store keeps it alive. The caller owns it:
    /// keep it for the store's lifetime, or dispose it when done. An index built per use and never
    /// disposed leaks, and every stale index still runs on each add and delete.
    /// </remarks>
    public EntityStoreIndex<T, TKey> CreateIndex<TKey>(Func<T, TKey> keySelector) where TKey : notnull
        => new(this, keySelector);

    /// <summary>Creates a secondary unique index mapping each key to a single id.</summary>
    /// <remarks>
    /// The caller owns the index: keep it for the store's lifetime, or dispose it when done (see
    /// <see cref="CreateIndex{TKey}"/>).
    /// </remarks>
    public EntityStoreUniqueIndex<T, TKey> CreateUniqueIndex<TKey>(Func<T, TKey> keySelector) where TKey : notnull
        => new(this, keySelector);
}
