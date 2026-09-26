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
/// compare-and-swap), and every change fires a synchronous <see cref="Event{T}"/> carrying the
/// committed values, so secondary indexes stay consistent with the store.
/// </summary>
/// <remarks>
/// Entities must be immutable values (records, changed with <c>with</c>): event payloads and readers share
/// the stored instance. Events for the same id can arrive out of order across threads; see
/// <see cref="IEntityStore{T,TId}"/>.
/// </remarks>
/// <typeparam name="T">The entity type, which must expose a <typeparamref name="TId"/>.</typeparam>
/// <typeparam name="TId">The identifier type.</typeparam>
[CollectionBuilder(typeof(EntityStoreBuilder), nameof(EntityStoreBuilder.Create))]
public class EntityStore<T, TId> : IEntityStore<T, TId>, IEnumerable<T>
    where T : IHasId<TId>
    where TId : notnull
{
    private readonly ConcurrentDictionary<TId, T> _entities;

    // Kept alongside the dictionary: ConcurrentDictionary.Count takes every internal lock.
    private int _count;

    /// <summary>Creates an empty store.</summary>
    public EntityStore() : this([])
    {
    }

    /// <summary>Creates a store seeded with <paramref name="initialEntities"/>.</summary>
    public EntityStore(IEnumerable<T> initialEntities)
    {
        _entities = new(initialEntities.Select(e => new KeyValuePair<TId, T>(e.Id, e)));
        _count = _entities.Count;
    }

    /// <summary>Fires after an entity not previously present is added via <see cref="Set"/> or <see cref="TryAdd"/>.</summary>
    public Event<EntityAdded<T, TId>> OnAdded { get; } = new();

    /// <summary>Fires after an existing entity changes via <see cref="Set"/> or <see cref="Mutate"/>.</summary>
    public Event<EntityUpdated<T, TId>> OnUpdated { get; } = new();

    /// <summary>Fires after an entity is removed via <see cref="Delete"/>.</summary>
    public Event<EntityDeleted<T, TId>> OnDeleted { get; } = new();

    /// <summary>
    /// Inserts or replaces <paramref name="entity"/>, firing <see cref="OnAdded"/> when it is new or
    /// <see cref="OnUpdated"/> when it replaces a different value. Writing a value equal to the current
    /// one is a no-op and fires nothing, as with <see cref="Mutate"/>.
    /// </summary>
    public void Set(T entity)
    {
        var id = entity.Id;

        // Decide add vs. update by what the write actually did. Checking for the id first and then
        // writing races a concurrent Delete: the write re-adds the entity but would report an update.
        while (true)
        {
            if (_entities.TryAdd(id, entity))
            {
                Interlocked.Increment(ref _count);
                OnAdded.Invoke(new(id, entity));
                return;
            }

            if (_entities.TryGetValue(id, out var current))
            {
                if (EqualityComparer<T>.Default.Equals(current, entity)) return;

                if (_entities.TryUpdate(id, entity, current))
                {
                    OnUpdated.Invoke(new(id, current, entity));
                    return;
                }
            }
            // removed or replaced between the two calls; retry
        }
    }

    /// <summary>
    /// Adds <paramref name="entity"/> only if no entity with its id is present, firing
    /// <see cref="OnAdded"/> on success. Returns <see langword="false"/> and leaves the existing entity
    /// untouched otherwise. The check and the insert are one atomic step.
    /// </summary>
    public bool TryAdd(T entity)
    {
        if (!_entities.TryAdd(entity.Id, entity)) return false;

        Interlocked.Increment(ref _count);
        OnAdded.Invoke(new(entity.Id, entity));
        return true;
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
                OnUpdated.Invoke(new(id, current, updated));
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

    /// <summary>
    /// Gets the number of entities currently in the store. Lock-free; while other threads write it may
    /// briefly lag a write that has already committed.
    /// </summary>
    // A delete can decrement before the matching add increments, so the raw counter may dip below zero.
    public int Count => Math.Max(0, Volatile.Read(ref _count));

    /// <summary>
    /// Returns a copy of the entities currently in the store. Lock-free, so like enumeration it is not a
    /// moment-in-time snapshot while other threads write.
    /// </summary>
    public IReadOnlyList<T> List()
    {
        var list = new List<T>(Count);
        foreach (var entry in _entities) list.Add(entry.Value);
        return list;
    }

    /// <summary>Removes the entity with <paramref name="id"/>, firing <see cref="OnDeleted"/> on success.</summary>
    public DeletionResult Delete(TId id)
    {
        if (!_entities.TryRemove(id, out var removed))
            return DeletionResult.NotFound();

        Interlocked.Decrement(ref _count);
        OnDeleted.Invoke(new(id, removed));
        return DeletionResult.Success();
    }

    /// <summary>Returns whether an entity with <paramref name="id"/> is present.</summary>
    public bool Contains(TId id) => _entities.ContainsKey(id);

    /// <summary>
    /// Enumerates the entities as a live view: no copy and no locks, safe while other threads write, but
    /// not a moment-in-time snapshot — entities added or removed during enumeration may or may not appear.
    /// <see cref="List"/> copies it.
    /// </summary>
    public IEnumerator<T> GetEnumerator() => _entities.Select(entry => entry.Value).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>
/// An <see cref="EntityStore{T,TId}"/> keyed by <see cref="Id{T}"/> — the default for durable,
/// globally-identified entities.
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
}
