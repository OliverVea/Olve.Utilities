using System.Diagnostics.CodeAnalysis;
using Olve.Results;

namespace Olve.Operations;

/// <summary>
///     Represents an asynchronous operation that takes an input.
/// </summary>
/// <typeparam name="TRequest">The type of the input.</typeparam>
[SuppressMessage("Design", "MA0048:File name must match type name")]
public interface IAsyncOperation<in TRequest>
{
    /// <summary>
    ///     Executes the operation asynchronously.
    /// </summary>
    /// <param name="request">The input to the operation.</param>
    /// <param name="ct">The cancellation token to cancel the operation.</param>
    /// <returns>The result of the operation.</returns>
    Task<Result> ExecuteAsync(TRequest request, CancellationToken ct = default);
}