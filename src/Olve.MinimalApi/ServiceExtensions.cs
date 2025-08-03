using Microsoft.Extensions.DependencyInjection;
using Olve.Paths;

namespace Olve.MinimalApi;

/// <summary>
/// Extension methods to configure minimal API services including JSON conversion for <see cref="IPath"/>.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Configures JSON options to use <see cref="PathJsonConverter"/> for <see cref="IPath"/>.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The configured <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection WithPathJsonConversion(this IServiceCollection services)
    {
        return services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new PathJsonConverter());
        });
    }
}
