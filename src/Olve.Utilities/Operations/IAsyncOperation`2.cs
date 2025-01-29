using System.Diagnostics.CodeAnalysis;
using Olve.Utilities.Types.Results;

namespace Olve.Utilities.Operations;

/// <summary>
///     Represents an asynchronous operation that takes a request and returns a result.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
[SuppressMessage("Design", "MA0048:File name must match type name")]
public interface IAsyncOperation<in TRequest, TResult>
{
    /// <summary>
    ///     Executes the operation asynchronously.
    /// </summary>
    /// <param name="request">The request to execute the operation with.</param>
    /// <param name="ct">The cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<Result<TResult>> ExecuteAsync(TRequest request, CancellationToken ct = default);
}