using BuilderGenerator;
using Olve.Results;
using Olve.Utilities.Builders;
using Olve.Validation;
using Olve.Validation.Validators;

namespace Olve.Utilities.Tests.Builders;

public class Dto
{
    public string Name { get; init; } = string.Empty;
    public int Age { get; init; }
}

[BuilderFor(typeof(Dto))]
public partial class DtoBuilder : IBuilder<Dto>;

public class DtoValidator : IValidator<Dto>
{
    private readonly IValidator<string> _nameValidator = GetNameValidator();
    private readonly IValidator<int> _ageValidator = GetAgeValidator();
    
    private static IValidator<string> GetNameValidator() => new StringValidator();
    
    private static IValidator<int> GetAgeValidator() => new IntValidator()
        .IsPositive()
        .IsGreaterThanOrEqualTo(18)
        .WithProblem(_ => new ResultProblem("Age must be at least 18."));
    
    public Result Validate(Dto value)
    {
        return Result.Concat(
            _nameValidator.Validate(value.Name),
            _ageValidator.Validate(value.Age));
    }
}

public class BuilderExtensionsTest
{
    [Test]
    public async Task Test()
    {
        DtoBuilder builder = new();
        DtoValidator validator = new();
        
        var result = builder.ValidateAndBuild(validator);

        await Assert.That(result.Failed).IsTrue();
    }

}