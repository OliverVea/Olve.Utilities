using System.Diagnostics.CodeAnalysis;

namespace Olve.Paths;

public abstract partial class Path : PurePath
{
    public abstract bool TryGetAbsolute([NotNullWhen(true)] out Path? absolute);

    protected abstract Path AppendPath(PurePath right);
    protected override PurePath Append(PurePath right) => AppendPath(right);
    public static Path operator /(Path left, PurePath right) => left.AppendPath(right);

    protected abstract Path AppendPath(string right);
    protected override PurePath Append(string right) => AppendPath(right);
    public static Path operator /(Path left, string right) => left.AppendPath(right);

    public abstract string GetLinkString(int? lineNumber = null, int? columnNumber = null);
}