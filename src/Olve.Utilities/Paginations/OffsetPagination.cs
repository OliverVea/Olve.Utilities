using System.Runtime.InteropServices;
using Olve.Results;

namespace Olve.Utilities.Paginations;

/// <summary>
///     Represents an offset/limit pagination.
/// </summary>
/// <param name="Offset">The (0-based) number of items to skip.</param>
/// <param name="Limit">The maximum number of items to take.</param>
[StructLayout(LayoutKind.Sequential)]
public readonly record struct OffsetPagination(int Offset, int Limit)
{
    /// <summary>
    ///     Validates that <see cref="Offset" /> is not negative and that <see cref="Limit" /> is between 1 and
    ///     <paramref name="maxLimit" /> inclusive.
    /// </summary>
    /// <param name="maxLimit">The largest allowed limit.</param>
    /// <returns>A successful <see cref="Result" />, or a failure listing every violated rule.</returns>
    public Result Validate(int maxLimit) =>
        PaginationRules.Validate(Offset, nameof(Offset), Limit, nameof(Limit), maxLimit);

    /// <summary>
    ///     Returns a valid pagination: a negative <see cref="Offset" /> becomes 0, a <see cref="Limit" /> of zero
    ///     or less becomes <paramref name="defaultLimit" />, and a <see cref="Limit" /> above
    ///     <paramref name="maxLimit" /> becomes <paramref name="maxLimit" />.
    /// </summary>
    /// <param name="defaultLimit">The limit to use when none (zero or less) is given.</param>
    /// <param name="maxLimit">The largest allowed limit.</param>
    /// <returns>The clamped pagination.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <paramref name="maxLimit" /> is less than 1, or <paramref name="defaultLimit" /> is not between 1
    ///     and <paramref name="maxLimit" />.
    /// </exception>
    public OffsetPagination Clamp(int defaultLimit, int maxLimit)
    {
        var (offset, limit) = PaginationRules.Clamp(Offset, Limit, defaultLimit, maxLimit);
        return new OffsetPagination(offset, limit);
    }

    /// <summary>
    ///     Converts this pagination to the equivalent page-based <see cref="Pagination" />. This is only possible
    ///     when <see cref="Offset" /> is not negative, <see cref="Limit" /> is positive, and <see cref="Offset" />
    ///     is a multiple of <see cref="Limit" />.
    /// </summary>
    /// <param name="pagination">
    ///     A <see cref="Pagination" /> with <see cref="Pagination.Page" /> = <see cref="Offset" /> /
    ///     <see cref="Limit" /> and <see cref="Pagination.PageSize" /> = <see cref="Limit" />, or <c>default</c>
    ///     when no equivalent exists.
    /// </param>
    /// <returns><c>true</c> if an equivalent page-based pagination exists; otherwise <c>false</c>.</returns>
    public bool TryToPagination(out Pagination pagination)
    {
        if (Offset < 0 || Limit <= 0 || Offset % Limit != 0)
        {
            pagination = default;
            return false;
        }

        pagination = new Pagination(Offset / Limit, Limit);
        return true;
    }
}
