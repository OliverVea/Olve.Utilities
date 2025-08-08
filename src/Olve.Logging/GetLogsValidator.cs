using Olve.Results;
using Olve.Validation;
using Olve.Validation.Validators;

namespace Olve.Logging;

/// <summary>
/// Validator for <see cref="GetLogsRequest"/> values.
/// Ensures the request parameters meet basic constraints.
/// </summary>
public class GetLogsValidator : IValidator<GetLogsRequest>
{
    private static readonly IntValidator CountValidator = new IntValidator()
        .MustBePositive()
        .WithMessage("Count must be positive");
    
    /// <summary>
    /// Validate the provided <see cref="GetLogsRequest"/>.
    /// </summary>
    /// <param name="value">The request to validate.</param>
    /// <returns>A <see cref="Result"/> which is successful when validation passes, otherwise contains problems.</returns>
    public Result Validate(GetLogsRequest value)
    {
        return Result.Concat(CountValidator.Validate(value.Count));
    }
}
