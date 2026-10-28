using Microsoft.AspNetCore.Http;

namespace Olve.TypeSpec;

/// <summary>
/// Writes a generated response variant's binary body (<c>bytes</c> with a non-JSON content type)
/// by copying <paramref name="content"/> to the response as it is read, so large files are never
/// buffered. The stream is disposed once written. A declared <c>Content-Length</c> header sets the length.
/// </summary>
public sealed class StreamResponse(Stream content, string contentType, int status) : IResult
{
    /// <inheritdoc />
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        await using (content)
        {
            httpContext.Response.StatusCode = status;
            httpContext.Response.ContentType = contentType;
            await content.CopyToAsync(httpContext.Response.Body, httpContext.RequestAborted);
        }
    }
}
