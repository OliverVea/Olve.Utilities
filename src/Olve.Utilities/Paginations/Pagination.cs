namespace Olve.Utilities.Paginations;

/// <summary>
/// Represents a pagination.
/// </summary>
/// <param name="Page">The (0-based) page number.</param>
/// <param name="PageSize">The number of items per page.</param>
public readonly record struct Pagination(int Page, int PageSize)
{
    /// <summary>
    /// The offset of the pagination.
    /// </summary>
    public int Offset => Page * PageSize;
}