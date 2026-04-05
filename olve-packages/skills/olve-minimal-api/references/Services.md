# ServiceExtensions and PathJsonConverter

API docs: [https://olivervea.github.io/Olve.Utilities/api/Olve.MinimalApi.ServiceExtensions.html](https://olivervea.github.io/Olve.Utilities/api/Olve.MinimalApi.ServiceExtensions.html)

Namespace: `Olve.MinimalApi`

## ServiceExtensions

Static class providing extension methods to configure Minimal API services.

### Public Members

| Member | Signature |
| --- | --- |
| `WithPathJsonConversion` | `static IServiceCollection WithPathJsonConversion(this IServiceCollection services)` |

### WithPathJsonConversion

Registers `PathJsonConverter` in the default `JsonSerializerOptions` via `ConfigureHttpJsonOptions`, enabling JSON round-trip serialization of `IPath` values in request/response models.

```csharp
builder.Services.WithPathJsonConversion();
```

Returns the `IServiceCollection` for chaining.

---

## PathJsonConverter

Sealed class that extends `JsonConverter<IPath>` for JSON serialization of `IPath` instances.

### Public Members

| Member | Signature |
| --- | --- |
| `Read` | `override IPath? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)` |
| `Write` | `override void Write(Utf8JsonWriter writer, IPath value, JsonSerializerOptions options)` |

### Behavior

- **Read**: Reads a JSON string and converts it to an `IPath` via `Path.Create(pathString)`. Returns `null` if the JSON value is null.
- **Write**: Writes the `IPath.Path` property as a JSON string value.
