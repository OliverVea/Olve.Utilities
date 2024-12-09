using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Olve.Utilities.Types;

/// <summary>
/// Represents a type that is always true.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 1)]
[DebuggerDisplay("{ToString()}")]
public readonly struct Yes
{
    /// <summary>
    /// Returns a string representation of the value.
    /// </summary>
    /// <returns>The string representation of the value.</returns>
    public override string ToString() => nameof(Yes);
}

/// <summary>
/// Represents a type that is always true with a value.
/// </summary>
/// <param name="Value">The value that is true.</param>
/// <typeparam name="T">The type of the value.</typeparam>
[StructLayout(LayoutKind.Sequential, Size = 1)]
[DebuggerDisplay("{ToString()}")]
public readonly record struct Yes<T>(T Value)
{
    /// <summary>
    /// Returns a string representation of the value.
    /// </summary>
    /// <returns>The string representation of the value.</returns>
    public override string ToString() => $"{nameof(Yes)}<{typeof(T).Name}>({Value})";
}