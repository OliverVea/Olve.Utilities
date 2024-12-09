namespace Olve.Utilities.Paginations;

/// <summary>
/// Represents a pagination.
/// </summary>
/// <param name="Page">The (0-based) page number.</param>
/// <param name="PageSize">The number of items per page.</param>
public sealed record Pagination(int Page, int PageSize);