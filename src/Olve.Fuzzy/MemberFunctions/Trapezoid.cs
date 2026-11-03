namespace Olve.Fuzzy.MemberFunctions;

/// <summary>
/// A trapezoid rising from <c>min</c> to <c>centerLeft</c>, 1 until <c>centerRight</c>, and falling to <c>max</c>.
/// </summary>
/// <param name="min">Where the membership starts rising from 0.</param>
/// <param name="centerLeft">Where the membership reaches 1.</param>
/// <param name="centerRight">Where the membership starts falling from 1.</param>
/// <param name="max">Where the membership is back at 0.</param>
public sealed class Trapezoid(double min, double centerLeft, double centerRight, double max) : IMemberFunction, IHasCentroid
{
    /// <inheritdoc />
    /// <returns>The degree of membership, or <see cref="double.NaN"/> when <paramref name="x"/> is not finite.</returns>
    public double Evaluate(double x)
    {
        if (!double.IsFinite(x)) return double.NaN;

        if (x < min || x > max) return 0.0;

        if (x < centerLeft) return (x - min) / (centerLeft - min);
        if (x <= centerRight) return 1.0;
        return (max - x) / (max - centerRight);
    }

    Centroid IHasCentroid.GetCentroid(double y)
    {
        if (y == 0) return new Centroid(0, 0);

        var leftSlope = 1 / (centerLeft - min);
        var leftMax = y / leftSlope + min;
        var leftCenter = (leftMax + min) / 2;
        var leftWeight = (leftMax - min) * y / 2;

        var rightSlope = 1 / (max - centerRight);
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
