using Olve.Results;
using Olve.Validation;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable MA0048

namespace Olve.Utilities.Builders;

public interface IBuilder<out T>
{
    T Build();
}

public static class BuilderExtensions
{
    public static Result<T> ValidateAndBuild<T, TValidator>(this IBuilder<T> builder, TValidator validator)
        where TValidator : IValidator<T>
    {
        var value = builder.Build();
        var validationResult = validator.Validate(value);
        if (validationResult.TryPickProblems(out var problems))
        {
            return problems;
        }

        return value;
    }
}