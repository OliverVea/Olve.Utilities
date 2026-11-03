using Olve.Fuzzy.MemberFunctions;

namespace Olve.Fuzzy.Tests.MemberFunctions;

public class LineTests
{
    [Test]
    [Arguments(false, 0, 0)]
    [Arguments(false, 0.25, 0)]
    [Arguments(false, 0.5, 0.5)]
    [Arguments(false, 0.75, 1)]
    [Arguments(false, 1, 1)]
    [Arguments(false, double.MinValue, 0)]
    [Arguments(false, double.MaxValue, 1)]
    [Arguments(false, double.NegativeInfinity, 0)]
    [Arguments(false, double.PositiveInfinity, 1)]
    [Arguments(true, 0, 1)]
    [Arguments(true, 0.25, 1)]
    [Arguments(true, 0.5, 0.5)]
    [Arguments(true, 0.75, 0)]
    [Arguments(true, 1, 0)]
    [Arguments(true, double.MinValue, 1)]
    [Arguments(true, double.MaxValue, 0)]
    [Arguments(true, double.NegativeInfinity, 1)]
    [Arguments(true, double.PositiveInfinity, 0)]
    public async Task Evaluate(bool reversed, double x, double expected)
    {
        var mf = reversed ? new Line(0.75, 0.25) : new Line(0.25, 0.75);

        await Assert.That(mf.Evaluate(x)).IsEqualTo(expected);
    }
}
