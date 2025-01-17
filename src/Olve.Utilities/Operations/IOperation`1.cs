using Olve.Utilities.Types.Results;

namespace Olve.Utilities.Operations;

/// <summary>
///     Represents an operation that can be executed.
/// </summary>
/// <typeparam name="TInput">The type of the input to the operation.</typeparam>
public interface IOperation<in TInput>
{
    /// <summary>
    ///     Executes the operation.
    /// </summary>
    /// <param name="input">The input to the operation.</param>
    Result Execute(TInput input);
}