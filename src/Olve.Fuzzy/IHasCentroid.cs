namespace Olve.Fuzzy;

/// <summary>
/// A membership function that can be used as an output, as it has a centroid when clipped at a given height.
/// </summary>
public interface IHasCentroid
{
    /// <summary>
    /// Gets the centroid of the shape clipped at height <paramref name="y"/>.
    /// </summary>
    /// <param name="y">The clipping height, the degree of activation.</param>
    /// <returns>The centroid, weighted by the clipped area.</returns>
    Centroid GetCentroid(double y);
}
