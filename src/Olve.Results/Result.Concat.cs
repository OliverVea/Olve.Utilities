namespace Olve.Results;

public readonly partial struct Result
{
    /// <summary>
    /// Concatenates two <see cref="Result{T}"/> values into a tuple if both succeed,
    /// or aggregates all problems if one or both fail.
    /// </summary>
    /// <typeparam name="T1">The type of the first result value.</typeparam>
    /// <typeparam name="T2">The type of the second result value.</typeparam>
    /// <param name="a">The first <see cref="Result{T1}"/> value.</param>
    /// <param name="b">The second <see cref="Result{T2}"/> value.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing a tuple of the two values if both succeeded;
    /// otherwise a <see cref="ResultProblemCollection"/> containing all validation problems.
    /// </returns>
    public static Result<(T1, T2)> Concat<T1, T2>(Result<T1> a, Result<T2> b)
    {
        if (a.Succeeded && b.Succeeded)
            return (a.Value!, b.Value!);

        var problems = new ResultProblemCollection();
        if (a.TryPickProblems(out var p1))
            problems.Append(p1);
        if (b.TryPickProblems(out var p2))
            problems.Append(p2);

        return problems;
    }

    /// <summary>
    /// Concatenates three <see cref="Result{T}"/> values into a tuple if all succeed,
    /// or aggregates all problems if any fail.
    /// </summary>
    /// <typeparam name="T1">The type of the first result value.</typeparam>
    /// <typeparam name="T2">The type of the second result value.</typeparam>
    /// <typeparam name="T3">The type of the third result value.</typeparam>
    /// <param name="a">The first <see cref="Result{T1}"/> value.</param>
    /// <param name="b">The second <see cref="Result{T2}"/> value.</param>
    /// <param name="c">The third <see cref="Result{T3}"/> value.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing a tuple of the three values if all succeeded;
    /// otherwise a <see cref="ResultProblemCollection"/> containing all validation problems.
    /// </returns>
    public static Result<(T1, T2, T3)> Concat<T1, T2, T3>(
        Result<T1> a,
        Result<T2> b,
        Result<T3> c)
    {
        if (a.Succeeded && b.Succeeded && c.Succeeded)
            return (a.Value!, b.Value!, c.Value!);

        var problems = new ResultProblemCollection();
        if (a.TryPickProblems(out var p1))
            problems.Append(p1);
        if (b.TryPickProblems(out var p2))
            problems.Append(p2);
        if (c.TryPickProblems(out var p3))
            problems.Append(p3);

        return problems;
    }

    /// <summary>
    /// Concatenates four <see cref="Result{T}"/> values into a tuple if all succeed,
    /// or aggregates all problems if any fail.
    /// </summary>
    /// <typeparam name="T1">The type of the first result value.</typeparam>
    /// <typeparam name="T2">The type of the second result value.</typeparam>
    /// <typeparam name="T3">The type of the third result value.</typeparam>
    /// <typeparam name="T4">The type of the fourth result value.</typeparam>
    /// <param name="a">The first <see cref="Result{T1}"/> value.</param>
    /// <param name="b">The second <see cref="Result{T2}"/> value.</param>
    /// <param name="c">The third <see cref="Result{T3}"/> value.</param>
    /// <param name="d">The fourth <see cref="Result{T4}"/> value.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing a tuple of the four values if all succeeded;
    /// otherwise a <see cref="ResultProblemCollection"/> containing all validation problems.
    /// </returns>
    public static Result<(T1, T2, T3, T4)> Concat<T1, T2, T3, T4>(
        Result<T1> a,
        Result<T2> b,
        Result<T3> c,
        Result<T4> d)
    {
        if (a.Succeeded && b.Succeeded && c.Succeeded && d.Succeeded)
            return (a.Value!, b.Value!, c.Value!, d.Value!);

        var problems = new ResultProblemCollection();
        if (a.TryPickProblems(out var p1))
            problems.Append(p1);
        if (b.TryPickProblems(out var p2))
            problems.Append(p2);
        if (c.TryPickProblems(out var p3))
            problems.Append(p3);
        if (d.TryPickProblems(out var p4))
            problems.Append(p4);

        return problems;
    }

    /// <summary>
    /// Concatenates five <see cref="Result{T}"/> values into a tuple if all succeed,
    /// or aggregates all problems if any fail.
    /// </summary>
    /// <typeparam name="T1">The type of the first result value.</typeparam>
    /// <typeparam name="T2">The type of the second result value.</typeparam>
    /// <typeparam name="T3">The type of the third result value.</typeparam>
    /// <typeparam name="T4">The type of the fourth result value.</typeparam>
    /// <typeparam name="T5">The type of the fifth result value.</typeparam>
    /// <param name="a">The first <see cref="Result{T1}"/> value.</param>
    /// <param name="b">The second <see cref="Result{T2}"/> value.</param>
    /// <param name="c">The third <see cref="Result{T3}"/> value.</param>
    /// <param name="d">The fourth <see cref="Result{T4}"/> value.</param>
    /// <param name="e">The fifth <see cref="Result{T5}"/> value.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing a tuple of the five values if all succeeded;
    /// otherwise a <see cref="ResultProblemCollection"/> containing all validation problems.
    /// </returns>
    public static Result<(T1, T2, T3, T4, T5)> Concat<T1, T2, T3, T4, T5>(
        Result<T1> a,
        Result<T2> b,
        Result<T3> c,
        Result<T4> d,
        Result<T5> e)
    {
        if (a.Succeeded && b.Succeeded && c.Succeeded && d.Succeeded && e.Succeeded)
            return (a.Value!, b.Value!, c.Value!, d.Value!, e.Value!);

        var problems = new ResultProblemCollection();
        if (a.TryPickProblems(out var p1))
            problems.Append(p1);
        if (b.TryPickProblems(out var p2))
            problems.Append(p2);
        if (c.TryPickProblems(out var p3))
            problems.Append(p3);
        if (d.TryPickProblems(out var p4))
            problems.Append(p4);
        if (e.TryPickProblems(out var p5))
            problems.Append(p5);

        return problems;
    }

    /// <summary>
    /// Concatenates six <see cref="Result{T}"/> values into a tuple if all succeed,
    /// or aggregates all problems if any fail.
    /// </summary>
    /// <typeparam name="T1">The type of the first result value.</typeparam>
    /// <typeparam name="T2">The type of the second result value.</typeparam>
    /// <typeparam name="T3">The type of the third result value.</typeparam>
    /// <typeparam name="T4">The type of the fourth result value.</typeparam>
    /// <typeparam name="T5">The type of the fifth result value.</typeparam>
    /// <typeparam name="T6">The type of the sixth result value.</typeparam>
    /// <param name="a">The first <see cref="Result{T1}"/> value.</param>
    /// <param name="b">The second <see cref="Result{T2}"/> value.</param>
    /// <param name="c">The third <see cref="Result{T3}"/> value.</param>
    /// <param name="d">The fourth <see cref="Result{T4}"/> value.</param>
    /// <param name="e">The fifth <see cref="Result{T5}"/> value.</param>
    /// <param name="f">The sixth <see cref="Result{T6}"/> value.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing a tuple of the six values if all succeeded;
    /// otherwise a <see cref="ResultProblemCollection"/> containing all validation problems.
    /// </returns>
    public static Result<(T1, T2, T3, T4, T5, T6)> Concat<T1, T2, T3, T4, T5, T6>(
        Result<T1> a,
        Result<T2> b,
        Result<T3> c,
        Result<T4> d,
        Result<T5> e,
        Result<T6> f)
    {
        if (a.Succeeded && b.Succeeded && c.Succeeded && d.Succeeded && e.Succeeded && f.Succeeded)
            return (a.Value!, b.Value!, c.Value!, d.Value!, e.Value!, f.Value!);

        var problems = new ResultProblemCollection();
        if (a.TryPickProblems(out var p1))
            problems.Append(p1);
        if (b.TryPickProblems(out var p2))
            problems.Append(p2);
        if (c.TryPickProblems(out var p3))
            problems.Append(p3);
        if (d.TryPickProblems(out var p4))
            problems.Append(p4);
        if (e.TryPickProblems(out var p5))
            problems.Append(p5);
        if (f.TryPickProblems(out var p6))
            problems.Append(p6);

        return problems;
    }
}