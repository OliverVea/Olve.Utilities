using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Olve.Utilities.Types;

[StructLayout(LayoutKind.Sequential, Size = 1)]
[DebuggerDisplay("{ToString()}")]
public readonly struct Yes
{
    public override string ToString() => nameof(Yes);
}

[StructLayout(LayoutKind.Sequential, Size = 1)]
[DebuggerDisplay("{ToString()}")]
public readonly record struct Yes<T>(T Value)
{
    public override string ToString() => $"{nameof(Yes)}<{typeof(T).Name}>({Value})";
}