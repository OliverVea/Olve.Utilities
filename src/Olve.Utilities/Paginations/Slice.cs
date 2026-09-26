using System.Diagnostics.CodeAnalysis;

namespace Olve.Utilities.Paginations;

/// <summary>
///     A contiguous slice of items from a larger collection, addressed by offset and limit, with metadata
///     describing position within that collection.
/// </summary>
/// <param name="Items">The items in this slice.</param>
/// <param name="Offset">Zero-based index of the first item of this slice within the collection.</param>
/// <param name="Limit">Maximum number of items per slice.</param>
/// <param name="TotalCount">Total number of items in the collection.</param>
public sealed record Slice<T>(
    IReadOnlyList<T> Items,
    int Offset,
    int Limit,
    int TotalCount)
{
    /// <summary>
    ///     Whether more items exist after this slice.
    /// </summary>
    public bool HasMore => Offset + Limit < TotalCount;

    /// <summary>
    ///     The <see cref="OffsetPagination" /> that would fetch the next slice, or
    ///     <c>null</c> when this is the last slice.
    /// </summary>
    public OffsetPagination? Next => HasMore ? new OffsetPagination(Offset + Limit, Limit) : null;

    /// <summary>
    ///     Converts this slice to the equivalent <see cref="Page{T}" />. This is only possible when
    ///     <see cref="Offset" /> is a multiple of <see cref="Limit" />
    ///     (see <see cref="OffsetPagination.TryToPagination" />).
    /// </summary>
    /// <param name="page">The equivalent page, or <c>null</c> when no equivalent exists.</param>
    /// <returns><c>true</c> if an equivalent page exists; otherwise <c>false</c>.</returns>
    public bool TryToPage([NotNullWhen(true)] out Page<T>? page)
    {
        if (!new OffsetPagination(Offset, Limit).TryToPagination(out var pagination))
        {
            page = null;
            return false;
        }

        page = new Page<T>(Items, pagination.Page, pagination.PageSize, TotalCount);
        return true;
    }
}
