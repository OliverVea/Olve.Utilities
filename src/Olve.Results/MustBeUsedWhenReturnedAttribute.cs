namespace Olve.Results;

/// <summary>
///     Marks a type whose values must always be observed when returned from a method.
///     The Olve.Results analyzer (<c>ORES001</c>) reports a diagnostic when a method that returns an
///     annotated type (directly or wrapped in <see cref="System.Threading.Tasks.Task{TResult}"/> /
///     <see cref="System.Threading.Tasks.ValueTask{TResult}"/>) is invoked as a statement without
///     observing the result.
/// </summary>
/// <remarks>
///     Applied to <see cref="Result"/>, <see cref="Result{T}"/> and <see cref="DeletionResult"/>.
///     Dependent projects can apply it to their own result-like types. To intentionally ignore a
///     result, use a discard (<c>_ = expression;</c>) or call <c>DiscardResult()</c>.
/// </remarks>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = false)]
public sealed class MustBeUsedWhenReturnedAttribute : Attribute;
