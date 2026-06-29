namespace Olve.Results;

/// <summary>
///     Marks a <see langword="partial" /> <see langword="struct" /> for which Olve.Results generates
///     multi-state result boilerplate. Each state is declared as a <see langword="static partial" />
///     factory method annotated with <see cref="SuccessCaseAttribute" />, <see cref="ErrorCaseAttribute" />
///     or <see cref="GreyCaseAttribute" />. The generator emits the factory bodies, a state discriminator,
///     per-case <c>Is…</c> predicates, <c>Succeeded</c>/<c>Failed</c>, and an exhaustive <c>Match</c>.
/// </summary>
/// <remarks>
///     Generalizes the hand-written <see cref="DeletionResult" /> pattern. See issue #55.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
public sealed class GenerateResultAttribute : Attribute;
