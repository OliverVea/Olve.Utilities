namespace Olve.Results;

public readonly partial struct Result
{
    /// <summary>
    /// Chains two result-producing functions, returning a combined result if both succeed.
    /// </summary>
    /// <param name="a">The first function that produces a result.</param>
    /// <param name="b">The second function that produces a result.</param>
    /// <typeparam name="T1">The type of the first result value.</typeparam>
    /// <typeparam name="T2">The type of the second result value.</typeparam>
    /// <returns>The result of the second function if both succeed; otherwise, the first encountered problem.</returns>
    public static Result<T2> Chain<T1, T2>(Func<Result<T1>> a, Func<T1, Result<T2>> b)
    {
        var resultA = a();
        if (!resultA.Succeeded)
        {
            return resultA.Problems!;
        }

        var resultB = b(resultA.Value!);
        if (!resultB.Succeeded)
        {
            return resultB.Problems!;
        }

        return resultB.Value!;
    }

    /// <summary>
    /// Chains three result-producing functions, returning a combined result if all succeed.
    /// </summary>
    /// <param name="a">The first function that produces a result.</param>
    /// <param name="b">The second function that produces a result.</param>
    /// <param name="c">The third function that produces a result.</param>
    /// <typeparam name="T1">The type of the first result value.</typeparam>
    /// <typeparam name="T2">The type of the second result value.</typeparam>
    /// <typeparam name="T3">The type of the third result value.</typeparam>
    /// <returns>The result of the third function if all succeed; otherwise, the first encountered problem.</returns>
    public static Result<T3> Chain<T1, T2, T3>(
        Func<Result<T1>> a,
        Func<T1, Result<T2>> b,
        Func<T2, Result<T3>> c
    )
    {
        var resultA = a();
        if (!resultA.Succeeded)
        {
            return resultA.Problems!;
        }

        var resultB = b(resultA.Value!);
        if (!resultB.Succeeded)
        {
            return resultB.Problems!;
        }

        var resultC = c(resultB.Value!);
        if (!resultC.Succeeded)
        {
            return resultC.Problems!;
        }

        return resultC.Value!;
    }

    /// <summary>
    /// Chains four result-producing functions, returning a combined result if all succeed.
    /// </summary>
    /// <param name="a">The first function that produces a result.</param>
    /// <param name="b">The second function that produces a result.</param>
    /// <param name="c">The third function that produces a result.</param>
    /// <param name="d">The fourth function that produces a result.</param>
    /// <typeparam name="T1">The type of the first result value.</typeparam>
    /// <typeparam name="T2">The type of the second result value.</typeparam>
    /// <typeparam name="T3">The type of the third result value.</typeparam>
    /// <typeparam name="T4">The type of the fourth result value.</typeparam>
    /// <returns>The result of the fourth function if all succeed; otherwise, the first encountered problem.</returns>
    public static Result<T4> Chain<T1, T2, T3, T4>(
        Func<Result<T1>> a,
        Func<T1, Result<T2>> b,
        Func<T2, Result<T3>> c,
        Func<T3, Result<T4>> d
    )
    {
        var resultA = a();
        if (!resultA.Succeeded)
        {
            return resultA.Problems!;
        }

        var resultB = b(resultA.Value!);
        if (!resultB.Succeeded)
        {
            return resultB.Problems!;
        }

        var resultC = c(resultB.Value!);
        if (!resultC.Succeeded)
        {
            return resultC.Problems!;
        }

        var resultD = d(resultC.Value!);
        if (!resultD.Succeeded)
        {
            return resultD.Problems!;
        }

        return resultD.Value!;
    }
}
