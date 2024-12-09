using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Olve.Utilities.Types;

[StructLayout(LayoutKind.Sequential, Size = 1)]
[DebuggerDisplay("{ToString()}")]
public readonly struct Any
{
    public override string ToString() => nameof(Any);
}

[StructLayout(LayoutKind.Sequential, Size = 1)]
[DebuggerDisplay("{ToString()}")]
public readonly struct Any<T>
{
    public override string ToString() => $"{nameof(Any)}<{typeof(T).Name}>";
}