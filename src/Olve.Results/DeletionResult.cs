namespace Olve.Results;

/// <summary>
///     Represents the result of a deletion operation, which can succeed, fail due to not being found, or fail due to an error.
/// </summary>
[GenerateResult]
public readonly partial struct DeletionResult
{
    /// <summary>
    ///     Creates a deletion result representing success.
    /// </summary>
    [SuccessCase]
    public static partial DeletionResult Success();

    /// <summary>
    ///     Creates a deletion result representing failure due to the entity not being found.
    /// </summary>
    [GreyCase]
    public static partial DeletionResult NotFound();

    /// <summary>
    ///     Creates a deletion result representing failure due to an error.
    /// </summary>
    /// <param name="problems">The problems associated with the failure.</param>
    /// <returns>A failure result.</returns>
    [ErrorCase]
    public static partial DeletionResult Error(ResultProblemCollection problems);

    /// <summary>
    ///     Creates a deletion result representing failure due to an error.
    /// </summary>
    /// <param name="problems">The problems associated with the failure.</param>
    /// <returns>A failure result.</returns>
    public static DeletionResult Error(params IEnumerable<ResultProblem> problems) =>
        Error(new ResultProblemCollection(problems));

    /// <summary>
    ///     Gets a value indicating whether the deletion failed due to the entity not being found.
    /// </summary>
    public bool WasNotFound => IsNotFound;
}
