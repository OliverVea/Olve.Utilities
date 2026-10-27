using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Olve.Results;

namespace Olve.TypeSpec;

/// <summary>
/// Answers request-binding failures (a <see cref="BadHttpRequestException"/>: a malformed or
/// incomplete JSON body, <c>limit=abc</c>, a missing required parameter) through the API's
/// <see cref="IApiErrorWriter"/>, with the matched <see cref="ApiOperation"/> so the writer can
/// answer in the contract's terms. Requires <c>RouteHandlerOptions.ThrowOnBadRequest</c>
/// (<see cref="TypeSpecApiExtensions.AddTypeSpecApi"/> sets it) so the failures reach the middleware.
/// </summary>
public static class BindingFailureMiddleware
{
    /// <summary>Adds the middleware (reflection-free, so it suits Native AOT).</summary>
    public static IApplicationBuilder UseBindingFailures(this IApplicationBuilder app) => app.Use(InvokeAsync);

    private static async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (BadHttpRequestException exception) when (!context.Response.HasStarted)
        {
            var services = context.RequestServices;
            services.GetRequiredService<ILoggerFactory>()
                .CreateLogger(typeof(BindingFailureMiddleware).FullName!)
                .LogDebug(exception, "Request binding failed on {Path}", context.Request.Path);

            var operation = context.GetEndpoint()?.Metadata.GetMetadata<ApiOperation>();
            context.Response.Clear();
            var result = services.GetRequiredService<IApiErrorWriter>()
                .InvalidRequest([new ResultProblem(exception.Message)], exception.StatusCode, operation);
            await result.ExecuteAsync(context);
        }
    }
}
