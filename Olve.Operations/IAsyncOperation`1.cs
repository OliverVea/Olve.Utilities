namespace Olve.Operations;

public interface IAsyncOperation<in TInput>
{
    Task ExecuteAsync(TInput input, CancellationToken ct = default);
}