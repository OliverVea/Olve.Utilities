using Olve.Results;
using Olve.Results.TUnit;
using Olve.Validation.Validators;

namespace Olve.Validation.Tests;

public class StringValidatorTests
{
    [Test]
    [Arguments(null, false)]
    [Arguments("", false)]
    [Arguments(" ", true)]
    [Arguments("a", true)]
    public async Task IsNotNullOrEmpty_Tests(string? value, bool expectedSuccess)
    {
        var result = new StringValidator()
            .IsNotNullOrEmpty()
            .Validate(value);

        await (expectedSuccess
            ? Assert.That(result).Succeeded()
            : Assert.That(result).Failed());
    }

    [Test]
    [Arguments(null, false)]
    [Arguments("", false)]
    [Arguments(" ", false)]
    [Arguments("a", true)]
    public async Task IsNotNullOrWhiteSpace_Tests(string? value, bool expectedSuccess)
    {
        var result = new StringValidator()
            .IsNotNullOrWhiteSpace()
            .Validate(value);

        await (expectedSuccess
            ? Assert.That(result).Succeeded()
            : Assert.That(result).Failed());
    }

    [Test]
    public async Task WithProblem_ReplacesMessage()
    {
        var result = new StringValidator()
            .IsNotNullOrWhiteSpace()
            .WithProblem(_ => new ResultProblem("custom message"))
            .Validate(null);

        await Assert.That(result)
            .FailedAndProblemCollection()
            .HasSingleItem()
            .HasMember(x => x.Single().Message)
            .EqualTo("custom message");
    }

    [Test]
    [Arguments(0, null, true)]
    [Arguments(0, "", true)]
    [Arguments(0, "abc", true)]
    [Arguments(1, null, true)]
    [Arguments(1, "", false)]
    [Arguments(1, "a", true)]
    [Arguments(7, null, true)]
    [Arguments(7, "abcdefg", true)]
    [Arguments(7, "abcdef", false)]
    public async Task MinLength_Various(int minLength, string? value, bool expectedSuccess)
    {
        var validator = new StringValidator().MinLength(minLength);
        var result = validator.Validate(value);

        await (expectedSuccess
            ? Assert.That(result).Succeeded()
            : Assert.That(result).Failed());

        if (!expectedSuccess)
        {
            await Assert.That(result)
                .FailedAndProblemCollection()
                .HasSingleItem()
                .HasMember(x => x.Single().ToBriefString())
                .EqualTo($"Value must be at least '{minLength}' characters");
        }
    }

    [Test]
    [Arguments(0, null, true)]
    [Arguments(0, "", true)]
    [Arguments(0, "a", false)]
    [Arguments(1, null, true)]
    [Arguments(1, "a", true)]
    [Arguments(1, "ab", false)]
    [Arguments(7, null, true)]
    [Arguments(7, "abcdefg", true)]
    [Arguments(7, "abcdefgh", false)]
    public async Task MaxLength_Various(int maxLength, string? value, bool expectedSuccess)
    {
        var validator = new StringValidator().MaxLength(maxLength);
        var result = validator.Validate(value);

        await (expectedSuccess
            ? Assert.That(result).Succeeded()
            : Assert.That(result).Failed());

        if (!expectedSuccess)
        {
            await Assert.That(result)
                .FailedAndProblemCollection()
                .HasSingleItem()
                .HasMember(x => x.Single().ToBriefString())
                .EqualTo($"Value must be at most '{maxLength}' characters");
        }
    }

    [Test]
    public async Task IsNotNullOrEmpty_DefaultProblemMessage()
    {
        var result = new StringValidator().IsNotNullOrEmpty().Validate("");
        await Assert.That(result)
            .FailedAndProblemCollection()
            .HasSingleItem()
            .HasMember(x => x.Single().Message)
            .EqualTo("Value is null or empty");
    }

    [Test]
    public async Task IsNotNullOrWhiteSpace_DefaultProblemMessage()
    {
        var result = new StringValidator().IsNotNullOrWhiteSpace().Validate(" ");
        await Assert.That(result)
            .FailedAndProblemCollection()
            .HasSingleItem()
            .HasMember(x => x.Single().Message)
            .EqualTo("Value is null or white space");
    }

    [Test]
    [Arguments(null, false)]
    [Arguments("value", true)]
    public async Task IsNotNull_Various(string? value, bool expectedSuccess)
    {
        var result = new StringValidator().IsNotNull().Validate(value);
        await (expectedSuccess
            ? Assert.That(result).Succeeded()
            : Assert.That(result).Failed());
        if (!expectedSuccess)
        {
            await Assert.That(result)
                .FailedAndProblemCollection()
                .HasSingleItem()
                .HasMember(x => x.Single().Message)
                .EqualTo("Value was null");
        }
    }

    [Test]
    public async Task IsNotOneOf_Various()
    {
        string[] allowed = ["a", "b"];
        var validator = new StringValidator().IsNotOneOf(allowed);

        var success = validator.Validate("a");
        await Assert.That(success).Succeeded();

        var failure = validator.Validate("c");
        await Assert.That(failure)
            .FailedAndProblemCollection()
            .HasSingleItem()
            .HasMember(x => x.Single().ToBriefString())
            .EqualTo("Value was not one of the allowed values: [a, b]");
    }

    [Test]
    public async Task MinLength_WithProblemOverride()
    {
        var result = new StringValidator()
            .MinLength(3)
            .WithProblem(_ => new ResultProblem("Custom min"))
            .Validate("a");
        await Assert.That(result)
            .FailedAndProblemCollection()
            .HasSingleItem()
            .HasMember(x => x.Single().Message)
            .EqualTo("Custom min");
    }

    [Test]
    public async Task MaxLength_WithProblemOverride()
    {
        var result = new StringValidator()
            .MaxLength(2)
            .WithProblem(_ => new ResultProblem("Custom max"))
            .Validate("abc");
        await Assert.That(result)
            .FailedAndProblemCollection()
            .HasSingleItem()
            .HasMember(x => x.Single().ToBriefString())
            .EqualTo("Custom max");
    }

    [Test]
    public async Task Chaining_MultipleRules_CollectsAllProblems()
    {
        var result = new StringValidator()
            .IsNotNullOrWhiteSpace()
            .MinLength(2)
            .Validate("");
        await Assert.That(result)
            .FailedAndProblemCollection()
            .HasMember(x => x.Count())
            .EqualTo(2);
    }

    [Test]
    public async Task MinLength_Negative_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new StringValidator().MinLength(-1));
        await Assert.That(ex.ParamName).IsEqualTo("minLength");
        await Assert.That(ex.Message).StartsWith("minLength must be non-negative");
    }

    [Test]
    public async Task MaxLength_Negative_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new StringValidator().MaxLength(-1));
        await Assert.That(ex.ParamName).IsEqualTo("maxLength");
        await Assert.That(ex.Message).StartsWith("maxLength must be non-negative");
    }
}
