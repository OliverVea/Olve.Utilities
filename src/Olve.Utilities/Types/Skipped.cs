using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Olve.Utilities.Types;

[StructLayout(LayoutKind.Sequential, Size = 1)]
[DebuggerDisplay("{ToString()}")]
public readonly struct Skipped
{
    public override string ToString() => nameof(Skipped);
}

[StructLayout(LayoutKind.Sequential, Size = 1)]
[DebuggerDisplay("{ToString()}")]
public readonly struct Skipped<T>
{
    public override string ToString() => $"{nameof(Skipped)}<{typeof(T).Name}>";
}