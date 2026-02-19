using System.Collections;

namespace Olve.Utilities.Collections;

/// <summary>
///     Represents a many-to-many collection of value pairs.
/// </summary>
/// <typeparam name="TLeft">The type of the left-hand elements.</typeparam>
/// <typeparam name="TRight">The type of the right-hand elements.</typeparam>
public class ManyToManyLookup<TLeft, TRight> : IManyToManyLookup<TLeft, TRight>
    where TLeft : notnull
    where TRight : notnull
{
    private readonly Dictionary<TLeft, HashSet<TRight>> _leftToRights;
    private readonly Dictionary<TRight, HashSet<TLeft>> _rightToLefts;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ManyToManyLookup{TLeft, TRight}" /> class.
    /// </summary>
    /// <param name="initialItems">The initial items to populate the lookup with.</param>
    /// <param name="leftComparer">The equality comparer for left-hand elements.</param>
    /// <param name="rightComparer">The equality comparer for right-hand elements.</param>
    public ManyToManyLookup(IEnumerable<KeyValuePair<TLeft, TRight>>? initialItems = null,
        IEqualityComparer<TLeft>? leftComparer = null,
        IEqualityComparer<TRight>? rightComparer = null)
    {
        _leftToRights = new Dictionary<TLeft, HashSet<TRight>>(leftComparer);
        _rightToLefts = new Dictionary<TRight, HashSet<TLeft>>(rightComparer);

        if (initialItems != null)
        {
            foreach (var (left, right) in initialItems)
            {
                Set(left, right, true);
            }
        }
    }

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
    public bool TryGet(TRight right, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out IReadOnlySet<TLeft>? lefts)
    {
        if (_rightToLefts.TryGetValue(right, out var set))
        {
            lefts = set;
            return true;
        }
        lefts = null;
        return false;
    }

    /// <inheritdoc />
    public void Set(TLeft left, ISet<TRight> rights)
    {
        if (_leftToRights.TryGetValue(left, out var existingRights))
        {
            foreach (var right in existingRights)
            {
                _rightToLefts[right]
                    .Remove(left);
                if (_rightToLefts[right].Count == 0)
                {
                    _rightToLefts.Remove(right);
                }
            }
        }

        if (rights.Count == 0)
        {
            _leftToRights.Remove(left);
            return;
        }

        _leftToRights[left] = [..rights];
        foreach (var right in rights)
        {
            if (!_rightToLefts.TryGetValue(right, out var lefts))
            {
                lefts = [];
                _rightToLefts[right] = lefts;
            }

            lefts.Add(left);
        }
    }

    /// <inheritdoc />
    public void Set(TRight right, ISet<TLeft> lefts)
    {
        if (_rightToLefts.TryGetValue(right, out var existingLefts))
        {
            foreach (var left in existingLefts)
            {
                _leftToRights[left]
                    .Remove(right);
                if (_leftToRights[left].Count == 0)
                {
                    _leftToRights.Remove(left);
                }
            }
        }

        if (lefts.Count == 0)
        {
            _rightToLefts.Remove(right);
            return;
        }

        _rightToLefts[right] = [..lefts];
        foreach (var left in lefts)
        {
            if (!_leftToRights.TryGetValue(left, out var rights))
            {
                rights = [];
                _leftToRights[left] = rights;
            }

            rights.Add(right);
        }
    }

    /// <inheritdoc />
    public bool Set(TRight right, TLeft left, bool value) => Set(left, right, value);

    /// <inheritdoc />
    public bool Set(TLeft left, TRight right, bool value)
    {
        if (value)
        {
            if (Contains(left, right))
            {
                return false;
            }

            if (!_leftToRights.TryGetValue(left, out var rights))
            {
                rights = [];
                _leftToRights[left] = rights;
            }

            rights.Add(right);

            if (!_rightToLefts.TryGetValue(right, out var lefts))
            {
                lefts = [];
                _rightToLefts[right] = lefts;
            }

            lefts.Add(left);

            return true;
        }

        if (!Contains(left, right))
        {
            return false;
        }

        _leftToRights[left]
            .Remove(right);
        if (_leftToRights[left].Count == 0)
        {
            _leftToRights.Remove(left);
        }

        _rightToLefts[right]
            .Remove(left);
        if (_rightToLefts[right].Count == 0)
        {
            _rightToLefts.Remove(right);
        }

        return true;
    }

    /// <inheritdoc />
    public void Clear()
    {
        _leftToRights.Clear();
        _rightToLefts.Clear();
    }

    /// <inheritdoc />
    public bool Remove(TLeft left)
    {
        if (!_leftToRights.TryGetValue(left, out var rights))
        {
            return false;
        }

        foreach (var right in rights)
        {
            _rightToLefts[right]
                .Remove(left);
            if (_rightToLefts[right].Count == 0)
            {
                _rightToLefts.Remove(right);
            }
        }

        _leftToRights.Remove(left);
        return true;
    }

    /// <inheritdoc />
    public bool Remove(TRight right)
    {
        if (!_rightToLefts.TryGetValue(right, out var lefts))
        {
            return false;
        }

        foreach (var left in lefts)
        {
            _leftToRights[left]
                .Remove(right);
            if (_leftToRights[left].Count == 0)
            {
                _leftToRights.Remove(left);
            }
        }

        _rightToLefts.Remove(right);
        return true;
    }

    /// <inheritdoc />
    public IEnumerable<TLeft> Lefts => _leftToRights.Keys;

    /// <inheritdoc />
    public IEnumerable<TRight> Rights => _rightToLefts.Keys;

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<TLeft, TRight>> GetEnumerator()
    {
        foreach (var (left, rights) in _leftToRights)
        {
            foreach (var right in rights)
            {
                yield return new KeyValuePair<TLeft, TRight>(left, right);
            }
        }
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}