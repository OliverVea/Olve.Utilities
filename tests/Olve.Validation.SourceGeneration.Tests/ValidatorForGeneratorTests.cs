using System.Linq;
using Olve.Validation;
using Olve.Validation.SourceGeneration;
using Olve.Results;
using Olve.Results.TUnit;

namespace Olve.Validation.SourceGeneration.Tests;

public class MyDto
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
}

[ValidatorFor(typeof(MyDto))]
public partial class MyDtoValidator : ValidatorFor<MyDto>
{
    private static IValidator<string> GetNameValidator() => new StringValidator().IsNotNullOrWhiteSpace();
    private static IValidator<int> GetAgeValidator() => new IntValidator().IsPositive();
}

public class ValidatorForGeneratorTests
{

    [Test]
    public async Task GeneratedConfigure_WiresValidators()
    {
        var v = new MyDtoValidator();
        var ok = v.Validate(new MyDto { Name = "Alice", Age = 30 });
        await Assert.That(ok.Succeeded).IsTrue();

        var fail = v.Validate(new MyDto { Name = "", Age = -5 });
        await Assert.That(fail.Succeeded).IsFalse();
        var problems = fail.Problems!;
        await Assert.That(problems.Any(p => p.Message.Contains("Name"))).IsTrue();
        await Assert.That(problems.Any(p => p.Message.Contains("Age"))).IsTrue();
    }
}
