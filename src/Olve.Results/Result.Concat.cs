namespace Olve.Results;

public readonly partial struct Result
{
    /// <summary>
    /// Concatenates two result-producing functions, returning a combined result if both succeed.
    /// </summary>
    /// <typeparam name="T1">The type of the first result value.</typeparam>
    /// <typeparam name="T2">The type of the second result value.</typeparam>
    /// <param name="a">A function that produces the first result.</param>
    /// <param name="b">A function that produces the second result.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing a tuple of the two values if both succeed;
    /// otherwise, the first encountered problem.
    /// </returns>
    public static Result<(T1, T2)> Concat<T1, T2>(Func<Result<T1>> a, Func<Result<T2>> b)
    {
        var resultA = a();
        if (!resultA.Succeeded)
        {
            return resultA.Problems!;
        }

        var resultB = b();
        if (!resultB.Succeeded)
        {
            return resultB.Problems!;
        }

        return (resultA.Value!, resultB.Value!);
    }

    /// <summary>
    /// Concatenates three result-producing functions, returning a combined result if all succeed.
    /// </summary>
    /// <typeparam name="T1">The type of the first result value.</typeparam>
    /// <typeparam name="T2">The type of the second result value.</typeparam>
    /// <typeparam name="T3">The type of the third result value.</typeparam>
    /// <param name="a">A function that produces the first result.</param>
    /// <param name="b">A function that produces the second result.</param>
    /// <param name="c">A function that produces the third result.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing a tuple of the three values if all succeed;
    /// otherwise, the first encountered problem.
    /// </returns>
    public static Result<(T1, T2, T3)> Concat<T1, T2, T3>(
        Func<Result<T1>> a,
        Func<Result<T2>> b,
        Func<Result<T3>> c
    )
    {
        var resultA = a();
        if (!resultA.Succeeded)
        {
            return resultA.Problems!;
        }

        var resultB = b();
        if (!resultB.Succeeded)
        {
            return resultB.Problems!;
        }

        var resultC = c();
        if (!resultC.Succeeded)
        {
            return resultC.Problems!;
        }

        return (resultA.Value!, resultB.Value!, resultC.Value!);
    }

    /// <summary>
    /// Concatenates four result-producing functions, returning a combined result if all succeed.
    /// </summary>
    /// <typeparam name="T1">The type of the first result value.</typeparam>
    /// <typeparam name="T2">The type of the second result value.</typeparam>
    /// <typeparam name="T3">The type of the third result value.</typeparam>
    /// <typeparam name="T4">The type of the fourth result value.</typeparam>
    /// <param name="a">A function that produces the first result.</param>
    /// <param name="b">A function that produces the second result.</param>
    /// <param name="c">A function that produces the third result.</param>
    /// <param name="d">A function that produces the fourth result.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing a tuple of the four values if all succeed;
    /// otherwise, the first encountered problem.
    /// </returns>
    public static Result<(T1, T2, T3, T4)> Concat<T1, T2, T3, T4>(
        Func<Result<T1>> a,
        Func<Result<T2>> b,
        Func<Result<T3>> c,
        Func<Result<T4>> d
    )
    {
        var resultA = a();
        if (!resultA.Succeeded)
        {
            return resultA.Problems!;
        }

        var resultB = b();
        if (!resultB.Succeeded)
        {
            return resultB.Problems!;
        }

        var resultC = c();
        if (!resultC.Succeeded)
        {
            return resultC.Problems!;
        }

        var resultD = d();
        if (!resultD.Succeeded)
        {
            return resultD.Problems!;
        }

        return (resultA.Value!, resultB.Value!, resultC.Value!, resultD.Value!);
    }

    /// <summary>
    /// Concatenates five result-producing functions, returning a combined result if all succeed.
    /// </summary>
    /// <typeparam name="T1">The type of the first result value.</typeparam>
    /// <typeparam name="T2">The type of the second result value.</typeparam>
    /// <typeparam name="T3">The type of the third result value.</typeparam>
    /// <typeparam name="T4">The type of the fourth result value.</typeparam>
    /// <typeparam name="T5">The type of the fifth result value.</typeparam>
    /// <param name="a">A function that produces the first result.</param>
    /// <param name="b">A function that produces the second result.</param>
    /// <param name="c">A function that produces the third result.</param>
    /// <param name="d">A function that produces the fourth result.</param>
    /// <param name="e">A function that produces the fifth result.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing a tuple of the five values if all succeed;
    /// otherwise, the first encountered problem.
    /// </returns>
    public static Result<(T1, T2, T3, T4, T5)> Concat<T1, T2, T3, T4, T5>(
        Func<Result<T1>> a,
        Func<Result<T2>> b,
        Func<Result<T3>> c,
        Func<Result<T4>> d,
        Func<Result<T5>> e
    )
    {
        var resultA = a();
        if (!resultA.Succeeded)
        {
            return resultA.Problems!;
        }

        var resultB = b();
        if (!resultB.Succeeded)
        {
            return resultB.Problems!;
        }

        var resultC = c();
        if (!resultC.Succeeded)
        {
            return resultC.Problems!;
        }

        var resultD = d();
        if (!resultD.Succeeded)
        {
            return resultD.Problems!;
        }

        var resultE = e();
        if (!resultE.Succeeded)
        {
            return resultE.Problems!;
        }

        return (resultA.Value!, resultB.Value!, resultC.Value!, resultD.Value!, resultE.Value!);
    }

    /// <summary>
    /// Concatenates six result-producing functions, returning a combined result if all succeed.
    /// </summary>
    /// <typeparam name="T1">The type of the first result value.</typeparam>
    /// <typeparam name="T2">The type of the second result value.</typeparam>
    /// <typeparam name="T3">The type of the third result value.</typeparam>
    /// <typeparam name="T4">The type of the fourth result value.</typeparam>
    /// <typeparam name="T5">The type of the fifth result value.</typeparam>
    /// <typeparam name="T6">The type of the sixth result value.</typeparam>
    /// <param name="a">A function that produces the first result.</param>
    /// <param name="b">A function that produces the second result.</param>
    /// <param name="c">A function that produces the third result.</param>
    /// <param name="d">A function that produces the fourth result.</param>
    /// <param name="e">A function that produces the fifth result.</param>
    /// <param name="f">A function that produces the sixth result.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing a tuple of the six values if all succeed;
    /// otherwise, the first encountered problem.
    /// </returns>
    public static Result<(T1, T2, T3, T4, T5, T6)> Concat<T1, T2, T3, T4, T5, T6>(
        Func<Result<T1>> a,
        Func<Result<T2>> b,
        Func<Result<T3>> c,
        Func<Result<T4>> d,
        Func<Result<T5>> e,
        Func<Result<T6>> f
    )
    {
        var resultA = a();
        if (!resultA.Succeeded)
        {
            return resultA.Problems!;
        }

        var resultB = b();
        if (!resultB.Succeeded)
        {
            return resultB.Problems!;
        }

        var resultC = c();
        if (!resultC.Succeeded)
        {
            return resultC.Problems!;
        }

        var resultD = d();
        if (!resultD.Succeeded)
        {
            return resultD.Problems!;
        }

        var resultE = e();
        if (!resultE.Succeeded)
        {
            return resultE.Problems!;
        }

        var resultF = f();
        if (!resultF.Succeeded)
        {
            return resultF.Problems!;
        }

        return (
            resultA.Value!,
            resultB.Value!,
            resultC.Value!,
            resultD.Value!,
            resultE.Value!,
            resultF.Value!
        );
    }
}
