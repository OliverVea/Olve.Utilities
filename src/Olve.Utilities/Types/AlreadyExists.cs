using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Olve.Utilities.Types;

[StructLayout(LayoutKind.Sequential, Size = 1)]
[DebuggerDisplay("{ToString()}")]
public readonly struct AlreadyExists
{
    public override string ToString() => nameof(AlreadyExists);
}

[StructLayout(LayoutKind.Sequential, Size = 1)]
[DebuggerDisplay("{ToString()}")]
public readonly struct AlreadyExists<T>
{
    public override string ToString() => $"{nameof(AlreadyExists)}<{typeof(T).Name}>";
}
