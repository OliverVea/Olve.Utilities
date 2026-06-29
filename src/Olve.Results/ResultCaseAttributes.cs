namespace Olve.Results;

/// <summary>
///     Marks a <see langword="static partial" /> factory method on a <see cref="GenerateResultAttribute" />
///     type as a <em>success</em> case. Success cases contribute to <c>Succeeded</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class SuccessCaseAttribute : Attribute;

/// <summary>
///     Marks a <see langword="static partial" /> factory method on a <see cref="GenerateResultAttribute" />
///     type as an <em>error</em> case. Error cases contribute to <c>Failed</c> and carry problems.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class ErrorCaseAttribute : Attribute;

/// <summary>
///     Marks a <see langword="static partial" /> factory method on a <see cref="GenerateResultAttribute" />
///     type as a <em>grey</em> case: a state that is neither success nor failure (e.g. not-found).
///     Grey cases make both <c>Succeeded</c> and <c>Failed</c> <see langword="false" />.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class GreyCaseAttribute : Attribute;
