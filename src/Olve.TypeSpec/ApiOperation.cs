namespace Olve.TypeSpec;

/// <summary>
/// One contract operation as the generated code describes it: its id, its success statuses
/// (usually one; e.g. <c>201, 202</c> for a create that may queue) and the error statuses it
/// declares. Attached to each generated endpoint as metadata, so middleware can answer in the
/// contract's terms (see <see cref="BindingFailureMiddleware"/>).
/// </summary>
/// <param name="OperationId">The operation id, as in the OpenAPI document.</param>
/// <param name="SuccessStatuses">The declared 2xx statuses, in contract order.</param>
/// <param name="ErrorStatuses">The declared error statuses.</param>
public sealed record ApiOperation(string OperationId, IReadOnlyList<int> SuccessStatuses, IReadOnlyList<int> ErrorStatuses)
{
    /// <summary>The first (for most operations, the only) declared success status.</summary>
    public int SuccessStatus => SuccessStatuses[0];

    /// <summary>Whether the contract declares <paramref name="status"/> for this operation.</summary>
    public bool Declares(int status) => SuccessStatuses.Contains(status) || ErrorStatuses.Contains(status);
}
