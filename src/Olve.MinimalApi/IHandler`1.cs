using Olve.Results;

namespace Olve.MinimalApi;

/// <summary>
/// Defines a handler for processing requests of type <typeparamref name="TRequest"/>.
/// </summary>
/// <typeparam name="TRequest">The type of request to handle.</typeparam>

public interface IHandler<in TRequest>
{
    /// <summary>
    /// Runs the handler asynchronously for the specified request.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="cancellationToken">Token to cancel operation.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    Task<Result> RunAsync(TRequest request, CancellationToken cancellationToken);
}
