namespace Olve.Utilities.IntegerMath2D;

/// <summary>
///     Represents a change in position.
/// </summary>
/// <param name="X">The change in the X position.</param>
/// <param name="Y">The change in the Y position.</param>
public readonly record struct DeltaPosition(int X, int Y)
{
    /// <summary>
    ///     Adds two <see cref="DeltaPosition" /> together.
    /// </summary>
    /// <param name="a">The LHS <see cref="DeltaPosition" /></param>
    /// <param name="b">The RHS <see cref="DeltaPosition" /></param>
    /// <returns>The sum of the two <see cref="DeltaPosition" /></returns>
    public static DeltaPosition operator +(DeltaPosition a, DeltaPosition b) => new(a.X + b.X, a.Y + b.Y);

    /// <summary>
    ///     Subtracts the RHS <see cref="DeltaPosition" /> from the LHS <see cref="DeltaPosition" />.
    /// </summary>
    /// <param name="a">The LHS <see cref="DeltaPosition" />.</param>
    /// <param name="b">The RHS <see cref="DeltaPosition" />.</param>
    /// <returns>The <see cref="DeltaPosition" /> resulting from the subtraction.</returns>
    public static DeltaPosition operator -(DeltaPosition a, DeltaPosition b) => new(a.X - b.X, a.Y - b.Y);

    /// <summary>
    ///     Multiplies the <see cref="DeltaPosition" /> by a scalar.
    /// </summary>
    /// <param name="a">The <see cref="DeltaPosition" /> to be scaled.</param>
    /// <param name="scalar">The scalar to scale by.</param>
    /// <returns>The resulting scaled <see cref="DeltaPosition" />.</returns>
    public static DeltaPosition operator *(DeltaPosition a, int scalar) => new(a.X * scalar, a.Y * scalar);
}