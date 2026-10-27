using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Olve.TypeSpec;

/// <summary>
/// Wires a generated API surface into the app. The generated code calls these from its own
/// <c>Add…Api</c> / <c>Use…Api</c>, passing what only it knows: its JSON context and handler types.
/// </summary>
public static class TypeSpecApiExtensions
{
    /// <summary>
    /// Registers the generated JSON source-gen context and makes binding failures throw, so
    /// <see cref="BindingFailureMiddleware"/> can answer them. The app registers its own
    /// <see cref="IApiErrorWriter"/>.
    /// </summary>
    public static IServiceCollection AddTypeSpecApi(this IServiceCollection services, JsonSerializerContext generatedContext)
    {
        services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, generatedContext);
            // Discriminated unions: the discriminator may appear anywhere in a request object.
            options.SerializerOptions.AllowOutOfOrderMetadataProperties = true;
        });
        services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = true);
        return services;
    }

    /// <summary>The handler interfaces among <paramref name="handlerTypes"/> that have no registered implementation.</summary>
    public static IReadOnlyList<Type> MissingHandlers(this IServiceProvider services, IReadOnlyList<Type> handlerTypes)
    {
        var isService = services.GetRequiredService<IServiceProviderIsService>();
        return [.. handlerTypes.Where(type => !isService.IsService(type))];
    }

    /// <summary>
    /// Checks every operation's handler and the <see cref="IApiErrorWriter"/> are registered, then
    /// adds the binding-failure middleware. Fails fast, so a new operation in the spec
    /// can't ship as a runtime 500. The generated <c>Map…Api</c> maps the routes afterwards.
    /// </summary>
    public static WebApplication UseTypeSpecApi(this WebApplication app, IReadOnlyList<Type> handlerTypes)
    {
        var missing = app.Services.MissingHandlers(handlerTypes);
        if (missing.Count > 0)
        {
            throw new InvalidOperationException("No handler registered for: " + string.Join(", ", missing.Select(type => type.Name)));
        }

        if (!app.Services.GetRequiredService<IServiceProviderIsService>().IsService(typeof(IApiErrorWriter)))
        {
            throw new InvalidOperationException($"No {nameof(IApiErrorWriter)} registered: the API needs one to answer binding and validation failures.");
        }

        app.UseBindingFailures();
        return app;
    }
}
