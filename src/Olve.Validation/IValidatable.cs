using Olve.Results;

namespace Olve.Validation;

/// <summary>
/// Defines a contract for an object that can validate its own state or data.
/// </summary>
public interface IValidatable
{
    /// <summary>
    /// Performs validation on this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="Result"/> indicating whether validation succeeded,
    /// and, if it failed, containing details of any validation errors.
    /// </returns>
    Result Validate();
}