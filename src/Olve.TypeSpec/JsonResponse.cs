using System.Text.Json.Serialization.Metadata;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Olve.TypeSpec;

/// <summary>
/// Writes a generated response variant's body as JSON with the given status, through the app's
/// <see cref="JsonOptions"/> (so the generated source-gen context serializes it). Resolving the
/// type info there, rather than through <c>TypedResults.Json</c>, keeps the generated code free of
/// reflection warnings under Native AOT.
/// </summary>
public sealed class JsonResponse<T>(T value, int status) : IResult
{
    /// <inheritdoc />
    public Task ExecuteAsync(HttpContext httpContext)
    {
        var options = httpContext.RequestServices.GetRequiredService<IOptions<JsonOptions>>().Value.SerializerOptions;
        var typeInfo = (JsonTypeInfo<T>)options.GetTypeInfo(typeof(T));
        httpContext.Response.StatusCode = status;
        return httpContext.Response.WriteAsJsonAsync(value, typeInfo, cancellationToken: httpContext.RequestAborted);
    }
}
