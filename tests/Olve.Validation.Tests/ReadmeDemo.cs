using Olve.Results;
using Olve.Results.TUnit;
using Olve.Validation.Validators;

namespace Olve.Validation.Tests;

public class ReadmeDemo
{
    [Test]
    public async Task StringValidation()
    {
        var result = new StringValidator()
            .CannotBeNullOrWhiteSpace()
            .MustHaveMinLength(3)
            .MustHaveMaxLength(50)
            .Validate("Alice");

        await Assert.That(result).Succeeded();
    }

    [Test]
    public async Task StringValidationFailure()
    {
        var result = new StringValidator()
            .CannotBeNullOrWhiteSpace()
            .MustHaveMinLength(3)
            .Validate("");

        // Multiple rules can fail at once — all problems are collected
        if (result.TryPickProblems(out var problems))
        {
            foreach (var p in problems)
                Console.WriteLine(p.Message);
        }

        // assert
        await Assert.That(result).Failed();
    }

    [Test]
    public async Task NumericValidation()
    {
        var result = new IntValidator()
            .MustBePositive()
            .MustBeLessThan(100)
            .Validate(42);

        await Assert.That(result).Succeeded();
    }

    [Test]
    public async Task GenericNumericValidation()
    {
        var result = new DecimalValidator<double>()
            .MustBeBetween(0.0, 1.0)
            .Validate(0.5);

        await Assert.That(result).Succeeded();
    }

    [Test]
    public async Task CollectionValidation()
    {
        var result = new EnumerableValidator<string>()
            .CannotBeNull()
            .CannotBeEmpty()
            .CannotContainDuplicates()
            .Validate(new[] { "a", "b", "c" });

        await Assert.That(result).Succeeded();
    }

    [Test]
    public async Task CustomErrorMessages()
    {
        var result = new StringValidator()
            .CannotBeNullOrWhiteSpace()
            .WithMessage("Username is required")
            .MustHaveMinLength(3)
            .WithMessage("Username must be at least 3 characters")
            .Validate(null);

        if (result.TryPickProblems(out var problems))
        {
            foreach (var p in problems)
                Console.WriteLine(p.Message);
        }

        await Assert.That(result).Failed();
    }

    [Test]
    public async Task ResultIntegration()
    {
        Result ValidateEmail(string? email)
        {
            return new StringValidator()
                .CannotBeNullOrWhiteSpace()
                .WithMessage("Email is required")
                .Validate(email);
        }

        var valid = ValidateEmail("user@example.com");
        var invalid = ValidateEmail(null);

        // assert
        await Assert.That(valid).Succeeded();
        await Assert.That(invalid).Failed();
    }
}
