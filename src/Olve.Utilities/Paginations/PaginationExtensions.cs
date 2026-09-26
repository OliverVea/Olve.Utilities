namespace Olve.Utilities.Paginations;

/// <summary>
///     Applies a <see cref="Pagination" /> or <see cref="OffsetPagination" /> to a sequence.
/// </summary>
public static class PaginationExtensions
{
    /// <summary>
    ///     Returns the page of <paramref name="source" /> described by <paramref name="pagination" />, together
    ///     with the total number of items in <paramref name="source" />.
    /// </summary>
    /// <param name="source">The items to paginate. Enumerated once.</param>
    /// <param name="pagination">The page to take.</param>
    /// <typeparam name="T">The type of the items.</typeparam>
    /// <returns>The requested page.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <see cref="Pagination.Page" /> is negative, or <see cref="Pagination.PageSize" /> is zero or negative.
    /// </exception>
    public static Page<T> Paginate<T>(this IEnumerable<T> source, Pagination pagination)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfNegative(pagination.Page, nameof(pagination));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pagination.PageSize, nameof(pagination));

        var (items, totalCount) = Take(source, pagination.Offset, pagination.PageSize);
        return new Page<T>(items, pagination.Page, pagination.PageSize, totalCount);
    }

    /// <summary>
    ///     Returns the page of <paramref name="source" /> described by <paramref name="pagination" />, together
    ///     with the total number of items in <paramref name="source" />. The count, skip and take are composed
    ///     onto the query, so query providers such as Entity Framework translate them.
    /// </summary>
    /// <param name="source">The query to paginate. Executed twice: once to count, once to fetch the page.</param>
    /// <param name="pagination">The page to take.</param>
    /// <typeparam name="T">The type of the items.</typeparam>
    /// <returns>The requested page.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <see cref="Pagination.Page" /> is negative, or <see cref="Pagination.PageSize" /> is zero or negative.
    /// </exception>
    public static Page<T> Paginate<T>(this IQueryable<T> source, Pagination pagination)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfNegative(pagination.Page, nameof(pagination));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pagination.PageSize, nameof(pagination));

        var (items, totalCount) = Take(source, pagination.Offset, pagination.PageSize);
        return new Page<T>(items, pagination.Page, pagination.PageSize, totalCount);
    }

    /// <summary>
    ///     Returns the slice of <paramref name="source" /> described by <paramref name="pagination" />, together
    ///     with the total number of items in <paramref name="source" />.
    /// </summary>
    /// <param name="source">The items to paginate. Enumerated once.</param>
    /// <param name="pagination">The slice to take.</param>
    /// <typeparam name="T">The type of the items.</typeparam>
    /// <returns>The requested slice.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <see cref="OffsetPagination.Offset" /> is negative, or <see cref="OffsetPagination.Limit" /> is zero or negative.
    /// </exception>
    public static Slice<T> Paginate<T>(this IEnumerable<T> source, OffsetPagination pagination)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfNegative(pagination.Offset, nameof(pagination));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pagination.Limit, nameof(pagination));

        var (items, totalCount) = Take(source, pagination.Offset, pagination.Limit);
        return new Slice<T>(items, pagination.Offset, pagination.Limit, totalCount);
    }

    /// <summary>
    ///     Returns the slice of <paramref name="source" /> described by <paramref name="pagination" />, together
    ///     with the total number of items in <paramref name="source" />. The count, skip and take are composed
    ///     onto the query, so query providers such as Entity Framework translate them.
    /// </summary>
    /// <param name="source">The query to paginate. Executed twice: once to count, once to fetch the slice.</param>
    /// <param name="pagination">The slice to take.</param>
    /// <typeparam name="T">The type of the items.</typeparam>
    /// <returns>The requested slice.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <see cref="OffsetPagination.Offset" /> is negative, or <see cref="OffsetPagination.Limit" /> is zero or negative.
    /// </exception>
    public static Slice<T> Paginate<T>(this IQueryable<T> source, OffsetPagination pagination)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfNegative(pagination.Offset, nameof(pagination));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pagination.Limit, nameof(pagination));

        var (items, totalCount) = Take(source, pagination.Offset, pagination.Limit);
        return new Slice<T>(items, pagination.Offset, pagination.Limit, totalCount);
    }

    private static (IReadOnlyList<T> Items, int TotalCount) Take<T>(IEnumerable<T> source, int offset, int count)
    {
        var all = source as IReadOnlyList<T> ?? source.ToList();
        var items = all.Skip(offset).Take(count).ToList();
        return (items, all.Count);
    }

    private static (IReadOnlyList<T> Items, int TotalCount) Take<T>(IQueryable<T> source, int offset, int count)
    {
        var totalCount = source.Count();
        var items = source.Skip(offset).Take(count).ToList();
        return (items, totalCount);
    }
}
