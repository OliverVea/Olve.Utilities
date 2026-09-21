using System.Diagnostics.CodeAnalysis;
using Olve.Results;
using Olve.Utilities.Ids;
using Olve.Utilities.Lookup;

namespace Olve.Utilities.Stores;

/// <summary>
/// The entity-at-a-time surface of a store: get, set, mutate, delete, and the change events that
/// keep secondary indexes consistent.
/// <para>
/// This is deliberately <em>not</em> the interface a simulation loop should use. Every member here
/// deals in whole entities, so an implementation that stores its data column-wise has to materialise
/// one per call. Such an implementation should satisfy this interface for persistence, seeding and
/// admin queries, and expose its columns directly for the hot path.
/// </para>
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
/// <typeparam name="TId">The identifier type, typically <see cref="Id{T}"/> or <see cref="ShortId{T}"/>.</typeparam>
public interface IEntityStore<T, TId>
    where T : IHasId<TId>
    where TId : notnull
{
    /// <summary>Fires after an entity not previously present is added.</summary>
    Event<TId> OnAdded { get; }

    /// <summary>Fires after an existing entity changes.</summary>
    Event<TId> OnUpdated { get; }

    /// <summary>Fires after an entity is removed.</summary>
    Event<TId> OnDeleted { get; }

    /// <summary>Inserts or replaces <paramref name="entity"/>.</summary>
    void Set(T entity);

    /// <summary>Atomically read-modify-write the entity with <paramref name="id"/>.</summary>
    Result Mutate(TId id, Func<T, T> mutate);

    /// <summary>Gets the entity with <paramref name="id"/>, returning <see langword="false"/> if absent.</summary>
    bool TryGet(TId id, [NotNullWhen(true)] out T? entity);

    /// <summary>Gets the number of entities currently in the store.</summary>
    int Count { get; }

    /// <summary>Returns a snapshot of all entities currently in the store.</summary>
    IReadOnlyList<T> List();

    /// <summary>Removes the entity with <paramref name="id"/>.</summary>
    DeletionResult Delete(TId id);

    /// <summary>Returns whether an entity with <paramref name="id"/> is present.</summary>
    bool Contains(TId id);
}
