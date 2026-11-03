namespace Olve.Fuzzy.Tests;

public partial class FuzzyRuleTests
{
    private const double Epsilon = 0.01;

    [Test]
    [Arguments(0, 1)]
    [Arguments(0.15, 1)]
    [Arguments(0.25, 1)]
    [Arguments(0.5, 0)]
    [Arguments(1, 0)]
    [Arguments(2, 0)]
    [Arguments(-1, 0)]
    [Arguments(double.MinValue, 0)]
    [Arguments(double.MaxValue, 0)]
    [Arguments(double.NaN, double.NaN)]
    [Arguments(double.NegativeInfinity, double.NaN)]
    [Arguments(double.PositiveInfinity, double.NaN)]
    public async Task Single_Antecedent_and_Consequent(double health, double expectedRetreat)
    {
        var w = new World();
        FuzzyRule.If(w.Health.Is(Low))
            .Then(w.Retreat.Is(High));

        w.HealthValue = health;

        await Assert.That(w.Retreat.Evaluate()).IsEqualTo(expectedRetreat);
    }

    [Test]
    [Arguments(0, 0, 0)]
    [Arguments(1, 2, 1)]
    [Arguments(0.5, 2, 0)]
    [Arguments(0.9, 2, 0)]
    [Arguments(1, 0, 0)]
    [Arguments(1, 10, 0)]
    public async Task Multiple_Antecedents(double health, int enemiesNearby, double expectedFight)
    {
        var w = new World();
        FuzzyRule.If(w.Health.Is(Max))
                .And(w.EnemiesNearby.Is(Low))
            .Then(w.Fight.Is(High));

        w.HealthValue = health;
        w.EnemiesNearbyValue = enemiesNearby;

        await Assert.That(w.Fight.Evaluate()).IsEqualTo(expectedFight);
    }

    [Test]
    [Arguments(0, 0)]
    [Arguments(0.15, 0)]
    [Arguments(0.25, 0)]
    [Arguments(0.5, 1)]
    [Arguments(1, 1)]
    [Arguments(2, 1)]
    [Arguments(-1, 1)]
    [Arguments(double.MinValue, 1)]
    [Arguments(double.MaxValue, 1)]
    [Arguments(double.NaN, double.NaN)]
    [Arguments(double.NegativeInfinity, double.NaN)]
    [Arguments(double.PositiveInfinity, double.NaN)]
    public async Task Negation(double health, double expectedRetreat)
    {
        var w = new World();
        FuzzyRule.If(w.Health.IsNot(Low))
            .Then(w.Retreat.Is(High));

        w.HealthValue = health;

        await Assert.That(w.Retreat.Evaluate()).IsEqualTo(expectedRetreat);
    }

    [Test]
    [Arguments(0, 0.875)]
    [Arguments(0.25, 0.5)]
    [Arguments(0.5, 0.0)]
    [Arguments(0.9, 0)]
    [Arguments(1, 0)]
    public async Task Multiple_Rules(double health, double expectedRetreat)
    {
        var w = new World();
        FuzzyRule.If(w.Health.Is(Critical)).Then(w.Retreat.Is(High));
        FuzzyRule.If(w.Health.Is(Low)).Then(w.Retreat.Is(Medium));
        FuzzyRule.If(w.Health.Is(Medium)).Then(w.Retreat.Is(Low));

        w.HealthValue = health;

        await Assert.That(w.Retreat.Evaluate()).IsEqualTo(expectedRetreat).Within(Epsilon);
    }

    [Test]
    [Arguments(0, 1, 0)]
    [Arguments(0.125, 1, 0)]
    [Arguments(0.25, 1, 0)]
    [Arguments(0.375, 0.5, 0.5)]
    [Arguments(0.5, 0, 1)]
    [Arguments(1, 0, 1)]
    public async Task Multiple_Consequents(double health, double expectedRetreat, double expectedFight)
    {
        var w = new World();
        FuzzyRule.If(w.Health.IsNot(Low))
            .Then(w.Fight.Is(High))
                .And(w.Retreat.Is(Low));

        FuzzyRule.If(w.Health.Is(Low))
            .Then(w.Retreat.Is(High))
                .And(w.Fight.Is(Low));

        w.HealthValue = health;

        using (Assert.Multiple())
        {
            await Assert.That(w.Retreat.Evaluate()).IsEqualTo(expectedRetreat);
            await Assert.That(w.Fight.Evaluate()).IsEqualTo(expectedFight);
        }
    }

    [Test]
    [Arguments(0, 0, 0.5, 0)]
    [Arguments(0.25, 0, 0.5, 0)]
    [Arguments(0.5, 0, 0, 0.5)]
    [Arguments(1, 0, 0, 0.5)]
    [Arguments(0, 10, 0.875, 0)]
    [Arguments(0.25, 10, 0.875, 0)]
    [Arguments(0.5, 10, 0.5, 0.125)]
    [Arguments(1, 10, 0.5, 0.125)]
    public async Task Complex_Fuzzy_Rule_Combinations(double health, int enemiesNearby, double expectedRetreat, double expectedFight)
    {
        var w = new World();
        FuzzyRule.If(w.Health.Is(Medium))
                .Or(w.Health.Is(High))
            .Then(w.Fight.Is(Medium))
                .And(w.Retreat.Is(Low))
            .Else(w.Retreat.Is(Medium))
                .And(w.Fight.Is(Low));

        FuzzyRule.If(w.EnemiesNearby.Is(High))
            .Then(w.Retreat.Is(High))
                .And(w.Fight.Is(Low));

        w.HealthValue = health;
        w.EnemiesNearbyValue = enemiesNearby;

        using (Assert.Multiple())
        {
            await Assert.That(w.Retreat.Evaluate()).IsEqualTo(expectedRetreat);
            await Assert.That(w.Fight.Evaluate()).IsEqualTo(expectedFight);
        }
    }

