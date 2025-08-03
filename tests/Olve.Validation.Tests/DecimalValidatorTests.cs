using Olve.Results.TUnit;
using Olve.Validation.Validators;

namespace Olve.Validation.Tests;

public class DecimalValidatorTests
{
    [Test]
    [Arguments(1.0, true)]
    [Arguments(-1.0, false)]
    public async Task IsPositive_Various(double value, bool expected)
    {
        var validator = new DecimalValidator<double>()
            .MustBePositive();
        var result = validator.Validate(value);

        await (expected
            ? Assert.That(result).Succeeded()
            : Assert.That(result).Failed());
    }

    [Test]
    [Arguments(0.0, true)]
    [Arguments(1.0, false)]
    public async Task IsZero_Various(double value, bool expected)
    {
        var validator = new DecimalValidator<double>()
            .MustBeZero();
        var result = validator.Validate(value);

        await (expected
            ? Assert.That(result).Succeeded()
            : Assert.That(result).Failed());
    }
}
