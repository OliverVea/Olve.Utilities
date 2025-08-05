using System;
using Olve.Results;
using Olve.Results.TUnit;
using Olve.Validation.Validators.Base;

namespace Olve.Validation.Tests;

public class BaseStructValidatorTests
{
    private class TestIntValidator : BaseStructValidator<int, TestIntValidator>
    {
        protected override TestIntValidator Validator => this;
        public TestIntValidator NotDefault() => CannotBeDefault();
    }

    [Test]
    [Arguments(0, false)]
    [Arguments(1, true)]
    public async Task IsNotDefault_Various(int value, bool expectedSuccess)
    {
        var result = new TestIntValidator()
            .NotDefault()
            .Validate(value);

        await (expectedSuccess
            ? Assert.That(result).Succeeded()
            : Assert.That(result).Failed());
    }

    [Test]
    public void WithProblem_NoPrevious_Throws()
    {
        var validator = new TestIntValidator();
        Assert.Throws<InvalidOperationException>(() => validator.WithProblem(_ => new ResultProblem("msg")));
    }

    [Test]
    public void WithProblem_AlreadyOverrode_Throws()
    {
        var validator = new TestIntValidator()
            .NotDefault()
            .WithMessage("first");
        Assert.Throws<InvalidOperationException>(() => validator.WithProblem(_ => new ResultProblem("msg")));
    }

    [Test]
    public async Task WithProblem_HappyPath_OverridesProblem()
    {
        var result = new TestIntValidator()
            .NotDefault()
            .WithProblem(_ => new ResultProblem("custom"))
            .Validate(0);

        await Assert.That(result)
            .FailedAndProblemCollection()
            .HasSingleItem()
            .HasMember(x => x.Single().Message)
            .EqualTo("custom");
    }

    [Test]
    public async Task WithMessage_HappyPath_OverridesMessage()
    {
        var result = new TestIntValidator()
            .NotDefault()
            .WithMessage("custom msg")
            .Validate(0);

        await Assert.That(result)
            .FailedAndProblemCollection()
            .HasSingleItem()
            .HasMember(x => x.Single().Message)
            .EqualTo("custom msg");
    }
}
