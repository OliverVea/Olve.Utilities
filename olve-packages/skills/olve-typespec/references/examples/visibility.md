# Visibility: request shapes

Generated from `src/typespec-csharp/test/fixtures/widgets.tsp` by the emitter's tests; do not edit.

A property's `@visibility` decides which shapes carry it. The response keeps everything; a request shape is named for what it may send: `<Model>Create` for a POST, `<Model>Writable` when Create and Update send the same properties, `<Model>Update` when they differ.

## Contract

```typespec
/** A widget. */
model Widget {
  @visibility(Lifecycle.Read) @format("uuid") id: string;

  /** Set once, at creation. */
  @visibility(Lifecycle.Create, Lifecycle.Read) serial: string;

  /** Only sent when updating. */
  @visibility(Lifecycle.Update) note?: string;

  @minLength(1) @maxLength(50) name: string;
  color?: Color;
  priority: Priority;
  description: string | null;
  parent?: string | null;
  @maxItems(10) tags?: string[];
  labels?: Record<string>;
  @minValue(0) @maxValue(100) weight?: int32;
  createdAt: utcDateTime;
  shape: Shape;
}
```

## Generated

```csharp
/// <summary>A widget.</summary>
public sealed record Widget
{
    [JsonPropertyName("id")]
    public required Guid Id { get; init; }

    /// <summary>Set once, at creation.</summary>
    [JsonPropertyName("serial")]
    public required string Serial { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("color"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Color? Color { get; init; }

    [JsonPropertyName("priority")]
    public required Priority Priority { get; init; }

    [JsonPropertyName("description")]
    public required string? Description { get; init; }

    [JsonPropertyName("parent")]
    public string? Parent { get; init; }

    [JsonPropertyName("tags"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<string>? Tags { get; init; }

    [JsonPropertyName("labels"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyDictionary<string, string>? Labels { get; init; }

    [JsonPropertyName("weight"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Weight { get; init; }

    [JsonPropertyName("createdAt")]
    public required DateTimeOffset CreatedAt { get; init; }

    [JsonPropertyName("shape")]
    public required Shape Shape { get; init; }
}

/// <summary>Request shape of <see cref="Widget"/> (only the properties a client may send).</summary>
public sealed record WidgetCreate
{
    /// <summary>Set once, at creation.</summary>
    [JsonPropertyName("serial")]
    public required string Serial { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("color"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Color? Color { get; init; }

    [JsonPropertyName("priority")]
    public required Priority Priority { get; init; }

    [JsonPropertyName("description")]
    public required string? Description { get; init; }

    [JsonPropertyName("parent")]
    public string? Parent { get; init; }

    [JsonPropertyName("tags"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<string>? Tags { get; init; }

    [JsonPropertyName("labels"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyDictionary<string, string>? Labels { get; init; }

    [JsonPropertyName("weight"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Weight { get; init; }

    [JsonPropertyName("createdAt")]
    public required DateTimeOffset CreatedAt { get; init; }

    [JsonPropertyName("shape")]
    public required Shape Shape { get; init; }
}

/// <summary>Request shape of <see cref="Widget"/> (only the properties a client may send).</summary>
public sealed record WidgetWritable
{
    /// <summary>Set once, at creation.</summary>
    [JsonPropertyName("serial")]
    public required string Serial { get; init; }

    /// <summary>Only sent when updating.</summary>
    [JsonPropertyName("note"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Note { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("color"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Color? Color { get; init; }

    [JsonPropertyName("priority")]
    public required Priority Priority { get; init; }

    [JsonPropertyName("description")]
    public required string? Description { get; init; }

    [JsonPropertyName("parent")]
    public string? Parent { get; init; }

    [JsonPropertyName("tags"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<string>? Tags { get; init; }

    [JsonPropertyName("labels"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyDictionary<string, string>? Labels { get; init; }

    [JsonPropertyName("weight"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Weight { get; init; }

    [JsonPropertyName("createdAt")]
    public required DateTimeOffset CreatedAt { get; init; }

    [JsonPropertyName("shape")]
    public required Shape Shape { get; init; }
}
```