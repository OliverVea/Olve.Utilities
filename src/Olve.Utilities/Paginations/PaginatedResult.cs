using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Olve.Utilities.Paginations;

/// <summary>
/// Represents a paginated result.
/// </summary>
/// <param name="items">The items in the current page.</param>
/// <param name="pagination">The pagination information for the results.</param>
/// <param name="totalCount">The total count of items.</param>
/// <typeparam name="T">The type of the items in the paginated result.</typeparam>
[method: SetsRequiredMembers]
public sealed class PaginatedResult<T>(IList<T> items, Pagination pagination, int totalCount) : IReadOnlyList<T>
{
    private ReadOnlyCollection<T> Items { get; init; } = items.AsReadOnly();

    /// <summary>
    /// The total count of items.
    /// </summary>
    [Range(0, int.MaxValue)]
    public required int TotalCount { get; init; } = totalCount;

    /// <summary>
    /// The page number.
    /// </summary>
    [Range(0, int.MaxValue)]
    public required int Page { get; init; } = pagination.Page;

    
    /// <summary>
    /// The size of the page.
    /// </summary>
    [Range(1, int.MaxValue)]
    public required int PageSize { get; init; } = pagination.PageSize;

    /// <summary>
    /// If there is a next page or if the current page is the last page based on the total count and page number and size.
    /// </summary>
    public bool HasNextPage => (Page + 1) * PageSize < TotalCount;
    
    /// <summary>
    /// The pagination information for the previous page.
    /// </summary>
    public Pagination? Next => HasNextPage ? new Pagination(Page + 1, PageSize) : null;
    
    
    // IReadOnlyList<T> implementation
    
    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    
    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    /// <inheritdoc />
    public int Count => Items.Count;
    
    /// <inheritdoc />
    public T this[int index] => Items[index];
}