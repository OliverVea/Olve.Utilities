namespace Olve.Fuzzy.MemberFunctions;

/// <summary>
/// A triangle rising from <c>min</c> to 1 at <c>center</c> and falling to <c>max</c>.
/// </summary>
/// <param name="min">Where the membership starts rising from 0.</param>
/// <param name="center">Where the membership is 1.</param>
/// <param name="max">Where the membership is back at 0.</param>
public sealed class Triangle(double min, double center, double max) : IMemberFunction, IHasCentroid
{
    /// <inheritdoc />
    /// <returns>The degree of membership, or <see cref="double.NaN"/> when <paramref name="x"/> is not finite.</returns>
    public double Evaluate(double x)
    {
        if (!double.IsFinite(x)) return double.NaN;

        if (x < min || x > max) return 0.0;

        if (x == center) return 1.0;
        if (x < center) return (x - min) / (center - min);
        return (max - x) / (max - center);
    }

    Centroid IHasCentroid.GetCentroid(double y)
    {
        if (y == 0) return new Centroid(0, 0);

        var leftSlope = 1 / (center - min);
        var leftMax = y / leftSlope + min;
        var leftCenter = (leftMax + min) / 2;
        var leftWeight = (leftMax - min) * y / 2;

        var rightSlope = 1 / (max - center);
        var rightMin = max - y / rightSlope;
        var rightCenter = (max + rightMin) / 2;
        var rightWeight = (max - rightMin) * y / 2;

        var centerCenter = (rightMin + leftMax) / 2;
        var centerWeight = (rightMin - leftMax) * y;

        return new[]
        {
            new Centroid(leftCenter, leftWeight),
            new Centroid(rightCenter, rightWeight),
            new Centroid(centerCenter, centerWeight),
        }.Sum();
    }
}
