namespace Olve.Utilities.Lookup;

/// <summary>
///     A lookup that maps items to their Ids.
/// </summary>
/// <param name="items">The items to include in the lookup.</param>
/// <typeparam name="T">The type of the items in the lookup. Must implement <see cref="IHasId{TId}" />.</typeparam>
/// <typeparam name="TId">The type of the Ids of the items in the lookup.</typeparam>
public class IdFrozenLookup<T, TId>(IEnumerable<T> items)
    : FrozenLookupBase<TId, T>(items.Select(x => new KeyValuePair<TId, T>(x.Id, x)))
    where T : IHasId<TId>
    where TId : notnull;