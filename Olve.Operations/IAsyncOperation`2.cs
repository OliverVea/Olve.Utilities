namespace Olve.Operations;

public interface IAsyncOperation<in TRequest, TResult>
{
    Task<TResult> ExecuteAsync(TRequest request, CancellationToken ct = default);
}