# Server-sent events

Generated from `src/typespec-csharp/test/fixtures/widgets.tsp` by the emitter's tests; do not edit.

An operation returning `SSEStream<Events>` of an `@events` union gets a response union whose `Ok` carries `IAsyncEnumerable<ServerSentEvent<Events>>`. The handler yields events; the runtime writes each as a frame named by the variant's `type`, with the item's `Id` as the SSE id (`null` = no id, for heartbeats). There is no session state: `Last-Event-ID` is an ordinary header parameter the handler reads, reconnecting is the client's, and the stream ends when the client disconnects or the app stops.

## Contract

```typespec
/** Keeps the connection alive. */
model Ping {
  type: "ping";
  at: utcDateTime;
}

// Carries the nested Shape union rather than a Widget: @typespec/openapi3 0.86 fails with
// duplicate-type-name when an event nests a model with properties invisible on read (Widget's
// update-only `note`).
model WidgetChanged {
  type: "widget.changed";
  at: utcDateTime;
  @format("uuid") widgetId: string;
  name: string;
  shape: Shape;
}

model WidgetRemoved {
  type: "widget.removed";
  at: utcDateTime;
  @format("uuid") widgetId: string;
}

/** The widget event stream. */
@TypeSpec.Events.events
union WidgetEvent {
  @TypeSpec.Events.contentType("application/json") ping: Ping,
  @TypeSpec.Events.contentType("application/json") `widget.changed`: WidgetChanged,
  @TypeSpec.Events.contentType("application/json") `widget.removed`: WidgetRemoved,
}

@route("/widget-events")
interface WidgetEvents {
  /** Streams widget events; `type` limits them to these event names. */
  @get stream(
    @query(#{ explode: false }) type?: string[],
    @header("Last-Event-ID") lastEventId?: string,
  ): SSEStream<WidgetEvent> | BadRequest;
}
```

## Generated

```csharp
/// <summary>The widget event stream.</summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(Ping), "ping")]
[JsonDerivedType(typeof(WidgetChanged), "widget.changed")]
[JsonDerivedType(typeof(WidgetRemoved), "widget.removed")]
public abstract record WidgetEvent : IServerSentEvent
{
    /// <summary>Every event name (<c>type</c>) of the stream, in contract order.</summary>
    public static IReadOnlyList<string> EventTypes { get; } = ["ping", "widget.changed", "widget.removed"];

    /// <inheritdoc />
    [JsonIgnore]
    public abstract string EventType { get; }
}

public sealed record WidgetChanged : WidgetEvent
{
    /// <inheritdoc />
    [JsonIgnore]
    public override string EventType => "widget.changed";

    [JsonPropertyName("at")]
    public required DateTimeOffset At { get; init; }

    [JsonPropertyName("widgetId")]
    public required Guid WidgetId { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("shape")]
    public required Shape Shape { get; init; }
}

/// <summary>The responses <c>WidgetEvents_stream</c> declares; its handler returns one.</summary>
public abstract record WidgetEventsStreamResponse : IOperationResponse
{
    private WidgetEventsStreamResponse()
    {
    }

    /// <inheritdoc />
    public abstract int Status { get; }

    /// <inheritdoc />
    public abstract IResult ToHttpResult();

    /// <summary>200.</summary>
    public sealed record Ok(IAsyncEnumerable<ServerSentEvent<WidgetEvent>> Events) : WidgetEventsStreamResponse
    {
        /// <inheritdoc />
        public override int Status => 200;

        /// <inheritdoc />
        public override IResult ToHttpResult() => new ServerSentEventsResult<WidgetEvent>(Events);
    }

    /// <summary>400.</summary>
    public sealed record BadRequest(FixtureErrorEnvelope Body) : WidgetEventsStreamResponse
    {
        /// <inheritdoc />
        public override int Status => 400;

        /// <inheritdoc />
        public override IResult ToHttpResult() => TypedResults.Json(Body, statusCode: 400);
    }
}
```

```csharp
WidgetEventsStream = app.MapGet("/api/widget-events", async (
        [FromServices] IWidgetEventsStreamHandler handler,
        [FromQuery(Name = "type")] string? type,
        [FromHeader(Name = "Last-Event-ID")] string? lastEventId,
        CancellationToken ct) =>
        (await handler.HandleAsync(new WidgetEventsStreamRequest(QueryParameters.List(type), lastEventId), ct)).ToHttpResult())
    .WithName("WidgetEvents_stream")
    .WithMetadata(FixtureOperations.WidgetEventsStream)
    .Produces<WidgetEvent>(200, "text/event-stream")
    .Produces<FixtureErrorEnvelope>(400),
```

## Hand-written handler

```csharp
/// <summary>
/// A finite stream: a ping (no id), then <c>widget.changed</c> (id 1) and <c>widget.removed</c>
/// (id 2). <c>type</c> keeps only the named events (an unknown name is a 400 before streaming);
/// <c>Last-Event-ID</c> skips ids up to it.
/// </summary>
private sealed class StreamWidgetEvents : IWidgetEventsStreamHandler
{
    public static readonly Guid WidgetId = Guid.Parse("0b6f7f0e-4a52-4d0e-9d7a-5b1f0f6c2a11");

    public Task<WidgetEventsStreamResponse> HandleAsync(WidgetEventsStreamRequest request, CancellationToken cancellationToken)
    {
        if (request.Type?.FirstOrDefault(t => !WidgetEvent.EventTypes.Contains(t)) is { } unknown)
        {
            return Task.FromResult<WidgetEventsStreamResponse>(
                new WidgetEventsStreamResponse.BadRequest(FixtureError.Create("UNKNOWN_EVENT", $"Unknown event type '{unknown}'.")));
        }

        var after = long.TryParse(request.LastEventId, out var id) ? id : 0;
        var events = Events()
            .Where(e => request.Type is null || request.Type.Contains(e.Data.EventType))
            .Where(e => e.Id is null || long.Parse(e.Id, System.Globalization.CultureInfo.InvariantCulture) > after);
        return Task.FromResult<WidgetEventsStreamResponse>(new WidgetEventsStreamResponse.Ok(events.ToAsyncEnumerable()));
    }

    private static IEnumerable<ServerSentEvent<WidgetEvent>> Events()
    {
        yield return new(new Ping { At = DateTimeOffset.UnixEpoch });
        yield return new(new WidgetChanged { At = DateTimeOffset.UnixEpoch, WidgetId = WidgetId, Name = "sample", Shape = new Square { Side = 2 } }, "1");
        yield return new(new WidgetRemoved { At = DateTimeOffset.UnixEpoch, WidgetId = WidgetId }, "2");
    }
}
```