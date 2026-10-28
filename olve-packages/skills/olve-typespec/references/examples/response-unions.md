# Response unions

Generated from `src/typespec-csharp/test/fixtures/widgets.tsp` by the emitter's tests; do not edit.

An operation's declared responses become one closed union with a variant per status. The handler returns a variant, so a status the contract doesn't declare can't compile. A success body no other variant carries converts implicitly; errors are always named.

## Contract

```typespec
@route("/widgets")
interface Widgets {
  /** List widgets. */
  @get list(
    @query color?: Color,
    @query limit?: int32,
    @header("x-request-id") requestId?: string,
  ): Widget[];

  /** Creates a widget now (201), or accepts it for later (202). */
  @post create(@body body: Widget):
    | (CreatedResponse & { @body body: Widget })
    | (AcceptedResponse & { @body body: Widget })
    | BadRequest;

  @put update(@path id: string, @query dryRun?: boolean, @body body: Widget):
    | Widget
    | BadRequest
    | NotFound
    | Conflict;

  @delete delete(@path id: string): NoContentResponse | NotFound;
}
```

## Generated: `Widgets_create` (201 | 202 | 400)

```csharp
/// <summary>Input of <c>Widgets_create</c> (POST /api/widgets).</summary>
public sealed record WidgetsCreateRequest(WidgetCreate Body);

/// <summary>The responses <c>Widgets_create</c> declares; its handler returns one.</summary>
public abstract record WidgetsCreateResponse : IOperationResponse
{
    private WidgetsCreateResponse()
    {
    }

    /// <inheritdoc />
    public abstract int Status { get; }

    /// <inheritdoc />
    public abstract IResult ToHttpResult();

    /// <summary>201.</summary>
    public sealed record Created(Widget Body) : WidgetsCreateResponse
    {
        /// <inheritdoc />
        public override int Status => 201;

        /// <inheritdoc />
        public override IResult ToHttpResult() => new JsonResponse<Widget>(Body, 201);
    }

    /// <summary>202.</summary>
    public sealed record Accepted(Widget Body) : WidgetsCreateResponse
    {
        /// <inheritdoc />
        public override int Status => 202;

        /// <inheritdoc />
        public override IResult ToHttpResult() => new JsonResponse<Widget>(Body, 202);
    }

    /// <summary>400.</summary>
    public sealed record BadRequest(FixtureErrorEnvelope Body) : WidgetsCreateResponse
    {
        /// <inheritdoc />
        public override int Status => 400;

        /// <inheritdoc />
        public override IResult ToHttpResult() => new JsonResponse<FixtureErrorEnvelope>(Body, 400);
    }
}

/// <summary>Creates a widget now (201), or accepts it for later (202).</summary>
public interface IWidgetsCreateHandler : IOperationHandler<WidgetsCreateRequest, WidgetsCreateResponse>;
```

## Generated: `Widgets_update` (200 | 400 | 404 | 409, implicit `Ok`)

```csharp
/// <summary>The responses <c>Widgets_update</c> declares; its handler returns one.</summary>
public abstract record WidgetsUpdateResponse : IOperationResponse
{
    private WidgetsUpdateResponse()
    {
    }

    /// <inheritdoc />
    public abstract int Status { get; }

    /// <inheritdoc />
    public abstract IResult ToHttpResult();

    /// <summary>200.</summary>
    public sealed record Ok(Widget Body) : WidgetsUpdateResponse
    {
        /// <inheritdoc />
        public override int Status => 200;

        /// <inheritdoc />
        public override IResult ToHttpResult() => new JsonResponse<Widget>(Body, 200);
    }

    /// <summary>400.</summary>
    public sealed record BadRequest(FixtureErrorEnvelope Body) : WidgetsUpdateResponse
    {
        /// <inheritdoc />
        public override int Status => 400;

        /// <inheritdoc />
        public override IResult ToHttpResult() => new JsonResponse<FixtureErrorEnvelope>(Body, 400);
    }

    /// <summary>404.</summary>
    public sealed record NotFound(FixtureErrorEnvelope Body) : WidgetsUpdateResponse
    {
        /// <inheritdoc />
        public override int Status => 404;

        /// <inheritdoc />
        public override IResult ToHttpResult() => new JsonResponse<FixtureErrorEnvelope>(Body, 404);
    }

    /// <summary>409.</summary>
    public sealed record Conflict(FixtureErrorEnvelope Body) : WidgetsUpdateResponse
    {
        /// <inheritdoc />
        public override int Status => 409;

        /// <inheritdoc />
        public override IResult ToHttpResult() => new JsonResponse<FixtureErrorEnvelope>(Body, 409);
    }

    public static implicit operator WidgetsUpdateResponse(Widget body) => new Ok(body);
}
```

