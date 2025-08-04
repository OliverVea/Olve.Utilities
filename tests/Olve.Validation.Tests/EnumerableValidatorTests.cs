using Olve.Results.TUnit;
using Olve.Validation.Validators;

namespace Olve.Validation.Tests
{
    public class EnumerableValidatorTests
    {
        [Test]
        public async Task CannotBeEmpty_Tests()
        {
            var validator = new EnumerableValidator<int>().CannotBeEmpty();

            // Null is considered valid
            var resultNull = validator.Validate(null);
            await Assert.That(resultNull).Succeeded();

            // Empty enumerable fails
            var resultEmpty = validator.Validate([]);
            await Assert.That(resultEmpty).Failed();

            // Non-empty enumerable succeeds
            var resultNonEmpty = validator.Validate([1]);
            await Assert.That(resultNonEmpty).Succeeded();
        }

        [Test]
        public async Task CannotContainDuplicates_Tests()
        {
            var validator = new EnumerableValidator<int>().CannotContainDuplicates();

            // No duplicates succeeds
            var resultUnique = validator.Validate([1, 2, 3]);
            await Assert.That(resultUnique).Succeeded();

            // Duplicates fails
            var resultDuplicate = validator.Validate([1, 2, 2, 3]);
            await Assert.That(resultDuplicate).Failed();
        }

        [Test]
        public async Task MustHaveCountGreaterThan_Tests()
        {
            var validator = new EnumerableValidator<int>().MustHaveCountGreaterThan(2);

            // Count <= threshold fails
            var resultTooFew = validator.Validate([1, 2]);
            await Assert.That(resultTooFew).Failed();

            // Count > threshold succeeds
            var resultEnough = validator.Validate([1, 2, 3]);
            await Assert.That(resultEnough).Succeeded();
        }

        [Test]
        public async Task ListValidator_CannotBeEmpty_Tests()
        {
            var validator = new ListValidator<string>().CannotBeEmpty();

            // Null is considered valid
            var resultNull = validator.Validate(null);
            await Assert.That(resultNull).Succeeded();

            // Empty list fails
            var resultEmpty = validator.Validate([]);
            await Assert.That(resultEmpty).Failed();

            // Non-empty list succeeds
            var resultNonEmpty = validator.Validate(["a"]);
            await Assert.That(resultNonEmpty).Succeeded();
        }
    }
}
