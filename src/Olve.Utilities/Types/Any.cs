using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Olve.Utilities.Types;

/// <summary>
///     Anything.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 1)]
[DebuggerDisplay("{ToString()}")]
public readonly struct Any
{
    /// <summary>
    ///     Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => nameof(Any);
}

/// <summary>
///     Anything of type <typeparamref name="T" />.
/// </summary>
/// <typeparam name="T">The type of the anything.</typeparam>
[StructLayout(LayoutKind.Sequential, Size = 1)]
[DebuggerDisplay("{ToString()}")]
public readonly struct Any<T>
{
    /// <summary>
    ///     Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => $"{nameof(Any)}<{typeof(T).Name}>";
}