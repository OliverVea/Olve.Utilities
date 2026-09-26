# Pagination

API docs: [https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.Paginations.html](https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.Paginations.html)

Source: `src/Olve.Utilities/Paginations/` (namespace `Olve.Utilities.Paginations`, plural)

Two request shapes, each with a matching result:

| Request | Result | Use for |
| --- | --- | --- |
| `Pagination(Page, PageSize)` | `Page<T>` | page-number APIs (`?page=2&pageSize=20`) |
| `OffsetPagination(Offset, Limit)` | `Slice<T>` | offset/limit APIs, where the offset need not be a multiple of the limit |

**Bounds are always explicit.** There are no library defaults or maximums: callers pass them to `Validate(max)` or `Clamp(default, max)`. `Paginate(...)` throws `ArgumentOutOfRangeException` on a negative page/offset or a size/limit of zero or less, so always `Clamp` (lenient) or `Validate` (strict) request input first.

## Pagination

```csharp
public readonly record struct Pagination(int Page, int PageSize)   // Page is 0-based
{
    public int Offset { get; }                                       // Page * PageSize
    public Result Validate(int maxPageSize);                         // Page >= 0, 1 <= PageSize <= maxPageSize; lists every violation
    public Pagination Clamp(int defaultPageSize, int maxPageSize);   // Page < 0 -> 0, PageSize <= 0 -> default, > max -> max
    public OffsetPagination ToOffsetPagination();                    // always possible
}
```

`Clamp` throws `ArgumentOutOfRangeException` if `maxPageSize < 1` or `defaultPageSize` is not in `1..maxPageSize`.

## OffsetPagination

```csharp
public readonly record struct OffsetPagination(int Offset, int Limit)
{
    public Result Validate(int maxLimit);                            // Offset >= 0, 1 <= Limit <= maxLimit
    public OffsetPagination Clamp(int defaultLimit, int maxLimit);   // Offset < 0 -> 0, Limit <= 0 -> default, > max -> max
    public bool TryToPagination(out Pagination pagination);          // only when Offset % Limit == 0 (and Offset >= 0, Limit > 0)
}
```

## Page\<T\>

```csharp
public sealed record Page<T>(IReadOnlyList<T> Items, int PageNumber, int PageSize, int TotalCount)
{
    public int TotalPages { get; }       // ceil(TotalCount / PageSize), 0 if PageSize == 0
    public bool HasNextPage { get; }     // (PageNumber + 1) * PageSize < TotalCount
    public Pagination? Next { get; }     // request for the next page, or null on the last page
    public Slice<T> ToSlice();           // always possible
}
```

Note the property is `PageNumber` (the request's field is `Page`).

## Slice\<T\>

```csharp
public sealed record Slice<T>(IReadOnlyList<T> Items, int Offset, int Limit, int TotalCount)
{
    public bool HasMore { get; }                 // Offset + Limit < TotalCount
    public OffsetPagination? Next { get; }       // request for the next slice, or null at the end
    public bool TryToPage([NotNullWhen(true)] out Page<T>? page);  // only when Offset % Limit == 0
}
```

## PaginationExtensions

```csharp
public static class PaginationExtensions
{
    public static Page<T> Paginate<T>(this IEnumerable<T> source, Pagination pagination);
    public static Page<T> Paginate<T>(this IQueryable<T> source, Pagination pagination);
    public static Slice<T> Paginate<T>(this IEnumerable<T> source, OffsetPagination pagination);
    public static Slice<T> Paginate<T>(this IQueryable<T> source, OffsetPagination pagination);
}
```

- `IEnumerable<T>`: enumerated once; materialized to a list unless it already is an `IReadOnlyList<T>`.
- `IQueryable<T>`: `Count()`, `Skip`, `Take` are composed onto the query (EF translates them). The query executes twice: once to count, once to fetch.

## Examples

Lenient: clamp whatever the client sent.

```csharp
var request = new OffsetPagination(Offset: 1, Limit: 2).Clamp(defaultLimit: 20, maxLimit: 100);

Slice<string> slice = users.Paginate(request);
// slice.Items == ["bob", "charlie"], slice.HasMore == true
// slice.Next == OffsetPagination { Offset = 3, Limit = 2 }
```

Strict: reject invalid input as a `Result`.

```csharp
Result<Page<User>> GetUsers(Pagination pagination)
{
    if (pagination.Validate(maxPageSize: 100).TryPickProblems(out var problems))
    {
        return problems;
    }

    return db.Users.OrderBy(u => u.Name).Paginate(pagination);
}
```

Conversions:

```csharp
var offset = new Pagination(Page: 2, PageSize: 2).ToOffsetPagination(); // Offset = 4, Limit = 2
offset.TryToPagination(out var pagination);                              // true, Page = 2, PageSize = 2
new OffsetPagination(1, 2).TryToPagination(out _);                        // false, 1 is not a multiple of 2

Slice<string> asSlice = page.ToSlice();
if (slice.TryToPage(out var asPage)) { /* ... */ }
```
