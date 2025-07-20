using Olve.Results.Validation;
using Olve.Results.TUnit;

namespace Olve.Results.Validation.Tests;

public class ValidateTests
{
    [Test]
    public async Task String_ReturnsStringValidator()
    {
        var validator = Validate.String("hello");
        await Assert.That(validator.GetType()).IsEqualTo(typeof(StringValidator));
    }

    [Test]
    public async Task Int32_ReturnsIntValidator()
    {
        var validator = Validate.Int32(1);
        await Assert.That(validator.GetType()).IsEqualTo(typeof(IntValidator));
    }
}
