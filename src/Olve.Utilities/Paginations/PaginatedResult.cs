using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Olve.Utilities.Paginations;

[method: SetsRequiredMembers]
public sealed class PaginatedResult<T>(IList<T> items, Pagination pagination, int totalCount) : IReadOnlyList<T>
{
    private IReadOnlyList<T> Items { get; init; } = items.AsReadOnly();

    [Range(0, int.MaxValue)]
    public required int TotalCount { get; init; } = totalCount;

    [Range(0, int.MaxValue)]
    public required int Page { get; init; } = pagination.Page;

    [Range(1, int.MaxValue)]
    public required int PageSize { get; init; } = pagination.PageSize;

    public bool HasNextPage => (Page + 1) * PageSize < TotalCount;
    public Pagination? Next => HasNextPage ? new Pagination(Page + 1, PageSize) : null;
    
    
    // IReadOnlyList<T> implementation
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public int Count => Items.Count;
    public T this[int index] => Items[index];
}