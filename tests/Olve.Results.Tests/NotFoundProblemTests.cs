using Olve.Results.TUnit;

namespace Olve.Results.Tests;

public class NotFoundProblemTests
{
    [Test]
    public async Task Constructor_RecordsCallSiteAsOrigin()
    {
        var problem = new NotFoundProblem("User {0} not found", 42);

        await Assert.That(problem.OriginInformation.FilePath.Path).EndsWith("NotFoundProblemTests.cs");
        await Assert.That(problem.OriginInformation.LineNumber).IsEqualTo(10);
    }

    [Test]
    public async Task Constructor_FormatsMessageAndIsNotRetryable()
    {
        var problem = new NotFoundProblem("User {0} not found", 42);

        await Assert.That(problem.FormattedMessage).IsEqualTo("User 42 not found");
        await Assert.That(problem.IsRetryable).IsFalse();
    }

    [Test]
    public async Task TryPickProblem_AfterPrependedContext_FindsNotFound()
    {
        Result result = new NotFoundProblem("User {0} not found", 42);

        Result withContext = result.Problems!.Prepend("Could not rename user");

        await Assert.That(withContext.TryPickProblem<NotFoundProblem>(out _)).IsTrue();
    }
}
