using System.Linq;
using Olve.Results;
using Olve.Results.Validation;
using Olve.Results.TUnit;

namespace Olve.Results.Validation.Tests;

public class StringValidatorTests
{
    [Test]
    public async Task IsNotNullOrEmpty_WithEmptyString_Fails()
    {
        var result = new StringValidator(string.Empty).IsNotNullOrEmpty();

        await Assert.That((Result)result).Failed();
    }

    [Test]
    public async Task IsNotNullOrEmpty_WithValue_Succeeds()
    {
        var result = new StringValidator("hello").IsNotNullOrEmpty();

        await Assert.That((Result)result).Succeeded();
    }

    [Test]
    public async Task WithProblem_ReplacesMessage()
    {
        var result = new StringValidator(" ")
            .IsNotNullOrWhiteSpace()
            .WithProblem("custom message");

        var problems = ((Result)result).Problems!;
        var message = problems.Single().Message;
        await Assert.That(message).IsEqualTo("custom message");
    }
}
