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
}