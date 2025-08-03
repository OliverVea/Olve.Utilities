using Olve.Results;

namespace Olve.MinimalApi;

/// <summary>
/// Defines a handler for processing requests of type <typeparamref name="TRequest"/> returning response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TRequest">The type of request to handle.</typeparam>
/// <typeparam name="TResponse">The type of response returned by the handler.</typeparam>
public interface IHandler<in TRequest, TResponse>
{
    /// <summary>
    /// Handles the request asynchronously and returns a <see cref="Result{TResponse}"/> containing the response or errors.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="cancellationToken">Token to cancel operation.</param>
    /// <returns>A <see cref="Result{TResponse}"/> with the operation result or failures.</returns>
    Task<Result<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken);
}
