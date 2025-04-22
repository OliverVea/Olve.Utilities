using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Olve.Operations;

#pragma warning disable MA0026
// Todo: Add warning for direct DI of operations
//       All operations should be created through a factory
#pragma warning restore MA0026

/// <summary>
///     A factory for creating instances of asynchronous operations that implement
///     <see cref="IAsyncOperation{TRequest, TResponse}" />.
/// </summary>
/// <typeparam name="TOperation">The type of the asynchronous operation to create.</typeparam>
/// <typeparam name="TRequest">The type of the request handled by the operation.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the operation.</typeparam>
/// <param name="serviceProvider">The <see cref="IServiceProvider" /> used to resolve dependencies for the operation.</param>
[SuppressMessage("Design", "MA0048:File name must match type name")]
public class AsyncOperationFactory<TOperation, TRequest, TResponse>(
    IServiceProvider serviceProvider
)
    where TOperation : IAsyncOperation<TRequest, TResponse>
{
    /// <summary>
    ///     Creates a new instance of the specified asynchronous operation type.
    /// </summary>
    /// <returns>A new instance of <typeparamref name="TOperation" />.</returns>
    public TOperation Build() => serviceProvider.GetRequiredService<TOperation>();
}
