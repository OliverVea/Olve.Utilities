﻿using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Olve.Operations;

/// <summary>
///     A factory for creating instances of operations that implement <see cref="IOperation{TRequest, TResponse}" />.
/// </summary>
/// <typeparam name="TOperation">The type of the operation to create.</typeparam>
/// <typeparam name="TRequest">The type of the request handled by the operation.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the operation.</typeparam>
/// <param name="serviceProvider">The <see cref="IServiceProvider" /> used to resolve dependencies for the operation.</param>
[SuppressMessage("Design", "MA0048:File name must match type name")]
public class OperationFactory<TOperation, TRequest, TResponse>(IServiceProvider serviceProvider)
    where TOperation : IOperation<TRequest, TResponse>
{
    /// <summary>
    ///     Creates a new instance of the specified operation type.
    /// </summary>
    /// <returns>A new instance of <typeparamref name="TOperation" />.</returns>
    public TOperation Build() => serviceProvider.GetRequiredService<TOperation>();
}