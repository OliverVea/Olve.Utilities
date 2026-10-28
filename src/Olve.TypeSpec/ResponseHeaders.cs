using Microsoft.AspNetCore.Http;

namespace Olve.TypeSpec;

/// <summary>
/// Sets a generated response variant's declared headers (its <c>@header</c> properties), then
/// writes the variant's body. The body is registered for disposal first, so a header value the
/// server refuses can't leak its stream. A null value (an optional header left unset) is not sent.
/// </summary>
public sealed class ResponseHeaders(IResult inner, IReadOnlyList<KeyValuePair<string, string?>> headers) : IResult
{
    /// <inheritdoc />
    public Task ExecuteAsync(HttpContext httpContext)
    {
        // A header value the server refuses throws here, before the body owns its stream.
        if (inner is IAsyncDisposable disposable)
        {
            httpContext.Response.RegisterForDisposeAsync(disposable);
        }

        foreach (var (name, value) in headers)
        {
            if (value is not null)
            {
                httpContext.Response.Headers[name] = value;
            }
        }

        return inner.ExecuteAsync(httpContext);
    }
}