## Hand-written handlers

The conformance suite's handlers answer magic ids so its tests can drive every variant; a real handler would look up the id.

```csharp
private sealed class CreateWidget : IWidgetsCreateHandler
{
    public Task<WidgetsCreateResponse> HandleAsync(WidgetsCreateRequest request, CancellationToken cancellationToken)
    {
        var body = request.Body;
        if (body.Serial == Invalid)
        {
            return Task.FromResult<WidgetsCreateResponse>(new WidgetsCreateResponse.BadRequest(Error("WIDGET_INVALID", body.Serial)));
        }

        var widget = new Widget
        {
            Id = Guid.NewGuid(),
            Serial = body.Serial,
            Name = body.Name,
            Color = body.Color,
            Priority = body.Priority,
            Description = body.Description,
            Parent = body.Parent,
            Tags = body.Tags,
            Labels = body.Labels,
            Weight = body.Weight,
            CreatedAt = body.CreatedAt,
            Shape = body.Shape,
        };
        return Task.FromResult<WidgetsCreateResponse>(body.Serial == Queued
            ? new WidgetsCreateResponse.Accepted(widget)
            : new WidgetsCreateResponse.Created(widget));
    }
}
```

```csharp
private sealed class UpdateWidget : IWidgetsUpdateHandler
{
    public Task<WidgetsUpdateResponse> HandleAsync(WidgetsUpdateRequest request, CancellationToken cancellationToken)
    {
        WidgetsUpdateResponse? failure = request.Id switch
        {
            Missing => new WidgetsUpdateResponse.NotFound(Error("WIDGET_NOT_FOUND", request.Id)),
            Conflict => new WidgetsUpdateResponse.Conflict(Error("WIDGET_LOCKED", request.Id)),
            Invalid => new WidgetsUpdateResponse.BadRequest(Error("WIDGET_INVALID", request.Id)),
            _ => null,
        };
        if (failure is not null)
        {
            return Task.FromResult(failure);
        }

        var body = request.Body;
        var labels = new Dictionary<string, string> { ["id"] = request.Id };
        if (request.DryRun is { } dryRun)
        {
            labels["dryRun"] = dryRun ? "true" : "false";
        }

        if (body.Note is { } note)
        {
            labels["note"] = note;
        }

        // A success body converts implicitly to its variant (Ok).
        return Task.FromResult<WidgetsUpdateResponse>(SampleWidget(labels) with
        {
            Serial = body.Serial,
            Name = body.Name,
            Description = body.Description,
            Shape = body.Shape,
        });
    }
}
```

```csharp
private sealed class DeleteWidget : IWidgetsDeleteHandler
{
    public Task<WidgetsDeleteResponse> HandleAsync(WidgetsDeleteRequest request, CancellationToken cancellationToken) =>
        Task.FromResult<WidgetsDeleteResponse>(request.Id == Missing
            ? new WidgetsDeleteResponse.NotFound(Error("WIDGET_NOT_FOUND", request.Id))
            : new WidgetsDeleteResponse.NoContent());
}
```