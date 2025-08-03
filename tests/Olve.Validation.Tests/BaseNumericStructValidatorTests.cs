using Olve.Results.TUnit;
using Olve.Validation.Validators;

namespace Olve.Validation.Tests;

public class BaseNumericStructValidatorTests
{
    [Test]
    [Arguments(5, 2, true)]
    [Arguments(1, 2, false)]
    public async Task IsGreaterThan_Various(int value, int limit, bool expected)
    {
        var result = new IntValidator()
            .MustBeGreaterThan(limit)
            .Validate(value);

        await (expected
            ? Assert.That(result).Succeeded()
            : Assert.That(result).Failed());
    }

    [Test]
    [Arguments(0, 0, true)]
    [Arguments(1, 0, false)]
    public async Task IsLessThanOrEqualTo_Various(int value, int limit, bool expected)
    {
        var result = new IntValidator()
            .MustBeLessThanOrEqualTo(limit)
            .Validate(value);

        await (expected
            ? Assert.That(result).Succeeded()
            : Assert.That(result).Failed());
    }

    [Test]
    [Arguments(5, 1, 10, true)]
    [Arguments(0, 1, 10, false)]
    [Arguments(11, 1, 10, false)]
    public async Task IsBetween_Various(int value, int min, int max, bool expected)
    {
        var result = new IntValidator()
            .MustBeBetween(min, max)
            .Validate(value);

        await (expected
            ? Assert.That(result).Succeeded()
            : Assert.That(result).Failed());
    }
}
