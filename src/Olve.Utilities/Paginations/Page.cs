namespace Olve.Utilities.Paginations;

/// <summary>
///     A single page of items from a larger collection, with metadata describing
///     position within that collection.
/// </summary>
/// <param name="Items">The items on this page.</param>
/// <param name="PageNumber">Zero-based index of this page.</param>
/// <param name="PageSize">Maximum number of items per page.</param>
/// <param name="TotalCount">Total number of items across all pages.</param>
public sealed record Page<T>(
    IReadOnlyList<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount)
{
    /// <summary>
    ///     Total number of pages. Zero when <see cref="PageSize" /> is zero.
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;

    /// <summary>
    ///     Whether another page exists after this one.
    /// </summary>
    public bool HasNextPage => (PageNumber + 1) * PageSize < TotalCount;

    /// <summary>
    ///     The <see cref="Pagination" /> that would fetch the next page, or
    ///     <c>null</c> when this is the last page.
    /// </summary>
    public Pagination? Next => HasNextPage ? new Pagination(PageNumber + 1, PageSize) : null;

    /// <summary>
    ///     Converts this page to the equivalent <see cref="Slice{T}" />. This is always possible.
    /// </summary>
    /// <returns>
    ///     A <see cref="Slice{T}" /> with the same items and total count, where <see cref="Slice{T}.Offset" /> =
    ///     <see cref="PageNumber" /> * <see cref="PageSize" /> and <see cref="Slice{T}.Limit" /> =
    ///     <see cref="PageSize" />.
    /// </returns>
    public Slice<T> ToSlice() => new(Items, PageNumber * PageSize, PageSize, TotalCount);
}
