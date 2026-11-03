using Olve.Fuzzy.MemberFunctions;

namespace Olve.Fuzzy.Tests;

public partial class FuzzyRuleTests
{
    private const int None = 0;
    private const int Critical = 1;
    private const int Low = 2;
    private const int Medium = 3;
    private const int High = 4;
    private const int Max = 5;
    private const int AtLeast2 = 6;
    private const int VeryHigh = 7;

    // A fresh world per test: rules attach to the outputs, so outputs must not be shared between tests.
    private sealed class World
    {
        public double HealthValue { get; set; }
        public int EnemiesNearbyValue { get; set; }
        public int AmmoValue { get; set; }

        public FuzzyInput Health { get; }
        public FuzzyInput Ammo { get; }
        public FuzzyInput EnemiesNearby { get; }

        public FuzzyOutput Retreat { get; }
        public FuzzyOutput Fight { get; }

        public World()
        {
            Health = new FuzzyInput(() => HealthValue);
            Health.Set(Critical, new Trapezoid(0, 0, 0.05, 0.2));
            Health.Set(Low, new Trapezoid(0, 0, 0.25, 0.5));
            Health.Set(Medium, new Triangle(0.25, 0.5, 0.75));
            Health.Set(High, new Trapezoid(0.5, 0.75, 1, 1));
            Health.Set(Max, new Trapezoid(0.99, 0.99, 1.01, 1.01));

            Ammo = new FuzzyInput(() => AmmoValue);
            Ammo.Set(Low, new Trapezoid(0, 0, 15, 25));

            EnemiesNearby = new FuzzyInput(() => EnemiesNearbyValue);
            EnemiesNearby.Set(None, new Value(0));
            EnemiesNearby.Set(Low, new Trapezoid(1, 1, 2, 5));
            EnemiesNearby.Set(Medium, new Triangle(2, 5, 7));
            EnemiesNearby.Set(High, new Trapezoid(5, 7, double.MaxValue, double.MaxValue));
            EnemiesNearby.Set(AtLeast2, new AtLeast(2));
            EnemiesNearby.Set(VeryHigh, new Trapezoid(8, 10, double.MaxValue, double.MaxValue));

            Retreat = new FuzzyOutput();
            Retreat.Set(Low, new Trapezoid(-0.5, -0.25, 0.25, 0.5));
            Retreat.Set(Medium, new Triangle(0.25, 0.5, 0.75));
            Retreat.Set(High, new Trapezoid(0.5, 0.75, 1.25, 1.5));

            Fight = new FuzzyOutput();
            Fight.Set(Low, new Trapezoid(-0.5, -0.25, 0.25, 0.5));
            Fight.Set(Medium, new Triangle(0.25, 0.5, 0.75));
            Fight.Set(High, new Trapezoid(0.5, 0.75, 1.25, 1.5));
        }
    }
}
