namespace Olve.Utilities.Lookup;

/// <summary>
///     Represents an object that has an identifier.
/// </summary>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public interface IHasId<out TId>
    where TId : notnull
{
    /// <summary>
    ///     The identifier of the object.
    /// </summary>
    TId Id { get; }
}
