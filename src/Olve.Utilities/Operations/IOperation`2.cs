namespace Olve.Utilities.Operations;

/// <summary>
/// Represents an operation that takes a request and returns a result.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface IOperation<in TRequest, out TResult>
{
    /// <summary>
    /// Executes the operation.
    /// </summary>
    /// <param name="request">The request to execute the operation with.</param>
    /// <returns>The result of the operation.</returns>
    TResult Execute(TRequest request);
}