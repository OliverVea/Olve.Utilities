using System.Text.Json.Serialization;
using Olve.Results;

namespace Olve.TypeSpec.Conformance;

/// <summary>
/// The fixture's error body, <c>{ "error": { "code", "message", "details": { "problems"? } } }</c>:
/// the contract's <c>ErrorEnvelope</c> model maps onto it (<c>external-types</c>). An app brings
/// its own error type the same way; the runtime knows errors only through <see cref="IApiErrorWriter"/>.
/// </summary>
public sealed record FixtureErrorEnvelope([property: JsonPropertyName("error")] FixtureError Error)
{
    public static implicit operator FixtureErrorEnvelope(FixtureError error) => new(error);
}

public sealed record FixtureError(
    [property: JsonPropertyName("code")] string Code,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("details")] FixtureErrorDetails Details)
{
    public static FixtureError Create(string code, string message) => new(code, message, new FixtureErrorDetails());
}

public sealed record FixtureErrorDetails
{
    [JsonPropertyName("problems")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<FixtureErrorProblem>? Problems { get; init; }
}

public sealed record FixtureErrorProblem(
    [property: JsonPropertyName("code")] string Code,
    [property: JsonPropertyName("message")] string Message);

[JsonSerializable(typeof(FixtureErrorEnvelope))]
internal sealed partial class FixtureErrorJsonContext : JsonSerializerContext;

public static class FixtureErrors
{
    public const string InvalidRequest = "INVALID_REQUEST";
}

/// <summary>
/// Answers binding and validation failures with the envelope when the operation declares the
/// status, else with the status alone: the first problem is the message, every problem goes in
/// <c>details.problems</c> when there are several.
/// </summary>
public sealed class FixtureErrorWriter : IApiErrorWriter
{
    public IResult InvalidRequest(IReadOnlyList<ResultProblem> problems, int status, ApiOperation? operation)
    {
        if (operation?.Declares(status) != true)
        {
            return TypedResults.StatusCode(status);
        }

        var entries = problems.Select(p => new FixtureErrorProblem(FixtureErrors.InvalidRequest, p.FormattedMessage)).ToList();
        var error = new FixtureError(FixtureErrors.InvalidRequest, entries[0].Message, new FixtureErrorDetails { Problems = entries.Count > 1 ? entries : null });
        return TypedResults.Json(new FixtureErrorEnvelope(error), FixtureErrorJsonContext.Default.FixtureErrorEnvelope, statusCode: status);
    }
}
