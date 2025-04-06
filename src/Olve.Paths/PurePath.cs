using System.Diagnostics.CodeAnalysis;

namespace Olve.Paths;

public abstract class PurePath
{
    public abstract PathPlatform Platform { get; }
    public abstract PathType Type { get; }

    public abstract string GetFullString();
    public override string ToString() => GetFullString();

    public abstract bool TryGetParent([NotNullWhen(true)] out PurePath? parent);
    public PurePath? Parent => TryGetParent(out var parent) ? parent : null;

    public abstract bool TryGetFileName([NotNullWhen(true)] out string? fileName);
    public string? FileName => TryGetFileName(out var fileName) ? fileName : null;

    protected abstract PurePath Append(PurePath right);
    public static PurePath operator /(PurePath left, PurePath right) => left.Append(right);

    protected abstract PurePath Append(string right);
    public static PurePath operator /(PurePath left, string right) => left.Append(right);
}