using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Olve.Utilities.Types;

/// <summary>
///     A value that is not found.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 1)]
[DebuggerDisplay("{ToString()}")]
public readonly struct NotFound
{
    /// <summary>
    ///     Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => nameof(NotFound);
}

/// <summary>
///     A value of type <typeparamref name="T" /> that is not found.
/// </summary>
/// <typeparam name="T">The type of the value that is not found.</typeparam>
[StructLayout(LayoutKind.Sequential, Size = 1)]
[DebuggerDisplay("{ToString()}")]
public readonly struct NotFound<T>
{
    /// <summary>
    ///     Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => $"{nameof(NotFound)}<{typeof(T).Name}>";
}