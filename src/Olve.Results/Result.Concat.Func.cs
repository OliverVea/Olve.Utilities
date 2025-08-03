namespace Olve.Results;

public readonly partial struct Result
{
    /// <summary>
    /// Invokes two functions that produce <see cref="Result{T}"/> values and concatenates them into a tuple
    /// by delegating to the non‑Func <see cref="Concat{T1,T2}(Result{T1},Result{T2})"/> overload.
    /// </summary>
    /// <typeparam name="T1">The type of the first result value.</typeparam>
    /// <typeparam name="T2">The type of the second result value.</typeparam>
    /// <param name="a">A function that produces the first <see cref="Result{T1}"/>.</param>
    /// <param name="b">A function that produces the second <see cref="Result{T2}"/>.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing a tuple of the two values if both succeeded;
    /// otherwise a <see cref="ResultProblemCollection"/> containing all aggregated problems.
    /// </returns>
    public static Result<(T1, T2)> Concat<T1, T2>(
        Func<Result<T1>> a,
        Func<Result<T2>> b)
        => Concat(a(), b());

    /// <summary>
    /// Invokes three functions that produce <see cref="Result{T}"/> values and concatenates them into a tuple
    /// by delegating to the non‑Func <see cref="Concat{T1,T2,T3}(Result{T1},Result{T2},Result{T3})"/> overload.
    /// </summary>
    /// <typeparam name="T1">The type of the first result value.</typeparam>
    /// <typeparam name="T2">The type of the second result value.</typeparam>
    /// <typeparam name="T3">The type of the third result value.</typeparam>
    /// <param name="a">A function that produces the first <see cref="Result{T1}"/>.</param>
    /// <param name="b">A function that produces the second <see cref="Result{T2}"/>.</param>
    /// <param name="c">A function that produces the third <see cref="Result{T3}"/>.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing a tuple of the three values if all succeeded;
    /// otherwise a <see cref="ResultProblemCollection"/> containing all aggregated problems.
    /// </returns>
    public static Result<(T1, T2, T3)> Concat<T1, T2, T3>(
        Func<Result<T1>> a,
        Func<Result<T2>> b,
        Func<Result<T3>> c)
        => Concat(a(), b(), c());

    /// <summary>
    /// Invokes four functions that produce <see cref="Result{T}"/> values and concatenates them into a tuple
    /// by delegating to the non‑Func <see cref="Concat{T1,T2,T3,T4}(Result{T1},Result{T2},Result{T3},Result{T4})"/> overload.
    /// </summary>
    /// <typeparam name="T1">The type of the first result value.</typeparam>
    /// <typeparam name="T2">The type of the second result value.</typeparam>
    /// <typeparam name="T3">The type of the third result value.</typeparam>
    /// <typeparam name="T4">The type of the fourth result value.</typeparam>
    /// <param name="a">A function that produces the first <see cref="Result{T1}"/>.</param>
    /// <param name="b">A function that produces the second <see cref="Result{T2}"/>.</param>
    /// <param name="c">A function that produces the third <see cref="Result{T3}"/>.</param>
    /// <param name="d">A function that produces the fourth <see cref="Result{T4}"/>.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing a tuple of the four values if all succeeded;
    /// otherwise a <see cref="ResultProblemCollection"/> containing all aggregated problems.
    /// </returns>
    public static Result<(T1, T2, T3, T4)> Concat<T1, T2, T3, T4>(
        Func<Result<T1>> a,
        Func<Result<T2>> b,
        Func<Result<T3>> c,
        Func<Result<T4>> d)
        => Concat(a(), b(), c(), d());

    /// <summary>
    /// Invokes five functions that produce <see cref="Result{T}"/> values and concatenates them into a tuple
    /// by delegating to the non‑Func <see cref="Concat{T1,T2,T3,T4,T5}(Result{T1},Result{T2},Result{T3},Result{T4},Result{T5})"/> overload.
    /// </summary>
    /// <typeparam name="T1">The type of the first result value.</typeparam>
    /// <typeparam name="T2">The type of the second result value.</typeparam>
    /// <typeparam name="T3">The type of the third result value.</typeparam>
    /// <typeparam name="T4">The type of the fourth result value.</typeparam>
    /// <typeparam name="T5">The type of the fifth result value.</typeparam>
    /// <param name="a">A function that produces the first <see cref="Result{T1}"/>.</param>
    /// <param name="b">A function that produces the second <see cref="Result{T2}"/>.</param>
    /// <param name="c">A function that produces the third <see cref="Result{T3}"/>.</param>
    /// <param name="d">A function that produces the fourth <see cref="Result{T4}"/>.</param>
    /// <param name="e">A function that produces the fifth <see cref="Result{T5}"/>.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing a tuple of the five values if all succeeded;
    /// otherwise a <see cref="ResultProblemCollection"/> containing all aggregated problems.
    /// </returns>
    public static Result<(T1, T2, T3, T4, T5)> Concat<T1, T2, T3, T4, T5>(
        Func<Result<T1>> a,
        Func<Result<T2>> b,
        Func<Result<T3>> c,
        Func<Result<T4>> d,
        Func<Result<T5>> e)
        => Concat(a(), b(), c(), d(), e());

    /// <summary>
    /// Invokes six functions that produce <see cref="Result{T}"/> values and concatenates them into a tuple
    /// by delegating to the non‑Func <see cref="Concat{T1,T2,T3,T4,T5,T6}(Result{T1},Result{T2},Result{T3},Result{T4},Result{T5},Result{T6})"/> overload.
    /// </summary>
    /// <typeparam name="T1">The type of the first result value.</typeparam>
    /// <typeparam name="T2">The type of the second result value.</typeparam>
    /// <typeparam name="T3">The type of the third result value.</typeparam>
    /// <typeparam name="T4">The type of the fourth result value.</typeparam>
    /// <typeparam name="T5">The type of the fifth result value.</typeparam>
    /// <typeparam name="T6">The type of the sixth result value.</typeparam>
    /// <param name="a">A function that produces the first <see cref="Result{T1}"/>.</param>
    /// <param name="b">A function that produces the second <see cref="Result{T2}"/>.</param>
    /// <param name="c">A function that produces the third <see cref="Result{T3}"/>.</param>
    /// <param name="d">A function that produces the fourth <see cref="Result{T4}"/>.</param>
    /// <param name="e">A function that produces the fifth <see cref="Result{T5}"/>.</param>
    /// <param name="f">A function that produces the sixth <see cref="Result{T6}"/>.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing a tuple of the six values if all succeeded;
    /// otherwise a <see cref="ResultProblemCollection"/> containing all aggregated problems.
    /// </returns>
    public static Result<(T1, T2, T3, T4, T5, T6)> Concat<T1, T2, T3, T4, T5, T6>(
        Func<Result<T1>> a,
        Func<Result<T2>> b,
        Func<Result<T3>> c,
        Func<Result<T4>> d,
        Func<Result<T5>> e,
        Func<Result<T6>> f)
        => Concat(a(), b(), c(), d(), e(), f());
}