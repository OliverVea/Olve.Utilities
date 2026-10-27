# Validation and the error body

Generated from `src/typespec-csharp/test/fixtures/widgets.tsp` by the emitter's tests; do not edit.

Constraints on a request body (`@minLength`, `@maxValue`, …) become an `IValidator<T>` that runs before the handler. Its failures, and request-binding failures, are answered by the app's `IApiErrorWriter`: the runtime never knows the error body. The contract's error model maps onto the app's type with `external-types`.

## Contract

```typespec
/** The error body (maps onto the app's own error type via external-types). */
model ErrorEnvelope {
  error: ErrorBody;
}

model ErrorBody {
  code: string;
  message: string;
  details: ErrorDetails;
}

model ErrorDetails {
  problems?: ErrorProblem[];
}

model ErrorProblem {
  code: string;
  message: string;
}

@error
model BadRequest {
  @statusCode status: 400;
  @body error: ErrorEnvelope;
}

@error
model NotFound {
  @statusCode status: 404;
  @body error: ErrorEnvelope;
}

@error
model Conflict {
  @statusCode status: 409;
  @body error: ErrorEnvelope;
}
```

## Generated validators

```csharp
/// <summary>Validates <see cref="WidgetCreate"/> against the spec's constraints.</summary>
public sealed class WidgetCreateValidator : IValidator<WidgetCreate>
{
    public Result Validate(WidgetCreate value) => Result.Concat(
    [
        value.Serial is null ? new ResultProblem("'serial' is required.") : Result.Success(),
        value.Name is null ? new ResultProblem("'name' is required.") : Result.Success(),
        value.Name is { Length: < 1 } ? new ResultProblem("'name' must be at least 1 character.") : Result.Success(),
        value.Name is { Length: > 50 } ? new ResultProblem("'name' cannot exceed 50 characters.") : Result.Success(),
        value.Tags is { Count: > 10 } ? new ResultProblem("'tags' cannot have more than 10 items.") : Result.Success(),
        value.Weight < 0 ? new ResultProblem("'weight' must be at least 0.") : Result.Success(),
        value.Weight > 100 ? new ResultProblem("'weight' cannot exceed 100.") : Result.Success(),
        value.Shape is null ? new ResultProblem("'shape' is required.") : Result.Success(),
    ]);
}

/// <summary>Validates <see cref="WidgetWritable"/> against the spec's constraints.</summary>
public sealed class WidgetWritableValidator : IValidator<WidgetWritable>
{
    public Result Validate(WidgetWritable value) => Result.Concat(
    [
        value.Serial is null ? new ResultProblem("'serial' is required.") : Result.Success(),
        value.Name is null ? new ResultProblem("'name' is required.") : Result.Success(),
        value.Name is { Length: < 1 } ? new ResultProblem("'name' must be at least 1 character.") : Result.Success(),
        value.Name is { Length: > 50 } ? new ResultProblem("'name' cannot exceed 50 characters.") : Result.Success(),
        value.Tags is { Count: > 10 } ? new ResultProblem("'tags' cannot have more than 10 items.") : Result.Success(),
        value.Weight < 0 ? new ResultProblem("'weight' must be at least 0.") : Result.Success(),
        value.Weight > 100 ? new ResultProblem("'weight' cannot exceed 100.") : Result.Success(),
        value.Shape is null ? new ResultProblem("'shape' is required.") : Result.Success(),
    ]);
}
```

## Hand-written error writer

```csharp
/// <summary>
/// Answers binding and validation failures with the envelope when the operation declares the
/// status, else with the status alone: the first problem is the message, every problem goes in
/// <c>details.problems</c> when there are several.
/// </summary>
public sealed class FixtureErrorWriter : IApiErrorWriter
{
    public IResult InvalidRequest(IReadOnlyList<ResultProblem> problems, int status, ApiOperation? operation)
    {
        if (operation?.Declares(status) != true)
        {
            return TypedResults.StatusCode(status);
        }

        var entries = problems.Select(p => new FixtureErrorProblem(FixtureErrors.InvalidRequest, p.FormattedMessage)).ToList();
        var error = new FixtureError(FixtureErrors.InvalidRequest, entries[0].Message, new FixtureErrorDetails { Problems = entries.Count > 1 ? entries : null });
        return TypedResults.Json(new FixtureErrorEnvelope(error), FixtureErrorJsonContext.Default.FixtureErrorEnvelope, statusCode: status);
    }
}
```