namespace Olve.Utilities.Stores;

/// <summary>Raised by <see cref="IEntityStore{T,TId}.OnAdded"/>: <paramref name="Entity"/> was added under <paramref name="Id"/>.</summary>
/// <param name="Id">The id of the added entity.</param>
/// <param name="Entity">The value the store committed.</param>
public readonly record struct EntityAdded<T, TId>(TId Id, T Entity);
