using Olve.Utilities.Types.Results;

namespace Olve.Utilities.Tests.Types;

public class ResultTests
{
    public void Test()
    {
        const string message = "Hi!";
        const int severity = 1;
        var arg0 = new { x = 1 };
        string[] tags = ["tag1", "tag2"];
        
        _ = new ResultProblem(message);
        _ = new ResultProblem(message, severity);
        _ = new ResultProblem(message, tags);
        _ = new ResultProblem(message, tags, severity);
        
        // With arguments
        _ = new ResultProblem(message + "{0}", arg0);
        _ = new ResultProblem(message + "{0}", severity, arg0);
        _ = new ResultProblem(message + "{0}", tags, arg0);
        _ = new ResultProblem(message + "{0}", tags, severity, arg0);
    }
}