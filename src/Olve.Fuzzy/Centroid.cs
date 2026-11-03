namespace Olve.Fuzzy;

/// <summary>
/// A weighted centroid of a fuzzy shape.
/// </summary>
/// <param name="value">The position of the centroid.</param>
/// <param name="weight">The weight of the centroid, the area of the shape.</param>
public sealed class Centroid(double value, double weight)
{
    /// <summary>
    /// Gets the position of the centroid.
    /// </summary>
    public double Value { get; } = value;

    /// <summary>
    /// Gets the weight of the centroid, the area of the shape.
    /// </summary>
    public double Weight { get; } = weight;

    /// <summary>
    /// A centroid at 0 with no weight.
    /// </summary>
    public static readonly Centroid Zero = new(0, 0);
}
