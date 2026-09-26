using System.Runtime.InteropServices;
using Olve.Results;

namespace Olve.Utilities.Paginations;

/// <summary>
///     Represents a pagination.
/// </summary>
/// <param name="Page">The (0-based) page number.</param>
/// <param name="PageSize">The number of items per page.</param>
[StructLayout(LayoutKind.Sequential)]
public readonly record struct Pagination(int Page, int PageSize)
{
    /// <summary>
    ///     The page size <see cref="Clamp" /> uses when none is given.
    /// </summary>
    public const int DefaultPageSize = 20;

    /// <summary>
    ///     The largest page size <see cref="Validate" /> and <see cref="Clamp" /> accept by default.
    /// </summary>
    public const int DefaultMaxPageSize = 100;

    /// <summary>
    ///     The offset of the pagination.
    /// </summary>
    public int Offset => Page * PageSize;

    /// <summary>
    ///     Validates that <see cref="Page" /> is not negative and that <see cref="PageSize" /> is between 1 and
    ///     <paramref name="maxPageSize" /> inclusive.
    /// </summary>
    /// <param name="maxPageSize">The largest allowed page size.</param>
    /// <returns>A successful <see cref="Result" />, or a failure listing every violated rule.</returns>
    public Result Validate(int maxPageSize = DefaultMaxPageSize) =>
        PaginationRules.Validate(Page, nameof(Page), PageSize, nameof(PageSize), maxPageSize);

    /// <summary>
    ///     Returns a valid pagination: a negative <see cref="Page" /> becomes 0, a <see cref="PageSize" /> of zero
    ///     or less becomes <paramref name="defaultPageSize" />, and a <see cref="PageSize" /> above
    ///     <paramref name="maxPageSize" /> becomes <paramref name="maxPageSize" />.
    /// </summary>
    /// <param name="defaultPageSize">The page size to use when none (zero or less) is given.</param>
    /// <param name="maxPageSize">The largest allowed page size.</param>
    /// <returns>The clamped pagination.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <paramref name="maxPageSize" /> is less than 1, or <paramref name="defaultPageSize" /> is not between 1
    ///     and <paramref name="maxPageSize" />.
    /// </exception>
    public Pagination Clamp(int defaultPageSize = DefaultPageSize, int maxPageSize = DefaultMaxPageSize)
    {
        var (page, pageSize) = PaginationRules.Clamp(Page, PageSize, defaultPageSize, maxPageSize);
        return new Pagination(page, pageSize);
    }

    /// <summary>
    ///     Converts this pagination to the equivalent <see cref="OffsetPagination" />. This is always possible.
    /// </summary>
    /// <returns>
    ///     An <see cref="OffsetPagination" /> with <see cref="OffsetPagination.Offset" /> = <see cref="Offset" />
    ///     and <see cref="OffsetPagination.Limit" /> = <see cref="PageSize" />.
    /// </returns>
    public OffsetPagination ToOffsetPagination() => new(Offset, PageSize);
}
