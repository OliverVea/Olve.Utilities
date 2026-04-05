# Pagination

API docs: [https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.Paginations.html](https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.Paginations.html)

Source: `src/Olve.Utilities/Paginations/`

## Pagination

Computes offset from zero-based page number and page size.

```csharp
public readonly record struct Pagination(int Page, int PageSize)
{
    public int Offset { get; } // == Page * PageSize
}
```

## PaginatedResult\<T\>

Wraps a page of items with total count and navigation metadata. Implements `IReadOnlyList<T>`.

```csharp
public sealed class PaginatedResult<T>(IList<T> items, Pagination pagination, int totalCount)
    : IReadOnlyList<T>
{
    public required int TotalCount { get; init; }
    public int TotalPages { get; }       // ceil(TotalCount / PageSize), 0 if PageSize == 0
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public bool HasNextPage { get; }     // (Page + 1) * PageSize < TotalCount
    public Pagination? Next { get; }     // next page Pagination, or null if last page

    // IReadOnlyList<T>
    public int Count { get; }
    public T this[int index] { get; }
    public IEnumerator<T> GetEnumerator();
}
```
