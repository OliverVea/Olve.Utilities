using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Olve.Utilities.Types;

/// <summary>
///     Represents a skipped value.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 1)]
[DebuggerDisplay("{ToString()}")]
public readonly struct Skipped
{
    /// <summary>
    ///     Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => nameof(Skipped);
}

/// <summary>
///     Represents a skipped value.
/// </summary>
/// <typeparam name="T">The type of the skipped value.</typeparam>
[StructLayout(LayoutKind.Sequential, Size = 1)]
[DebuggerDisplay("{ToString()}")]
public readonly struct Skipped<T>
{
    /// <summary>
    ///     Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => $"{nameof(Skipped)}<{typeof(T).Name}>";
}