    [Test]
    [Arguments(0, 0, 1)]
    [Arguments(0, 2, 1)]
    [Arguments(0, 5, 1)]
    [Arguments(0.25, 0, 1)]
    [Arguments(0.25, 2, 1)]
    [Arguments(0.25, 5, 1)]
    [Arguments(0.5, 0, 1)]
    [Arguments(0.5, 2, 0)]
    [Arguments(0.5, 5, 0)]
    public async Task Fuzzy_Quantifiers(double health, int enemiesNearby, double expectedRetreat)
    {
        var w = new World();
        FuzzyRule.If(w.EnemiesNearby.Is(AtLeast2))
                .And(w.Health.IsNot(Low))
            .Then(w.Retreat.Is(Low))
            .Else(w.Retreat.Is(High));

        w.HealthValue = health;
        w.EnemiesNearbyValue = enemiesNearby;

        await Assert.That(w.Retreat.Evaluate()).IsEqualTo(expectedRetreat);
    }

    [Test]
    [Arguments(0, 0, 0, 0)]
    [Arguments(0, 20, 0, 0)]
    [Arguments(0, 100, 0, 0)]
    [Arguments(0.25, 0, 0, 0)]
    [Arguments(0.25, 20, 0, 0)]
    [Arguments(0.25, 100, 0, 0)]
    [Arguments(0.5, 0, 0, 0)]
    [Arguments(0.5, 20, 0, 0)]
    [Arguments(0.5, 100, 0, 0)]
    [Arguments(0, 0, 10, 1)]
    [Arguments(0, 20, 10, 1)]
    [Arguments(0, 100, 10, 1)]
    [Arguments(0.25, 0, 10, 1)]
    [Arguments(0.25, 20, 10, 1)]
    [Arguments(0.25, 100, 10, 1)]
    [Arguments(0.5, 0, 10, 1)]
    [Arguments(0.5, 20, 10, 1)]
    [Arguments(0.5, 100, 10, 0)]
    public async Task Compound_Or_Antecedent(double health, int ammo, int enemiesNearby, double expectedRetreat)
    {
        var w = new World();
        FuzzyRule.If(w.Health.Is(Low))
                .Or(w.Ammo.Is(Low))
                .And(w.EnemiesNearby.Is(High))
            .Then(w.Retreat.Is(High));

        w.HealthValue = health;
        w.AmmoValue = ammo;
        w.EnemiesNearbyValue = enemiesNearby;

        await Assert.That(w.Retreat.Evaluate()).IsEqualTo(expectedRetreat);
    }

    [Test]
    [Arguments(0, 0, 0, 1)]
    [Arguments(0, 20, 0, 1)]
    [Arguments(0, 100, 0, 0)]
    [Arguments(0.25, 0, 0, 1)]
    [Arguments(0.25, 20, 0, 1)]
    [Arguments(0.25, 100, 0, 0)]
    [Arguments(0.5, 0, 0, 0)]
    [Arguments(0.5, 20, 0, 0)]
    [Arguments(0.5, 100, 0, 0)]
    [Arguments(0, 0, 10, 1)]
    [Arguments(0, 20, 10, 1)]
    [Arguments(0, 100, 10, 1)]
    [Arguments(0.25, 0, 10, 1)]
    [Arguments(0.25, 20, 10, 1)]
    [Arguments(0.25, 100, 10, 1)]
    [Arguments(0.5, 0, 10, 1)]
    [Arguments(0.5, 20, 10, 1)]
    [Arguments(0.5, 100, 10, 1)]
    public async Task Compound_And_Antecedent(double health, int ammo, int enemiesNearby, double expectedRetreat)
    {
        var w = new World();
        FuzzyRule.If(w.Health.Is(Low))
                .And(w.Ammo.Is(Low))
                .Or(w.EnemiesNearby.Is(High))
            .Then(w.Retreat.Is(High));

        w.HealthValue = health;
        w.AmmoValue = ammo;
        w.EnemiesNearbyValue = enemiesNearby;

        await Assert.That(w.Retreat.Evaluate()).IsEqualTo(expectedRetreat);
    }

    [Test]
    [Arguments(0, 0, 0, 0.5)]
    [Arguments(0.25, 0, 0, 0.5)]
    [Arguments(0.5, 0, 0, 0)]
    [Arguments(1, 0, 0, 0)]
    [Arguments(0, 1, 0, 0.5)]
    [Arguments(0.25, 1, 0, 0.5)]
    [Arguments(0.5, 1, 1, 0)]
    [Arguments(1, 1, 1, 0)]
    [Arguments(0, 10, 0, 0.5)]
    [Arguments(0.25, 10, 0, 0.5)]
    [Arguments(0.5, 10, 1, 0)]
    [Arguments(1, 10, 1, 0)]
    public async Task Chained_Rules(double health, int enemiesNearby, double expectedFight, double expectedRetreat)
    {
        var w = new World();
        FuzzyRule.If(w.Health.Is(Low))
            .Then(w.Retreat.Is(Medium))
            .ElseIf(w.EnemiesNearby.IsNot(None))
            .Then(w.Fight.Is(High));

        w.HealthValue = health;
        w.EnemiesNearbyValue = enemiesNearby;

        using (Assert.Multiple())
        {
            await Assert.That(w.Retreat.Evaluate()).IsEqualTo(expectedRetreat);
            await Assert.That(w.Fight.Evaluate()).IsEqualTo(expectedFight);
        }
    }
}
