namespace Olve.Results;

/// <summary>
///     Extension methods for <see cref="DeletionResult"/>.
/// </summary>
public static class DeletionResultExtensions
{
    /// <summary>
    ///     Exhaustively matches over the three states of a <see cref="DeletionResult"/>.
    /// </summary>
    /// <param name="result">The deletion result to match.</param>
    /// <param name="onSuccess">Called when the deletion succeeded.</param>
    /// <param name="onNotFound">Called when the entity was not found.</param>
    /// <param name="onProblems">Called when the deletion failed with problems.</param>
    /// <typeparam name="T">The return type.</typeparam>
    /// <returns>The value produced by the matched handler.</returns>
    public static T Match<T>(
        this DeletionResult result,
        Func<T> onSuccess,
        Func<T> onNotFound,
        Func<ResultProblemCollection, T> onProblems)
    {
        if (result.Problems is { } problems)
        {
            return onProblems(problems);
        }

        if (result.Succeeded)
        {
            return onSuccess();
        }

        if (result.WasNotFound)
        {
            return onNotFound();
        }

        ResultProblemCollection resultProblems = new(new ResultProblem("Deletion result was invalid"));
        return onProblems(resultProblems);
    }

    /// <summary>
    ///     Converts a <see cref="DeletionResult"/> to a <see cref="Result"/>,
    ///     optionally treating not-found as success.
    /// </summary>
    /// <param name="result">The deletion result to convert.</param>
    /// <param name="allowNotFound">
    ///     If <see langword="true"/>, not-found is treated as success.
    ///     If <see langword="false"/>, not-found is treated as failure.
    /// </param>
    /// <returns>A <see cref="Result"/> representing the outcome.</returns>
    public static Result MapToResult(this DeletionResult result, bool allowNotFound = true)
    {
        if (result.Problems is { } problems)
        {
            return problems;
        }

        if (result.Succeeded)
        {
            return Result.Success();
        }

        if (result.WasNotFound)
        {
            return allowNotFound
                ? Result.Success()
                : new ResultProblem("Did not find required entity to delete");
        }

        return new ResultProblemCollection(new ResultProblem("Deletion result was invalid"));
    }
}
