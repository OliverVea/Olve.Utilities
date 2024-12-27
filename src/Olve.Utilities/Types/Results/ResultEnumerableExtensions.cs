namespace Olve.Utilities.Types.Results;

/// <summary>
/// Extension methods for working with <see cref="IResult"/>, <see cref="Result"/>, and <see cref="Result{T}"/> collections.
/// </summary>
public static class ResultEnumerableExtensions
{
    /// <summary>
    /// Determines whether any of the results in the collection indicate failure.
    /// </summary>
    /// <param name="results">The collection of <see cref="IResult"/> objects.</param>
    /// <returns><c>true</c> if any result in the collection failed; otherwise, <c>false</c>.</returns>
    public static bool HasProblems(this IEnumerable<IResult> results) => results.Any(r => !r.Succeded);

    /// <summary>
    /// Determines whether any of the results in the collection indicate failure.
    /// </summary>
    /// <param name="results">The collection of <see cref="Result"/> objects.</param>
    /// <returns><c>true</c> if any result in the collection failed; otherwise, <c>false</c>.</returns>
    public static bool HasProblems(this IEnumerable<Result> results) => results.Any(r => !r.Succeded);

    /// <summary>
    /// Determines whether any of the results in the collection indicate failure.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="results">The collection of <see cref="Result{T}"/> objects.</param>
    /// <returns><c>true</c> if any result in the collection failed; otherwise, <c>false</c>.</returns>
    public static bool HasProblems<T>(this IEnumerable<Result<T>> results) => results.Any(r => !r.Succeded);

    /// <summary>
    /// Retrieves all problems from the results in the collection.
    /// </summary>
    /// <param name="results">The collection of <see cref="IResult"/> objects.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ResultProblem"/> objects from the failed results.</returns>
    public static IEnumerable<ResultProblem> GetProblems(this IEnumerable<IResult> results) =>
        results.SelectMany(r => r.Problems ?? [ ]);

    /// <summary>
    /// Retrieves all problems from the results in the collection.
    /// </summary>
    /// <param name="results">The collection of <see cref="Result"/> objects.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ResultProblem"/> objects from the failed results.</returns>
    public static IEnumerable<ResultProblem> GetProblems(this IEnumerable<Result> results) =>
        results.SelectMany(r => r.Problems ?? [ ]);

    /// <summary>
    /// Retrieves all problems from the results in the collection.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="results">The collection of <see cref="Result{T}"/> objects.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ResultProblem"/> objects from the failed results.</returns>
    public static IEnumerable<ResultProblem> GetProblems<T>(this IEnumerable<Result<T>> results) =>
        results.SelectMany(r => r.Problems ?? [ ]);

    /// <summary>
    /// Retrieves all successful result values from the collection.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="results">The collection of <see cref="Result{T}"/> objects.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> of successful result values.</returns>
    public static IEnumerable<T> GetValues<T>(this IEnumerable<Result<T>> results) =>
        results
            .Select(r => r.Value)
            .OfType<T>();
}
