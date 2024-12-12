namespace Olve.Utilities.IntegerMath2D;

/// <summary>
/// Represents a size in 2D space.
/// </summary>
/// <param name="Width">The width of the size.</param>
/// <param name="Height">The height of the size.</param>
public readonly record struct Size(int Width, int Height)
{
    /// <summary>
    /// Creates a new <see cref="Size"/> with the specified width and height.
    /// </summary>
    /// <param name="tuple">The width and height of the size.</param>
    /// <returns>A new <see cref="Size"/> with the specified width and height.</returns>
    public static implicit operator Size((int Width, int Height) tuple) => new(tuple.Width, tuple.Height);
}