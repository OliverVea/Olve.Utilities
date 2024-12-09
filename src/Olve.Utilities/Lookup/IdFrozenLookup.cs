namespace Olve.Utilities.Lookup;

public class IdFrozenLookup<T, TId>(IEnumerable<T> items) : FrozenLookupBase<TId, T>(items.Select(x => new KeyValuePair<TId, T>(x.Id, x)))
    where T : IHasId<TId>
    where TId : notnull;