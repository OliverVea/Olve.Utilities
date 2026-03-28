using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Olve.Results;

namespace Olve.MinimalApi;

/// <summary>
/// Provides extension methods to map <see cref="Result"/> and <see cref="Result{T}"/> to minimal API HTTP responses.
/// </summary>
public static class ResultMappingExtensions
{
    /// <summary>
    /// Configures the endpoint to produce success and error HTTP responses based on <see cref="Result"/>.
    /// </summary>
    /// <param name="builder">The route handler builder to configure.</param>
    /// <returns>The configured <see cref="RouteHandlerBuilder"/>.</returns>
    public static RouteHandlerBuilder WithResultMapping(this RouteHandlerBuilder builder)
    {
        builder.Produces(StatusCodes.Status200OK)
               .Produces<ResultProblem[]>(StatusCodes.Status400BadRequest);

        return AddResultFilter(builder);
    }

    /// <summary>
    /// Configures the endpoint to produce success and error HTTP responses based on <see cref="Result{TResult}"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="builder">The route handler builder to configure.</param>
    /// <returns>The configured <see cref="RouteHandlerBuilder"/>.</returns>
    public static RouteHandlerBuilder WithResultMapping<TResult>(this RouteHandlerBuilder builder)
    {
        builder.Produces<TResult>()
               .Produces<ResultProblem[]>(StatusCodes.Status400BadRequest);

        return AddResultFilter(builder);
    }

    private static RouteHandlerBuilder AddResultFilter(RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilterFactory((context, next) =>
        {
            var returnType = context.MethodInfo.ReturnType;
            if (typeof(Task).IsAssignableFrom(returnType) && returnType.IsGenericType)
            {
                returnType = returnType.GenericTypeArguments[0];
            }

            if (!typeof(IResultType).IsAssignableFrom(returnType))
            {
                return next;
            }

            return async invocationContext =>
            {
                var result = await next(invocationContext);

                if (result is not IResultType r)
                {
                    return result;
                }

                if (r.Failed)
                {
                    return TypedResults.BadRequest(r.Problems?.ToArray());
                }

                return r.HasValue
                    ? TypedResults.Ok(r.BoxedValue)
                    : TypedResults.Ok();
            };
        });
    }

    /// <summary>
    /// Converts a <see cref="Result"/> into an <see cref="IResult"/> HTTP response.
    /// </summary>
    /// <param name="result">The <see cref="Result"/> to convert.</param>
    /// <returns>An <see cref="IResult"/> representing the HTTP response, either OK or BadRequest with problems.</returns>
    public static IResult ToHttpResult(this Result result)
    {
        return result.TryPickProblems(out var problems)
            ? TypedResults.BadRequest(problems.ToArray())
            : TypedResults.Ok();
    }

    /// <summary>
    /// Converts a <see cref="Result{T}"/> into an <see cref="IResult"/> HTTP response.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The <see cref="Result{T}"/> to convert.</param>
    /// <returns>An <see cref="IResult"/> representing the HTTP response, either OK with value or BadRequest with problems.</returns>
    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        return result.TryPickProblems(out var problems, out var value)
            ? TypedResults.BadRequest(problems.ToArray())
            : TypedResults.Ok(value);
    }
}
