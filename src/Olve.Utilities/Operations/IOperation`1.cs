using Olve.Utilities.Types.Results;

namespace Olve.Utilities.Operations;

/// <summary>
///     Represents an operation that can be executed.
/// </summary>
/// <typeparam name="TRequest">The type of the input to the operation.</typeparam>
public interface IOperation<in TRequest>
{
    /// <summary>
    ///     Executes the operation.
    /// </summary>
    /// <param name="request">The input to the operation.</param>
    Result Execute(TRequest request);
}