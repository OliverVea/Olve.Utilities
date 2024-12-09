using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Olve.Utilities.Types;

[StructLayout(LayoutKind.Sequential, Size = 1)]
[DebuggerDisplay("{ToString()}")]
public readonly struct Waiting
{
    public override string ToString() => nameof(Waiting);
}

[StructLayout(LayoutKind.Sequential, Size = 1)]
[DebuggerDisplay("{ToString()}")]
public readonly struct Waiting<T>
{
    public override string ToString() => $"{nameof(Waiting)}<{typeof(T).Name}>";
}