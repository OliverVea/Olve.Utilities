using Microsoft.Extensions.DependencyInjection;

namespace Olve.Utilities.AsyncOnStartup;

public static class ServiceProviderExtensions
{
    public static async Task RunAsyncOnStartup(
        this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        var asyncOnStartups = serviceProvider.GetServices<IAsyncOnStartup>();
        
        var asyncOnStartupGroups = asyncOnStartups.GroupBy(x => x.Priority).OrderBy(x => x.Key);

        foreach (var asyncOnStartupGroup in asyncOnStartupGroups)
        {
            var tasks = asyncOnStartupGroup.Select(x => x.OnStartupAsync(cancellationToken)).ToArray();
            await Task.WhenAll(tasks);
        }
    }
}