using Olve.Results.TUnit;
using TUnit.Assertions.Exceptions;

namespace Olve.Results.Tests;

public class TUnitValueExtensions
{
    private const string SomeProblemMessage = "Some problem happened";
    private static readonly ResultProblem SomeProblem = new(SomeProblemMessage);
    private static readonly Result<int> Failure = SomeProblem;
    private static readonly Result<int> Success = 11;
    
    [Test]
    public async Task Failed_WithFailure_Succeeds()
    {
        await Assert.That(Failure).Failed();
    }
    
    [Test]
    public async Task Failed_WithSuccess_ThrowsAssertionException()
    {
        await Assert.ThrowsAsync<AssertionException>(async () => await Assert.That(Success).Failed());
    }
    
    [Test]
    public async Task Succeeded_WithSuccess_Succeeds()
    {
        await Assert.That(Success).Succeeded();
    }
    
    [Test]
    public async Task Succeeded_WithFailure_ThrowsAssertionException()
    {
        await Assert.ThrowsAsync<AssertionException>(async () => await Assert.That(Failure).Succeeded());
    }
    
    
    [Test]
    public async Task SucceedsAndValue_WithSuccess_UnwrapsValue()
    {
        await Assert.That(Success).SucceededAndValue().IsEqualTo(11);
    }

    [Test]
    public async Task SucceedsAndValue_WithFailure_ThrowsAssertionException()
    {
        await Assert.ThrowsAsync<AssertionException>(async () => 
            await Assert.That(Failure).SucceededAndValue().IsEqualTo(42));
    }

    [Test]
    public async Task FailsAndProblemCollection_WithFailure_UnwrapsProblem()
    {
        await Assert.That(Failure).FailedAndProblemCollection().HasCount().EqualTo(1);
    }

    [Test]
    public async Task FailsAndProblemCollection_WithSuccess_ThrowsAssertionException()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
            await Assert.That(Success).FailedAndProblemCollection().IsNotNull());
    }
}