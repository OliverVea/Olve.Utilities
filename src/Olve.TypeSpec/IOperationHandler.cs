namespace Olve.TypeSpec;

/// <summary>
/// Implements one contract operation: takes its generated request record and returns one of the
/// responses the contract declares for it (the generated per-operation response union), so an
/// undeclared status can't compile. Expected failures are declared responses, not exceptions.
/// </summary>
/// <typeparam name="TRequest">The operation's generated request record.</typeparam>
/// <typeparam name="TResponse">The operation's generated response union.</typeparam>
public interface IOperationHandler<in TRequest, TResponse>
    where TResponse : IOperationResponse
{
    /// <summary>Handles one request.</summary>
    Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
}
