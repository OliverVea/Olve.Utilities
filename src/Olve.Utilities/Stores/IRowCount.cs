namespace Olve.Utilities.Stores;

/// <summary>Lets a column read its owner's current row count.</summary>
internal interface IRowCount
{
    int RowCount { get; }
}
