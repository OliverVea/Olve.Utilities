namespace Olve.Results;

/// <summary>
///     Extension methods for <see cref="DeletionResult"/>.
/// </summary>
public static class DeletionResultExtensions
{
    /// <summary>
    ///     Explicitly discards a <see cref="DeletionResult"/>, signalling that its outcome is
    ///     intentionally not observed, e.g. <c>(await DeleteUser(id)).DiscardResult();</c>.
    /// </summary>
    /// <param name="result">The deletion result to discard.</param>
    public static void DiscardResult(this DeletionResult result) => _ = result;

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
