using Olve.Utilities.Ids;
using Olve.Utilities.Lookup;

namespace Olve.Utilities.Stores;

/// <summary>
/// Builds entity stores from collection expressions, e.g. <c>EntityStore&lt;Train&gt; trains = [];</c>.
/// </summary>
public static class EntityStoreBuilder
{
    /// <summary>Creates an <see cref="EntityStore{T}"/> seeded with <paramref name="entities"/>.</summary>
    public static EntityStore<T> Create<T>(ReadOnlySpan<T> entities)
        where T : IHasId<Id<T>>
        => new(entities.ToArray());

    /// <summary>Creates an <see cref="EntityStore{T,TId}"/> seeded with <paramref name="entities"/>.</summary>
    public static EntityStore<T, TId> Create<T, TId>(ReadOnlySpan<T> entities)
        where T : IHasId<TId>
        where TId : notnull
        => new(entities.ToArray());
}
