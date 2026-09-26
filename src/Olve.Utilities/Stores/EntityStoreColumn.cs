namespace Olve.Utilities.Stores;

/// <summary>
/// One column of an <see cref="EntityStoreColumns{T,TId}"/>: a value per row, stored contiguously.
/// </summary>
/// <typeparam name="TValue">The value type; prefer small structs for hot loops.</typeparam>
public sealed class EntityStoreColumn<TValue>
{
    private readonly IRowCount _owner;
    internal TValue[] Array;

    internal EntityStoreColumn(IRowCount owner, int capacity)
    {
        _owner = owner;
        Array = new TValue[capacity];
    }

    /// <summary>
    /// Gets the values of all rows, in row order (matching <see cref="EntityStoreColumns{T,TId}.Ids"/>).
    /// Valid until the next <see cref="EntityStoreColumns{T,TId}.Sync"/>.
    /// </summary>
    public Span<TValue> Values => Array.AsSpan(0, _owner.RowCount);

    /// <summary>Gets a reference to the value at <paramref name="row"/>.</summary>
    public ref TValue this[int row] => ref Values[row];
}
