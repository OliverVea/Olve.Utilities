using System.Diagnostics.CodeAnalysis;
using Olve.Utilities.Types.Results;

namespace Olve.Utilities.Operations;

/// <summary>
///     Represents an asynchronous operation that takes an input.
/// </summary>
/// <typeparam name="TInput">The type of the input.</typeparam>
[SuppressMessage("Design", "MA0048:File name must match type name")]
public interface IAsyncOperation<in TInput>
{
    /// <summary>
    ///     Executes the operation asynchronously.
    /// </summary>
    /// <param name="input">The input to the operation.</param>
    /// <param name="ct">The cancellation token to cancel the operation.</param>
    /// <returns>The result of the operation.</returns>
    Task<Result> ExecuteAsync(TInput input, CancellationToken ct = default);
}