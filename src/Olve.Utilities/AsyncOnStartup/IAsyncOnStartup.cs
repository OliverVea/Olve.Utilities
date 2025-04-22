namespace Olve.Utilities.AsyncOnStartup;

/// <summary>
///     Represents a task that runs on startup.
/// </summary>
public interface IAsyncOnStartup
{
    /// <summary>
    ///     Priority of the startup task. Lower values run first.
    /// </summary>
    int Priority => 0;

    /// <summary>
    ///     The method to be called on startup.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the task.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task OnStartupAsync(CancellationToken cancellationToken = default);
}
