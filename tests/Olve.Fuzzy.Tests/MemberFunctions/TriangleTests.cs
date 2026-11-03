using Olve.Fuzzy.MemberFunctions;

namespace Olve.Fuzzy.Tests.MemberFunctions;

public class TriangleTests
{
    [Test]
    [Arguments(0, 0)]
    [Arguments(0.25, 0)]
    [Arguments(0.375, 0.5)]
    [Arguments(0.5, 1)]
    [Arguments(0.625, 0.5)]
    [Arguments(0.75, 0)]
    [Arguments(1, 0)]
    [Arguments(double.MinValue, 0)]
    [Arguments(double.MaxValue, 0)]
    [Arguments(double.NegativeInfinity, double.NaN)]
    [Arguments(double.PositiveInfinity, double.NaN)]
    public async Task Evaluate(double x, double expected)
    {
        var mf = new Triangle(0.25, 0.5, 0.75);

        await Assert.That(mf.Evaluate(x)).IsEqualTo(expected);
    }

    // A triangle is a trapezoid whose two centre points coincide, so both must agree everywhere.
    // Regression: Triangle's centroid dropped the min/max offsets, so it was wrong for shapes not starting at 0,
    // and Evaluate returned NaN at a peak that coincides with an edge.
    [Test]
    [Arguments(0, 0.5, 1)]
    [Arguments(0.5, 1, 1.5)]
    [Arguments(2, 5, 7)]
    [Arguments(0, 1, 1)]
    [Arguments(1, 1, 3)]
    public async Task Matches_trapezoid_with_a_single_centre(double min, double center, double max)
    {
        var triangle = new Triangle(min, center, max);
        var trapezoid = new Trapezoid(min, center, center, max);

        using (Assert.Multiple())
        {
            for (var i = 0; i <= 8; i++)
            {
                var x = min + (max - min) * i / 8;
                await Assert.That(triangle.Evaluate(x)).IsEqualTo(trapezoid.Evaluate(x)).Within(1e-12);
            }

            foreach (var y in new[] { 0.25, 0.6, 1 })
            {
                var expected = ((IHasCentroid)trapezoid).GetCentroid(y);
                var actual = ((IHasCentroid)triangle).GetCentroid(y);
                await Assert.That(actual.Value).IsEqualTo(expected.Value).Within(1e-12);
                await Assert.That(actual.Weight).IsEqualTo(expected.Weight).Within(1e-12);
            }
        }
    }
}
