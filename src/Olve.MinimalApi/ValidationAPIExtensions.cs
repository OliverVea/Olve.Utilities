using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Olve.Validation;

namespace Olve.MinimalApi;

/// <summary>
/// Extension methods to add validation filters for minimal API endpoints.
/// </summary>

public static class ValidationApiExtensions
{
    /// <summary>
    /// Adds a validation filter that creates a new <typeparamref name="TValidator"/> to validate requests of type <typeparamref name="TRequest"/>.
    /// </summary>
    /// <typeparam name="TRequest">The type of requests to validate.</typeparam>
    /// <typeparam name="TValidator">The validator type to instantiate.</typeparam>
    /// <param name="builder">The route handler builder to configure.</param>
    /// <returns>The configured <see cref="RouteHandlerBuilder"/> with validation filter.</returns>
    public static RouteHandlerBuilder WithValidation<TRequest, TValidator>(this RouteHandlerBuilder builder)
        where TValidator : IValidator<TRequest>, new()
    {
        var validator = new TValidator();
        return builder.WithValidation<TRequest, TValidator>(validator);
    }

    /// <summary>
    /// Adds a validation filter using the specified <typeparamref name="TValidator"/> instance to validate requests of type <typeparamref name="TRequest"/>.
    /// </summary>
    /// <typeparam name="TRequest">The type of requests to validate.</typeparam>
    /// <typeparam name="TValidator">The type of the validator.</typeparam>
    /// <param name="builder">The route handler builder to configure.</param>
    /// <param name="validator">The validator instance to use.</param>
    /// <returns>The configured <see cref="RouteHandlerBuilder"/> with validation filter.</returns>
    public static RouteHandlerBuilder WithValidation<TRequest, TValidator>(this RouteHandlerBuilder builder, TValidator validator)
        where TValidator : IValidator<TRequest>
    {
        return builder.AddEndpointFilter(async (context, next) =>
        {
            var request = context.Arguments.OfType<TRequest>().FirstOrDefault();
            if (request is null)
            {
                return await next(context);
            }

            var validationResult = validator.Validate(request);

            if (validationResult.Failed)
            {
                return validationResult.ToHttpResult();
            }

            return await next(context);
        });
    }
}
