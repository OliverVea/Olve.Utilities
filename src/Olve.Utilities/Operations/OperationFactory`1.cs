using Microsoft.Extensions.DependencyInjection;

namespace Olve.Utilities.Operations;

/// <summary>
///     A factory for creating instances of operations that implement <see cref="IOperation{TRequest}" />.
/// </summary>
/// <typeparam name="TOperation">The type of the operation to create.</typeparam>
/// <typeparam name="TRequest">The type of the request handled by the operation.</typeparam>
/// <param name="serviceProvider">The <see cref="IServiceProvider" /> used to resolve dependencies for the operation.</param>
public class OperationFactory<TOperation, TRequest>(IServiceProvider serviceProvider)
    where TOperation : IOperation<TRequest>
{
    /// <summary>
    ///     Creates a new instance of the specified operation type.
    /// </summary>
    /// <returns>A new instance of <typeparamref name="TOperation" />.</returns>
    public TOperation Build() => serviceProvider.GetRequiredService<TOperation>();
}