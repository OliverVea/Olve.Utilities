using Microsoft.Extensions.DependencyInjection;

namespace Olve.Utilities.AsyncOnStartup;

/// <summary>
///     Extensions for <see cref="IServiceProvider" /> to run <see cref="IAsyncOnStartup" /> services.
/// </summary>
public static class ServiceProviderExtensions
{
    /// <summary>
    ///     Runs all <see cref="IAsyncOnStartup" /> services.
    /// </summary>
    /// <param name="serviceProvider">The service provider to get the services from.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the tasks.</param>
    /// <remarks>Will run in order of <see cref="IAsyncOnStartup.Priority" />.</remarks>
    public static async Task RunAsyncOnStartup(
        this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        var asyncOnStartups = serviceProvider.GetServices<IAsyncOnStartup>();

        var asyncOnStartupGroups = asyncOnStartups
            .GroupBy(x => x.Priority)
            .OrderBy(x => x.Key);

        foreach (var asyncOnStartupGroup in asyncOnStartupGroups)
        {
            var tasks = asyncOnStartupGroup
                .Select(x => x.OnStartupAsync(cancellationToken))
                .ToArray();
            await Task.WhenAll(tasks);
        }
    }
}