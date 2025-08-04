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
}
