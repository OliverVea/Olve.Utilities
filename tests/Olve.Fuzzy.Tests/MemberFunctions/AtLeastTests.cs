using Olve.Fuzzy.MemberFunctions;

namespace Olve.Fuzzy.Tests.MemberFunctions;

public class AtLeastTests
{
    [Test]
    [Arguments(0, 0)]
    [Arguments(0.499, 0)]
    [Arguments(0.5, 1)]
    [Arguments(1, 1)]
    [Arguments(double.MinValue, 0)]
    [Arguments(double.MaxValue, 1)]
    [Arguments(double.NegativeInfinity, 0)]
    [Arguments(double.PositiveInfinity, 1)]
    public async Task Evaluate(double x, double expected)
    {
        var mf = new AtLeast(0.5);

        await Assert.That(mf.Evaluate(x)).IsEqualTo(expected);
    }
}
