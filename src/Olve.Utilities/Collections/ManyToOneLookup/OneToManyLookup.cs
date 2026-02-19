using System.Collections;

namespace Olve.Utilities.Collections;

/// <summary>
/// Represents a one-to-many lookup between two sets of items.
/// </summary>
/// <typeparam name="TLeft">The type of the left-hand elements.</typeparam>
/// <typeparam name="TRight">The type of the right-hand elements.</typeparam>
public class OneToManyLookup<TLeft, TRight> : IOneToManyLookup<TLeft, TRight>
    where TLeft : notnull
    where TRight : notnull
{
    private readonly Dictionary<TLeft, HashSet<TRight>> _leftToRights;
    private readonly Dictionary<TRight, TLeft> _rightToLeft;

    /// <summary>
    /// Initializes a new instance of the <see cref="OneToManyLookup{TLeft, TRight}"/> class.
    /// </summary>
    /// <param name="leftComparer">Comparer for left-hand elements.</param>
    /// <param name="rightComparer">Comparer for right-hand elements.</param>
    public OneToManyLookup(IEqualityComparer<TLeft>? leftComparer = null, IEqualityComparer<TRight>? rightComparer = null)
    {
        _leftToRights = new Dictionary<TLeft, HashSet<TRight>>(leftComparer);
        _rightToLeft = new Dictionary<TRight, TLeft>(rightComparer);
    }

    /// <inheritdoc />
    public IEnumerable<TLeft> Lefts => _leftToRights.Keys;

    /// <inheritdoc />
    public IEnumerable<TRight> Rights => _rightToLeft.Keys;

    /// <inheritdoc />
    public bool Contains(TLeft left, TRight right) =>
        _leftToRights.TryGetValue(left, out var rights) && rights.Contains(right);

    /// <inheritdoc />
    public bool TryGet(TLeft left, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out IReadOnlySet<TRight>? rights)
    {
        if (_leftToRights.TryGetValue(left, out var set))
        {
            rights = set;
            return true;
        }
        rights = null;
        return false;
    }

    /// <inheritdoc />
    public bool TryGet(TRight right, [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TLeft left)
    {
        return _rightToLeft.TryGetValue(right, out left);
    }

    /// <inheritdoc />
    public void Set(TLeft left, ISet<TRight> rights)
    {
        if (rights.Count == 0)
        {
            Remove(left);
            return;
        }

        foreach (var right in rights)
        {
            if (_rightToLeft.TryGetValue(right, out var existingLeft) && !EqualityComparer<TLeft>.Default.Equals(existingLeft, left))
            {
                _leftToRights[existingLeft].Remove(right);
            }
            _rightToLeft[right] = left;
        }
        _leftToRights[left] = [..rights];
    }

    /// <inheritdoc />
    public bool Set(TLeft left, TRight right, bool value)
    {
        if (value)
        {
            if (Contains(left, right)) return false;
            if (_rightToLeft.TryGetValue(right, out var existingLeft))
            {
                _leftToRights[existingLeft].Remove(right);
            }
            _rightToLeft[right] = left;
            if (!_leftToRights.TryGetValue(left, out var rights))
            {
                rights = new HashSet<TRight>();
                _leftToRights[left] = rights;
            }
            rights.Add(right);
            return true;
        }
        return Remove(left, right);
    }

    /// <inheritdoc />
    public bool Remove(TLeft left)
    {
        if (!_leftToRights.TryGetValue(left, out var rights)) return false;
        foreach (var right in rights)
        {
            _rightToLeft.Remove(right);
        }
        _leftToRights.Remove(left);
        return true;
    }

    /// <inheritdoc />
    public bool Remove(TRight right)
    {
        if (!_rightToLeft.Remove(right, out var left)) return false;

        _leftToRights[left].Remove(right);

        if (_leftToRights[left].Count == 0)
        {
            _leftToRights.Remove(left);
        }

        return true;
    }

    /// <inheritdoc />
    public bool Remove(TLeft left, TRight right)
    {
        if (!_leftToRights.TryGetValue(left, out var rights) || !rights.Remove(right)) return false;
        _rightToLeft.Remove(right);
        if (rights.Count == 0) _leftToRights.Remove(left);
        return true;
    }

    /// <inheritdoc />
    public void Clear()
    {
        _leftToRights.Clear();
        _rightToLeft.Clear();
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<TLeft, IReadOnlySet<TRight>>> GetEnumerator() => _leftToRights.Select(pair => new KeyValuePair<TLeft, IReadOnlySet<TRight>>(pair.Key, pair.Value)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}