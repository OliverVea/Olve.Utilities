namespace Olve.Utilities.Types.Results;

/// <summary>
///     Extension methods for working with <see cref="Result" />, <see cref="Result" />, and <see cref="Result{T}" />
///     collections.
/// </summary>
public static class ResultEnumerableExtensions
{
    /// <summary>
    ///     Determines whether any of the results in the collection indicate failure.
    /// </summary>
    /// <param name="results">The collection of <see cref="Result" /> objects.</param>
    /// <returns><c>true</c> if any result in the collection failed; otherwise, <c>false</c>.</returns>
    public static bool HasProblems(this IEnumerable<Result> results) => results.Any(r => !r.Succeeded);

    /// <summary>
    ///     Determines whether any of the results in the collection indicate failure.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="results">The collection of <see cref="Result{T}" /> objects.</param>
    /// <returns><c>true</c> if any result in the collection failed; otherwise, <c>false</c>.</returns>
    public static bool HasProblems<T>(this IEnumerable<Result<T>> results) => results.Any(r => !r.Succeeded);

    /// <summary>
    ///     Attempts to collect problems from the results in the collection.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="results">The collection of <see cref="Result{T}" /> objects.</param>
    /// <param name="problems">
    ///     When this method returns, contains a <see cref="ResultProblemCollection" /> that includes
    ///     all problems found in the failed results. If no problems were found, this will be an empty collection.
    /// </param>
    /// <returns>
    ///     <c>true</c> if any result in the collection had problems; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryPickProblems<T>(this IEnumerable<Result<T>> results, out ResultProblemCollection problems)
    {
        problems = new ResultProblemCollection();
        var hadProblems = false;

        foreach (var result in results)
        {
            if (!result.TryPickProblems(out var resultProblems))
            {
                continue;
            }

            problems = ResultProblemCollection.Merge(resultProblems, problems);
            hadProblems = true;
        }

        return hadProblems;
    }

    /// <summary>
    ///     Attempts to collect problems and successful values from the results in the collection.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="results">The collection of <see cref="Result{T}" /> objects.</param>
    /// <param name="problems">
    ///     When this method returns, contains a <see cref="ResultProblemCollection" /> that includes
    ///     all problems found in the failed results. If no problems were found, this will be an empty collection.
    /// </param>
    /// <param name="values">
    ///     When this method returns, contains a collection of successful result values. If no successful results were found,
    ///     this will be an empty collection.
    /// </param>
    /// <returns>
    ///     <c>true</c> if any result in the collection had problems; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryPickProblems<T>(this IEnumerable<Result<T>> results,
        out ResultProblemCollection problems,
        out IEnumerable<T> values)
    {
        List<T> valuesList = new();
        values = valuesList;

        problems = new ResultProblemCollection();
        var hadProblems = false;

        foreach (var result in results)
        {
            if (!result.TryPickProblems(out var resultProblems, out var value))
            {
                valuesList.Add(value);
                continue;
            }

            problems = ResultProblemCollection.Merge(resultProblems, problems);
            hadProblems = true;
        }

        return hadProblems;
    }


    /// <summary>
    ///     Retrieves all problems from the results in the collection.
    /// </summary>
    /// <param name="results">The collection of <see cref="Result" /> objects.</param>
    /// <returns>An <see cref="IEnumerable{T}" /> of <see cref="ResultProblem" /> objects from the failed results.</returns>
    public static IEnumerable<ResultProblem> GetProblems(this IEnumerable<Result> results) =>
        results.SelectMany(r => r.Problems ?? []);

    /// <summary>
    ///     Retrieves all problems from the results in the collection.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="results">The collection of <see cref="Result{T}" /> objects.</param>
    /// <returns>An <see cref="IEnumerable{T}" /> of <see cref="ResultProblem" /> objects from the failed results.</returns>
    public static IEnumerable<ResultProblem> GetProblems<T>(this IEnumerable<Result<T>> results) =>
        results.SelectMany(r => r.Problems ?? []);

    /// <summary>
    ///     Retrieves all successful result values from the collection.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="results">The collection of <see cref="Result{T}" /> objects.</param>
    /// <returns>An <see cref="IEnumerable{T}" /> of successful result values.</returns>
    public static IEnumerable<T> GetValues<T>(this IEnumerable<Result<T>> results) =>
        results
            .Select(r => r.Value)
            .OfType<T>();
}