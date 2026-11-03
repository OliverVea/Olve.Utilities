using Olve.Fuzzy.MemberFunctions;

namespace Olve.Fuzzy.Tests;

public class ReadmeDemo
{
    [Test]
    public async Task FleeFromLowHealth()
    {
        var healthValue = 35.0;
        var health = new FuzzyInput(() => healthValue);
        var flee = new FuzzyOutput();

        const int low = 0;
        const int medium = 1;
        const int high = 2;

        health.Set(low, new Trapezoid(0, 0, 25, 50));
        health.Set(medium, new Triangle(25, 50, 75));
        health.Set(high, new Trapezoid(50, 75, 100, 100));

        flee.Set(low, new Triangle(-0.5, 0, 0.5));
        flee.Set(medium, new Triangle(0, 0.5, 1));
        flee.Set(high, new Triangle(0.5, 1, 1.5));

        FuzzyRule.If(health.Is(high)).Then(flee.Is(low));
        FuzzyRule.If(health.Is(medium)).Then(flee.Is(medium));
        FuzzyRule.If(health.Is(low)).Then(flee.Is(high));

        var result = flee.Evaluate(); // ≈ 0.78

        await Assert.That(result).IsEqualTo(0.784).Within(0.01);
    }
}
