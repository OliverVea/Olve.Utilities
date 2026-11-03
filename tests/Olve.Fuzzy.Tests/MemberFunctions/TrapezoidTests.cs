using Olve.Fuzzy.MemberFunctions;

namespace Olve.Fuzzy.Tests.MemberFunctions;

public class TrapezoidTests
{
    [Test]
    [Arguments(0, 0)]
    [Arguments(0.25, 0)]
    [Arguments(0.3, 0.5)]
    [Arguments(0.35, 1)]
    [Arguments(0.5, 1)]
    [Arguments(0.65, 1)]
    [Arguments(0.7, 0.5)]
    [Arguments(0.75, 0)]
    [Arguments(1, 0)]
    [Arguments(double.MinValue, 0)]
    [Arguments(double.MaxValue, 0)]
    [Arguments(double.NegativeInfinity, double.NaN)]
    [Arguments(double.PositiveInfinity, double.NaN)]
    public async Task Evaluate(double x, double expected)
    {
        var mf = new Trapezoid(0.25, 0.35, 0.65, 0.75);

        await Assert.That(Math.Round(mf.Evaluate(x), 5)).IsEqualTo(expected);
    }
}
