using Microsoft.AspNetCore.Http;
using Olve.Results;

namespace Olve.TypeSpec;

/// <summary>
/// Writes the errors the runtime itself answers with, before a handler runs: a request-binding
/// failure (malformed JSON, <c>limit=abc</c>, a missing required parameter) or a failed request
/// validation. The service registers one implementation, so every API keeps its own error body;
/// a typical implementation answers the body only when the matched operation declares
/// the status (<see cref="ApiOperation.Declares"/>) and the bare status otherwise.
/// </summary>
public interface IApiErrorWriter
{
    /// <summary>
    /// Answers an invalid request with <paramref name="status"/> (400 unless the framework says
    /// otherwise), explaining it with <paramref name="problems"/>.
    /// </summary>
    /// <param name="problems">What is wrong; never empty.</param>
    /// <param name="status">The status to answer with.</param>
    /// <param name="operation">The matched operation, or null when the request matched none.</param>
    IResult InvalidRequest(IReadOnlyList<ResultProblem> problems, int status, ApiOperation? operation);
}
