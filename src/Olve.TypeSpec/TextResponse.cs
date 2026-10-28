using System.Text;
using Microsoft.AspNetCore.Http;

namespace Olve.TypeSpec;

/// <summary>
/// Writes a generated response variant's text body (a <c>string</c> with a <c>text/*</c> content
/// type) as UTF-8, as is: not as a JSON string.
/// </summary>
public sealed class TextResponse(string text, string contentType, int status) : IResult
{
    /// <inheritdoc />
    public Task ExecuteAsync(HttpContext httpContext) =>
        TypedResults.Text(text, contentType, Encoding.UTF8, status).ExecuteAsync(httpContext);
}
