using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Olve.Utilities.Types;

/// <summary>
/// Represents a waiting state.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 1)]
[DebuggerDisplay("{ToString()}")]
public readonly struct Waiting
{
    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => nameof(Waiting);
}

/// <summary>
/// Represents a waiting state.
/// </summary>
/// <typeparam name="T">The type of the waiting state.</typeparam>
[StructLayout(LayoutKind.Sequential, Size = 1)]
[DebuggerDisplay("{ToString()}")]
public readonly struct Waiting<T>
{
    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => $"{nameof(Waiting)}<{typeof(T).Name}>";
}