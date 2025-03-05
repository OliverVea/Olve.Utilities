using Olve.Utilities.Types.Results;

namespace Olve.Utilities.Tests.Types;

public class ResultTests
{
    private void SyntaxTesting()
    {
        const string message = "Hi!";
        const int severity = 1;
        var arg0 = new { x = 1 };
        var arg1 = new { x = 2 };
        string[] tags = ["tag1", "tag2"];

        _ = new ResultProblem(message);
        _ = new ResultProblem(message) { Severity = severity };
        _ = new ResultProblem(message) { Tags = tags };
        _ = new ResultProblem(message) { Severity = severity, Tags = tags };

        // With arguments
        _ = new ResultProblem(message + "{0}", arg0);
        _ = new ResultProblem(message + "{0}, {1}", arg0, arg1);
        _ = new ResultProblem(message + "{0}", arg0) { Severity = severity };
        _ = new ResultProblem(message + "{0}", arg0) { Tags = tags };
        _ = new ResultProblem(message + "{0}", arg0) { Severity = severity, Tags = tags };
    }

    [Test]
    public async Task Test()
    {
        // Arrange
        const string message = "some problem occurred";

        // Act
        var problem = new ResultProblem(message);

        // Assert
        await Assert.That(problem.OriginInformation.MemberName).IsEqualTo("MoveNext");
        await Assert.That(problem.OriginInformation.FilePath).EndsWith("ResultTests.cs");
        await Assert.That(problem.OriginInformation.LineNumber).IsEqualTo(35);
    }

    [Test]
    public async Task Test2()
    {
        // Arrange

        // Act
        var problem = GetResultProblem();

        // Assert
        await Assert.That(problem.OriginInformation.MemberName).IsEqualTo(nameof(GetResultProblem));
        await Assert.That(problem.OriginInformation.FilePath).EndsWith("ResultTests.cs");
        await Assert.That(problem.OriginInformation.LineNumber).IsEqualTo(59);
    }

    private static ResultProblem GetResultProblem()
    {
        return new ResultProblem("some problem occurred");
    }
}