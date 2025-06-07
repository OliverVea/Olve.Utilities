using Olve.Results.TUnit;
using TUnit.Assertions.Exceptions;

namespace Olve.Results.Tests;

public class TUnitValuelessExtensions
{
    private const string SomeProblemMessage = "Some problem happened";
    private static readonly ResultProblem SomeProblem = new(SomeProblemMessage);
    private static readonly Result Failure = SomeProblem;
    private static readonly Result Success = Result.Success();
    
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