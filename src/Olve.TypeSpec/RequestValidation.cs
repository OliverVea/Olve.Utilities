using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Olve.Validation;

namespace Olve.TypeSpec;

/// <summary>
/// Runs a generated request-body validator (from the spec's <c>@maxLength</c> etc.) before the
/// handler; a failure is a 400 written by the API's <see cref="IApiErrorWriter"/>, like a binding failure.
/// </summary>
public static class RequestValidation
{
    /// <summary>Validates the endpoint's <typeparamref name="TRequest"/> argument with a new <typeparamref name="TValidator"/>.</summary>
    public static RouteHandlerBuilder WithRequestValidation<TRequest, TValidator>(this RouteHandlerBuilder builder, ApiOperation operation)
        where TValidator : IValidator<TRequest>, new()
    {
        var validator = new TValidator();
        return builder.AddEndpointFilter(async (context, next) =>
        {
            if (FindArgument(context.Arguments) is { } request && validator.Validate(request).TryPickProblems(out var problems))
            {
                var errors = context.HttpContext.RequestServices.GetRequiredService<IApiErrorWriter>();
                return errors.InvalidRequest([.. problems], StatusCodes.Status400BadRequest, operation);
            }

            return await next(context);
        });

        // The body is one of a handful of arguments; a plain scan beats a LINQ enumerator per request.
        static TRequest? FindArgument(IList<object?> arguments)
        {
            for (var i = 0; i < arguments.Count; i++)
            {
                if (arguments[i] is TRequest request)
                {
                    return request;
                }
            }

            return default;
        }
    }
}
