namespace Olve.Utilities.Stores;

/// <summary>Raised by <see cref="IEntityStore{T,TId}.OnDeleted"/>: <paramref name="Entity"/> was removed from under <paramref name="Id"/>.</summary>
/// <param name="Id">The id of the removed entity.</param>
/// <param name="Entity">The value the store removed.</param>
public readonly record struct EntityDeleted<T, TId>(TId Id, T Entity);
