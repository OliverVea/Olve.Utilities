using Olve.Results.TUnit;
using Olve.Validation.Validators;

namespace Olve.Validation.Tests;

public class IntValidatorTests
{
    [Test]
    [Arguments(2, true)]
    [Arguments(3, false)]
    public async Task IsEven_Various(int value, bool expected)
    {
        var result = new IntValidator()
            .MustBeEven()
            .Validate(value);

        await (expected
            ? Assert.That(result).Succeeded()
            : Assert.That(result).Failed());

        if (!expected)
        {
            await Assert.That(result)
                .FailedAndProblemCollection(y => y.Satisfies(z =>
                    z?.Count() == 1
                    && z.Single().Message == "Value must be even"));
        }
    }

    [Test]
    [Arguments(1, true)]
    [Arguments(2, false)]
    public async Task IsOdd_Various(int value, bool expected)
    {
        var result = new IntValidator()
            .MustBeOdd()
            .Validate(value);

        await (expected
            ? Assert.That(result).Succeeded()
            : Assert.That(result).Failed());

        if (!expected)
        {
            await Assert.That(result)
                .FailedAndProblemCollection(y => y.Satisfies(z =>
                    z?.Count() == 1
                    && z.Single().Message == "Value must be odd"));
        }
    }
}
