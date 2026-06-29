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
}
