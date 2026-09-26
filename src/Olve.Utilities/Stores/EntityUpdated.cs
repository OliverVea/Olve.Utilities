namespace Olve.Utilities.Stores;

/// <summary>Raised by <see cref="IEntityStore{T,TId}.OnUpdated"/>: the entity under <paramref name="Id"/> changed from <paramref name="Before"/> to <paramref name="After"/>.</summary>
/// <param name="Id">The id of the updated entity.</param>
/// <param name="Before">The value the write replaced.</param>
/// <param name="After">The value the write committed.</param>
public readonly record struct EntityUpdated<T, TId>(TId Id, T Before, T After);
