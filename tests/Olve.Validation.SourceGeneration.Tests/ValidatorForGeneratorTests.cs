using Olve.Validation;
using Olve.Validation.SourceGeneration;
using Olve.Results.TUnit;

namespace Olve.Validation.SourceGeneration.Tests;

public class ValidatorForGeneratorTests
{
    public class MyDto
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    public class MyDtoValidator : ValidatorFor<MyDto>
    {
        private static IValidator<string> GetNameValidator() => new StringValidator().MinLength(1);
        private static IValidator<int> GetAgeValidator() => new IntValidator().IsPositive();

        protected override void Configure(ValidationDescriptor<MyDto> descriptor)
        {
            descriptor.For(x => x.Name, GetNameValidator(), nameof(MyDto.Name));
            descriptor.For(x => x.Age, GetAgeValidator(), nameof(MyDto.Age));
        }
    }

    [Test]
    public async Task GeneratedValidator_WiresUpProperties()
    {
        var validator = new MyDtoValidator();

        var ok = validator.Validate(new MyDto { Name = "Alice", Age = 30 });
        await Assert.That(ok).Succeeded();

        var fail = validator.Validate(new MyDto { Name = string.Empty, Age = -5 });
        await Assert.That(fail).Failed();
        var sources = fail.Problems!.Select(p => p.Source!).Where(s => s is not null);
        await Assert.That(sources).Contains("Name");
        await Assert.That(sources).Contains("Age");
    }
}
