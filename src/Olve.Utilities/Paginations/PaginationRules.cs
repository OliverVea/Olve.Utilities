using Olve.Results;

namespace Olve.Utilities.Paginations;

/// <summary>
///     Validation and clamping shared by <see cref="Pagination" /> and <see cref="OffsetPagination" />.
/// </summary>
internal static class PaginationRules
{
    public static Result Validate(int position, string positionName, int size, string sizeName, int maxSize)
    {
        List<ResultProblem> problems = [];

        if (position < 0)
        {
            problems.Add(new ResultProblem("{0} must be at least 0, but was {1}", positionName, position));
        }

        if (size < 1 || size > maxSize)
        {
            problems.Add(new ResultProblem("{0} must be between 1 and {1}, but was {2}", sizeName, maxSize, size));
        }

        return problems.Count == 0 ? Result.Success() : Result.Failure(problems);
    }

    public static (int Position, int Size) Clamp(int position, int size, int defaultSize, int maxSize)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(maxSize, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(defaultSize, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(defaultSize, maxSize);

        var clampedPosition = Math.Max(position, 0);
        var clampedSize = size <= 0 ? defaultSize : Math.Min(size, maxSize);

        return (clampedPosition, clampedSize);
    }
}
