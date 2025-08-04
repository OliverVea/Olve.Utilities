using Olve.Results;
using Olve.Results.TUnit;

namespace Olve.Validation.Tests
{
    public class InterfaceTests
    {
        private class DummyValidatable : IValidatable
        {
            private readonly bool _shouldFail;
            public DummyValidatable(bool shouldFail) => _shouldFail = shouldFail;
            public Result Validate() => _shouldFail 
                ? new ResultProblem("Validation failed") 
                : Result.Success();
        }

        [Test]
        [Arguments(false)]
        [Arguments(true)]
        public async Task IValidatable_ReturnsExpectedResult(bool shouldFail)
        {
            var instance = new DummyValidatable(shouldFail);
            var result = instance.Validate();
            await (shouldFail
                ? Assert.That(result).Failed()
                : Assert.That(result).Succeeded());
        }

        private class DummyValidator : IValidator<int>
        {
            public Result Validate(int value) => value > 0 
                ? Result.Success() 
                : new ResultProblem("Value must be positive");
        }

        [Test]
        [Arguments(1, true)]
        [Arguments(0, false)]
        public async Task IValidator_ReturnsExpectedResult(int value, bool expected)
        {
            var validator = new DummyValidator();
            var result = validator.Validate(value);
            await (expected
                ? Assert.That(result).Succeeded()
                : Assert.That(result).Failed());
        }
    }
}
