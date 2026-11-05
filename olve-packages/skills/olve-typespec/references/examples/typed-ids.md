# Typed ids

Generated from `src/typespec-csharp/test/fixtures/widgets.tsp` by the emitter's tests; do not edit.

Opt-in: a uuid is `Guid` unless tagged. `@id(Model)` (from `import "@olivervea/typespec-csharp"`, `using Olve.CSharp;`) on a uuid-format string scalar makes every property and path/query parameter of that scalar Olve.Utilities' `Id<T>`, nullable and arrays included; `T` is the model's generated record, or the C# type given as the second argument (the app's own record). It can also tag one property or parameter, or be applied from a C#-only file with `@@id(Scalar, Model)`. The wire is unchanged: a uuid string, `format: uuid` in OpenAPI, so keep `@format("uuid")` on a base scalar. A value that isn't a uuid is a binding failure. `@id` on anything but a uuid-format string is an error; `explode: false` query lists of ids are reported as unsupported. The app references Olve.Utilities.

## Contract

```typespec
/** A uuid on the wire. */
@format("uuid")
scalar Uuid extends string;

/** A widget's id: `Id<Widget>` in C#, a uuid string on the wire. */
@id(Widget)
scalar WidgetId extends Uuid;

/** A widget's id typed as the app's own record (`Id<WidgetRecord>`). */
@id(Widget, "Olve.TypeSpec.Conformance.WidgetRecord")
scalar StoredWidgetId extends Uuid;

/** A link from one widget to others. */
model WidgetLink {
  from: WidgetId;
  to: WidgetId[];
  replaces?: WidgetId | null;
  stored?: StoredWidgetId;
  @minLength(1) label: string;
}

@route("/widget-links")
interface WidgetLinks {
  /** Links the widget in the path; `after` orders it after another link's widget. */
  @post create(@path from: WidgetId, @query after?: WidgetId, @body link: WidgetLink): WidgetLink | BadRequest;
}
```

## Generated

```csharp
/// <summary>A link from one widget to others.</summary>
public sealed record WidgetLink
{
    [JsonPropertyName("from")]
    public required Olve.Utilities.Ids.Id<Widget> From { get; init; }

    [JsonPropertyName("to")]
    public required IReadOnlyList<Olve.Utilities.Ids.Id<Widget>> To { get; init; }

    [JsonPropertyName("replaces")]
    public Olve.Utilities.Ids.Id<Widget>? Replaces { get; init; }

    [JsonPropertyName("stored"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Olve.Utilities.Ids.Id<Olve.TypeSpec.Conformance.WidgetRecord>? Stored { get; init; }

    [JsonPropertyName("label")]
    public required string Label { get; init; }
}

/// <summary>Input of <c>WidgetLinks_create</c> (POST /api/widget-links/{from}).</summary>
public sealed record WidgetLinksCreateRequest(Olve.Utilities.Ids.Id<Widget> From, Olve.Utilities.Ids.Id<Widget>? After, WidgetLink Body);
```

```csharp
WidgetLinksCreate = app.MapPost("/api/widget-links/{from}", async (
        [FromServices] IWidgetLinksCreateHandler handler,
        [FromRoute(Name = "from")] Olve.Utilities.Ids.Id<Widget> from,
        [FromQuery(Name = "after")] Olve.Utilities.Ids.Id<Widget>? after,
        [FromBody] WidgetLink body,
        CancellationToken ct) =>
        (await handler.HandleAsync(new WidgetLinksCreateRequest(from, after, body), ct)).ToHttpResult())
    .WithName("WidgetLinks_create")
    .WithMetadata(FixtureOperations.WidgetLinksCreate)
    .WithRequestValidation<WidgetLink, WidgetLinkValidator>(FixtureOperations.WidgetLinksCreate)
    .Produces<WidgetLink>(200)
    .Produces<FixtureErrorEnvelope>(400),
```

## Hand-written handler

Converting between id types is explicit: `new Id<WidgetRecord>(id.Value)`.

```csharp
/// <summary>Echoes the link from the path's widget; <c>after</c> goes first in <c>to</c>. Without a stored id, the path's id is it.</summary>
[OperationHandler]
internal sealed class CreateWidgetLink : IWidgetLinksCreateHandler
{
    public Task<WidgetLinksCreateResponse> HandleAsync(WidgetLinksCreateRequest request, CancellationToken cancellationToken)
    {
        var link = request.Body with
        {
            From = request.From,
            To = request.After is { } after ? [after, .. request.Body.To] : request.Body.To,
            Stored = request.Body.Stored ?? new Id<WidgetRecord>(request.From.Value),
        };
        return Task.FromResult<WidgetLinksCreateResponse>(link);
    }
}
```