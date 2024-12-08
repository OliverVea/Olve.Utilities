namespace Olve.Operations.Operations;

public interface IAsyncOperation<in TRequest, TResult>
{
    Task<TResult> ExecuteAsync(TRequest request, CancellationToken ct = default);
}