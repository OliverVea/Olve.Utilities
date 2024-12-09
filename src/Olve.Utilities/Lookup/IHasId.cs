namespace Olve.Utilities.Lookup;

public interface IHasId<out TId>
    where TId : notnull
{
    TId Id { get; }
}