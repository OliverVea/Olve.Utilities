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
/// A bare 400 that the endpoint set without writing anything is answered the same way: the request
/// delegate generator (on under Native AOT) answers a failed parse of an optional parameter that
/// way instead of throwing.
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
            Logger(context).LogDebug(exception, "Request binding failed on {Path}", context.Request.Path);
            context.Response.Clear();
            await WriteAsync(context, exception.Message, exception.StatusCode);
            return;
        }

        var response = context.Response;
        if (response.StatusCode == StatusCodes.Status400BadRequest && !response.HasStarted
            && response.ContentLength is null && string.IsNullOrEmpty(response.ContentType))
        {
            Logger(context).LogDebug("Request binding failed on {Path} (bare 400)", context.Request.Path);
            await WriteAsync(context, "A request parameter could not be bound.", StatusCodes.Status400BadRequest);
        }
    }

    private static Task WriteAsync(HttpContext context, string message, int status)
    {
        var operation = context.GetEndpoint()?.Metadata.GetMetadata<ApiOperation>();
        var result = context.RequestServices.GetRequiredService<IApiErrorWriter>()
            .InvalidRequest([new ResultProblem(message)], status, operation);
        return result.ExecuteAsync(context);
    }

    private static ILogger Logger(HttpContext context) =>
        context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(BindingFailureMiddleware).FullName!);
}
