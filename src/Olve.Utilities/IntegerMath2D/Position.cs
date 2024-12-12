namespace Olve.Utilities.IntegerMath2D;

/// <summary>
/// A position in 2D space.
/// </summary>
/// <param name="X">The x-coordinate.</param>
/// <param name="Y">The y-coordinate.</param>
public readonly record struct Position(int X, int Y)
{
    /// <summary>
    /// Subtracts two positions to get a delta position.
    /// </summary>
    /// <param name="a">LHS position.</param>
    /// <param name="b">RHS position.</param>
    /// <returns>The delta position.</returns>
    public static DeltaPosition operator-(Position a, Position b) => new(a.X - b.X, a.Y - b.Y);
    
    /// <summary>
    /// Adds a delta position to a position to get a new position.
    /// </summary>
    /// <param name="a">LHS position.</param>
    /// <param name="b">RHS delta position.</param>
    /// <returns>The new position.</returns>
    public static Position operator+(Position a, DeltaPosition b) => new(a.X + b.X, a.Y + b.Y);
    
    /// <summary>
    /// Creates a position from an (int X, int Y) tuple.
    /// </summary>
    /// <param name="tuple">The tuple to create the position from.</param>
    /// <returns>The position.</returns>
    public static implicit operator Position((int X, int Y) tuple) => new(tuple.X, tuple.Y);
}