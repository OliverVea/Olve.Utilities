using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Olve.Operations;

/// <summary>
///     A factory for creating instances of asynchronous operations that implement <see cref="IAsyncOperation{TRequest}" />
///     .
/// </summary>
/// <typeparam name="TOperation">The type of the asynchronous operation to create.</typeparam>
/// <typeparam name="TRequest">The type of the request handled by the operation.</typeparam>
/// <param name="serviceProvider">The <see cref="IServiceProvider" /> used to resolve dependencies for the operation.</param>
[SuppressMessage("Design", "MA0048:File name must match type name")]
public class AsyncOperationFactory<TOperation, TRequest>(IServiceProvider serviceProvider)
    where TOperation : IAsyncOperation<TRequest>
{
    /// <summary>
    ///     Creates a new instance of the specified asynchronous operation type.
    /// </summary>
    /// <returns>A new instance of <typeparamref name="TOperation" />.</returns>
    public TOperation Build() => serviceProvider.GetRequiredService<TOperation>();
}
