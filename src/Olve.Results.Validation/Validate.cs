namespace Olve.Results.Validation;

/// <summary>
/// Provides helper methods to create validators for common value types.
/// </summary>
public static class Validate
{
    /// <summary>
    /// Returns a <see cref="StringValidator"/> for the specified value.
    /// </summary>
    public static StringValidator String(string value) => new(value);

    /// <summary>
    /// Returns a <see cref="IntValidator"/> for the specified value.
    /// </summary>
    public static IntValidator Int32(int value) => new(value);

    /// <summary>
    /// Returns a <see cref="FloatValidator"/> for the specified value.
    /// </summary>
    public static FloatValidator Single(float value) => new(value);

    /// <summary>
    /// Returns a <see cref="DoubleValidator"/> for the specified value.
    /// </summary>
    public static DoubleValidator Double(double value) => new(value);

    /// <summary>
    /// Returns a <see cref="DecimalValidator"/> for the specified value.
    /// </summary>
    public static DecimalValidator Decimal(decimal value) => new(value);
}
