using System.Diagnostics;

namespace Olve.Results.Tests;

public class ResultTests
{
    private const string Message = "some problem occurred";

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

        // Act
        var problem = new ResultProblem(Message);

        // Assert
        await Assert.That(problem.OriginInformation.FilePath).EndsWith("ResultTests.cs");
        await Assert.That(problem.OriginInformation.LineNumber).IsEqualTo(36);
    }

    [Test]
    public async Task Test2()
    {
        // Arrange

        // Act
        var problem = GetResultProblem();

        // Assert
        await Assert.That(problem.OriginInformation.FilePath).EndsWith("ResultTests.cs");
        await Assert.That(problem.OriginInformation.LineNumber).IsEqualTo(58);
    }

    private static ResultProblem GetResultProblem()
    {
        return new ResultProblem(Message);
    }

    [Test]
    public async Task Test3()
    {
        // Arrange
        var problem = GetResultProblem();

        // Act
        var debugString = problem.ToDebugString();

        Debug.WriteLine(debugString);
        Console.WriteLine(debugString);

        // Assert
        await Assert.That(debugString).EndsWith("ResultTests.cs:l58] some problem occurred");
    }

    [Test]
    [Arguments(false, false, false, false)]
    [Arguments(true, false, true, false)]
    [Arguments(false, true, false, false)]
    [Arguments(true, true, true, true)]
    public async Task Test4(bool aSucceeds, bool bSucceeds, bool bInvokedExpected, bool expected)
    {
        // Arrange
        bool aInvoked = false, bInvoked = false;

        Func<Result> a = aSucceeds ? () => Success(() => aInvoked = true) : () => Fail(() => aInvoked = true);
        Func<Result> b = bSucceeds ? () => Success(() => bInvoked = true) : () => Fail(() => bInvoked = true);

        // Act
        var actual = Result.Chain(a, b);

        await Assert.That(actual.Succeeded).IsEqualTo(expected);
        await Assert.That(aInvoked).IsTrue();
        await Assert.That(bInvoked).IsEqualTo(bInvokedExpected);
    }

    [Test]
    public async Task Test5()
    {
        // Arrange
        Func<Result<int>> a = () => Result.Success(1), b = () => Result.Success(2), c = () => Result.Success(3);

        // Act
        var result = Result.Chain(a, b, c);

        // Assert
        await Assert.That(result.Succeeded).IsTrue();

        var (aValue, bValue, cValue) = result.Value;

        await Assert.That(aValue).IsEqualTo(1);
        await Assert.That(bValue).IsEqualTo(2);
        await Assert.That(cValue).IsEqualTo(3);
    }

    private Result Fail(Action? action)
    {
        action?.Invoke();

        return new ResultProblem("fail!");
    }

    private Result Success(Action? action = null)
    {
        action?.Invoke();

        return Result.Success();
    }
}