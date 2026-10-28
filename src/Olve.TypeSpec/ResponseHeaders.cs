using Microsoft.AspNetCore.Http;

namespace Olve.TypeSpec;

/// <summary>
/// Sets a generated response variant's declared headers (its <c>@header</c> properties), then
/// writes the variant's body. A null value (an optional header the handler left unset) is not sent.
/// </summary>
public sealed class ResponseHeaders(IResult inner, IReadOnlyList<KeyValuePair<string, string?>> headers) : IResult
{
    /// <inheritdoc />
    public Task ExecuteAsync(HttpContext httpContext)
    {
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
