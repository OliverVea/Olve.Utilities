using Olve.Fuzzy.MemberFunctions;

namespace Olve.Fuzzy.Tests.MemberFunctions;

public class ValueTests
{
    private const double Tolerance = 0.01;

    [Test]
    [Arguments(0, 0)]
    [Arguments(0.5 - 1.5 * Tolerance, 0)]
    [Arguments(0.5 - 0.5 * Tolerance, 1)]
    [Arguments(0.5, 1)]
    [Arguments(0.5 + 0.5 * Tolerance, 1)]
    [Arguments(0.5 + 1.5 * Tolerance, 0)]
    [Arguments(1, 0)]
    [Arguments(double.MinValue, 0)]
    [Arguments(double.MaxValue, 0)]
    [Arguments(double.NegativeInfinity, 0)]
    [Arguments(double.PositiveInfinity, 0)]
    public async Task Evaluate(double x, double expected)
    {
        var mf = new Value(0.5, Tolerance);

        await Assert.That(mf.Evaluate(x)).IsEqualTo(expected);
    }
}
