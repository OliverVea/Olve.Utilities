using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Olve.Results.Analyzers;

/// <summary>
///     A value-equatable wrapper over <typeparamref name="T"/><c>[]</c> so that models flowing through
///     the incremental generator pipeline compare by content, not array reference. Without this the
///     pipeline would re-run on every keystroke even when nothing changed.
/// </summary>
internal readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>, IEnumerable<T>
    where T : IEquatable<T>
{
    private readonly T[]? _array;

    public EquatableArray(T[] array) => _array = array;

    public int Count => _array?.Length ?? 0;

    public bool Equals(EquatableArray<T> other)
    {
        if (_array is null || other._array is null)
        {
            return _array is null && other._array is null;
        }

        if (_array.Length != other._array.Length)
        {
            return false;
        }

        for (var i = 0; i < _array.Length; i++)
        {
            if (!_array[i].Equals(other._array[i]))
            {
                return false;
            }
        }

        return true;
    }

    public override bool Equals(object? obj) => obj is EquatableArray<T> other && Equals(other);

    public override int GetHashCode()
    {
        if (_array is null)
        {
            return 0;
        }

        var hash = 17;
        foreach (var item in _array)
        {
            hash = (hash * 31) + item.GetHashCode();
        }

        return hash;
    }

    public IEnumerator<T> GetEnumerator() =>
        ((IEnumerable<T>)(_array ?? Array.Empty<T>())).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>
///     A value-equatable snapshot of a source <see cref="Location"/>. Stores only the equatable pieces so
///     it can live on a pipeline model; <see cref="ToLocation"/> rebuilds the real location at report time.
/// </summary>
internal sealed record LocationInfo(string FilePath, TextSpan TextSpan, LinePositionSpan LineSpan)
{
    public Location ToLocation() => Location.Create(FilePath, TextSpan, LineSpan);

    public static LocationInfo? CreateFrom(Location? location)
    {
        if (location?.SourceTree is null)
        {
            return null;
        }

        return new LocationInfo(location.SourceTree.FilePath, location.SourceSpan, location.GetLineSpan().Span);
    }
}

/// <summary>
///     A value-equatable description of a diagnostic to report, deferred until <c>RegisterSourceOutput</c>.
///     Carrying this on the pipeline model (rather than a live <see cref="Diagnostic"/>) keeps the model
///     cacheable. <see cref="ToDiagnostic"/> materialises it.
/// </summary>
internal sealed record DiagnosticInfo(
    DiagnosticDescriptor Descriptor,
    LocationInfo? Location,
    EquatableArray<string> MessageArgs)
{
    public Diagnostic ToDiagnostic()
    {
        var location = Location?.ToLocation() ?? Microsoft.CodeAnalysis.Location.None;
        var args = new List<object?>();
        foreach (var arg in MessageArgs)
        {
            args.Add(arg);
        }

        return Diagnostic.Create(Descriptor, location, args.ToArray());
    }
}
