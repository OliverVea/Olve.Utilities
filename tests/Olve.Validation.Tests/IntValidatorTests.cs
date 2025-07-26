using Olve.Results;
using Olve.Results.TUnit;

namespace Olve.Validation.Tests;

public class IntValidatorTests
{
    [Test]
    [Arguments(2, true)]
    [Arguments(3, false)]
    public async Task IsEven_Various(int value, bool expected)
    {
        var result = new IntValidator()
            .IsEven()
            .Validate(value);

        await (expected
            ? Assert.That(result).Succeeded()
            : Assert.That(result).Failed());

        if (!expected)
        {
            await Assert.That(result)
                .FailedAndProblemCollection()
                .HasSingleItem()
                .HasMember(x => x.Single().Message)
                .EqualTo("Value must be even");
        }
    }

    [Test]
    [Arguments(1, true)]
    [Arguments(2, false)]
    public async Task IsOdd_Various(int value, bool expected)
    {
        var result = new IntValidator()
            .IsOdd()
            .Validate(value);

        await (expected
            ? Assert.That(result).Succeeded()
            : Assert.That(result).Failed());

        if (!expected)
        {
            await Assert.That(result)
                .FailedAndProblemCollection()
                .HasSingleItem()
                .HasMember(x => x.Single().Message)
                .EqualTo("Value must be odd");
        }
    }
}
