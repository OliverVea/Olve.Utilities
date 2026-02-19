using System.Diagnostics.CodeAnalysis;

namespace Olve.Utilities.Collections;

/// <summary>
///     Represents a many-to-many lookup between two sets of items.
/// </summary>
/// <typeparam name="TLeft">The type of the left-hand elements.</typeparam>
/// <typeparam name="TRight">The type of the right-hand elements.</typeparam>
public interface IManyToManyLookup<TLeft, TRight> : IEnumerable<KeyValuePair<TLeft, TRight>>
    where TLeft : notnull
    where TRight : notnull
{

    /// <summary>
    ///     Gets all left-hand elements in the lookup.
    /// </summary>
    IEnumerable<TLeft> Lefts { get; }

    /// <summary>
    ///     Gets all right-hand elements in the lookup.
    /// </summary>
    IEnumerable<TRight> Rights { get; }

    /// <summary>
    ///     Determines whether the specified pair exists in the lookup.
    /// </summary>
    /// <param name="left">The left-hand element.</param>
    /// <param name="right">The right-hand element.</param>
    /// <returns><see langword="true" /> if the pair exists; otherwise, <see langword="false" />.</returns>
    bool Contains(TLeft left, TRight right);

    /// <summary>
    ///     Tries to get all right-hand elements associated with the specified left-hand element.
    /// </summary>
    /// <param name="left">The left-hand element.</param>
    /// <param name="rights">When this method returns, contains the associated right-hand elements if found; otherwise, <see langword="null" />.</param>
    /// <returns><see langword="true" /> if the left-hand element was found; otherwise, <see langword="false" />.</returns>
    bool TryGet(TLeft left, [NotNullWhen(true)] out IReadOnlySet<TRight>? rights);

    /// <summary>
    ///     Tries to get all left-hand elements associated with the specified right-hand element.
    /// </summary>
    /// <param name="right">The right-hand element.</param>
    /// <param name="lefts">When this method returns, contains the associated left-hand elements if found; otherwise, <see langword="null" />.</param>
    /// <returns><see langword="true" /> if the right-hand element was found; otherwise, <see langword="false" />.</returns>
    bool TryGet(TRight right, [NotNullWhen(true)] out IReadOnlySet<TLeft>? lefts);

    /// <summary>
    ///     Sets the mapping for the specified left-hand element to the provided right-hand elements.
    ///     Overwrites any existing mappings.
    /// </summary>
    /// <param name="left">The left-hand element.</param>
    /// <param name="rights">The set of right-hand elements to associate.</param>
    void Set(TLeft left, ISet<TRight> rights);

    /// <summary>
    ///     Sets the mapping for the specified right-hand element to the provided left-hand elements.
    ///     Overwrites any existing mappings.
    /// </summary>
    /// <param name="right">The right-hand element.</param>
    /// <param name="lefts">The set of left-hand elements to associate.</param>
    void Set(TRight right, ISet<TLeft> lefts);

    /// <summary>
    ///     Adds or removes a pair to/from the lookup.
    /// </summary>
    /// <param name="left">The left-hand element.</param>
    /// <param name="right">The right-hand element.</param>
    /// <param name="value"><see langword="true" /> to add the pair; <see langword="false" /> to remove it.</param>
    /// <returns>
    ///     <see langword="true" /> if the operation made changes to the lookup; otherwise, <see langword="false" />.
    /// </returns>
    bool Set(TLeft left, TRight right, bool value);

    /// <summary>
    ///     Adds or removes a pair to/from the lookup.
    /// </summary>
    /// <param name="right">The right-hand element.</param>
    /// <param name="left">The left-hand element.</param>
    /// <param name="value"><see langword="true" /> to add the pair; <see langword="false" /> to remove it.</param>
    /// <returns>
    ///     <see langword="true" /> if the operation made changes to the lookup; otherwise, <see langword="false" />.
    /// </returns>
    bool Set(TRight right, TLeft left, bool value);

    /// <summary>
    ///     Clears all mappings in the lookup.
    /// </summary>
    void Clear();

    /// <summary>
    ///     Removes all mappings for the specified left-hand element.
    /// </summary>
    /// <param name="left">The left-hand element.</param>
    /// <returns><see langword="true" /> if mappings were removed; otherwise, <see langword="false" />.</returns>
    bool Remove(TLeft left);

    /// <summary>
    ///     Removes all mappings for the specified right-hand element.
    /// </summary>
    /// <param name="right">The right-hand element.</param>
    /// <returns><see langword="true" /> if mappings were removed; otherwise, <see langword="false" />.</returns>
    bool Remove(TRight right);
}