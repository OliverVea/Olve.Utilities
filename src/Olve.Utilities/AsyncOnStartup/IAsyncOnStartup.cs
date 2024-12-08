namespace Olve.Operations.AsyncOnStartup;

public interface IAsyncOnStartup
{
    /// <summary>
    /// Priority of the startup task. Lower values run first.
    /// </summary>
    int Priority => 0;
    
    Task OnStartupAsync(CancellationToken cancellationToken = default);
}